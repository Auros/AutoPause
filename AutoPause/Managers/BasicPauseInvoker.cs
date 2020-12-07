using AutoPause.Interfaces;
using SiraUtil.Tools;

namespace AutoPause.Managers
{
    internal class BasicPauseInvoker : IPauseInvoker
    {
        private readonly SiraLog _siraLog;
        private readonly PauseController _pauseController;

        internal BasicPauseInvoker(SiraLog siraLog, PauseController pauseController)
        {
            _siraLog = siraLog;
            _pauseController = pauseController;
        }

        public void Pause(string reason)
        {
            _siraLog.Info(reason);
            _pauseController.Pause();
        }
    }
}