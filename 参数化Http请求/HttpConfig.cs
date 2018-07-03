using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 参数化Http请求
{
    public class HttpConfig
    {
        public bool Check { get; internal set; }
        public string RegStr { get; internal set; }
        public bool TimerEnable { get; internal set; }
        public int ThreadNum { get; internal set; }
        public bool ProxyEnable { get; internal set; }
        public string ProxyAPI { get; internal set; }
    }
}
