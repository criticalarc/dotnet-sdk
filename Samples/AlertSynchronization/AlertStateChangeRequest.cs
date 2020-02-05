using CriticalArc.Messaging;

namespace AlertSynchronization
{
    public class AlertStateChangeRequest
    {
        public AlertStateChangeRequest(XServerId safeZoneId, XAccountId raiserId, XStringId alertId, AlertState newState)
        {
            SafeZoneId = safeZoneId;
            RaiserId = raiserId;
            AlertId = alertId;
            NewState = newState;
        }

        public XServerId SafeZoneId { get; }
        public XAccountId RaiserId { get; }
        public XStringId AlertId { get; }
        public AlertState NewState { get; }
    }
}