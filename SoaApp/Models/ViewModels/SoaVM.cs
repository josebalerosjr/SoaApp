using System.Collections.Generic;

namespace SoaApp.Models.ViewModels
{
    public class SoaVM
    {
        public Company Company { get; set; }
        public Customer Customer { get; set; }
        public SoaParams SoaParams { get; set; }
        public IEnumerable<BAPITable> BAPITable { get; set; }
        public string Company_Code { get; set; }
        public string Customer_Number { get; set; }
        public string Posting_Date { get; set; }
        public string PreviewMonthLastDay { get; set; }
        public double PreviousBalance { get; set; }
    }
}