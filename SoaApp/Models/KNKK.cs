using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoaApp.Models
{
    public class KNKK
    {
        [Key]
        public int Id { get; set; }
        public string KUNNR { get; set; }
        public string KKBER { get; set; }
        public string KLIMK { get; set; }
    }
}
