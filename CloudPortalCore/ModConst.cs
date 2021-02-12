using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudPortal.Models
{
	public class ModConst
	{
		public const string VERSION = "2.0";
		public const string SCARRIER = "Select Carrier";

		public const int DEFPAGESIZE = 25;
		public const string SRPTBYTS = "rptb";
		public const string ADDRLOGO = "alogo";
		public const string SPICKTOKEN = "pcktn";
		public const string SDOCRECID = "docrecid";

		private static string[] mainTableNames = new string[] { "shiphdr", "shipdtl" };
		private static string[] tempTableNames = new string[] { "tmpshiphdr", "tmpshipdtl" };

		public static string[] countryList = { "USA", "Canada", "Mexico" };

		public const string validationKey = "AEAAEACZ3SgquWQI8nKlFtveN7pfAwABAAE=";
		public const string GDLICENSE = "21187444692637971111412425278535728212";

		public const string SSHIPMENT = "S";
		public const string STRAILER = "T";
		public const string SBOL = "B";

		public const string SMASTERBOL = "M";
		public const string SSINGLEBOL = "S";
		public const string SUNDERLYINGBOL = "U";

		public const string LICAB = "AB";
		public const string LICBD = "BD";

		public const string STDBOLDTFORMAT = "M/d/yyyy";
		public const string STDDATETIMEFORMAT = "M/d/yyyy h:mmtt";
		public const string STDTIMEFORMAT = "hh:mm tt";

		public const string IMPTRANSIDFORMAT = "ddHHmmssffff";
		public const string DLSHIPDATEFORMAT = "yyyyMMddHHmmss";

		public const string passPhrase = "sbsol#@519";
		public const string saltValue = "slt@505!ent";

		public const string rptPassPhrase = "frmdsgn0118";
		public const string rptSaltValue = "saltjan18";

		public const string DEFUSRCOUNT = "5";
		public const string DEFUSRTYPE = "N";
		public const string DEFLOCCOUNT = "1";

		public const string DEFSHIPTYPE = "V";
		public const string STRCOMMA = ", - Comma";
		public const string STRSEMICOLON = "; - Semi-colon";
		public const string STRPIPE = "| - Pipe";

		public const string STRSELECTPRINTER = "--Select Printer --";

	}
}
