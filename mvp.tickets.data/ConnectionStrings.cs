namespace mvp.tickets.data
{
    public interface IConnectionStrings
    {
        string DefaultConnection { get; set; }
        string RedisConnection { get; set; }
        string KafkaConnection { get; set; }
    }

    public record ConnectionStrings : IConnectionStrings
    {
        public string DefaultConnection { get; set; }
        public string RedisConnection { get; set; }
        public string KafkaConnection { get; set; }
    }
}
