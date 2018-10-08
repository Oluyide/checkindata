using APILogic.DBSet;
using APILogic.Entities;
using APILogic.Logic;
using Checkin.Models;
using Dapper;
using Hangfire;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Checkin.Controllers
{
    public class HomeController : Controller
    {
        public static string constr = ConfigurationManager.ConnectionStrings["DashboardContext"].ConnectionString;
        public static string constrin = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        SqlConnection con = new SqlConnection(constr);
        SqlConnection cont = new SqlConnection(constrin);
        CheckingData data = new CheckingData();
        private LogsContext db = new LogsContext();
        static string baseUrl = "https://online.hygeiahmo.com/hygeiaapiservice";
        static string username = "festusoluyide@gmail.com";
        static string password = "Pass@word1";
        //static int providerId = 888;
        //static int key = 888;
        static DateTime startDate = DateTime.Today.AddDays(-1);
        static DateTime endDate = DateTime.Now;
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [HttpGet]
        public ActionResult CheckingData()
        {
            return View();
        }

        [HttpPost]
        [AutomaticRetry(Attempts = 10, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task<ActionResult> CheckingData(string y = "")
        {
            var providerData = db.Facilities.ToList();
            using (var client = new HttpClient())
            {
                
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));


                    Task<string> callTask = Task.Run(() => GetAccessToken());
                    // Wait for it to finish
                    callTask.Wait();
                    // Get the result
                    var ret = callTask.Result;
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ret);
                   
               
                
                //try
                //{
                    foreach (var value in providerData)
                    {
                        // create the URL string.
                        string url = string.Format(client.BaseAddress + "/api/PCClaims/GetCheckInData");

                        var parameters = new Dictionary<string, string> { { "ProviderId", value.ProviderId.ToString() }, { "Key", value.Key.ToString() }, { "StartDate", startDate.ToString() }, { "EndDate", endDate.ToString() } };
                        var encodedContent = new FormUrlEncodedContent(parameters);
                    // make the request
                    //System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var response = await client.PostAsync(url, encodedContent);

                        // parse the response and return the data.
                        if (response.IsSuccessStatusCode)
                        {
                            string jsonString = await response.Content.ReadAsStringAsync();
                            List<APIResponse> responseData = JsonConvert.DeserializeObject<List<APIResponse>>(jsonString);
                            foreach (var api in responseData)
                            {
                            
                            Logs logs = new Logs();
                                logs.ProviderName = value.ProviderName;
                                logs.ProviderKey = api.providerId.ToString();
                                logs.Action = api.enrolleeNo;
                                logs.LastName = api.lastName;
                                logs.FirstName = api.firstName;
                                logs.OtherNames = api.othername;
                                logs.TrasactionDate = api.checkInDate;
                                //logs.BatchTime = DateTime.Today;

                                var objDetails = SqlMapper.QueryMultiple(con, "GetReg", new { EnrolleeNum = api.enrolleeNo, startDate= startDate, lastDate = endDate }, null, 100000, commandType: CommandType.StoredProcedure);
                                PatientDetails details = new PatientDetails();
                                var Data = objDetails.Read<PatientDetails>();
                            
                                details.RegistrationNo = Data.Select(x => x.RegistrationNo).LastOrDefault();
                                details.PlanType = Data.Select(x => x.PlanType).LastOrDefault();
                                details.CheckInBy = Data.Select(x => x.CheckInBy).LastOrDefault();
                            
                            if (!string.IsNullOrEmpty(details.RegistrationNo))
                                    logs.PatientHospitalNo = details.RegistrationNo;
                                else logs.PatientHospitalNo = "This enrollee number is incorrect,please edit the enrollee number";
                                if (!string.IsNullOrEmpty(details.PlanType))
                                    logs.PlanType = details.PlanType;
                                else logs.PlanType = "The enrollee number must be corrected to view this Data";
                                if (!string.IsNullOrEmpty(details.CheckInBy))
                                    logs.CheckInBy = details.CheckInBy;
                                else logs.CheckInBy = "The enrollee number must be corrected to view this Data";
                            con.Close();
                            //check Values
                            var checkexistence = data.CheckexistingValues(api.enrolleeNo, api.checkInDate).ToList();
                            if (checkexistence.Count < 1)
                            {
                                db.PatientChecks.Add(logs);
                            }
                            db.SaveChanges();
                        }
                       
                    }
                   
                    var result = await data.SendReportPerlocation(startDate, value.ProviderId.ToString(), value.ProviderEmail,value.ProviderName);
                        if (result== "Success")
                        {
                            TempData["MessageSent"] = "Message Sent Sucessfully";
                        }
                        else
                        {
                            TempData["MessageSent"] = result;
                            break;
                        }
                    }
                //}
                //catch (Exception ex)
                //{

                //    TempData["error"] = "Invalid Value: This is not an Hospital Number" + ex.InnerException;
                //    return View();
                //}

                TempData["sucess"] = "Sucessfully Completed";

                return View();
            }

        }


        //private bool SendEmail()
        //{
        //    string sampleContent = Base64Encode("Testing");  // base64 encoded string
        //    var client = new SendGridClient("apiKey");

        //    var msg = new SendGridMessage()
        //    {
        //        From = new EmailAddress(sender),
        //        Subject = "Adherence Report",
        //        PlainTextContent = "Sample Content ",
        //        HtmlContent = "<strong>Hello, Email!</strong>"
        //    };
        //    msg.AddTo(new EmailAddress(recipient, null));
        //    msg.AddAttachment("myfile.csv", sampleContent, "text/csv", "attachment", "banner");

        //    var response = await client.SendEmailAsync(msg);
        //}

        private static async Task<string> GetAccessToken()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


              

                // Build up the data to POST.
                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("grant_type", "password"));
                postData.Add(new KeyValuePair<string, string>("username", username));
                postData.Add(new KeyValuePair<string, string>("password", password));

                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);
                //System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                //System.Net.ServicePointManager.SecurityProtocol |=  SecurityProtocolType.Tls12;
                // Post to the Server and parse the response.
                HttpResponseMessage response = await client.PostAsync(baseUrl + "/oauth2/token", content);
                string jsonString = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<AccessToken>(jsonString);

                return responseData.access_token;
            }
        }

    }
    //public ActionResult About()
    //    {
    //        ViewBag.Message = "Your application description page.";

    //        return View();
    //    }

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    
}