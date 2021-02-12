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

        public List<DocumentModel> GetDocumentList(string compid, int pagenumber)
        {
            return _iDocumentRepository.GetDocumentList(compid,  pagenumber);
        }
      
    }
}
