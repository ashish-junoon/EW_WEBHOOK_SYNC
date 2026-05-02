using NLog;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Xml.Linq;

namespace DataSyncScheduler
{
    public class ScheduledEmailService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static class NotificationEmailHelper
        {
            public static string BuildSubject(dynamic item, string action_name)
            {
                switch (action_name.ToUpper())
                {
                    case "ENACH":
                        return $"ENach {item.status} Alert - Customer Id {item.user_id}";
                    case "PAYMENTGATEWAYSUCCESS":
                        return $"Payment Gateway {item.status} Alert - Loan Number {item.loan_id}";
                    case "PAYMENTGATEWAYFAILED":
                        return $"Payment Gateway {item.status} Alert - Loan Number {item.loan_id}";
                    case "PULLPAYMENTSUCCESS":
                        return $"Pull Payment {item.status} Alert - Loan Number {item.loan_id}";
                    case "PULLPAYMENTFAILED":
                        return $"Pull Payment {item.status} Alert - Loan Number {item.loan_id}";
                    default:
                        return $"System success Alert - Action {action_name}";
                }
            }

            public static string BuildBody(dynamic item, string action_name)
            {
                switch (action_name.ToUpper())
                {
                    case "ENACH":
                        return $@"
                            <!DOCTYPE html>
                            <html>
                              <body style=""font-family: Arial, sans-serif; line-height: 1.6; background-color: #f9fafb; margin: 0; padding: 20px;"">
                                <div style=""border: 1px solid #d1d5db; border-radius: 6px; padding: 20px; max-width: 650px; margin: auto; background-color: #ffffff;"">
                                  <div style=""font-size: 18px; font-weight: bold; margin-bottom: 12px; color: #111827;"">
                                    Dear {ConfigurationManager.AppSettings["Product"]} Team,
                                  </div>
                                  <p style=""margin: 0 0 12px 0; color: #374151;"">
                                    We regret to inform you that the ENach update for customer 
                                    <strong>{item.customer_name}</strong> has 
                                    <span style=""color: #dc2626; font-weight: bold;"">FAILED</span>.
                                  </p>
                                  <p style=""margin: 12px 0; font-weight: bold; color: #111827;"">ENach Details:</p>
                                  <table style=""width: 100%; border-collapse: collapse; margin: 8px 0; font-size: 14px;"">
                                    <tr>
                                      <td style=""padding: 8px; border: 1px solid #e5e7eb; background-color: #f9fafb;"">Mandate ID</td>
                                      <td style=""padding: 8px; border: 1px solid #e5e7eb;""><strong>{item.mandate_id}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 8px; border: 1px solid #e5e7eb; background-color: #f9fafb;"">Transaction ID</td>
                                      <td style=""padding: 8px; border: 1px solid #e5e7eb;""><strong>{item.transaction_id}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 8px; border: 1px solid #e5e7eb; background-color: #f9fafb;"">Status</td>
                                      <td style=""padding: 8px; border: 1px solid #e5e7eb;""><strong>{item.status}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 8px; border: 1px solid #e5e7eb; background-color: #f9fafb;"">User ID</td>
                                      <td style=""padding: 8px; border: 1px solid #e5e7eb;""><strong>{item.user_id}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 8px; border: 1px solid #e5e7eb; background-color: #f9fafb;"">Lead ID</td>
                                      <td style=""padding: 8px; border: 1px solid #e5e7eb;""><strong>{item.lead_id}</strong></td>
                                    </tr>
                                  </table>
                                  <p style=""margin: 16px 0; color: #374151;"">
                                    This customer's ENach has failed due to certain issues.  
                                    Kindly reprocess the ENach at the earliest to ensure smooth continuation of services.
                                  </p>
                                  <p style=""color: #6b7280; margin-top: 20px; font-size: 13px;"">
                                    <em>Note: This is an automated notification. Please do not reply.</em>
                                  </p>
                                </div>
                              </body>
                            </html>";

                    case "PAYMENTGATEWAYSUCCESS":
                        return $@"
                          <!DOCTYPE html>
                            <html>
                              <body style=""font-family: Arial, sans-serif; line-height: 1.6; background-color: #f3f4f6; margin: 0; padding: 20px;"">
                                <div style=""background-color: #eef6fb; border: 1px solid #cce0f5; border-radius: 6px; padding: 20px; max-width: 600px; margin: auto; box-shadow: 0 2px 5px rgba(0,0,0,0.1);"">
                                  <div style=""font-size: 18px; font-weight: bold; margin-bottom: 15px;"">
                                    Dear {ConfigurationManager.AppSettings["Product"]} Team,
                                  </div>
                                  <p>
                                    We are pleased to inform you that the Payment Gateway transaction initiated by customer 
                                    <strong>{item.customer_name}</strong> through the portal has been 
                                    <span style=""color: green; font-weight: bold;"">{item.status}</span>.
                                  </p>
                                  <p style=""margin-top: 15px; font-weight: bold;"">Transaction Details:</p>
                                  <table style=""width: 100%; border-collapse: collapse; margin: 10px 0;"">
                                    <tr>
                                      <td style=""padding: 8px; border: 1px solid #ddd;"">Loan Number</td>
                                      <td style=""padding: 8px; border: 1px solid #ddd;""><strong>{item.loan_id}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 8px; border: 1px solid #ddd;"">Transaction ID</td>
                                      <td style=""padding: 8px; border: 1px solid #ddd;""><strong>{item.transaction_id}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 6px; border: 1px solid #ddd;"">Easebuzz Pay ID</td>
                                      <td style=""padding: 6px; border: 1px solid #ddd;""><strong>{item.easebuzz_id}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 8px; border: 1px solid #ddd;"">Amount</td>
                                      <td style=""padding: 8px; border: 1px solid #ddd;""><strong>{item.amount}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 8px; border: 1px solid #ddd;"">Status</td>
                                      <td style=""padding: 8px; border: 1px solid #ddd;""><strong>{item.status}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 6px; border: 1px solid #ddd;"">Customer Email</td>
                                      <td style=""padding: 6px; border: 1px solid #ddd;""><strong>{item.customer_email}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 6px; border: 1px solid #ddd;"">Message</td>
                                      <td style=""padding: 6px; border: 1px solid #ddd;""><strong>{item.message}</strong></td>
                                    </tr>
                                  </table>
                                  <p>
                                    The payment has been processed successfully and securely recorded in the system.  
                                    This confirms that the transaction made via the customer portal is complete.
                                  </p>
                                  <p style=""color: #555; margin-top: 20px;"">
                                    <em>Note: This is an automated notification. Please do not reply.</em>
                                  </p>
                                </div>
                              </body>
                            </html>";
                    
                    case "PAYMENTGATEWAYFAILED":
                        return $@"
                        <!DOCTYPE html>
                            <html>
                              <body style=""font-family: Arial, sans-serif; line-height: 1.6; background-color: #f3f4f6; margin: 0; padding: 20px;"">
                                <div style=""border: 1px solid #ddd; border-radius: 4px; padding: 15px; max-width: 600px; margin: auto;"">
                                  <div style=""font-size: 18px; font-weight: bold; margin-bottom: 10px;"">
                                    Dear {ConfigurationManager.AppSettings["Product"]} Team,
                                  </div>
                                  <p>
                                    We regret to inform you that the Payment Gateway transaction initiated by customer 
                                    <strong>{item.customer_name}</strong> through the portal has 
                                    <span style=""color: red; font-weight: bold;"">FAILED</span>.
                                  </p>
                                  <p style=""margin-top: 10px; font-weight: bold;"">Transaction Details:</p>
                                  <table style=""width: 100%; border-collapse: collapse; margin: 8px 0;"">
                                    <tr>
                                      <td style=""padding: 6px; border: 1px solid #ddd;"">Loan Number</td>
                                      <td style=""padding: 6px; border: 1px solid #ddd;""><strong>{item.loan_id}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 6px; border: 1px solid #ddd;"">Transaction ID</td>
                                      <td style=""padding: 6px; border: 1px solid #ddd;""><strong>{item.transaction_id}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 6px; border: 1px solid #ddd;"">Easebuzz Pay ID</td>
                                      <td style=""padding: 6px; border: 1px solid #ddd;""><strong>{item.easebuzz_id}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 6px; border: 1px solid #ddd;"">Amount</td>
                                      <td style=""padding: 6px; border: 1px solid #ddd;""><strong>{item.amount}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 6px; border: 1px solid #ddd;"">Status</td>
                                      <td style=""padding: 6px; border: 1px solid #ddd;""><strong>{item.status}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 6px; border: 1px solid #ddd;"">Customer Email</td>
                                      <td style=""padding: 6px; border: 1px solid #ddd;""><strong>{item.customer_email}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 6px; border: 1px solid #ddd;"">Message</td>
                                      <td style=""padding: 6px; border: 1px solid #ddd;""><strong>{item.message}</strong></td>
                                    </tr>
                                  </table>
                                  <p>
                                    The payment could not be processed and has not been recorded in the system.  
                                    Please review the transaction details and take necessary action if required.
                                  </p>
                                  <p style=""color: #555; margin-top: 15px;"">
                                    <em>Note: This is an automated notification. Please do not reply.</em>
                                  </p>
                                </div>
                              </body>
                        </html>";

                    case "PULLPAYMENTSUCCESS":
                        return $@"
                             <!DOCTYPE html>
                            <html>
                              <body style=""font-family: Arial, sans-serif; line-height: 1.6; background-color: #f3f4f6; margin: 0; padding: 20px;"">
                                <!-- Outer Card -->
                                <div style=""background-color: #eef6fb; border: 1px solid #cce0f5; border-radius: 8px; padding: 24px; max-width: 640px; margin: auto; box-shadow: 0 3px 8px rgba(0,0,0,0.1);"">
                                  <div style=""font-size: 18px; font-weight: bold; margin-bottom: 15px; color:#111827;"">
                                    Dear {ConfigurationManager.AppSettings["Product"]} Team,
                                  </div>
                                  <p style=""margin-bottom: 15px; color:#1f2937;"">
                                    We are pleased to inform you that the pull payment transaction for customer 
                                    <strong>{item.customer_name}</strong> has been 
                                    <span style=""color: green; font-weight: bold;"">{item.status}</span>.
                                    This is a moment of satisfaction for us.
                                  </p>
                                  <p style=""margin-top: 15px; font-weight: bold; color:#111827;"">Transaction Details:</p>
                                  <table style=""width: 100%; border-collapse: collapse; margin: 10px 0; font-size:14px;"">
                                    <tr style=""background-color:#f9fafb;"">
                                      <td style=""padding: 10px; border: 1px solid #ddd;"">Loan Number</td>
                                      <td style=""padding: 10px; border: 1px solid #ddd;""><strong>{item.loan_id}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 10px; border: 1px solid #ddd;"">Mandate ID</td>
                                      <td style=""padding: 10px; border: 1px solid #ddd;""><strong>{item.mandate_id}</strong></td>
                                    </tr>
                                    <tr style=""background-color:#f9fafb;"">
                                      <td style=""padding: 10px; border: 1px solid #ddd;"">Mandate Transaction ID</td>
                                      <td style=""padding: 10px; border: 1px solid #ddd;""><strong>{item.merchant_request_number}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 10px; border: 1px solid #ddd;"">Amount</td>
                                      <td style=""padding: 10px; border: 1px solid #ddd;""><strong>{item.amount}</strong></td>
                                    </tr>
                                    <tr style=""background-color:#f9fafb;"">
                                      <td style=""padding: 10px; border: 1px solid #ddd;"">Status</td>
                                      <td style=""padding: 10px; border: 1px solid #ddd;""><strong>{item.status}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 10px; border: 1px solid #ddd;"">Created On</td>
                                      <td style=""padding: 10px; border: 1px solid #ddd;""><strong>{item.CreatedOn}</strong></td>
                                    </tr>
                                  </table>
                                  <p style=""margin-top: 15px; color:#374151;"">
                                    The pull payment has been processed successfully and securely recorded in the system.  
                                    This confirms that the transaction initiated via the pull process is complete.
                                  </p>
                                  <p style=""color: #6b7280; margin-top: 20px; font-size:13px; text-align:center;"">
                                    <em>Note: This is an automated notification. Please do not reply.</em>
                                  </p>
                                </div>
                              </body>
                        </html>";

                    case "PULLPAYMENTFAILED":
                        return $@"
                            <!DOCTYPE html>
                            <html>
                              <body style=""font-family: Arial, sans-serif; line-height: 1.6; background-color: #f3f4f6; margin: 0; padding: 20px;"">
                                <div style=""background-color: #fff5f5; border: 1px solid #f5c2c2; border-radius: 8px; padding: 24px; max-width: 640px; margin: auto; box-shadow: 0 3px 8px rgba(0,0,0,0.1);"">
      
                                  <div style=""font-size: 18px; font-weight: bold; margin-bottom: 15px; color:#111827;"">
                                    Dear {ConfigurationManager.AppSettings["Product"]} Team,
                                  </div>
                                  <p style=""margin-bottom: 15px; color:#1f2937;"">
                                    We regret to inform you that the pull payment transaction for customer 
                                     has 
                                    <span style=""color: red; font-weight: bold;"">{item.status}</span>.  
                                  </p>
                                  <p style=""margin-top: 15px; font-weight: bold; color:#111827;"">Transaction Details:</p>
                                  <table style=""width: 100%; border-collapse: collapse; margin: 10px 0; font-size:14px;"">
                                    <tr style=""background-color:#f9fafb;"">
                                      <td style=""padding: 10px; border: 1px solid #ddd;"">Loan Number</td>
                                      <td style=""padding: 10px; border: 1px solid #ddd;""><strong>{item.loan_id}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 10px; border: 1px solid #ddd;"">Mandate ID</td>
                                      <td style=""padding: 10px; border: 1px solid #ddd;""><strong>{item.mandate_id}</strong></td>
                                    </tr>
                                    <tr style=""background-color:#f9fafb;"">
                                      <td style=""padding: 10px; border: 1px solid #ddd;"">Mandate Transaction ID</td>
                                      <td style=""padding: 10px; border: 1px solid #ddd;""><strong>{item.merchant_request_number}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 10px; border: 1px solid #ddd;"">Amount</td>
                                      <td style=""padding: 10px; border: 1px solid #ddd;""><strong>{item.amount}</strong></td>
                                    </tr>
                                    <tr style=""background-color:#f9fafb;"">
                                      <td style=""padding: 10px; border: 1px solid #ddd;"">Status</td>
                                      <td style=""padding: 10px; border: 1px solid #ddd;""><strong>{item.status}</strong></td>
                                    </tr>
                                    <tr>
                                      <td style=""padding: 10px; border: 1px solid #ddd;"">Created On</td>
                                      <td style=""padding: 10px; border: 1px solid #ddd;""><strong>{item.CreatedOn}</strong></td>
                                    </tr>
                                  </table>
                                  <p style=""margin-top: 15px; color:#374151;"">
                                    The pull payment could not be processed successfully and has not been recorded in the system.  
                                    Please review the transaction details and take necessary action to resolve the issue.
                                  </p>
                                  <p style=""color: #6b7280; margin-top: 20px; font-size:13px; text-align:center;"">
                                    <em>Note: This is an automated notification. Please do not reply.</em>
                                  </p>
                                </div>
                              </body>
                        </html>";
                    default:
                        return $@"<html><body><p>No template defined for action: {action_name}</p></body></html>";
                }
            }
        }

