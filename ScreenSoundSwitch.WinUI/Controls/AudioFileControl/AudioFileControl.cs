using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScreenSoundSwitch.WinUI.Controls
{
    public sealed class AudioFileControl : Control
    {
        public AudioFileControl()
        {
            this.DefaultStyleKey = typeof(AudioFileControl);
        }
        //添加标题属性
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(AudioFileControl), new PropertyMetadata(string.Empty));

        //添加音频文件属性
        public StorageFile AudioFile
        {
            get { return (StorageFile)GetValue(AudioFileProperty); }
            set { SetValue(AudioFileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AudioFile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AudioFileProperty =
            DependencyProperty.Register("AudioFile", typeof(StorageFile), typeof(AudioFileControl), new PropertyMetadata(string.Empty));

    }
}
