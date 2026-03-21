using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSyncScheduler
{
    public class PullPaymentServiceModel
    {
        public string status { get; set; } 
        public string merchant_request_number { get; set; }
        public string mandate_id { get; set; } 
        public string mandate_transaction_id { get; set; }  
        public DateTime CreatedOn { get; set; }
        public string lead_id { get; set; } 
        public string user_id { get; set; }
        public string loan_id { get; set; }
        public string customer_name { get; set; }
        public string request_type { get; set; } = "PAYMENTCOLLECTION";
        public string response_description { get; set; }
        public decimal amount { get; set; }
        public bool is_pull_process { get; set; }
    }
}
