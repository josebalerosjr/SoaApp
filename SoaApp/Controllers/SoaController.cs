using Microsoft.AspNetCore.Mvc;
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
        private readonly ITotalAmountRepository _totalAmount;
        private readonly IBalancesRepository _balances;

        [BindProperty]
        public SoaVM SoaVM { get; set; }

        public SoaController(
            IBapiRepository bapi,
            ISoaDetailsRepository soaDetails,
            ICurrencyChecker checker,
            ITotalAmountRepository totalAmount,
            IBalancesRepository balances)
        {
            _bapi = bapi;
            _soaDetails = soaDetails;
            _checker = checker;
            _totalAmount = totalAmount;
            _balances = balances;
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
            SoaVM.SoaDetails = GetCompanyCustomerDetails();
            SoaVM.PreviousBalance = GetPreviousBalance();
            SoaVM.CurrentBalance = GetCurrentBalance();

            #endregion API Calls

            // Set Company and Customer Data to View Model
            GetCompanyCustomerDetailsToVM();

            #region Currency Checker

            SoaVM.UsdChecker = _checker.UsdChecker(SoaVM.OpenItemsAsOfDate);
            SoaVM.PhpChecker = _checker.PhpChecker(SoaVM.OpenItemsAsOfDate);

            #endregion Currency Checker

            #region Previous Balance

            SoaVM.PreviousBalancePhp = _balances.GetTotalBalance(SoaVM.PreviousBalance, "PHP");
            SoaVM.PreviousBalanceUsd = _balances.GetTotalBalance(SoaVM.PreviousBalance, "USD");

            #endregion Previous Balance

            #region Current Billings

            //SoaVM.CurrentBillingsList = GetCurrentBillingsList();
            //SoaVM.TotalCurrentBillingsPhp = _totalAmount.GetTotalAmount(SoaVM.CurrentBillingsList, currency: "PHP");
            //SoaVM.TotalCurrentBillingsUsd = _totalAmount.GetTotalAmount(SoaVM.CurrentBillingsList, currency: "USD");
            SoaVM.TotalCurrentBillingsPhp = _balances.GetTotalBalance(SoaVM.CurrentBalance, "PHP");
            SoaVM.TotalCurrentBillingsUsd = _balances.GetTotalBalance(SoaVM.CurrentBalance, "USD");

            #endregion Current Billings

            #region Payments

            SoaVM.PaymentsListItem = GetPaymentsList();     // setting Payment Lists
            SoaVM.TotalPaymentsUsd = _totalAmount.GetTotalAmount(SoaVM.PaymentsListItem, currency: "USD");        // setting  total payments
            SoaVM.TotalPaymentsPhp = _totalAmount.GetTotalAmount(SoaVM.PaymentsListItem, currency: "PHP");       // setting  total payments

            #endregion Payments

            #region Unpaid

            SoaVM.UnpaidListItem = GetUnpaidItems();

            SoaVM.Partials = GetPartialsList();
            SoaVM.Unpaids = GetUnpaidList();

            SoaVM.UnpaidListItemNew = GetUnpaidsList();

            SoaVM.TotalUnpaidUsd = _totalAmount.GetTotalAmount(SoaVM.Unpaids, currency: "USD");
            SoaVM.TotalUnpaidPhp = _totalAmount.GetTotalAmount(SoaVM.Unpaids, currency: "PHP");

            #endregion Unpaid

            #region Credit And Debit

            SoaVM.CreditAndDebitList = GetCreditAndDebitList();
            SoaVM.TotalCreditAndDebitPhp = _totalAmount.GetTotalAmount(SoaVM.CreditAndDebitList, currency: "PHP");
            SoaVM.TotalCreditAndDebitUsd = _totalAmount.GetTotalAmount(SoaVM.CreditAndDebitList, currency: "USD");

            #endregion Credit And Debit

            #region CWT

            SoaVM.UncollectedCwtList = GetUncollectedCwtList();         // setting list of uncollected CWT
            SoaVM.TotalUncollectedCwtPhp = _totalAmount.GetTotalAmount(SoaVM.UncollectedCwtList, currency: "PHP");   // setting total uncollected CWT
            SoaVM.TotalUncollectedCwtUsd = _totalAmount.GetTotalAmount(SoaVM.UncollectedCwtList, currency: "USD");   // setting total uncollected CWT

            #endregion CWT

            #region TotalSOA

            SoaVM.TotalSOAUsd = (SoaVM.TotalUncollectedCwtUsd) + (SoaVM.TotalCreditAndDebitUsd) + (SoaVM.TotalUnpaidUsd);
            SoaVM.TotalSOAPhp = (SoaVM.TotalUncollectedCwtPhp) + (SoaVM.TotalCreditAndDebitPhp) + (SoaVM.TotalUnpaidPhp);

            #endregion TotalSOA

            return View(SoaVM);
        }

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


        private IEnumerable<BapiKeyDateBalanceDto> GetCurrentBalance()
        {
            SoaParams soaParams = new SoaParams
            {
                Customer_Number = SoaVM.Customer_Number,
                Company_Code = SoaVM.Company_Code,
                Key_Date = SoaVM.Posting_Date
            };

            return _bapi.GetResponseBalance(soaParams, SD.BAPI_AR_ACC_GETKEYDATEBALANCE);
        }

        private IEnumerable<BapiKeyDateBalanceDto> GetPreviousBalance()
        {
            SoaParams soaParams = new SoaParams
            {
                Customer_Number = SoaVM.Customer_Number,
                Company_Code = SoaVM.Company_Code,
                Key_Date = SoaVM.PreviewMonthLastDay.ToString()
            };

            return _bapi.GetResponseBalance(soaParams, SD.BAPI_AR_ACC_GETKEYDATEBALANCE);
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

        #region Data List Item


        // Get Payments Item List
        private IEnumerable<BapiOpenItemDto> GetPaymentsList()
        {
            return SoaVM.OpenItemsAsOfDate
                    .Where(x =>
                        !(x.SP_GL_IND == "C") &&
                         (x.DOC_TYPE == "DX" || x.DOC_TYPE == "DY" || x.DOC_TYPE == "DZ"))
                    .OrderByDescending(x => x.ENTRY_DATE);
        }

        // Get Unpaid Item List
        private IList<BapiOpenItemDto> GetUnpaidItems()
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
                .OrderByDescending(x => x.ENTRY_DATE).ToList();
        }

        // Get Credit and Debit Item List
        private IEnumerable<BapiOpenItemDto> GetCreditAndDebitList()
        {
            return SoaVM.OpenItemsAsOfDate
                .Where(x =>
                    x.DOC_TYPE == "P8" ||
                    x.DOC_TYPE == "P9" ||
                    x.DOC_TYPE == "DG" ||
                    x.DOC_TYPE == "DJ" ||
                    x.DOC_TYPE == "DM" ||
                    x.DOC_TYPE == "DH" ||
                    x.DOC_TYPE == "PA" ||
                    x.DOC_TYPE == "PB");
        }

        // Get Uncollected CWT Item List
        private IEnumerable<BapiOpenItemDto> GetUncollectedCwtList()
        {
            return SoaVM.OpenItemsAsOfDate
                .Where(
                    x => x.SP_GL_IND == "C"
                    )
                .OrderByDescending(
                    e => e.ENTRY_DATE
                    );
        }

        #endregion Data List Item

        //Get Unpaid
        private IList<BapiOpenItemDto> GetUnpaidsList()
        {
            foreach (var item in SoaVM.Unpaids)
            {
                foreach (var payments in SoaVM.Partials)
                {
                    if (payments.INV_REF == item.DOC_NO)
                    {
                        item.AMOUNT -= payments.AMOUNT;
                    }
                }
            }
            return SoaVM.Unpaids;
        }

        private IList<BapiOpenItemDto> GetPartialsList()
        {
            return SoaVM.UnpaidListItem.Where(x => (x.DOC_TYPE == "DX" || x.DOC_TYPE == "DY" || x.DOC_TYPE == "DZ")).ToList();
        }

        private IList<BapiOpenItemDto> GetUnpaidList()
        {
            return SoaVM.UnpaidListItem.Where(x => !(x.DOC_TYPE == "DX" || x.DOC_TYPE == "DY" || x.DOC_TYPE == "DZ")).ToList();
        }

    }
}