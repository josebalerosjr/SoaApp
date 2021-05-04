using Intranet.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using SoaApp.Models;
using SoaApp.Models.ViewModels;
using SoaApp.Utilities;
using System;
using System.Linq;

namespace SoaApp.Controllers
{
    public class SoaController : Controller
    {
        private readonly SOADbContext _context;

        [BindProperty]
        public SoaVM SoaVM { get; set; }

        public SoaController(SOADbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Failed()
        {
            return View();
        }

        private IRestResponse PostSoa(SoaParams soaParams)
        {
            var SoaJson = JsonConvert.SerializeObject(soaParams);

            var client = new RestClient(SD.ApiUri);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "[\r\n" + SoaJson + "\r\n]\r\n", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }

        [HttpPost]
        public IActionResult Index2(string company, string customer, string asof)
        {
            // Create new instance of Person
            SoaParams soaParams = new SoaParams
            {
                Posting_Date = asof,
                Company_Code = company,
                Customer_Number = customer
            };

            return View(PostSoa(soaParams));
        }

        public IActionResult GenerateSoa(string customer, int company, DateTime asof)
        {
            //SD.DollarTotal = 1;

            SoaVM = new SoaVM()
            {
                Customer = new Models.Customer(),
                Company = new Models.Company()
            };

            SoaVM.Customer.AsOfDate = asof;

            ClearVariables();

            GetCustomerDetails(customer);

            GetCompanyDetails(company);

            ClearDbTables();    
            
            //GetAllBkpfAndPutToNewDb();
            //GetAllBsegAndPutToNewDb();
            //GetAllRBkpfAndPutToNewDb();
            //GetAllBsadAndPutToNewDb();
            GetAllBsidAndPutToNewDb();

            SoaVM.Customer.PreviousBalance = GetPreviousBalance();

            //GetPaymentsData(asof);

            //GetCwtDetails();

            //GetCurrentPreviousBill(asof);

            //GetUnpaidDetails();

            //GetCreditDebitDetails();

            //GetTotalAmountDue();

            return View(SoaVM);
        }

        private double GetPreviousBalance()
        {
            var previousBill = _context.BSIDNews.Where(i => i.KUNNR == SoaVM.Customer.Code);

            foreach (var bill in previousBill)
            {
                if (bill.BUDAT <= DateMangement.GetPreviousMonthLastDay(SoaVM.Customer.AsOfDate))
                {
                    if (bill.SHKZG == "S")
                    {
                        SoaVM.Customer.PreviousBalance += bill.WRBTR;
                    }
                    //else
                    //{
                    //    SD.PreviousBillsTotal -= Convert.ToDouble(bill.WRBTR);
                    //}
                }
            }

            return SoaVM.Customer.PreviousBalance;
        }

        public void GetAllBkpfAndPutToNewDb()
        {
            var bKPFs = _context.BKPFs;
            foreach (var item in bKPFs)
            {
                var bkpfnew = new BKPFNew
                {
                    BUKRS = Convert.ToInt16(item.BUKRS),
                    BELNR = Convert.ToInt32(item.BELNR),
                    GJAHR = Convert.ToInt32(item.GJAHR),
                    BLART = item.BLART,
                    BLDAT = Convert.ToDateTime(item.BLDAT),
                    BUDAT = Convert.ToDateTime(item.BUDAT),
                    MONAT = Convert.ToInt32(item.MONAT),
                    CPUDT = item.CPUDT,
                    CPUTM = item.CPUTM,
                    AEDAT = item.AEDAT,
                    UPDDT = item.UPDDT,
                    WWERT = item.WWERT,
                    USNAM = item.USNAM,
                    TCODE = item.TCODE,
                    BVORG = item.BVORG,
                    XBLNR = item.XBLNR,
                    DBBLG = item.DBBLG,
                    STBLG = item.STBLG,
                    STJAH = item.STJAH,
                    BKTXT = item.BKTXT,
                    WAERS = item.WAERS,
                    KURSF = item.KURSF,
                    KZWRS = item.KZWRS,
                    KZKRS = item.KZKRS,
                    BSTAT = item.BSTAT,
                    XNETB = item.XNETB,
                    FRATH = item.FRATH,
                    XRUEB = item.XRUEB,
                    GLVOR = item.GLVOR,
                    GRPID = item.GRPID,
                    DOKID = item.DOKID,
                    ARCID = item.ARCID,
                    IBLAR = item.IBLAR,
                    AWTYP = item.AWTYP,
                    AWKEY = item.AWKEY,
                    FIKRS = item.FIKRS,
                    HWAER = item.HWAER,
                    HWAE2 = item.HWAE2,
                    HWAE3 = item.HWAE3,
                    KURS2 = item.KURS2,
                    KURS3 = item.KURS3,
                    BASW2 = item.BASW2,
                    BASW3 = item.BASW3,
                    UMRD2 = item.UMRD2,
                    UMRD3 = item.UMRD3,
                    XSTOV = item.XSTOV,
                    STODT = item.STODT,
                    XMWST = item.XMWST,
                    CURT2 = item.CURT2,
                    CURT3 = item.CURT3,
                    KUTY2 = item.KUTY2,
                    KUTY3 = item.KUTY3,
                    XSNET = item.XSNET,
                    AUSBK = item.AUSBK,
                    XUSVR = item.XUSVR,
                    DUEFL = item.DUEFL,
                    AWSYS = item.AWSYS,
                    TXKRS = item.TXKRS,
                    CTXKRS = item.CTXKRS,
                    LOTKZ = item.LOTKZ,
                    XWVOF = item.XWVOF,
                    STGRD = item.STGRD,
                    PPNAM = item.PPNAM,
                    PPDAT = item.PPDAT,
                    PPTME = item.PPTME,
                    PPTCOD = item.PPTCOD,
                    BRNCH = item.BRNCH,
                    NUMPG = item.NUMPG,
                    ADISC = item.ADISC,
                    XREF1_HD = item.XREF1_HD,
                    XREF2_HD = item.XREF2_HD,
                    XREVERSAL = item.XREVERSAL,
                    REINDAT = item.REINDAT,
                    RLDNR = item.RLDNR,
                    LDGRP = item.LDGRP,
                    PROPMANO = item.PROPMANO,
                    XBLNR_ALT = item.XBLNR_ALT,
                    VATDATE = item.VATDATE,
                    DOCCAT = item.DOCCAT,
                    XSPLIT = item.XSPLIT,
                    CASH_ALLOC = item.CASH_ALLOC,
                    FOLLOW_ON = item.FOLLOW_ON,
                    XREORG = item.XREORG,
                    SUBSET = item.SUBSET,
                    KURST = item.KURST,
                    KURSX = item.KURSX,
                    KUR2X = item.KUR2X,
                    KUR3X = item.KUR3X,
                    XMCA = item.XMCA,
                    RESUBMISSION = item.RESUBMISSION,
                    SAPF15_STATUS = item.SAPF15_STATUS,
                    PSOTY = item.PSOTY,
                    PSOAK = item.PSOAK,
                    PSOKS = item.PSOKS,
                    PSOSG = item.PSOSG,
                    PSOFN = item.PSOFN,
                    PSOBT = item.PSOBT,
                    PSOZL = item.PSOZL,
                    PSODT = item.PSODT,
                    PSOTM = item.PSOTM,
                    FM_UMART = item.FM_UMART,
                    CCINS = item.CCINS,
                    CCNUM = item.CCNUM,
                    SSBLK = item.SSBLK,
                    BATCH = item.BATCH,
                    SNAME = item.SNAME,
                    SAMPLED = item.SAMPLED,
                    EXCLUDE_FLAG = item.EXCLUDE_FLAG,
                    BLIND = item.BLIND,
                    OFFSET_STATUS = item.OFFSET_STATUS,
                    OFFSET_REFER_DAT = item.OFFSET_REFER_DAT,
                    PENRC = item.PENRC,
                    KNUMV = item.KNUMV,
                    OINETNUM = item.OINETNUM,
                    OINJAHR = item.OINJAHR,
                    OININD = item.OININD,
                    RECHN = item.RECHN,
                    PYBASTYP = item.PYBASTYP,
                    PYBASNO = item.PYBASNO,
                    PYBASDAT = item.PYBASDAT,
                    PYIBAN = item.PYIBAN,
                    INWARDNO_HD = item.INWARDNO_HD,
                    INWARDDT_HD = item.INWARDDT_HD
                };
                _context.BKPFNews.Add(bkpfnew);
            }

            _context.SaveChanges();
        }

        public void GetAllBsadAndPutToNewDb()
        {
            var bSADs = _context.BSADs;
            foreach (var item in bSADs)
            {
                var bsadnew = new BSADNew
                {
                    BUKRS = Convert.ToInt32(item.BUKRS),
                    KUNNR = item.KUNNR,
                    UMSKS = item.UMSKS,
                    UMSKZ = item.UMSKZ,
                    AUGDT = item.AUGDT,
                    AUGBL = item.AUGBL,
                    ZUONR = item.ZUONR,
                    GJAHR = Convert.ToInt32(item.GJAHR),
                    BELNR = item.BELNR,
                    BUZEI = item.BUZEI,
                    BUDAT = Convert.ToDateTime(item.BUDAT),
                    BLDAT = Convert.ToDateTime(item.BLDAT),
                    CPUDT = item.CPUDT,
                    WAERS = item.WAERS,
                    XBLNR = item.XBLNR,
                    BLART = item.BLART,
                    MONAT = Convert.ToInt32(item.MONAT),
                    BSCHL = Convert.ToInt32(item.BSCHL),
                    ZUMSK = item.ZUMSK,
                    SHKZG = item.SHKZG,
                    GSBER = item.GSBER,
                    MWSKZ = item.MWSKZ,
                    DMBTR = Convert.ToDouble(item.DMBTR),
                    WRBTR = Convert.ToDouble(item.WRBTR),
                    MWSTS = item.MWSTS,
                    WMWST = item.WMWST,
                    BDIFF = item.BDIFF,
                    BDIF2 = item.BDIF2,
                    SGTXT = item.SGTXT,
                    PROJN = item.PROJN,
                    AUFNR = item.AUFNR,
                    ANLN1 = item.ANLN1,
                    ANLN2 = item.ANLN2,
                    SAKNR = Convert.ToInt32(item.SAKNR),
                    HKONT = Convert.ToInt32(item.HKONT),
                    FKONT = item.FKONT,
                    FILKD = item.FILKD,
                    ZFBDT = Convert.ToDateTime(item.ZFBDT),
                    ZTERM = item.ZTERM,
                    ZBD1T = item.ZBD1T,
                    ZBD2T = item.ZBD2T,
                    ZBD3T = item.ZBD3T,
                    ZBD1P = item.ZBD1P,
                    ZBD2P = item.ZBD2P,
                    SKFBT = item.SKFBT,
                    SKNTO = item.SKNTO,
                    WSKTO = item.WSKTO,
                    ZLSCH = item.ZLSCH,
                    ZLSPR = item.ZLSPR,
                    ZBFIX = item.ZBFIX,
                    HBKID = item.HBKID,
                    BVTYP = item.BVTYP,
                    REBZG = item.REBZG,
                    REBZJ = item.REBZJ,
                    REBZZ = item.REBZZ,
                    SAMNR = item.SAMNR,
                    ANFBN = item.ANFBN,
                    ANFBJ = item.ANFBJ,
                    ANFBU = item.ANFBU,
                    ANFAE = item.ANFAE,
                    MANSP = item.MANSP,
                    MSCHL = item.MSCHL,
                    MADAT = item.MADAT,
                    MANST = item.MANST,
                    MABER = item.MABER,
                    XNETB = item.XNETB,
                    XANET = item.XANET,
                    XCPDD = item.XCPDD,
                    XINVE = item.XINVE,
                    XZAHL = item.XZAHL,
                    MWSK1 = item.MWSK1,
                    DMBT1 = item.DMBT1,
                    WRBT1 = item.WRBT1,
                    MWSK2 = item.MWSK2,
                    DMBT2 = item.DMBT2,
                    WRBT2 = item.WRBT2,
                    MWSK3 = item.MWSK3,
                    DMBT3 = item.DMBT3,
                    WRBT3 = item.WRBT3,
                    BSTAT = item.BSTAT,
                    VBUND = item.VBUND,
                    VBELN = item.VBELN,
                    REBZT = item.REBZT,
                    INFAE = item.INFAE,
                    STCEG = item.STCEG,
                    EGBLD = item.EGBLD,
                    EGLLD = item.EGLLD,
                    RSTGR = item.RSTGR,
                    XNOZA = item.XNOZA,
                    VERTT = item.VERTT,
                    VERTN = item.VERTN,
                    VBEWA = item.VBEWA,
                    WVERW = item.WVERW,
                    PROJK = item.PROJK,
                    FIPOS = item.FIPOS,
                    NPLNR = item.NPLNR,
                    AUFPL = item.AUFPL,
                    APLZL = item.APLZL,
                    XEGDR = item.XEGDR,
                    DMBE2 = item.DMBE2,
                    DMBE3 = item.DMBE3,
                    DMB21 = item.DMB21,
                    DMB22 = item.DMB22,
                    DMB23 = item.DMB23,
                    DMB31 = item.DMB31,
                    DMB32 = item.DMB32,
                    DMB33 = item.DMB33,
                    BDIF3 = item.BDIF3,
                    XRAGL = item.XRAGL,
                    UZAWE = item.UZAWE,
                    XSTOV = item.XSTOV,
                    MWST2 = item.MWST2,
                    MWST3 = item.MWST3,
                    SKNT2 = item.SKNT2,
                    SKNT3 = item.SKNT3,
                    XREF1 = item.XREF1,
                    XREF2 = item.XREF2,
                    XARCH = item.XARCH,
                    PSWSL = item.PSWSL,
                    PSWBT = item.PSWBT,
                    LZBKZ = item.LZBKZ,
                    LANDL = item.LANDL,
                    IMKEY = item.IMKEY,
                    VBEL2 = item.VBEL2,
                    VPOS2 = item.VPOS2,
                    POSN2 = item.POSN2,
                    ETEN2 = item.ETEN2,
                    FISTL = item.FISTL,
                    GEBER = item.GEBER,
                    DABRZ = item.DABRZ,
                    XNEGP = item.XNEGP,
                    KOSTL = item.KOSTL,
                    RFZEI = item.RFZEI,
                    KKBER = item.KKBER,
                    EMPFB = item.EMPFB,
                    PRCTR = item.PRCTR,
                    XREF3 = item.XREF3,
                    QSSKZ = item.QSSKZ,
                    ZINKZ = item.ZINKZ,
                    DTWS1 = item.DTWS1,
                    DTWS2 = item.DTWS2,
                    DTWS3 = item.DTWS3,
                    DTWS4 = item.DTWS4,
                    XPYPR = item.XPYPR,
                    KIDNO = item.KIDNO,
                    ABSBT = item.ABSBT,
                    CCBTC = item.CCBTC,
                    PYCUR = item.PYCUR,
                    PYAMT = item.PYAMT,
                    BUPLA = item.BUPLA,
                    SECCO = item.SECCO,
                    CESSION_KZ = item.CESSION_KZ,
                    PPDIFF = item.PPDIFF,
                    PPDIF2 = item.PPDIF2,
                    PPDIF3 = item.PPDIF3,
                    KBLNR = item.KBLNR,
                    KBLPOS = item.KBLPOS,
                    GRANT_NBR = item.GRANT_NBR,
                    GMVKZ = item.GMVKZ,
                    SRTYPE = item.SRTYPE,
                    LOTKZ = item.LOTKZ,
                    FKBER = item.FKBER,
                    PPRCT = item.PPRCT,
                    BUZID = item.BUZID,
                    AUGGJ = item.AUGGJ,
                    HKTID = item.HKTID,
                    BUDGET_PD = item.BUDGET_PD,
                    PAYS_PROV = item.PAYS_PROV,
                    PAYS_TRAN = item.PAYS_TRAN,
                    MNDID = item.MNDID,
                    KONTT = item.KONTT,
                    KONTL = item.KONTL,
                    UEBGDAT = item.UEBGDAT,
                    VNAME = item.VNAME,
                    EGRUP = item.EGRUP,
                    BTYPE = item.BTYPE,
                    OIEXGNUM = item.OIEXGNUM,
                    OINETCYC = item.OINETCYC,
                    OIEXGTYP = item.OIEXGTYP,
                    PROPMANO = item.PROPMANO
                };
                _context.BSADNews.Add(bsadnew);
            }

            _context.SaveChanges();
        }

        public void GetAllBsegAndPutToNewDb()
        {
            var bSEGs = _context.BSEGs;

            foreach (var item in bSEGs)
            {
                var bsegnew = new BSEGNew
                {
                    BUKRS = Convert.ToInt32(item.BUKRS),
                    BELNR = Convert.ToInt32(item.BELNR),
                    GJAHR = Convert.ToInt32(item.GJAHR),
                    KOART = item.KOART,
                    BUZEI = item.BUZEI,
                    HKONT = Convert.ToInt32(item.HKONT),
                    SGTXT = item.SGTXT,
                    SHKZG = item.SHKZG,
                    DMBTR = Convert.ToDouble(item.DMBTR),
                    WRBTR = Convert.ToDouble(item.WRBTR)
                };
                _context.BSEGNews.Add(bsegnew);
            }

            _context.SaveChanges();
        }

        public void GetAllBsidAndPutToNewDb()
        {
            var bSIDs = _context.BSIDs.Where(i => i.KUNNR == SoaVM.Customer.Code);
            foreach (var item in bSIDs)
            {
                var bsidnew = new BSIDNew
                {
                    BUKRS = Convert.ToInt32(item.BUKRS),
                    KUNNR = item.KUNNR,
                    UMSKS = item.UMSKS,
                    UMSKZ = item.UMSKZ,
                    AUGDT = item.AUGDT,
                    AUGBL = item.AUGBL,
                    ZUONR = item.ZUONR,
                    GJAHR = Convert.ToInt32(item.GJAHR),
                    BELNR = item.BELNR,
                    BUZEI = item.BUZEI,
                    BUDAT = Convert.ToDateTime(item.BUDAT),
                    BLDAT = Convert.ToDateTime(item.BLDAT),
                    CPUDT = item.CPUDT,
                    WAERS = item.WAERS,
                    XBLNR = item.XBLNR,
                    BLART = item.BLART,
                    MONAT = Convert.ToInt32(item.MONAT),
                    BSCHL = Convert.ToInt32(item.BSCHL),
                    ZUMSK = item.ZUMSK,
                    SHKZG = item.SHKZG,
                    GSBER = item.GSBER,
                    MWSKZ = item.MWSKZ,
                    DMBTR = Convert.ToDouble(item.DMBTR),
                    WRBTR = Convert.ToDouble(item.WRBTR),
                    MWSTS = item.MWSTS,
                    WMWST = item.WMWST,
                    BDIFF = item.BDIFF,
                    BDIF2 = item.BDIF2,
                    SGTXT = item.SGTXT,
                    PROJN = item.PROJN,
                    AUFNR = item.AUFNR,
                    ANLN1 = item.ANLN1,
                    ANLN2 = item.ANLN2,
                    SAKNR = Convert.ToInt32(item.SAKNR),
                    HKONT = Convert.ToInt32(item.HKONT),
                    FKONT = item.FKONT,
                    FILKD = item.FILKD,
                    ZFBDT = Convert.ToDateTime(item.ZFBDT),
                    ZTERM = item.ZTERM,
                    ZBD1T = item.ZBD1T,
                    ZBD2T = item.ZBD2T,
                    ZBD3T = item.ZBD3T,
                    ZBD1P = item.ZBD1P,
                    ZBD2P = item.ZBD2P,
                    SKFBT = item.SKFBT,
                    SKNTO = item.SKNTO,
                    WSKTO = item.WSKTO,
                    ZLSCH = item.ZLSCH,
                    ZLSPR = item.ZLSPR,
                    ZBFIX = item.ZBFIX,
                    HBKID = item.HBKID,
                    BVTYP = item.BVTYP,
                    REBZG = item.REBZG,
                    REBZJ = item.REBZJ,
                    REBZZ = item.REBZZ,
                    SAMNR = item.SAMNR,
                    ANFBN = item.ANFBN,
                    ANFBJ = item.ANFBJ,
                    ANFBU = item.ANFBU,
                    ANFAE = item.ANFAE,
                    MANSP = item.MANSP,
                    MSCHL = item.MSCHL,
                    MADAT = item.MADAT,
                    MANST = item.MANST,
                    MABER = item.MABER,
                    XNETB = item.XNETB,
                    XANET = item.XANET,
                    XCPDD = item.XCPDD,
                    XINVE = item.XINVE,
                    XZAHL = item.XZAHL,
                    MWSK1 = item.MWSK1,
                    DMBT1 = item.DMBT1,
                    WRBT1 = item.WRBT1,
                    MWSK2 = item.MWSK2,
                    DMBT2 = item.DMBT2,
                    WRBT2 = item.WRBT2,
                    MWSK3 = item.MWSK3,
                    DMBT3 = item.DMBT3,
                    WRBT3 = item.WRBT3,
                    BSTAT = item.BSTAT,
                    VBUND = item.VBUND,
                    VBELN = item.VBELN,
                    REBZT = item.REBZT,
                    INFAE = item.INFAE,
                    STCEG = item.STCEG,
                    EGBLD = item.EGBLD,
                    EGLLD = item.EGLLD,
                    RSTGR = item.RSTGR,
                    XNOZA = item.XNOZA,
                    VERTT = item.VERTT,
                    VERTN = item.VERTN,
                    VBEWA = item.VBEWA,
                    WVERW = item.WVERW,
                    PROJK = item.PROJK,
                    FIPOS = item.FIPOS,
                    NPLNR = item.NPLNR,
                    AUFPL = item.AUFPL,
                    APLZL = item.APLZL,
                    XEGDR = item.XEGDR,
                    DMBE2 = item.DMBE2,
                    DMBE3 = item.DMBE3,
                    DMB21 = item.DMB21,
                    DMB22 = item.DMB22,
                    DMB23 = item.DMB23,
                    DMB31 = item.DMB31,
                    DMB32 = item.DMB32,
                    DMB33 = item.DMB33,
                    BDIF3 = item.BDIF3,
                    XRAGL = item.XRAGL,
                    UZAWE = item.UZAWE,
                    XSTOV = item.XSTOV,
                    MWST2 = item.MWST2,
                    MWST3 = item.MWST3,
                    SKNT2 = item.SKNT2,
                    SKNT3 = item.SKNT3,
                    XREF1 = item.XREF1,
                    XREF2 = item.XREF2,
                    XARCH = item.XARCH,
                    PSWSL = item.PSWSL,
                    PSWBT = item.PSWBT,
                    LZBKZ = item.LZBKZ,
                    LANDL = item.LANDL,
                    IMKEY = item.IMKEY,
                    VBEL2 = item.VBEL2,
                    VPOS2 = item.VPOS2,
                    POSN2 = item.POSN2,
                    ETEN2 = item.ETEN2,
                    FISTL = item.FISTL,
                    GEBER = item.GEBER,
                    DABRZ = item.DABRZ,
                    XNEGP = item.XNEGP,
                    KOSTL = item.KOSTL,
                    RFZEI = item.RFZEI,
                    KKBER = item.KKBER,
                    EMPFB = item.EMPFB,
                    PRCTR = item.PRCTR,
                    XREF3 = item.XREF3,
                    QSSKZ = item.QSSKZ,
                    ZINKZ = item.ZINKZ,
                    DTWS1 = item.DTWS1,
                    DTWS2 = item.DTWS2,
                    DTWS3 = item.DTWS3,
                    DTWS4 = item.DTWS4,
                    XPYPR = item.XPYPR,
                    KIDNO = item.KIDNO,
                    ABSBT = item.ABSBT,
                    CCBTC = item.CCBTC,
                    PYCUR = item.PYCUR,
                    PYAMT = item.PYAMT,
                    BUPLA = item.BUPLA,
                    SECCO = item.SECCO,
                    CESSION_KZ = item.CESSION_KZ,
                    PPDIFF = item.PPDIFF,
                    PPDIF2 = item.PPDIF2,
                    PPDIF3 = item.PPDIF3,
                    KBLNR = item.KBLNR,
                    KBLPOS = item.KBLPOS,
                    GRANT_NBR = item.GRANT_NBR,
                    GMVKZ = item.GMVKZ,
                    SRTYPE = item.SRTYPE,
                    LOTKZ = item.LOTKZ,
                    FKBER = item.FKBER,
                    PPRCT = item.PPRCT,
                    BUZID = item.BUZID,
                    AUGGJ = item.AUGGJ,
                    HKTID = item.HKTID,
                    BUDGET_PD = item.BUDGET_PD,
                    PAYS_PROV = item.PAYS_PROV,
                    PAYS_TRAN = item.PAYS_TRAN,
                    MNDID = item.MNDID,
                    KONTT = item.KONTT,
                    KONTL = item.KONTL,
                    UEBGDAT = item.UEBGDAT,
                    VNAME = item.VNAME,
                    EGRUP = item.EGRUP,
                    BTYPE = item.BTYPE,
                    OIEXGNUM = item.OIEXGNUM,
                    OINETCYC = item.OINETCYC,
                    OIEXGTYP = item.OIEXGTYP,
                    PROPMANO = item.PROPMANO
                };
                _context.BSIDNews.Add(bsidnew);
            }

            _context.SaveChanges();
        }

        private void GetAllRBkpfAndPutToNewDb()
        {
            var r_BKPFs = _context.R_BKPFs;
            foreach (var item in r_BKPFs)
            {
                var r_bkpfnew = new R_BKPFNew
                {
                    BUKRS = Convert.ToInt32(item.BUKRS),
                    BELNR = Convert.ToInt32(item.BELNR),
                    GJAHR = Convert.ToInt32(item.GJAHR),
                    BLART = item.BLART,
                    BLDAT = Convert.ToDateTime(item.BLDAT),
                    BUDAT = Convert.ToDateTime(item.BUDAT),
                    MONAT = Convert.ToInt32(item.MONAT),
                    CPUDT = item.CPUDT,
                    CPUTM = item.CPUTM,
                    AEDAT = item.AEDAT,
                    UPDDT = item.UPDDT,
                    WWERT = item.WWERT,
                    USNAM = item.USNAM,
                    TCODE = item.TCODE,
                    BVORG = item.BVORG,
                    XBLNR = item.XBLNR,
                    DBBLG = item.DBBLG,
                    STBLG = item.STBLG,
                    STJAH = item.STJAH,
                    BKTXT = item.BKTXT,
                    WAERS = item.WAERS,
                    KURSF = item.KURSF,
                    KZWRS = item.KZWRS,
                    KZKRS = item.KZKRS,
                    BSTAT = item.BSTAT,
                    XNETB = item.XNETB,
                    FRATH = item.FRATH,
                    XRUEB = item.XRUEB,
                    GLVOR = item.GLVOR,
                    GRPID = item.GRPID,
                    DOKID = item.DOKID,
                    ARCID = item.ARCID,
                    IBLAR = item.IBLAR,
                    AWTYP = item.AWTYP,
                    AWKEY = item.AWKEY,
                    FIKRS = item.FIKRS,
                    HWAER = item.HWAER,
                    HWAE2 = item.HWAE2,
                    HWAE3 = item.HWAE3,
                    KURS2 = item.KURS2,
                    KURS3 = item.KURS3,
                    BASW2 = item.BASW2,
                    BASW3 = item.BASW3,
                    UMRD2 = item.UMRD2,
                    UMRD3 = item.UMRD3,
                    XSTOV = item.XSTOV,
                    STODT = item.STODT,
                    XMWST = item.XMWST,
                    CURT2 = item.CURT2,
                    CURT3 = item.CURT3,
                    KUTY2 = item.KUTY2,
                    KUTY3 = item.KUTY3,
                    XSNET = item.XSNET,
                    AUSBK = item.AUSBK,
                    XUSVR = item.XUSVR,
                    DUEFL = item.DUEFL,
                    AWSYS = item.AWSYS,
                    TXKRS = item.TXKRS,
                    CTXKRS = item.CTXKRS,
                    LOTKZ = item.LOTKZ,
                    XWVOF = item.XWVOF,
                    STGRD = item.STGRD,
                    PPNAM = item.PPNAM,
                    PPDAT = item.PPDAT,
                    PPTME = item.PPTME,
                    PPTCOD = item.PPTCOD,
                    BRNCH = item.BRNCH,
                    NUMPG = item.NUMPG,
                    ADISC = item.ADISC,
                    XREF1_HD = item.XREF1_HD,
                    XREF2_HD = item.XREF2_HD,
                    XREVERSAL = item.XREVERSAL,
                    REINDAT = item.REINDAT,
                    RLDNR = item.RLDNR,
                    LDGRP = item.LDGRP,
                    PROPMANO = item.PROPMANO,
                    XBLNR_ALT = item.XBLNR_ALT,
                    VATDATE = item.VATDATE,
                    DOCCAT = item.DOCCAT,
                    XSPLIT = item.XSPLIT,
                    CASH_ALLOC = item.CASH_ALLOC,
                    FOLLOW_ON = item.FOLLOW_ON,
                    XREORG = item.XREORG,
                    SUBSET = item.SUBSET,
                    KURST = item.KURST,
                    KURSX = item.KURSX,
                    KUR2X = item.KUR2X,
                    KUR3X = item.KUR3X,
                    XMCA = item.XMCA,
                    RESUBMISSION = item.RESUBMISSION,
                    SAPF15_STATUS = item.SAPF15_STATUS,
                    PSOTY = item.PSOTY,
                    PSOAK = item.PSOAK,
                    PSOKS = item.PSOKS,
                    PSOSG = item.PSOSG,
                    PSOFN = item.PSOFN,
                    PSOBT = item.PSOBT,
                    PSOZL = item.PSOZL,
                    PSODT = item.PSODT,
                    PSOTM = item.PSOTM,
                    FM_UMART = item.FM_UMART,
                    CCINS = item.CCINS,
                    CCNUM = item.CCNUM,
                    SSBLK = item.SSBLK,
                    BATCH = item.BATCH,
                    SNAME = item.SNAME,
                    SAMPLED = item.SAMPLED,
                    EXCLUDE_FLAG = item.EXCLUDE_FLAG,
                    BLIND = item.BLIND,
                    OFFSET_STATUS = item.OFFSET_STATUS,
                    OFFSET_REFER_DAT = item.OFFSET_REFER_DAT,
                    PENRC = item.PENRC,
                    KNUMV = item.KNUMV,
                    OINETNUM = item.OINETNUM,
                    OINJAHR = item.OINJAHR,
                    OININD = item.OININD,
                    RECHN = item.RECHN,
                    PYBASTYP = item.PYBASTYP,
                    PYBASNO = item.PYBASNO,
                    PYBASDAT = item.PYBASDAT,
                    PYIBAN = item.PYIBAN,
                    INWARDNO_HD = item.INWARDNO_HD,
                    INWARDDT_HD = item.INWARDDT_HD
                };
                _context.R_BKPFNews.Add(r_bkpfnew);
            }

            _context.SaveChanges();
        }

        private void GetTotalAmountDue()
        {
            SD.TotalAmountDue = ((SD.UpTotalAmount + SD.WhTotalAmount) + SD.DebitCreditTotalAmount) - SD.TotalPayments;
        }

        private void GetCurrentPreviousBill(DateTime asof)
        {
            var currentPrevBill = _context.BSIDNews
                .Where(i => i.KUNNR == SoaVM.Customer.Code);

            // Current Bills
            foreach (var bill in currentPrevBill)
            {
                if (bill.BUDAT >= DateMangement.GetFirstDayOfCurrentMonth(asof) && bill.BUDAT <= asof)
                {
                    if (bill.SHKZG == "S")
                    {
                        SD.CurrentBillsTotal += Convert.ToDouble(bill.WRBTR);
                    }
                    else
                    {
                        SD.CurrentBillsTotal -= Convert.ToDouble(bill.WRBTR);
                    }
                }
            }

            foreach (var bill in currentPrevBill)
            {
                if (bill.BUDAT <= DateMangement.GetPreviousMonthLastDay(asof))
                {
                    if (bill.SHKZG == "S")
                    {
                        SD.PreviousBillsTotal += Convert.ToDouble(bill.WRBTR);
                    }
                    else
                    {
                        SD.PreviousBillsTotal -= Convert.ToDouble(bill.WRBTR);
                    }
                }
            }
            //SD.PreviousBillsTotal += SD.WhTotalAmount;
            //SD.PreviousBillsTotal += SD.WhTotalAmount;
        }

        private void GetCreditDebitDetails()
        {
            // get all credit and debit from table
            var bsidCmdm = _context.BSIDs.Where(i => (i.UMSKZ == "" || i.UMSKZ != "C") &&
                                                        (i.KUNNR == SoaVM.Customer.Code) &&
                                                        (i.BLART == "P8" ||
                                                            i.BLART == "DG" ||
                                                            i.BLART == "DJ" ||
                                                            i.BLART == "DH" ||
                                                            i.BLART == "DM"));
            // put list of cwt items to the webpage
            ViewBag.BSID_CMDM = bsidCmdm;

            // get all credit items
            var bsidCredit = _context.BSIDs.Where(i =>
                                    (i.UMSKZ == "" || i.UMSKZ != "C") &&
                                    (i.KUNNR == SoaVM.Customer.Code) &&
                                    (i.SHKZG == "H") &&
                                    (i.BLART == "P8" ||
                                        i.BLART == "DG" ||
                                        i.BLART == "DJ"));

            // sum of all credit
            foreach (var credit in bsidCredit)
            {
                SD.CreditTotalAmount += Convert.ToDouble(credit.DMBTR);
            }

            // get all debit items
            var bsidDebit = _context.BSIDs.Where(i =>
                                    (i.UMSKZ == "" || i.UMSKZ != "C") &&
                                    (i.KUNNR == SoaVM.Customer.Code) &&
                                    (i.SHKZG == "S") &&
                                    (i.BLART == "DH" || i.BLART == "DM"));

            // sum of all debit items
            foreach (var debit in bsidDebit)
            {
                SD.DebitTotalAmount += Convert.ToDouble(debit.DMBTR);
            }

            // debit and credit total = debit amount - credit amount
            SD.DebitCreditTotalAmount = SD.DebitTotalAmount - SD.CreditTotalAmount;
        }

        private void GetUnpaidDetails()
        {
            //getting Unpaid items
            var unpaidItems = _context.BSIDs.Where(i => (i.UMSKZ == "" || i.UMSKZ != "C") &&
                                                        (i.KUNNR == SoaVM.Customer.Code) &&
                                                        !(i.BLART == "P8" ||
                                                          i.BLART == "DG" ||
                                                          i.BLART == "DJ" ||
                                                          i.BLART == "DH" ||
                                                          i.BLART == "DM")).OrderByDescending(i => i.ZFBDT);

            // put list of cwt items to the webpage
            ViewBag.BSID_UNPAID = unpaidItems;

            // sum of all unpaid transaction
            foreach (var unpaid in unpaidItems)
            {
                if (unpaid.SHKZG == "S")
                {
                    SD.UpAmount += Convert.ToDouble(unpaid.DMBTR);
                }
                else
                {
                    SD.PAmount += Convert.ToDouble(unpaid.DMBTR);
                }
            }

            SD.UpTotalAmount = SD.UpAmount - SD.PAmount;
        }

        private void GetCwtDetails()
        {
            // get all cwt from database
            var bsidCwt = _context.BSIDs.Where(i => (i.UMSKZ == "C") && (i.KUNNR == SoaVM.Customer.Code));

            // put list of cwt items to the webpage
            ViewBag.BSID_CWT = bsidCwt;

            // get the total amount of cwt
            foreach (var cwt in bsidCwt)
            {
                if (cwt.SHKZG == "H")
                {
                    SD.WhPaidAmount += Convert.ToDouble(cwt.DMBTR);
                }
                else
                {
                    SD.WhUnpaidAmount += Convert.ToDouble(cwt.DMBTR);
                }
                SD.WhTotalAmount = SD.WhUnpaidAmount - SD.WhPaidAmount;
            }
        }

        private void PaymentComputation()
        {
            var bsidBsad = _context.Payments
                .Where(i =>
                        (i.UMSKZ == "" || i.UMSKZ != "C") &&
                        (i.KUNNR == SoaVM.Customer.Code) &&
                        (
                            i.BLART == "DX" ||
                            i.BLART == "DY" ||
                            i.BLART == "DZ"
                        ));

            foreach (var item in bsidBsad)
            {
                if (item.SHKZG == "H")
                {
                    SD.UnpaidPayments += Convert.ToDouble(item.DMBTR);
                }
                else
                {
                    SD.PaidPayments += Convert.ToDouble(item.DMBTR);
                }
            }

            SD.TotalPayments = SD.UnpaidPayments - SD.PaidPayments;
        }

        private void GetPaymentsData(DateTime asof)
        {
            AddBsadToPaymentTable(asof);
            AddBsidToPaymentTable(asof);
            PaymentComputation();

            var bsidBsad = _context.Payments
                .Where(i =>
                        (i.UMSKZ == "" || i.UMSKZ != "C") &&
                        (i.KUNNR == SoaVM.Customer.Code) &&
                        (
                            i.BLART == "DX" ||
                            i.BLART == "DY" ||
                            i.BLART == "DZ"
                        ))
                .OrderByDescending(i => i.ZFBDT);

            ViewBag.DxDyDz = bsidBsad;
        }

        private void ClearDbTables()
        {
            var payments = _context.Payments;
            var bkpfNewDel = _context.BKPFNews;
            var bsadNewDel = _context.BSADNews;
            var bsegNewDel = _context.BSEGNews;
            var bsidNewDel = _context.BSIDNews;
            var rBkpfNewDel = _context.R_BKPFNews;

            //foreach (var item in payments) { _context.Payments.Remove(item); }
            //_context.SaveChanges();
            //foreach (var item in bkpfNewDel) { _context.BKPFNews.Remove(item); }
            //_context.SaveChanges();
            //foreach (var item in bsadNewDel) { _context.BSADNews.Remove(item); }
            //_context.SaveChanges();
            //foreach (var item in bsegNewDel) { _context.BSEGNews.Remove(item); }
            //_context.SaveChanges();
            foreach (var item in bsidNewDel) { _context.BSIDNews.Remove(item); }
            _context.SaveChanges();
            //foreach (var item in rBkpfNewDel) { _context.R_BKPFNews.Remove(item); }
            //_context.SaveChanges();

            ViewBag.DxDyDz = null;
        }

        private void AddBsadToPaymentTable(DateTime asof)
        {
            SoaVM = new SoaVM()
            {
                Customer = new Models.Customer()
            };

            var testdata = SoaVM.Customer.Code;

            var bsadDxdydz = _context.BSADNews
                .Where(i =>
                        (i.UMSKZ == "" || i.UMSKZ != "C") &&
                        (i.GJAHR == asof.Year) &&
                        (i.KUNNR == SoaVM.Customer.Code) &&
                        (
                            i.BLART == "DX" ||
                            i.BLART == "DY" ||
                            i.BLART == "DZ"
                        ));

            foreach (var bsad in bsadDxdydz)
            {
                var payment = new Payment
                {
                    BUKRS = bsad.BUKRS,
                    KUNNR = bsad.KUNNR,
                    UMSKS = bsad.UMSKS,
                    UMSKZ = bsad.UMSKZ,
                    AUGDT = bsad.AUGDT,
                    AUGBL = bsad.AUGBL,
                    ZUONR = bsad.ZUONR,
                    GJAHR = bsad.GJAHR,
                    BELNR = bsad.BELNR,
                    BUZEI = bsad.BUZEI,
                    BUDAT = bsad.BUDAT,
                    BLDAT = bsad.BLDAT,
                    CPUDT = bsad.CPUDT,
                    WAERS = bsad.WAERS,
                    XBLNR = bsad.XBLNR,
                    BLART = bsad.BLART,
                    MONAT = bsad.MONAT,
                    BSCHL = bsad.BSCHL,
                    ZUMSK = bsad.ZUMSK,
                    SHKZG = bsad.SHKZG,
                    GSBER = bsad.GSBER,
                    MWSKZ = bsad.MWSKZ,
                    DMBTR = bsad.DMBTR,
                    WRBTR = bsad.WRBTR,
                    MWSTS = bsad.MWSTS,
                    WMWST = bsad.WMWST,
                    BDIFF = bsad.BDIFF,
                    BDIF2 = bsad.BDIF2,
                    SGTXT = bsad.SGTXT,
                    PROJN = bsad.PROJN,
                    AUFNR = bsad.AUFNR,
                    ANLN1 = bsad.ANLN1,
                    ANLN2 = bsad.ANLN2,
                    SAKNR = bsad.SAKNR,
                    HKONT = bsad.HKONT,
                    FKONT = bsad.FKONT,
                    FILKD = bsad.FILKD,
                    ZFBDT = bsad.ZFBDT,
                    ZTERM = bsad.ZTERM,
                    ZBD1T = bsad.ZBD1T,
                    ZBD2T = bsad.ZBD2T,
                    ZBD3T = bsad.ZBD3T,
                    ZBD1P = bsad.ZBD1P,
                    ZBD2P = bsad.ZBD2P,
                    SKFBT = bsad.SKFBT,
                    SKNTO = bsad.SKNTO,
                    WSKTO = bsad.WSKTO,
                    ZLSCH = bsad.ZLSCH,
                    ZLSPR = bsad.ZLSPR,
                    ZBFIX = bsad.ZBFIX,
                    HBKID = bsad.HBKID,
                    BVTYP = bsad.BVTYP,
                    REBZG = bsad.REBZG,
                    REBZJ = bsad.REBZJ,
                    REBZZ = bsad.REBZZ,
                    SAMNR = bsad.SAMNR,
                    ANFBN = bsad.ANFBN,
                    ANFBJ = bsad.ANFBJ,
                    ANFBU = bsad.ANFBU,
                    ANFAE = bsad.ANFAE,
                    MANSP = bsad.MANSP,
                    MSCHL = bsad.MSCHL,
                    MADAT = bsad.MADAT,
                    MANST = bsad.MANST,
                    MABER = bsad.MABER,
                    XNETB = bsad.XNETB,
                    XANET = bsad.XANET,
                    XCPDD = bsad.XCPDD,
                    XINVE = bsad.XINVE,
                    XZAHL = bsad.XZAHL,
                    MWSK1 = bsad.MWSK1,
                    DMBT1 = bsad.DMBT1,
                    WRBT1 = bsad.WRBT1,
                    MWSK2 = bsad.MWSK2,
                    DMBT2 = bsad.DMBT2,
                    WRBT2 = bsad.WRBT2,
                    MWSK3 = bsad.MWSK3,
                    DMBT3 = bsad.DMBT3,
                    WRBT3 = bsad.WRBT3,
                    BSTAT = bsad.BSTAT,
                    VBUND = bsad.VBUND,
                    VBELN = bsad.VBELN,
                    REBZT = bsad.REBZT,
                    INFAE = bsad.INFAE,
                    STCEG = bsad.STCEG,
                    EGBLD = bsad.EGBLD,
                    EGLLD = bsad.EGLLD,
                    RSTGR = bsad.RSTGR,
                    XNOZA = bsad.XNOZA,
                    VERTT = bsad.VERTT,
                    VERTN = bsad.VERTN,
                    VBEWA = bsad.VBEWA,
                    WVERW = bsad.WVERW,
                    PROJK = bsad.PROJK,
                    FIPOS = bsad.FIPOS,
                    NPLNR = bsad.NPLNR,
                    AUFPL = bsad.AUFPL,
                    APLZL = bsad.APLZL,
                    XEGDR = bsad.XEGDR,
                    DMBE2 = bsad.DMBE2,
                    DMBE3 = bsad.DMBE3,
                    DMB21 = bsad.DMB21,
                    DMB22 = bsad.DMB22,
                    DMB23 = bsad.DMB23,
                    DMB31 = bsad.DMB31,
                    DMB32 = bsad.DMB32,
                    DMB33 = bsad.DMB33,
                    BDIF3 = bsad.BDIF3,
                    XRAGL = bsad.XRAGL,
                    UZAWE = bsad.UZAWE,
                    XSTOV = bsad.XSTOV,
                    MWST2 = bsad.MWST2,
                    MWST3 = bsad.MWST3,
                    SKNT2 = bsad.SKNT2,
                    SKNT3 = bsad.SKNT3,
                    XREF1 = bsad.XREF1,
                    XREF2 = bsad.XREF2,
                    XARCH = bsad.XARCH,
                    PSWSL = bsad.PSWSL,
                    PSWBT = bsad.PSWBT,
                    LZBKZ = bsad.LZBKZ,
                    LANDL = bsad.LANDL,
                    IMKEY = bsad.IMKEY,
                    VBEL2 = bsad.VBEL2,
                    VPOS2 = bsad.VPOS2,
                    POSN2 = bsad.POSN2,
                    ETEN2 = bsad.ETEN2,
                    FISTL = bsad.FISTL,
                    GEBER = bsad.GEBER,
                    DABRZ = bsad.DABRZ,
                    XNEGP = bsad.XNEGP,
                    KOSTL = bsad.KOSTL,
                    RFZEI = bsad.RFZEI,
                    KKBER = bsad.KKBER,
                    EMPFB = bsad.EMPFB,
                    PRCTR = bsad.PRCTR,
                    XREF3 = bsad.XREF3,
                    QSSKZ = bsad.QSSKZ,
                    ZINKZ = bsad.ZINKZ,
                    DTWS1 = bsad.DTWS1,
                    DTWS2 = bsad.DTWS2,
                    DTWS3 = bsad.DTWS3,
                    DTWS4 = bsad.DTWS4,
                    XPYPR = bsad.XPYPR,
                    KIDNO = bsad.KIDNO,
                    ABSBT = bsad.ABSBT,
                    CCBTC = bsad.CCBTC,
                    PYCUR = bsad.PYCUR,
                    PYAMT = bsad.PYAMT,
                    BUPLA = bsad.BUPLA,
                    SECCO = bsad.SECCO,
                    CESSION_KZ = bsad.CESSION_KZ,
                    PPDIFF = bsad.PPDIFF,
                    PPDIF2 = bsad.PPDIF2,
                    PPDIF3 = bsad.PPDIF3,
                    KBLNR = bsad.KBLNR,
                    KBLPOS = bsad.KBLPOS,
                    GRANT_NBR = bsad.GRANT_NBR,
                    GMVKZ = bsad.GMVKZ,
                    SRTYPE = bsad.SRTYPE,
                    LOTKZ = bsad.LOTKZ,
                    FKBER = bsad.FKBER,
                    INTRENO = bsad.INTRENO,
                    PPRCT = bsad.PPRCT,
                    BUZID = bsad.BUZID,
                    AUGGJ = bsad.AUGGJ,
                    HKTID = bsad.HKTID,
                    BUDGET_PD = bsad.BUDGET_PD,
                    PAYS_PROV = bsad.PAYS_PROV,
                    PAYS_TRAN = bsad.PAYS_TRAN,
                    MNDID = bsad.MNDID,
                    KONTT = bsad.KONTT,
                    KONTL = bsad.KONTL,
                    UEBGDAT = bsad.UEBGDAT,
                    VNAME = bsad.VNAME,
                    EGRUP = bsad.EGRUP,
                    BTYPE = bsad.BTYPE,
                    OIEXGNUM = bsad.OIEXGNUM,
                    OINETCYC = bsad.OINETCYC,
                    OIEXGTYP = bsad.OIEXGTYP,
                    PROPMANO = bsad.PROPMANO
                };

                _context.Add(payment);
            }

            _context.SaveChanges();
        }

        private void AddBsidToPaymentTable(DateTime asof)
        {
            var bsidDxdydz = _context.BSIDNews
               .Where(i =>
                       (i.UMSKZ == "" || i.UMSKZ != "C") &&
                       (i.GJAHR == asof.Year) &&
                       (i.KUNNR == SoaVM.Customer.Code) &&
                       (
                           i.BLART == "DX" ||
                           i.BLART == "DY" ||
                           i.BLART == "DZ"
                       ));

            foreach (var bsid in bsidDxdydz)
            {
                var payment = new Payment
                {
                    BUKRS = bsid.BUKRS,
                    KUNNR = bsid.KUNNR,
                    UMSKS = bsid.UMSKS,
                    UMSKZ = bsid.UMSKZ,
                    AUGDT = bsid.AUGDT,
                    AUGBL = bsid.AUGBL,
                    ZUONR = bsid.ZUONR,
                    GJAHR = bsid.GJAHR,
                    BELNR = bsid.BELNR,
                    BUZEI = bsid.BUZEI,
                    BUDAT = bsid.BUDAT,
                    BLDAT = bsid.BLDAT,
                    CPUDT = bsid.CPUDT,
                    WAERS = bsid.WAERS,
                    XBLNR = bsid.XBLNR,
                    BLART = bsid.BLART,
                    MONAT = bsid.MONAT,
                    BSCHL = bsid.BSCHL,
                    ZUMSK = bsid.ZUMSK,
                    SHKZG = bsid.SHKZG,
                    GSBER = bsid.GSBER,
                    MWSKZ = bsid.MWSKZ,
                    DMBTR = bsid.DMBTR,
                    WRBTR = bsid.WRBTR,
                    MWSTS = bsid.MWSTS,
                    WMWST = bsid.WMWST,
                    BDIFF = bsid.BDIFF,
                    BDIF2 = bsid.BDIF2,
                    SGTXT = bsid.SGTXT,
                    PROJN = bsid.PROJN,
                    AUFNR = bsid.AUFNR,
                    ANLN1 = bsid.ANLN1,
                    ANLN2 = bsid.ANLN2,
                    SAKNR = bsid.SAKNR,
                    HKONT = bsid.HKONT,
                    FKONT = bsid.FKONT,
                    FILKD = bsid.FILKD,
                    ZFBDT = bsid.ZFBDT,
                    ZTERM = bsid.ZTERM,
                    ZBD1T = bsid.ZBD1T,
                    ZBD2T = bsid.ZBD2T,
                    ZBD3T = bsid.ZBD3T,
                    ZBD1P = bsid.ZBD1P,
                    ZBD2P = bsid.ZBD2P,
                    SKFBT = bsid.SKFBT,
                    SKNTO = bsid.SKNTO,
                    WSKTO = bsid.WSKTO,
                    ZLSCH = bsid.ZLSCH,
                    ZLSPR = bsid.ZLSPR,
                    ZBFIX = bsid.ZBFIX,
                    HBKID = bsid.HBKID,
                    BVTYP = bsid.BVTYP,
                    REBZG = bsid.REBZG,
                    REBZJ = bsid.REBZJ,
                    REBZZ = bsid.REBZZ,
                    SAMNR = bsid.SAMNR,
                    ANFBN = bsid.ANFBN,
                    ANFBJ = bsid.ANFBJ,
                    ANFBU = bsid.ANFBU,
                    ANFAE = bsid.ANFAE,
                    MANSP = bsid.MANSP,
                    MSCHL = bsid.MSCHL,
                    MADAT = bsid.MADAT,
                    MANST = bsid.MANST,
                    MABER = bsid.MABER,
                    XNETB = bsid.XNETB,
                    XANET = bsid.XANET,
                    XCPDD = bsid.XCPDD,
                    XINVE = bsid.XINVE,
                    XZAHL = bsid.XZAHL,
                    MWSK1 = bsid.MWSK1,
                    DMBT1 = bsid.DMBT1,
                    WRBT1 = bsid.WRBT1,
                    MWSK2 = bsid.MWSK2,
                    DMBT2 = bsid.DMBT2,
                    WRBT2 = bsid.WRBT2,
                    MWSK3 = bsid.MWSK3,
                    DMBT3 = bsid.DMBT3,
                    WRBT3 = bsid.WRBT3,
                    BSTAT = bsid.BSTAT,
                    VBUND = bsid.VBUND,
                    VBELN = bsid.VBELN,
                    REBZT = bsid.REBZT,
                    INFAE = bsid.INFAE,
                    STCEG = bsid.STCEG,
                    EGBLD = bsid.EGBLD,
                    EGLLD = bsid.EGLLD,
                    RSTGR = bsid.RSTGR,
                    XNOZA = bsid.XNOZA,
                    VERTT = bsid.VERTT,
                    VERTN = bsid.VERTN,
                    VBEWA = bsid.VBEWA,
                    WVERW = bsid.WVERW,
                    PROJK = bsid.PROJK,
                    FIPOS = bsid.FIPOS,
                    NPLNR = bsid.NPLNR,
                    AUFPL = bsid.AUFPL,
                    APLZL = bsid.APLZL,
                    XEGDR = bsid.XEGDR,
                    DMBE2 = bsid.DMBE2,
                    DMBE3 = bsid.DMBE3,
                    DMB21 = bsid.DMB21,
                    DMB22 = bsid.DMB22,
                    DMB23 = bsid.DMB23,
                    DMB31 = bsid.DMB31,
                    DMB32 = bsid.DMB32,
                    DMB33 = bsid.DMB33,
                    BDIF3 = bsid.BDIF3,
                    XRAGL = bsid.XRAGL,
                    UZAWE = bsid.UZAWE,
                    XSTOV = bsid.XSTOV,
                    MWST2 = bsid.MWST2,
                    MWST3 = bsid.MWST3,
                    SKNT2 = bsid.SKNT2,
                    SKNT3 = bsid.SKNT3,
                    XREF1 = bsid.XREF1,
                    XREF2 = bsid.XREF2,
                    XARCH = bsid.XARCH,
                    PSWSL = bsid.PSWSL,
                    PSWBT = bsid.PSWBT,
                    LZBKZ = bsid.LZBKZ,
                    LANDL = bsid.LANDL,
                    IMKEY = bsid.IMKEY,
                    VBEL2 = bsid.VBEL2,
                    VPOS2 = bsid.VPOS2,
                    POSN2 = bsid.POSN2,
                    ETEN2 = bsid.ETEN2,
                    FISTL = bsid.FISTL,
                    GEBER = bsid.GEBER,
                    DABRZ = bsid.DABRZ,
                    XNEGP = bsid.XNEGP,
                    KOSTL = bsid.KOSTL,
                    RFZEI = bsid.RFZEI,
                    KKBER = bsid.KKBER,
                    EMPFB = bsid.EMPFB,
                    PRCTR = bsid.PRCTR,
                    XREF3 = bsid.XREF3,
                    QSSKZ = bsid.QSSKZ,
                    ZINKZ = bsid.ZINKZ,
                    DTWS1 = bsid.DTWS1,
                    DTWS2 = bsid.DTWS2,
                    DTWS3 = bsid.DTWS3,
                    DTWS4 = bsid.DTWS4,
                    XPYPR = bsid.XPYPR,
                    KIDNO = bsid.KIDNO,
                    ABSBT = bsid.ABSBT,
                    CCBTC = bsid.CCBTC,
                    PYCUR = bsid.PYCUR,
                    PYAMT = bsid.PYAMT,
                    BUPLA = bsid.BUPLA,
                    SECCO = bsid.SECCO,
                    CESSION_KZ = bsid.CESSION_KZ,
                    PPDIFF = bsid.PPDIFF,
                    PPDIF2 = bsid.PPDIF2,
                    PPDIF3 = bsid.PPDIF3,
                    KBLNR = bsid.KBLNR,
                    KBLPOS = bsid.KBLPOS,
                    GRANT_NBR = bsid.GRANT_NBR,
                    GMVKZ = bsid.GMVKZ,
                    SRTYPE = bsid.SRTYPE,
                    LOTKZ = bsid.LOTKZ,
                    FKBER = bsid.FKBER,
                    INTRENO = bsid.INTRENO,
                    PPRCT = bsid.PPRCT,
                    BUZID = bsid.BUZID,
                    AUGGJ = bsid.AUGGJ,
                    HKTID = bsid.HKTID,
                    BUDGET_PD = bsid.BUDGET_PD,
                    PAYS_PROV = bsid.PAYS_PROV,
                    PAYS_TRAN = bsid.PAYS_TRAN,
                    MNDID = bsid.MNDID,
                    KONTT = bsid.KONTT,
                    KONTL = bsid.KONTL,
                    UEBGDAT = bsid.UEBGDAT,
                    VNAME = bsid.VNAME,
                    EGRUP = bsid.EGRUP,
                    BTYPE = bsid.BTYPE,
                    OIEXGNUM = bsid.OIEXGNUM,
                    OINETCYC = bsid.OINETCYC,
                    OIEXGTYP = bsid.OIEXGTYP,
                    PROPMANO = bsid.PROPMANO
                };
                _context.Add(payment);
            }
            _context.SaveChanges();
        }

        private static void ClearVariables()
        {
            SD.WhTotalAmount = 0;
            SD.UpAmount = 0;
            SD.PAmount = 0;
            SD.UpTotalAmount = 0;
            SD.TotalAmountDue = 0;
            SD.DebitTotalAmount = 0;
            SD.CreditTotalAmount = 0;
            SD.DebitCreditTotalAmount = 0;
            SD.PaidPayments = 0;
            SD.UnpaidPayments = 0;
            SD.TotalPayments = 0;
            SD.CurrentBillsTotal = 0;
            SD.PreviousBillsTotal = 0;
            SD.DollarTotal = 0;
            SD.WhPaidAmount = 0;
            SD.WhUnpaidAmount = 0;
        }

        #region Final Functions

        private void GetCompanyDetails(int companyCode)
        {
            //Get Address Code
            var t001 = _context.T001s.Where(i => i.BURKS == Convert.ToString(companyCode));

            var addressCode = "";
            foreach (var comp in t001)
            {
                addressCode = comp.ADRNR;
                SoaVM.Company.TinNo = comp.STCEG;
            }

            //Company Details
            var adrc = _context.ADRCs.Where(i => i.ADDRNUMBER == addressCode);
            foreach (var compa in adrc)
            {
                SoaVM.Company.Name = compa.NAME1;
                SoaVM.Company.TelNo = compa.TEL_NUMBER;
                SoaVM.Company.StreetAddress = compa.STREET + " ";
                SoaVM.Company.CityAddress = compa.CITY1;
            }
        }

        private void GetCustomerDetails(string customerNumber)
        {
            //get Customer details
            var kna1 = _context.KNA1s.Where(i => i.KUNNR == customerNumber);

            foreach (var cust in kna1)
            {
                SoaVM.Customer.Code = cust.KUNNR;
                SoaVM.Customer.Name = cust.NAME1;
                SoaVM.Customer.StreetAddress = cust.STRAS + " ";
                SoaVM.Customer.CityAddress = cust.ORT01;
            }

            //Get Customer Number and Attention To
            var knvk = _context.KNVKs.Where(i => i.KUNNR == customerNumber);

            foreach (var custo in knvk)
            {
                SoaVM.Customer.AttentionTo = custo.NAME1;
            }
        }

        #endregion Final Functions
    }
}