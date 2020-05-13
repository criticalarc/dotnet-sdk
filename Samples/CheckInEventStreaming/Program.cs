using System;
using CriticalArc;
using CriticalArc.SafeZone;

namespace CheckInLocationStreaming
{
    internal class Program
    {
        private const string UserName = "!!FIXME!!";
        private const string Password = "!!FIXME!!";

        private static void Main(string[] args)
        {
            SdkLog.EnableNLog();

            using var client = new SafeZoneClient(new SafeZoneClientSettings(UserName, Password));

            using var subscription = client.CheckInEvents.Subscribe(x =>
            {
                if (x.Type == SafeZoneEventType.AddOrUpdate)
                {
                    if (x.Item.Timestamp == x.Item.CheckInTimestamp)
                        Console.WriteLine($"Check-In session started. Timestamp: {x.Item.Timestamp?.ToString("O")} UserId: {x.Item.UserId}, Lat: {x.Item.Location.Latitude}, Lon: {x.Item.Location.Longitude}");
                    else
                        Console.WriteLine($"Check-In session updated. Timestamp: {x.Item.Timestamp?.ToString("O")} UserId: {x.Item.UserId}, Lat: {x.Item.Location.Latitude}, Lon: {x.Item.Location.Longitude}");
                }
                else
                {
                    Console.WriteLine($"Check-In session ended. Timestamp: {x.Item.Timestamp?.ToString("O")} UserId: {x.Item.UserId}, Lat: {x.Item.Location.Latitude}, Lon: {x.Item.Location.Longitude}");
                }
            });

            Console.WriteLine("Started check-in stream, press any key to end.");

            client.CheckInEventsEnabled = true;
            client.Connect();

            Console.ReadKey();
        }
    }
}