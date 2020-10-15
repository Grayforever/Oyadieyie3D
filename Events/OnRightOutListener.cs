using Ramotion.PaperOnboarding.Listeners;
using System;

namespace Oyadieyie3D.Events
{
    internal sealed class OnRightOutListener : Java.Lang.Object, IPaperOnboardingOnRightOutListener
    {
        private Action _onRightOut;
        public OnRightOutListener(Action onRightOut)
        {
            _onRightOut = onRightOut;
        }
        public void OnRightOut() => _onRightOut?.Invoke();
    }
}