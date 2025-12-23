using System.Collections.ObjectModel;
using MvvmHelpers;
using MvvmHelpers.Commands;
using SendGitMsg.Model;
using System.ComponentModel;
using System.Windows.Threading;
using System.Diagnostics;
using System.Text;
using SendGitMsg.sqlite;
using SendGitMsg.service;

namespace SendGitMsg.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        // DataGrid와 Binding 될 ObservableCollection
        public ObservableCollection<CommitRecord> Records { get; set; } = [];

        // 버튼 클릭용 커맨드
        public Command CheckCommitCommand { get; set; }

        private DispatcherTimer _timer;            // 타이머 필드 추가
        private DateTime? _lastExecutedDate = null; // 마지막 실행 날짜

        private readonly SmsService _smsService;

        // 생성자
        public MainViewModel()
        {
            StatusMsg = "ViewModel 로딩됨";

            // 버튼 커맨드
            CheckCommitCommand = new Command(CheckCommits);

            LoadCommitLogs();

            if (CommitLog.IsTodayAlreadyExecuted())
            {
                StatusMsg = "오늘은 이미 커밋 체크를 완료했습니다.";
                CanCheckCommit = false;
            }
            else
            {
                StatusMsg = "아직 커밋 체크를 하지 않았습니다.";
                CanCheckCommit = true;
            }

            // 타이머 생성
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(30); // 30초마다 체크
            _timer.Tick += Timer_Tick;
            _timer.Start();                             // 타이머 시작
        }


        // 커밋 체크 + 메시지 전송 로직 메서드
        private async void CheckCommits()
        {
            if (CommitLog.IsTodayAlreadyExecuted())
            {
                StatusMsg = "오늘은 이미 커밋 체크를 완료했습니다.";
                CanCheckCommit = false;
                return;
            }

            int commitCount = GetYesterdayCommitCount();
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            var record = new CommitRecord
            {
                Date = today,
                CommitCnt = commitCount,
                SmsStts = "미전송",
                SlackStts = "대기"
            };

            Records.Add(record);

            string smsStatus = "미전송";

            if (commitCount == 0)
            {
                string message =
                    $"{DateTime.Now:MM월 dd일} 커밋 횟수 0번.. ㅠ.ㅠ 벌금을 송금합니다.";

                bool sent = await _smsService.SendSmsAsync(message);
                smsStatus = sent ? "전송 완료" : "전송 실패";

                record.SmsStts = smsStatus;
            }


            CommitLog.InsertCommitLog(today, commitCount, smsStatus, "대기");

            StatusMsg = "커밋 체크 완료";
            CanCheckCommit = false;
        }




        private void Timer_Tick(object? sender, EventArgs e)
        {
            var now = DateTime.Now;

            // 매일 00:01에만 실행
            if (now.Hour == 0 && now.Minute == 1 && _lastExecutedDate?.Date != now.Date)
            {
                CheckCommits();
                _lastExecutedDate = now;
            }
        }



        // 전날 커밋 개수 카운트 메서드
        private int GetYesterdayCommitCount()
        {
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);

            var startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"log --since=\"{yesterday:yyyy-MM-dd} 00:00\" --until=\"{today:yyyy-MM-dd} 00:00\" --oneline",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (string.IsNullOrWhiteSpace(output))
                return 0;

            // 줄 수 = 커밋 수
            return output.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
        }


        private string _statusMsg = "";
        public string StatusMsg
        {
            get => _statusMsg;
            set => SetProperty(ref _statusMsg, value);
        }

        private bool _canCheckCommit = true;
        public bool CanCheckCommit
        {
            get => _canCheckCommit;
            set => SetProperty(ref _canCheckCommit, value);
        }



        private void LoadCommitLogs()
        {
            Records.Clear();

            var logs = CommitLog.GetAllLogs();

            foreach (var log in logs)
            {
                Records.Add(log);
            }
        }

    }
}
