using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.WinUI.ViewModels;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.UI.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.View
{
    public sealed partial class AudioDeviceControl : UserControl
    {
        private AudioDeviceControlViewModel viewModel;
        private CancellationTokenSource _scrollCancellationTokenSource;
        private bool _isScrolling = false; // ��¼�Ƿ����ڹ���
        MMDevice device;
        bool block = false;
        private AudioEndpointVolume audioEndpointVolume;
        public StackPanel _ProcessStackPanel;
        
        public AudioDeviceControl()
        {
            this.InitializeComponent();
            viewModel=this.DataContext as AudioDeviceControlViewModel;
        }
        public AudioDeviceControl(MMDevice device)
        {
            this.InitializeComponent();
            viewModel = this.DataContext as AudioDeviceControlViewModel;
            this.device = device;
            this._ProcessStackPanel = ProcessStackPanel;
            UpdateDeviceMsg();
        }
        //private async void DeviceNameScrollViewer_PointerEntered(object sender, PointerRoutedEventArgs e)
        //{
        //    var scrollViewer = sender as ScrollViewer;
        //    if (scrollViewer == null || _isScrolling) return;

        //    _isScrolling = true; // ������ڹ���
        //    _scrollCancellationTokenSource?.Cancel(); // ȡ��֮ǰ������
        //    _scrollCancellationTokenSource = new CancellationTokenSource();
        //    var token = _scrollCancellationTokenSource.Token;

        //    double scrollWidth = DeviceNameTextBlock.ActualWidth - scrollViewer.ActualWidth;
        //    if (scrollWidth <= 0)
        //    {
        //        _isScrolling = false;
        //        return; // ����ı����С�� ScrollViewer���򲻹���
        //    }

        //    try
        //    {
        //        // ���ҹ���
        //        for (double i = 0; i <= scrollWidth; i += 1)
        //        {
        //            token.ThrowIfCancellationRequested(); // ����Ƿ�ȡ��
        //            scrollViewer.ChangeView(i, null, null);
        //            await Task.Delay(20, token); // ���ƹ����ٶ�
        //        }

        //        // ����ͣ��
        //        await Task.Delay(500, token);
        //        token.ThrowIfCancellationRequested();

        //        // ���������ȥ
        //        for (double i = scrollWidth; i >= 0; i -= 1)
        //        {
        //            token.ThrowIfCancellationRequested();
        //            scrollViewer.ChangeView(i, null, null);
        //            await Task.Delay(20, token);
        //        }
        //    }
        //    catch (TaskCanceledException)
        //    {
        //        Debug.WriteLine($"ScroolCamcell");
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        Debug.WriteLine($"ScroolCamcell");
        //    }
        //    finally
        //    {
        //        _isScrolling = false; // ���������������һ�δ���
        //    }
        //}

        private void DeviceNameScrollViewer_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _scrollCancellationTokenSource?.Cancel(); // ȡ����ǰ��������
            _isScrolling = false;
            (sender as ScrollViewer)?.ChangeView(0, null, null); // �����ص���ʼλ��
        }

        private void SessionList_Expanded(Expander sender, ExpanderExpandingEventArgs args)
        {
            ProcessStackPanel.Children.Clear();
            device.AudioSessionManager.RefreshSessions();
            var sessions = device.AudioSessionManager.Sessions;
            for (int i = 0; i < sessions.Count; i++)
            {
                if (sessions[i].IsSystemSoundsSession)
                {
                    continue;
                }
                ProcessStackPanel.Children.Add(new ProcessControl(sessions[i]));//�ع����顪���ύ��viewModel
            }
        }

        private void RightChannelSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (block)
                {
                    return;
                }
                try
                {
                    if (sender.GetType() == typeof(Slider))
                    {

                        if (device.AudioEndpointVolume.Channels.Count == 2)
                        {
                            device.AudioEndpointVolume.Channels[1].VolumeLevelScalar = (float)(e.NewValue / 100);
                            RightChannelVolumeText.Text = ((int)e.NewValue).ToString();
                        }
                    }
                }
                catch
                {
                    throw new Exception("Error: RightChannelSlider_ValueChanged");
                }
            });

        }

        private void LeftChannelSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (block)
                {
                    return;
                }
                try
                {
                    if (sender.GetType() == typeof(Slider))
                    {
                        if (device.AudioEndpointVolume.Channels.Count == 2)
                        {
                            device.AudioEndpointVolume.Channels[0].VolumeLevelScalar = (float)(e.NewValue / 100);
                            LeftChannelVolumeText.Text = ((int)e.NewValue).ToString();
                        }
                    }

                }
                catch
                {
                    throw new Exception("Error: LeftChannelSlider_ValueChanged");
                }
            });


        }

        private void MainVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (block)
                {
                    return;
                }
                try
                {
                    if (sender.GetType() == typeof(Slider))
                    {
                        device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(e.NewValue / 100);
                    }
                }
                catch
                {
                    throw new NotImplementedException("Error: MainVolumeSlider_ValueChanged");
                }
            });


        }

        public void UpdateDeviceMsg()
        {
            audioEndpointVolume = device.AudioEndpointVolume;
            viewModel.SetDeviceName(device.FriendlyName) ;
            MainVolumeSlider.Value = device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
            if (device.AudioEndpointVolume.Channels.Count == 2)
            {
                LeftChannelSlider.Value = device.AudioEndpointVolume.Channels[0].VolumeLevelScalar * 100;
                RightChannelSlider.Value = device.AudioEndpointVolume.Channels[1].VolumeLevelScalar * 100;
            }
            audioEndpointVolume.OnVolumeNotification += MasterVolumeChanged;

        }

        public void MasterVolumeChanged(AudioVolumeNotificationData data)
        {
            block = true;
            DispatcherQueue.TryEnqueue(() =>
            {
                // ��ȫ�ظ��� UI ���

                MainVolumeSlider.Value = audioEndpointVolume.MasterVolumeLevelScalar * 100;

                if (data.Channels == 2)
                {
                    LeftChannelSlider.Value = audioEndpointVolume.Channels[0].VolumeLevelScalar * 100;
                    RightChannelSlider.Value = audioEndpointVolume.Channels[1].VolumeLevelScalar * 100;
                }
            });
            block = false;

        }
    }
}
