using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendGitMsg.model
{
    internal class Config
    {
        public string CoolSmsApiKey { get; set; } = "";
        public string CoolSmsApiSecret { get; set; } = "";
        public string FromPhone { get; set; } = "";
        public string ToPhone { get; set; } = "";
    }
}
