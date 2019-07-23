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
        public WarningPopup(string Text, string TopLabel)
        {
            InitializeComponent();
            WarningTextBlock.Text = Text;
            WarningTopLabel.Content = TopLabel;
        }

        public bool SelectionMade = false;
        public bool YesBool = false;

        private async void WarningNoButton_Click(object sender, RoutedEventArgs e)
        {
            await FadeOut(this);
            YesBool = false;
            SelectionMade = true;
        }

        private async void WarningYesButton_Click(object sender, RoutedEventArgs e)
        {
            await FadeOut(this);
            YesBool = true;
            SelectionMade = true;
        }

        async Task FadeIn(UIElement FadeElement)
        {
            for (int i = 0; i < 100; i += 5)
            {
                FadeElement.Opacity = ((double)i / (double)100);
                await Task.Delay(10);
            }
        }

        public async Task FadeOut(UIElement FadeElement)
        {
            for (int i = 100; i >= 0; i -= 5)
            {
                FadeElement.Opacity = ((double)i / (double)100);
                await Task.Delay(10);
            }
        }

        private async void Grid_Initialized(object sender, EventArgs e)
        {
            await FadeIn(this);
        }
    }
}
