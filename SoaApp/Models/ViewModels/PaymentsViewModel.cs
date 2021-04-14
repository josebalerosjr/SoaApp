using System.Collections.Generic;

namespace SoaApp.Models.ViewModels
{
    public class PaymentsViewModel
    {
        public IList<BSID> BSIDs { get; set; }
        public IList<BSAD> BSADs { get; set; }
    }
}