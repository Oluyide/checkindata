using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Checkin.Models
{
    public class APIResponse
    {
        public string enrolleeNo { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string othername { get; set; }
        public int providerId { get; set; }
        public int source { get; set; }
        public DateTime checkInDate { get; set; }
    }
}