using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Zekri_Tool.Components;
using Zekri_Tool.Models;
using Zekri_Tool.Windows;

namespace Zekri_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // configure button events
            btn_open.MouseDown += (object s, MouseButtonEventArgs args) => {
                // browse for the file he wants to open
                OpenFileDialog fd = new OpenFileDialog();
                fd.Filter = "Excel Files (*.xlsx) | *.xlsx";
                fd.ShowDialog();
                // check if there's a selected file
                if (File.Exists(fd.FileName))
                {
                    // save a log
                    App.appSettings.Logs.Add(new Log
                    {
                        FullPath = fd.FileName,
                        Time = DateTime.Now
                    });
                    // refresh recent files
                    Refresh();
                    // run window
                    App.RunWindow(new ManageWindow(fd.FileName));
                }
                else
                    MessageBox.Show("Please, select a valid excel file!", "Loading failed", MessageBoxButton.OK, MessageBoxImage.Error);
            };
            btn_create.MouseDown += (object s, MouseButtonEventArgs args) => {
                App.RunWindow(new ManageWindow());
            };

            // refresh the list of recent opened files
            Refresh();
        }

        public void FillLogs(List<Log> logs)
        {
            stpnl_log.Children.Clear();

            sv_container.Visibility = logs.Count == 0 ? Visibility.Hidden : Visibility.Visible;
            lbl_header.Content = logs.Count == 0 ? "Sorry, it seems that there are no recent opened files." : "Recent Opened Files:";

            foreach (Log log in logs)
                stpnl_log.Children.Add(new LogListItem(log, Refresh));
        }

        public void Refresh()
        {
            FillLogs(App.appSettings.Logs.OrderByDescending(l => l.Time).ToList());
        }
    }
}
