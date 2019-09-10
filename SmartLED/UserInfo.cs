using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace SmartLED
{
    [Table("UserInfo")]
    public class UserInfo
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string Pwd { get; set; }
        public int Remember { get; set; }
        public int Lastest { get; set; }
    }
}