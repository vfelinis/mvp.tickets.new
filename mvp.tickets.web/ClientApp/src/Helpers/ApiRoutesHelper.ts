export class ApiRoutesHelper {
    static user = {
        report: '/api/users/report/',
        create: '/api/users/',
        get: (id: number) : string => `/api/users/${id}/`,
        update: (id: number) : string => `/api/users/${id}/`,
        current: '/api/users/current/',
        login: '/api/users/login/',
        logout: '/api/users/logout/',
        
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
}