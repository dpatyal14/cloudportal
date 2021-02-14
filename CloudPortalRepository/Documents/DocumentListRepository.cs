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
        public List<DocumentModel> GetDocumentList(string txtdate, string ddDateRange, string ddlistSearch1, string txtship,string chkArchive, string compid, string ddlLoc, string chkPO, string chkriv, int pagenumber)
        {
            int pagecount = 0;

            List<DocumentModel> objModel = new List<DocumentModel>();
            using (MySqlConnector.MySqlConnection conn = GetConnection())
            {
                conn.Open();
              

                MySqlCommand cmd = new MySqlCommand("", conn);
                StringBuilder tempGetSQL = new StringBuilder();
                //  MySqlCommand cmd = new MySqlCommand("select createdate,apptmnt,shipnum,bolnumber,trailer,doctype,filetype,receiver,carriername,comment,active,imgrecid from imgrecs where companyid=" + compid + tempGetSQL + " limit " + pagenumber + ",10", conn);
               
                if (txtdate != null)
                {
                    DateTime dt = Convert.ToDateTime(txtdate).Date;
                    tempGetSQL.Append(" and date(createdate) = '"+ dt.ToString("yyyy-MM-dd")+"'");
                }
                if (!string.IsNullOrEmpty(ddlLoc))
                {
                    tempGetSQL.Append(" and srclocation ="+ ddlLoc);
                }
               
                if (chkriv != null && chkriv != "true") {
                    tempGetSQL.Append(" and active='Y' ");
                }
                if (chkArchive != null && chkArchive != "true")
                {
                    tempGetSQL.Append(" and active='Y' ");
                }
                string fldname = "";
                bool likeFlag = true;
                switch (ddlistSearch1)
                {
                    case "shp":
                        fldname = "shipnum";
                        break;
                    case "blnm":
                        fldname = "bolnumber";
                        break;
                    case "pro":
                        fldname = "pronum";
                        break;
                    case "dlv":
                        fldname = "delvnum";
                        break;
                    case "rcvr":
                        likeFlag = true;
                        fldname = "receiver";
                        break;
                    case "po":
                        fldname = "ponum";
                        break;
                    case "app":
                        fldname = "apptmnt";
                        break;
                    case "trl":
                        fldname = "trailer";
                        break;
                    case "ldn":
                        fldname = "loadnum";
                        break;
                    case "cac":
                        fldname = "consigneeacc";
                        break;
                    case "sb":
                        likeFlag = true;
                        fldname = "signedby";
                        break;
                    case "sd":
                        fldname = "sbolnum";
                        break;
                    case "so":
                        fldname = "salesorder";
                        break;
                    case "inv":
                        fldname = "invoice";
                        break;
                    case "txt":

                        break;
                    default:
                        txtship = "";
                        break;
                }

                if (!string.IsNullOrEmpty(txtship))
                {
                    if (ddlistSearch1 == "txt")
                    {
                        likeFlag = true;

                        tempGetSQL.Append(" and imgrecid in (select imgrecid from imgtxt where txtfld like @1)");
                        if (likeFlag)
                            cmd.Parameters.AddWithValue("@1", "%" + txtship + "%");
                        else
                            cmd.Parameters.AddWithValue("@1", txtship);
                    }
                    else if (fldname.Length != 0)
                    {
                        tempGetSQL.Append(" and ");
                        tempGetSQL.Append(fldname);
                        tempGetSQL.Append((likeFlag ? " like " : " = "));
                        tempGetSQL.Append(" @1");

                        if (likeFlag)
                            cmd.Parameters.AddWithValue("@1", "%" + txtship + "%");
                        else
                            cmd.Parameters.AddWithValue("@1", txtship);
                    }
                }

                if (!string.IsNullOrEmpty(txtdate))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@dt", Convert.ToDateTime(txtdate).ToString("yyyy-MM-dd"));
                    }
                    catch (Exception)
                    {
                        cmd.Parameters.AddWithValue("@dt", DateTime.Now.ToString("yyyy-MM-dd"));
                    
                    }
                    tempGetSQL.Append(" and date(createdate) = @dt");
                }
                else
                {
                    bool flag = true;
                    DateTime dtPVal = DateTime.Now.Date;
                    DateTime dt = DateTime.Now;

                    switch (ddDateRange)
                    {
                        case "7d":
                            //GetSQL = GetSQL & " and createdate > @dt"
                            dtPVal = dt.AddDays(-7);

                            break;
                        case "30d":
                            //GetSQL = GetSQL & " and createdate > @dt"
                            dtPVal = dt.AddDays(-30);

                            break;
                        case "3m":
                            //GetSQL = GetSQL & " and createdate > @dt"
                            dtPVal = dt.AddMonths(-3);

                            break;
                        case "1y":
                            //GetSQL = GetSQL & " and createdate > @dt"
                            dtPVal = dt.AddYears(-1);

                            break;
                        case "1d":
                            //GetSQL = GetSQL & " and createdate > @dt"
                            dtPVal = dt.AddDays(-1);

                            break;
                        default:
                            flag = false;
                            break;
                    }

                    if (flag)
                    {
                        tempGetSQL.Append(" and createdate > @dt");
                        cmd.Parameters.AddWithValue("@dt", dtPVal.ToString("yyyy-MM-dd"));
                    }
                }
               
                // MySqlCommand cmd1 = new MySqlCommand("select count(*) as cnt from imgrecs where companyid=" + compid, conn);
                cmd.CommandText="select count(*) as cnt from imgrecs where companyid=" + compid + tempGetSQL;
                using (var reader1 = cmd.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        pagecount = Convert.ToInt32(reader1["cnt"]);
                    }
                }
                cmd.CommandText = "select createdate,apptmnt,shipnum,bolnumber,trailer,doctype,filetype,receiver,carriername,comment,active,imgrecid from imgrecs where companyid=" + compid + tempGetSQL + " limit " + pagenumber + ",10";

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


        public List<DocumentLocations> PopulateLocations(string compid)
        {
           
            List<DocumentLocations> objModel = new List<DocumentLocations>();
            using (MySqlConnector.MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd1 = new MySqlCommand("select srclocation from locationprofile where companyid=@cid", conn);
                cmd1.Parameters.AddWithValue("@cid", compid);
                using (var reader = cmd1.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        DocumentLocations model = new DocumentLocations();
                       
                        model.srclocation = Convert.ToString(reader["srclocation"]);
                        
                        objModel.Add(model);
                    }
                }
            }
            return objModel;
        }


    }
}
