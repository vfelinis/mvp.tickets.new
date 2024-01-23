using mvp.tickets.data.Models;

namespace mvp.tickets.web.Kafka
{
    public class KafkaModels
    {
        public static readonly string _ticketsTopic = "tickets";

        public enum MessageType
        {
            NewTicket = 1,
            NewCommentFromUser = 2,
            NewCommentFromEmployee = 3
        }

        public class Message
        {
            public int CompanyId { get; set; }
            public int UserId { get; set; }
            public MessageType Type { get; set; }
            public Ticket Ticket { get; set; }
            public TicketComment Comment { get; set; }
        }
    }
}
