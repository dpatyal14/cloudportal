using CloudPortal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPortalServices.Services
{
    public interface IAccountService
    {
        int GetCompanyId(string userid);
        void GetLicenseInfo(string compid, ref int usrCount, ref string usrType, ref int locCount);
        int CheckConcurrentUsers(string compid, string userid);
        void UpdateLoginlog(string companyid, string userid, string ip, string machinename, string wserver, string loginst, int srcID, string sessionid = "");
        int ValidateUserAuth(AuthenticationMode authMode, ref UserProfile usrProfile, string ip, bool signflag = false, bool adminflag = false);
        void UpdateLastLogin(string userid, int compid, string sessionid);
        int CheckLoginAttempts(string userid, string ip);
    }
}
