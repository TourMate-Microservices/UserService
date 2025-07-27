using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourMate.UserService.Repositories.ResponseModels
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public string CustomerAvatar { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public int InvoiceId { get; set; }
        public string Content { get; set; }
        public int ServiceId { get; set; }
    }
}
