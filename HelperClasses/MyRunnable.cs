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
using Java.Lang;

namespace Oyadieyie3D.HelperClasses
{
    internal sealed class MyRunnable : Java.Lang.Object, IRunnable
    {
        Action run;
        public MyRunnable(Action run)
        {
            this.run = run;
        }
        public void Run()
        {
            run?.Invoke();
        }
    }
}