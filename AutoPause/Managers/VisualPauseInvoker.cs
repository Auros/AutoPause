using TMPro;
using Zenject;
using UnityEngine;
using IPA.Utilities;
using SiraUtil.Tools;
using UnityEngine.UI;
using AutoPause.Interfaces;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;

namespace AutoPause.Managers
{
    internal class VisualPauseInvoker : IPauseInvoker
    {
        private bool _errorHappening;
        private readonly bool _doVoices;
        private readonly SiraLog _siraLog;
        private readonly TextMeshProUGUI _text;
        private readonly AudioSource _audioSource;
        private readonly PauseController _pauseController;
        private readonly PauseMenuManager _pauseMenuManager;
        private readonly AutoPauseAudioManager _autoPauseAudioManager;

        internal VisualPauseInvoker(SiraLog siraLog, PauseController pauseController, PauseMenuManager pauseMenuManager, AutoPauseAudioManager autoPauseAudioManager, AudioTimeSyncController audioTimeSyncController, [Inject(Id = "autopause.voice")] bool doVoices)
        {
            _siraLog = siraLog;
            _doVoices = doVoices;
            _pauseController = pauseController;
            _pauseMenuManager = pauseMenuManager;
            _autoPauseAudioManager = autoPauseAudioManager;
            var rect = (_pauseMenuManager.GetField<Button, PauseMenuManager>("_restartButton").transform as RectTransform)!;
            _text = BeatSaberUI.CreateText((rect.parent as RectTransform)!, "", Vector2.zero);
            _text.gameObject.transform.localPosition = new Vector2(0f, -8f);
            _text.gameObject.name = "AutoPauseVisualDisplay";
            _text.alignment = TextAlignmentOptions.Center;
            _text.gameObject.SetActive(false);
            _text.fontSize = 6f;

            _audioSource = new GameObject("AutoPauseAudioPlayer").AddComponent<AudioSource>();
            var syncAudio = audioTimeSyncController.GetField<AudioSource, AudioTimeSyncController>("_audioSource");
            _audioSource.outputAudioMixerGroup = syncAudio.outputAudioMixerGroup;
            _audioSource.volume = syncAudio.volume;
        }

        public void Pause(string reason)
        {
            _siraLog.Info(reason);
            if (_errorHappening) return;
            _pauseController.Pause();
            _ = DoPauseStuff(2.5f, reason.Contains("FPS"));
            _text.gameObject.SetActive(true);
            _text.text = reason;
        }

        private async Task DoPauseStuff(float time, bool fpsAudioClip = false)
        {
            _errorHappening = true;
            await SiraUtil.Utilities.AwaitSleep((int)(time * 100));
            _audioSource.PlayOneShot(fpsAudioClip ? _autoPauseAudioManager.FPSAudio : _autoPauseAudioManager.TrackingAudio);
            await SiraUtil.Utilities.AwaitSleep((int)(time * 1000));
            _text.gameObject.SetActive(false);
            _errorHappening = false;
        }
    }
}