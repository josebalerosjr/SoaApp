using System.ComponentModel.DataAnnotations;

namespace SoaApp.Models
{
    public class KNB1
    {
        [Key]
        public int Id { get; set; }

        public string KUNNR { get; set; }
        public string BUKRS { get; set; }
        public string KVERM { get; set; }
    }
}