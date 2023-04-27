using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        public string CompensationId { get; set; }
        public Employee Employee { get; set; }
        public Decimal Salary {get; set; }
        public DateTime EffectiveDate { get; set; }

    }
}