        public static void SendEmail(dynamic item ,string action_name)
        {
            try
            {
                string senderEmail = ConfigurationManager.AppSettings["EmailFrom"];
                string senderPassword = ConfigurationManager.AppSettings["EmailPassword"];
                string recipientEmail = ConfigurationManager.AppSettings["EmailTo"];
                string Emailcc = ConfigurationManager.AppSettings["Emailcc"];
                string EmailBcc = ConfigurationManager.AppSettings["EmailBcc"];
                string smtpServer = ConfigurationManager.AppSettings["SmtpServer"];
                int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                string htmlFilePath = Path.GetTempFileName() + ".html";
                string subject = NotificationEmailHelper.BuildSubject(item, action_name);
                string emailBody = NotificationEmailHelper.BuildBody(item, action_name);
                File.WriteAllText(htmlFilePath, emailBody);
                string emailHtmlContent = File.ReadAllText(htmlFilePath);
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = subject,
                    Body = emailHtmlContent,
                    IsBodyHtml = true
                };
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsDevelopment"]))
                {
                    mail.To.Add(new MailAddress(ConfigurationManager.AppSettings["DevelopmentEmail"], "Namrata Singh"));
                }
                else
                {
                    mail.To.Add(new MailAddress(recipientEmail, "Early Wages"));

                    if (!string.IsNullOrEmpty(Emailcc))
                    {
                        foreach (var email in Emailcc.Split('|'))
                        {
                            mail.CC.Add(email);
                        }
                    }
                    if (!string.IsNullOrEmpty(EmailBcc))
                    {
                        foreach (var email in EmailBcc.Split('|'))
                        {
                            mail.Bcc.Add(email);
                        }
                    }
                }
                using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mail);
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error while sending ENach failure email: {ex.Message}");
            }
        }
    }
}
