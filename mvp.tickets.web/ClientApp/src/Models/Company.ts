export interface ICompanyModel {
    id: number,
    name: string,
    host: string,
    isActive: boolean,
    isRoot: boolean,
    dateCreated: Date,
    logo: string | null,
    color: string,
}

export interface ICompanyCreateCommandRequest
{
    name: string,
    host: string,
    email: string,
    password: string,
    code: string,
    color: string,
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
    removeLogo: boolean,
    logo: string | null
}