using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using SoaApp.Models;
using SoaApp.Models.ViewModels;
using SoaApp.Repository.IRepository;
using SoaApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoaApp.Controllers
{
    public class SoaController : Controller
    {
        private readonly IBapiRepository _bapi;

        [BindProperty]
        public SoaVM SoaVM { get; set; }

        public SoaController(IBapiRepository bapi)
        {
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

            #region Soa Parameters

            SoaVM.Posting_Date = asof.ToShortDateString();      // setting AS OF DATE
            SoaVM.Customer_Number = customer;                   // setting CUSTOMER NO
            SoaVM.Company_Code = company;                       // setting COMPANY CODE

            // setting Previous Month Lastday
            SoaVM.PreviewMonthLastDay = DateMangement.GetPreviousMonthLastDay(
                                            Convert.ToDateTime(SoaVM.Posting_Date))
                                            .ToShortDateString();

            // setting current perion first day
            SoaVM.Date_From = DateMangement.GetFirstDayOfCurrentMonth(
                                                Convert.ToDateTime(SoaVM.Posting_Date))
                                                .ToShortDateString();

            #endregion Soa Parameters

            #region API Calls

            SoaVM.OpenItemsAsOfDate = GetOpenItemsAsOfDate();
            SoaVM.OpenItemsPreviousMonth = GetOpenItemsPreviousMonth();
            SoaVM.StatementCurrentMonth = GetStatementCurrentMonth();

            #endregion API Calls

            #region Previous Balance

            SoaVM.PreviousBalance = GetPreviousBalance();  // Get Previous Balance

            #endregion Previous Balance

            #region Current Billings

            SoaVM.TotalCurrentBillings = GetCurrentBillings();

            #endregion Current Billings

            #region Payments

            SoaVM.PaymentsListItem = GetPaymentsList();     // setting Payment Lists
            SoaVM.TotalPayments = GetTotalPayments();       // setting  total payments

            #endregion Payments

            #region Unpaid

            SoaVM.UnpaidListItem = GetUnpaidItems();
            SoaVM.TotalUnpaid = GetUnpaidTotal();

            #endregion Unpaid

            #region Credit And Debit

            SoaVM.CreditAndDebitList = GetCreditAndDebitList();

            SoaVM.TotalCreditAndDebit = GetTotalCreditAndDebit();

            #endregion Credit And Debit

            #region CWT

            SoaVM.UncollectedCwtList = GetUncollectedCwtList();         // setting list of uncollected CWT
            SoaVM.TotalUncollectedCwtPhp = GetTotalUncollectedCwtPhp();   // setting total uncollected CWT
            SoaVM.TotalUncollectedCwtUsd = GetTotalUncollectedCwtUsd();   // setting total uncollected CWT

            #endregion CWT

            return View(SoaVM);
        }

        //private void GetCompanyDetails(int companyCode)
        //{
        //    //Get Address Code
        //    var t001 = _context.T001s.Where(i => i.BURKS == Convert.ToString(companyCode));

        //    var addressCode = "";
        //    foreach (var comp in t001)
        //    {
        //        addressCode = comp.ADRNR;
        //        SoaVM.Company.TinNo = comp.STCEG;
        //    }

        //    //Company Details
        //    var adrc = _context.ADRCs.Where(i => i.ADDRNUMBER == addressCode);
        //    foreach (var compa in adrc)
        //    {
        //        SoaVM.Company.Name = compa.NAME1;
        //        SoaVM.Company.TelNo = compa.TEL_NUMBER;
        //        SoaVM.Company.StreetAddress = compa.STREET + " ";
        //        SoaVM.Company.CityAddress = compa.CITY1;
        //    }
        //}

        //private void GetCustomerDetails(string customerNumber)
        //{
        //    //get Customer details
        //    var kna1 = _context.KNA1s.Where(i => i.KUNNR == customerNumber);

        //    foreach (var cust in kna1)
        //    {
        //        SoaVM.Customer.Code = cust.KUNNR;
        //        SoaVM.Customer.Name = cust.NAME1;
        //        SoaVM.Customer.StreetAddress = cust.STRAS + " ";
        //        SoaVM.Customer.CityAddress = cust.ORT01;
        //    }

        //    //Get Customer Number and Attention To
        //    var knvk = _context.KNVKs.Where(i => i.KUNNR == customerNumber);

        //    foreach (var custo in knvk)
        //    {
        //        SoaVM.Customer.AttentionTo = custo.NAME1;
        //    }
        //}

        #region API Calls

        private IEnumerable<BapiOpenItemDto> GetOpenItemsAsOfDate()
        {
            SoaParams soaParams = new SoaParams
            {
                Customer_Number = SoaVM.Customer_Number,
                Company_Code = SoaVM.Company_Code,
                Posting_Date = SoaVM.Posting_Date
            };

            return _bapi.GetResponse(soaParams, SD.ApiUriOpenItems);
        }

        private IEnumerable<BapiOpenItemDto> GetOpenItemsPreviousMonth()
        {
            SoaParams soaParams = new SoaParams
            {
                Customer_Number = SoaVM.Customer_Number,
                Company_Code = SoaVM.Company_Code,
                Posting_Date = SoaVM.PreviewMonthLastDay
            };

            return _bapi.GetResponse(soaParams, SD.ApiUriOpenItems);
        }

        private IEnumerable<BapiOpenItemDto> GetStatementCurrentMonth()
        {
            SoaParams soaParams = new SoaParams
            {
                Customer_Number = SoaVM.Customer_Number,
                Company_Code = SoaVM.Company_Code,
                Date_From = SoaVM.Date_From,
                Date_To = SoaVM.Posting_Date
            };

            return _bapi.GetResponse(soaParams, SD.ApiUriStatement);
        }

        #endregion API Calls

        #region Previews Balance

        private double GetPreviousBalance()
        {
            var soaPrevious = SoaVM.OpenItemsPreviousMonth;

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

        #endregion Previews Balance

        #region Current Billings

        private double GetCurrentBillings()
        {
            var currentBilling = SoaVM.StatementCurrentMonth
                                    .Where(x =>
                                            (x.DB_CR_IND == "S") &&
                                            !(x.SP_GL_IND == "C"));
            double TotalCurrentBills = 0;

            foreach (var item in currentBilling)
            {
                if (item.DB_CR_IND == "S")
                {
                    TotalCurrentBills += item.LC_AMOUNT;
                }
                else
                {
                    TotalCurrentBills -= item.LC_AMOUNT;
                }
            }

            return TotalCurrentBills;
        }

        #endregion Current Billings

        #region Payments

        private IEnumerable<BapiOpenItemDto> GetPaymentsList()
        {
            var openitems = SoaVM.OpenItemsAsOfDate
                    .Where(x =>
                        !(x.SP_GL_IND == "C") &&
                         (x.DOC_TYPE == "DX" || x.DOC_TYPE == "DY" || x.DOC_TYPE == "DZ"));

            var balanceditem = SoaVM.OpenItemsPreviousMonth
                                .Where(x =>
                                    !(x.SP_GL_IND == "C") &&
                                     (x.DOC_TYPE == "DX" || x.DOC_TYPE == "DY" || x.DOC_TYPE == "DZ"));

            List<BapiOpenItemDto> allPayments = new List<BapiOpenItemDto>().Distinct().ToList();

            foreach (var item in openitems)
            {
                allPayments.Add(item);
            }

            foreach (var item in balanceditem)
            {
                allPayments.Add(item);
            }

            var distPayments = allPayments.GroupBy(x => x.DOC_NO).Select(y => y.FirstOrDefault());

            return distPayments;
        }

        private double GetTotalPayments()
        {
            var totalPaymentsList = SoaVM.PaymentsListItem;

            double TotalPayments = 0;

            foreach (var payment in totalPaymentsList)
            {
                if (payment.DB_CR_IND == "S")
                {
                    TotalPayments += Convert.ToDouble(payment.LC_AMOUNT);
                }
                else
                {
                    TotalPayments -= Convert.ToDouble(payment.LC_AMOUNT);
                }
            }

            return TotalPayments;
        }

        #endregion Payments

        #region Unpaid Transaction

        private IEnumerable<BapiOpenItemDto> GetUnpaidItems()
        {
            return SoaVM.OpenItemsAsOfDate
                .Where(y =>
                    !(y.SP_GL_IND == "C") &&
                    !(y.DOC_TYPE == "P8" ||
                        y.DOC_TYPE == "P9" ||
                        y.DOC_TYPE == "DG" ||
                        y.DOC_TYPE == "DJ" ||
                        y.DOC_TYPE == "DM" ||
                        y.DOC_TYPE == "DH" ||
                        y.DOC_TYPE == "PA" ||
                        y.DOC_TYPE == "PB"))
                .OrderByDescending(x => x.ENTRY_DATE);
        }

        private double GetUnpaidTotal()
        {
            var totalunpaid = SoaVM.UnpaidListItem
                                .OrderByDescending(x => x.ENTRY_DATE)
                                .Where(y =>
                                  !(y.SP_GL_IND == "C") &&
                                  !(y.DOC_TYPE == "P8" ||
                                        y.DOC_TYPE == "P9" ||
                                        y.DOC_TYPE == "DG" ||
                                        y.DOC_TYPE == "DJ" ||
                                        y.DOC_TYPE == "DM" ||
                                        y.DOC_TYPE == "DH" ||
                                        y.DOC_TYPE == "PA" ||
                                        y.DOC_TYPE == "PB"
                                    ));
            double unpaids = 0;

            foreach (var unpaid in totalunpaid)
            {
                if (unpaid.DB_CR_IND == "S")
                {
                    unpaids += Convert.ToDouble(unpaid.LC_AMOUNT);
                }
                else
                {
                    unpaids -= Convert.ToDouble(unpaid.LC_AMOUNT);
                }
            }

            return unpaids;
        }

        #endregion Unpaid Transaction

        #region Credit and Debit

        private IEnumerable<BapiOpenItemDto> GetCreditAndDebitList()
        {
            SoaParams soaParams = new SoaParams
            {
                Customer_Number = SoaVM.Customer_Number,
                Company_Code = SoaVM.Company_Code,
                Date_From = SoaVM.Date_From,
                Date_To = SoaVM.Posting_Date
            };

            return SoaVM.StatementCurrentMonth
                .Where(x => x.DOC_TYPE == "P8" ||
                            x.DOC_TYPE == "P9" ||
                            x.DOC_TYPE == "DG" ||
                            x.DOC_TYPE == "DJ" ||
                            x.DOC_TYPE == "DM" ||
                            x.DOC_TYPE == "DH" ||
                            x.DOC_TYPE == "PA" ||
                            x.DOC_TYPE == "PB");
        }

        private double GetTotalCreditAndDebit()
        {
            var soaStatement = SoaVM.CreditAndDebitList
                                .Where(x => x.DOC_TYPE == "P8" ||
                                            x.DOC_TYPE == "P9" ||
                                            x.DOC_TYPE == "DG" ||
                                            x.DOC_TYPE == "DJ" ||
                                            x.DOC_TYPE == "DM" ||
                                            x.DOC_TYPE == "DH" ||
                                            x.DOC_TYPE == "PA" ||
                                            x.DOC_TYPE == "PB");
            double totalcmdm = 0;

            foreach (var cmDm in soaStatement)
            {
                if (cmDm.DB_CR_IND == "S")
                {
                    totalcmdm += Convert.ToDouble(cmDm.LC_AMOUNT);
                }
                else
                {
                    totalcmdm -= Convert.ToDouble(cmDm.LC_AMOUNT);
                }
            }

            return totalcmdm;
        }

        #endregion Credit and Debit

        #region Uncollected CWT

        private IEnumerable<BapiOpenItemDto> GetUncollectedCwtList()
        {
            return SoaVM.OpenItemsAsOfDate
                .Where(x => x.SP_GL_IND == "C").OrderByDescending(e => e.ENTRY_DATE);
        }   // Getting the list of Uncollected Cwt

        private double GetTotalUncollectedCwtUsd()
        {
            var cwtList = SoaVM.UncollectedCwtList.Where(x => (x.SP_GL_IND == "C") && (x.CURRENCY == "USD"));

            double totalcwt = 0;

            foreach (var cwt in cwtList)
            {
                if (cwt.DB_CR_IND == "S")
                {
                    totalcwt += Convert.ToDouble(cwt.LC_AMOUNT);
                }
                else
                {
                    totalcwt -= Convert.ToDouble(cwt.LC_AMOUNT);
                }
            }

            return totalcwt;
        }

        private double GetTotalUncollectedCwtPhp()
        {
            var cwtList = SoaVM.UncollectedCwtList.Where(x => (x.SP_GL_IND == "C") && (x.CURRENCY == "PHP"));

            double totalcwt = 0;

            foreach (var cwt in cwtList)
            {
                if (cwt.DB_CR_IND == "S")
                {
                    totalcwt += Convert.ToDouble(cwt.LC_AMOUNT);
                }
                else
                {
                    totalcwt -= Convert.ToDouble(cwt.LC_AMOUNT);
                }
            }

            return totalcwt;
        }       // Getting the total uncollected cwt

        #endregion Uncollected CWT
    }
}