using SoaApp.Dtos;
using SoaApp.Models;
using System.Collections.Generic;

namespace SoaApp.Repository.IRepository
{
    public interface IBapiRepository
    {
        List<BapiOpenItemDto> GetResponse(SoaParams soaParams, string uri);
        List<BapiKeyDateBalanceDto> GetResponseBalance(SoaParams soaParams, string uri);
    }
}