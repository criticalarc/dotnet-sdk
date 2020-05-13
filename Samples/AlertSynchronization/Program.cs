using System.Threading;
using System.Threading.Tasks;
using CriticalArc;
using CriticalArc.SafeZone;

namespace AlertSynchronization
{
    /// <summary>
    ///     This is a very simple example of how to process alert events coming from the SafeZone system.
    /// </summary>
    internal class Program
    {
        private const string UserName = "!!FIXME!!";
        private const string Password = "!!FIXME!!";

        private static async Task Main(string[] args)
        {
            SdkLog.EnableNLog();

            await new AlertSynchronizer(new SafeZoneClientSettings(UserName, Password), new NLogSynchroniationTarget()).SynchronizeAsync(CancellationToken.None);
        }
    }
}