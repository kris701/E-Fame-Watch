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
    /// Interaction logic for GraphLoadingControl.xaml
    /// </summary>
    public partial class GraphLoadingControl : UserControl
    {
        public GraphLoadingControl()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingRectangle.Width = this.ActualWidth - 10;
            while (true)
            {
                for (int i = (int)LoadingRectangle.Height; i < this.ActualHeight - 10; i += 10)
                {
                    LoadingRectangle.Height = i;
                    await Task.Delay(10);
                }
                for (int i = (int)LoadingRectangle.Width; i > 10; i -= 10)
                {
                    LoadingRectangle.Width = i;
                    await Task.Delay(10);
                }
                for (int i = (int)LoadingRectangle.Height; i > 10; i -= 10)
                {
                    LoadingRectangle.Height = i;
                    await Task.Delay(10);
                }
                for (int i = (int)LoadingRectangle.Width; i < this.ActualWidth - 10; i += 10)
                {
                    LoadingRectangle.Width = i;
                    await Task.Delay(10);
                }
            }
        }
    }
}
