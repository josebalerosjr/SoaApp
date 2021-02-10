using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
