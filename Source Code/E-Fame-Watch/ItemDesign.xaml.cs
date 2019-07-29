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
        MainWindow SenderWindow;

        public ItemDesign(MainWindow _SenderWindow)
        {
            SenderWindow = _SenderWindow;
            InitializeComponent();
        }

        private async void ItemRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            WarningPopup inputDialog = new WarningPopup(SenderWindow, "Are you sure you want to remove this item?", "Warning");
            SenderWindow.MainGrid.Children.Add(inputDialog);
            while (true)
            {
                if (inputDialog.SelectionMade)
                {
                    if (inputDialog.YesBool)
                    {
                        Button SenderButton = sender as Button;
                        Grid SenderGrid = SenderButton.Parent as Grid;
                        ItemDesign SenderDesign = SenderGrid.Parent as ItemDesign;
                        StackPanel SenderPanel = SenderDesign.Parent as StackPanel;
                        SenderPanel.Children.Remove(SenderDesign);
                        SetWindowHeight(SenderPanel);
                        SenderWindow.MainGrid.Children.Remove(inputDialog);
                        break;
                    }
                    else
                    {
                        if (!inputDialog.YesBool)
                        {
                            SenderWindow.MainGrid.Children.Remove(inputDialog);
                            break;
                        }
                    }
                }
                await Task.Delay(100);
            }
        }

        private async void ItemMinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Button SenderButton = sender as Button;
            Grid SenderGrid = SenderButton.Parent as Grid;
            ItemDesign SenderDesign = SenderGrid.Parent as ItemDesign;
            StackPanel SenderPanel = SenderDesign.Parent as StackPanel;
            if (SenderDesign.Height != 34)
            {
                SenderButton.Content = "^";
                for (int i = 190; i >= 34; i -= 10)
                {
                    SenderDesign.Height = i;
                    SetWindowHeight(SenderPanel);
                    await Task.Delay(1);
                }
                SenderDesign.Height = 34;
            }
            else
            {
                SenderButton.Content = "V";
                for (int i = 34; i < 190; i += 10)
                {
                    SenderDesign.Height = i;
                    SetWindowHeight(SenderPanel);
                    await Task.Delay(1);
                }
                SenderDesign.Height = 190;
            }
            SetWindowHeight(SenderPanel);
        }

        private void ItemMoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            Button SenderButton = sender as Button;
            Grid SenderGrid = SenderButton.Parent as Grid;
            ItemDesign SenderDesign = SenderGrid.Parent as ItemDesign;
            if (SenderWindow.ItemStack.Children.IndexOf(SenderDesign) >= 1)
            {
                int Index = SenderWindow.ItemStack.Children.IndexOf(SenderDesign) - 1;
                SenderWindow.ItemStack.Children.Remove(SenderDesign);
                SenderWindow.ItemStack.Children.Insert(Index, SenderDesign);
                SetWindowHeight(SenderWindow.ItemStack);
            }
        }

        private void ItemMoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            Button SenderButton = sender as Button;
            Grid SenderGrid = SenderButton.Parent as Grid;
            ItemDesign SenderDesign = SenderGrid.Parent as ItemDesign;
            if (SenderWindow.ItemStack.Children.IndexOf(SenderDesign) < SenderWindow.ItemStack.Children.Count - 1)
            {
                int Index = SenderWindow.ItemStack.Children.IndexOf(SenderDesign) + 1;
                SenderWindow.ItemStack.Children.Remove(SenderDesign);
                SenderWindow.ItemStack.Children.Insert(Index, SenderDesign);
                SetWindowHeight(SenderWindow.ItemStack);
            }
        }

        void SetWindowHeight(StackPanel SenderStack)
        {
            ScrollViewer SenderScrollViewer = SenderStack.Parent as ScrollViewer;
            double AddHeight = 300;
            for (int i = 0; i < SenderStack.Children.Count; i++)
            {
                ItemDesign SenderDesign = SenderStack.Children[i] as ItemDesign;
                AddHeight += SenderDesign.Height;
            }
            if (AddHeight > 1000)
            {
                SenderWindow.Clip = new RectangleGeometry { Rect = new Rect(0, 0, SenderWindow.Clip.Bounds.Width, 1000) };
                SenderScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            }
            else
            {
                SenderWindow.Clip = new RectangleGeometry { Rect = new Rect(0, 0, SenderWindow.Clip.Bounds.Width, AddHeight) };
                SenderScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }

        private async void ItemBorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowColorPickerTask(false);
        }

        private async void ItemFillColorButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowColorPickerTask(true);
        }

        async Task ShowColorPickerTask(bool FillColorOrBorderColor)
        {
            bool RetractAgain = false;
            if (SenderWindow.Clip.Bounds.Width == 260)
                RetractAgain = true;
            await GI.ExpandWindow(SenderWindow, true);

            ColorPicker PickColor = new ColorPicker(SenderWindow);
            SenderWindow.MainGrid.Children.Add(PickColor);
            while (true)
            {
                if (PickColor.SelectionMade)
                {
                    if (FillColorOrBorderColor)
                    {
                        if (PickColor.SelectedColor != null)
                            ItemFillColorButton.Background = PickColor.SelectedColor;
                    }
                    else
                    {
                        if (PickColor.SelectedColor != null)
                            ItemBorderColorButton.Background = PickColor.SelectedColor;
                    }
                    SenderWindow.MainGrid.Children.Remove(PickColor);
                    break;
                }
                await Task.Delay(100);
            }

            if (RetractAgain)
                await GI.ExpandWindow(SenderWindow, false);
        }

        private void ItemNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ItemNameLabel.Content = ItemNameTextBox.Text;
        }

        public void ResetWarningLabels()
        {
            URLErrorLabel.Foreground = (Brush)Application.Current.FindResource("StandartItemDesignWarnColor");
            XPathErrorLabel.Foreground = (Brush)Application.Current.FindResource("StandartItemDesignWarnColor");
            URLErrorLabel.Visibility = Visibility.Visible;
            XPathErrorLabel.Visibility = Visibility.Visible;
        }

        public void UpdateStatisticals(List<GI.GraphColunm> GraphData, double TotalValue, int Statistical_UseIndexIs, bool IsPieChartEnabled, int TimeElementsIndex)
        {
            int Index = SenderWindow.ItemStack.Children.IndexOf(this);
            if (ItemURLTextBox.Text != "" && ItemXPathTextBox.Text != "")
            {
                if (GraphData[0].GraphElements != null && GraphData[1].GraphElements != null)
                {
                    if (GraphData[0].GraphElements.Count > 0 && GraphData[1].GraphElements.Count > 0)
                    {
                        ItemValueLabel.Content = "Value: " + GraphData[0].GraphElements[Index].Value[Statistical_UseIndexIs];
                        ItemValueLabel.ToolTip = "( " + GraphData[0].TimeTable + " )";

                        if (IsPieChartEnabled)
                        {
                            double ShareVal = (((double)GraphData[0].GraphElements[Index].Value[Statistical_UseIndexIs] / TotalValue) * 100);
                            ItemShareLabel.Content = "Share: " + (int)ShareVal + "%";
                        }
                        else
                        {
                            ItemShareLabel.Content = "Share: Disabled";
                        }
                        ItemShareLabel.ToolTip = "( " + GraphData[0].TimeTable + " )";

                        double ChangeVal = GraphData[0].GraphElements[Index].Value[0] - GraphData[1].GraphElements[Index].Value[0];
                        ItemChangeLabel.Foreground = GI.GetColorFromPosNegNeuValue(ChangeVal, (Brush)Application.Current.FindResource("StandartItemDesignGoodColor"), (Brush)Application.Current.FindResource("StandartItemDesignBadColor"), (Brush)Application.Current.FindResource("StandartItemDesignLabelsForground"));
                        ItemChangeLabel.Content = "Change: " + ChangeVal;
                        ItemChangeLabel.ToolTip = "( " + GraphData[0].TimeTable + " )";

                        double AvrChange = 0;
                        for (int j = 1; j < TimeElementsIndex - 1; j++)
                        {
                            if (GraphData[j].GraphElements.Count > 0)
                                if (GraphData[j + 1].GraphElements.Count > 0)
                                    AvrChange += GraphData[j].GraphElements[Index].Value[Statistical_UseIndexIs] - GraphData[j + 1].GraphElements[Index].Value[Statistical_UseIndexIs];
                        }
                        AvrChange = AvrChange / (TimeElementsIndex - 1);

                        ItemAvrChangeLabel.Foreground = GI.GetColorFromPosNegNeuValue(AvrChange, (Brush)Application.Current.FindResource("StandartItemDesignGoodColor"), (Brush)Application.Current.FindResource("StandartItemDesignBadColor"), (Brush)Application.Current.FindResource("StandartItemDesignLabelsForground"));
                        ItemAvrChangeLabel.Content = "Avr Change: " + Math.Round(AvrChange, 2);
                        ItemAvrChangeLabel.ToolTip = "( " + GraphData[0].TimeTable + " )";
                    }
                }
            }
        }
    }
}
