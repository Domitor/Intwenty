using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Helpers
{
    public static class PhoneNumberHelper
    {
        public static string GetCellPhone(this string cellphone)
        {

            cellphone = cellphone.Trim();

            if (cellphone.Length < 9)
            {
                return "INVALID";
            }

            cellphone = cellphone.Replace("+", "");

            foreach (var c in cellphone.ToCharArray())
            {
                if (!char.IsNumber(c))
                    return "INVALID";
            }

            if (cellphone.Length == 10 && cellphone.Substring(0, 2) == "07")
            {
                return "0046" + cellphone.Substring(1).Trim();
            }
            else if (cellphone.Length == 9 && cellphone.Substring(0, 1) == "7")
            {
                return "0046" + cellphone.Trim();
            }
            else if (cellphone.Length == 13 && cellphone.Substring(0, 5) == "00467")
            {
                return cellphone;
            }
            else if (cellphone.Length == 14 && cellphone.Substring(0, 6) == "004607")
            {
                return "0046" + cellphone.Substring(5).Trim();
            }
            else if (cellphone.Length == 11 && cellphone.Substring(0, 3) == "467")
            {
                return "00" + cellphone.Trim();
            }
            else if (cellphone.Length == 12 && cellphone.Substring(0, 4) == "4607")
            {
                return "0046" + cellphone.Substring(3).Trim();
            }
            else
            {
                return "INVALID";
            }


        }

    }

}
