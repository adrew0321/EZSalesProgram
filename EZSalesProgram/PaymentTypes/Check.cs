using EZSalesProgram.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZSalesProgram.PaymentTypes
{
    class Check : Payment
    {
        public string CheckNum { set; get; }
        public double AmountGiven { set; get; }
    }
}
