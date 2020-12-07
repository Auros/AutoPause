using Zenject;
using SiraUtil;
using IPA.Logging;
using AutoPause.Managers;

namespace AutoPause.Installers
{
    internal class AutoPauseCoreInstaller : Installer<Config, Logger, AutoPauseCoreInstaller>
    {
        private readonly Config _config;
        private readonly Logger _logger;

        internal AutoPauseCoreInstaller(Config config, Logger logger)
        {
            _config = config;
            _logger = logger;
        }

        public override void InstallBindings()
        {
            Container.BindLoggerAsSiraLogger(_logger);
            Container.BindInstance(_config).AsSingle();
            Container.BindInterfacesAndSelfTo<AutoPauseAudioManager>().AsSingle();
        }
    }
}