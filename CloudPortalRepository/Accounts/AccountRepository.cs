using CloudPortal.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace CloudPortalRepository.Repository
{
    public class AccountRepository : IAccountRepository
    {
        public const int PASSWDLEN = 8;

        private const int LOGINATTEMPTS = 5;
        public const int SIDLEN = 30;

      //  public string ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

        private readonly IConfiguration configuration;

        public AccountRepository(IConfiguration config)
        {
            this.configuration = config;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(this.configuration.GetConnectionString("Default"));
        }
        public int GetCompanyId(string userid)
        {

            int companyid = 0;
            using (MySqlConnector.MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select companyid from userprofile where userid='" + userid + "'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        companyid = Convert.ToInt32(reader["companyid"]);


                    }
                }
            }
            return companyid;
        }

        public void GetLicenseInfo(string compid, ref int usrCount, ref string usrType, ref int locCount)
        {


            try
            {

                using (MySqlConnector.MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("select usrcnt,usrtype,loccnt from companyprofile where companyid=@cid", conn))
                    {
                        cmd.Parameters.AddWithValue("@cid", compid);
                        using (MySqlDataReader dtr = cmd.ExecuteReader())
                        {
                            if (dtr.Read())
                            {
                                usrCount = Convert.ToInt32(dtr[0]);
                                usrType = Convert.ToString(dtr[1]);
                                locCount = Convert.ToInt32(dtr[2]);
                            }
                            else
                            {
                                usrType = ModConst.DEFUSRTYPE;
                                usrCount = Convert.ToInt32(ModConst.DEFUSRCOUNT);
                                locCount = Convert.ToInt32(ModConst.DEFLOCCOUNT);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }
        public int CheckConcurrentUsers(string compid, string userid)
        {
            int tempCheckCUsers = 1;
            int locCount = 0;
            int usrCount = 0;
            string usrType = "";

            //tempCheckCUsers = 1;
            GetLicenseInfo(compid, ref usrCount, ref usrType, ref locCount);

            if (usrType == "C") // concurrent users
            {
                try
                {
                    //cmd = new MySqlCommand("update userprofile set sessionid='' where lastaccess < dateadd(mi, -20, sysdate())", conn);
                    using (MySqlConnector.MySqlConnection conn = GetConnection())
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand("update userprofile set sessionid='' where lastaccess < (sysdate() + interval -20 minute)", conn))
                        {
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "select count(userid) as cnt from userprofile where companyid=@cid and userid <> @uid and lastaccess > (sysdate() + interval -20 minute) and length(sessionid) <> 0";
                            cmd.Parameters.AddWithValue("@cid", compid);
                            cmd.Parameters.AddWithValue("@uid", userid);
                            using (MySqlDataReader dtr = cmd.ExecuteReader())
                            {
                                if (dtr.Read())
                                {
                                    int loggedInUsers = Convert.ToInt32(dtr[0]);
                                    if (loggedInUsers < usrCount)
                                        tempCheckCUsers = 0;
                                    else tempCheckCUsers = 100 + loggedInUsers;
                                }
                                else tempCheckCUsers = 0;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    tempCheckCUsers = 99;
                }
            }
            else tempCheckCUsers = 0;

            return tempCheckCUsers;
        }

        public void UpdateLoginlog(string companyid, string userid, string ip, string machinename, string wserver, string loginst, int srcID, string sessionid = "")
        {


            try
            {
                string tblname = (srcID == 0 ? "loginlog" : "loginlogapi");

                using (MySqlConnector.MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.CommandText = "insert into " + tblname + "(companyid,userid,loginstatus,ip,machinename,wserver,srcid,sessionid) values (@cid,@uid,@loginstatus,@ip,@machinename,@wserver,@srcid,@sessionid)";
                        cmd.Connection = conn;
                        cmd.Parameters.AddWithValue("@cid", companyid);
                        cmd.Parameters.AddWithValue("@uid", userid);
                        cmd.Parameters.AddWithValue("@loginstatus", loginst);
                        cmd.Parameters.AddWithValue("@ip", ip);
                        cmd.Parameters.AddWithValue("@machinename", machinename);
                        cmd.Parameters.AddWithValue("@wserver", wserver);
                        cmd.Parameters.AddWithValue("@srcid", srcID);
                        cmd.Parameters.AddWithValue("@sessionid", sessionid);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }


        public int ValidateUserAuth(AuthenticationMode authMode, ref UserProfile usrProfile, string ip, bool signflag = false, bool adminflag = false)
        {
            int retval = 99998;
            //string whallowed = "";

            if ((retval = ValidateUser(authMode, ref usrProfile)) == 0)
            {
                if ((retval = ValidateCompany(ref usrProfile)) == 0)
                {
                    if (authMode == AuthenticationMode.Portal || authMode == AuthenticationMode.AppDriver || (retval = ValidateWH(usrProfile)) == 0)
                    {
                        GetExtendedInfo(ref usrProfile, authMode, signflag, adminflag);
                    }
                }
            }

            return retval;
        }
        private int ValidateWH(UserProfile usrProfile)
        {
            int retval = 99020;
            try
            {
                string[] wh = usrProfile.allowedwh.Split(new string[] { "," }, StringSplitOptions.None);
                usrProfile.defwh = wh[0];
                bool whfound = false;

                try
                {
                    foreach (string srcloc in wh)
                    {
                        if (srcloc == usrProfile.wh)
                        {
                            whfound = true;
                            retval = 0;
                            break;
                        }
                    }

                    if (!whfound)
                        retval = 99021;
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }

            return retval;
        }


        private int GetExtendedInfo(ref UserProfile usrProfile, AuthenticationMode authMode, bool signflag, bool adminflag)
        {
            int retval = 99997;

            try
            {
                using (MySqlConnector.MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("select * from userprofile where userid=@uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", usrProfile.userid);

                        using (MySqlDataReader dtr = cmd.ExecuteReader())
                        {
                            retval = 99010;

                            //LogMesg.SendLogMessage("Login:" + usrProfile.userid, "Login:" + usrProfile.userType + ":" + passMode);

                            if (dtr.Read())
                            {
                                //bool adminflag = SessionExtensions.Get<string>(SessionVals.UserType) == ADMINUSER;
                                // HttpContext.Session.Get<string>(SessionVals.UserType) 

                                usrProfile.allowedwh = Convert.ToString(dtr["whallow"]);

                                try
                                {
                                    string[] wh = usrProfile.allowedwh.Split(new string[] { "," }, StringSplitOptions.None);
                                    usrProfile.defwh = wh[0];


                                }
                                catch (Exception ex)
                                {

                                }

                                usrProfile.rights = Convert.ToString(dtr["usrpriv"]);

                                if (usrProfile.rights.Length < 5)
                                {
                                    if (usrProfile.rights.Length == 1)
                                        usrProfile.rights = usrProfile.rights + "10000";
                                    else
                                        usrProfile.rights = "010000";
                                }

                                usrProfile.allowDelete = adminflag ? "1" : (usrProfile.rights.Length > 0 ? usrProfile.rights.Substring(0, 1) : "0");
                                usrProfile.allowDeliveries = adminflag ? "1" : (usrProfile.rights.Length > 1 ? (usrProfile.rights.Substring(1, 1)) : "0");

                                usrProfile.addUsers = adminflag ? "1" : (usrProfile.rights.Length > 4 ? usrProfile.rights.Substring(3, 1) : "0");
                                usrProfile.allowUnlock = adminflag ? "1" : (usrProfile.rights.Length > 5 ? usrProfile.rights.Substring(4, 1) : "0");

                                usrProfile.username = Convert.ToString(dtr["fullname"]);
                                usrProfile.email = Convert.ToString(dtr["email"]);
                                usrProfile.ccemail = Convert.ToString(dtr["ccemail"]);

                                //usrProfile.edbol = Convert.ToString(dtr["disableedit"]);
                                usrProfile.shipsigntype = Convert.ToString(dtr["usersigntype"]);


                                retval = 0;
                            }
                            else retval = 99011; // userid not found
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retval = 99012;

            }

            return retval;
        }

        public void UpdateLastLogin(string userid, int compid, string sessionid)
        {
            try
            {
                string strUpdate = "delete from tmpfiles where createdate < (sysdate() + interval -1 day);update userprofile set lastlogin=sysdate(),lastaccess=sysdate(),sessionid=@sessionid where userid=@uid and companyid=@cid";
                using (MySqlConnector.MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(strUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@sessionid", sessionid);
                        cmd.Parameters.AddWithValue("@uid", userid);
                        cmd.Parameters.AddWithValue("@cid", compid);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public int ValidateCompany(ref UserProfile usrProfile)
        {
            int retval = 99040;

            try
            {
                using (MySqlConnector.MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand("select accountcode,updboldt,signdtformat,prefs,mpod,sign,expirydate,comptype,compstatus,shipnumcaption,picknumcaption from companyprofile where companyid=@cid", conn))
                    {
                        cmd.Parameters.AddWithValue("@cid", usrProfile.companyid);

                        using (MySqlDataReader dtr = cmd.ExecuteReader())
                        {
                            retval = 99041;

                            //LogMesg.SendLogMessage("Login:" + usrProfile.userid, "Login:" + usrProfile.userType + ":" + passMode);

                            if (dtr.Read())
                            {
                                usrProfile.companyStatus = Convert.ToString(dtr["compstatus"]);
                                usrProfile.companyType = Convert.ToString(dtr["comptype"]);

                                if (usrProfile.companyStatus == "E") // evaluation
                                {
                                    DateTime dt;

                                    try
                                    {
                                        dt = Convert.ToDateTime(dtr["expirydate"]);
                                    }
                                    catch (Exception)
                                    {
                                        dt = DateTime.Now.Date;
                                    }

                                    if (dt < DateTime.Now.Date)
                                        usrProfile.companyStatus = "X";
                                }

                                if (usrProfile.companyStatus == "X")
                                {
                                    retval = 99042;
                                }
                                else
                                {
                                    usrProfile.account = Convert.ToString(dtr["accountcode"]);
                                    usrProfile.mpod = Convert.ToString(dtr["mpod"]);
                                    usrProfile.sign = Convert.ToString(dtr["sign"]);
                                    usrProfile.shipnumcaption = Convert.ToString(dtr["shipnumcaption"]);
                                    usrProfile.picknumcaption = Convert.ToString(dtr["picknumcaption"]);


                                    usrProfile.signdtformat = Convert.ToString(dtr["signdtformat"]);
                                    if (usrProfile.signdtformat.Trim().Length == 0)
                                        usrProfile.signdtformat = ModConst.STDDATETIMEFORMAT;

                                    string tstr = Convert.ToString(dtr["prefs"]);
                                    try
                                    {
                                        string[] strs = tstr.Split(new string[] { "::" }, StringSplitOptions.None);
                                        //usrProfile.sealAtBOL = ModUtils.GetArrayVal(strs, 0);
                                        //usrProfile.driverNameMandatory = ModUtils.GetArrayVal(strs, 1);
                                        //usrProfile.LockRecordAfterSigning = ModUtils.GetArrayVal(strs, 2);
                                        //usrProfile.appendloc = ModUtils.GetArrayVal(strs, 3);
                                        //usrProfile.autocheckout = ModUtils.GetArrayVal(strs, 4);
                                        usrProfile.edbol = ModUtils.GetArrayVal(strs, 5);
                                        //usrProfile.decPointsPkg = ModUtils.GetArrayVal(strs, 6);
                                        //usrProfile.decPointsWeight = ModUtils.GetArrayVal(strs, 7);
                                        //usrProfile.allowCarrNameEdit = ModUtils.GetArrayVal(strs, 8);
                                        //usrProfile.ignoreTerms = ModUtils.GetArrayVal(strs, 9);
                                        //usrProfile.appendlocBOL = ModUtils.GetArrayVal(strs, 10);
                                        //usrProfile.driverNote = ModUtils.GetArrayVal(strs, 11);
                                        //usrProfile.showContainerSheet = ModUtils.GetArrayVal(strs, 1);
                                        usrProfile.signonlyoneshipment = ModUtils.GetArrayVal(strs, 12);
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    retval = 0;
                                }
                            }
                            else retval = 99043;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retval = 99044;

            }

            return retval;
        }
        public int CheckLoginAttempts(string userid, string ip)
        {
            int tempCheckLoginAttempts = 0;
            int cnt = 0;
            DateTime dt = DateTime.Now;
            DateTime dtNow = DateTime.Now;

            string tstr = "select xdate,loginstatus,sysdate() from loginlog where userid=@uid and srcid=0 and xdate > (sysdate() + interval -5 minute) order by xdate desc";

            try
            {
                using (MySqlConnector.MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(tstr, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", userid);
                        using (MySqlDataReader dtr = cmd.ExecuteReader())
                        {
                            while (dtr.Read())
                            {
                                if (cnt == 0)
                                {
                                    dt = Convert.ToDateTime(dtr[0]).AddMinutes(5);
                                    dtNow = Convert.ToDateTime(dtr[2]);
                                }

                                if (Convert.ToString(dtr[1]) == "1")
                                    break;

                                cnt += 1;
                            }
                        }
                    }
                }
                if (cnt < LOGINATTEMPTS)
                {
                    tempCheckLoginAttempts = 0;
                }
                else
                {
                    tempCheckLoginAttempts = (int)DateHelper.DateDiff(DateHelper.DateInterval.Minute, dtNow, dt);
                    if (tempCheckLoginAttempts == 0)
                        tempCheckLoginAttempts = 1;
                }
            }
            catch (Exception ex)
            {
                tempCheckLoginAttempts = 99;
            }
            finally
            {

            }

            return tempCheckLoginAttempts;
        }

        private int ValidateUser(AuthenticationMode authMode, ref UserProfile usrProfile)
        {
            int retval = 99050;
            string tstr = null;

            try
            {
                if (usrProfile.password.Length >= PASSWDLEN)
                {
                    retval = 99051;

                    if (authMode == AuthenticationMode.Portal)
                    {
                        usrProfile.userid = usrProfile.userid.ToLower();
                        tstr = "select companyid,userid,active,passwd,usertype,whallow,changepwd from userprofile where userid=@uid";
                    }
                    else
                        tstr = "select companyid,userid,active,str2,ts2,'Z' as usertype,srclocation as whallow,'0' as changepwd from a02 where str1=@uid";
                    using (MySqlConnector.MySqlConnection conn = GetConnection())
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(tstr, conn))
                        {
                            cmd.Parameters.AddWithValue("@uid", usrProfile.userid);

                            using (MySqlDataReader dtr = cmd.ExecuteReader())
                            {
                                retval = 99052;

                                //LogMesg.SendLogMessage("Login:" + usrProfile.userid, "Login:" + usrProfile.userType + ":" + passMode);

                                if (dtr.Read())
                                {
                                    usrProfile.companyid = Convert.ToString(dtr["companyid"]);
                                    if (authMode == AuthenticationMode.AppKiosk)
                                        usrProfile.userid = Convert.ToString(dtr["userid"]);

                                    usrProfile.userType = Convert.ToString(dtr["usertype"]);
                                    usrProfile.userstatus = Convert.ToString(dtr["active"]);
                                    usrProfile.allowedwh = Convert.ToString(dtr["whallow"]);

                                    if (usrProfile.userType == "W") // && authMode != AuthenticationMode.WebService)
                                    {
                                        retval = 99053;
                                    }
                                    else if (usrProfile.userType == "Z")
                                    {
                                        if (Convert.ToString(dtr["str2"]) == GetPassNum(ClsCrypt.DecString(usrProfile.password, Convert.ToString(dtr["ts2"]))).ToString())
                                        {
                                            usrProfile.wh = Convert.ToString(dtr["whallow"]);
                                            retval = 0;
                                        }
                                    }
                                    else if (Convert.ToString(dtr["active"]) == "Y")
                                    {
                                        retval = ValidatePasswd(usrProfile.password, Convert.ToString(dtr["passwd"]), authMode == AuthenticationMode.Portal || usrProfile.password.Length < SIDLEN);
                                        usrProfile.changepwd = Convert.ToString(dtr["changepwd"]) == "1";
                                    }
                                    else retval = 99054; // inactive user
                                }
                                else retval = 99055; // userid not found
                            }
                        }
                    }
                }
                else retval = 99056;
            }
            catch (Exception ex)
            {
                retval = 99056;

            }

            return retval;
        }

        private static int ValidatePasswd(string passwd, string dbpasswd, bool userPasswordFlag)
        {
            if (passwd.Length != 0)
            {
                if (userPasswordFlag)
                {
                    dbpasswd = dbpasswd.Substring(2, dbpasswd.Length - 4);
                    passwd = GetPassNum(passwd).ToString();
                }

                //if (passwd == dbpasswd)
                //    return 0;
                //else return 99999;

                return (passwd == dbpasswd ? 0 : 9999);
            }
            else return 99990;
        }

        public static Int64 GetPassNum(string pwd)
        {
            Int64 tempGetPassNum = 0;
            StringBuilder sBuf = new StringBuilder(pwd);

            try
            {
                for (int i = 0; i < sBuf.Length; i++)
                    tempGetPassNum = tempGetPassNum + Convert.ToInt64(sBuf[i]);

                tempGetPassNum = (67 * tempGetPassNum * tempGetPassNum) - (23 * tempGetPassNum) + 82391;
            }
            catch (Exception ex)
            {
                tempGetPassNum = -1;
            }
            return tempGetPassNum;
        }
    }
}
