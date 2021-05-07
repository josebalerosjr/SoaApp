using Intranet.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using SoaApp.Models.ViewModels;
using SoaApp.Repository.IRepository;
using SoaApp.Utilities;
using System;
using System.Linq;

namespace SoaApp.Controllers
{
    public class SoaController : Controller
    {
        private readonly SOADbContext _context;
        private readonly IBapiRepository _bapi;

        [BindProperty]
        public SoaVM SoaVM { get; set; }

        public SoaController(SOADbContext context, IBapiRepository bapi)
        {
            _context = context;
            _bapi = bapi;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GenerateSoa(string customer, string company, DateTime asof)
        {
            SoaVM = new SoaVM()
            {
                Customer = new Models.Customer(),
                Company = new Models.Company()
            };

            SoaVM.Posting_Date = asof.ToShortDateString();
            SoaVM.Customer_Number = customer;
            SoaVM.Company_Code = company;
            SoaVM.PreviewMonthLastDay = DateMangement.GetPreviousMonthLastDay(Convert.ToDateTime(SoaVM.Posting_Date)).ToShortDateString();

            // Get Previous Balance
            SoaVM.PreviousBalance = GetPreviousBalance(SoaVM, SoaVM.PreviewMonthLastDay);



            return View(SoaVM);
        }

        private double GetPreviousBalance(SoaVM soaVM, string date)
        {
            var soaPrevious = _bapi.GetBapi(soaVM, date);

            double totalbalance = 0;

            foreach (var item in soaPrevious)
            {
                if (item.DB_CR_IND == "S")
                {
                    totalbalance += Convert.ToDouble(item.LC_AMOUNT);
                }
                else
                {
                    totalbalance -= Convert.ToDouble(item.LC_AMOUNT);
                }
            }

            return totalbalance;
        }

        private void GetTotalAmountDue()
        {
            SD.TotalAmountDue = ((SD.UpTotalAmount + SD.WhTotalAmount) + SD.DebitCreditTotalAmount) - SD.TotalPayments;
        }

        private void GetCurrentPreviousBill(DateTime asof)
        {
            var currentPrevBill = _context.BSIDNews
                .Where(i => i.KUNNR == SoaVM.Customer.Code);

            // Current Bills
            foreach (var bill in currentPrevBill)
            {
                if (bill.BUDAT >= DateMangement.GetFirstDayOfCurrentMonth(asof) && bill.BUDAT <= asof)
                {
                    if (bill.SHKZG == "S")
                    {
                        SD.CurrentBillsTotal += Convert.ToDouble(bill.WRBTR);
                    }
                    else
                    {
                        SD.CurrentBillsTotal -= Convert.ToDouble(bill.WRBTR);
                    }
                }
            }

            foreach (var bill in currentPrevBill)
            {
                if (bill.BUDAT <= DateMangement.GetPreviousMonthLastDay(asof))
                {
                    if (bill.SHKZG == "S")
                    {
                        SD.PreviousBillsTotal += Convert.ToDouble(bill.WRBTR);
                    }
                    else
                    {
                        SD.PreviousBillsTotal -= Convert.ToDouble(bill.WRBTR);
                    }
                }
            }
            //SD.PreviousBillsTotal += SD.WhTotalAmount;
            //SD.PreviousBillsTotal += SD.WhTotalAmount;
        }

        private void GetCreditDebitDetails()
        {
            // get all credit and debit from table
            var bsidCmdm = _context.BSIDs.Where(i => (i.UMSKZ == "" || i.UMSKZ != "C") &&
                                                        (i.KUNNR == SoaVM.Customer.Code) &&
                                                        (i.BLART == "P8" ||
                                                            i.BLART == "DG" ||
                                                            i.BLART == "DJ" ||
                                                            i.BLART == "DH" ||
                                                            i.BLART == "DM"));
            // put list of cwt items to the webpage
            ViewBag.BSID_CMDM = bsidCmdm;

            // get all credit items
            var bsidCredit = _context.BSIDs.Where(i =>
                                    (i.UMSKZ == "" || i.UMSKZ != "C") &&
                                    (i.KUNNR == SoaVM.Customer.Code) &&
                                    (i.SHKZG == "H") &&
                                    (i.BLART == "P8" ||
                                        i.BLART == "DG" ||
                                        i.BLART == "DJ"));

            // sum of all credit
            foreach (var credit in bsidCredit)
            {
                SD.CreditTotalAmount += Convert.ToDouble(credit.DMBTR);
            }

            // get all debit items
            var bsidDebit = _context.BSIDs.Where(i =>
                                    (i.UMSKZ == "" || i.UMSKZ != "C") &&
                                    (i.KUNNR == SoaVM.Customer.Code) &&
                                    (i.SHKZG == "S") &&
                                    (i.BLART == "DH" || i.BLART == "DM"));

            // sum of all debit items
            foreach (var debit in bsidDebit)
            {
                SD.DebitTotalAmount += Convert.ToDouble(debit.DMBTR);
            }

            // debit and credit total = debit amount - credit amount
            SD.DebitCreditTotalAmount = SD.DebitTotalAmount - SD.CreditTotalAmount;
        }

        private void GetUnpaidDetails()
        {
            //getting Unpaid items
            var unpaidItems = _context.BSIDs.Where(i => (i.UMSKZ == "" || i.UMSKZ != "C") &&
                                                        (i.KUNNR == SoaVM.Customer.Code) &&
                                                        !(i.BLART == "P8" ||
                                                          i.BLART == "DG" ||
                                                          i.BLART == "DJ" ||
                                                          i.BLART == "DH" ||
                                                          i.BLART == "DM")).OrderByDescending(i => i.ZFBDT);

            // put list of cwt items to the webpage
            ViewBag.BSID_UNPAID = unpaidItems;

            // sum of all unpaid transaction
            foreach (var unpaid in unpaidItems)
            {
                if (unpaid.SHKZG == "S")
                {
                    SD.UpAmount += Convert.ToDouble(unpaid.DMBTR);
                }
                else
                {
                    SD.PAmount += Convert.ToDouble(unpaid.DMBTR);
                }
            }

            SD.UpTotalAmount = SD.UpAmount - SD.PAmount;
        }

        private void GetCwtDetails()
        {
            // get all cwt from database
            var bsidCwt = _context.BSIDs.Where(i => (i.UMSKZ == "C") && (i.KUNNR == SoaVM.Customer.Code));

            // put list of cwt items to the webpage
            ViewBag.BSID_CWT = bsidCwt;

            // get the total amount of cwt
            foreach (var cwt in bsidCwt)
            {
                if (cwt.SHKZG == "H")
                {
                    SD.WhPaidAmount += Convert.ToDouble(cwt.DMBTR);
                }
                else
                {
                    SD.WhUnpaidAmount += Convert.ToDouble(cwt.DMBTR);
                }
                SD.WhTotalAmount = SD.WhUnpaidAmount - SD.WhPaidAmount;
            }
        }

        private void PaymentComputation()
        {
            var bsidBsad = _context.Payments
                .Where(i =>
                        (i.UMSKZ == "" || i.UMSKZ != "C") &&
                        (i.KUNNR == SoaVM.Customer.Code) &&
                        (
                            i.BLART == "DX" ||
                            i.BLART == "DY" ||
                            i.BLART == "DZ"
                        ));

            foreach (var item in bsidBsad)
            {
                if (item.SHKZG == "H")
                {
                    SD.UnpaidPayments += Convert.ToDouble(item.DMBTR);
                }
                else
                {
                    SD.PaidPayments += Convert.ToDouble(item.DMBTR);
                }
            }

            SD.TotalPayments = SD.UnpaidPayments - SD.PaidPayments;
        }

        private static void ClearVariables()
        {
            SD.WhTotalAmount = 0;
            SD.UpAmount = 0;
            SD.PAmount = 0;
            SD.UpTotalAmount = 0;
            SD.TotalAmountDue = 0;
            SD.DebitTotalAmount = 0;
            SD.CreditTotalAmount = 0;
            SD.DebitCreditTotalAmount = 0;
            SD.PaidPayments = 0;
            SD.UnpaidPayments = 0;
            SD.TotalPayments = 0;
            SD.CurrentBillsTotal = 0;
            SD.PreviousBillsTotal = 0;
            SD.DollarTotal = 0;
            SD.WhPaidAmount = 0;
            SD.WhUnpaidAmount = 0;
        }

        #region Final Functions

        private void GetCompanyDetails(int companyCode)
        {
            //Get Address Code
            var t001 = _context.T001s.Where(i => i.BURKS == Convert.ToString(companyCode));

            var addressCode = "";
            foreach (var comp in t001)
            {
                addressCode = comp.ADRNR;
                SoaVM.Company.TinNo = comp.STCEG;
            }

            //Company Details
            var adrc = _context.ADRCs.Where(i => i.ADDRNUMBER == addressCode);
            foreach (var compa in adrc)
            {
                SoaVM.Company.Name = compa.NAME1;
                SoaVM.Company.TelNo = compa.TEL_NUMBER;
                SoaVM.Company.StreetAddress = compa.STREET + " ";
                SoaVM.Company.CityAddress = compa.CITY1;
            }
        }

        private void GetCustomerDetails(string customerNumber)
        {
            //get Customer details
            var kna1 = _context.KNA1s.Where(i => i.KUNNR == customerNumber);

            foreach (var cust in kna1)
            {
                SoaVM.Customer.Code = cust.KUNNR;
                SoaVM.Customer.Name = cust.NAME1;
                SoaVM.Customer.StreetAddress = cust.STRAS + " ";
                SoaVM.Customer.CityAddress = cust.ORT01;
            }

            //Get Customer Number and Attention To
            var knvk = _context.KNVKs.Where(i => i.KUNNR == customerNumber);

            foreach (var custo in knvk)
            {
                SoaVM.Customer.AttentionTo = custo.NAME1;
            }
        }

        #endregion Final Functions
    }
}