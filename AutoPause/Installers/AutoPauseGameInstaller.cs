using Zenject;
using AutoPause.Managers;
using AutoPause.Interfaces;

namespace AutoPause.Installers
{
    internal class AutoPauseGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            var config = Container.Resolve<Config>();

            if (!config.Enabled) return;

            int fpsValue = config.FPSThreshold;
            if (config.FPSThreshold >= UnityEngine.XR.XRDevice.refreshRate)
            {
                fpsValue = (int)(0.7f * UnityEngine.XR.XRDevice.refreshRate);
            }

            Container.Bind<int>().WithId("autopause.fps").FromInstance(fpsValue).AsSingle();
            Container.Bind<bool>().WithId("autopause.voice").FromInstance(config.DoVoices).AsSingle();
            Container.Bind<SenseLevel>().WithId("autopause.sense").FromInstance(config.Sensitivity).AsSingle();

            Container.Bind<IPauseInvoker>().To<VisualPauseInvoker>().AsSingle();
            if (config.DetectTracking)
            {
                Container.BindInterfacesTo<TrackingLossDetector>().AsSingle();
            }
            if (config.DetectFPS)
            {
                Container.BindInterfacesTo<FPSDetector>().AsSingle();
            }
        }
    }
}