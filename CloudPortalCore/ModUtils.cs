using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudPortal.Models
{
    public class ModUtils
    {
        public static string AddEscChar(string str)
        {
            return str.Replace("'", "\\'"); //test
        }

        public string Removenbsp(string str)
        {
            return (str == "&nbsp;" ? "" : str);
        }

        public static string GetArrayVal(string[] arr, int indx)
        {
            string tempGetArrayVal = "";
            try
            {
                if (indx < arr.Length)
                    tempGetArrayVal = arr[indx];
            }
            catch (Exception ex)
            {
                tempGetArrayVal = "";
            }
            return tempGetArrayVal;
        }


    }
}
