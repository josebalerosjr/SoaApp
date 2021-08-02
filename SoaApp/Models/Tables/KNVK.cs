using System.ComponentModel.DataAnnotations;

namespace SoaApp.Models
{
    public class KNVK
    {
        [Key]
        public int Id { get; set; }

        public string KUNNR { get; set; }
        public string NAME1 { get; set; }
    }
}