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

        public override string ToString()
        {
            return string.Format(
                "{{ Id: '{0}', Id2: '{1}', X: '{2}', Date: '{3}', Date2: {4} }}",
                this.Id,
                this.Id2,
                this.X,
                this.Date,
                this.Date2);
        }
    }
}
