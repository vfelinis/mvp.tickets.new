import { Permissions } from '../Enums/Permissions';

export interface IUserModel {
    id: number,
    email: string,
    firstName: string,
    lastName: string,
    permissions: Permissions,
    isLocked: boolean,
    dateCreated: Date,
    dateModified: Date,
    companyId: number,
}

export interface IUserCreateCommandRequest
{
    email: string
    firstName: string
    lastName: string
    permissions: Permissions
    isLocked: boolean
    password: string
}

export interface IUserUpdateCommandRequest extends IUserCreateCommandRequest
{
    id: number
}

export interface IUserLoginCommandRequest
{
    email: string
    password: string
}

export interface IUserRegisterRequestCommandRequest
{
    email: string
}

export interface IUserRegisterCommandRequest
{
    firstName: string
    lastName: string
    password: string
    code: string
}

export interface IUserForgotPasswordCommandRequest
{
    email: string
}

export interface IUserResetPasswordCommandRequest
{
    password: string
    code: string
}

export interface IUserAssigneeModel
{
    id: number
    name: string
}