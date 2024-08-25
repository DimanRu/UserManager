using System.Configuration;
using System.Data;
using System.Windows;
using UserManager.ViewModel;
using UserManager.Data;

namespace UserManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ApplicationContext DbContext;
        private void App_Startup(object sender, StartupEventArgs e)
        {
            DbContext = new ApplicationContext();
            var mainWindowVM = new MainWindowViewModel(DbContext);
            MainWindow = new MainWindow(mainWindowVM); ;
            MainWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            DbContext.Dispose();
        }
    }
}
