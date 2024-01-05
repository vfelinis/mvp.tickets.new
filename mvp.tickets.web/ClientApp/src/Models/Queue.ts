export interface IQueueModel {
    id: number,
    name: string,
    isDefault: boolean,
    isActive: boolean,
    dateCreated: Date,
    dateModified: Date,
}

export interface IQueueQueryRequest {
    id?: number | null,
    onlyActive: boolean,
}

export interface IQueueCreateCommandRequest {
    name: string,
    isActive: boolean,
    isDefault: boolean,
}

export interface IQueueUpdateCommandRequest extends IQueueCreateCommandRequest {
    id: number,
}