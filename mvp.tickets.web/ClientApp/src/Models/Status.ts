export interface IStatusModel {
    id: number,
    name: string,
    isDefault: boolean,
    isCompletion: boolean,
    isActive: boolean,
    dateCreated: Date,
    dateModified: Date,
}

export interface IStatusQueryRequest {
    id?: number | null,
    onlyActive: boolean,
}

export interface IStatusCreateCommandRequest {
    name: string,
    isDefault: boolean,
    isCompletion: boolean,
    isActive: boolean,
}

export interface IStatusUpdateCommandRequest extends IStatusCreateCommandRequest {
    id: number,
}