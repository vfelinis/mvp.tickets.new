import axios from 'axios';
import { observable, action, makeObservable } from 'mobx';
import { IBaseCommandResponse, IBaseQueryResponse } from '../Models/Base';
import { RootStore } from './RootStore';
import { ApiRoutesHelper } from '../Helpers/ApiRoutesHelper';
import { IPriorityCreateCommandRequest, IPriorityModel, IPriorityQueryRequest, IPriorityUpdateCommandRequest } from '../Models/Priority';
import { UIRoutesHelper } from '../Helpers/UIRoutesHelper';
import { browserHistory } from '..';

export class PriorityStore {
    private rootStore: RootStore;
    isLoading: boolean;
    entries: IPriorityModel[];
    entry: IPriorityModel | null;
    

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

    setIsLoading(isLoading: boolean) : void {
        this.isLoading = isLoading;
    }

    setEntries(entries: IPriorityModel[]) : void {
        this.entries = entries;
    }

    setEntry(entry: IPriorityModel | null) : void {
        this.entry = entry;
    }

    getEntries(onlyActive: boolean = false) : void {
        const request: IPriorityQueryRequest = {
            onlyActive: onlyActive
        };
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<IPriorityModel[]>>(ApiRoutesHelper.priority, {params:request})
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

    create(request: IPriorityCreateCommandRequest) : void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<number>>(ApiRoutesHelper.priority, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.adminPriorities.getRoute());
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
        const request: IPriorityQueryRequest = {
            id: id,
            onlyActive: false,
        };
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<IPriorityModel[]>>(ApiRoutesHelper.priority, {params:request})
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

    update(request: IPriorityUpdateCommandRequest) : void {
        this.setIsLoading(true);
        axios.put<IBaseCommandResponse<boolean>>(ApiRoutesHelper.priority, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.adminPriorities.getRoute());
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
