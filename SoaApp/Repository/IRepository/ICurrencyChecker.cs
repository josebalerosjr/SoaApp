using SoaApp.Dtos;
using System.Collections.Generic;

namespace SoaApp.Repository.IRepository
{
    public interface ICurrencyChecker
    {
        bool UsdChecker(IEnumerable<BapiOpenItemDto> ListItems);

        bool PhpChecker(IEnumerable<BapiOpenItemDto> ListItems);
    }
}