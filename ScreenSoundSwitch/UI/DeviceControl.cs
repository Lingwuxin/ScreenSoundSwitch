namespace ScreenSoundSwitch.UI
{
    public partial class DeviceControl : UserControl
    {
        public DeviceControl()
        {
            InitializeComponent();
        }
        //根据传入的键值对，设置ListBox的内容
        public void updateList(string screenName,string audioName)
        {
            //遍历screenList的每一个元素，如果找到了键值对中的键，则将其值更新为新的值，否则添加新的键值对
            for (int i = 0; i < screenList.Items.Count; i++)
            {
                if (screenList.Items[i].ToString().Contains(screenName))
                {
                    screenList.Items[i] = screenName;
                    audioDeviceList.Items[i]=audioName;
                    return;
                }
            }
            screenList.Items.Add(screenName);
            audioDeviceList.Items.Add(audioName);
        }

    }

}
