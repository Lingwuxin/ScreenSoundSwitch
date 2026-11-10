using SoundSwitch.Common.Framework.Process;
using Xunit;

namespace ScreenSoundSwitch.Tests
{
    public class ProcessIdentityTests
    {
        private const string EdgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";

        [Fact]
        public void IsSameApplication_MatchesSameProcessId()
        {
            Assert.True(ProcessIdentity.IsSameApplication(42, null, 42, null));
        }

        [Fact]
        public void IsSameApplication_MatchesSameProcessIdEvenWhenPathsDiffer()
        {
            Assert.True(ProcessIdentity.IsSameApplication(42, @"C:\A\a.exe", 42, @"C:\B\b.exe"));
        }

        [Fact]
        public void IsSameApplication_MatchesDifferentPidsRunningTheSameExecutable()
        {
            // Edge: the window belongs to the browser process, the audio session to the
            // audio-service utility process; both run the same msedge.exe.
            Assert.True(ProcessIdentity.IsSameApplication(100, EdgePath, 200, EdgePath));
        }

        [Fact]
        public void IsSameApplication_IgnoresExecutablePathCasing()
        {
            Assert.True(ProcessIdentity.IsSameApplication(100, EdgePath, 200, EdgePath.ToUpperInvariant()));
        }

        [Fact]
        public void IsSameApplication_TrimsExecutablePaths()
        {
            Assert.True(ProcessIdentity.IsSameApplication(100, " " + EdgePath + " ", 200, EdgePath));
        }

        [Fact]
        public void IsSameApplication_DoesNotMatchDifferentExecutablesInTheSameDirectory()
        {
            Assert.False(ProcessIdentity.IsSameApplication(
                100,
                @"C:\Apps\player.exe",
                200,
                @"C:\Apps\player-helper.exe"));
        }

        [Fact]
        public void IsSameApplication_DoesNotMatchSameFileNameFromDifferentDirectories()
        {
            Assert.False(ProcessIdentity.IsSameApplication(
                100,
                @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                200,
                @"C:\Users\me\Edge\Application\msedge.exe"));
        }

        [Fact]
        public void IsSameApplication_DoesNotMatchWhenExecutablePathIsUnknown()
        {
            Assert.False(ProcessIdentity.IsSameApplication(100, null, 200, EdgePath));
            Assert.False(ProcessIdentity.IsSameApplication(100, EdgePath, 200, null));
            Assert.False(ProcessIdentity.IsSameApplication(100, "", 200, " "));
        }

        [Fact]
        public void IsSameApplication_DoesNotMatchDifferentProcessesWithoutPaths()
        {
            Assert.False(ProcessIdentity.IsSameApplication(0, null, 0, null));
        }
    }
}
