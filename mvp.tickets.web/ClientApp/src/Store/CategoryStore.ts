import axios from 'axios';
import { observable, action, makeObservable } from 'mobx';
import { IBaseCommandResponse, IBaseQueryResponse } from '../Models/Base';
import { RootStore } from './RootStore';
import { ApiRoutesHelper } from '../Helpers/ApiRoutesHelper';
import { ICategoryCreateCommandRequest, ICategoryModel, ICategoryQueryRequest, ICategoryUpdateCommandRequest } from '../Models/Category';
import { UIRoutesHelper } from '../Helpers/UIRoutesHelper';
import { browserHistory } from '..';

export class CategoryStore {
    private rootStore: RootStore;
    isLoading: boolean;
    categories: ICategoryModel[];
    category: ICategoryModel | null;
    

    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;
        this.isLoading = false;
        this.categories = [];
        this.category = null;
        makeObservable(this, {
            isLoading: observable,
            categories: observable,
            category: observable,
            setIsLoading: action,
            setCategories: action,
            setCategory: action,
            getCategories: action,
            getDataForCreateForm: action,
            create: action,
            getDataForUpdateForm: action,
            update: action,
        });
    }

    setIsLoading(isLoading: boolean) : void {
        this.isLoading = isLoading;
    }

    setCategories(categories: ICategoryModel[]) : void {
        this.categories = categories;
    }

    setCategory(category: ICategoryModel | null) : void {
        this.category = category;
    }

    getCategories(onlyActive: boolean = false) : void {
        const request: ICategoryQueryRequest = {
            onlyActive: onlyActive,
            onlyRoot: false
        };
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<ICategoryModel[]>>(ApiRoutesHelper.category, {params:request})
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setCategories(response.data.data);

                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    getDataForCreateForm() : void {
        const request: ICategoryQueryRequest = {
            onlyActive: true,
            onlyRoot: true
        };
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<ICategoryModel[]>>(ApiRoutesHelper.category, {params:request})
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setCategories(response.data.data);

                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    create(request: ICategoryCreateCommandRequest) : void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<number>>(ApiRoutesHelper.category, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.adminCategories.getRoute());
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
        const request: ICategoryQueryRequest = {
            id: id,
            onlyActive: false,
            onlyRoot: false
        };
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<ICategoryModel[]>>(ApiRoutesHelper.category, {params:request})
            .then(response => {
                if (response.data.isSuccess) {
                    this.setCategory(response.data.data[0]);
                    this.getDataForCreateForm();
                } else {
                    this.setIsLoading(false);
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    update(request: ICategoryUpdateCommandRequest) : void {
        this.setIsLoading(true);
        axios.put<IBaseCommandResponse<boolean>>(ApiRoutesHelper.category, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.adminCategories.getRoute());
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
