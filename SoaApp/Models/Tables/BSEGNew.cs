using System.ComponentModel.DataAnnotations;

namespace SoaApp.Models
{
    public class BSEGNew
    {
        [Key]
        public int Id { get; set; }

        public int BUKRS { get; set; }
        public int BELNR { get; set; }
        public int GJAHR { get; set; }
        public string KOART { get; set; }
        public string BUZEI { get; set; }
        public int HKONT { get; set; }
        public string SGTXT { get; set; }
        public string SHKZG { get; set; }
        public double DMBTR { get; set; }
        public double WRBTR { get; set; }
    }
}