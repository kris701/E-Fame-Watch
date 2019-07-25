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
    /// Interaction logic for ListSelection.xaml
    /// </summary>
    public partial class ListSelection : UserControl
    {
        MainWindow SenderWindow;
        
        public ListSelection(MainWindow _SenderWindow, List<string> _SelectionList, string Title)
        {
            SenderWindow = _SenderWindow;
            InitializeComponent();
            this.VerticalAlignment = VerticalAlignment.Top;
            this.Height = SenderWindow.Clip.Bounds.Height;

            for (int i = 0; i < _SelectionList.Count; i++)
            {
                Button NewButtonItem = new Button();
                NewButtonItem.Content = _SelectionList[i];
                NewButtonItem.Style = _SenderWindow.FindResource("StandartButtonStyle") as Style;
                NewButtonItem.Tag = i;
                NewButtonItem.Click += ListSelectionButton_Click;
                NewButtonItem.Height = 40;

                ListSelectionStackPanel.Children.Add(NewButtonItem);
            }

            ListSelectionTopLabel.Content = Title;
        }

        public bool SelectionMade = false;
        public int SelectedIndex = -1;

        private async void ListSelectionCancelButton_Click(object sender, RoutedEventArgs e)
        {
            await SenderWindow.FadeOut(this);
            SelectionMade = true;
        }

        private async void Grid_Initialized(object sender, EventArgs e)
        {
            await SenderWindow.FadeIn(this);
        }

        private async void ListSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            await SenderWindow.FadeOut(this);
            SelectedIndex = (int)(sender as Button).Tag;
            SelectionMade = true;
        }
    }
}
