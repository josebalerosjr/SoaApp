﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoaApp.Models
{
    public class ADRC
    {
        [Key]
        public int Id { get; set; }
        public string ADDRNUMBER { get; set; }
        public string DATE_FROM { get; set; }
        public string NATION { get; set; }
        public string DATE_TO { get; set; }
        public string TITLE { get; set; }
        public string NAME1 { get; set; }
        public string NAME2 { get; set; }
        public string NAME3 { get; set; }
        public string NAME4 { get; set; }
        public string NAME_TEXT { get; set; }
        public string NAME_CO { get; set; }
        public string CITY1 { get; set; }
        public string CITY2 { get; set; }
        public string CITY_CODE { get; set; }
        public string CITYP_CODE { get; set; }
        public string HOME_CITY { get; set; }
        public string CITYH_CODE { get; set; }
        public string CHCKSTATUS { get; set; }
        public string REGIOGROUP { get; set; }
        public string POST_CODE1 { get; set; }
        public string POST_CODE2 { get; set; }
        public string POST_CODE3 { get; set; }
        public string PCODE1_EXT { get; set; }
        public string PCODE2_EXT { get; set; }
        public string PCODE3_EXT { get; set; }
        public string PO_BOX { get; set; }
        public string DONT_USE_P { get; set; }
        public string PO_BOX_NUM { get; set; }
        public string PO_BOX_LOC { get; set; }
        public string CITY_CODE2 { get; set; }
        public string PO_BOX_REG { get; set; }
        public string PO_BOX_CTY { get; set; }
        public string POSTALAREA { get; set; }
        public string TRANSPZONE { get; set; }
        public string STREET { get; set; }
        public string DONT_USE_S { get; set; }
        public string STREETCODE { get; set; }
        public string STREETABBR { get; set; }
        public string HOUSE_NUM1 { get; set; }
        public string HOUSE_NUM2 { get; set; }
        public string HOUSE_NUM3 { get; set; }
        public string STR_SUPPL1 { get; set; }
        public string STR_SUPPL2 { get; set; }
        public string STR_SUPPL3 { get; set; }
        public string LOCATION { get; set; }
        public string BUILDING { get; set; }
        public string FLOOR { get; set; }
        public string ROOMNUMBER { get; set; }
        public string COUNTRY { get; set; }
        public string LANGU { get; set; }
        public string REGION { get; set; }
        public string ADDR_GROUP { get; set; }
        public string FLAGGROUPS { get; set; }
        public string PERS_ADDR { get; set; }
        public string SORT1 { get; set; }
        public string SORT2 { get; set; }
        public string SORT_PHN { get; set; }
        public string DEFLT_COMM { get; set; }
        public string TEL_NUMBER { get; set; }
        public string TEL_EXTENS { get; set; }
        public string FAX_NUMBER { get; set; }
        public string FAX_EXTENS { get; set; }
        public string FLAGCOMM2 { get; set; }
        public string FLAGCOMM3 { get; set; }
        public string FLAGCOMM4 { get; set; }
        public string FLAGCOMM5 { get; set; }
        public string FLAGCOMM6 { get; set; }
        public string FLAGCOMM7 { get; set; }
        public string FLAGCOMM8 { get; set; }
        public string FLAGCOMM9 { get; set; }
        public string FLAGCOMM10 { get; set; }
        public string FLAGCOMM11 { get; set; }
        public string FLAGCOMM12 { get; set; }
        public string FLAGCOMM13 { get; set; }
        public string ADDRORIGIN { get; set; }
        public string MC_NAME1 { get; set; }
        public string MC_CITY1 { get; set; }
        public string MC_STREET { get; set; }
        public string EXTENSION1 { get; set; }
        public string EXTENSION2 { get; set; }
        public string TIME_ZONE { get; set; }
        public string TAXJURCODE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string LANGU_CREA { get; set; }
        public string ADRC_UUID { get; set; }
        public string UUID_BELATED { get; set; }
        public string ID_CATEGORY { get; set; }
        public string ADRC_ERR_STATUS { get; set; }
        public string PO_BOX_LOBBY { get; set; }
        public string DELI_SERV_TYPE { get; set; }
        public string DELI_SERV_NUMBER { get; set; }
        public string COUNTY_CODE { get; set; }
        public string COUNTY { get; set; }
        public string TOWNSHIP_CODE { get; set; }
        public string TOWNSHIP { get; set; }
        public string MC_COUNTY { get; set; }
        public string MC_TOWNSHIP { get; set; }
        public string XPCPT { get; set; }
        public string Address_notes {get;set;}

    }
}