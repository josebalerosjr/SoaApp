using SoaApp.Dtos;
using System.Collections.Generic;

namespace SoaApp.Models.ViewModels
{
    public class SoaVM
    {
        #region Parameter details

        public string Company_Code { get; set; }
        public string Customer_Number { get; set; }
        public string Posting_Date { get; set; }
        public string PreviewMonthLastDay { get; set; }

        public string Date_From { get; set; }

        #endregion Parameter details

        public Company Company { get; set; }
        public Customer Customer { get; set; }
        public IEnumerable<BapiOpenItemDto> OpenItemsAsOfDate { get; set; }
        public IEnumerable<BapiKeyDateBalanceDto> PreviousBalance { get; set; }
        public IEnumerable<BapiKeyDateBalanceDto> CurrentBalance { get; set; }
        public IEnumerable<BapiOpenItemDto> PaymentsTesting { get; set; }

        public decimal TotalSOAUsd { get; set; }
        public decimal TotalSOAPhp { get; set; }

        #region Preview Balance

        public decimal PreviousBalancePhp { get; set; }
        public decimal PreviousBalanceUsd { get; set; }

        #endregion Preview Balance

        #region Company and Customer details

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanysCity { get; set; }
        public string CompanyStreet { get; set; }
        public string CompanyTelNumber { get; set; }
        public string CompanyTinNumber { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerContact { get; set; }
        public string CustomerStreet { get; set; }
        public string CustomerCity { get; set; }
        public IEnumerable<SoaDetailsDto> SoaDetails { get; set; }

        #endregion Company and Customer details

        #region Billings

        public IEnumerable<BapiOpenItemDto> CurrentBillingsList { get; set; }
        public decimal TotalCurrentBillingsUsd { get; set; }
        public decimal TotalCurrentBillingsPhp { get; set; }
        //public bool BillingUsdCheck { get; set; }
        //public bool BillingPhpCheck { get; set; }

        #endregion Billings

        #region Payments

        public IEnumerable<BapiOpenItemDto> PaymentsListItem { get; set; }
        public decimal TotalPaymentsPhp { get; set; }
        public decimal TotalPaymentsUsd { get; set; }

        #endregion Payments

        #region Unpaid

        public IList<BapiOpenItemDto> UnpaidListItem { get; set; }
        public IList<BapiOpenItemDto> UnpaidListItemNew { get; set; }
        public IList<BapiOpenItemDto> Partials { get; set; }
        public IList<BapiOpenItemDto> Unpaids { get; set; }
        public decimal TotalUnpaidPhp { get; set; }
        public decimal TotalUnpaidUsd { get; set; }

        #endregion Unpaid

        #region Credit and Debit

        public IEnumerable<BapiOpenItemDto> CreditAndDebitList { get; set; }
        public decimal TotalCreditAndDebitUsd { get; set; }
        public decimal TotalCreditAndDebitPhp { get; set; }
        //public bool CreditDebitUsdCheck { get; set; }
        //public bool CreditDebitPhpCheck { get; set; }

        #endregion Credit and Debit

        #region Uncollected CWT

        public IEnumerable<BapiOpenItemDto> UncollectedCwtList { get; set; }
        public decimal TotalUncollectedCwtPhp { get; set; }
        public decimal TotalUncollectedCwtUsd { get; set; }

        #endregion Uncollected CWT

        #region Currency Checker

        public bool UsdChecker { get; set; }
        public bool PhpChecker { get; set; }

        #endregion Currency Checker
    }
}