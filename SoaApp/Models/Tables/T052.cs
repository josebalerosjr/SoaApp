using System.ComponentModel.DataAnnotations;

namespace SoaApp.Models
{
    public class T052
    {
        [Key]
        public int Id { get; set; }

        public string ZTERM { get; set; }
        public string ZTAG1 { get; set; }
        public string ZTAG2 { get; set; }
        public string ZTAG3 { get; set; }
    }
}