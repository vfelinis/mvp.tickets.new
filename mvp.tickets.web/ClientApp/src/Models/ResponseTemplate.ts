export interface IResponseTemplateModel {
    id: number,
    name: string,
    text: string,
    isActive: boolean,
    dateCreated: Date,
    dateModified: Date,
    ticketResponseTemplateTypeId: number,
    ticketResponseTemplateType: string,
}

export interface IResponseTemplateQueryRequest {
    id?: number | null,
    onlyActive: boolean,
}

export interface IResponseTemplateCreateCommandRequest {
    name: string,
    text: string,
    isActive: boolean,
    ticketResponseTemplateTypeId: number,
}

export interface IResponseTemplateUpdateCommandRequest extends IResponseTemplateCreateCommandRequest {
    id: number,
}