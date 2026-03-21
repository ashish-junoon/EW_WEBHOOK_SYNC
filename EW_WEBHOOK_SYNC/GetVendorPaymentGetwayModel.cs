using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSyncScheduler
{
    public class GetVendorPaymentGetwayModel
    {
        public string mode { get; set; }
        public string lead_id { get; set; }
        public string loan_id { get; set; }
        public string product_info { get; set; }
        public string status { get; set; } 
        public DateTime paid_on { get; set; }
        public decimal amount { get; set; }
        public bool is_payment_proccess { get; set; }
        public string transaction_id { get; set; }
        public string easebuzz_id { get; set; }
        public string customer_name { get; set; }
        public string customer_email { get; set; }
        public string message { get; set; }

    }
}
