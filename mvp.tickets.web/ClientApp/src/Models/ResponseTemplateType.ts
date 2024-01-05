export interface IResponseTemplateTypeModel {
    id: number,
    name: string,
    isActive: boolean,
    dateCreated: Date,
    dateModified: Date,
}

export interface IResponseTemplateTypeQueryRequest {
    id?: number | null,
    onlyActive: boolean,
}

export interface IResponseTemplateTypeCreateCommandRequest {
    name: string,
    isActive: boolean,
}

export interface IResponseTemplateTypeUpdateCommandRequest extends IResponseTemplateTypeCreateCommandRequest {
    id: number,
}