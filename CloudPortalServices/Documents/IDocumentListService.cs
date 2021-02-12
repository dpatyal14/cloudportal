using CloudPortal.Models;
using CloudPortalCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPortalServices.Services
{
    public interface IDocumentListService
    {
        List<DocumentModel> GetDocumentList(string compid, int pagenumber);

    }
}
