using IPA;
using System.IO;
using IPA.Utilities;
using SiraUtil.Zenject;
using System.Reflection;
using IPA.Config.Stores;
using AutoPause.Installers;
using Conf = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace AutoPause
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static string fpsAudioPath = Path.Combine(UnityGame.UserDataPath, "AutoPause", "Audio", "fps.ogg");
        public static string trackingAudioPath = Path.Combine(UnityGame.UserDataPath, "AutoPause", "Audio", "tracking.ogg");

        internal static IPALogger? Log { get; private set; }

        [Init]
        public Plugin(Conf conf, IPALogger logger, Zenjector zenjector)
        {
            Log = logger;

            zenjector.OnApp<AutoPauseCoreInstaller>().WithParameters(conf.Generated<Config>(), logger);
            zenjector.OnGame<AutoPauseGameInstaller>().OnlyForStandard();
            zenjector.OnMenu<AutoPauseMenuInstaller>();

            Directory.CreateDirectory(Path.Combine(UnityGame.UserDataPath, "AutoPause", "Audio"));
            if (!(File.Exists(fpsAudioPath) || File.Exists(trackingAudioPath)))
            {
                File.WriteAllBytes(fpsAudioPath, BeatSaberMarkupLanguage.Utilities.GetResource(Assembly.GetExecutingAssembly(), "AutoPause.Resources.fps.ogg"));
                File.WriteAllBytes(trackingAudioPath, BeatSaberMarkupLanguage.Utilities.GetResource(Assembly.GetExecutingAssembly(), "AutoPause.Resources.tracking.ogg"));
            }
        }

        [OnEnable, OnDisable]
        public void OnState() { /* Don't need this */ }
    }
}