export interface IInviteModel {
    id: number,
    email: string,
    company: string,
    dateSent: Date,
}

export interface IInviteCreateCommandRequest
{
    email: string,
    company: string,
}

export interface IInviteValidateCommandRequest
{
    email: string,
    code: string,
}