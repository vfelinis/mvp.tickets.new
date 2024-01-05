export interface IPriorityModel {
    id: number,
    name: string,
    level: number,
    isActive: boolean,
    dateCreated: Date,
    dateModified: Date,
}

export interface IPriorityQueryRequest {
    id?: number | null,
    onlyActive: boolean,
}

export interface IPriorityCreateCommandRequest {
    name: string,
    isActive: boolean,
    level: number
}

export interface IPriorityUpdateCommandRequest extends IPriorityCreateCommandRequest {
    id: number,
}