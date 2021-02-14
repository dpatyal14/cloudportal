using CloudPortal.Models;
using CloudPortalCore;
using CloudPortalRepository.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPortalServices.Services
{
    public class DocumentListService: IDocumentListService
    {
        private readonly IDocumentListRepository _iDocumentRepository;
        public DocumentListService(IDocumentListRepository iDocumentRepository)
        {
            _iDocumentRepository = iDocumentRepository;
        }

        public List<DocumentModel> GetDocumentList(string txtdate, string ddDateRange, string ddlistSearch1, string txtship, string chkArchive,string compid, string ddlLoc, string chkPO, string chkriv, int pagenumber)
        {
            return _iDocumentRepository.GetDocumentList(txtdate,ddDateRange,ddlistSearch1,txtship, chkArchive,compid,  ddlLoc,  chkPO,  chkriv, pagenumber);
        }

        public List<DocumentLocations> PopulateLocations(string compid)
        {
            return _iDocumentRepository.PopulateLocations(compid);
        }


    }
}
