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
    isRootCompany: boolean,
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