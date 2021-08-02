using SoaApp.Dtos;
using SoaApp.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;

namespace SoaApp.Repository
{
    public class CurrencyChecker : ICurrencyChecker
    {
        public bool PhpChecker(IEnumerable<BapiOpenItemDto> ListItems)
        {
            var list = ListItems.Where(x => x.CURRENCY == "PHP");

            if (list.Count() > 0)
            {
                return true;
            }

            return false;
        }

        public bool UsdChecker(IEnumerable<BapiOpenItemDto> ListItems)
        {
            var list = ListItems.Where(x => x.CURRENCY == "USD");

            if (list.Count() > 0)
            {
                return true;
            }

            return false;
        }
    }
}