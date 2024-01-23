import axios from 'axios';
import { observable, action, makeObservable } from 'mobx';
import { IBaseReportQueryRequest, IBaseReportQueryResponse, IBaseCommandResponse, IBaseQueryResponse } from '../Models/Base';
import { RootStore } from './RootStore';
import { ApiRoutesHelper } from '../Helpers/ApiRoutesHelper';
import { IInviteCreateCommandRequest, IInviteModel, IInviteValidateCommandRequest } from '../Models/Invite';
import { browserHistory } from '..';
import { UIRoutesHelper } from '../Helpers/UIRoutesHelper';

export class InviteStore {
    private rootStore: RootStore;
    isLoading: boolean;
    isValid: boolean;
    report: IInviteModel[];

    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;
        this.isLoading = false;
        this.isValid = false;
        this.report = [];
        makeObservable(this, {
            isLoading: observable,
            isValid: observable,
            report: observable,
            setReport: action,
            getReport: action,
            create: action,
            delete: action,
            validate: action,
            setIsLoading: action,
            setIsValid: action,
        });
    }

    setIsLoading(isLoading: boolean) : void {
        this.isLoading = isLoading;
    }

    setIsValid(isValid: boolean) : void {
        this.isValid = isValid;
    }

    setReport(entries: IInviteModel[]) : void {
        this.report = entries;
    }

    getReport() : void {
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<IInviteModel[]>>(ApiRoutesHelper.invite.report)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setReport(response.data.data);

                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    create(request: IInviteCreateCommandRequest) : void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<number>>(ApiRoutesHelper.invite.create, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.invites.getRoute());
                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    delete(id: number) : void {
        this.setIsLoading(true);
        axios.delete<IBaseCommandResponse<boolean>>(ApiRoutesHelper.invite.delete(id))
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setReport(this.report.filter(s => s.id !== id));
                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    validate(request: IInviteValidateCommandRequest) : void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<boolean>>(ApiRoutesHelper.invite.validate, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setIsValid(true);
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