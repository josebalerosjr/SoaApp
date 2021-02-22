using Intranet.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using SoaApp.Utilities;
using System;
using System.Linq;

namespace SoaApp.Controllers
{
    public class SoaController : Controller
    {
        private readonly SOADbContext _context;

        public SoaController(SOADbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GenerateSoa(int customer, int company, DateTime asof)
        {
            SD.GuiCompany = company.ToString();
            SD.GuiAsOf = asof;
            SD.GuiCustomerNum = customer.ToString();

            // Get Address Code
            var t001 = _context.T001s.Where(i => i.BURKS == SD.GuiCompany);
            foreach (var comp in t001)
            {
                SD.AddressCode = comp.ADRNR;
                SD.CompanyTinNum = comp.STCEG;
            }

            // Company Details
            var adrc = _context.ADRCs.Where(i => i.ADDRNUMBER == SD.AddressCode);
            foreach (var compa in adrc)
            {
                SD.CompanyName = compa.NAME1;
                SD.CompanyTellNum = compa.TEL_NUMBER;
                SD.CompanyCity = compa.CITY1;
                SD.CompanyStreet = compa.STREET + " ";
            }

            // get Customer details
            var kna1 = _context.KNA1s.Where(i => i.KUNNR == SD.GuiCustomerNum);
            foreach (var cust in kna1)
            {
                SD.CustomerName = cust.NAME1;
                SD.CustomerCity = cust.ORT01;
                SD.CustomerStreet = cust.STRAS + " ";
            }

            //Get Customer Number and Attention To
            var knvk = _context.KNVKs.Where(i => i.KUNNR == SD.GuiCustomerNum);
            foreach (var custo in knvk)
            {
                SD.CustomerNum = custo.KUNNR;
                SD.AttentionTo = custo.NAME1;
            }

            // getting CWT
            var bsid_cwt = _context.BSIDs.Where(i => i.UMSKZ == "C");
            ViewBag.BSID_CWT = bsid_cwt;
            foreach (var cwt in bsid_cwt)
            {
                SD.WHTotalAmount += Convert.ToDouble(cwt.DMBTR);
            }

            // getting Unpaid items
            var bsid_unpaid = _context.BSIDs.Where(i => i.UMSKZ == "").OrderByDescending(e => e.ZFBDT);
            ViewBag.BSID_UNPAID = bsid_unpaid;

            var bsid_unpaid_items = _context.BSIDs.Where(e => e.UMSKZ == "" && e.BLART != "DJ");
            foreach (var unpaid in bsid_unpaid_items)
            {
                SD.UPAmount += Convert.ToDouble(unpaid.DMBTR);
            }

            var bsid_unpaid_payments = _context.BSIDs.Where(e => e.UMSKZ == "" && e.BLART == "DJ");
            foreach (var payments in bsid_unpaid_payments)
            {
                SD.PAmount += Convert.ToDouble(payments.DMBTR);
            }
            SD.UPTotalAmount = SD.UPAmount - SD.PAmount;

            return View();
        }
    }
}