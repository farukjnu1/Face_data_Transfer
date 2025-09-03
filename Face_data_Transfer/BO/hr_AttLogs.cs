using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face_data_Transfer.BO
{
    public class hr_AttLogs
    {
        public int Id { get; set; }
        public string EnrollNumber { get; set; }
        public int VerifyMode { get; set; }
        public int InOutMode { get; set; }
        public DateTime Log_time { get; set; }
        public int WorkCode { get; set; }
    }
}
