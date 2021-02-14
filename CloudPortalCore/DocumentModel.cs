using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPortalCore
{
    public class DocumentModel
    {
        //public DocumentModel()
        //{
        //    DocumentLocationList = new List<SelectListItem>();
        //}
        public int pagecount;
        public int pagenum;

        //public List<SelectListItem> DocumentLocationList
        //{
        //    get;
        //    set;
        //}
        public DateTime createdate { get; set; }
       public string apptmnt { get; set; }
       public string shipnum { get; set; }
        public string bolnumber { get; set; }
        public string trailer { get; set; }
        public string doctype { get; set; }
        public string filetype { get; set; }
        public string receiver { get; set; }
        public string carriername { get; set; }
        public string comment { get; set; }
        public string active { get; set; }
        public string imgrecid { get; set; }
    }
}
