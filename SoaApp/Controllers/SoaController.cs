using Intranet.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using SoaApp.Models;
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

        public IActionResult GenerateSoa(string customer, int company, DateTime asof)
        {
            ClearVariables();

            //SD.DollarTotal = 1;
            
            SD.GuiCompany = company.ToString();
            SD.GuiCustomerNum = customer;
            SD.GuiAsOf = asof;
            
            GetCompanyDetails();
            
            GetCustomerDetails();
            
            ClearDbTables();
            
            GetPaymentsData();

            GetCurrentPreviousBill(asof);

            GetUnpaidDetails();

            GetCreditDebitDetails();

            GetCwtDetails();

            GetTotalAmountDue();
            
            return View();
        }

        private void GetTotalAmountDue()
        {
            SD.TotalAmountDue = ((SD.UpTotalAmount + SD.WhTotalAmount) + SD.DebitCreditTotalAmount) - SD.TotalPayments;
        }

        private void GetCurrentPreviousBill(DateTime AsOfDate)
        {
            var currentPrevBill = _context.BSIDs
                .Where(i => (i.UMSKZ == "" || i.UMSKZ != "C") &&
                            (i.KUNNR == SD.CustomerNum)
                );

            // Current Bills
            foreach (var bill in currentPrevBill)
            {
                SD.DateBudat = Convert.ToDateTime(bill.BUDAT);
                if (SD.DateBudat >= DateMangement.GetFirstDayOfCurrentMonth(AsOfDate) && SD.DateBudat <= SD.GuiAsOf)
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
                SD.DateBudat = Convert.ToDateTime(bill.BUDAT);
                if (SD.DateBudat <= Convert.ToDateTime(DateMangement.GetPreviousMonthLastDay(AsOfDate)))
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
            
            SD.PreviousBillsTotal += SD.WhTotalAmount;
        }

        private void GetCreditDebitDetails()
        {
            // get all credit and debit from table
            var bsidCmdm = _context.BSIDs.Where(i => (i.UMSKZ == "" || i.UMSKZ != "C") &&
                                                        (i.KUNNR == SD.CustomerNum) &&
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
                                    (i.KUNNR == SD.CustomerNum) &&
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
                                    (i.KUNNR == SD.CustomerNum) &&
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
                                                        (i.KUNNR == SD.CustomerNum) &&
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

        private void GetCompanyDetails()
        {
            //Get Address Code
            var t001 = _context.T001s.Where(i => i.BURKS == SD.GuiCompany);
            foreach (var comp in t001)
            {
                SD.AddressCode = comp.ADRNR;
                SD.CompanyTinNum = comp.STCEG;
            }
            
            //Company Details
            var adrc = _context.ADRCs.Where(i => i.ADDRNUMBER == SD.AddressCode);
            foreach (var compa in adrc)
            {
                SD.CompanyName = compa.NAME1;
                SD.CompanyTellNum = compa.TEL_NUMBER;
                SD.CompanyCity = compa.CITY1;
                SD.CompanyStreet = compa.STREET + " ";
            }
        }

        private void GetCustomerDetails()
        {
            //get Customer details
            var kna1 = _context.KNA1s.Where(i => i.KUNNR == SD.GuiCustomerNum);
            foreach (var cust in kna1)
            {
                SD.CustomerNum = cust.KUNNR;
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
        }

        private void GetCwtDetails()
        {
            // get all cwt from database
            var bsidCwt = _context.BSIDs.Where(i => (i.UMSKZ == "C") && (i.KUNNR == SD.CustomerNum));

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
                        (i.KUNNR == SD.CustomerNum) &&
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
        
        private void GetPaymentsData()
        {
            AddBsadToPaymentTable();
            AddBsidToPaymentTable();
            PaymentComputation();
            
            var bsidBsad = _context.Payments
                .Where(i =>
                        (i.UMSKZ == "" || i.UMSKZ != "C") &&
                        (i.KUNNR == SD.CustomerNum) &&
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
            var bsidBsadDel = _context.Payments
                .Where(i =>
                    (i.UMSKZ == "" || i.UMSKZ != "C") &&
                    (i.KUNNR == SD.CustomerNum) &&
                    (
                        i.BLART == "DX" ||
                        i.BLART == "DY" ||
                        i.BLART == "DZ"
                    ));

            foreach (var item in bsidBsadDel)
            {
                _context.Payments.Remove(item);
            }
            _context.SaveChanges();

            ViewBag.DxDyDz = null;
        }
        
        private void AddBsadToPaymentTable()
        {
            var bsadDxdydz = _context.BSADs
                .Where(i =>
                        (i.UMSKZ == "" || i.UMSKZ != "C") &&
                        (i.GJAHR == Convert.ToString(SD.GuiAsOf.Year)) &&
                        (i.KUNNR == SD.CustomerNum) &&
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
        
        private void AddBsidToPaymentTable()
        {
            var bsidDxdydz = _context.BSIDs
               .Where(i =>
                       (i.UMSKZ == "" || i.UMSKZ != "C") &&
                       (i.GJAHR == Convert.ToString(SD.GuiAsOf.Year)) &&
                       (i.KUNNR == SD.CustomerNum) &&
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
            SD.GuiCompany = "";
            SD.GuiCustomerNum = "";
            SD.AddressCode = "";
            SD.CompanyTinNum = "";
            SD.CompanyName = "";
            SD.CompanyTellNum = "";
            SD.CompanyCity = "";
            SD.CompanyStreet = "";
            SD.CustomerName = "";
            SD.CustomerCity = "";
            SD.CustomerStreet = "";
            SD.CustomerNum = "";
            SD.AttentionTo = "";
            SD.GuiCustomerNumSelected = "";
            //SD.PreviousMonthLastDay = 0;
            SD.WhPaidAmount = 0;
            SD.WhUnpaidAmount = 0;
        }
    }
}