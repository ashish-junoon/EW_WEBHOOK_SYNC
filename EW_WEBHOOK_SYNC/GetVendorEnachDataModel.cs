using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSyncScheduler
{
    public class GetVendorEnachDataModel
    {
        public string vendor_code { get; set; }
        public string user_id { get; set; }
        public string lead_id { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string transaction_id { get; set; }
        public string mandate_id { get; set; }
        public string customer_name { get; set; }

    }
}
