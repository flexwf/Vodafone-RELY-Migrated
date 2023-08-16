using System;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace RELY_EMAIL
{
    public partial class RelyEmailService : ServiceBase
    {
        public RelyEmailService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.WriteToFile("RELYEmail Service started {0}");
            this.WriteToFile("RELYEmail Service Mode: Interval");
            this.ScheduleService();
        }

        protected override void OnStop()
        {
            this.WriteToFile("RELYEmail Service stopped {0}");
            this.Schedular.Dispose();
        }

        private Timer Schedular;

        public void ScheduleService()
        {
            //Set the Default Time.
            DateTime scheduledTime = DateTime.MinValue;
            try
            {
                Schedular = new Timer(new TimerCallback(SchedularCallback));
                //string mode = ConfigurationManager.AppSettings["Mode"].ToUpper();
                string mode = "INTERVAL";

                if (mode == "DAILY")
                {
                    //Get the Scheduled Time from AppSettings.
                    scheduledTime = DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["ScheduledTime"]);
                    if (DateTime.Now > scheduledTime)
                    {
                        //If Scheduled Time is passed set Schedule for the next day.
                        scheduledTime = scheduledTime.AddDays(1);
                    }
                }

                if (mode.ToUpper() == "INTERVAL")
                {
                    //Get the Interval in Minutes from AppSettings.
                    int intervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalMinutes"]);

                    //Set the Scheduled Time by adding the Interval to Current Time.
                    scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);
                    if (DateTime.Now > scheduledTime)
                    {
                        //If Scheduled Time is passed set Schedule for the next Interval.
                        scheduledTime = scheduledTime.AddMinutes(intervalMinutes);
                    }
                    else
                    {
                        //Call method to send Email
                        SendEmails();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToFile("RELYEmail Service Error on: {0} " + ex.Message + ex.StackTrace);

                //Stop the Windows Service.
                using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController("RELYEmailService"))
                {
                    // serviceController.Stop(); commented by Shubham as service should continue even after exception
                }
            }
            TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
            string schedule = string.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            //this.WriteToFile("RELYEmail Service scheduled to run after: " + schedule + " {0}");

            //Get the difference in Minutes between the Scheduled and Current Time.
            int dueTime = Math.Abs(Convert.ToInt32(timeSpan.TotalMilliseconds));

            //Change the Timer's Due Time.
            Schedular.Change(dueTime, Timeout.Infinite);
        }

        private void SchedularCallback(object e)
        {
            this.WriteToFile("RELYEmail Service Log CallBack: {0}");
            this.ScheduleService();
        }

        private void WriteToFile(string text)
        {
            string path = ConfigurationManager.AppSettings["LogFilePath"];
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }

        private void SendEmails()
        {
            //Connect to database
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();

            //Get the list of unsent emails
            //Suggested Values - InQueue/Sent/Failed/Bouncedback
            string qry = "Select C.RequiresSSL,C.EmailId,C.DisplayName,C.ReplyTo, C.SmtpHost,C.SmtpLoginId,C.SMTPPassword,C.PortNumber,B.ReplyToList,B.Priority,B.IsHTML,B.RecipientList,B.CCList,B.BCCList,B.Body,B.Subject,B.AttachmentList,B.Id From LEmailBucket B Inner Join GEmailConfigurations C ON B.SenderConfigId = C.Id Where B.Status = 'InQueue' ORDER BY CASE WHEN Priority LIKE '%Low' THEN 3 WHEN Priority LIKE '%Normal' THEN 2 WHEN Priority LIKE '%High' THEN 1 END";
            SqlCommand cmd = new SqlCommand(qry, cn);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable tb = new DataTable();
            da.Fill(tb);

            if (tb.Rows.Count > 0)//check if there are any pending emails that needs to be sent
            {
                //For each iteration of the list update the database with the status of the email sending for that particular row.
                //Loop through the list and send emails one by one
                foreach (DataRow dr in tb.Rows)
                {
                    try
                    {
                        SmtpClient Client = new SmtpClient(dr.Field<string>("SmtpHost"));
                        Client.Credentials = new NetworkCredential(dr.Field<string>("SmtpLoginId"), dr.Field<string>("SMTPPassword"));
                        Client.Port = dr.Field<int>("PortNumber");
                        Client.EnableSsl = dr.Field<bool>("RequiresSSL");
                        MailMessage message = new MailMessage();
                        message.From = new MailAddress(dr.Field<string>("EmailId"), dr.Field<string>("DisplayName"));

                        //set priority
                        switch (dr.Field<string>("Priority").ToLower())
                        {
                            case "low":
                                message.Priority = MailPriority.Low;
                                break;
                            case "normal":
                                message.Priority = MailPriority.Normal;
                                break;
                            case "high":
                                message.Priority = MailPriority.High;
                                break;
                        }
                        var ReceipientArray = dr.Field<string>("RecipientList").Split(',');// comma, seperator;,
                        var ReplyToList = new System.Collections.Generic.List<string>();
                        //Add ReplyTo Email from LEmailBucket and if it null then use ReplyToEmail from GEmailConfigurations
                        if (!string.IsNullOrEmpty(dr.Field<string>("ReplyToList")))
                        {
                            var ReplyToArray = dr.Field<string>("ReplyToList").Split(',');
                            ReplyToList = ReplyToArray.ToList(); //|;
                        }
                        else
                        {
                            message.ReplyToList.Add(dr.Field<string>("ReplyTo"));
                        }
                        //Loop through Receipient array
                        for (int j = 0; j < ReceipientArray.Length; j++)
                        {
                            message.To.Add(ReceipientArray[j]);//Add receipients to the mail
                        }

                        for (var k = 0; k < ReplyToList.Count(); k++)
                        {
                            message.ReplyToList.Add(ReplyToList.ElementAt(k));
                        }

                        if (!string.IsNullOrEmpty(dr.Field<string>("CCList")))
                        {
                            var CCArray = dr.Field<string>("CCList").Split(',');
                            for (int j = 0; j < CCArray.Length; j++)
                            {
                                message.To.Add(CCArray[j]);//Add CC to the mail
                            }
                        }

                        if (!string.IsNullOrEmpty(dr.Field<string>("BCCList")))
                        {
                            var BCCArray = dr.Field<string>("BCCList").Split(',');
                            for (int j = 0; j < BCCArray.Length; j++)
                            {
                                message.To.Add(BCCArray[j]);//Add BCC to the mail
                            }
                        }

                        message.Body = dr.Field<string>("Body");
                        message.Subject = dr.Field<string>("Subject");
                        message.IsBodyHtml = dr.Field<bool>("IsHTML");
                        if (!string.IsNullOrEmpty(dr.Field<string>("AttachmentList")))
                        {
                            var AttachmentArray = dr.Field<string>("AttachmentList").Split('|');//pipe seperated string
                            for (int j = 0; j < AttachmentArray.Length; j++)
                            {
                                //add attachment
                                var Attachment = new System.Net.Mail.Attachment(AttachmentArray[j]);
                                message.Attachments.Add(Attachment);
                            }
                        }

                        Client.Send(message);
                        qry = "update LEmailBucket set Status = 'Sent', UpdatedDateTime=GetDate() where Id=" + dr.Field<int>("Id");
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        WriteToFile("RELYEmail Service Error on: {0} " + ex.Message + ex.StackTrace);
                        qry = "update LEmailBucket set Status = 'Failed', UpdatedDateTime=GetDate(), Comments='" + ex.InnerException.Message + "' where Id=" + dr.Field<int>("Id");
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();
                        continue;
                    }
                }
                cn.Close();
            }
        }
    }
}
