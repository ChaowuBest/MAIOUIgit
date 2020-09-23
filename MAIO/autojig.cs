using System;
using System.Collections.Generic;
using System.Text;

namespace MAIO
{
    class autojig
    {
        private static char[] constant = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private static char[] num = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        public  string GenerateRandomnum(int length)
        {
            string checkCode = string.Empty;
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                checkCode += num[rd.Next(10)].ToString();
            }
            return checkCode;
        }
        public  string GenerateRandomString(int length)
        {
            string checkCode = string.Empty;
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                checkCode += constant[rd.Next(26)].ToString();
            }
            return checkCode;
        }
    }
}
