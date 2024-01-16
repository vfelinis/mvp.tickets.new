export interface ICompanyModel {
    id: number,
    name: string,
    host: string,
    isActive: boolean,
    dateCreated: Date,
}

export interface ICompanyCreateCommandRequest
{
    name: string,
    host: string,
    email: string,
    password: string,
    code: string,
}

export interface ICompanySetActiveCommandRequest
{
    id: number,
    isActive: boolean,
}