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
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        public ColorPicker()
        {
            InitializeComponent();
        }

        public bool SelectionMade = false;
        public Brush SelectedColor;

        private async void Grid_Initialized(object sender, EventArgs e)
        {
            int Count = 0;
            Type brushesType = typeof(Brushes);
            var properties = brushesType.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            for (int i = 0; i < ColorGrid.ColumnDefinitions.Count; i++)
            {
                for (int j = 0; j < ColorGrid.RowDefinitions.Count; j++)
                {
                    Button NewColorButton = new Button();
                    NewColorButton.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(properties[Count].Name);
                    NewColorButton.Click += ColorClick_Click;
                    Grid.SetRow(NewColorButton, j);
                    Grid.SetColumn(NewColorButton, i);
                    ColorGrid.Children.Add(NewColorButton);
                    Count++;
                    if (Count >= properties.Length)
                    {
                        i = ColorGrid.ColumnDefinitions.Count;
                        break;
                    }
                }
            }
            await FadeIn(this);
        }

        private async void ColorClick_Click(object sender, RoutedEventArgs e)
        {
            await FadeOut(this);
            SelectedColor = (sender as Button).Background;
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
    }
}
