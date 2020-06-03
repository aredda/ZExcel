using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Zekri_Tool.Models;
using Zekri_Tool.Windows;

namespace Zekri_Tool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static List<Window> windows;
        public static Settings appSettings;

        public static void RunWindow(Window window)
        {
            // initialize window list
            if (windows == null) windows = new List<Window>();
            // hide opened window
            if (windows.Count > 0) windows.Last().Hide();
            // configure window
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Closing += (object s, CancelEventArgs args) => { CloseWindow(); };
            // show new window
            window.Show();
            // save window
            windows.Add(window);
        }

        public static void CloseWindow()
        {
            // dispose of the last window
            windows.Remove(windows.Last());
            // show previous window if there's any
            if (windows.Count > 0)
            {
                windows.Last().Show();

                if (windows.Last() is MainWindow)
                    ((MainWindow)windows.Last()).Refresh();
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // load app settings
            appSettings = new Settings();
            appSettings.Load();
            // run the main window
            RunWindow(new MainWindow());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            appSettings.Save();
        }
    }
}
