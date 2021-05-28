using Newtonsoft.Json;
using RestSharp;
using SoaApp.Models;
using SoaApp.Models.ViewModels;
using SoaApp.Repository.IRepository;
using SoaApp.Utilities;
using System;
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

        //public List<BapiOpenItemDto> GetStatementItems(SoaVM soaVM)
        //{
        //    SoaParams soaParams = new SoaParams
        //    {
        //        Customer_Number = soaVM.Customer_Number,
        //        Company_Code = soaVM.Company_Code,
        //        Date_From = soaVM.Date_From,
        //        Date_To = soaVM.Posting_Date
        //    };

        //    var SoaJson = JsonConvert.SerializeObject(soaParams);
        //    var client = new RestClient(SD.ApiUriStatement);
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddParameter("application/json", "[\r\n" + SoaJson + "\r\n]\r\n", ParameterType.RequestBody);
        //    IRestResponse<List<BapiOpenItemDto>> response = client.Execute<List<BapiOpenItemDto>>(request);
        //    return response.Data;
        //}

        //public List<BapiOpenItemDto> GetPreviousBalanceItems(SoaVM soaVM)
        //{
        //    SoaParams soaParams = new SoaParams
        //    {
        //        Customer_Number = soaVM.Customer_Number,
        //        Company_Code = soaVM.Company_Code,
        //        Posting_Date = soaVM.PreviewMonthLastDay
        //    };
        //    var SoaJson = JsonConvert.SerializeObject(soaParams);
        //    var client = new RestClient(SD.ApiUriOpenItems);
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddParameter("application/json", "[\r\n" + SoaJson + "\r\n]\r\n", ParameterType.RequestBody);
        //    IRestResponse<List<BapiOpenItemDto>> response = client.Execute<List<BapiOpenItemDto>>(request);
        //    return response.Data;
        //}

        //public List<BapiOpenItemDto> GetOpenItems(SoaVM soaVM)
        //{
        //    SoaParams soaParams = new SoaParams
        //    {
        //        Customer_Number = soaVM.Customer_Number,
        //        Company_Code = soaVM.Company_Code,
        //        Posting_Date = soaVM.Posting_Date
        //    };
        //    var SoaJson = JsonConvert.SerializeObject(soaParams);
        //    var client = new RestClient(SD.ApiUriOpenItems);
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddParameter("application/json", "[\r\n" + SoaJson + "\r\n]\r\n", ParameterType.RequestBody);
        //    IRestResponse<List<BapiOpenItemDto>> response = client.Execute<List<BapiOpenItemDto>>(request);
        //    return response.Data;
        //}

        //public List<BapiOpenItemDto> GetBalanceItems(SoaVM soaVM)
        //{
        //    SoaParams soaParams = new SoaParams
        //    {
        //        Customer_Number = soaVM.Customer_Number,
        //        Company_Code = soaVM.Company_Code,
        //        Date_From = soaVM.Date_From,
        //        Date_To = soaVM.Posting_Date
        //    };
        //    var SoaJson = JsonConvert.SerializeObject(soaParams);
        //    var client = new RestClient(SD.ApiUriStatement);
        //    client.Timeout = -1;
        //    var request = new RestRequest(Method.POST);
        //    request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddParameter("application/json", "[\r\n" + SoaJson + "\r\n]\r\n", ParameterType.RequestBody);
        //    IRestResponse<List<BapiOpenItemDto>> response = client.Execute<List<BapiOpenItemDto>>(request);
        //    return response.Data;
        //}
    }
}