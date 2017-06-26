using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTAPIExample
{
    public class organization
    {
        public string isDefault { get; set; }
        public string organizationName { get; set; }
        public string portalURL { get; set; }
        public string id { get; set; }
        public string logoURL { get; set; }
    }

    public class organizationList
    {
        public List<organization> data { get; set; }
    }
}
