using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoaApp.Dtos
{
    public class BapiKeyDateBalanceDto
    {
        public string SP_GL_IND { get; set; }
        public string CURRENCY { get; set; }
        public string DB_CR_IND { get; set; }
        public string T_CURR_BAL { get; set; }
        public string LOC_CURRCY { get; set; }
        public string LC_BAL { get; set; }
    }
}
