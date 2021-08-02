using SoaApp.Dtos;
using SoaApp.Models;
using System.Collections.Generic;

namespace SoaApp.Repository.IRepository
{
    public interface ISoaDetailsRepository
    {
        List<SoaDetailsDto> GetResponse(SoaParams soaParams, string uri);
    }
}