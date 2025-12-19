using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendGitMsg.Model
{
    internal class CommitRecord
    {
        public string Date { get; set; } = string.Empty;       // 커밋 날짜
        public int CommitCnt { get; set; } = 0;                // 커밋 횟수
        public string SmsStts { get; set; } = string.Empty;    // SMS 전송 결과 (성공/실패/미전송)
        public string SlackStts { get; set; } = string.Empty;  // Slack 전송 결과 (성공/실패/미전송)
    }
}
