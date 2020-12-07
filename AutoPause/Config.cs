using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace AutoPause
{
    internal class Config
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual int FPSThreshold { get; set; } = 60;
        public virtual bool DetectTracking { get; set; } = true;
        public virtual bool DetectFPS { get; set; } = true;
        public virtual bool DoVoices { get; set; } = false;

        [UseConverter(typeof(EnumConverter<SenseLevel>))]
        public virtual SenseLevel Sensitivity { get; set; }
    }
}