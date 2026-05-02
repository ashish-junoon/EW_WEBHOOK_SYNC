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

    public class MasterData
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static List<GetVendorEnachDataModel> GetVendorEnachData(string masterConnStr)
        {
            List<GetVendorEnachDataModel> getVendorEnachDataModels = new List<GetVendorEnachDataModel>();
            DataSet objDs = null;
            try
            {
                using (var connection = new SqlConnection(masterConnStr))
                {
                    SqlParameter[] param = new SqlParameter[0]; 

                    objDs = helper_class.SqlHelper.ExecuteDataset(masterConnStr, CommandType.StoredProcedure, "USP_EW_GetEasebuzz_ENachdetails", param);
                }

                if (objDs?.Tables[0] != null && objDs.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in objDs.Tables[0].Rows)
                    {
                        GetVendorEnachDataModel information = new GetVendorEnachDataModel
                        {
                            vendor_code = row["vendor_code"] != DBNull.Value ? Convert.ToString(row["vendor_code"]) : string.Empty,
                            user_id = row["user_id"] != DBNull.Value ? Convert.ToString(row["user_id"]) : string.Empty,
                            lead_id = row["lead_id"] != DBNull.Value ? Convert.ToString(row["lead_id"]) : string.Empty,
                            status = row["status"] != DBNull.Value ? Convert.ToString(row["status"]) : string.Empty,
                            message = row["message"] != DBNull.Value ? Convert.ToString(row["message"]) : string.Empty,
                            transaction_id = row["transaction_id"] != DBNull.Value ? Convert.ToString(row["transaction_id"]) : string.Empty,
                            mandate_id = row["emandate_id"] != DBNull.Value ? Convert.ToString(row["emandate_id"]) : string.Empty,
                            customer_name = row["customer_name"] != DBNull.Value ? Convert.ToString(row["customer_name"]) : string.Empty,
                        };
                        getVendorEnachDataModels.Add(information);
                    }
                }
            }
            catch (Exception ex)
            {
                //logger.Error($"Error fetching incomplete data: {ex.Message}");
            }
            return getVendorEnachDataModels;
        }

        public static void UpdateMasterEnachData(List<GetVendorEnachDataModel> vendorEnachDataList, string masterConnStr)
        {
            using (var masterConn = new SqlConnection(masterConnStr))
            {
                masterConn.Open();
                foreach (var item in vendorEnachDataList)
                {
                    using (var cmd = new SqlCommand("USP_UpdateMasterEnachData", masterConn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@mandate_id", item.mandate_id);
                        cmd.Parameters.AddWithValue("@transaction_id", item.transaction_id);
                        try
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string transactionId = reader["transaction_id"].ToString();
                                    string status = reader["status"].ToString();
                                    string message = reader["message"].ToString();
                                    if (status.Equals("success", StringComparison.OrdinalIgnoreCase))
                                    {
                                        logger.Info($"Transaction Id update Successfully in master table: {transactionId}: {message}");
                                    }
                                    else
                                    {
                                        logger.Error($"Transaction {transactionId}: {message}");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception while updating mandate_id={item.mandate_id}, transaction_id={item.transaction_id}: {ex.Message}");
                        }
                    }
                }
            }
        }

        public static List<GetVendorPaymentGetwayModel> GetPaymentTransactions(string masterConnStr)
        {
            List<GetVendorPaymentGetwayModel> paymentTransactions = new List<GetVendorPaymentGetwayModel>();
            DataSet objDs = null;
            try
            {
                using (var connection = new SqlConnection(masterConnStr))
                {
                    SqlParameter[] param = new SqlParameter[]
                    {
                        new SqlParameter("@vendor_code", "EWPL")
                    };
                    objDs = helper_class.SqlHelper.ExecuteDataset(masterConnStr,CommandType.StoredProcedure,"USP_GetPaymentTransactions",param);
                }
                if (objDs?.Tables[0] != null && objDs.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in objDs.Tables[0].Rows)
                    {
                        GetVendorPaymentGetwayModel transaction = new GetVendorPaymentGetwayModel
                        {
                            mode = row["mode"] != DBNull.Value ? Convert.ToString(row["mode"]) : string.Empty,
                            lead_id = row["lead_id"] != DBNull.Value ? Convert.ToString(row["lead_id"]) : string.Empty,
                            loan_id = row["loan_id"] != DBNull.Value ? Convert.ToString(row["loan_id"]) : string.Empty,
                            product_info = row["product_info"] != DBNull.Value ? Convert.ToString(row["product_info"]) : string.Empty,
                            status = row["status"] != DBNull.Value ? Convert.ToString(row["status"]) : string.Empty,
                            paid_on = row["paid_on"] != DBNull.Value ? Convert.ToDateTime(row["paid_on"]) : DateTime.MinValue,
                            amount = row["amount"] != DBNull.Value ? Convert.ToDecimal(row["amount"]) : 0,
                            is_payment_proccess = row["is_payment_proccess"] != DBNull.Value ? Convert.ToBoolean(row["is_payment_proccess"]) : false ,
                            transaction_id = row["transaction_id"] != DBNull.Value ? Convert.ToString(row["transaction_id"]) : string.Empty,
                            easebuzz_id = row["easebuzz_id"] != DBNull.Value ? Convert.ToString(row["easebuzz_id"]) : string.Empty,
                            customer_name = row["customer_name"] != DBNull.Value ? Convert.ToString(row["customer_name"]) : string.Empty,
                            customer_email = row["customer_email"] != DBNull.Value ? Convert.ToString(row["customer_email"]) : string.Empty,
                            message = row["message"] != DBNull.Value ? Convert.ToString(row["message"]) : string.Empty
                        };
                        paymentTransactions.Add(transaction);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error fetching payment transactions: {ex.Message}");
            }
            return paymentTransactions;
        }

        public static void UpdateMasterPaymentData(List<GetVendorPaymentGetwayModel> vendorPaymentGetwayModels, string masterConnStr)
        {
            using (var masterConn = new SqlConnection(masterConnStr))
            {
                masterConn.Open();
                foreach (var item in vendorPaymentGetwayModels)
                {
                    using (var cmd = new SqlCommand("USP_UpdateMasterPaymentDetails", masterConn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@loan_id", item.loan_id);
                        cmd.Parameters.AddWithValue("@transaction_id", item.transaction_id);
                        try
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string transactionId = reader["transaction_id"].ToString();
                                    string status = reader["status"].ToString();
                                    string message = reader["message"].ToString();
                                    if (status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        logger.Error($"Transaction {transactionId}: {message}");
                                    }
                                    else if (status.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                                    {
                                        logger.Error($"Transaction {transactionId}: {message}");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error($"Exception while updating mandate_id={item.loan_id}, transaction_id={item.transaction_id}: {ex.Message}");
                        }
                    }
                }
            }
        }

        public static List<PullPaymentServiceModel> GetPullPaymentTransactions(string masterConnStr)
        {
            List<PullPaymentServiceModel> pullPaymentServices = new List<PullPaymentServiceModel>();
            DataSet objDs = null;
            try
            {
                using (var connection = new SqlConnection(masterConnStr))
                {
                    SqlParameter[] param = new SqlParameter[0];
                    objDs = helper_class.SqlHelper.ExecuteDataset(masterConnStr, CommandType.StoredProcedure, "USP_EW_GetPullPaymentTransactions_V1", param);
                }
                if (objDs?.Tables[0] != null && objDs.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in objDs.Tables[0].Rows)
                    {
                        PullPaymentServiceModel transaction = new PullPaymentServiceModel
                        {
                            status = row["status"] != DBNull.Value ? Convert.ToString(row["status"]) : string.Empty,
                            merchant_request_number = row["merchant_request_number"] != DBNull.Value ? Convert.ToString(row["merchant_request_number"]) : string.Empty,
                            mandate_id = row["mandate_id"] != DBNull.Value ? Convert.ToString(row["mandate_id"]) : string.Empty,
                            mandate_transaction_id = row["mandate_transaction_id"] != DBNull.Value ? Convert.ToString(row["mandate_transaction_id"]) : string.Empty,
                            CreatedOn = row["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(row["CreatedOn"]) : DateTime.MinValue,
                            lead_id = row["lead_id"] != DBNull.Value ? Convert.ToString(row["lead_id"]) : string.Empty,
                            user_id = row["user_id"] != DBNull.Value ? Convert.ToString(row["user_id"]) : string.Empty,
                            loan_id = row["loan_id"] != DBNull.Value ? Convert.ToString(row["loan_id"]) : string.Empty,
                            customer_name = row["customer_name"] != DBNull.Value ? Convert.ToString(row["customer_name"]) : string.Empty,
                            request_type = row["request_type"] != DBNull.Value ? Convert.ToString(row["request_type"]) : string.Empty,
                            response_description = row["response_description"] != DBNull.Value ? Convert.ToString(row["response_description"]) : string.Empty,
                            amount = row["amount"] != DBNull.Value ? Convert.ToDecimal(row["amount"]) : 0,
                            is_pull_process = row["is_pull_process"] != DBNull.Value ? Convert.ToBoolean(row["is_pull_process"]) : false,
                            presentment_id = row["presentment_id"] != DBNull.Value ? Convert.ToString(row["presentment_id"]) : string.Empty,
                            presentment_date = row["presentment_date"] != DBNull.Value ? Convert.ToString(row["presentment_date"]) : string.Empty,
                            responce_type = row["responce_type"] != DBNull.Value ? Convert.ToString(row["responce_type"]) : string.Empty,
                            transaction_reference_number = row["transaction_reference_number"] != DBNull.Value ? Convert.ToString(row["transaction_reference_number"]) : string.Empty,
                            bank_reference_number = row["bank_reference_number"] != DBNull.Value ? Convert.ToString(row["bank_reference_number"]) : string.Empty,
                        };
                        pullPaymentServices.Add(transaction);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error fetching payment transactions: {ex.Message}");
            }
            return pullPaymentServices;
        }

        public static void UpdateVendorPullPaymentData(List<PullPaymentServiceModel> pullPaymentModels, string masterConnStr)
        {
            using (var masterConn = new SqlConnection(masterConnStr))
            {
                masterConn.Open();
                foreach (var item in pullPaymentModels)
                {
                    using (var cmd = new SqlCommand("USP_UpdateMasterPullPaymentDetails", masterConn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@loan_id", item.loan_id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@transaction_id", item.mandate_transaction_id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@merchant_request_number", item.merchant_request_number ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@mandate_id", item.mandate_id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@lead_id", item.lead_id ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@user_id", item.user_id ?? (object)DBNull.Value);
                        try
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string transactionId = reader["transaction_id"].ToString();
                                    string merchant_request_number = reader["merchant_request_number"].ToString();
                                    string status = reader["status"].ToString();
                                    string message = reader["message"].ToString();
                                    if (status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        logger.Info($"Pull Payment Transaction {transactionId}: {message}");
                                    }
                                    else if (status.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                                    {
                                        logger.Error($"Transaction {transactionId}: {message}");
                                    }
                                    else
                                    {
                                        logger.Warn($"Transaction {transactionId}: {message}");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error($"Exception while updating loan_id={item.loan_id}, transaction_id={item.mandate_transaction_id}: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}
