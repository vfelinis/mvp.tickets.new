import axios from 'axios';
import { observable, action, makeObservable } from 'mobx';
import { IBaseCommandResponse, IBaseQueryResponse, IBaseReportQueryRequest, IBaseReportQueryResponse } from '../Models/Base';
import { RootStore } from './RootStore';
import { ApiRoutesHelper } from '../Helpers/ApiRoutesHelper';
import { ITicketModel, ITicketQueryRequest, ITicketUpdateCommandRequest } from '../Models/Ticket';
import { UIRoutesHelper } from '../Helpers/UIRoutesHelper';
import { browserHistory } from '..';

export class TicketStore {
    private rootStore: RootStore;
    isLoading: boolean;
    report: ITicketModel[];
    total: number;
    entry: ITicketModel | null;
    

    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;
        this.isLoading = false;
        this.report = [];
        this.total = 0;
        this.entry = null;
        makeObservable(this, {
            isLoading: observable,
            report: observable,
            total: observable,
            entry: observable,
            setIsLoading: action,
            setReport: action,
            getReport: action,
            getEntry: action,
            setEntry: action,
            create: action,
            createComment: action,
            update: action,
        });
    }

    setIsLoading(isLoading: boolean) : void {
        this.isLoading = isLoading;
    }

    setReport(report: ITicketModel[], total: number) : void {
        this.report = report;
        this.total = total;
    }

    setEntry(entry: ITicketModel | null) : void {
        this.entry = entry;
    }

    getReport(request: IBaseReportQueryRequest) : void {
        this.setIsLoading(true);
        axios.post<IBaseReportQueryResponse<ITicketModel[]>>(ApiRoutesHelper.ticket.report, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setReport(response.data.data, response.data.total);

                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    getEntry(id: number, isUserView: boolean, token: string|null = null) : void {
        const request: ITicketQueryRequest = {
            isUserView: isUserView,
            token: token
        }
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<ITicketModel>>(ApiRoutesHelper.ticket.get(id), {params: request})
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setEntry(response.data.data);

                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    create(request: FormData) : void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<number>>(ApiRoutesHelper.ticket.create, request, { headers: { "Content-Type": "multipart/form-data" } })
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.rootStore.infoStore.setMessage('Заявка будет создана в ближайшее время.');
                    browserHistory.push(UIRoutesHelper.tickets.getRoute());
                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    close(id: number, isUser: boolean) : void {
        this.setIsLoading(true);
        axios.put<IBaseCommandResponse<boolean>>(ApiRoutesHelper.ticket.close(id))
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setEntry(null);
                    if (isUser) {
                        browserHistory.push(UIRoutesHelper.tickets.getRoute());
                    }
                    else {
                        browserHistory.push(UIRoutesHelper.employee.getRoute());
                    }
                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    createComment(id: number, isUserView: boolean, request: FormData, token: string|null = null) : void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<number>>(ApiRoutesHelper.ticket.createComment(id), request, { params:{ token:token }, headers: { "Content-Type": "multipart/form-data" } })
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.rootStore.infoStore.setMessage('Комментарий будет создан в ближайшее время.');
                    browserHistory.push(
                        isUserView
                            ? token
                                ? UIRoutesHelper.ticketsDetailAlt.getRoute(id, token)
                                : UIRoutesHelper.ticketsDetail.getRoute(id)
                            : UIRoutesHelper.employeeTicketDetail.getRoute(id));
                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    update(request: ITicketUpdateCommandRequest) : void {
        this.setIsLoading(true);
        axios.put<IBaseCommandResponse<ITicketModel>>(ApiRoutesHelper.ticket.update(request.id), request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setEntry(response.data.data);
                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }
}
