using UnityEngine;

namespace VarVarGamejam.Map
{
    public class Timer
    {
        public void Start(float time, bool goUp)
        {
            Time = goUp ? 0f : time;
            _max = time;
            _goUp = goUp;
            _isRunning = true;
        }

        public void Update(float elapsed)
        {
            if (_isRunning)
            {
                Time += elapsed * (_goUp ? 1f : -1f);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public float Lerp(float max)
        {
            return Mathf.Lerp(0f, max, Time / _max);
        }

        public float Time { get; private set; }
        public float _max;
        private bool _goUp;
        private bool _isRunning;
    }
}
