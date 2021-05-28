using RestSharp;
using SoaApp.Models;
using SoaApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SoaApp.Repository.IRepository
{
    public interface IBapiRepository
    {
        List<BapiOpenItemDto> GetResponse(SoaParams soaParams, string uri);
        //List<BapiOpenItemDto> GetPreviousBalanceItems(SoaVM soaVM);
        //List<BapiOpenItemDto> GetOpenItems(SoaVM soaVM);
        //List<BapiOpenItemDto> GetStatementItems(SoaVM soaVM);
        //List<BapiOpenItemDto> GetBalanceItems(SoaVM soaVM);
    }
}
