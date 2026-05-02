using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSyncScheduler
{
    public class PaymentGetwayService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void ProcessVendorPaymentData(List<GetVendorPaymentGetwayModel> vendorPaymentGetwayModels, string paisaudharConnStr, string masterConnStr)
        {
            try
            {
                foreach (var data in vendorPaymentGetwayModels)
                {
                    try
                    {
                        PaymentGetwayService.UpdateVendorPaymentData(data, paisaudharConnStr);
                        if (!string.IsNullOrWhiteSpace(data.loan_id) &&
                            !string.IsNullOrWhiteSpace(data.transaction_id) &&
                            !string.IsNullOrWhiteSpace(data.status))
                        {
                            bool isSuccess = data.status.Equals("success", StringComparison.OrdinalIgnoreCase);
                            if (!isSuccess)
                            {
                                logger.Error("PaymentGateWay Issues :" + data.transaction_id);
                                ScheduledEmailService.SendEmail(data, "PAYMENTGATEWAYFAILED");
                                logger.Info("PaymentGateWay Failed for Transaction ID: " + data.transaction_id + " | Email notification has been sent.");
                            }
                            else
                            {
                                logger.Error("PaymentGateWay Success :" + data.transaction_id);
                                ScheduledEmailService.SendEmail(data, "PAYMENTGATEWAYSUCCESS");
                                logger.Info("PaymentGateWay Success for Transaction ID: " + data.transaction_id + " | Email notification has been sent.");
                            }
                        }
                    }
                    catch (Exception innerEx)
                    {
                        logger.Error($"Error processing record LeadId={data.lead_id}: {innerEx.Message}");
                    }
                }
                if (vendorPaymentGetwayModels.Count > 0)
                {
                    try
                    {
                        MasterData.UpdateMasterPaymentData(vendorPaymentGetwayModels, masterConnStr);
                    }
                    catch (Exception insertEx)
                    {
                        logger.Error($"Error inserting into Master DB: {insertEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Unexpected error in ProcessVendorPaymentData: {ex.Message}");
            }
        }

        public static void UpdateVendorPaymentData(GetVendorPaymentGetwayModel data, string paisaudharConnStr)
        {
            try
            {
                using (var connection = new SqlConnection(paisaudharConnStr))
                {
                    SqlParameter[] param = new SqlParameter[]
                    {
                        new SqlParameter("@payment_mode", data.mode),
                        new SqlParameter("@lead_id", data.lead_id),
                        new SqlParameter("@loan_id", data.loan_id),
                        new SqlParameter("@collection_status", data.status),
                        new SqlParameter("@collection_date", data.paid_on.ToString("yyyy-MM-dd")),
                        //new SqlParameter("@collection_date", data.paid_on),
                        new SqlParameter("@collection_amount", data.amount),
                        new SqlParameter("@transction_id", data.transaction_id)
                    };
                    helper_class.SqlHelper.ExecuteNonQuery(paisaudharConnStr, CommandType.StoredProcedure, "USP_easebuzz_payment_getways_collection_NEW", param);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Payment Gateway data: {ex.Message}");
            }
        }


        //public static List<(string loan_id, string transaction_id, string is_payment_proccess)> UpdateVendorPaymentData(GetVendorPaymentGetwayModel data, string connectionString)
        //{
        //    var resultList = new List<(string loan_id, string transaction_id , string is_payment_proccess)>();
        //    try
        //    {
        //        using (var connection = new SqlConnection(connectionString))
        //        {
        //            SqlParameter[] param = new SqlParameter[]
        //            {
        //                new SqlParameter("@payment_mode", data.mode),
        //                new SqlParameter("@lead_id", data.lead_id),
        //                new SqlParameter("@loan_id", data.loan_id),
        //                new SqlParameter("@collection_status", data.status),
        //                new SqlParameter("@collection_date", data.paid_on),
        //                new SqlParameter("@collection_amount", data.amount),
        //                new SqlParameter("@transction_id", data.transaction_id)
        //            };
        //            using (SqlDataReader reader = LMS_DL.SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, "USP_easebuzz_payment_getways_collection", param))
        //            {
        //                while (reader.Read())
        //                {
        //                   string loanid = data.loan_id.ToString();
        //                   string transactionId = data.transaction_id.ToString();
        //                   string is_payment_proccess = data.is_payment_proccess.ToString();
        //                   resultList.Add((loanid, transactionId , is_payment_proccess));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error updating ENach data for : {ex.Message}");
        //    }
        //    return resultList;
        //}

    }
}
