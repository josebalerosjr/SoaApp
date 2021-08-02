using System.ComponentModel.DataAnnotations;

namespace SoaApp.Models
{
    public class T001
    {
        [Key]
        public int Id { get; set; }

        public string BURKS { get; set; }
        public string ADRNR { get; set; }
        public string BUTXT { get; set; }
        public string STCEG { get; set; }
    }
}