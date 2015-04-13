using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleService.Contract
{
    public class SampleType
    {
        public long Id { get; set; }

        public Guid Id2 { get; set; }

        private string X { get; set; }

        public DateTime Date { get; set; }

        public DateTime Date2 { get; set; }
    }
}
