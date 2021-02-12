using CloudPortal.Models;
using CloudPortalCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPortalRepository.Repository
{
    public interface IDocumentListRepository
    {
        List<DocumentModel> GetDocumentList(string txtdate, string ddDateRange, string ddlistSearch1, string txtship,string compid, int pagenumber);

    }
}
