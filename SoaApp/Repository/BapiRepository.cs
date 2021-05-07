using Newtonsoft.Json;
using RestSharp;
using SoaApp.Models;
using SoaApp.Models.ViewModels;
using SoaApp.Repository.IRepository;
using SoaApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoaApp.Repository
{
    public class BapiRepository : IBapiRepository
    {
        public List<BapiDto> GetBapi(SoaVM soaVM, string date)
        {
            SoaParams soaParams = new SoaParams
            { 
                Customer_Number = soaVM.Customer_Number,
                Company_Code = soaVM.Company_Code,
                Posting_Date = date
            };

            var SoaJson = JsonConvert.SerializeObject(soaParams);

            var client = new RestClient(SD.ApiUri);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "[\r\n" + SoaJson + "\r\n]\r\n", ParameterType.RequestBody);
            IRestResponse<List<BapiDto>> response = client.Execute<List<BapiDto>>(request);
            return response.Data;
        }
    }
}
