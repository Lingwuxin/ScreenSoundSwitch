using System;
using System.Linq;
using NAudio.CoreAudioApi;
using SoundSwitch.Common.Framework.Audio.Collection;
using SoundSwitch.Common.Framework.Audio.Device;
using Xunit;

namespace ScreenSoundSwitch.Tests
{
    public class DeviceReadOnlyCollectionTests
    {
        private static DeviceInfo Device(string name, string id, DataFlow type = DataFlow.Render)
        {
            return new DeviceInfo(name, id, type, false, DateTime.UtcNow);
        }

        private static DeviceReadOnlyCollection<DeviceInfo> Collection(DataFlow flow, params DeviceInfo[] devices)
        {
            return new DeviceReadOnlyCollection<DeviceInfo>(devices, flow);
        }

        [Fact]
        public void IntersectWith_MatchesById()
        {
            var collection = Collection(DataFlow.Render, Device("Speakers", "dev-1"));

            var found = collection.IntersectWith(new[] { Device("Renamed", "dev-1") }).ToList();

            Assert.Single(found);
            Assert.Equal("dev-1", found[0].Id);
        }

        [Fact]
        public void IntersectWith_FallsBackToCleanName_WhenIdDiffers()
        {
            var collection = Collection(DataFlow.Render, Device("Speakers (Realtek Audio)", "dev-1"));

            var found = collection.IntersectWith(new[] { Device("Speakers (Realtek Audio)", "dev-2") }).ToList();

            Assert.Single(found);
            Assert.Equal("dev-1", found[0].Id);
        }

        [Fact]
        public void IntersectWith_IgnoresDevicesOfAnotherDataFlow()
        {
            var collection = Collection(DataFlow.Render, Device("Speakers", "dev-1", DataFlow.Render));

            var found = collection.IntersectWith(new[] { Device("Speakers", "dev-1", DataFlow.Capture) });

            Assert.Empty(found);
        }

        [Fact]
        public void IntersectWith_IgnoresUnknownDevices()
        {
            var collection = Collection(DataFlow.Render, Device("Speakers", "dev-1"));

            var found = collection.IntersectWith(new[] { Device("Something else", "dev-9") });

            Assert.Empty(found);
        }

        [Fact]
        public void IntersectWith_ReturnsDistinctDevices()
        {
            var collection = Collection(DataFlow.Render, Device("Speakers", "dev-1"));

            var found = collection.IntersectWith(new[]
            {
                Device("Speakers", "dev-1"),
                Device("Renamed speakers", "dev-1"),
            });

            Assert.Single(found);
        }

        [Fact]
        public void Count_ReportsIndexedDevices()
        {
            var collection = Collection(
                DataFlow.Render,
                Device("Speakers", "dev-1"),
                Device("Headphones", "dev-2"));

            Assert.Equal(2, collection.Count);
        }
    }
}
