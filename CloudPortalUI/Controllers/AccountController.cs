using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudPortal.Models;
using CloudPortal.Repository;
using CloudPortal.Services;
using CloudPortalServices.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CloudPortal.Models.SessionExtensions;

namespace CloudPortal.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _userService;
        private IAccountService _accountService;
        private IDocumentListService _documentService;

        public AccountController(IUserService userService, IAccountService accountService, IDocumentListService documentService)
        {
            _userService = userService;
            _accountService = accountService;
            _documentService = documentService;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
           // AccountRepository context = HttpContext.RequestServices.GetService(typeof(AccountRepository)) as AccountRepository;
            UserProfile userInfo = new UserProfile();
            
            User req = new User();
            req.Username = model.Email;
            req.Password = model.Password;
            // var response = _userService.Authenticate(req);
            userInfo.userid = model.Email;
            userInfo.password = model.Password;
            // userInfo.companyid = _userService.GetById(req.Username);
            userInfo.companyid = _accountService.GetCompanyId(model.Email).ToString();
            if (userInfo.companyid.Length == 0)
                userInfo.companyid = "0";

            int loginSt = 0;
          

               
                if (userInfo.companyid != "0")
                {
                    // check whether the userid has too many failed attempts
                    int retcode = _accountService.CheckLoginAttempts(req.Username, "");

                    if (retcode != 0)
                    {
                       
                       ViewBag.lblerrormsg = "Too many unsuccessful login attempts. Try after " + retcode + " minutes";
                    }
                    else
                    {
                        bool adminflag = HttpContext.Session.GetString(SessionVals.UserType.ToString()) == ADMINUSER;
                        if (_accountService.ValidateUserAuth(Models.AuthenticationMode.Portal, ref userInfo, "", true, adminflag) == 0)
                    {
                        var response = _userService.Authenticate(req);
                        if (userInfo.userstatus != "Y")
                            {
                                loginSt = 2;
                              
                                ViewBag.lblerrormsg = "This Account has been de-activated.";
                            }
                            else if (userInfo.companyStatus == "C" || userInfo.companyStatus == "E")
                            {
                                if (userInfo.userstatus == Models.SessionExtensions.APIUSER || userInfo.userstatus == Models.SessionExtensions.DRIVERUSER)
                                {
                                    loginSt = 2;
                                    
                                    ViewBag.lblerrormsg = "Login not allowed";
                                }
                                else
                                {
                                    //ModUtils.DeleteTmpFiles();
                                    // HttpContext.Session.SetObject("ComplexObject", objNew);
                                    HttpContext.Session.Get("ComplexObject");
                                  
                                    retcode = _accountService.CheckConcurrentUsers(userInfo.companyid, userInfo.userid);
                                    //LogMesg.sendlogmessage("Check:" & retcode, "")
                                    if (retcode == 0)
                                    {
                                       
                                        HttpContext.Session.SetString(SessionVals.EncryptExt.ToString(), DateTime.Now.Millisecond.ToString());
                                        loginSt = 1;
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.Account), userInfo.account);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.UserID), userInfo.userid);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.UserName), userInfo.username);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.UserType), userInfo.userType);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.FromEmail), userInfo.email);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.CCEmail), userInfo.ccemail);
                                     
                                      //  HttpContext.Session.SetString(Convert.ToString(SessionVals.ShipSign), userInfo.shipSign);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.AllowDeliveries), userInfo.allowDeliveries);
                                     
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.AllowDelete), userInfo.allowDelete);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.AllowUnlock), userInfo.allowUnlock);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.AddUsers), userInfo.addUsers);
                                    
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.AllowedWHList), userInfo.allowedwh);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.DefaultWH), userInfo.defwh);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.CurrentWH), userInfo.defwh);
                                   
                                        //HttpContext.Session.SetString(n.SessionVals.DecPointsPkg, userInfo.decPointsPkg);
                                        //HttpContext.Session.SetString(n.SessionVals.DecPointsWeight, userInfo.decPointsWeight);
                                       
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.DisableEdit), userInfo.edbol);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.SignOnlyOneShipment), userInfo.signonlyoneshipment);
                                       
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.ShipnumCaption), userInfo.shipnumcaption);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.PicknumCaption), userInfo.picknumcaption);

                                        //ClsSession.SetSessionVal(ClsSession.SessionVals.SealAtBOL, userInfo.sealAtBOL);

                                        //ClsSession.SetSessionVal(ClsSession.SessionVals.ShowTrailerSheet, userInfo.showContainerSheet);
                                        //ClsSession.SetSessionVal(ClsSession.SessionVals.DriverNote, userInfo.driverNote);
                                        //ClsSession.SetSessionVal(ClsSession.SessionVals.LockRecordAfterSigning, userInfo.LockRecordAfterSigning);
                                        //ClsSession.SetSessionVal(ClsSession.SessionVals.AutoCheckout, userInfo.autocheckout);
                                        //ClsSession.SetSessionVal(ClsSession.SessionVals.AppendLoc, userInfo.appendloc);
                                        //ClsSession.SetSessionVal(ClsSession.SessionVals.AppendLocBOL, userInfo.appendlocBOL);

                                        //ClsSession.SetSessionVal(ClsSession.SessionVals.DriverNameMandatory, userInfo.driverNameMandatory);
                                        //ClsSession.SetSessionVal(ClsSession.SessionVals.AllowCarrNameEdit, userInfo.allowCarrNameEdit);
                                        //ClsSession.SetSessionVal(ClsSession.SessionVals.IgnoreTerms, userInfo.ignoreTerms);


                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.mPOD), userInfo.mpod);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.Sign), userInfo.sign);

                                        if (userInfo.signdtformat.Trim().Length == 0)
                                            userInfo.signdtformat = ModConst.STDDATETIMEFORMAT;

                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.SignDateFormat), userInfo.signdtformat);

                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.OpMode), Convert.ToString(userInfo.opmode));

                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.CompanyID), userInfo.companyid);
                                        HttpContext.Session.SetString(Convert.ToString(SessionVals.CompanyType), userInfo.companyType);

                                        try
                                        {
                                            //if (Request.Cookies[ClsCookies.COOKIENAME] != null)
                                            //{
                                            //    HttpContext.Session.SetString(Convert.ToString(SessionVals.ShipTimeSpan), Server.HtmlEncode(Request.Cookies[ClsCookies.COOKIENAME][ClsSession.SessionVals.ShipTimeSpan.ToString()]));
                                            //    HttpContext.Session.SetString(Convert.ToString(SessionVals.DocTimeSpan), Server.HtmlEncode(Request.Cookies[ClsCookies.COOKIENAME][ClsSession.SessionVals.DocTimeSpan.ToString()]));
                                            //    HttpContext.Session.SetString(Convert.ToString(SessionVals.ShowBOL), Server.HtmlEncode(Request.Cookies[ClsCookies.COOKIENAME][ClsSession.SessionVals.ShowBOL.ToString()]));
                                            //    HttpContext.Session.SetString(Convert.ToString(SessionVals.Printer), Server.HtmlEncode(Request.Cookies[ClsCookies.COOKIENAME][ClsSession.SessionVals.Printer.ToString()]));
                                            //    HttpContext.Session.SetString(Convert.ToString(SessionVals.ConsolidateRows), Server.HtmlEncode(Request.Cookies[ClsCookies.COOKIENAME][ClsSession.SessionVals.ConsolidateRows.ToString()]));
                                            //    HttpContext.Session.SetString(Convert.ToString(SessionVals.ShipRows), Server.HtmlEncode(Request.Cookies[ClsCookies.COOKIENAME][ClsSession.SessionVals.ShipRows.ToString()]));
                                            //    //ClsSession.SetSessionVal(ClsSession.SessionVals.DocumentRows, Server.HtmlEncode(Request.Cookies(ClsCookies.COOKIENAME)(ClsSession.SessionVals.DocumentRows)))
                                            //}
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                    //if (ClsConfig.GetConfigString(ClsConfig.Config.loginalert).ToString() == "Y")
                                    //    LogMesg.SendLogMessage(wserver + " ID:" + userInfo.userid + " " + Environment.MachineName + " Client: " + ip + " " + clientmachine + " " + Request.Url.OriginalString, wserver + " " + Environment.MachineName + " ID:" + userInfo.userid);

                                    _accountService.UpdateLastLogin(userInfo.userid, Convert.ToInt32(userInfo.companyid), HttpContext.Session.GetString("SessionID"));
                                    }
                                    else
                                    {
                                        ViewBag.lblerrormsg = "Licensed User Limit Exceeded";
                                     
                                    }
                                }
                            }
                            else
                            {
                                loginSt = 9;

                                ViewBag.lblerrormsg = "Unpaid or Deactivated Account";
                            }
                        }
                        else
                        {
                            loginSt = 0;
                            if (userInfo.companyStatus == "X")
                            {
                                loginSt = 2;

                                ViewBag.lblerrormsg = "Trial has expired";
                            }
                            else
                            {

                                ViewBag.lblerrormsg = "Inactive User, Invalid UserID or Password";
                            }

                            //if (ClsConfig.GetConfigString(ClsConfig.Config.loginalert).ToString() == "YES")
                            //    LogMesg.SendLogMessage("Login Failed:" + userInfo.userid + " " + lblerrormsg.Text, "Login Failed:" + userInfo.userid);
                        }
                    }

                _accountService.UpdateLoginlog(userInfo.companyid, userInfo.userid, "", "", "", loginSt.ToString(), 0, HttpContext.Session.GetString("SessionID"));
                }

            if (loginSt == 1)
            {
                if (userInfo.companyType == "S")
                {
                    if (userInfo.userType == "P" && HttpContext.Session.GetString(Convert.ToString(SessionVals.Sign)) == "1")
                    {
                        HttpContext.Session.SetString(Convert.ToString(SessionVals.MenuVisible), "n");
                        return RedirectToAction("kioskpickup");
                    }
                    else
                    {
                        HttpContext.Session.SetString(Convert.ToString(SessionVals.MenuVisible), "");
                        if (userInfo.userType == "W")
                        {

                            ViewBag.lblerrormsg = "Login not allowed";
                        }
                        else if (userInfo.changepwd)
                            return RedirectToAction("usersettings.aspx?c=y");
                        else if (userInfo.userType == "D")
                            return RedirectToAction("DocumentList", "Document");
                        else if (HttpContext.Session.GetString(Convert.ToString(SessionVals.Sign)) == "1")
                            return RedirectToAction("DocumentList", "Document");//dashboard--dheeraj
                        else
                            return RedirectToAction("shipments");
                    }
                }
                else
                {
                    return RedirectToAction("DocumentList","Document");
                }
            }

            userInfo = null;


            //SessionExtensions.Set(this,"", "");
            return View();
        }
       

        public IActionResult CheckApi()
        {
           
            return Content("testing");
        }

        

        public IActionResult Dashboard()
        {

            return Content("Dashboard");
        }
       
      

       
        public IActionResult Kioskpickup()
        {
            return Content("Kioskpickup");
        }

        public IActionResult Signout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }



    }
}
