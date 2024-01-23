using Confluent.Kafka;
using mvp.tickets.data;

namespace mvp.tickets.web.Kafka
{
    public class KafkaClientHandle : IDisposable
    {
        IProducer<byte[], byte[]> kafkaProducer;

        public KafkaClientHandle(IConnectionStrings connectionStrings)
        {
            var conf = new ProducerConfig
            {
                BootstrapServers = connectionStrings.KafkaConnection,
                Acks = Acks.All,
            };
            kafkaProducer = new ProducerBuilder<byte[], byte[]>(conf).Build();
        }

        public Handle Handle { get => kafkaProducer.Handle; }

        public void Dispose()
        {
            kafkaProducer.Flush();
            kafkaProducer.Dispose();
        }
    }
}
