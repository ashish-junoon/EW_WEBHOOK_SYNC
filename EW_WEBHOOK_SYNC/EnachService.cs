using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace DataSyncScheduler
{
    public class EnachService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void ProcessVendorEnachData(List<GetVendorEnachDataModel> vendorEnachDataList, string earlywagesConnStr, string masterConnStr)
        {
            try
            {
                foreach (var data in vendorEnachDataList)
                {
                    try
                    {
                        EnachService.UpdateVendorEnachData(data, earlywagesConnStr);
                        if (!string.IsNullOrWhiteSpace(data.mandate_id) &&
                            !string.IsNullOrWhiteSpace(data.transaction_id) &&
                            !string.IsNullOrWhiteSpace(data.status) &&
                            !string.IsNullOrWhiteSpace(data.customer_name)
                            )
                        {
                            bool isSuccess = data.status.Equals("failed", StringComparison.OrdinalIgnoreCase);
                            if (isSuccess)
                            {
                                logger.Error("Enach Failed :" + data.transaction_id);
                                ScheduledEmailService.SendEmail(data, "ENACH");
                                logger.Error("Enach Failed for Transaction ID: " + data.transaction_id + " | Email notification has been sent.");
                            }
                        }
                    }
                    catch (Exception innerEx)
                    {
                        logger.Error($"Error processing record LeadId={data.lead_id}: {innerEx.Message}");
                    }
                }
                if (vendorEnachDataList.Count > 0)
                {
                    try
                    {
                        MasterData.UpdateMasterEnachData(vendorEnachDataList, masterConnStr);
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

        public static void UpdateVendorEnachData(GetVendorEnachDataModel data, string connectionString)
        {
          try
          {
              using (var connection = new SqlConnection(connectionString))
              {
                  SqlParameter[] param =
                  {
                      new SqlParameter("@user_id", SqlDbType.NVarChar, 20)
                      {
                          Value = (object)data.user_id ?? DBNull.Value
                      },
                      new SqlParameter("@lead_id", SqlDbType.NVarChar, 20)
                      {
                          Value = (object)data.lead_id ?? DBNull.Value
                      },
                      new SqlParameter("@status", SqlDbType.NVarChar, 20)
                      {
                          Value = (object)data.status ?? DBNull.Value
                      },
                      new SqlParameter("@message", SqlDbType.NVarChar, 200)
                      {
                          Value = (object)data.message ?? DBNull.Value
                      },
                      new SqlParameter("@transaction_id", SqlDbType.NVarChar, 100)
                      {
                          Value = (object)data.transaction_id ?? DBNull.Value
                      },
                      new SqlParameter("@mandate_id", SqlDbType.NVarChar, 50)
                      {
                          Value = (object)data.mandate_id ?? DBNull.Value
                      },
                      new SqlParameter("@vendor_code", SqlDbType.NVarChar, 10)
                      {
                          Value = (object)data.vendor_code ?? DBNull.Value
                      }
                  };
                  using (SqlDataReader reader = helper_class.SqlHelper.ExecuteReader(connectionString,CommandType.StoredProcedure,"USP_UpdateVendorEnachData",param))
                  {
                      while (reader.Read())
                      {
                          string mandateId = reader["mandate_id"]?.ToString();
                          string transactionId = reader["transaction_id"]?.ToString();
                          string status = reader["status"]?.ToString();
                          string customer_name = data.customer_name;
                          logger.Info("Updated mandate_id={MandateId}, transaction_id={TransactionId}, status={Status}", mandateId,transactionId,status);
                      }
                  }
              }
          }
          catch (Exception ex)
          {
              logger.Error(ex, "Error updating ENach data for user_id {UserId}", data.user_id);
          }
        }
    }
}
