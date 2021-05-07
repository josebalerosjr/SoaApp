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
        List<BapiDto> GetBapi(SoaVM soaVM, string date);
    }
}
