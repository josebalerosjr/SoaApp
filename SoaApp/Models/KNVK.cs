using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
