export class ApiRoutesHelper {
    static user = {
        report: '/api/users/report/',
        create: '/api/users/',
        get: (id: number) : string => `/api/users/${id}/`,
        update: (id: number) : string => `/api/users/${id}/`,
        current: '/api/users/current/',
        login: '/api/users/login/',
        logout: '/api/users/logout/',
        registerRequest: '/api/users/registerRequest/',
        register: '/api/users/register/',
        forgotPassword: '/api/users/forgotPassword/',
        resetPassword: '/api/users/resetPassword/',
        
    };

    static category = '/api/categories/';
    static priority = '/api/priorities/';
    static queue = '/api/queues/';
    static resolution = '/api/resolutions/';
    static responseTemplate = '/api/responseTemplates/';
    static responseTemplateType = '/api/responseTemplateTypes/';
    static status = '/api/statuses/';
    static ticket = {
        report: '/api/tickets/report/',
        create: '/api/tickets/',
        get: (id: number) : string => `/api/tickets/${id}/`,
        createComment: (id: number) : string => `/api/tickets/${id}/comments/`,
    };

    static invite = {
        report: '/api/invites/',
        create: '/api/invites/',
        delete: (id: number) : string => `/api/invites/${id}/`,
        validate: '/api/invites/validation/',
    };

    static company = {
        report: '/api/companies/',
        create: '/api/companies/',
        current: '/api/companies/current/',
        setActive: '/api/companies/activation/',
        get: (id: number) : string => `/api/companies/${id}/`,
        update: (id: number) : string => `/api/companies/${id}/`,
    };
}