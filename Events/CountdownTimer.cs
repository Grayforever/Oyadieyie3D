using Android.OS;
using System;

namespace Oyadieyie3D.Events
{
    public class CountdownTimer : CountDownTimer
    {
        Action _onFinsh;
        Action<long> _onTick;
        public CountdownTimer(long millisInFuture, long countDownInterval, Action onFinsh, Action<long> onTick):base(millisInFuture, countDownInterval)
        {
            _onFinsh = onFinsh;
            _onTick = onTick;
        }
        public override void OnFinish() => _onFinsh?.Invoke();

        public override void OnTick(long millisUntilFinished) => _onTick?.Invoke(millisUntilFinished);
    }
}