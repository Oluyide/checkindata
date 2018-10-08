using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILogic.Entities
{
    public class FacilityIdentities
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public int Key { get; set; }
        public string ProviderName { get; set; }
        public string ProviderEmail { get; set; }
    }
}
