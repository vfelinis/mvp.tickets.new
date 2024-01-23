import axios from 'axios';
import { observable, action, makeObservable } from 'mobx';
import { IBaseReportQueryRequest, IBaseReportQueryResponse, IBaseCommandResponse, IBaseQueryResponse } from '../Models/Base';
import { RootStore } from './RootStore';
import { ApiRoutesHelper } from '../Helpers/ApiRoutesHelper';
import { IUserAssigneeModel, IUserCreateCommandRequest, IUserForgotPasswordCommandRequest, IUserLoginByCodeCommandRequest, IUserLoginCommandRequest, IUserModel, IUserRegisterCommandRequest, IUserRegisterRequestCommandRequest, IUserResetPasswordCommandRequest, IUserUpdateCommandRequest } from '../Models/User';
import { browserHistory } from '..';
import { UIRoutesHelper } from '../Helpers/UIRoutesHelper';

export class UserStore {
    private rootStore: RootStore;
    isLoading: boolean;
    wasInit: boolean;
    currentUser: IUserModel | null;
    report: IUserModel[];
    assignees: IUserAssigneeModel[];
    total: number;
    editableUser: IUserModel | null;

    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;
        this.isLoading = false;
        this.wasInit = false;
        this.currentUser = null;
        this.report = [];
        this.assignees = [];
        this.total = 0;
        this.editableUser = null;
        makeObservable(this, {
            isLoading: observable,
            wasInit: observable,
            currentUser: observable,
            report: observable,
            assignees: observable,
            total: observable,
            editableUser: observable,
            login: action,
            loginByCode: action,
            logout: action,
            setCurrentUser: action,
            setReport: action,
            getReport: action,
            create: action,
            setEditableUser: action,
            getDataForUpdateForm: action,
            update: action,
            setIsLoading: action,
            setAssignees: action,
        });
    }

    setAssignees(assignees: IUserAssigneeModel[]) : void {
        this.assignees = assignees;
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
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    getAssignees() : void {
        this.setIsLoading(true);
        axios.get<IBaseQueryResponse<IUserAssigneeModel[]>>(ApiRoutesHelper.user.assignees)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setAssignees(response.data.data);

                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
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
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
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
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
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
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    login(request: IUserLoginCommandRequest): void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<IUserModel>>(ApiRoutesHelper.user.login, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setCurrentUser(response.data.data);

                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    loginByCode(request: IUserLoginByCodeCommandRequest): void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<IUserModel>>(ApiRoutesHelper.user.loginByCode, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setCurrentUser(response.data.data);

                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    logout(): void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<object>>(ApiRoutesHelper.user.logout)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    this.setCurrentUser(null);

                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    registerRequest(request: IUserRegisterRequestCommandRequest): void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<boolean>>(ApiRoutesHelper.user.registerRequest, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.emailConfirmation.getRoute());
                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    register(request: IUserRegisterCommandRequest): void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<boolean>>(ApiRoutesHelper.user.register, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.login.getRoute());
                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    forgotPassword(request: IUserForgotPasswordCommandRequest): void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<boolean>>(ApiRoutesHelper.user.forgotPassword, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.emailConfirmation.getRoute());
                } else {
                    this.rootStore.infoStore.setError(response.data.errorMessage ?? response.data.code.toString());
                }
            })
            .catch(error => {
                this.setIsLoading(false);
                this.rootStore.infoStore.setError(JSON.stringify(error));
            })
    }

    resetPassword(request: IUserResetPasswordCommandRequest): void {
        this.setIsLoading(true);
        axios.post<IBaseCommandResponse<boolean>>(ApiRoutesHelper.user.resetPassword, request)
            .then(response => {
                this.setIsLoading(false);
                if (response.data.isSuccess) {
                    browserHistory.push(UIRoutesHelper.login.getRoute());
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