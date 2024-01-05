export interface ITicketQueryRequest
{
    isUserView: boolean,
    token: string|null
}

export interface ITicketCreateCommandRequest
{
    name: string
    ticketCategoryId: number
    text?: string | null
}

export interface ITicketCommentCreateCommandRequest
{
    text?: string | null
    isInternal: boolean
}

export interface ITicketModel
{
    id: number
    name: string
    isClosed: boolean
    dateCreated: Date
    dateModified: Date
    source: string

    reporterId: number
    reporterEmail: string
    reporterFirstName: string
    reporterLastName: string

    assigneeId?: number | null
    assigneeEmail: string
    assigneeFirstName: string
    assigneeLastName: string

    ticketPriorityId?: number | null
    ticketPriority: string

    ticketQueueId: number
    ticketQueue: string

    ticketResolutionId?: number | null
    ticketResolution: string

    ticketStatusId: number
    ticketStatus: string

    ticketCategoryId: number
    ticketCategory: string

    ticketComments: ITicketCommentModel[]
}

export interface ITicketCommentModel
{
    id: number
    text: string
    isInternal: boolean
    dateCreated: Date
    dateModified: Date
    creatorId: number
    creatorEmail: string
    creatorFirstName: string
    creatorLastName: string
    ticketCommentAttachmentModels: ITicketCommentAttachmentModel[]
}

export interface ITicketCommentAttachmentModel
{
    id: number
    path: string
    originalFileName: string
    dateCreated: Date
}