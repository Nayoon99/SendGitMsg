using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using SendGitMsg.Model;

namespace SendGitMsg.sqlite
{
    // 초기 테이블 생성용
    internal class DatabaseInitializer
    {
        private static readonly string connStr = "Data Source=commit.db";

        public static void Init()
        {
            using var conn = new SqliteConnection(connStr);
            conn.Open();

            string sql = @"
            CREATE TABLE IF NOT EXISTS commit_log (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                run_date TEXT,
                commit_cnt INTEGER,
                sms_status TEXT,
                slack_status TEXT,
                created_at TEXT
            );";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }
    }


    // 당일 커밋 체크 여부 확인용
    internal class CommitLog
    {
        private static readonly string connStr = "Data Source=commit.db";

        public static bool IsTodayAlreadyExecuted()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            using var conn = new SqliteConnection(connStr);
            conn.Open();

            string sql = @"
                SELECT COUNT(1)
                FROM commit_log
                WHERE run_date = @date;
            ";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@date", today);

            object? result = cmd.ExecuteScalar();
            long count = result == null ? 0 : (long)result;

            return count > 0;
        }


        // 커밋 체크 시 로그 삽입용
        public static void InsertCommitLog(string runDate, int commitCnt, string smsStatus, string slackStatus)
        {
            using var conn = new SqliteConnection(connStr);
            conn.Open();

            string sql = @"
                        INSERT INTO commit_log (run_date, commit_cnt, sms_status, slack_status, created_at)
                        VALUES (@runDate, @commitCnt, @smsStatus, @slackStatus, @createdAt);";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@runDate", runDate);
            cmd.Parameters.AddWithValue("@commitCnt", commitCnt);
            cmd.Parameters.AddWithValue("@smsStatus", smsStatus);
            cmd.Parameters.AddWithValue("@slackStatus", slackStatus);
            cmd.Parameters.AddWithValue("@createdAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
        }


        // 전체 이력 조회용
        public static List<CommitRecord> GetAllLogs()
        {
            var list = new List<CommitRecord>();

            using var conn = new SqliteConnection(connStr);
            conn.Open();

            string sql = @"SELECT run_date, commit_cnt, sms_status, slack_status
                            FROM commit_log
                            ORDER BY run_date DESC;";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new CommitRecord
                {
                    Date = reader["run_date"]?.ToString() ?? "",
                    CommitCnt = Convert.ToInt32(reader["commit_cnt"]),
                    SmsStts = reader["sms_status"]?.ToString() ?? "",
                    SlackStts = reader["slack_status"]?.ToString() ?? ""
                });
            }

            return list;
        }

    }
}
