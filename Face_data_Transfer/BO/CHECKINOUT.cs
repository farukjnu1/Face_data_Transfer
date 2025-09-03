using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face_data_Transfer.BO
{
   public class CHECKINOUT
    {
       public string BADGENUMBER { get; set; }
       public Int32 USERID { get; set; }
       public DateTime CHECKTIME { get; set; }
       public int VERIFYCODE { get; set; }
       public string WorkCode { get; set; }
    }
}
