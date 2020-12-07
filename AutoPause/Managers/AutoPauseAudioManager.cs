using System.Threading;
using UnityEngine;
using Zenject;

namespace AutoPause.Managers
{
    internal class AutoPauseAudioManager : IInitializable
    {
        private readonly CachedMediaAsyncLoader _cachedMediaAsyncLoader;

        internal AudioClip FPSAudio { get; private set; } = null!;
        internal AudioClip TrackingAudio { get; private set; } = null!;

        internal AutoPauseAudioManager(CachedMediaAsyncLoader cachedMediaAsyncLoader)
        {
            _cachedMediaAsyncLoader = cachedMediaAsyncLoader;
        }

        public async void Initialize()
        {
            TrackingAudio = await _cachedMediaAsyncLoader.LoadAudioClipAsync(Plugin.trackingAudioPath, CancellationToken.None);
            FPSAudio = await _cachedMediaAsyncLoader.LoadAudioClipAsync(Plugin.fpsAudioPath, CancellationToken.None);
        }
    }
}
