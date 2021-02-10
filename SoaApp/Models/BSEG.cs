using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoaApp.Models
{
    public class BSEG
    {
        [Key]
        public int Id { get; set; }
        public string BUKRS { get; set; }
        public string BELNR { get; set; }
        public string GJAHR { get; set; }
        public string KOART { get; set; }
        public string BUZEI { get; set; }
        public string HKONT { get; set; }
        public string SGTXT { get; set; }
        public string SHKZG { get; set; }
        public string DMBTR { get; set; }
        public string WRBTR { get; set; }

    }
}
