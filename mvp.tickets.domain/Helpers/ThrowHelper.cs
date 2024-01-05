namespace mvp.tickets.domain.Helpers
{
    public static class ThrowHelper
    {
        public static T ArgumentNull<T>(string name = null)
        {
            throw new ArgumentNullException(name ?? typeof(T).ToString());
        }

        public static void ArgumentNull(string name)
        {
            throw new ArgumentNullException(name);
        }
    }
}
