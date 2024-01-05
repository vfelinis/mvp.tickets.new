import { ResponseCodes } from '../Enums/ResponseCodes';
import { SortDirection } from '../Enums/SortDirection';

export interface IBaseResponse {
    isSuccess: boolean,
    code: ResponseCodes,
    errorMessage: string,
}

export interface IBaseReportQueryRequest
{
    searchBy: object | null
    sortBy: string
    sortDirection: SortDirection
    offset: number
}

export interface IBaseCommandResponse<T> extends IBaseResponse {
    data: T
}

export interface IBaseQueryResponse<T> extends IBaseResponse {
    data: T
}

export interface IBaseReportQueryResponse<T> extends IBaseQueryResponse<T>
{
    total: number
}