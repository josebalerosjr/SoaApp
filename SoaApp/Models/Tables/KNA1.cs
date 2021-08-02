using System.ComponentModel.DataAnnotations;

namespace SoaApp.Models
{
    public class KNA1
    {
        [Key]
        public int Id { get; set; }

        public string KUNNR { get; set; }
        public string NAME1 { get; set; }
        public string STRAS { get; set; }
        public string ORT01 { get; set; }
        public string PSTLZ { get; set; }
    }
}