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
using HtmlAgilityPack;

namespace E_Fame_Watch
{
    struct GraphElement
    {
        public int Value;
        public string Name;
        public SolidColorBrush FillColor;
        public SolidColorBrush BorderColor;

        public GraphElement(int _Value, string _Name, SolidColorBrush _FillColor, SolidColorBrush _BorderColor)
        {
            Value = _Value;
            Name = _Name;
            FillColor = _FillColor;
            BorderColor = _BorderColor;
        }
    }

    public partial class MainWindow : Window
    {
        bool Draging = false;
        Point StartDragPoint = new Point();
        List<List<GraphElement>> _GraphData = new List<List<GraphElement>>();
        List<DateTime> _TimeTable = new List<DateTime>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            List<List<GraphElement>> GraphData = new List<List<GraphElement>>();
            List<DateTime> TimeTable = new List<DateTime>();

            this.Width = 260;

            TimeFrameCombobox.Items.Add("Days");
            TimeFrameCombobox.Items.Add("Hours");
            TimeFrameCombobox.Items.Add("Minutes");
            TimeFrameCombobox.Items.Add("Seconds");
            TimeFrameCombobox.SelectedIndex = 0;

            for (int i = 0; i < 5; i++)
            {
                GraphData.Add(new List<GraphElement>());
                TimeTable.Add(new DateTime());
            }

            for (int i = 5; i < 105; i += 5)
                TimeElementsCombobox.Items.Add(i);
            TimeElementsCombobox.SelectedIndex = 0;

            LoadSettings(GraphData, TimeTable);
            await GetAllEFame(GraphData, TimeTable);
        }

