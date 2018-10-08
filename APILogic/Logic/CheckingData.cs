using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.IO;
using System.Diagnostics;
using APILogic.Entities;
using APILogic.DBSet;
using System.Globalization;
using System.Web;
using System.Net.Mail;
//using SendGrid;
//using SendGrid.Helpers.Mail;
using System.Configuration;
using System.Net;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace APILogic.Logic
{
    public class CheckingData
    {
       
        
        private LogsContext db = new LogsContext();
        public string pathToFiles = @"C:\inetpub\wwwroot\LagoonCheckInLog\DataFile\file.xlsx";//@"C:\Users\festu\Documents\Visual Studio 2015\Projects\LagoonCheckInLog\file.xlsx"; //@"C:\file.xlsx"; C:\Users\festu\Documents\Visual Studio 2015\Projects\LagoonCheckInLog\file.xlsx";

        //HttpContext.Current.Server.MapPath("~/LagoonCheckInLog/Reports/file.xlsx");
        // GET: api/LogsC:\Users\festu\Documents\Visual Studio 2015\Projects\LagoonCheckInLog\LagoonCheckInLog\Reports\json.json
        public IQueryable<Logs> GetPatientChecks()
        {
            return db.PatientChecks;
        }


        public async Task<string> SendReportPerlocation(DateTime date, string providerkey, string providerEmail)
        {
            //Execute();
            try
            {
                List<Logs> codeDetails = FacilityWise(providerkey);
                FileInfo fileInfo = new FileInfo(pathToFiles);

                // If there is any file having same name as 'Sample2.xlsx', then delete it first
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                    //string pathToFiles = HttpContext.Current.Server.MapPath(pathToFiles);
                    fileInfo = new FileInfo(pathToFiles);
                }
                using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
                {
                    var workSheet = GetWorkSheet(excelPackage, 0);

                    workSheet.Cells["A2"].LoadFromCollection(codeDetails, false, OfficeOpenXml.Table.TableStyles.Medium1);

                    excelPackage.Save();

                }
               await SendMail(providerEmail);
                return "Success"; 
            }
            catch(Exception ex)
            {
                //throw new System.InvalidOperationException(ex.Message);
                return ex.Message;
                
            }
        }
        public List<Logs> FacilityWise(string providerkey)
        {
            var list = db.PatientChecks.Where(x => x.ProviderKey == providerkey && x.BatchTime == DateTime.Today).ToList();
            return list;
        }

        public static ExcelWorksheet GetWorkSheet(ExcelPackage excelPackage, int count)
        {
            var workSheet = excelPackage.Workbook.Worksheets.Add("DailyCheckIn " + count);
            workSheet.View.ShowGridLines = false;
            workSheet.Cells["A1"].Value = "S/N";
            workSheet.Cells["B1"].Value = "Provider Key";
            workSheet.Cells["C1"].Value = "Facility Name";
            workSheet.Cells["D1"].Value = "Enrollee Number";
            workSheet.Cells["E1"].Value = "Last Name";
            workSheet.Cells["F1"].Value = "FirstName";
            workSheet.Cells["G1"].Value = "Other Name";
            workSheet.Cells["H1"].Value = "Transaction Date";
            workSheet.Cells["I1"].Value = "Patient Hospital Number";
            workSheet.Cells["J1"].Value = "Batch";
            workSheet.Cells["A1:J1"].Style.Font.Bold = true;
            workSheet.Column(7).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.FullDateTimePattern;
            workSheet.Column(1).Hidden = true;
            workSheet.Column(2).Hidden = true;
            workSheet.Column(10).Hidden = true;
            return workSheet;
        }



       public async Task SendMail(string providermail)
        {
            Byte[] bytes = File.ReadAllBytes(pathToFiles);
            String file = Convert.ToBase64String(bytes);
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("noreply@lagoonshospitals.com", "Lagoon Hospitals"),
                Subject = "Hygeia Check-in Data For The Last Three Hours",
                HtmlContent = "<strong><span>Dear PCS</span><br/><span><p>Please find attached file for your perusal.</p>  <p>It contain the Check-In data for hygeia patients as being reported on hydirect for the last three hours</p><p>Kind regards</p></span></strong>"
            };
            //msg.AddTo(new EmailAddress(providermail, ""));
            msg.AddTo(new EmailAddress("olugbenga.oluyide@lagoonhospitals.com", ""));
            msg.AddBcc(new EmailAddress("gbengaoluyide@gmail.com", ""));
            //msg.AddBcc(new EmailAddress("rsteiner@myitguy.ng", ""));
            //msg.AddBcc(new EmailAddress("richard.steiner@lagoonhospitals.com", ""));
            msg.AddBcc(new EmailAddress("adekunle.omidiora@lagoonhospitals.com", ""));
            msg.AddBcc(new EmailAddress("olugbenga.oluyide@lagoonhospitals.com", ""));
            msg.AddBcc(new EmailAddress("olugbenga.oluyide@lagoonhospitals.com", ""));
            msg.AddAttachment(pathToFiles, file, "text/csv", "attachment", "banner");
            var response = await  client.SendEmailAsync(msg);
        }



        //public void SendMail(string providerEmail)
        //{
        //    try
        //    {
        //        MailMessage mail = new MailMessage();
        //        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);

        //        mail.From = new MailAddress("noreply.lagoonshospitals@gmail.com");
        //        mail.To.Add(providerEmail);
        //        mail.Bcc.Add("gbengaoluyide@gmail.com");
        //        mail.Bcc.Add("adekunle.omidiora@lagoonhospitals.com");
        //        mail.Subject = "Testing Check-In Data";
        //        mail.Body = "Dear PCS Team: If You recieve this mail then it works!!! Yipee";


        //        Attachment attachment = new Attachment(pathToFiles);
        //        mail.Attachments.Add(attachment);
        //        //SmtpServer.Port = 587;

        //        SmtpServer.EnableSsl = false;
        //        SmtpServer.TargetName = "STARTTLS/smtp.gmail.com";
        //        SmtpServer.Credentials = new System.Net.NetworkCredential("noreply.lagoonshospitals@gmail.com", "Pass@word2");
        //        SmtpServer.Send(mail);

        //        //using (var smtp = new SmtpClient())
        //        //{
        //        //    var credential = new NetworkCredential
        //        //    {
        //        //        UserName = "noreply.lagoonshospitals@gmail.com",  // replace with valid value
        //        //        Password = "Pass@word2"  // replace with valid value
        //        //    };
        //        //    smtp.Credentials = credential;
        //        //    smtp.Port = 587;
        //        //    smtp.EnableSsl = true;
        //        //    smtp.SendMailAsync(mail);

        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        //ex.Message;
        //    }
        //}


        //public void ToAdminEmail(string providerEmail)
        //{
        //    try
        //    {
        //        var myMessage = new MailMessage();
        //        myMessage.To.Add(new MailAddress(providerEmail));  // replace with valid value 
        //        myMessage.CC.Add(new MailAddress("gbengaoluyide@gmail.com"));
        //        //myMessage.CC.Add(new MailAddress("ayodeleenitilo@yahoo.com"));
        //        //myMessage.CC.Add(new MailAddress("edekin02@yahoo.com"));

        //        myMessage.From = new MailAddress("festusoluyide@gmail.com");  // replace with valid value
        //        myMessage.Subject = "Checkin Data";
        //        myMessage.Body = "it Seems it works";
        //        myMessage.IsBodyHtml = true;

        //        Attachment attachment = new Attachment(pathToFiles);
        //        myMessage.Attachments.Add(attachment);

        //        using (var smtp = new SmtpClient())
        //        {
        //            var credential = new NetworkCredential
        //            {
        //                UserName = "festusoluyide@gmail.com",  // replace with valid value
        //                Password = "oluyide123"  // replace with valid value
        //            };
        //            smtp.Credentials = credential;
        //            smtp.Host = "smtp.gmail.com";
        //            smtp.Port = 587;
        //            smtp.EnableSsl = false;
        //            smtp.Send(myMessage);
        //        }
        //    }
        //    catch(Exception ex)
        //    {

        //    }
        //}
        //}
    }
}
