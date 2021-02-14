using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudPortalServices.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static CloudPortal.Models.SessionExtensions;

namespace CloudPortal.Controllers
{
    public class DocumentController : BaseController
    {
        private IDocumentListService _documentService;

        public DocumentController(IDocumentListService documentService)
        {
            
            _documentService = documentService;
        }
      
        public IActionResult DocumentList(string txtdate, string ddDateRange, string ddlistSearch1, string txtship,string chkArchive, string ddlLoc, string chkPO, string chkriv, int page = 0)
        {
            string compid = HttpContext.Session.GetString(Convert.ToString(SessionVals.CompanyID));
          //  var docs = _documentService.GetDocumentList(txtdate, ddDateRange, ddlistSearch1, txtship, chkArchive, compid,  ddlLoc,  chkPO,  chkriv, page);
            IEnumerable<SelectListItem> locations = new List<SelectListItem>();

            locations = _documentService.PopulateLocations(compid).AsEnumerable().Select(m => new SelectListItem() { Text = m.srclocation, Value = m.srclocation });

            ViewBag.Locations = locations;// new SelectList(locations, "srclocation", "srclocation");
            return View();
        }
        public IActionResult PartialDocumentList(string txtdate, string ddDateRange, string ddlistSearch1, string txtship, string chkArchive, string ddlLoc, string chkPO, string chkriv, int page = 0)
        {
            string compid = HttpContext.Session.GetString(Convert.ToString(SessionVals.CompanyID));
            var docs = _documentService.GetDocumentList(txtdate, ddDateRange, ddlistSearch1, txtship, chkArchive, compid, ddlLoc, chkPO, chkriv, page);
           // IEnumerable<SelectListItem> locations = new List<SelectListItem>();

            //locations = _documentService.PopulateLocations(compid).AsEnumerable().Select(m => new SelectListItem() { Text = m.srclocation, Value = m.srclocation });

            //  ViewBag.Locations = locations;// new SelectList(locations, "srclocation", "srclocation");
            return PartialView("_PartialDocument",docs);
        }
    }
}
