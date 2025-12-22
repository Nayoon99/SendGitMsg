using System.Configuration;
using System.Data;
using System.Windows;
using SendGitMsg.sqlite;

namespace SendGitMsg
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DatabaseInitializer.Init();
        }
    }

}