        async Task GetAllEFame(List<List<GraphElement>> GraphData, List<DateTime> TimeTable)
        {
            while (true)
            {
                if (ItemStack.Children.Count > 0)
                {
                    try
                    {
                        ResizeTimeTabelAndGraphData(GraphData, TimeTable);

                        TimeTable[GraphData.Count - 1] = DateTime.Now;
                        if (TimeFrameCombobox.SelectedIndex == 0)
                        {
                            if ((TimeTable[GraphData.Count - 1] - TimeTable[GraphData.Count - 2]).Days >= 1)
                            {
                                for (int i = 0; i < TimeTable.Count - 1; i++)
                                {
                                    GraphData[i] = GraphData[i + 1];
                                    TimeTable[i] = TimeTable[i + 1];
                                }
                            }
                        }
                        if (TimeFrameCombobox.SelectedIndex == 1)
                        {
                            if ((TimeTable[GraphData.Count - 1] - TimeTable[GraphData.Count - 2]).Hours >= 1)
                            {
                                for (int i = 0; i < TimeTable.Count - 1; i++)
                                {
                                    GraphData[i] = GraphData[i + 1];
                                    TimeTable[i] = TimeTable[i + 1];
                                }
                            }
                        }
                        if (TimeFrameCombobox.SelectedIndex == 2)
                        {
                            if ((TimeTable[GraphData.Count - 1] - TimeTable[GraphData.Count - 2]).Minutes >= 1)
                            {
                                for (int i = 0; i < TimeTable.Count - 1; i++)
                                {
                                    GraphData[i] = GraphData[i + 1];
                                    TimeTable[i] = TimeTable[i + 1];
                                }
                            }
                        }
                        if (TimeFrameCombobox.SelectedIndex == 3)
                        {
                            if ((TimeTable[GraphData.Count - 1] - TimeTable[GraphData.Count - 2]).Seconds >= 1)
                            {
                                for (int i = 0; i < TimeTable.Count - 1; i++)
                                {
                                    GraphData[i] = GraphData[i + 1];
                                    TimeTable[i] = TimeTable[i + 1];
                                }
                            }
                        }

                        GraphData[GraphData.Count - 1] = new List<GraphElement>();
                        for (int i = 0; i < ItemStack.Children.Count; i++)
                        {
                            ItemDesign SenderDesign = ItemStack.Children[i] as ItemDesign;
                            SolidColorBrush GetBorderBrush = Brushes.Red;
                            if (SenderDesign.ItemBorderColorComboBox.SelectedItem != null)
                                GetBorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(SenderDesign.ItemBorderColorComboBox.SelectedItem.ToString());
                            SolidColorBrush GetFillBrush = Brushes.Red;
                            if (SenderDesign.ItemFillColorComboBox.SelectedItem != null)
                                GetFillBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(SenderDesign.ItemFillColorComboBox.SelectedItem.ToString());

                            if (SenderDesign.ItemURLTextBox.Text != "" && SenderDesign.ItemXPathTextBox.Text != "")
                            {
                                HtmlWeb web = new HtmlWeb();
                                HtmlDocument doc = await web.LoadFromWebAsync(SenderDesign.ItemURLTextBox.Text);

                                HtmlNode node = doc.DocumentNode.SelectSingleNode(SenderDesign.ItemXPathTextBox.Text.Replace("/tbody", ""));

                                GraphData[GraphData.Count - 1].Add(new GraphElement(Int32.Parse(node.InnerText.Replace(",", "").Replace(".", "")), SenderDesign.ItemNameTextBox.Text, GetFillBrush, GetBorderBrush));
                            }
                        }
                    }
                    catch
                    {
                        GraphData[GraphData.Count - 1].Clear();
                        GraphData[GraphData.Count - 1].Add(new GraphElement(0, "Error", Brushes.Red, Brushes.Pink));
                    }
                }
                if (TimeFrameCombobox.SelectedIndex == 0)
                    await Task.Delay(86400000);
                if (TimeFrameCombobox.SelectedIndex == 1)
                    await Task.Delay(3600000);
                if (TimeFrameCombobox.SelectedIndex == 2)
                    await Task.Delay(60000);
                if (TimeFrameCombobox.SelectedIndex == 3)
                    await Task.Delay(1000);
                UpdateVisualData(GraphData);
                SaveSettings(GraphData, TimeTable);
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Draging = true;
            Mouse.Capture(MainBorder);
            StartDragPoint = e.GetPosition(this);
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Draging = false;
            Mouse.Capture(null);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (Draging)
            {
                Left = (System.Windows.Forms.Cursor.Position.X - StartDragPoint.X);
                Top = (System.Windows.Forms.Cursor.Position.Y - StartDragPoint.Y);
            }
        }

        void UpdateVisualData(List<List<GraphElement>> Values)
        {
            //Gain Label
            int Difference = GetDiff(Values[Values.Count - 1], Values[0]);
            if (Difference > 0)
            {
                GainedLabel.Foreground = Brushes.Green;
            }
            if (Difference < 0)
            {
                GainedLabel.Foreground = Brushes.Red;
            }
            if (Difference == 0)
            {
                GainedLabel.Foreground = Brushes.White;
            }
            GainedLabel.Content = "Gained: " + Difference;

            //Total Label
            int HighestVal = 0;
            for (int i = 0; i < Values.Count; i++)
            {
                int Moment = SumOfList(Values[i]);

                if (Moment > HighestVal)
                    HighestVal = Moment;
            }

            TotalLabel.Content = "Total: " + HighestVal;

            //Charts
            UpdateColumnChart(Values, HighestVal);

            UpdatePieChart(Values, HighestVal);

            //Individual items data

            UpdateItemsStatisticalData(Values, HighestVal);
        }

        void UpdateColumnChart(List<List<GraphElement>> Values, int TotalValue)
        {
            GraphCanvas.Children.Clear();

            TotalValue += 25;

            GraphTopValLabel.Content = TotalValue;
            GraphTopValLabel.ToolTip = new ToolTip { Content = TotalValue };

            double TransformX = GraphCanvas.ActualWidth / Values.Count;
            double TransformY = GraphCanvas.ActualHeight / TotalValue;

            for (int i = 0; i < Values.Count; i++)
            {
                for (int j = 0; j < Values[i].Count; j++)
                {
                    double AddYA = 0;
                    double AddYB = 0;
                    double AddYC = 0;
                    double AddYD = 0;

                    if (i == 0)
                    {
                        if (j == 0)
                        {
                            AddYA = 0;
                            AddYB = Values[i][j].Value;
                            AddYC = Values[i][j].Value;
                            AddYD = 0;
                        }
                        else
                        {
                            AddYA = SumOfRange(Values[i], 0, j);
                            AddYB = SumOfRange(Values[i], 0, j + 1);
                            AddYC = SumOfRange(Values[i], 0, j + 1);
                            AddYD = SumOfRange(Values[i], 0, j);
                        }
                    }
                    else
                    {
                        if (j == 0)
                        {
                            AddYA = SumOfRange(Values[i - 1], 0, j);
                            AddYB = SumOfRange(Values[i - 1], 0, j + 1);
                            AddYC = SumOfRange(Values[i], 0, j + 1);
                            AddYD = SumOfRange(Values[i], 0, j);
                        }
                        else
                        {
                            AddYA = SumOfRange(Values[i - 1], 0, j);
                            AddYB = SumOfRange(Values[i - 1], 0, j + 1);
                            AddYC = SumOfRange(Values[i], 0, j + 1);
                            AddYD = SumOfRange(Values[i], 0, j);
                        }
                    }

                    Polygon NewPolygon = new Polygon();
                    NewPolygon.Stroke = Values[i][j].BorderColor;
                    NewPolygon.Fill = Values[i][j].FillColor;
                    NewPolygon.StrokeThickness = 2;
                    NewPolygon.Points = new PointCollection() {
                        new Point(i * TransformX, GraphCanvas.ActualHeight - AddYA * TransformY),
                        new Point(i * TransformX, GraphCanvas.ActualHeight - AddYB * TransformY),
                        new Point((i + 1) * TransformX, GraphCanvas.ActualHeight - AddYC * TransformY),
                        new Point((i + 1) * TransformX, GraphCanvas.ActualHeight - AddYD * TransformY)
                    };

                    ToolTip tooltip = new ToolTip { Content = Values[i][j].Name + ": " + Values[i][j].Value + " (" + SumOfRange(Values[i], 0, Values[i].Count) + ")" };
                    NewPolygon.ToolTip = tooltip;

                    GraphCanvas.Children.Add(NewPolygon);
                }
            }
        }

        void UpdatePieChart(List<List<GraphElement>> Values, int TotalValue)
        {
            double OffSetAngle = 359;
            List<GraphElement> SortedList = new List<GraphElement>(Values[Values.Count - 1]);

            SortedList.Sort((s1, s2) => s1.Value.CompareTo(s2.Value));

            for (int j = 0; j < SortedList.Count; j++)
            {
                Path Newpath = new Path();
                Newpath.Stroke = SortedList[j].BorderColor;
                Newpath.Fill = SortedList[j].FillColor;
                Newpath.StrokeThickness = 2;
                Newpath.Margin = new Thickness((PieGraphCanvas.ActualWidth / 2), (PieGraphCanvas.ActualHeight / 2), 0, 0);

                PathGeometry pathGeometry = new PathGeometry();

                PathFigure pathFigure = new PathFigure();
                pathFigure.StartPoint = new Point(0, 0);
                pathFigure.IsClosed = true;
                double Procent = ((double)SortedList[j].Value / (double)TotalValue);
                double radius = 70;
                double angle = OffSetAngle;
                OffSetAngle -= 360 * Procent;

                LineSegment lineSegment = new LineSegment(new Point(radius, 0), true);
                ArcSegment arcSegment = new ArcSegment();
                arcSegment.IsLargeArc = angle >= 180.0;
                arcSegment.Point = new Point(Math.Cos(angle * Math.PI / 180) * radius, Math.Sin(angle * Math.PI / 180) * radius);
                arcSegment.Size = new Size(radius, radius);
                arcSegment.SweepDirection = SweepDirection.Clockwise;

                pathFigure.Segments.Add(lineSegment);
                pathFigure.Segments.Add(arcSegment);

                pathGeometry.Figures.Add(pathFigure);

                Newpath.Data = pathGeometry;

                ToolTip tooltip = new ToolTip { Content = SortedList[j].Name + ": " + (int)(Procent * 100) + "%" };
                Newpath.ToolTip = tooltip;

                PieGraphCanvas.Children.Add(Newpath);
            }
        }

        void UpdateItemsStatisticalData(List<List<GraphElement>> Values, int TotalValue)
        {
            if (Values.Count > 0)
            {
                for (int i = 0; i < ItemStack.Children.Count; i++)
                {
                    ItemDesign SenderDesign = ItemStack.Children[i] as ItemDesign;
                    if (SenderDesign.ItemURLTextBox.Text != "" && SenderDesign.ItemXPathTextBox.Text != "")
                    {
                        if (Values[Values.Count - 1].Count > 0 && Values[0].Count > i)
                        {
                            double ShareVal = (((double)Values[Values.Count - 1][i].Value / (double)TotalValue) * 100);
                            SenderDesign.ItemShareLabel.Content = "Share: " + (int)ShareVal + "%";

                            int ChangeVal = Values[Values.Count - 1][i].Value - Values[0][i].Value;
                            if (ChangeVal > 0)
                                SenderDesign.ItemChangeLabel.Foreground = Brushes.Green;
                            if (ChangeVal < 0)
                                SenderDesign.ItemChangeLabel.Foreground = Brushes.Red;
                            if (ChangeVal == 0)
                                SenderDesign.ItemChangeLabel.Foreground = Brushes.White;
                            SenderDesign.ItemChangeLabel.Content = "Change: " + ChangeVal;

                            double AvrChange = 0;
                            for (int j = 1; j < Values.Count; j++)
                            {
                                AvrChange += Values[j][i].Value - Values[j - 1][i].Value;
                            }
                            AvrChange = AvrChange / Values.Count;

                            if (AvrChange > 0)
                                SenderDesign.ItemAvrChangeLabel.Foreground = Brushes.Green;
                            if (AvrChange < 0)
                                SenderDesign.ItemAvrChangeLabel.Foreground = Brushes.Red;
                            if (AvrChange == 0)
                                SenderDesign.ItemAvrChangeLabel.Foreground = Brushes.White;
                            SenderDesign.ItemAvrChangeLabel.Content = "Avr Change: " + AvrChange;
                        }
                    }
                }
            }
        }

        double SumOfRange(List<GraphElement> _Input, int _From, int _To)
        {
            if (_Input.Count == 0)
                return 0;
            if (_To > _Input.Count)
                _To = _Input.Count;
            double ReturnVal = 0;
            for (int i = _From; i < _To; i++)
                ReturnVal += _Input[i].Value;
            return ReturnVal;
        }

        private void AddNewItemButton_Click(object sender, RoutedEventArgs e)
        {
            MakeNewItemPanel("Name", "", "", 0, 0, false);
        }

        RowDefinition GetRowWithHeight(int _Height)
        {
            RowDefinition NewRow = new RowDefinition();
            NewRow.Height = new GridLength(_Height);
            return NewRow;
        }

        ColumnDefinition GetColumnWithWidth(int _Width)
        {
            ColumnDefinition NewColumn = new ColumnDefinition();
            NewColumn.Width = new GridLength(_Width);
            return NewColumn;
        }

        void SaveSettings(List<List<GraphElement>> GraphData, List<DateTime> TimeTable)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("cfg.txt"))
            {
                file.WriteLine(TimeFrameCombobox.SelectedIndex + ";" + TimeElementsCombobox.SelectedIndex + ";");
                for (int i = 0; i < ItemStack.Children.Count; i++)
                {
                    ItemDesign SenderDesign = ItemStack.Children[i] as ItemDesign;

                    file.WriteLine("ITEM;" + SenderDesign.ItemNameTextBox.Text + ";" + SenderDesign.ItemURLTextBox.Text + ";" + SenderDesign.ItemXPathTextBox.Text + ";" + SenderDesign.ItemBorderColorComboBox.SelectedIndex + ";" + SenderDesign.ItemFillColorComboBox.SelectedIndex + ";");
                }

                for (int i = 0; i < GraphData.Count; i++)
                {
                    string WriteString = "DATA;" + TimeTable[i] + ";";
                    for (int j = 0; j < GraphData[i].Count; j++)
                        WriteString += "SET;" + GraphData[i][j].Name + ";" + GraphData[i][j].Value + ";" + GraphData[i][j].BorderColor + ";" + GraphData[i][j].FillColor + ";END;";
                    file.WriteLine(WriteString);
                }
            }
        }

        void LoadSettings(List<List<GraphElement>> GraphData, List<DateTime> TimeTable)
        {
            if (System.IO.File.Exists("cfg.txt"))
            {
                GraphData.Clear();
                TimeTable.Clear();

                string[] lines = System.IO.File.ReadAllLines("cfg.txt");

                TimeFrameCombobox.SelectedIndex = Int32.Parse(lines[0].Split(';')[0]);
                TimeElementsCombobox.SelectedIndex = Int32.Parse(lines[0].Split(';')[1]);

                ResizeTimeTabelAndGraphData(GraphData, TimeTable);

                int InnerIndex = 0;

                for (int i = 1; i < lines.Length; i++)
                {
                    string[] Split = lines[i].Split(';');
                    if (Split[0] == "ITEM")
                    {
                        MakeNewItemPanel(Split[1], Split[2], Split[3], Int32.Parse(Split[4]), Int32.Parse(Split[5]), true);
                    }
                    if (Split[0] == "DATA")
                    {
                        TimeTable[TimeTable.Count - 1] = (DateTime.Parse(Split[1]));
                        for (int j = 2; j < Split.Length; j++)
                        {
                            if (Split[j] == "SET")
                            {
                                GraphData[InnerIndex].Add(new GraphElement(Int32.Parse(Split[j + 2]), Split[j + 1], (SolidColorBrush)(new BrushConverter().ConvertFrom(Split[j + 4])), (SolidColorBrush)(new BrushConverter().ConvertFrom(Split[j + 3]))));
                                j += 5;
                            }
                        }
                        InnerIndex++;
                    }
                }
                UpdateVisualData(GraphData);
            }
        }

        void ResizeTimeTabelAndGraphData(List<List<GraphElement>> GraphData, List<DateTime> TimeTable)
        {
            if ((int)TimeElementsCombobox.SelectedItem > GraphData.Count)
            {
                while (GraphData.Count < (int)TimeElementsCombobox.SelectedItem)
                {
                    GraphData.Insert(0, new List<GraphElement>());
                    TimeTable.Insert(0, new DateTime());
                }
            }
            if ((int)TimeElementsCombobox.SelectedItem < GraphData.Count)
            {
                while (GraphData.Count > (int)TimeElementsCombobox.SelectedItem)
                {
                    GraphData.RemoveAt(0);
                    TimeTable.RemoveAt(0);
                }
            }
        }

        void MakeNewItemPanel(string _Name, string _URL, string _XPath, int _BorderColorIndex, int _FillColorIndex, bool _Minimized)
        {
            ItemDesign NewDesign = new ItemDesign();

            if (_Minimized)
            {
                NewDesign.MainItemGrid.Height = 30;
                this.Height += 34;
            }
            else
            {
                NewDesign.MainItemGrid.Height = 160;
                this.Height += 164;
            }

            NewDesign.ItemNameTextBox.Text = _Name;
            NewDesign.ItemURLTextBox.Text = _URL;
            NewDesign.ItemXPathTextBox.Text = _XPath;
            NewDesign.ItemBorderColorComboBox.SelectedIndex = _BorderColorIndex;
            NewDesign.ItemFillColorComboBox.SelectedIndex = _FillColorIndex;
            if (_Minimized)
                NewDesign.ItemMinimizeButton.Content = "^";
            else
                NewDesign.ItemMinimizeButton.Content = "V";

            ItemStack.Children.Add(NewDesign);
        }

        int GetDiff(List<GraphElement> ListA, List<GraphElement> ListB)
        {
            return SumOfList(ListA) - SumOfList(ListB);
        }

        int SumOfList(List<GraphElement> Values)
        {
            int Moment = 0;
            for (int i = 0; i < Values.Count; i++)
                Moment += Values[i].Value;
            return Moment;
        }

        private async void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Width == 260)
            {
                for (int i = 260; i < 520; i += 20)
                {
                    this.Width = i;
                    await Task.Delay(1);
                }
                ExpandButton.Content = "<";
                this.Width = 520;
            }
            else
            {
                for (int i = 520; i >= 260; i -= 20)
                {
                    this.Width = i;
                    await Task.Delay(1);
                }
                ExpandButton.Content = ">";
                this.Width = 260;
            }
        }
    }
}
