using CloudPortal.Models;
using CloudPortalCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPortalRepository.Repository
{
    public interface IDocumentListRepository
    {
        List<DocumentModel> GetDocumentList(string txtdate, string ddDateRange, string ddlistSearch1, string txtship, string chkArchive, string compid, string ddlLoc, string chkPO, string chkriv, int pagenumber);
        List<DocumentLocations> PopulateLocations(string compid);
    }
}
