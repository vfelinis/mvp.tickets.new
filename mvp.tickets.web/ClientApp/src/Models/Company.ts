export interface ICompanyModel {
    id: number,
    name: string,
    host: string,
    isActive: boolean,
    isRoot: boolean,
    dateCreated: Date,
    logo: string | null,
    color: string,
    authType: AuthTypes
}

export interface ICompanyCreateCommandRequest
{
    name: string,
    host: string,
    email: string,
    password: string,
    code: string,
    color: string,
    authType: AuthTypes | null
}

export interface ICompanySetActiveCommandRequest
{
    id: number,
    isActive: boolean,
}

export interface ICompanyUpdateCommandRequest
{
    id: number,
    name: string,
    host: string,
    color: string,
    logo: string | null,
    removeLogo: boolean,
    authType: AuthTypes | null
}

export enum AuthTypes
{
    Standard = 1,
    WithoutRegister = 2
}