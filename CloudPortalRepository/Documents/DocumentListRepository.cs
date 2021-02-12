using CloudPortal.Models;
using CloudPortalCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace CloudPortalRepository.Repository
{
    public class DocumentListRepository: IDocumentListRepository
    {
        public const int PASSWDLEN = 8;

        private const int LOGINATTEMPTS = 5;
        public const int SIDLEN = 30;

      //  public string ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

        private readonly IConfiguration configuration;

        public DocumentListRepository(IConfiguration config)
        {
            this.configuration = config;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(this.configuration.GetConnectionString("Default"));
        }
        public List<DocumentModel> GetDocumentList(string compid,int pagenumber)
        {
            int pagecount = 0;

            List<DocumentModel> objModel = new List<DocumentModel>();
            using (MySqlConnector.MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd1 = new MySqlCommand("select count(*) as cnt from imgrecs where companyid=" + compid, conn);

                using (var reader1 = cmd1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        pagecount = Convert.ToInt32(reader1["cnt"]);
                    }
                }
                        MySqlCommand cmd = new MySqlCommand("select createdate,apptmnt,shipnum,bolnumber,trailer,doctype,filetype,receiver,carriername,comment,active,imgrecid from imgrecs where companyid="+ compid+" limit "+pagenumber+",10", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DocumentModel model = new DocumentModel();
                        model.pagecount = pagecount;
                        model.pagenum = pagenumber;
                        model.bolnumber = Convert.ToString(reader["bolnumber"]);
                        model.shipnum = Convert.ToString(reader["shipnum"]);
                        model.filetype = Convert.ToString(reader["filetype"]);
                        model.doctype = Convert.ToString(reader["doctype"]);
                        model.receiver = Convert.ToString(reader["receiver"]);
                        model.trailer = Convert.ToString(reader["trailer"]);
                        model.active = Convert.ToString(reader["active"]);
                        model.carriername = Convert.ToString(reader["carriername"]);
                        model.createdate = Convert.ToDateTime(reader["createdate"]);
                        objModel.Add(model);
                    }
                }
            }
            return objModel;
        }

      
    }
}
