using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceApp.DbModel
{
    public class Transaction
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string BillStatus { get; set; }

        public string Description { get; set; }

        public string PaymentStatus { get; set; }

        public float Amount { get; set; }

    }
}
