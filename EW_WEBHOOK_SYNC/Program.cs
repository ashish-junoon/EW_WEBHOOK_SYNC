using DataSyncScheduler;
using System;
using System.Collections.Generic;
using System.Configuration;


class Program
{
    private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    public static void Main()
    {
        string masterConnStr = ConfigurationManager.ConnectionStrings["MasterConnection"]?.ConnectionString;
        string earlywagesConnStr = ConfigurationManager.ConnectionStrings["EarlyWagesConnection"]?.ConnectionString;
        if (string.IsNullOrEmpty(masterConnStr) ||
            string.IsNullOrEmpty(earlywagesConnStr))
        {
            logger.Error("One or more connection strings are missing. Process aborted.");
            return; 
        }
        try
        {
            //// ************************ Call method to get PU Enach data list *********************************************
            logger.Info("Call Method GetVendorEnachData");

            List<GetVendorEnachDataModel> vendorEnachDataList = MasterData.GetVendorEnachData(masterConnStr);

            logger.Info("End Method GetVendorEnachData successfully !!");

            if (vendorEnachDataList != null && vendorEnachDataList.Count > 0)
            {
                logger.Info("Call Method ProcessVendorEnachData");

                EnachService.ProcessVendorEnachData(vendorEnachDataList, earlywagesConnStr, masterConnStr);

                logger.Info("End Method ProcessVendorEnachData successfully !!");
            }
            else
            {
                logger.Info("No ENach data found. Skipping ENach processing.");
            }
            // ************************ Call method to get Payment Gateway data *********************************************
            logger.Info("Call Method GetPaymentTransactions.");

            List<GetVendorPaymentGetwayModel> vendorpaymentgetwaysList = MasterData.GetPaymentTransactions(masterConnStr);
            
            logger.Info("End Method GetPaymentTransactions successfully !!");

           if (vendorpaymentgetwaysList != null && vendorpaymentgetwaysList.Count > 0)
           {
                logger.Info("Call Method ProcessVendorPaymentData");

                PaymentGetwayService.ProcessVendorPaymentData(vendorpaymentgetwaysList, earlywagesConnStr, masterConnStr);

                logger.Info("End Method ProcessVendorPaymentData successfully !!");
           }
           else
           {
                logger.Info("No Payment Gateway data found. Skipping Payment Gateway processing.");
           }
            // ************************ Call method to get Pull Payment data *********************************************
            logger.Info("Call Method GetPullPaymentTransactions");

            List<PullPaymentServiceModel> pullPaymentServiceModels = MasterData.GetPullPaymentTransactions(masterConnStr);
            
            logger.Info("End Method GetPullPaymentTransactions successfully !!");

            if (pullPaymentServiceModels != null && pullPaymentServiceModels.Count > 0)
            {
                logger.Info("Call Method ProcessVendorPullPaymentData");

                PullPaymentService.ProcessVendorPullPaymentData(pullPaymentServiceModels, earlywagesConnStr, masterConnStr);
                
                logger.Info("End Method ProcessVendorPullPaymentData successfully !!");
            }
            else
            {
                logger.Info("No Pull Payment data found. Skipping Pull Payment processing.");
            }
        }
        catch (Exception ex)
        {
            logger.Error($"Error occurred while processing: {ex.Message}");
        }
    }

}