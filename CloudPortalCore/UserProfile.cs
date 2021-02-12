using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudPortal.Models
{
    public class UserProfile
    {
        public string account = "";
        public string userid = "";
        public string password;
        public string username;
        public string userphone;
        public string fullname;
        public bool changepwd = false;
        public DateTime lastlogin;
        public string companyid = "";
        public string companyName;
        public string companyType = "S";
        public string companyStatus;

        public string allowedwh;
        public string currwh;
        public string defwh;
        public string wh;

        public int servType;
        public int winUser;
        public string userType;
        public string email;
        public string ccemail;
        public string userstatus;
        public string shipsigntype;

        public string edbol;
        public string signdtformat;
        public string signonlyoneshipment;

        public string shipnumcaption;
        public string picknumcaption;

        public string showContainerSheet;

        public string rights;
        public string addUsers = "0";
        public string allowDelete = "0";
        public string allowUnlock = "0";

        public string allowDeliveries = "0";
        public string phone = "";
        public string mpod = "";
        public string sign = "";

        //public System.Drawing.Image shipSign = null;

        public int opmode = 0; // 0 - normal login, 1 - kiosk, 2 - popup from ERP GUI
    }
}
