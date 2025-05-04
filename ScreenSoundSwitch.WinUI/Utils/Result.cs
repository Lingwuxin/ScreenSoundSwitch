using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSoundSwitch.WinUI.Utils
{

    //用于返回http请求的结果
    public class Result(bool status, string message, object data)
    {
        public bool Status { get; set; } = status;
        public string Message { get; set; } = message;
        public object Data { get; set; } = data;
    }
}
