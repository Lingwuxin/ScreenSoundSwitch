using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ScreenSoundSwitch.WinUI.Data;
using System.Collections.ObjectModel;
using System.Linq;

namespace ScreenSoundSwitch.WinUI.Views
{
    public sealed partial class DebugPage : Page
    {
        private readonly ObservableCollection<string> _logItems = new();

        public DebugPage()
        {
            this.InitializeComponent();
            LogListView.ItemsSource = _logItems;
            DebugLogStore.LogAdded += DebugLogStore_LogAdded;
            DebugLogStore.LogsCleared += DebugLogStore_LogsCleared;
            ReloadLogs();
        }

        private void DebugLogStore_LogAdded(string log)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                _logItems.Add(log);
                ScrollToLatest();
                DebugStatusInfoBar.Severity = InfoBarSeverity.Success;
                DebugStatusInfoBar.Message = "收到新日志。";
            });
        }

        private void DebugLogStore_LogsCleared()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                _logItems.Clear();
                DebugStatusInfoBar.Severity = InfoBarSeverity.Informational;
                DebugStatusInfoBar.Message = "日志已清空。";
            });
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadLogs();
            ScrollToLatest();
            DebugStatusInfoBar.Severity = InfoBarSeverity.Informational;
            DebugStatusInfoBar.Message = "日志已刷新。";
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DebugLogStore.Clear();
            _logItems.Clear();
        }

        private void ReloadLogs()
        {
            _logItems.Clear();
            foreach (var log in DebugLogStore.Logs)
            {
                _logItems.Add(log);
            }
        }

        private void ScrollToLatest()
        {
            if (_logItems.Count == 0)
            {
                return;
            }

            LogListView.ScrollIntoView(_logItems.Last(), ScrollIntoViewAlignment.Leading);
        }

        protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            DebugLogStore.LogAdded -= DebugLogStore_LogAdded;
            DebugLogStore.LogsCleared -= DebugLogStore_LogsCleared;
        }
    }
}
