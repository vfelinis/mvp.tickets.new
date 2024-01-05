namespace mvp.tickets.domain.Enums
{
    [Flags]
    public enum Permissions
    {
        None     = 0b000000000,
        User     = 0b000000001,
        Admin    = 0b000000010,
        Employee = 0b000000100
    }
}
