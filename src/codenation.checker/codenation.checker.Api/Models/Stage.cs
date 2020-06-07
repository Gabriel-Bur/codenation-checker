using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace codenation.checker.Api.Models
{
    public class Stage
    {
        public string id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public int type { get; set; }
        public bool locked { get; set; }
        public int challenge_count { get; set; }

    }
}
