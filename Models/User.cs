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

namespace Oyadieyie3D.Models
{
    public class User
    {
        public string ID { get; set; }
        public string Username { get; set; }
        public string ProfileImgUrl { get; set; }
        public string Status { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}