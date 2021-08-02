using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using SoaApp.Dtos;
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
        private readonly ISoaDetailsRepository _soaDetails;
        private readonly ICurrencyChecker _checker;

        [BindProperty]
        public SoaVM SoaVM { get; set; }

        public SoaController(IBapiRepository bapi, ISoaDetailsRepository soaDetails, ICurrencyChecker checker)
        {
            _bapi = bapi;
            _soaDetails = soaDetails;
            _checker = checker;
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
            SoaVM.SoaDetails = GetCompanyCustomerDetails();

            #endregion API Calls

            #region Company and Customer Details

            GetCompanyCustomerDetailsToVM();

            #endregion Company and Customer Details

            #region Currency Checker

            SoaVM.UsdChecker = _checker.UsdChecker(SoaVM.OpenItemsAsOfDate);
            SoaVM.PhpChecker = _checker.PhpChecker(SoaVM.OpenItemsAsOfDate);

            #endregion Currency Checker

            #region Previous Balance

            SoaVM.PreviousBalancePhp = GetPreviousBalancePhp();  // Get Previous Balance PHP
            SoaVM.PreviousBalanceUsd = GetPreviousBalanceUsd();  // Get Previous Balance USD

            #endregion Previous Balance

            #region Current Billings

            SoaVM.TotalCurrentBillingsUsd = GetCurrentBillingsUsd();
            SoaVM.TotalCurrentBillingsPhp = GetCurrentBillingsPhp();

            #endregion Current Billings

            #region Payments

            SoaVM.PaymentsListItem = GetPaymentsList();     // setting Payment Lists
            SoaVM.TotalPaymentsPhp = GetTotalPaymentsPhp();       // setting  total payments
            SoaVM.TotalPaymentsUsd = GetTotalPaymentsUsd();       // setting  total payments

            #endregion Payments

            #region Unpaid

            SoaVM.UnpaidListItem = GetUnpaidItems();
            SoaVM.TotalUnpaidUsd = GetUnpaidTotalUsd();
            SoaVM.TotalUnpaidPhp = GetUnpaidTotalPhp();

            #endregion Unpaid

            #region Credit And Debit

            SoaVM.CreditAndDebitList = GetCreditAndDebitList();
            SoaVM.TotalCreditAndDebitUsd = GetTotalCreditAndDebitUsd();
            SoaVM.TotalCreditAndDebitPhp = GetTotalCreditAndDebitPhp();

            #endregion Credit And Debit

            #region CWT

            SoaVM.UncollectedCwtList = GetUncollectedCwtList();         // setting list of uncollected CWT
            SoaVM.TotalUncollectedCwtPhp = GetTotalUncollectedCwtPhp();   // setting total uncollected CWT
            SoaVM.TotalUncollectedCwtUsd = GetTotalUncollectedCwtUsd();   // setting total uncollected CWT

            #endregion CWT

            #region TotalSOA

            SoaVM.TotalSOAUsd = (SoaVM.TotalUncollectedCwtUsd) + (SoaVM.TotalCreditAndDebitUsd) + (SoaVM.TotalUnpaidUsd);
            SoaVM.TotalSOAPhp = (SoaVM.TotalUncollectedCwtPhp) + (SoaVM.TotalCreditAndDebitPhp) + (SoaVM.TotalUnpaidPhp);

            #endregion TotalSOA

            return View(SoaVM);
        }

        #region Testing

        private IEnumerable<SoaDetailsDto> GetPaymentDetails()
        {
            return SoaVM.SoaDetails;
        }

        private IEnumerable<BapiOpenItemDto> GetPaymentClean()
        {
            // list of unique rows
            var data = SoaVM.StatementCurrentMonth
                .Where(x =>
                        !(x.SP_GL_IND == "C") &&
                         (x.DOC_TYPE == "DX" || x.DOC_TYPE == "DY" || x.DOC_TYPE == "DZ"))
                .Distinct().ToList();

            // list of items with duplicates
            var dupe = from list in data
                       group list by list.DOC_NO into grouped
                       where grouped.Count() > 1
                       select grouped.ToList();

            List<BapiOpenItemDto> noDuplicates = new List<BapiOpenItemDto>(data).Distinct().ToList();

            List<BapiOpenItemDto> duplicateItems = new List<BapiOpenItemDto>().Distinct().ToList();

            foreach (var item in dupe)
            {
                foreach (var additem in item)
                {
                    duplicateItems.Add(additem);
                }
            }

            foreach (var item in duplicateItems)
            {
                noDuplicates.Remove(item);
            }

            var splitted = duplicateItems.GroupBy(x => x.DOC_NO.ToList()).ToList();

            ////  initialize payment
            //List<BapiOpenItemDto> payments = new List<BapiOpenItemDto>();

            //// initialized distinct items
            //List<BapiOpenItemDto> nodupeitems = new List<BapiOpenItemDto>().Distinct().ToList();

            //// foreach loop on the data and add to nodupeitems list all distinctitems
            //foreach (var item in data)
            //{
            //    nodupeitems.Add(item);

            //}

            //// foreach dupeitems and get the dupe ones
            //foreach (var list in nodupeitems)
            //{
            //    // compare
            //    foreach (var list2 in data)
            //    {
            //        if ((list.DOC_NO == list2.DOC_NO) && !(list.DB_CR_IND == list2.DB_CR_IND))
            //        {
            //            payments.Add(list2);
            //            itemstodelete = list2.DOC_NO;
            //        }
            //    }
            //}

            //// deleting items with duplicate from list
            //foreach (var todeletefromlist in nodupeitems.ToList())
            //{
            //    if (todeletefromlist.DOC_NO == itemstodelete)
            //    {
            //        nodupeitems.Remove(todeletefromlist);
            //    }
            //}

            return noDuplicates;
        }

        #endregion Testing

        #region API Calls

        private IEnumerable<BapiOpenItemDto> GetOpenItemsAsOfDate()
        {
            SoaParams soaParams = new SoaParams
            {
                Customer_Number = SoaVM.Customer_Number,
                Company_Code = SoaVM.Company_Code,
                Posting_Date = SoaVM.Posting_Date
            };

            return _bapi.GetResponse(soaParams, SD.BAPI_AR_ACC_GETOPENITEMS);
        }

        private IEnumerable<BapiOpenItemDto> GetOpenItemsPreviousMonth()
        {
            SoaParams soaParams = new SoaParams
            {
                Customer_Number = SoaVM.Customer_Number,
                Company_Code = SoaVM.Company_Code,
                Posting_Date = SoaVM.PreviewMonthLastDay
            };

            return _bapi.GetResponse(soaParams, SD.BAPI_AR_ACC_GETOPENITEMS);
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

            return _bapi.GetResponse(soaParams, SD.BAPI_AR_ACC_GETSTATEMENT);
        }

        private IEnumerable<SoaDetailsDto> GetCompanyCustomerDetails()
        {
            SoaParams soaParams = new SoaParams
            {
                Company_Code = SoaVM.Company_Code,
                Customer_Number = SoaVM.Customer_Number
            };

            return _soaDetails.GetResponse(soaParams, SD.Z_FI_SOA_DETAILS);
        }

        #endregion API Calls

        #region Company and Customer Details

        private void GetCompanyCustomerDetailsToVM()
        {
            foreach (var item in SoaVM.SoaDetails)
            {
                SoaVM.CompanyCode = item.BUKRS;
                SoaVM.CompanyName = item.BUTXT;
                SoaVM.CompanysCity = item.CITY1;
                SoaVM.CompanyStreet = item.STREET;
                SoaVM.CompanyTelNumber = item.TEL_NUMBER;
                SoaVM.CompanyTinNumber = item.STCEG;
                SoaVM.CustomerNumber = item.KUNNR;
                SoaVM.CustomerName = item.NAME1;
                SoaVM.CustomerContact = item.CONTACT;
                SoaVM.CustomerStreet = item.STRAS;
                SoaVM.CustomerCity = item.ORT01;
            }
        }

        #endregion Company and Customer Details

        #region Previews Balance

        private decimal GetPreviousBalancePhp()
        {
            var soaPrevious = SoaVM.OpenItemsPreviousMonth
                                .Where(x => x.CURRENCY == "PHP");

            decimal totalbalance = 0;

            foreach (var item in soaPrevious)
            {
                if (item.DB_CR_IND == "S")
                {
                    totalbalance += item.AMOUNT;
                }
                else
                {
                    totalbalance -= item.AMOUNT;
                }
            }

            return totalbalance;
        }

        private decimal GetPreviousBalanceUsd()
        {
            var soaPrevious = SoaVM.OpenItemsPreviousMonth
                                .Where(x => x.CURRENCY == "USD");

            decimal totalbalance = 0;

            foreach (var item in soaPrevious)
            {
                if (item.DB_CR_IND == "S")
                {
                    totalbalance += item.AMOUNT;
                }
                else
                {
                    totalbalance -= item.AMOUNT;
                }
            }

            return totalbalance;
        }

        #endregion Previews Balance

        #region Current Billings

        private decimal GetCurrentBillingsUsd()
        {
            var currentBilling = SoaVM.StatementCurrentMonth
                                    .Where(x =>
                                            (x.DB_CR_IND == "S") &&
                                            !(x.SP_GL_IND == "C") &&
                                            !(x.DOC_TYPE == "DZ" || x.DOC_TYPE == "DX" || x.DOC_TYPE == "DY") &&
                                             (x.CURRENCY == "USD"));
            decimal TotalCurrentBills = 0;

            foreach (var item in currentBilling)
            {
                if (item.DB_CR_IND == "S")
                {
                    TotalCurrentBills += item.AMOUNT;
                }
                else
                {
                    TotalCurrentBills -= item.AMOUNT;
                }
            }

            return TotalCurrentBills;
        }

        private decimal GetCurrentBillingsPhp()
        {
            var currentBilling = SoaVM.StatementCurrentMonth
                                    .Where(x =>
                                            (x.DB_CR_IND == "S") &&
                                            !(x.SP_GL_IND == "C") &&
                                            !(x.DOC_TYPE == "DZ" || x.DOC_TYPE == "DX" || x.DOC_TYPE == "DY") &&
                                             (x.CURRENCY == "PHP"));
            decimal TotalCurrentBills = 0;

            foreach (var item in currentBilling)
            {
                if (item.DB_CR_IND == "S")
                {
                    TotalCurrentBills += item.AMOUNT;
                }
                else
                {
                    TotalCurrentBills -= item.AMOUNT;
                }
            }

            return TotalCurrentBills;
        }

        #endregion Current Billings

        #region Payments

        private IEnumerable<BapiOpenItemDto> GetPaymentsList2()
        {
            var openitems = SoaVM.OpenItemsAsOfDate
                    .Where(x =>
                        !(x.SP_GL_IND == "C") &&
                         (x.DOC_TYPE == "DX" || x.DOC_TYPE == "DY" || x.DOC_TYPE == "DZ"));

            var balanceditem = SoaVM.OpenItemsPreviousMonth
                                .Where(x =>
                                    !(x.SP_GL_IND == "C") &&
                                     (x.DOC_TYPE == "DX" || x.DOC_TYPE == "DY" || x.DOC_TYPE == "DZ"));

            // removing distinct
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

            return distPayments.OrderByDescending(x => x.ENTRY_DATE);
        }

        private IEnumerable<BapiOpenItemDto> GetPaymentsList3()
        {
            var payments = SoaVM.StatementCurrentMonth
                    .Where(x =>
                        !(x.SP_GL_IND == "C") &&
                         (x.DOC_TYPE == "DX" || x.DOC_TYPE == "DY" || x.DOC_TYPE == "DZ"));
            return payments.OrderByDescending(x => x.ENTRY_DATE);
        }

        private IEnumerable<BapiOpenItemDto> GetPaymentsList()
        {
            var payments = SoaVM.OpenItemsAsOfDate
                    .Where(x =>
                        !(x.SP_GL_IND == "C") &&
                         (x.DOC_TYPE == "DX" || x.DOC_TYPE == "DY" || x.DOC_TYPE == "DZ"));
            return payments.OrderByDescending(x => x.ENTRY_DATE);
        }

        private decimal GetTotalPaymentsPhp()
        {
            var totalPaymentsList = SoaVM.PaymentsListItem.Where(x => x.CURRENCY == "PHP");

            decimal TotalPayments = 0;

            foreach (var payment in totalPaymentsList)
            {
                if (payment.DB_CR_IND == "S")
                {
                    TotalPayments += payment.AMOUNT;
                }
                else
                {
                    TotalPayments -= payment.AMOUNT;
                }
            }

            return TotalPayments;
        }

        private decimal GetTotalPaymentsUsd()
        {
            var totalPaymentsList = SoaVM.PaymentsListItem.Where(x => x.CURRENCY == "USD");

            decimal TotalPayments = 0;

            foreach (var payment in totalPaymentsList)
            {
                if (payment.DB_CR_IND == "S")
                {
                    TotalPayments += payment.AMOUNT;
                }
                else
                {
                    TotalPayments -= payment.AMOUNT;
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

        private decimal GetUnpaidTotalPhp()
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
                                    y.DOC_TYPE == "PB") &&
                                    (y.CURRENCY == "PHP"));
            decimal unpaidsPhp = 0;

            foreach (var unpaid in totalunpaid)
            {
                if (unpaid.DB_CR_IND == "S")
                {
                    unpaidsPhp += unpaid.AMOUNT;
                }
                else
                {
                    unpaidsPhp -= unpaid.AMOUNT;
                }
            }

            return unpaidsPhp;
        }

        private decimal GetUnpaidTotalUsd()
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
                                    y.DOC_TYPE == "PB") &&
                                    (y.CURRENCY == "USD"));
            decimal unpaidsUsd = 0;

            foreach (var unpaid in totalunpaid)
            {
                if (unpaid.DB_CR_IND == "S")
                {
                    unpaidsUsd += unpaid.AMOUNT;
                }
                else
                {
                    unpaidsUsd -= unpaid.AMOUNT;
                }
            }

            return unpaidsUsd;
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

            //return SoaVM.StatementCurrentMonth
            return SoaVM.OpenItemsAsOfDate
                .Where(x => x.DOC_TYPE == "P8" ||
                            x.DOC_TYPE == "P9" ||
                            x.DOC_TYPE == "DG" ||
                            x.DOC_TYPE == "DJ" ||
                            x.DOC_TYPE == "DM" ||
                            x.DOC_TYPE == "DH" ||
                            x.DOC_TYPE == "PA" ||
                            x.DOC_TYPE == "PB");
        }

        private decimal GetTotalCreditAndDebitUsd()
        {
            var soaStatement = GetCreditAndDebitList().Where(x => x.CURRENCY == "USD");

            decimal totalcmdmusd = 0;
            if (soaStatement.Count() != 0)
            {
                foreach (var cmDm in soaStatement)
                {
                    if (cmDm.DB_CR_IND == "S")
                    {
                        totalcmdmusd += cmDm.AMOUNT;
                    }
                    else
                    {
                        totalcmdmusd -= cmDm.AMOUNT;
                    }
                }
            }
            return totalcmdmusd;
        }

        private decimal GetTotalCreditAndDebitPhp()
        {
            var soaStatement = GetCreditAndDebitList().Where(x => x.CURRENCY == "PHP");

            decimal totalcmdmphp = 0;
            if (soaStatement.Count() != 0)
            {
                foreach (var cmDm in soaStatement)
                {
                    if (cmDm.DB_CR_IND == "S")
                    {
                        totalcmdmphp += cmDm.AMOUNT;
                    }
                    else
                    {
                        totalcmdmphp -= cmDm.AMOUNT;
                    }
                }
            }
            return totalcmdmphp;
        }

        #endregion Credit and Debit

        #region Uncollected CWT

        private IEnumerable<BapiOpenItemDto> GetUncollectedCwtList()
        {
            return SoaVM.OpenItemsAsOfDate
                .Where(x => x.SP_GL_IND == "C").OrderByDescending(e => e.ENTRY_DATE);
        }   // Getting the list of Uncollected Cwt

        private decimal GetTotalUncollectedCwtUsd()
        {
            //var cwtList = SoaVM.UncollectedCwtList.Where(x => (x.SP_GL_IND == "C") && (x.CURRENCY == "USD"));
            var cwtList = GetUncollectedCwtList().Where(x => x.CURRENCY == "USD");

            decimal totalcwtusd = 0;
            if (cwtList.Count() != 0)
            {
                foreach (var cwt in cwtList)
                {
                    if (cwt.DB_CR_IND == "S")
                    {
                        totalcwtusd += cwt.AMOUNT;
                    }
                    else
                    {
                        totalcwtusd -= cwt.AMOUNT;
                    }
                }
            }
            return totalcwtusd;
        }

        private decimal GetTotalUncollectedCwtPhp()
        {
            //var cwtList = SoaVM.UncollectedCwtList.Where(x => (x.SP_GL_IND == "C") && (x.CURRENCY == "PHP"));
            var cwtList = GetUncollectedCwtList().Where(x => x.CURRENCY == "PHP");

            decimal totalcwtphp = 0;
            if (cwtList.Count() != 0)
            {
                foreach (var cwt in cwtList)
                {
                    if (cwt.DB_CR_IND == "S")
                    {
                        totalcwtphp += cwt.AMOUNT;
                    }
                    else
                    {
                        totalcwtphp -= cwt.AMOUNT;
                    }
                }
            }
            return totalcwtphp;
        }       // Getting the total uncollected cwt

        #endregion Uncollected CWT
    }
}