using System.Threading.Tasks;
using CriticalArc.Messaging.Protocols.SafeZone.Alerts;
using NLog;

namespace AlertSynchronization
{
    public class NLogSynchroniationTarget : AlertSynchronizationTarget
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public override async Task OnAlertAddedOrUpdatedAsync(XSafeZoneAlert alert)
        {
            // NOTE: Compare the constants in XSafeZoneAlertStates to alert.State in order to detemine what the current state is.
            //       Location data is in the alert.Location object, and at minumum provides Latitude, Longitude, Timestamp.

            //       Certain metadata, like name, email, mobile should always be available, but the program should handle it being absent.
            //       Metadata about the raiser can be found in the alert.Raiser object.
            //       e.g. to get their email address, use:
            //       EmailAddress emailAddress = XSafeZoneAlertRaiserFields.EmailAddress.GetValueOrDefault(alert.Raiser);

            Log.Info($"Added alert/updated with id: {alert.GlobalId}, state: {alert.State}, lat: {alert.Location?.Latitude}, lon: {alert.Location?.Longitude}.");
        }

        public override async Task OnAlertRemovedAsync(XSafeZoneAlert alert)
        {
            Log.Info($"Removed alert with id: {alert.GlobalId}.");
        }
    }
}