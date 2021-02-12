using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace CloudPortal.Models
{
    public static class SessionExtensions
    {
        public enum SessionVals
        {
            ShowBOL,
            EncryptExt,
            Account,
            CompanyID,
            CompanyName,
            CompanyType,
            UserID,
            UserName,
            UserType,

            DateRange,
            DefaultWH,
            CurrentWH,
            AllowedWHList,
            TimeZone,
            ShipRows,
            DashRows,
            ConsolidateRows,
            //DocumentRows
            Printer,
            ShipTimeSpan,
            DocTimeSpan,

            FromEmail,
            CCEmail,
            //ShipSign,
            DisableEdit,
            AllowDelete,
            AllowUnlock,
            AddUsers,

            SignDateFormat,

            //SealAtBOL,
            //DriverNameMandatory,
            //DriverNote,
            //DecPointsPkg,
            //DecPointsWeight,

            DeLimiter,
            MenuVisible,
            DriverInfo,
            //DLFront
            //DLBack
            LogoImg,
            SBytes,
            SRpt,
            OpMode,
            LicenseName,

            ShowPO,
            ShowRevisions,
            ShowArchiveDocs,

            //LockRecordAfterSigning,
            //AutoCheckout,
            //AppendLoc,
            //AppendLocBOL,
            //AllowCarrNameEdit,
            //IgnoreTerms,

            mPOD,
            Sign,

            AllowDeliveries,

            ShowSignPopup,
            ShipnumCaption,
            PicknumCaption,
            SignOnlyOneShipment

            //ShowTrailerSheet,
        }
        public static string ADMINUSER = "A";
        public static string SYSTEMUSER = "S";
        public static string PICKUPUSER = "P";
        public static string DOCVIEWERUSER = "D";
        public static string DRIVERUSER = "V";
        public static string NOLOGIN = "N";
        public static string APIUSER = "W";

        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }


    }
}
