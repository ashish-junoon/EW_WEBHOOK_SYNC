using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSyncScheduler
{
    public class PullPaymentService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void ProcessVendorPullPaymentData(List<PullPaymentServiceModel> vendorpullpaymentModels, string paisaudharConnStr, string masterConnStr)
        {
            try
            {
                foreach (var data in vendorpullpaymentModels)
                {
                    try
                    {
                        PullPaymentService.UpdateVendorPullPaymentData(data, paisaudharConnStr);
                        if (!string.IsNullOrWhiteSpace(data.loan_id) &&
                            !string.IsNullOrWhiteSpace(data.merchant_request_number) &&
                            !string.IsNullOrWhiteSpace(data.status))
                        {

                            bool isSuccess = data.status.Equals("success", StringComparison.OrdinalIgnoreCase) ||data.status.Equals("in_process", StringComparison.OrdinalIgnoreCase);
                            if (!isSuccess)
                            {
                                logger.Error("PullPayment Issues :" + data.loan_id);
                                ScheduledEmailService.SendEmail(data, "PULLPAYMENTFAILED");
                                logger.Info("PullPayment Failed for Transaction ID: " + data.loan_id + " | Email notification has been sent.");
                            }
                            else
                            {
                                logger.Error("PullPayment Success :" + data.loan_id);
                                ScheduledEmailService.SendEmail(data, "PULLPAYMENTSUCCESS");
                                logger.Info("PaymentGateWay Success for Transaction ID: " + data.loan_id + " | Email notification has been sent.");
                            }
                        }
                    }
                    catch (Exception innerEx)
                    {
                        logger.Error($"Error processing record LeadId={data.lead_id}: {innerEx.Message}");
                    }
                }
                if (vendorpullpaymentModels.Count > 0)
                {
                    try
                    {
                        MasterData.UpdateVendorPullPaymentData(vendorpullpaymentModels, masterConnStr);
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

        public static void UpdateVendorPullPaymentData(PullPaymentServiceModel data, string paisaudharConnStr)
        {
            try
            {
                using (var connection = new SqlConnection(paisaudharConnStr))
                {
                    SqlParameter[] param = new SqlParameter[]
                    {
                        new SqlParameter("@lead_id", data.lead_id ?? (object)DBNull.Value),
                        new SqlParameter("@loan_id", data.loan_id ?? (object)DBNull.Value),
                        new SqlParameter("@collection_status", data.status ?? (object)DBNull.Value),
                        new SqlParameter("@collection_date", data.CreatedOn),
                        new SqlParameter("@collection_amount", data.amount),
                        new SqlParameter("@transction_id", data.mandate_transaction_id ?? (object)DBNull.Value),
                        new SqlParameter("@merchant_request_number", data.merchant_request_number ?? (object)DBNull.Value),
                        new SqlParameter("@mandate_id", data.mandate_id ?? (object)DBNull.Value),
                        new SqlParameter("@user_id", data.user_id ?? (object)DBNull.Value),
                        new SqlParameter("@request_type", data.request_type ?? (object)DBNull.Value),
                        new SqlParameter("@response_description", data.response_description ?? (object)DBNull.Value),
                        new SqlParameter("@is_pull_process", data.is_pull_process)
                    };
                    helper_class.SqlHelper.ExecuteNonQuery(
                        paisaudharConnStr,
                        CommandType.StoredProcedure,
                        "USP_easebuzz_payment_getways_collection",
                        param
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Payment Gateway data: {ex.Message}");
            }
        }
    }
}
