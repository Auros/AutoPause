using Zenject;
using UnityEngine;
using SiraUtil.Tools;
using AutoPause.Interfaces;

namespace AutoPause.Managers
{
    internal class FPSDetector : ITickable
    {
        private int _frameCount;
        private float _timeCount = -3f;
        private float _lastFrameRate;
        private readonly int _fpsThreshold = 75;
        private readonly float _refreshTime = 0.5f;
        private readonly SiraLog _siraLog;
        private readonly IPauseInvoker _pauseInvoker;

        internal FPSDetector(SiraLog siraLog, IPauseInvoker pauseInvoker, [Inject(Id = "autopause.fps")] int fpsThreshold, [Inject(Id = "autopause.sense")] SenseLevel senseLevel)
        {
            _siraLog = siraLog;
            _pauseInvoker = pauseInvoker;
            _fpsThreshold = fpsThreshold;

            switch (senseLevel)
            {
                case SenseLevel.Low:
                    _refreshTime = 1f;
                    break;
                case SenseLevel.Medium:
                    _refreshTime = 0.5f;
                    break;
                case SenseLevel.High:
                    _refreshTime = 0.25f;
                    break;
            }
        }

        public void Tick()
        {
            if (_timeCount < _refreshTime)
            {
                _timeCount += Time.deltaTime;
                _frameCount++;
            }
            else
            {
                _lastFrameRate = _frameCount / _timeCount;
                _frameCount = 0;
                _timeCount = 0;
                Check();
            }
        }
        
        private void Check()
        {
            if (_lastFrameRate <= _fpsThreshold)
            {
                _siraLog.Debug($"Framerate: {_lastFrameRate}");
                _pauseInvoker.Pause("FPS Drop Detected");
            }
        }
    }
}