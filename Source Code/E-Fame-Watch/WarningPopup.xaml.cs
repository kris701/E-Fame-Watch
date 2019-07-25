using System;
using System.Collections.Generic;
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

namespace E_Fame_Watch
{
    /// <summary>
    /// Interaction logic for WarningPopup.xaml
    /// </summary>
    public partial class WarningPopup : UserControl
    {
        MainWindow SenderWindow;

        public WarningPopup(MainWindow _SenderWindow, string Text, string TopLabel)
        {
            SenderWindow = _SenderWindow;
            InitializeComponent();
            WarningTextBlock.Text = Text;
            WarningTopLabel.Content = TopLabel;
            this.VerticalAlignment = VerticalAlignment.Top;
            this.Height = SenderWindow.Clip.Bounds.Height;
            this.Opacity = 0;
        }

        public bool SelectionMade = false;
        public bool YesBool = false;
        private bool RetractAgain = false;

        private async void WarningNoButton_Click(object sender, RoutedEventArgs e)
        {
            await SenderWindow.FadeOut(this);
            if (RetractAgain)
                await SenderWindow.ExpandWindow(false);
            YesBool = false;
            SelectionMade = true;
        }

        private async void WarningYesButton_Click(object sender, RoutedEventArgs e)
        {
            await SenderWindow.FadeOut(this);
            if (RetractAgain)
                await SenderWindow.ExpandWindow(false);
            YesBool = true;
            SelectionMade = true;
        }

        private async void Grid_Initialized(object sender, EventArgs e)
        {
            if (SenderWindow.Clip.Bounds.Width == 260)
                RetractAgain = true;
            await SenderWindow.ExpandWindow(true);
            await SenderWindow.FadeIn(this);
        }
    }
}
