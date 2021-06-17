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
        public decimal PreviousBalancePhp { get; set; }
        public decimal PreviousBalanceUsd { get; set; }
        public string Date_From { get; set; }

        #endregion Parameter details

        public Company Company { get; set; }
        public Customer Customer { get; set; }
        public IEnumerable<BapiOpenItemDto> OpenItemsAsOfDate { get; set; }
        public IEnumerable<BapiOpenItemDto> OpenItemsPreviousMonth { get; set; }
        public IEnumerable<BapiOpenItemDto> StatementCurrentMonth { get; set; }

        public decimal TotalSOAUsd { get; set; }
        public decimal TotalSOAPhp { get; set; }

        #region Billings

        public decimal TotalCurrentBillingsUsd { get; set; }
        public decimal TotalCurrentBillingsPhp { get; set; }

        #endregion Billings

        #region Payments

        public IEnumerable<BapiOpenItemDto> PaymentsListItem { get; set; }
        public decimal TotalPaymentsPhp { get; set; }
        public decimal TotalPaymentsUsd { get; set; }

        #endregion Payments

        #region Unpaid

        public IEnumerable<BapiOpenItemDto> UnpaidListItem { get; set; }
        public decimal TotalUnpaidPhp { get; set; }
        public decimal TotalUnpaidUsd { get; set; }

        #endregion Unpaid

        #region Credit and Debit

        public IEnumerable<BapiOpenItemDto> CreditAndDebitList { get; set; }
        public decimal TotalCreditAndDebitUsd { get; set; }
        public decimal TotalCreditAndDebitPhp { get; set; }

        #endregion Credit and Debit

        #region Uncollected CWT

        public IEnumerable<BapiOpenItemDto> UncollectedCwtList { get; set; }
        public decimal TotalUncollectedCwtPhp { get; set; }
        public decimal TotalUncollectedCwtUsd { get; set; }

        #endregion Uncollected CWT
    }
}