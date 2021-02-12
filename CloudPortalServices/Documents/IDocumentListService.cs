using CloudPortal.Models;
using CloudPortalCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPortalServices.Services
{
    public interface IDocumentListService
    {
        List<DocumentModel> GetDocumentList(string txtdate, string ddDateRange, string ddlistSearch1, string txtship,string compid, int pagenumber);

    }
}
