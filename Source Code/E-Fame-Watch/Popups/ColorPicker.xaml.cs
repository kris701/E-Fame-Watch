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
        MainWindow SenderWindow;

        public ColorPicker(MainWindow _SenderWindow)
        {
            SenderWindow = _SenderWindow;
            InitializeComponent();
            this.VerticalAlignment = VerticalAlignment.Top;
            this.Height = SenderWindow.Clip.Bounds.Height;
        }

        public bool SelectionMade = false;
        public Brush SelectedColor;

        private async void Grid_Initialized(object sender, EventArgs e)
        {
            int Count = 0;
            Type brushesType = typeof(Brushes);
            var properties = brushesType.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            for (int i = 0; i < ColorGrid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < ColorGrid.ColumnDefinitions.Count; j++)
                {
                    Button NewColorButton = new Button();
                    NewColorButton.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(properties[Count].Name);
                    NewColorButton.Click += ColorClick_Click;
                    Grid.SetRow(NewColorButton, i);
                    Grid.SetColumn(NewColorButton, j);
                    ColorGrid.Children.Add(NewColorButton);
                    Count++;
                    if (Count >= properties.Length)
                    {
                        i = ColorGrid.RowDefinitions.Count;
                        break;
                    }
                }
            }

            await GI.FadeIn(this);
        }

        private async void ColorClick_Click(object sender, RoutedEventArgs e)
        {
            await GI.FadeOut(this);
            SelectedColor = (sender as Button).Background;
            SelectionMade = true;
        }

        private async void ColorPickerCancel_Click(object sender, RoutedEventArgs e)
        {
            await GI.FadeOut(this);
            SelectedColor = null;
            SelectionMade = true;
        }
    }
}
