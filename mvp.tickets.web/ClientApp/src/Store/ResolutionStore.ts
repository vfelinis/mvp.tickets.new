import axios from 'axios';
import { observable, action, makeObservable } from 'mobx';
import { IBaseCommandResponse, IBaseQueryResponse } from '../Models/Base';
import { RootStore } from './RootStore';
import { ApiRoutesHelper } from '../Helpers/ApiRoutesHelper';
import { IResolutionCreateCommandRequest, IResolutionModel, IResolutionQueryRequest, IResolutionUpdateCommandRequest } from '../Models/Resolution';
import { UIRoutesHelper } from '../Helpers/UIRoutesHelper';
import { browserHistory } from '..';

export class ResolutionStore {
    private rootStore: RootStore;
    isLoading: boolean;
    entries: IResolutionModel[];
    entry: IResolutionModel | null;
    

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

    setEntries(entries: IResolutionModel[]) : void {
        this.entries = entries;
    }

    setEntry(entry: IResolutionModel | null) : void {
        this.entry = entry;
    }

    getEntries(onlyActive: boolean = false) : void {
        const request: IResolutionQueryRequest = {
            onlyActive: onlyActive
        };
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<IResolutionModel[]>>(ApiRoutesHelper.resolution, {params:request})
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

    create(request: IResolutionCreateCommandRequest) : void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<number>>(ApiRoutesHelper.resolution, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.adminResolutions.getRoute());
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
        const request: IResolutionQueryRequest = {
            id: id,
            onlyActive: false,
        };
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<IResolutionModel[]>>(ApiRoutesHelper.resolution, {params:request})
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

    update(request: IResolutionUpdateCommandRequest) : void {
        this.setIsLoading(true);
        axios.put<IBaseCommandResponse<boolean>>(ApiRoutesHelper.resolution, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.adminResolutions.getRoute());
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
