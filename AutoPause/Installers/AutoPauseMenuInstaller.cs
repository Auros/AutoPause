using Zenject;
using AutoPause.UI;

namespace AutoPause.Installers
{
    internal class AutoPauseMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<MenuManager>().AsSingle();
        }
    }
}
