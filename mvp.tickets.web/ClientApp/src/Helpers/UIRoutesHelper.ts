export class UIRoutesHelper {
    static home: IRoute = {
        path: '/',
        getRoute: () => `${this.home.path}`,
    };


    static tickets: IRoute = {
        path: 'tickets',
        getRoute: () => `/${this.tickets.path}/`,
    };
    static ticketsCreate: IRoute = {
        path: 'tickets/create',
        getRoute: () => `/${this.ticketsCreate.path}/`,
    };
    static ticketsDetail: IRoute = {
        path: 'tickets/:id',
        getRoute: (id: number) => `/${this.ticketsDetail.path.replace(":id", id.toString())}/`,
    };
    static ticketsDetailAlt: IRoute = {
        path: 'tickets/:id/alt',
        getRoute: (id: number, token: string) => `/${this.ticketsDetailAlt.path.replace(":id", id.toString())}/?token=${token}`,
    };
    static ticketsCommentCreate: IRoute = {
        path: 'tickets/:id/comments/create',
        getRoute: (id: number) => `/${this.ticketsCommentCreate.path.replace(":id", id.toString())}/`,
    };
    static ticketsCommentCreateAlt: IRoute = {
        path: 'tickets/:id/comments/create/alt',
        getRoute: (id: number, token: string) => `/${this.ticketsCommentCreateAlt.path.replace(":id", id.toString())}/?token=${token}`,
    };


    static adminUsers: IRoute = {
        path: 'admin/users',
        getRoute: () => `/${this.adminUsers.path}/`,
    };
    static adminUsersCreate: IRoute = {
        path: 'admin/users/create',
        getRoute: () => `/${this.adminUsersCreate.path}/`,
    };
    static adminUsersUpdate: IRoute = {
        path: 'admin/users/:id',
        getRoute: (id: number) => `/${this.adminUsersUpdate.path.replace(":id", id.toString())}/`,
    };


    static adminCategories: IRoute = {
        path: 'admin/categories',
        getRoute: () => `/${this.adminCategories.path}/`,
    };
    static adminCategoriesCreate: IRoute = {
        path: 'admin/categories/create',
        getRoute: () => `/${this.adminCategoriesCreate.path}/`,
    };
    static adminCategoriesUpdate: IRoute = {
        path: 'admin/categories/:id',
        getRoute: (id: number) => `/${this.adminCategoriesUpdate.path.replace(":id", id.toString())}/`,
    };


    static adminPriorities: IRoute = {
        path: 'admin/priorities',
        getRoute: () => `/${this.adminPriorities.path}/`,
    };
    static adminPrioritiesCreate: IRoute = {
        path: 'admin/priorities/create',
        getRoute: () => `/${this.adminPrioritiesCreate.path}/`,
    };
    static adminPrioritiesUpdate: IRoute = {
        path: 'admin/priorities/:id',
        getRoute: (id: number) => `/${this.adminPrioritiesUpdate.path.replace(":id", id.toString())}/`,
    };


    static adminQueues: IRoute = {
        path: 'admin/queues',
        getRoute: () => `/${this.adminQueues.path}/`,
    };
    static adminQueuesCreate: IRoute = {
        path: 'admin/queues/create',
        getRoute: () => `/${this.adminQueuesCreate.path}/`,
    };
    static adminQueuesUpdate: IRoute = {
        path: 'admin/queues/:id',
        getRoute: (id: number) => `/${this.adminQueuesUpdate.path.replace(":id", id.toString())}/`,
    };


    // static adminStatusRules: IRoute = {
    //     path: 'admin/statusrules',
    //     getRoute: () => `/${this.adminStatusRules.path}/`,
    // };
    // static adminStatusRulesCreate: IRoute = {
    //     path: 'admin/statusrules/create',
    //     getRoute: () => `/${this.adminStatusRulesCreate.path}/`,
    // };
    // static adminStatusRulesUpdate: IRoute = {
    //     path: 'admin/statusrules/:id',
    //     getRoute: (id: number) => `/${this.adminStatusRulesUpdate.path.replace(":id", id.toString())}/`,
    // };


    static adminStatuses: IRoute = {
        path: 'admin/statuses',
        getRoute: () => `/${this.adminStatuses.path}/`,
    };
    static adminStatusesCreate: IRoute = {
        path: 'admin/statuses/create',
        getRoute: () => `/${this.adminStatusesCreate.path}/`,
    };
    static adminStatusesUpdate: IRoute = {
        path: 'admin/statuses/:id',
        getRoute: (id: number) => `/${this.adminStatusesUpdate.path.replace(":id", id.toString())}/`,
    };


    static adminResolutions: IRoute = {
        path: 'admin/resolutions',
        getRoute: () => `/${this.adminResolutions.path}/`,
    };
    static adminResolutionsCreate: IRoute = {
        path: 'admin/resolutions/create',
        getRoute: () => `/${this.adminResolutionsCreate.path}/`,
    };
    static adminResolutionsUpdate: IRoute = {
        path: 'admin/resolutions/:id',
        getRoute: (id: number) => `/${this.adminResolutionsUpdate.path.replace(":id", id.toString())}/`,
    };


    static adminResponseTemplateTypes: IRoute = {
        path: 'admin/responseTemplateTypes',
        getRoute: () => `/${this.adminResponseTemplateTypes.path}/`,
    };
    static adminResponseTemplateTypesCreate: IRoute = {
        path: 'admin/responseTemplateTypes/create',
        getRoute: () => `/${this.adminResponseTemplateTypesCreate.path}/`,
    };
    static adminResponseTemplateTypesUpdate: IRoute = {
        path: 'admin/responseTemplateTypes/:id',
        getRoute: (id: number) => `/${this.adminResponseTemplateTypesUpdate.path.replace(":id", id.toString())}/`,
    };


    static adminResponseTemplates: IRoute = {
        path: 'admin/responseTemplates',
        getRoute: () => `/${this.adminResponseTemplates.path}/`,
    };
    static adminResponseTemplatesCreate: IRoute = {
        path: 'admin/responseTemplates/create',
        getRoute: () => `/${this.adminResponseTemplatesCreate.path}/`,
    };
    static adminResponseTemplatesUpdate: IRoute = {
        path: 'admin/responseTemplates/:id',
        getRoute: (id: number) => `/${this.adminResponseTemplatesUpdate.path.replace(":id", id.toString())}/`,
    };


    static employee: IRoute = {
        path: 'employee',
        getRoute: () => `/${this.employee.path}/`,
    };
    static employeeTicketDetail: IRoute = {
        path: 'employeeTicketDetail/:id',
        getRoute: (id: number) => `/${this.employeeTicketDetail.path.replace(":id", id.toString())}/`,
    };
    static employeeTicketCommentCreate: IRoute = {
        path: 'employeeTicketDetail/:id/comments/create',
        getRoute: (id: number) => `/${this.employeeTicketCommentCreate.path.replace(":id", id.toString())}/`,
    };


    static login: IRoute = {
        path: 'login',
        getRoute: () => `/${this.login.path}/`,
    };

    
    static notFound: IRoute = {
        path: '404',
        getRoute: () => `/${this.notFound.path}/`,
    };
}

export interface IRoute {
    path: string,
    getRoute: Function
}