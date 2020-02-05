using System;
using System.Threading.Tasks;
using CriticalArc.Reactive.Subjects;
using CriticalArc.SafeZone.Alerts.Protocol;

namespace AlertSynchronization
{
    public abstract class AlertSynchronizationTarget
    {
        private readonly WeakSubject<AlertStateChangeRequest> _alertStateChangeRequested =
            new WeakSubject<AlertStateChangeRequest>();

        /// <summary>
        ///     Subscribed to by AlertSynchronizer to action alert state change requests.
        /// </summary>
        public IObservable<AlertStateChangeRequest> AlertStateChangeRequested => _alertStateChangeRequested.Subscriber;

        /// <summary>
        ///     Call this method to request a state change for a given alert.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        protected void OnAlertStateChangeRequested(AlertStateChangeRequest request)
        {
            _alertStateChangeRequested.OnNext(request);
        }

        /// <summary>
        ///     Called when an alert has been added or updated in SafeZone.
        ///     A new alert should be added in the target system if one does not already
        ///     exist with the same raiser and alert id combination, otherwise the existing
        ///     alert should be updated with any state and location changes that have occured.
        /// </summary>
        /// <remarks>
        ///     This method will be called for oustanding unresolved alerts when the application first
        ///     starts. It is therefore important that updates in the target system are either idempotent,
        ///     or the appropriate checks are made before hand to ensure the state transition is appropriate.
        /// </remarks>
        /// <param name="alert">An object containing the full state of the alert in the SafeZone system.</param>
        public abstract Task OnAlertAddedOrUpdatedAsync(XSafeZoneAlert alert);

        /// <summary>
        ///     Called when an alert has been resolved or canceled by a responder in SafeZone.
        /// </summary>
        /// <remarks>
        ///     The alert parameter is the state of the alert before it was
        ///     removed from the SafeZone system and will not necessarily have
        ///     its state property set to 'resolved' or 'canceled'.
        /// </remarks>
        /// <param name="alert">An object containing the last known full state of the alert in the SafeZone system.</param>
        public abstract Task OnAlertRemovedAsync(XSafeZoneAlert alert);
    }
}