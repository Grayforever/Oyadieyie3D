using Android.Animation;
using Android.Views;
using System;
using static Android.Animation.Animator;

namespace Oyadieyie3D.HelperClasses
{
    public class ViewAnimator
    {
        public static bool RotateFab(View v, bool rotate)
        {
            v.Animate()
                .SetDuration(200)
                .SetListener(new AnimatorListener
                {
                    AnimationCancel = (Animator animator) =>
                    {

                    },
                    AnimationEnd = (Animator animator) =>
                    {
                        
                    },
                    AnimationStart = (Animator animator) =>
                    {

                    },
                    AnimationRepeat = (Animator animator) =>
                    {

                    }
                })
                .Rotation(rotate ? 135f : 0f);
            return rotate;
        }

        internal sealed class AnimatorListener : Java.Lang.Object, IAnimatorListener
        {
            public Action<Animator> AnimationCancel;
            public Action<Animator> AnimationEnd;
            public Action<Animator> AnimationRepeat;
            public Action<Animator> AnimationStart;
            public void OnAnimationCancel(Animator animation)
            {
                AnimationCancel(animation);
            }

            public void OnAnimationEnd(Animator animation)
            {
                AnimationEnd(animation);
            }

            public void OnAnimationRepeat(Animator animation)
            {
                AnimationRepeat(animation);
            }

            public void OnAnimationStart(Animator animation)
            {
                AnimationStart(animation);
            }
        }
    }
}