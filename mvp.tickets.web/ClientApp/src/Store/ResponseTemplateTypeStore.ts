import axios from 'axios';
import { observable, action, makeObservable } from 'mobx';
import { IBaseCommandResponse, IBaseQueryResponse } from '../Models/Base';
import { RootStore } from './RootStore';
import { ApiRoutesHelper } from '../Helpers/ApiRoutesHelper';
import { IResponseTemplateTypeCreateCommandRequest, IResponseTemplateTypeModel, IResponseTemplateTypeQueryRequest, IResponseTemplateTypeUpdateCommandRequest } from '../Models/ResponseTemplateType';
import { UIRoutesHelper } from '../Helpers/UIRoutesHelper';
import { browserHistory } from '..';

export class ResponseTemplateTypeStore {
    private rootStore: RootStore;
    isLoading: boolean;
    entries: IResponseTemplateTypeModel[];
    entry: IResponseTemplateTypeModel | null;


    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;
        this.isLoading = false;
        this.entries = [];
        this.entry = null;
        makeObservable(this, {
            isLoading: observable,
            entries: observable,
            entry: observable,
            setIsLoading: action,
            setEntries: action,
            setEntry: action,
            getEntries: action,
            create: action,
            getDataForUpdateForm: action,
            update: action,
        });
    }

    setIsLoading(isLoading: boolean): void {
        this.isLoading = isLoading;
    }

    setEntries(entries: IResponseTemplateTypeModel[]): void {
        this.entries = entries;
    }

    setEntry(entry: IResponseTemplateTypeModel | null): void {
        this.entry = entry;
    }

    getEntries(onlyActive: boolean = false): void {
        const request: IResponseTemplateTypeQueryRequest = {
            onlyActive: onlyActive
        };
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<IResponseTemplateTypeModel[]>>(ApiRoutesHelper.responseTemplateType, { params: request })
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setEntries(response.data.data);

                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    create(request: IResponseTemplateTypeCreateCommandRequest): void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<number>>(ApiRoutesHelper.responseTemplateType, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.adminResponseTemplateTypes.getRoute());
                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    getDataForUpdateForm(id: number): void {
        const request: IResponseTemplateTypeQueryRequest = {
            id: id,
            onlyActive: false,
        };
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<IResponseTemplateTypeModel[]>>(ApiRoutesHelper.responseTemplateType, { params: request })
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setEntry(response.data.data[0]);
                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    update(request: IResponseTemplateTypeUpdateCommandRequest): void {
        this.setIsLoading(true);
        axios.put<IBaseCommandResponse<boolean>>(ApiRoutesHelper.responseTemplateType, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.adminResponseTemplateTypes.getRoute());
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
