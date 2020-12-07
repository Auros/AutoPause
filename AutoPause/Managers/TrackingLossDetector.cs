using Zenject;
using System.Linq;
using UnityEngine;
using AutoPause.Interfaces;
using System.Collections.Generic;
using System;

namespace AutoPause.Managers
{
    internal class TrackingLossDetector : IInitializable, IDisposable, ITickable
    {
        private bool _canPing = true;
        private float _currentTime = -3f;
        private readonly float _cycleTime = 0.075f;
        private readonly List<Pose> _leftPool;
        private readonly List<Pose> _rightPool;
        private readonly IGamePause _gamePause;
        private readonly SaberManager _saberManager;
        private readonly IPauseInvoker _pauseInvoker;
        private readonly IMenuButtonTrigger _menuButtonTrigger;
        private readonly SaberType? _activeHand;

        internal TrackingLossDetector(SaberManager saberManager, IPauseInvoker pauseInvoker, IGamePause gamePause, IMenuButtonTrigger menuButtonTrigger, SaberManager.InitData init, [Inject(Id = "autopause.sense")] SenseLevel senseLevel)
        {
            _gamePause = gamePause;
            _saberManager = saberManager;
            _pauseInvoker = pauseInvoker;
            _leftPool = new List<Pose>();
            _rightPool = new List<Pose>();
            _menuButtonTrigger = menuButtonTrigger;
            if (init.oneSaberMode)
            {
                _activeHand = init.oneSaberType;
            }

            switch (senseLevel)
            {
                case SenseLevel.Low:
                    _cycleTime = 0.15f;
                    break;
                case SenseLevel.Medium:
                    _cycleTime = 0.075f;
                    break;
                case SenseLevel.High:
                    _cycleTime = 0.035f;
                    break;
            }
        }

        public void Tick()
        {
            if (!_canPing) return;
            _currentTime += Time.deltaTime;
            if (_currentTime >= _cycleTime)
            {
                _currentTime = 0;

                var leftPose = _leftPool.FirstOrDefault();
                var rightPose = _rightPool.FirstOrDefault();

                bool checkLeft = Available(SaberType.SaberA);
                bool checkRight = Available(SaberType.SaberB);

                bool leftSaberPosFailed = _leftPool.All(u => u.position == leftPose.position) && checkLeft;
                bool rightSaberPosFailed = _rightPool.All(u => u.position == rightPose.position) && checkRight;

                if (leftSaberPosFailed || rightSaberPosFailed)
                {
                    _pauseInvoker.Pause(leftSaberPosFailed ? "Left Saber Lost Tracking" : "Right Saber Lost Tracking");
                    _canPing = false;
                }
                else
                {
                    bool leftSaberRotFailed = _leftPool.All(u => u.rotation == leftPose.rotation) && checkLeft;
                    bool rightSaberRotFailed = _rightPool.All(u => u.rotation == rightPose.rotation) && checkRight;

                    if (leftSaberRotFailed || rightSaberRotFailed)
                    {
                        _pauseInvoker.Pause(leftSaberRotFailed ? "Left Saber Lost Tracking [Rotation]" : "Right Saber Lost Tracking [Rotation]");
                        _canPing = false;
                    }
                    else
                    {
                        var leftDriftValid = OutBox(leftPose.position);
                        if (leftDriftValid || OutBox(rightPose.position))
                        {
                            _pauseInvoker.Pause(leftDriftValid ? "Left Saber Lost Tracking [Drift]" : "Right Saber Lost Tracking [Drift]");
                            _canPing = false;
                        }
                    }
                }
                
                _leftPool.Clear();
                _rightPool.Clear();
            }
            var leftTransform = _saberManager.leftSaber.transform;
            var rightTransform = _saberManager.rightSaber.transform;
            _leftPool.Add(new Pose(leftTransform.position, leftTransform.rotation));
            _rightPool.Add(new Pose(rightTransform.position, rightTransform.rotation));
        }

        public void Initialize()
        {
            _gamePause.didResumeEvent += DidResume;
            _menuButtonTrigger.menuButtonTriggeredEvent += MenuTriggered;
        }

        private void MenuTriggered()
        {
            _canPing = false;
        }

        private void DidResume()
        {
            _canPing = true;
        }

        public void Dispose()
        {
            _gamePause.didResumeEvent -= DidResume;
            _menuButtonTrigger.menuButtonTriggeredEvent -= MenuTriggered;
        }

        private bool Available(SaberType saber)
        {
            if (_activeHand.HasValue)
            {
                return _activeHand == saber;
            }
            return true;
        }

        private const float box = 4.20f;
        public static bool OutBox(Vector3 vec)
        {
            return Math.Abs(vec.x) > box && Math.Abs(vec.y) > box && Math.Abs(vec.z) > box;
        }
    }
}
