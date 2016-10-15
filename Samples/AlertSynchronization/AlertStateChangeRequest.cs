using Intrinsic.Messaging;

namespace AlertSynchronization
{
    public class AlertStateChangeRequest
    {
        public AlertStateChangeRequest(XId safeZoneId, XId raiserId, string alertId, AlertState newState)
        {
            SafeZoneId = safeZoneId;
            RaiserId = raiserId;
            AlertId = alertId;
            NewState = newState;
        }

        public XId SafeZoneId { get; }
        public XId RaiserId { get; }
        public string AlertId { get; }
        public AlertState NewState { get; }
    }
}