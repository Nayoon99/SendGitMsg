using System.Collections.ObjectModel;
using MvvmHelpers;
using MvvmHelpers.Commands;
using SendGitMsg.Model;
using System.ComponentModel;
using System.Windows.Threading;
using System.Diagnostics;
using System.Text;
using SendGitMsg.sqlite;

namespace SendGitMsg.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        // DataGrid와 Binding 될 ObservableCollection
        public ObservableCollection<CommitRecord> Records { get; set; } = new();

        // 버튼 클릭용 커맨드
        public Command CheckCommitCommand { get; set; }

        private DispatcherTimer _timer;            // 타이머 필드 추가
        private DateTime? _lastExecutedDate = null; // 마지막 실행 날짜

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
        private void CheckCommits()
        {
            // 오늘 이미 실행했으면 중단
            if (CommitLog.IsTodayAlreadyExecuted())
            {
                StatusMsg = "오늘은 이미 커밋 체크를 완료했습니다.";
                CanCheckCommit = false;
                return;
            }

            int commitCount = GetYesterdayCommitCount();
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string smsStatus = commitCount > 0 ? "전송 예정" : "어제 커밋이 없습니다!";
            string slackStatus = "대기";

            // DB INSERT
            CommitLog.InsertCommitLog(
                today,
                commitCount,
                smsStatus,
                slackStatus);

            // 화면 표시
            Records.Add(new CommitRecord
            {
                Date = today,
                CommitCnt = commitCount,
                SmsStts = smsStatus,
                SlackStts = slackStatus
            });

            StatusMsg = "커밋 체크가 완료되었습니다.";
            CanCheckCommit = false;
        }




        private void Timer_Tick(object? sender, EventArgs e)
        {
            var now = DateTime.Now;

            // 매일 00:01에 실행
            if (now.Hour == 0 && now.Minute == 1)
            {
                // 00:01에만 실행되도록 마지막 실행 날짜 확인
                if (now.Hour == 0 && now.Minute == 1)
                {
                    // 오늘 이미 실행했는지 체크
                    if (_lastExecutedDate?.Date == now.Date) return;

                    // 실행
                    CheckCommits();

                    // 실행 날짜 저장
                    _lastExecutedDate = now;
                }
                CheckCommits();
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


        private string _statusMsg;
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
