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
                for (int i = 150; i >= 30; i -= 10)
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
                for (int i = 30; i < 150; i += 10)
                {
                    SenderGrid.Height = i;
                    SetWindowHeight(SenderPanel);
                    await Task.Delay(1);
                }
                SenderButton.Content = "V";
                SenderGrid.Height = 150;
            }
            SetWindowHeight(SenderPanel);
        }

        private void ItemMoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            Button SenderButton = sender as Button;
            Grid SenderGrid = SenderButton.Parent as Grid;
            ItemDesign SenderDesign = SenderGrid.Parent as ItemDesign;
            StackPanel SenderPanel = SenderDesign.Parent as StackPanel;
            if (SenderPanel.Children.IndexOf(SenderDesign) >= 1)
            {
                int Index = SenderPanel.Children.IndexOf(SenderDesign) - 1;
                SenderPanel.Children.Remove(SenderDesign);
                SenderPanel.Children.Insert(Index, SenderDesign);
                SetWindowHeight(SenderPanel);
            }
        }

        private void ItemMoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            Button SenderButton = sender as Button;
            Grid SenderGrid = SenderButton.Parent as Grid;
            ItemDesign SenderDesign = SenderGrid.Parent as ItemDesign;
            StackPanel SenderPanel = SenderDesign.Parent as StackPanel;
            if (SenderPanel.Children.IndexOf(SenderDesign) < SenderPanel.Children.Count - 1)
            {
                int Index = SenderPanel.Children.IndexOf(SenderDesign) + 1;
                SenderPanel.Children.Remove(SenderDesign);
                SenderPanel.Children.Insert(Index, SenderDesign);
                SetWindowHeight(SenderPanel);
            }
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

        private async void ItemBorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            bool Expand = false;
            if ((((((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Border).Parent as MainWindow).Width == 260)
            {
                Expand = true;
            }

            if (Expand)
            {
                for (int i = 260; i < 520; i += 20)
                {
                    (((((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Border).Parent as MainWindow).Width = i;
                    await Task.Delay(1);
                }
                (((((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Border).Parent as MainWindow).Width = 520;
            }

            ColorPicker PickColor = new ColorPicker();
            (((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Children.Add(PickColor);
            while (true)
            {
                if (PickColor.SelectionMade)
                {
                    ItemBorderColorButton.Background = PickColor.SelectedColor;
                    (((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Children.Remove(PickColor);
                    break;
                }
                await Task.Delay(100);
            }

            if (Expand)
            {
                for (int i = 520; i >= 260; i -= 20)
                {
                    (((((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Border).Parent as MainWindow).Width = i;
                    await Task.Delay(1);
                }
                (((((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Border).Parent as MainWindow).Width = 260;
            }
        }

        private async void ItemFillColorButton_Click(object sender, RoutedEventArgs e)
        {
            bool Expand = false;
            if ((((((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Border).Parent as MainWindow).Width == 260)
            {
                Expand = true;
            }

            if (Expand)
            {
                for (int i = 260; i < 520; i += 20)
                {
                    (((((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Border).Parent as MainWindow).Width = i;
                    await Task.Delay(1);
                }
                (((((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Border).Parent as MainWindow).Width = 520;
            }

            ColorPicker PickColor = new ColorPicker();
            (((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Children.Add(PickColor);
            while (true)
            {
                if (PickColor.SelectionMade)
                {
                    ItemFillColorButton.Background = PickColor.SelectedColor;
                    (((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Children.Remove(PickColor);
                    break;
                }
                await Task.Delay(100);
            }

            if (Expand)
            {
                for (int i = 520; i >= 260; i -= 20)
                {
                    (((((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Border).Parent as MainWindow).Width = i;
                    await Task.Delay(1);
                }
                (((((this.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Border).Parent as MainWindow).Width = 260;
            }
        }
    }
}
