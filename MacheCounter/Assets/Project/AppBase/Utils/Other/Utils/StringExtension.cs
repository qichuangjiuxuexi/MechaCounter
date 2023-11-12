using System;
using System.Collections.Generic;

namespace WordGame.Utils
{
    public static class StringExtension
    {
        public static (int, int) ToItemCountInfo(this string str,char splitChar = ':')
        {
            var arr = str.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length > 1)
            {
                int id = -1;
                int count = -1;
                int.TryParse(arr[0], out id);
                int.TryParse(arr[1], out count);
                return (id,count);
            }
            return (-1,-1);
        }
        
        public static List<int> ToListInt(this string str,char splitChar = ':')
        {
            var arr = str.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length > 0)
            {
                var rst = new List<int>(arr.Length);
                for (int i = 0; i < arr.Length; i++)
                {
                    int id = -1;
                    int.TryParse(arr[i], out id);
                    rst.Add(id);
                }

                return rst;

            }

            return new List<int>(0);
        }
    }
}