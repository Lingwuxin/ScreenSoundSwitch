using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using NAudio.CoreAudioApi;
using ScreenSoundSwitch.WinUI.ViewModels;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Views
{
    public sealed partial class AudioDeviceControl : UserControl
    {
        private AudioDeviceControlViewModel viewModel;
        MMDevice device;
        private AudioEndpointVolume audioEndpointVolume;
        private bool suppressSliderEvents;
        public StackPanel _ProcessStackPanel;
        public string DeviceId => device?.ID;

        public AudioDeviceControl()
        {
            this.InitializeComponent();
            viewModel = this.DataContext as AudioDeviceControlViewModel;
            this.Unloaded += AudioDeviceControl_Unloaded;
        }
        public AudioDeviceControl(MMDevice device)
        {
            this.InitializeComponent();
            viewModel = this.DataContext as AudioDeviceControlViewModel;
            this.device = device;
            this._ProcessStackPanel = ProcessStackPanel;
            UpdateDeviceMsg();
            UpdateProcessSession();
            this.Unloaded += AudioDeviceControl_Unloaded;
        }

        private void AudioDeviceControl_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (audioEndpointVolume != null)
            {
                try
                {
                    audioEndpointVolume.OnVolumeNotification -= MasterVolumeChanged;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to unsubscribe from OnVolumeNotification: {ex.Message}");
                }
            }
        }

        private void SessionList_Expanded(Expander sender, ExpanderExpandingEventArgs args)
        {
            UpdateProcessSession();
        }
        public void UpdateProcessSession()
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
                ProcessStackPanel.Children.Add(new ProcessControl(sessions[i]));
            }
        }

        private void RightChannelSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (suppressSliderEvents)
            {
                return;
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    if (sender.GetType() == typeof(Slider))
                    {

                        if (device.AudioEndpointVolume.Channels.Count == 2)
                        {
                            device.AudioEndpointVolume.Channels[1].VolumeLevelScalar = (float)(e.NewValue / 100);
                            RightChannelVolumeText.Text = ((int)e.NewValue).ToString();

                            var maxValue = Math.Max(LeftChannelSlider.Value, e.NewValue);
                            device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(maxValue / 100);
                            SetSliderValueSilently(MainVolumeSlider, maxValue);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in RightChannelSlider_ValueChanged: {ex.Message}");
                }
            });

        }

        public void UpdateChannelSlidersFromDevice()
        {
            if (device == null || device.AudioEndpointVolume == null)
            {
                return;
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                var masterValue = device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
                SetSliderValueSilently(MainVolumeSlider, masterValue);

                if (device.AudioEndpointVolume.Channels.Count >= 2)
                {
                    var leftValue = device.AudioEndpointVolume.Channels[0].VolumeLevelScalar * 100;
                    var rightValue = device.AudioEndpointVolume.Channels[1].VolumeLevelScalar * 100;
                    SetSliderValueSilently(LeftChannelSlider, leftValue);
                    SetSliderValueSilently(RightChannelSlider, rightValue);
                    LeftChannelVolumeText.Text = ((int)leftValue).ToString();
                    RightChannelVolumeText.Text = ((int)rightValue).ToString();
                }
            });
        }

        private void LeftChannelSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (suppressSliderEvents)
            {
                return;
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    if (sender.GetType() == typeof(Slider))
                    {
                        if (device.AudioEndpointVolume.Channels.Count == 2)
                        {
                            device.AudioEndpointVolume.Channels[0].VolumeLevelScalar = (float)(e.NewValue / 100);
                            LeftChannelVolumeText.Text = ((int)e.NewValue).ToString();

                            var maxValue = Math.Max(e.NewValue, RightChannelSlider.Value);
                            device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(maxValue / 100);
                            SetSliderValueSilently(MainVolumeSlider, maxValue);
                        }
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in LeftChannelSlider_ValueChanged: {ex.Message}");
                }
            });


        }

        private void MainVolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (suppressSliderEvents)
            {
                return;
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    if (sender.GetType() == typeof(Slider))
                    {
                        if (device.AudioEndpointVolume.Channels.Count == 2)
                        {
                            var currentLeft = LeftChannelSlider.Value;
                            var currentRight = RightChannelSlider.Value;
                            var currentMaster = Math.Max(currentLeft, currentRight);
                            var delta = e.NewValue - currentMaster;

                            var newLeft = ClampVolume(currentLeft + delta);
                            var newRight = ClampVolume(currentRight + delta);
                            var newMaster = Math.Max(newLeft, newRight);

                            device.AudioEndpointVolume.Channels[0].VolumeLevelScalar = (float)(newLeft / 100);
                            device.AudioEndpointVolume.Channels[1].VolumeLevelScalar = (float)(newRight / 100);
                            device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(newMaster / 100);

                            SetSliderValueSilently(LeftChannelSlider, newLeft);
                            SetSliderValueSilently(RightChannelSlider, newRight);
                            SetSliderValueSilently(MainVolumeSlider, newMaster);
                            LeftChannelVolumeText.Text = ((int)newLeft).ToString();
                            RightChannelVolumeText.Text = ((int)newRight).ToString();
                        }
                        else
                        {
                            var scalar = (float)(e.NewValue / 100);
                            device.AudioEndpointVolume.MasterVolumeLevelScalar = scalar;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in MainVolumeSlider_ValueChanged: {ex.Message}");
                }
            });


        }

        public void UpdateDeviceMsg()
        {
            audioEndpointVolume = device.AudioEndpointVolume;
            viewModel.SetDeviceName(device.FriendlyName);

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["EnableScreenPositionChannelBalance"] is bool enabled &&
                enabled &&
                device.AudioEndpointVolume.Channels.Count == 2)
            {
                device.AudioEndpointVolume.Channels[0].VolumeLevelScalar = 0.5f;
                device.AudioEndpointVolume.Channels[1].VolumeLevelScalar = 0.5f;
                device.AudioEndpointVolume.MasterVolumeLevelScalar = 0.5f;
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                SetSliderValueSilently(MainVolumeSlider, device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
                if (device.AudioEndpointVolume.Channels.Count == 2)
                {
                    var leftValue = device.AudioEndpointVolume.Channels[0].VolumeLevelScalar * 100;
                    var rightValue = device.AudioEndpointVolume.Channels[1].VolumeLevelScalar * 100;
                    SetSliderValueSilently(LeftChannelSlider, leftValue);
                    SetSliderValueSilently(RightChannelSlider, rightValue);
                    LeftChannelVolumeText.Text = ((int)leftValue).ToString();
                    RightChannelVolumeText.Text = ((int)rightValue).ToString();
                }
            });
            audioEndpointVolume.OnVolumeNotification += MasterVolumeChanged;

        }

        public void MasterVolumeChanged(AudioVolumeNotificationData data)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                // 安全地更新 UI 组件
                if (data.EventContext != Guid.Empty)
                {
                    //由自身引起的音量变化不做响应
                    SetSliderValueSilently(MainVolumeSlider, audioEndpointVolume.MasterVolumeLevelScalar * 100);

                    if (data.Channels == 2)
                    {
                        var leftValue = audioEndpointVolume.Channels[0].VolumeLevelScalar * 100;
                        var rightValue = audioEndpointVolume.Channels[1].VolumeLevelScalar * 100;
                        SetSliderValueSilently(LeftChannelSlider, leftValue);
                        SetSliderValueSilently(RightChannelSlider, rightValue);
                        LeftChannelVolumeText.Text = ((int)leftValue).ToString();
                        RightChannelVolumeText.Text = ((int)rightValue).ToString();
                    }
                }
            });

        }

        private void SetSliderValueSilently(Slider slider, double value)
        {
            suppressSliderEvents = true;
            slider.Value = value;
            suppressSliderEvents = false;
        }

        private static double ClampVolume(double value)
        {
            if (value < 0)
            {
                return 0;
            }

            if (value > 100)
            {
                return 100;
            }

            return value;
        }
    }
}
