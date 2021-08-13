using Newtonsoft.Json;
using RestSharp;
using SoaApp.Dtos;
using SoaApp.Models;
using SoaApp.Repository.IRepository;
using System.Collections.Generic;

namespace SoaApp.Repository
{
    public class BapiRepository : IBapiRepository
    {
        public List<BapiOpenItemDto> GetResponse(SoaParams soaParams, string uri)
        {
            var SoaJson = JsonConvert.SerializeObject(soaParams);
            var client = new RestClient(uri);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "[\r\n" + SoaJson + "\r\n]\r\n", ParameterType.RequestBody);
            IRestResponse<List<BapiOpenItemDto>> response = client.Execute<List<BapiOpenItemDto>>(request);
            return response.Data;
        }

        public List<BapiKeyDateBalanceDto> GetResponseBalance(SoaParams soaParams, string uri)
        {
            var SoaJson = JsonConvert.SerializeObject(soaParams);
            var client = new RestClient(uri);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "[\r\n" + SoaJson + "\r\n]\r\n", ParameterType.RequestBody);
            IRestResponse<List<BapiKeyDateBalanceDto>> response = client.Execute<List<BapiKeyDateBalanceDto>>(request);
            return response.Data;
        }
    }
}