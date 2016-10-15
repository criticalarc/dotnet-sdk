using System;
using System.Threading;
using System.Threading.Tasks;
using CriticalArc.Messaging.Protocols.SafeZone.Alerts;
using CriticalArc.SafeZone;
using Intrinsic.Reactive.Threading.Tasks;
using Intrinsic.Threading.Tasks;
using NLog;

namespace AlertSynchronization
{
    /// <summary>
    ///     Synchronize alert state from SafeZone to a 3rd party system as well as
    ///     limited state synchronization from the 3rd party system back to SafeZone.
    /// </summary>
    public class AlertSynchronizer
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly SafeZoneClientSettings _clientSettings;
        private readonly AlertSynchronizationTarget _target;

        public AlertSynchronizer(SafeZoneClientSettings clientSettings, AlertSynchronizationTarget target)
        {
            _clientSettings = clientSettings;
            _target = target;
        }

        public async Task SynchronizeAsync(CancellationToken cancellationToken)
        {
            var asyncOptions = new AsyncOptions(cancellationToken);

            using (var client = new SafeZoneClient(_clientSettings))
            {
                var consumeEventsTask = client.AlertEvents.ConsumeAsync(
                    async ev =>
                    {
                        try
                        {
                            if (ev.Type == SafeZoneEventType.AddOrUpdate)
                                await _target.OnAlertAddedOrUpdatedAsync(ev.Item);
                            else if (ev.Type == SafeZoneEventType.Remove)
                                await _target.OnAlertRemovedAsync(ev.Item);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Failed to process alert event.");
                        }
                    },
                    asyncOptions);

                var consumeRequestsTask = _target.AlertStateChangeRequested.ConsumeAsync(
                    async req =>
                    {
                        try
                        {
                            switch (req.NewState)
                            {
                                case AlertState.Acknowledge:
                                    await client.AcknowledgeAlertAsync(req.SafeZoneId, req.RaiserId, req.AlertId, asyncOptions);
                                    break;
                                case AlertState.Resolve:
                                    await client.ResolveAlertAsync(req.SafeZoneId, req.RaiserId, req.AlertId, asyncOptions);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Failed to process alert state change request.");
                        }
                    },
                    asyncOptions);

                // Always enable events after subscribing to the observable
                client.AlertEventsEnabled = true;
                client.Connect();

                await Task.WhenAll(consumeEventsTask, consumeRequestsTask);
            }
        }
    }
}