using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcWPFExample.Models
{
    public class CalcStorageFormat
    {
        public int sum;
        public List<string> history;
    }

    public class CalcResultsFormat
    {
        public string Operation { get; set; }
        public string Value { get; set; }
        public string Summary { get; set; }
    }
}
