using System;
using Zenject;
using System.Linq;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.Attributes;

namespace AutoPause.UI
{
    internal class MenuManager : IInitializable, IDisposable
    {
        private readonly Config _config;

        [UIValue("enabled")]
        protected bool Enabled
        {
            get => _config.Enabled;
            set => _config.Enabled = value;
        }

        [UIValue("fps-threshold")]
        protected int FPSThreshold
        {
            get => _config.FPSThreshold;
            set => _config.FPSThreshold = value;
        }

        [UIValue("detect-tracking")]
        protected bool DetectTracking
        {
            get => _config.DetectTracking;
            set => _config.DetectTracking = value;
        }

        [UIValue("detect-fps")]
        protected bool DetectFPS
        {
            get => _config.DetectFPS;
            set => _config.DetectFPS = value;
        }

        [UIValue("do-voices")]
        protected bool DoVoices
        {
            get => _config.DoVoices;
            set => _config.DoVoices = value;
        }

        [UIValue("sensitivity")]
        protected SenseLevel Sensitivity
        {
            get => _config.Sensitivity;
            set => _config.Sensitivity = value;

        }

        [UIValue("sensitivities")]
        protected List<object> Sensitivities => ((SenseLevel[])Enum.GetValues(typeof(SenseLevel))).AsEnumerable().Select(v => (object)v).ToList();

        internal MenuManager(Config config)
        {
            _config = config;
        }

        public void Initialize()
        {
            BSMLSettings.instance.AddSettingsMenu("AutoPause", "AutoPause.Views.settings.bsml", this);
        }

        public void Dispose()
        {
            BSMLSettings.instance.RemoveSettingsMenu(this);
        }
    }
}