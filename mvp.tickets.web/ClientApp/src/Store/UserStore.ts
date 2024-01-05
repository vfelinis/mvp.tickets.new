import axios from 'axios';
import { observable, action, makeObservable } from 'mobx';
import { IBaseReportQueryRequest, IBaseReportQueryResponse, IBaseCommandResponse, IBaseQueryResponse } from '../Models/Base';
import { RootStore } from './RootStore';
import { ApiRoutesHelper } from '../Helpers/ApiRoutesHelper';
import { IUserCreateCommandRequest, IUserModel, IUserUpdateCommandRequest } from '../Models/User';
import { browserHistory } from '..';
import { UIRoutesHelper } from '../Helpers/UIRoutesHelper';

export class UserStore {
    private rootStore: RootStore;
    isLoading: boolean;
    wasInit: boolean;
    currentUser: IUserModel | null;
    report: IUserModel[];
    total: number;
    editableUser: IUserModel | null;

    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;
        this.isLoading = false;
        this.wasInit = false;
        this.currentUser = null;
        this.report = [];
        this.total = 0;
        this.editableUser = null;
        makeObservable(this, {
            isLoading: observable,
            wasInit: observable,
            currentUser: observable,
            report: observable,
            total: observable,
            editableUser: observable,
            login: action,
            logout: action,
            setCurrentUser: action,
            setReport: action,
            getReport: action,
            create: action,
            setEditableUser: action,
            getDataForUpdateForm: action,
            update: action,
            setIsLoading: action,
        });
    }

    setIsLoading(isLoading: boolean) : void {
        this.isLoading = isLoading;
    }

    setCurrentUser(user: IUserModel | null): void {
        this.currentUser = user;
        this.wasInit = true;
    }

    setReport(users: IUserModel[], total: number) : void {
        this.report = users;
        this.total = total;
    }

    setEditableUser(user: IUserModel | null): void {
        this.editableUser = user;
    }

    getReport(request: IBaseReportQueryRequest) : void {
        this.setIsLoading(true);
        axios.post<IBaseReportQueryResponse<IUserModel[]>>(ApiRoutesHelper.user.report, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setReport(response.data.data, response.data.total);

                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    create(request: IUserCreateCommandRequest) : void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<number>>(ApiRoutesHelper.user.create, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.adminUsers.getRoute());
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
        axios.get<IBaseQueryResponse<IUserModel>>(ApiRoutesHelper.user.get(id))
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setEditableUser(response.data.data);
                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    update(request: IUserUpdateCommandRequest) : void {
        this.setIsLoading(true);
        axios.put<IBaseCommandResponse<boolean>>(ApiRoutesHelper.user.update(request.id), request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.adminUsers.getRoute());
                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    login(idToken: string): void {
        axios.post<IBaseCommandResponse<IUserModel>>(ApiRoutesHelper.user.login, { idToken: idToken })
            .then(response => {
                if (response.data.isSuccess) {
                    this.setCurrentUser(response.data.data);

                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }

    logout(): void {
        axios.post<IBaseCommandResponse<object>>(ApiRoutesHelper.user.logout)
            .then(response => {
                if (response.data.isSuccess) {
                    this.setCurrentUser(null);

                } else {
                    this.rootStore.errorStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.rootStore.errorStore.setError(JSON.stringify(error));
            })
    }
}