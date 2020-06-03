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
using Zekri_Tool.Models;
using Zekri_Tool.Windows;

namespace Zekri_Tool.Components
{
    /// <summary>
    /// Interaction logic for LogListItem.xaml
    /// </summary>
    public partial class LogListItem : UserControl
    {
        public Log Log { get; set; }
        public SolidColorBrush HoverBackColor { get; set; }
        private Action Refresh { get; set; }

        public LogListItem()
        {
            InitializeComponent();
        }

        public LogListItem(Log log) : this ()
        {
            this.Log = log;

            this.lbl_name.Content = Log.FileName;
            this.lbl_path.Content = Log.FullPath;
            this.lbl_time.Content = Log.Time.ToString();

            HoverBackColor = TryFindResource("clr_light") as SolidColorBrush;

            // hover effect
            this.MouseEnter += (object s, MouseEventArgs args) => { li_container.Background = HoverBackColor; };
            this.MouseLeave += (object s, MouseEventArgs args) => { li_container.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)); };

            // click
            this.MouseDown += Click;
        }
        public LogListItem(Log log, Action refreshAction)
            : this (log)
        {
            this.Refresh = refreshAction;
        }

        public void Click(object sender, MouseButtonEventArgs args)
        {
            try
            {
                if (!File.Exists(Log.FullPath))
                    throw new Exception("It seems that the file couldn't be found, possibly because it was deleted or moved.");

                App.RunWindow(new ManageWindow(Log.FullPath));
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Opening File Has Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                
                // remove this log item
                App.appSettings.Logs.Remove(Log);
                // clean up
                if (this.Refresh != null) this.Refresh.Invoke();
            }
        }
    }
}
