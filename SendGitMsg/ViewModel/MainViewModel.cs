using System.Collections.ObjectModel;
using MvvmHelpers;
using MvvmHelpers.Commands;
using SendGitMsg.Model;
using System.ComponentModel;
using System.Windows.Threading;
using System.Diagnostics;
using System.Text;

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
            // 버튼 커맨드
            CheckCommitCommand = new Command(CheckCommits);

            // 타이머 생성
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(30); // 30초마다 체크
            _timer.Tick += Timer_Tick;
            _timer.Start();                             // 타이머 시작
        }


        // 커밋 체크 + 메시지 전송 로직
        private void CheckCommits()
        {
            int commitCount = GetTodayCommitCount();

            Records.Add(new CommitRecord
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                CommitCnt = commitCount,
                SmsStts = "대기",
                SlackStts = "대기"
            });
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


        private int GetTodayCommitCount()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "log --since=midnight --oneline",
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



    }
}
