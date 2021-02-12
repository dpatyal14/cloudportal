using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudPortalServices.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
      
        public IActionResult DocumentList(int page = 1)
        {
            string compid = HttpContext.Session.GetString(Convert.ToString(SessionVals.CompanyID));
            var docs = _documentService.GetDocumentList(compid, page);

            return View(docs);
        }
    }
}
