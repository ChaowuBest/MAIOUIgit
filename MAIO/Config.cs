using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MAIO
{
    static class Config
    {
        public static string Key { get; set; }
        public static string webhook { get; set; }
        public static string cid { get; set; }
        public static string cjevent { get; set; }
        public static string delay { get; set; }
        public static string quantity { get; set; }
        public static string Usemonitor { get; set; }
        public static string UseAdvancemode { get; set; }
        public static bool autoclearcookie { get; set; }
        public static string hwid { get; set; }
        public static string ip { get; set; }
        public static Main mn = null;
        public static List<int> qual = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        
    }
   
}
