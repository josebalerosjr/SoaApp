using SoaApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoaApp.Repository.IRepository
{
    public interface IBalancesRepository
    {
        decimal GetTotalBalance(
            IEnumerable<BapiKeyDateBalanceDto> ListItems,
            string currency = null);
    }
}
