using CloudPortal.Models;
using CloudPortalRepository.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPortalServices.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _iAccountRepository;
        public AccountService(IAccountRepository iAccountRepository)
        {
            _iAccountRepository = iAccountRepository;
        }

        public int GetCompanyId(string userid) {
            return _iAccountRepository.GetCompanyId(userid);
        }
        public void GetLicenseInfo(string compid, ref int usrCount, ref string usrType, ref int locCount) {
             _iAccountRepository.GetLicenseInfo(compid, ref usrCount, ref usrType, ref locCount);
        }
        public int CheckConcurrentUsers(string compid, string userid) {
            return _iAccountRepository.CheckConcurrentUsers(compid,userid);
        }
        public void UpdateLoginlog(string companyid, string userid, string ip, string machinename, string wserver, string loginst, int srcID, string sessionid = "") {
            _iAccountRepository.UpdateLoginlog(companyid,  userid,  ip,  machinename,  wserver,  loginst,  srcID,  sessionid);
        }
        public int ValidateUserAuth(AuthenticationMode authMode, ref UserProfile usrProfile, string ip, bool signflag = false, bool adminflag = false) {
            return _iAccountRepository.ValidateUserAuth(authMode,ref usrProfile, ip, signflag,  adminflag);
        }
        public void UpdateLastLogin(string userid, int compid, string sessionid) {
             _iAccountRepository.UpdateLastLogin(userid, compid, sessionid);
        }
        public int CheckLoginAttempts(string userid, string ip) {
            return _iAccountRepository.CheckLoginAttempts(userid, ip);
        }
    }
}
