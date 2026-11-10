using System;

namespace SoundSwitch.Common.Framework.Process
{
    /// <summary>
    /// Decides whether a window and an audio session belong to the same application.
    /// </summary>
    /// <remarks>
    /// Windows applies the per-application default endpoint to the executable, so the
    /// executable is the identity the audio stack understands. Comparing process IDs alone
    /// fails for multi-process applications: Chromium-based browsers (Edge, Chrome, ...)
    /// render audio in a utility process that runs the same executable as the browser
    /// process owning the window, but with a different PID. Matching the executable path
    /// keeps the granularity identical to the Windows API, and deliberately does not fall
    /// back to the process tree: processes started by the same parent (for example anything
    /// launched from Explorer) would otherwise be confused with each other.
    /// </remarks>
    public static class ProcessIdentity
    {
        /// <summary>
        /// Returns true when the window process and the audio session process are the same
        /// application: either the exact same process, or two processes running the same
        /// executable.
        /// </summary>
        public static bool IsSameApplication(
            uint windowProcessId,
            string windowExecutablePath,
            uint sessionProcessId,
            string sessionExecutablePath)
        {
            if (windowProcessId != 0 && windowProcessId == sessionProcessId)
            {
                return true;
            }

            var windowPath = NormalizePath(windowExecutablePath);
            var sessionPath = NormalizePath(sessionExecutablePath);

            return windowPath != null
                   && sessionPath != null
                   && string.Equals(windowPath, sessionPath, StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizePath(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? null : path.Trim();
        }
    }
}
