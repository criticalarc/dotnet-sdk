using System.Threading;
using CriticalArc;
using CriticalArc.SafeZone;

namespace AlertSynchronization
{
    /// <summary>
    ///     This is a very simple example of how to process alert events coming from the SafeZone system.
    /// </summary>
    class Program
    {
        private const string UserName = "!!FIXME!!";
        private const string Password = "!!FIXME!!";

        static void Main(string[] args)
        {
            SdkLog.EnableNLog();

            new AlertSynchronizer(new SafeZoneClientSettings(UserName, Password), new NLogSynchroniationTarget())
                .SynchronizeAsync(CancellationToken.None)
                .Wait();
        }
    }
}