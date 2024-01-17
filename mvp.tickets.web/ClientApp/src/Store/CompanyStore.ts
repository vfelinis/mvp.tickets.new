import axios from 'axios';
import { observable, action, makeObservable } from 'mobx';
import { IBaseReportQueryRequest, IBaseReportQueryResponse, IBaseCommandResponse, IBaseQueryResponse } from '../Models/Base';
import { RootStore } from './RootStore';
import { ApiRoutesHelper } from '../Helpers/ApiRoutesHelper';
import { ICompanySetActiveCommandRequest, ICompanyCreateCommandRequest, ICompanyModel, ICompanyUpdateCommandRequest } from '../Models/Company';
import { browserHistory } from '..';
import { UIRoutesHelper } from '../Helpers/UIRoutesHelper';

export class CompanyStore {
    private rootStore: RootStore;
    isLoading: boolean;
    report: ICompanyModel[];
    company: ICompanyModel | null;

    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;
        this.isLoading = false;
        this.report = [];
        this.company = null;
        makeObservable(this, {
            isLoading: observable,
            report: observable,
            company: observable,
            setReport: action,
            getReport: action,
            create: action,
            setIsLoading: action,
            setCompany: action,
            getDataForUpdateForm: action,
            update: action,
        });
    }

    setIsLoading(isLoading: boolean) : void {
        this.isLoading = isLoading;
    }

    setReport(entries: ICompanyModel[]) : void {
        this.report = entries;
    }

    setCompany(entry: ICompanyModel | null) : void {
        this.company = entry;
    }

    getReport() : void {
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<ICompanyModel[]>>(ApiRoutesHelper.company.report)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setReport(response.data.data);

                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    create(request: ICompanyCreateCommandRequest) : void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<string>>(ApiRoutesHelper.company.create, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(response.data.data);
                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    setActive(request: ICompanySetActiveCommandRequest) : void {
        this.setIsLoading(true);
        axios.put<IBaseCommandResponse<boolean>>(ApiRoutesHelper.company.setActive, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    let entry = this.report.find(s => s.id === request.id);
                    if (entry) {
                        entry.isActive = request.isActive;
                        this.setReport([...this.report]);
                    }
                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    getDataForUpdateForm(id: number) : void {
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<ICompanyModel>>(ApiRoutesHelper.company.get(id))
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setCompany(response.data.data);

                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    update(request: ICompanyUpdateCommandRequest) : void {
        this.setIsLoading(true);
        axios.put<IBaseCommandResponse<boolean>>(ApiRoutesHelper.company.update(request.id), request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.home.getRoute());
                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }
}