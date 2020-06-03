using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace Zekri_Tool.Components
{
    /// <summary>
    /// Interaction logic for CustomButton.xaml
    /// </summary>
    public partial class CustomButton : UserControl
    {
        public int TextFontSize { get; set; }
        public string Text { get; set; }
        public BitmapImage Icon { get; set; }
        public int IconSize { get; set; }
        public LinearGradientBrush BackColor { get; set; }

        public CustomButton()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public void UpdateText(string text)
        {
            this.lbl_text.Content = text;
        }

        public void UpdateIcon(BitmapImage image)
        {
            this.img_icon.Source = image;
        }

        public void UpdateBackColor(System.Windows.Media.Brush color)
        {
            this.pnl_container.Background = color;
        }

        public void SetEnabled(bool enabled)
        {
            this.IsEnabled = enabled;
            this.pnl_container.Background = enabled ? (System.Windows.Media.Brush)BackColor : ((System.Windows.Media.Brush)TryFindResource("clr_disabled"));
        }
    }
}
