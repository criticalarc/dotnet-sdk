using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using CriticalArc.Collections.Generic;
using CriticalArc.Messaging;
using CriticalArc.Messaging.Modules.Tags.Protocol;
using CriticalArc.Messaging.Stanzas;
using CriticalArc.SafeZone;
using CriticalArc.SafeZone.CheckIns.Protocol;
using CriticalArc.SafeZone.CheckIns.Service;
using CriticalArc.SafeZone.Core.Protocol;
using CriticalArc.SafeZone.Core.Service;
using CriticalArc.SafeZone.Users.Service;
using CriticalArc.Threading.Tasks;

namespace CheckInSummaryReporting
{
    internal class Program
    {
        private const string UserName = "!!FIXME!!";
        private const string Password = "!!FIXME!!";
        private const int MaxResultsPerPage = 1000;
        private static readonly XServerId SafeZoneId = (XServerId) "!!FIXME!!";
        private static readonly DateTimeOffset ReportStartTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(1);
        private static readonly DateTimeOffset ReportEndTime = DateTimeOffset.UtcNow;

        private static async Task Main(string[] args)
        {
            using var client = new SafeZoneClient(new SafeZoneClientSettings(UserName, Password));
            
            client.Connect();

            var tags = new XTags
            {
                // report on check-ins for every region
                XTag.Build.RegionTag(string.Empty)
            };
            var regionNames = new ConcurrentDictionary<XStringId, string>();
            var userNames = new ConcurrentDictionary<XAccountId, string>();

            var results = client.CheckInClient.GetSessions(SafeZoneId, ReportStartTime, ReportEndTime, tags, MaxResultsPerPage);

            await results.ForEachPageAsync(async page =>
                {
                    foreach (var session in page)
                    {
                        if (!regionNames.TryGetValue(session.RegionId, out var regionName))
                        {
                            try
                            {
                                var region = await client.CoreClient.GetRegionAsync(SafeZoneId, session.RegionId, AsyncOptions.None);
                                regionNames[session.RegionId] = regionName = region.DisplayNameOrId;
                            }
                            catch (XStanzaErrorException)
                            {
                                regionNames[session.RegionId] = regionName = (string) session.RegionId;
                            }
                        }

                        if (!userNames.TryGetValue(session.UserId, out var userName))
                        {
                            try
                            {
                                var user = await client.UserClient.GetUserAsync(SafeZoneId, session.UserId, AsyncOptions.None);
                                userNames[session.UserId] = userName = user.Fields.DisplayName ?? user.Fields.GivenFamilyName;
                            }
                            catch (XStanzaErrorException)
                            {
                                userNames[session.UserId] = userName = (string) session.UserId;
                            }
                        }

                        if (session.ExitedTimestamp == null)
                            Console.WriteLine($"{userName} is currently checked in at {regionName} since {session.EnteredTimestamp.Value.ToLocalTime():g}");
                        else
                            Console.WriteLine($"{userName} was checked in at {regionName} between {session.EnteredTimestamp.Value.ToLocalTime():g} and {session.ExitedTimestamp.Value.ToLocalTime():g}");
                    }
                },
                AsyncOptions.None);
        }
    }
}