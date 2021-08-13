using SoaApp.Dtos;
using SoaApp.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoaApp.Repository
{
    public class BalancesRepository : IBalancesRepository
    {
        public decimal GetTotalBalance(IEnumerable<BapiKeyDateBalanceDto> ListItems, string currency = null)
        {
            var balances = ListItems.Where(x => x.CURRENCY == currency);

            decimal balance = 0;

            foreach (var item in balances)
            {
                balance = Convert.ToDecimal(item.T_CURR_BAL);
            }

            return balance;
        }
    }
}
