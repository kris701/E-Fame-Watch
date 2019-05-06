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
    public partial class ItemDesign : UserControl
    {
        public ItemDesign()
        {
            InitializeComponent();
        }

        private void ItemRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button SenderButton = sender as Button;
            Grid SenderGrid = SenderButton.Parent as Grid;
            ItemDesign SenderDesign = SenderGrid.Parent as ItemDesign;
            StackPanel SenderPanel = SenderDesign.Parent as StackPanel;
            SenderPanel.Children.Remove(SenderDesign);
            SetWindowHeight(SenderPanel);
        }

        private async void ItemMinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Button SenderButton = sender as Button;
            Grid SenderGrid = SenderButton.Parent as Grid;
            ItemDesign SenderDesign = SenderGrid.Parent as ItemDesign;
            StackPanel SenderPanel = SenderDesign.Parent as StackPanel;
            if (SenderGrid.Height != 30)
            {
                for (int i = 160; i >= 30; i -= 20)
                {
                    SenderGrid.Height = i;
                    SetWindowHeight(SenderPanel);
                    await Task.Delay(1);
                }
                SenderButton.Content = "^";
                SenderGrid.Height = 30;
            }
            else
            {
                for (int i = 30; i < 160; i += 20)
                {
                    SenderGrid.Height = i;
                    SetWindowHeight(SenderPanel);
                    await Task.Delay(1);
                }
                SenderButton.Content = "V";
                SenderGrid.Height = 160;
            }
            SetWindowHeight(SenderPanel);
        }

        void SetWindowHeight(StackPanel SenderStack)
        {
            ScrollViewer SenderScrollViewer = SenderStack.Parent as ScrollViewer;
            int AddHeight = 260;
            for (int i = 0; i < SenderStack.Children.Count; i++)
            {
                ItemDesign SenderDesign = SenderStack.Children[i] as ItemDesign;
                AddHeight += (int)(SenderDesign.MainItemGrid.Height + SenderDesign.MainItemGrid.Margin.Top + SenderDesign.MainItemGrid.Margin.Bottom);
            }
            if (AddHeight > 1000)
            {
                (((SenderScrollViewer.Parent as Grid).Parent as Border).Parent as MainWindow).Height = 1000;
                SenderScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            }
            else
            {
                (((SenderScrollViewer.Parent as Grid).Parent as Border).Parent as MainWindow).Height = AddHeight;
                SenderScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }

        private void MainItemGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Type brushesType = typeof(Brushes);
            var properties = brushesType.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var prop in properties)
            {
                ItemBorderColorComboBox.Items.Add(prop.Name);
                ItemFillColorComboBox.Items.Add(prop.Name);
            }
        }
    }
}
