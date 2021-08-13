using SoaApp.Dtos;
using SoaApp.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoaApp.Repository
{
    public class TotalAmountRepository : ITotalAmountRepository
    {
        public decimal GetTotalAmount(
            IEnumerable<BapiOpenItemDto> ListItems, 
            string currency = null)
        {
            var lists = ListItems
                .Where(x => x.CURRENCY == currency);

            decimal amount = 0;

            foreach (var list in lists)
            {
                if (list.DB_CR_IND == "S") amount += list.AMOUNT;
                else amount -= list.AMOUNT;
            }
            return amount;
        }
    }
}
