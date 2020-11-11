﻿using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Card;
using System;

namespace Oyadieyie3D.Utils
{
    public class CheckableCardView : MaterialCardView
    {
        public CheckableCardView(IntPtr javaReference, JniHandleOwnership transfer):base(javaReference, transfer) { }

        public CheckableCardView(Context context):base(context)
        {
            Init(null);
        }

        public CheckableCardView(Context context, IAttributeSet attrs):base(context, attrs)
        {
            Init(attrs);
        }

        public CheckableCardView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(attrs);
        }


        private void Init(IAttributeSet attrs)
        {
            LayoutInflater.From(Context).Inflate(Resource.Layout.checkable_cardview, this, true);
            Radius = TypedValue.ApplyDimension(ComplexUnitType.Dip, 12, Context.Resources.DisplayMetrics);

            if(attrs != null)
            {
                TypedArray ta = Context.ObtainStyledAttributes(attrs, Resource.Styleable.CheckableCardView, 0, 0);
                try
                {
                    string planTitle = ta.GetString(Resource.Styleable.CheckableCardView_planTitle);
                    string planOffer = ta.GetString(Resource.Styleable.CheckableCardView_planOffer);
                    string planTerms = ta.GetString(Resource.Styleable.CheckableCardView_planTerms);

                    TextView titleTv = FindViewById<TextView>(Resource.Id.s_card_txt);
                    TextView offerTv = FindViewById<TextView>(Resource.Id.s_card_box_txt);
                    TextView termsTv = FindViewById<TextView>(Resource.Id.s_card_offer_det_txt);

                    titleTv.Text = !string.IsNullOrEmpty(planTitle) ? planTitle : "";
                    offerTv.Text = !string.IsNullOrEmpty(planOffer) ? planOffer : "";
                    termsTv.Text = !string.IsNullOrEmpty(planTerms) ? planTerms : "";

                }
                finally
                {
                    ta.Recycle();
                }
            }
        }
    }
}