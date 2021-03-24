using Intranet.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using SoaApp.Utilities;
using System;
using System.Linq;

namespace SoaApp.Controllers
{
    public class SoaController : Controller
    {
        private readonly SOADbContext _context;

        public SoaController(SOADbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GenerateSoa(string customer, int company, DateTime asof)
        {
            ClearAmounts();

            #region Date management for SOA

            //First day of Current Month
            var DateToday = asof;
            SD.GuiAsOf = asof;
            var FirstDateOfCurrentMonth = new DateTime(DateToday.Year, DateToday.Month, 1);
            var PreviousMonthFirstDay = FirstDateOfCurrentMonth.AddMonths(-1);
            var PreviousMonthLastDay = DateTime.DaysInMonth(DateToday.Year, PreviousMonthFirstDay.Month);

            //Get last day of Previews month
            var PreviewsBalanceDate = PreviousMonthFirstDay.Month.ToString() + "/" +
                PreviousMonthLastDay.ToString() + "/" +
                PreviousMonthFirstDay.Year.ToString();

            #endregion Date management for SOA

            SD.GuiCompany = company.ToString();
            SD.GuiCustomerNum = customer.ToString();

            #region Stable Data

            #region Address of Business

            //Get Address Code
            var t001 = _context.T001s.Where(i => i.BURKS == SD.GuiCompany);
            foreach (var comp in t001)
            {
                SD.AddressCode = comp.ADRNR;
                SD.CompanyTinNum = comp.STCEG;
            }

            #endregion Address of Business

            #region Business Details

            //Company Details
            var adrc = _context.ADRCs.Where(i => i.ADDRNUMBER == SD.AddressCode);
            foreach (var compa in adrc)
            {
                SD.CompanyName = compa.NAME1;
                SD.CompanyTellNum = compa.TEL_NUMBER;
                SD.CompanyCity = compa.CITY1;
                SD.CompanyStreet = compa.STREET + " ";
            }

            #endregion Business Details

            #region Customer Details

            //get Customer details
            var kna1 = _context.KNA1s.Where(i => i.KUNNR == SD.GuiCustomerNum);
            foreach (var cust in kna1)
            {
                SD.CustomerNum = cust.KUNNR;
                SD.CustomerName = cust.NAME1;
                SD.CustomerCity = cust.ORT01;
                SD.CustomerStreet = cust.STRAS + " ";
            }

            //Get Customer Number and Attention To
            var knvk = _context.KNVKs.Where(i => i.KUNNR == SD.GuiCustomerNum);
            foreach (var custo in knvk)
            {
                SD.CustomerNum = custo.KUNNR;
                SD.AttentionTo = custo.NAME1;
            }

            #endregion Customer Details

            #endregion Stable Data

            // <-------------------------------------------------->

            #region Adding items in tables

            #region Payments

            //var bsid_bsad = _context.BSIDs
            //    .Where(i =>
            //            (i.UMSKZ == "" || i.UMSKZ != "C") &&
            //            (
            //                i.BLART == "DX" ||
            //                i.BLART == "DY" ||
            //                i.BLART == "DZ"
            //            ))
            //    .OrderByDescending(i => i.ZFBDT);

            //var bsid_bsad2 = _context.BSADs
            //    .Where(i =>
            //            (i.UMSKZ == "" || i.UMSKZ != "C") &&
            //            (
            //                i.BLART == "DX" ||
            //                i.BLART == "DY" ||
            //                i.BLART == "DZ"
            //            ))
            //    .OrderByDescending(i => i.ZFBDT);

            //ViewBag.BSID_PAYMENTS1 = bsid_bsad;
            //ViewBag.BSID_PAYMENTS2 = bsid_bsad2;

            #endregion Payments

            #endregion Adding items in tables

            #region Computation

            #region Payments

            //var bsid_bsad_amount = _context.BSIDs
            //    .Where(i =>
            //            (i.UMSKZ == "" || i.UMSKZ != "C") &&
            //            (
            //                i.BLART == "DX" ||
            //                i.BLART == "DY" ||
            //                i.BLART == "DZ"
            //            ));

            //var bsid_bsad_amount2 = _context.BSADs
            //    .Where(i =>
            //            (i.UMSKZ == "" || i.UMSKZ != "C") &&
            //            (
            //                i.BLART == "DX" ||
            //                i.BLART == "DY" ||
            //                i.BLART == "DZ"
            //            ));

            //foreach (var item in bsid_bsad_amount)
            //{
            //    SD.TotalPayments += Convert.ToDouble(item.DMBTR);
            //}

            //foreach (var item in bsid_bsad_amount2)
            //{
            //    SD.TotalPayments += Convert.ToDouble(item.DMBTR);
            //}

            #endregion Payments

            #endregion Computation

            // <-------------------------------------------------->

            #region New Process

            #region Current and Previous Bill

            var current_bill = _context.BSIDs
                .Where(i => (i.UMSKZ == "" || i.UMSKZ != "C") && 
                            (i.KUNNR == SD.CustomerNum)
                      );

            // Current Bills
            foreach (var bill in current_bill)
            {
                SD.DateBudat = Convert.ToDateTime(bill.BUDAT);
                if (SD.DateBudat >= FirstDateOfCurrentMonth && SD.DateBudat <= asof)
                {
                    if ((bill.SHKZG == "S" && bill.XZAHL == "") || (bill.SHKZG == "S" && bill.XZAHL == "X"))
                    {
                        SD.CurrentBillsTotal += Convert.ToDouble(bill.WRBTR);
                    }
                    else
                    {
                        SD.CurrentBillsTotal -= Convert.ToDouble(bill.WRBTR);
                    }
                }
            }

            foreach (var bill in current_bill)
            {
                SD.DateBudat = Convert.ToDateTime(bill.BUDAT);
                if (SD.DateBudat <= Convert.ToDateTime(PreviewsBalanceDate))
                {
                    if ((bill.SHKZG == "S" && bill.XZAHL == "") || (bill.SHKZG == "S" && bill.XZAHL == "X"))
                    {
                        SD.PreviousBillsTotal += Convert.ToDouble(bill.WRBTR);
                    }
                    else
                    {
                        SD.PreviousBillsTotal -= Convert.ToDouble(bill.WRBTR);
                    }
                }
            }

            #endregion Current Bill

            #region Unpaid table

            //getting Unpaid items
            var unpaid_items = _context.BSIDs.Where(i => (i.UMSKZ == "" || i.UMSKZ != "C") &&
                                                            (i.KUNNR == SD.CustomerNum) &&
                                                            !(i.BLART == "P8" ||
                                                                i.BLART == "DG" ||
                                                                i.BLART == "DJ" ||
                                                                i.BLART == "DH" ||
                                                                i.BLART == "DM")).OrderByDescending(i => i.ZFBDT);

            // put list of cwt items to the webpage
            ViewBag.BSID_UNPAID = unpaid_items;

            // sum of all unpaid transaction
            foreach (var unpaid in unpaid_items)
            {
                if ((unpaid.SHKZG == "S" && unpaid.XZAHL == "") || (unpaid.SHKZG == "S" && unpaid.XZAHL == "X"))
                {
                    SD.UPAmount += Convert.ToDouble(unpaid.DMBTR);
                }
                else
                {
                    SD.PAmount += Convert.ToDouble(unpaid.DMBTR);
                }
            }


            SD.UPTotalAmount = SD.UPAmount - SD.PAmount;

            #endregion Unpaid table

            #region Credit and Debit table

            // get all credit and debit from table
            var bsid_cmdm = _context.BSIDs.Where(i => (i.UMSKZ == "" || i.UMSKZ != "C") &&
                                                        (i.KUNNR == SD.CustomerNum) &&
                                                        (i.BLART == "P8" ||
                                                            i.BLART == "DG" ||
                                                            i.BLART == "DJ" ||
                                                            i.BLART == "DH" ||
                                                            i.BLART == "DM"));
            // put list of cwt items to the webpage
            ViewBag.BSID_CMDM = bsid_cmdm;

            // get all credit items
            var bsid_credit = _context.BSIDs.Where(i =>
                                    (i.UMSKZ == "" || i.UMSKZ != "C") &&
                                    (i.KUNNR == SD.CustomerNum) &&
                                    (i.SHKZG == "H") &&
                                    (i.BLART == "P8" ||
                                        i.BLART == "DG" ||
                                        i.BLART == "DJ"));

            // sum of all credit
            foreach (var credit in bsid_credit)
            {
                SD.CreditTotalAmount += Convert.ToDouble(credit.DMBTR);
            }

            // get all debit items
            var bsid_debit = _context.BSIDs.Where(i =>
                                    (i.UMSKZ == "" || i.UMSKZ != "C") &&
                                    (i.KUNNR == SD.CustomerNum) &&
                                    (i.SHKZG == "S") &&
                                    (i.BLART == "DH" ||
                                        i.BLART == "DM"));
            // sum of all debit items
            foreach (var debit in bsid_debit)
            {
                SD.DebitTotalAmount += Convert.ToDouble(debit.DMBTR);
            }

            // debit and credit total = debit amount - credit amount
            SD.DebitCreditTotalAmount = SD.DebitTotalAmount - SD.CreditTotalAmount;

            #endregion Credit and Debit table

            #region CWT Table

            // get all cwt from database
            var bsid_cwt = _context.BSIDs.Where(i => (i.UMSKZ == "C") && (i.KUNNR == SD.CustomerNum));

            // put list of cwt items to the webpage
            ViewBag.BSID_CWT = bsid_cwt;

            // get the total amount of cwt
            foreach (var cwt in bsid_cwt)
            {
                SD.WHTotalAmount += Convert.ToDouble(cwt.DMBTR);
            }

            #endregion CWT Table

            #region Preview Bill

            #endregion

            #region TotalAmount

            SD.TotalAmountDue = ((SD.UPTotalAmount + SD.WHTotalAmount) + SD.DebitCreditTotalAmount) - SD.TotalPayments;

            #endregion

            #endregion New Process

            return View();
        }

        public void ClearAmounts()
        {
            SD.WHTotalAmount = 0;
            SD.UPAmount = 0;
            SD.PAmount = 0;
            SD.UPTotalAmount = 0;
            SD.TotalAmountDue = 0;
            SD.DebitTotalAmount = 0;
            SD.CreditTotalAmount = 0;
            SD.DebitCreditTotalAmount = 0;
            SD.TotalPayments = 0;
            SD.CurrentBillsTotal = 0;
            SD.PreviousBillsTotal = 0;

            SD.GuiCompany = "";
            SD.GuiCustomerNum = "";
            SD.AddressCode = "";
            SD.CompanyTinNum = "";
            SD.CompanyName = "";
            SD.CompanyTellNum = "";
            SD.CompanyCity = "";
            SD.CompanyStreet = "";

            SD.CustomerName = "";
            SD.CustomerCity = "";
            SD.CustomerStreet = "";

            SD.CustomerNum = "";
            SD.AttentionTo = "";
        }
    }
}