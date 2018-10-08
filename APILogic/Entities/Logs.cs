using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILogic.Entities
{
    public class Logs
    {
        public int Id { get; set; }
        //public string User { get; set; }
        //public string ModuleName { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderName { get; set; }
        public string Action { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public DateTime TrasactionDate { get; set; }
        public string PatientHospitalNo { get; set; }
        public string PlanType { get; set; }
        public string CheckinBy { get; set; }
        public DateTime BatchTime { get; set; }
    }
}
