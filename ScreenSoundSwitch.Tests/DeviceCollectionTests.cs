using System;
using System.Linq;
using NAudio.CoreAudioApi;
using SoundSwitch.Common.Framework.Audio.Collection;
using SoundSwitch.Common.Framework.Audio.Device;
using Xunit;

namespace ScreenSoundSwitch.Tests
{
    public class DeviceCollectionTests
    {
        private static DeviceInfo Device(
            string name,
            string id,
            DataFlow type = DataFlow.Render,
            DateTime? discoveredAt = null)
        {
            return new DeviceInfo(name, id, type, false, discoveredAt ?? DateTime.UtcNow);
        }

        [Fact]
        public void Constructor_SkipsNullEntries()
        {
            var collection = new DeviceCollection<DeviceInfo>(new DeviceInfo[] { null, Device("Speakers", "dev-1") });

            Assert.Single(collection);
        }

        [Fact]
        public void Contains_MatchesById_WhenNameDiffers()
        {
            var collection = new DeviceCollection<DeviceInfo>(new[] { Device("Speakers", "dev-1") });

            Assert.True(collection.Contains(Device("Completely different name", "dev-1")));
        }

        [Fact]
        public void Contains_MatchesByName_WhenIdDiffers()
        {
            var collection = new DeviceCollection<DeviceInfo>(new[] { Device("Speakers (Realtek Audio)", "dev-1") });

            Assert.True(collection.Contains(Device("Speakers (Realtek Audio)", "dev-2")));
        }

        [Fact]
        public void Contains_DoesNotMatch_SameNameOtherDataFlow()
        {
            var collection = new DeviceCollection<DeviceInfo>(
                new[] { Device("Speakers (Realtek Audio)", "dev-1", DataFlow.Render) });

            Assert.False(collection.Contains(Device("Speakers (Realtek Audio)", "dev-2", DataFlow.Capture)));
        }

        [Fact]
        public void Add_DuplicateId_DoesNotIncreaseCount()
        {
            var collection = new DeviceCollection<DeviceInfo>(new[] { Device("Speakers", "dev-1") });

            collection.Add(Device("Another name", "dev-1"));

            Assert.Single(collection);
        }

        [Fact]
        public void Add_NewDevice_IncreasesCount()
        {
            var collection = new DeviceCollection<DeviceInfo>(Array.Empty<DeviceInfo>());

            collection.Add(Device("Speakers", "dev-1"));

            Assert.Single(collection);
            Assert.True(collection.Contains(Device("Speakers", "dev-1")));
        }

        [Fact]
        public void Remove_ByNameMatch_RemovesOriginalDevice()
        {
            var collection = new DeviceCollection<DeviceInfo>(new[] { Device("Speakers (Realtek Audio)", "dev-1") });

            Assert.True(collection.Remove(Device("Speakers (Realtek Audio)", "dev-2")));

            Assert.Empty(collection);
            Assert.False(collection.Contains(Device("Speakers (Realtek Audio)", "dev-1")));
        }

        [Fact]
        public void Remove_UnknownDevice_ReturnsFalse()
        {
            var collection = new DeviceCollection<DeviceInfo>(new[] { Device("Speakers", "dev-1") });

            Assert.False(collection.Remove(Device("Other", "dev-9")));
            Assert.Single(collection);
        }

        [Fact]
        public void Enumerate_OrdersByDiscoveredAt()
        {
            var start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var collection = new DeviceCollection<DeviceInfo>(new[]
            {
                Device("Third", "dev-3", discoveredAt: start.AddMinutes(2)),
                Device("First", "dev-1", discoveredAt: start),
                Device("Second", "dev-2", discoveredAt: start.AddMinutes(1)),
            });

            Assert.Equal(new[] { "dev-1", "dev-2", "dev-3" }, collection.Select(device => device.Id));
        }

        [Fact]
        public void Clear_EmptiesCollection()
        {
            var collection = new DeviceCollection<DeviceInfo>(new[] { Device("Speakers", "dev-1") });

            collection.Clear();

            Assert.Empty(collection);
        }

        [Fact]
        public void Add_Null_Throws()
        {
            var collection = new DeviceCollection<DeviceInfo>(Array.Empty<DeviceInfo>());

            Assert.Throws<ArgumentNullException>(() => collection.Add(null));
        }

        [Fact]
        public void Contains_Null_Throws()
        {
            var collection = new DeviceCollection<DeviceInfo>(Array.Empty<DeviceInfo>());

            Assert.Throws<ArgumentNullException>(() => collection.Contains(null));
        }

        [Fact]
        public void Remove_Null_Throws()
        {
            var collection = new DeviceCollection<DeviceInfo>(Array.Empty<DeviceInfo>());

            Assert.Throws<ArgumentNullException>(() => collection.Remove(null));
        }

        [Fact]
        public void IsReadOnly_IsFalse()
        {
            Assert.False(new DeviceCollection<DeviceInfo>(Array.Empty<DeviceInfo>()).IsReadOnly);
        }
    }
}
