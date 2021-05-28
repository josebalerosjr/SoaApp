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
        public double PreviousBalance { get; set; }
        public string Date_From { get; set; }

        #endregion Parameter details

        public Company Company { get; set; }
        public Customer Customer { get; set; }
        public IEnumerable<BapiOpenItemDto> OpenItemsAsOfDate { get; set; }
        public IEnumerable<BapiOpenItemDto> OpenItemsPreviousMonth { get; set; }
        public IEnumerable<BapiOpenItemDto> StatementCurrentMonth { get; set; }

        public double TotalCurrentBillings { get; set; }

        #region Payments

        public IEnumerable<BapiOpenItemDto> PaymentsListItem { get; set; }
        public double TotalPayments { get; set; }

        #endregion Payments

        #region Unpaid

        public IEnumerable<BapiOpenItemDto> UnpaidListItem { get; set; }
        public double TotalUnpaid { get; set; }

        #endregion Unpaid

        #region Credit and Debit

        public IEnumerable<BapiOpenItemDto> CreditAndDebitList { get; set; }
        public double TotalCreditAndDebit { get; set; }

        #endregion Credit and Debit

        #region Uncollected CWT

        public IEnumerable<BapiOpenItemDto> UncollectedCwtList { get; set; }
        public double TotalUncollectedCwtPhp { get; set; }
        public double TotalUncollectedCwtUsd { get; set; }

        #endregion Uncollected CWT
    }
}