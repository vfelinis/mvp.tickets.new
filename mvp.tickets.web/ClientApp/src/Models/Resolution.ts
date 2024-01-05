export interface IResolutionModel {
    id: number,
    name: string,
    isActive: boolean,
    dateCreated: Date,
    dateModified: Date,
}

export interface IResolutionQueryRequest {
    id?: number | null,
    onlyActive: boolean,
}

export interface IResolutionCreateCommandRequest {
    name: string,
    isActive: boolean,
}

export interface IResolutionUpdateCommandRequest extends IResolutionCreateCommandRequest {
    id: number,
}