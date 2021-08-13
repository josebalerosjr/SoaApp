using SoaApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoaApp.Repository.IRepository
{
    public interface ITotalAmountRepository
    {
        decimal GetTotalAmount(
            IEnumerable<BapiOpenItemDto> ListItems, 
            string currency = null);
    }
}
