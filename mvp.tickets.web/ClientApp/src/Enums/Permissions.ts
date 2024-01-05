export enum Permissions
{
    None     = 0,
    User     = 1 << 0,
    Admin    = 1 << 1,
    Employee = 1 << 2,
}

export function hasPermission(source: Permissions, permission: Permissions) : boolean {
    return permission === (source & permission);
}