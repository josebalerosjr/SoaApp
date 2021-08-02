﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SoaApp.Models
{
    public class BSIDNew
    {
        [Key]
        public int Id { get; set; }

        public int BUKRS { get; set; }
        public string KUNNR { get; set; }
        public string UMSKS { get; set; }
        public string UMSKZ { get; set; }
        public string AUGDT { get; set; }
        public string AUGBL { get; set; }
        public string ZUONR { get; set; }
        public int GJAHR { get; set; }
        public string BELNR { get; set; }
        public string BUZEI { get; set; }
        public DateTime BUDAT { get; set; }
        public DateTime BLDAT { get; set; }
        public string CPUDT { get; set; }
        public string WAERS { get; set; }
        public string XBLNR { get; set; }
        public string BLART { get; set; }
        public int MONAT { get; set; }
        public int BSCHL { get; set; }
        public string ZUMSK { get; set; }
        public string SHKZG { get; set; }
        public string GSBER { get; set; }
        public string MWSKZ { get; set; }
        public double DMBTR { get; set; }
        public double WRBTR { get; set; }
        public string MWSTS { get; set; }
        public string WMWST { get; set; }
        public string BDIFF { get; set; }
        public string BDIF2 { get; set; }
        public string SGTXT { get; set; }
        public string PROJN { get; set; }
        public string AUFNR { get; set; }
        public string ANLN1 { get; set; }
        public string ANLN2 { get; set; }
        public int SAKNR { get; set; }
        public int HKONT { get; set; }
        public string FKONT { get; set; }
        public string FILKD { get; set; }
        public DateTime ZFBDT { get; set; }
        public string ZTERM { get; set; }
        public string ZBD1T { get; set; }
        public string ZBD2T { get; set; }
        public string ZBD3T { get; set; }
        public string ZBD1P { get; set; }
        public string ZBD2P { get; set; }
        public string SKFBT { get; set; }
        public string SKNTO { get; set; }
        public string WSKTO { get; set; }
        public string ZLSCH { get; set; }
        public string ZLSPR { get; set; }
        public string ZBFIX { get; set; }
        public string HBKID { get; set; }
        public string BVTYP { get; set; }
        public string REBZG { get; set; }
        public string REBZJ { get; set; }
        public string REBZZ { get; set; }
        public string SAMNR { get; set; }
        public string ANFBN { get; set; }
        public string ANFBJ { get; set; }
        public string ANFBU { get; set; }
        public string ANFAE { get; set; }
        public string MANSP { get; set; }
        public string MSCHL { get; set; }
        public string MADAT { get; set; }
        public string MANST { get; set; }
        public string MABER { get; set; }
        public string XNETB { get; set; }
        public string XANET { get; set; }
        public string XCPDD { get; set; }
        public string XINVE { get; set; }
        public string XZAHL { get; set; }
        public string MWSK1 { get; set; }
        public string DMBT1 { get; set; }
        public string WRBT1 { get; set; }
        public string MWSK2 { get; set; }
        public string DMBT2 { get; set; }
        public string WRBT2 { get; set; }
        public string MWSK3 { get; set; }
        public string DMBT3 { get; set; }
        public string WRBT3 { get; set; }
        public string BSTAT { get; set; }
        public string VBUND { get; set; }
        public string VBELN { get; set; }
        public string REBZT { get; set; }
        public string INFAE { get; set; }
        public string STCEG { get; set; }
        public string EGBLD { get; set; }
        public string EGLLD { get; set; }
        public string RSTGR { get; set; }
        public string XNOZA { get; set; }
        public string VERTT { get; set; }
        public string VERTN { get; set; }
        public string VBEWA { get; set; }
        public string WVERW { get; set; }
        public string PROJK { get; set; }
        public string FIPOS { get; set; }
        public string NPLNR { get; set; }
        public string AUFPL { get; set; }
        public string APLZL { get; set; }
        public string XEGDR { get; set; }
        public string DMBE2 { get; set; }
        public string DMBE3 { get; set; }
        public string DMB21 { get; set; }
        public string DMB22 { get; set; }
        public string DMB23 { get; set; }
        public string DMB31 { get; set; }
        public string DMB32 { get; set; }
        public string DMB33 { get; set; }
        public string BDIF3 { get; set; }
        public string XRAGL { get; set; }
        public string UZAWE { get; set; }
        public string XSTOV { get; set; }
        public string MWST2 { get; set; }
        public string MWST3 { get; set; }
        public string SKNT2 { get; set; }
        public string SKNT3 { get; set; }
        public string XREF1 { get; set; }
        public string XREF2 { get; set; }
        public string XARCH { get; set; }
        public string PSWSL { get; set; }
        public string PSWBT { get; set; }
        public string LZBKZ { get; set; }
        public string LANDL { get; set; }
        public string IMKEY { get; set; }
        public string VBEL2 { get; set; }
        public string VPOS2 { get; set; }
        public string POSN2 { get; set; }
        public string ETEN2 { get; set; }
        public string FISTL { get; set; }
        public string GEBER { get; set; }
        public string DABRZ { get; set; }
        public string XNEGP { get; set; }
        public string KOSTL { get; set; }
        public string RFZEI { get; set; }
        public string KKBER { get; set; }
        public string EMPFB { get; set; }
        public string PRCTR { get; set; }
        public string XREF3 { get; set; }
        public string QSSKZ { get; set; }
        public string ZINKZ { get; set; }
        public string DTWS1 { get; set; }
        public string DTWS2 { get; set; }
        public string DTWS3 { get; set; }
        public string DTWS4 { get; set; }
        public string XPYPR { get; set; }
        public string KIDNO { get; set; }
        public string ABSBT { get; set; }
        public string CCBTC { get; set; }
        public string PYCUR { get; set; }
        public string PYAMT { get; set; }
        public string BUPLA { get; set; }
        public string SECCO { get; set; }
        public string CESSION_KZ { get; set; }
        public string PPDIFF { get; set; }
        public string PPDIF2 { get; set; }
        public string PPDIF3 { get; set; }
        public string KBLNR { get; set; }
        public string KBLPOS { get; set; }
        public string GRANT_NBR { get; set; }
        public string GMVKZ { get; set; }
        public string SRTYPE { get; set; }
        public string LOTKZ { get; set; }
        public string FKBER { get; set; }
        public string INTRENO { get; set; }
        public string PPRCT { get; set; }
        public string BUZID { get; set; }
        public string AUGGJ { get; set; }
        public string HKTID { get; set; }
        public string BUDGET_PD { get; set; }
        public string PAYS_PROV { get; set; }
        public string PAYS_TRAN { get; set; }
        public string MNDID { get; set; }
        public string KONTT { get; set; }
        public string KONTL { get; set; }
        public string UEBGDAT { get; set; }
        public string VNAME { get; set; }
        public string EGRUP { get; set; }
        public string BTYPE { get; set; }
        public string OIEXGNUM { get; set; }
        public string OINETCYC { get; set; }
        public string OIEXGTYP { get; set; }
        public string PROPMANO { get; set; }
    }
}