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
        public GraphLoadingControl(double _Width, double _Height)
        {
            Width = _Width;
            Height = _Height;
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingRectangle.Width = this.ActualHeight - 10;
            while (true)
            {
                for (int i = 0; i < 180; i += 10)
                {
                    LoadingRectangle.RenderTransform = new RotateTransform(i);
                    await Task.Delay(2);
                }
                LoadingRectangle.RenderTransform = new RotateTransform(0);
            }
        }
    }
}
