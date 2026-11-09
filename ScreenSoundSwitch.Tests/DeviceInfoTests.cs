using System;
using NAudio.CoreAudioApi;
using SoundSwitch.Common.Framework.Audio.Device;
using Xunit;

namespace ScreenSoundSwitch.Tests
{
    public class DeviceInfoTests
    {
        private static DeviceInfo Create(
            string name,
            string id = "device-id",
            DataFlow type = DataFlow.Render,
            bool isUsb = false,
            DateTime? discoveredAt = null)
        {
            return new DeviceInfo(name, id, type, isUsb, discoveredAt ?? DateTime.UtcNow);
        }

        [Theory]
        [InlineData("Speakers")]
        [InlineData("Headphones (Realtek(R) Audio)")]
        [InlineData("Microphone (USB Audio Device)")]
        public void NameClean_LeavesCanonicalNamesUnchanged(string name)
        {
            Assert.Equal(name, Create(name).NameClean);
        }

        [Theory]
        [InlineData("2- Speakers (Realtek Audio)", "Speakers (Realtek Audio)")]
        [InlineData("1- Headphones (Realtek Audio)", "Headphones (Realtek Audio)")]
        public void NameClean_StripsWindowsDuplicateIndexFromFriendlyName(string name, string expected)
        {
            Assert.Equal(expected, Create(name).NameClean);
        }

        [Fact]
        public void NameClean_StripsIndexFromDevicePart()
        {
            Assert.Equal("Speakers (Realtek Audio)", Create("Speakers (2- Realtek Audio)").NameClean);
        }

        [Fact]
        public void NameClean_RemovesDuplicateMarkerFromFriendlyName()
        {
            Assert.Equal("Speakers (Realtek Audio)", Create("Speakers (1) (Realtek Audio)").NameClean);
        }

        [Fact]
        public void NameClean_IsComputedOnceAndCached()
        {
            var device = Create("Speakers (Realtek Audio)");

            Assert.Same(device.NameClean, device.NameClean);
        }

        [Fact]
        public void Equals_SameId_DifferentName_AreEqual()
        {
            var first = Create("Speakers", "dev-1");
            var renamed = Create("Renamed Speakers", "dev-1");

            Assert.True(first.Equals(renamed));
            Assert.True(first == renamed);
            Assert.False(first != renamed);
        }

        [Fact]
        public void Equals_DifferentId_SameCleanName_AreEqual()
        {
            var first = Create("Speakers (Realtek Audio)", "dev-1");
            var renumbered = Create("Speakers (Realtek Audio)", "dev-2");

            Assert.True(first.Equals(renumbered));
        }

        [Fact]
        public void Equals_DifferentType_AreNotEqual()
        {
            var playback = Create("Speakers (Realtek Audio)", "dev-1", DataFlow.Render);
            var recording = Create("Speakers (Realtek Audio)", "dev-1", DataFlow.Capture);

            Assert.False(playback.Equals(recording));
        }

        [Fact]
        public void Equals_Null_IsFalse()
        {
            var device = Create("Speakers");

            Assert.False(device.Equals((DeviceInfo)null));
            Assert.False(device.Equals((object)null));
            Assert.False(device == null);
            Assert.True(device != null);
        }

        [Fact]
        public void Equals_OtherObjectType_IsFalse()
        {
            var device = Create("Speakers");

            Assert.False(device.Equals("Speakers"));
        }

        [Fact]
        public void GetHashCode_IsEqualForEqualDevices()
        {
            var first = Create("Speakers (Realtek Audio)", "dev-1");
            var renumbered = Create("Speakers (Realtek Audio)", "dev-2");

            Assert.Equal(first.GetHashCode(), renumbered.GetHashCode());
        }

        [Fact]
        public void ToString_ContainsTypeAndCleanName()
        {
            var device = Create("2- Speakers (Realtek Audio)", "dev-1", DataFlow.Render);

            Assert.Equal("[Render]Speakers (Realtek Audio)", device.ToString());
        }

        [Fact]
        public void Constructor_KeepsGivenValues()
        {
            var discoveredAt = new DateTime(2024, 5, 6, 7, 8, 9, DateTimeKind.Utc);
            var device = Create("Speakers", "dev-1", DataFlow.Capture, isUsb: true, discoveredAt: discoveredAt);

            Assert.Equal("dev-1", device.Id);
            Assert.Equal(DataFlow.Capture, device.Type);
            Assert.True(device.IsUsb);
            Assert.Equal(discoveredAt, device.DiscoveredAt);
        }
    }
}
