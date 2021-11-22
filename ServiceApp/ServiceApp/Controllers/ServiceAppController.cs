
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceApp.DataLayer;
using ServiceApp.DbModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceApp.Controllers
{
    [Route("api/[controller]")]
    public class ServiceAppController : ControllerBase
    {
        private readonly ApiContext _context;

        public ServiceAppController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<Transaction> Get(int id)
        {
            if (id < 0)
                throw new Exception("ID is empty");

            var transactions = await _context.Transactions.Where(p => p.Id == id).FirstOrDefaultAsync();
            
            if (transactions == null)
                throw new Exception("Transaction not found");

           return transactions;
        }

        [HttpPost]
        [Route("createTransactionService")]
        public async Task<Transaction> CreateTransaction(Transaction transactions)
        {
            Transaction trans;

            if (transactions.Id < 1)
            {
                trans = _context.Transactions.Add(transactions)?.Entity;
            }
            else
            {
                trans = await _context.Transactions.AsNoTracking().Where(p => p.Id == transactions.Id).FirstOrDefaultAsync();
                trans = transactions;
                _context.Transactions.Update(trans);

            }
                
            await _context.SaveChangesAsync();

            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, DateTime.UtcNow.Day.ToString() + DateTime.UtcNow.Month.ToString() + DateTime.UtcNow.Year.ToString() + "_"+ trans.Id + ".txt")))
            {
                await outputFile.WriteAsync("Id: " + trans.Id.ToString() + " Amount:" + trans.Amount.ToString() + "$ Status:" + trans.BillStatus + " Date:" + trans.Date.ToLongDateString() + " Description:" + trans.Description);

            }

            return trans;
        }


        [HttpPost]
        public async Task<IActionResult> CreateInvoice(InvoiceDate invoiceDates)
        {
            String Text = String.Empty;
            List<Transaction> transactionDate = await _context.Transactions.Where(p => p.Date >= invoiceDates.entryDate && p.Date <= invoiceDates.endDate ).ToListAsync();

            foreach(Transaction i in transactionDate)
            {
                i.BillStatus = "billed";
                i.PaymentStatus = "paid";
                _context.Transactions.Update(i);
                Text += "Id: " + i.Id.ToString() + " Amount:" + i.Amount.ToString() + "$ Status:" + i.BillStatus + " Date:" + i.Date.ToLongDateString() + " Description:" + i.Description;
                
            }
            await _context.SaveChangesAsync();


            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, invoiceDates.entryDate.Day.ToString() + invoiceDates.entryDate.Month.ToString() + invoiceDates.entryDate.Year.ToString() + "-" + invoiceDates.endDate.Day.ToString() + invoiceDates.endDate.Month.ToString() + invoiceDates.endDate.Year.ToString() +  "invoice.txt")))
            {
                await outputFile.WriteAsync(Text);
            }

            return Ok($"Invoice Created");
        }
    }
}
