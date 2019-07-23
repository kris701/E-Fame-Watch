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
        public double[] Value;
        public string Name;
        public SolidColorBrush FillColor;
        public SolidColorBrush BorderColor;

        public GraphElement(double[] _Value, string _Name, SolidColorBrush _FillColor, SolidColorBrush _BorderColor)
        {
            Value = _Value;
            Name = _Name;
            FillColor = _FillColor;
            BorderColor = _BorderColor;
        }
    }

    struct GraphColunm
    {
        public DateTime TimeTable;
        public List<GraphElement> GraphElements;

        public GraphColunm(DateTime _TimeTable, List<GraphElement> _GraphElements)
        {
            TimeTable = _TimeTable;
            GraphElements = _GraphElements;
        }
    }

    public partial class MainWindow : Window
    {
        bool Draging = false;
        Point StartDragPoint = new Point();
        enum ChartModes { Total, AVRChange };
        int CurrentMode = 0;
        static readonly int TotalChartModes = 1;
        static readonly string[] ChartModesNames = { "Total", "AVR Change" };
        static readonly int[] DelayTimes = { 86400, 3600, 60, 1 };
        static readonly int DataCacheSize = 100;
        bool RefreshModeChange = false;
        bool ClearGraphData = false;
        bool FlatlineGraphData = false;

        public MainWindow()
        {
            InitializeComponent();
            this.Opacity = 0;
        }

        async Task GetAllEFame(List<GraphColunm> GraphData)
        {
            while (true)
            {
                if (ClearGraphData)
                {
                    GraphData.Clear();
                    ClearGraphData = false;
                    for (int i = 0; i < DataCacheSize; i++)
                    {
                        GraphData.Add(new GraphColunm(DateTime.Now, new List<GraphElement>()));
                    }
                }

                if (FlatlineGraphData)
                {
                    FlatlineGraphData = false;
                    for (int i = 1; i < DataCacheSize; i++)
                    {
                        GraphData[i] = new GraphColunm(GraphData[0].TimeTable, GraphData[0].GraphElements);
                    }
                }

                GraphLoadingControl NewLoading = new GraphLoadingControl();
                NewLoading.Width = GraphCanvas.ActualWidth;
                NewLoading.Height = GraphCanvas.ActualHeight;
                GraphCanvas.Children.Add(NewLoading);

                GraphLoadingControl NewLoading2 = new GraphLoadingControl();
                NewLoading2.Width = PieGraphCanvas.ActualWidth;
                NewLoading2.Height = PieGraphCanvas.ActualHeight;
                PieGraphCanvas.Children.Add(NewLoading2);

                if (ItemStack.Children.Count > 0)
                {
                    GraphColunm NewDataSet = new GraphColunm(DateTime.Now, new List<GraphElement>());
                    try
                    {
                        for (int i = 0; i < ItemStack.Children.Count; i++)
                        {
                            ItemDesign SenderDesign = ItemStack.Children[i] as ItemDesign;
                            SolidColorBrush GetBorderBrush = (SolidColorBrush)SenderDesign.ItemBorderColorButton.Background;
                            SolidColorBrush GetFillBrush = (SolidColorBrush)SenderDesign.ItemFillColorButton.Background;

                            if (SenderDesign.ItemURLTextBox.Text != "" && SenderDesign.ItemXPathTextBox.Text != "")
                            {
                                HtmlWeb web = new HtmlWeb();
                                HtmlDocument doc = await web.LoadFromWebAsync(SenderDesign.ItemURLTextBox.Text);

                                HtmlNode node = doc.DocumentNode.SelectSingleNode(SenderDesign.ItemXPathTextBox.Text.Replace("/tbody", ""));

                                double[] NewData = { Mode1Method(node), 0 };
                                if (GraphData[1].GraphElements != null)
                                    if (GraphData[1].GraphElements.Count > i)
                                    {
                                        NewData[1] = Mode2Method(node, GraphData[1].GraphElements[i]);
                                    }

                                NewDataSet.GraphElements.Add(new GraphElement(NewData, SenderDesign.ItemNameTextBox.Text, GetFillBrush, GetBorderBrush));
                            }
                        }

                        if (TimeFrameCombobox.SelectedIndex == 0 && (GraphData[0].TimeTable - GraphData[1].TimeTable).Days >= 1 ||
                            TimeFrameCombobox.SelectedIndex == 1 && (GraphData[0].TimeTable - GraphData[1].TimeTable).Hours >= 1 ||
                            TimeFrameCombobox.SelectedIndex == 2 && (GraphData[0].TimeTable - GraphData[1].TimeTable).Minutes >= 1 ||
                            TimeFrameCombobox.SelectedIndex == 3 && (GraphData[0].TimeTable - GraphData[1].TimeTable).Seconds >= 1)
                        {
                            for (int i = GraphData.Count - 1; i > 0; i--)
                            {
                                GraphData[i] = GraphData[i - 1];
                            }
                        }

                        GraphData[0] = NewDataSet;
                    }
                    catch { }
                }
                UpdateVisualData(GraphData);

                GraphCanvas.Children.Remove(NewLoading);
                PieGraphCanvas.Children.Remove(NewLoading2);

                SaveSettings(GraphData);

                await WaitUntil(TimeFrameCombobox.SelectedIndex, TimeElementsCombobox.SelectedIndex, GraphData);
            }
        }

        #region UI Events Region

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            List<GraphColunm> GraphData = new List<GraphColunm>();

            this.Width = 260;

            TimeFrameCombobox.Items.Add("Days");
            TimeFrameCombobox.Items.Add("Hours");
            TimeFrameCombobox.Items.Add("Minutes");
            TimeFrameCombobox.Items.Add("Seconds");
            TimeFrameCombobox.SelectedIndex = 0;

            for (int i = 0; i < DataCacheSize; i++)
            {
                GraphData.Add(new GraphColunm(DateTime.Now, new List<GraphElement>()));
            }

            for (int i = 5; i < DataCacheSize + 5; i += 5)
                TimeElementsCombobox.Items.Add(i);
            TimeElementsCombobox.SelectedIndex = 0;

            LoadSettings(GraphData);

            await FadeIn(this);

            await GetAllEFame(GraphData);
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

        private void AddNewItemButton_Click(object sender, RoutedEventArgs e)
        {
            MakeNewItemPanel("Name", "", "", Brushes.White, Brushes.White, false);
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

        private void ChangeModeButton_Click(object sender, RoutedEventArgs e)
        {
            NextGraphButton.Content = "Change to mode " + (CurrentMode + 1).ToString();
            CurrentMode++;
            if (CurrentMode > TotalChartModes)
            {
                CurrentMode = 0;
            }
            GraphTopLabel.Content = ChartModesNames[CurrentMode];
            RefreshModeChange = true;
        }

        private async void ResetGraphButton_Click(object sender, RoutedEventArgs e)
        {
            WarningPopup inputDialog = new WarningPopup("You are about to reset all graph data, this will remove all previous data. Are you sure you want to continue?", "Warning");
            MainGrid.Children.Add(inputDialog);
            while (true)
            {
                if (inputDialog.SelectionMade)
                {
                    if (inputDialog.YesBool)
                    {
                        ClearGraphData = true;
                        RefreshModeChange = true;
                        MainGrid.Children.Remove(inputDialog);
                        break;
                    }
                    else
                    {
                        if (!inputDialog.YesBool)
                        {
                            MainGrid.Children.Remove(inputDialog);
                            break;
                        }
                    }
                }
                await Task.Delay(100);
            }
        }

        private async void FlatlineGraphButton_Click(object sender, RoutedEventArgs e)
        {
            WarningPopup inputDialog = new WarningPopup("Flatninging the graph will set all historic value to the newest one. Are you sure you want to continue?", "Warning");
            MainGrid.Children.Add(inputDialog);
            while (true)
            {
                if (inputDialog.SelectionMade)
                {
                    if (inputDialog.YesBool)
                    {
                        FlatlineGraphData = true;
                        RefreshModeChange = true;
                        MainGrid.Children.Remove(inputDialog);
                        break;
                    }
                    else
                    {
                        if (!inputDialog.YesBool)
                        {
                            MainGrid.Children.Remove(inputDialog);
                            break;
                        }
                    }
                }
                await Task.Delay(100);
            }
        }

        private async void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            await FadeOut(this);
            Application.Current.Shutdown();
        }

        #endregion

        #region Update Visual Region

        void UpdateVisualData(List<GraphColunm> Values)
        {
            //Avr change label

            double AvrChange = 0;
            for (int i = 0; i < (int)TimeElementsCombobox.SelectedValue - 1; i++)
            {
                AvrChange += SumOfList(Values[i], CurrentMode) - SumOfList(Values[i + 1], CurrentMode);
            }
            AvrChange = AvrChange / ((int)TimeElementsCombobox.SelectedValue - 1);

            if (AvrChange > 0)
                AvrChangeLabel.Foreground = Brushes.Green;
            if (AvrChange < 0)
                AvrChangeLabel.Foreground = Brushes.Red;
            if (AvrChange == 0)
                AvrChangeLabel.Foreground = Brushes.White;
            AvrChangeLabel.Content = "Avr Change: " + Math.Round(AvrChange, 2);

            //Total Label
            double HighestVal = 0;
            for (int i = 0; i < (int)TimeElementsCombobox.SelectedValue; i++)
            {
                double Moment = SumOfList(Values[i], CurrentMode);

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

        void UpdateColumnChart(List<GraphColunm> Values, double TotalValue)
        {
            GraphCanvas.Children.Clear();

            if (CurrentMode == 0)
            {
                TotalValue += 25;

                GraphTopValLabel.Content = TotalValue;
                GraphTopValLabel.ToolTip = new ToolTip { Content = TotalValue };
                GraphBottomValLabel.Content = 0;
                GraphBottomValLabel.ToolTip = new ToolTip { Content = 0 };
            }
            else
            {
                if (CurrentMode == 1)
                {
                    TotalValue = 25;
                    GraphTopValLabel.Content = TotalValue + 5;
                    GraphTopValLabel.ToolTip = new ToolTip { Content = TotalValue + 5 };
                    GraphBottomValLabel.Content = -(TotalValue + 5);
                    GraphBottomValLabel.ToolTip = new ToolTip { Content = -(TotalValue + 5) };
                }
                else
                {

                }
            }

            double TransformX = GraphCanvas.ActualWidth / (int)TimeElementsCombobox.SelectedValue;
            double TransformY = GraphCanvas.ActualHeight / TotalValue;
            double YOffset = 0;
            if (CurrentMode == 1)
                YOffset = GraphCanvas.ActualHeight / 2;

            for (int i = 0; i < (int)TimeElementsCombobox.SelectedValue; i++)
            {
                if (Values[i].GraphElements != null)
                {
                    for (int j = 0; j < Values[i].GraphElements.Count; j++)
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
                                AddYB = Values[i].GraphElements[j].Value[CurrentMode];
                                AddYC = Values[i].GraphElements[j].Value[CurrentMode];
                                AddYD = 0;
                            }
                            else
                            {
                                AddYA = SumOfRange(Values[i], 0, j, CurrentMode);
                                AddYB = SumOfRange(Values[i], 0, j + 1, CurrentMode);
                                AddYC = SumOfRange(Values[i], 0, j + 1, CurrentMode);
                                AddYD = SumOfRange(Values[i], 0, j, CurrentMode);
                            }
                        }
                        else
                        {
                            if (j == 0)
                            {
                                AddYA = SumOfRange(Values[i - 1], 0, j, CurrentMode);
                                AddYB = SumOfRange(Values[i - 1], 0, j + 1, CurrentMode);
                                AddYC = SumOfRange(Values[i], 0, j + 1, CurrentMode);
                                AddYD = SumOfRange(Values[i], 0, j, CurrentMode);
                            }
                            else
                            {
                                AddYA = SumOfRange(Values[i - 1], 0, j, CurrentMode);
                                AddYB = SumOfRange(Values[i - 1], 0, j + 1, CurrentMode);
                                AddYC = SumOfRange(Values[i], 0, j + 1, CurrentMode);
                                AddYD = SumOfRange(Values[i], 0, j, CurrentMode);
                            }
                        }

                        Polygon NewPolygon = new Polygon();
                        NewPolygon.Stroke = Values[i].GraphElements[j].BorderColor;
                        NewPolygon.Fill = Values[i].GraphElements[j].FillColor;
                        NewPolygon.StrokeThickness = 2;
                        NewPolygon.Points = new PointCollection() {
                            new Point((i * TransformX) + 1, (GraphCanvas.ActualHeight - AddYA * TransformY - YOffset) - 1),
                            new Point((i * TransformX) + 1, (GraphCanvas.ActualHeight - AddYB * TransformY - YOffset) + 1),
                            new Point(((i + 1) * TransformX) - 1, (GraphCanvas.ActualHeight - AddYC * TransformY - YOffset) + 1),
                            new Point(((i + 1) * TransformX) - 1, (GraphCanvas.ActualHeight - AddYD * TransformY - YOffset) - 1)
                        };

                        ToolTip tooltip = new ToolTip { Content = Values[i].GraphElements[j].Name + ": " + Values[i].GraphElements[j].Value[CurrentMode] + " ( " + SumOfList(Values[i], CurrentMode) + " )" };
                        NewPolygon.ToolTip = tooltip;

                        GraphCanvas.Children.Add(NewPolygon);
                    }
                }
            }
        }

        void UpdatePieChart(List<GraphColunm> Values, double TotalValue)
        {
            PieGraphCanvas.Children.Clear();

            double OffSetAngle = 359;
            List<GraphElement> SortedList = new List<GraphElement>(Values[0].GraphElements);

            SortedList.Sort((s1, s2) => s1.Value[0].CompareTo(s2.Value[0]));

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
                double Procent = ((double)SortedList[j].Value[0] / (double)TotalValue);
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

                if (Procent > 1)
                    Procent = 1;
                if (Procent < 0)
                    Procent = 0;

                ToolTip tooltip = new ToolTip { Content = SortedList[j].Name + ": " + (int)(Procent * 100) + "%" };
                Newpath.ToolTip = tooltip;

                PieGraphCanvas.Children.Add(Newpath);
            }
        }

        void UpdateItemsStatisticalData(List<GraphColunm> Values, double TotalValue)
        {
            if (Values.Count > 0)
            {
                for (int i = 0; i < ItemStack.Children.Count; i++)
                {
                    try
                    {
                        ItemDesign SenderDesign = ItemStack.Children[i] as ItemDesign;
                        if (SenderDesign.ItemURLTextBox.Text != "" && SenderDesign.ItemXPathTextBox.Text != "")
                        {
                            if (Values[0].GraphElements != null && Values[1].GraphElements != null)
                            {
                                if (Values[0].GraphElements.Count > 0 && Values[1].GraphElements.Count > 0)
                                {
                                    SenderDesign.ItemValueLabel.Content = "Value: " + Values[0].GraphElements[i].Value[CurrentMode];

                                    double ShareVal = (((double)Values[0].GraphElements[i].Value[CurrentMode] / (double)TotalValue) * 100);
                                    SenderDesign.ItemShareLabel.Content = "Share: " + (int)ShareVal + "%";

                                    double ChangeVal = Values[0].GraphElements[i].Value[0] - Values[1].GraphElements[i].Value[0];
                                    if (ChangeVal > 0)
                                        SenderDesign.ItemChangeLabel.Foreground = Brushes.Green;
                                    if (ChangeVal < 0)
                                        SenderDesign.ItemChangeLabel.Foreground = Brushes.Red;
                                    if (ChangeVal == 0)
                                        SenderDesign.ItemChangeLabel.Foreground = Brushes.White;
                                    SenderDesign.ItemChangeLabel.Content = "Change: " + ChangeVal;

                                    double AvrChange = 0;
                                    for (int j = 1; j < (int)TimeElementsCombobox.SelectedValue - 1; j++)
                                    {
                                        if (Values[j].GraphElements.Count > 0)
                                            if (Values[j + 1].GraphElements.Count > 0)
                                                AvrChange += SumOfListSimple(Values[j].GraphElements[i].Value, CurrentMode) - SumOfListSimple(Values[j + 1].GraphElements[i].Value, CurrentMode);
                                    }
                                    AvrChange = AvrChange / ((int)TimeElementsCombobox.SelectedValue - 1);

                                    if (AvrChange > 0)
                                        SenderDesign.ItemAvrChangeLabel.Foreground = Brushes.Green;
                                    if (AvrChange < 0)
                                        SenderDesign.ItemAvrChangeLabel.Foreground = Brushes.Red;
                                    if (AvrChange == 0)
                                        SenderDesign.ItemAvrChangeLabel.Foreground = Brushes.White;
                                    SenderDesign.ItemAvrChangeLabel.Content = "Avr Change: " + Math.Round(AvrChange, 2);
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        #endregion

        #region Save and Load Region

        void SaveSettings(List<GraphColunm> GraphData)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("cfg.txt"))
                {
                    file.WriteLine(TimeFrameCombobox.SelectedIndex + ";" + TimeElementsCombobox.SelectedIndex + ";");
                    for (int i = 0; i < ItemStack.Children.Count; i++)
                    {
                        ItemDesign SenderDesign = ItemStack.Children[i] as ItemDesign;
                        file.WriteLine("I;" + SenderDesign.ItemNameTextBox.Text + ";" + SenderDesign.ItemURLTextBox.Text + ";" + SenderDesign.ItemXPathTextBox.Text + ";" + (SolidColorBrush)SenderDesign.ItemBorderColorButton.Background + ";" + (SolidColorBrush)SenderDesign.ItemFillColorButton.Background + ";");
                    }

                    for (int i = 0; i < DataCacheSize; i++)
                    {
                        string WriteString = "D;" + GraphData[i].TimeTable + ";";
                        for (int j = 0; j < GraphData[i].GraphElements.Count; j++)
                        {
                            if (i != 0)
                            {
                                WriteString += "S;";
                                if (GraphData[i].GraphElements[j].Name == GraphData[i - 1].GraphElements[j].Name)
                                {
                                    WriteString += "^;";
                                }
                                else
                                {
                                    WriteString += GraphData[i].GraphElements[j].Name + ";";
                                }

                                if (DoubleArrayToString(":", GraphData[i].GraphElements[j].Value) == DoubleArrayToString(":", GraphData[i - 1].GraphElements[j].Value))
                                {
                                    WriteString += "^;";
                                }
                                else
                                {
                                    WriteString += DoubleArrayToString(":", GraphData[i].GraphElements[j].Value) + ";";
                                }

                                if (GraphData[i].GraphElements[j].BorderColor == GraphData[i - 1].GraphElements[j].BorderColor)
                                {
                                    WriteString += "^;";
                                }
                                else
                                {
                                    WriteString += GraphData[i].GraphElements[j].BorderColor + ";";
                                }

                                if (GraphData[i].GraphElements[j].FillColor == GraphData[i - 1].GraphElements[j].FillColor)
                                {
                                    WriteString += "^;";
                                }
                                else
                                {
                                    WriteString += GraphData[i].GraphElements[j].FillColor + ";";
                                }

                                WriteString += "E;";
                            }
                            else
                            {
                                WriteString += "S;" + GraphData[i].GraphElements[j].Name + ";" + DoubleArrayToString(":", GraphData[i].GraphElements[j].Value) + ";" + GraphData[i].GraphElements[j].BorderColor + ";" + GraphData[i].GraphElements[j].FillColor + ";E;";
                            }
                        }
                        file.WriteLine(WriteString);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Could not save cfg!");
            }
        }

        void LoadSettings(List<GraphColunm> GraphData)
        {
            try
            {
                if (System.IO.File.Exists("cfg.txt"))
                {
                    GraphData.Clear();
                    for (int i = 0; i < DataCacheSize; i++)
                    {
                        GraphData.Add(new GraphColunm(DateTime.Now, new List<GraphElement>()));
                    }

                    string[] lines = System.IO.File.ReadAllLines("cfg.txt");

                    TimeFrameCombobox.SelectedIndex = Int32.Parse(lines[0].Split(';')[0]);
                    TimeElementsCombobox.SelectedIndex = Int32.Parse(lines[0].Split(';')[1]);

                    int InnerIndex = 0;

                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] Split = lines[i].Split(';');
                        if (Split[0] == "I")
                        {
                            MakeNewItemPanel(Split[1], Split[2], Split[3], (SolidColorBrush)(new BrushConverter().ConvertFrom(Split[4])), (SolidColorBrush)(new BrushConverter().ConvertFrom(Split[5])), true);
                        }
                        if (Split[0] == "D")
                        {
                            GraphColunm NewData = new GraphColunm();
                            NewData.TimeTable = DateTime.Parse(Split[1]);
                            NewData.GraphElements = new List<GraphElement>();
                            int Count = 0;
                            for (int j = 2; j < Split.Length; j++)
                            {
                                if (Split[j] == "S")
                                {
                                    GraphElement NewElement = new GraphElement();
                                    if (Split[j + 2] == "^")
                                    {
                                        NewElement.Value = GraphData[InnerIndex - 1].GraphElements[Count].Value;
                                    }
                                    else
                                    {
                                        NewElement.Value = SplitString(':', Split[j + 2]);
                                    }

                                    if (Split[j + 1] == "^")
                                    {
                                        NewElement.Name = GraphData[InnerIndex - 1].GraphElements[Count].Name;
                                    }
                                    else
                                    {
                                        NewElement.Name = Split[j + 1];
                                    }

                                    if (Split[j + 4] == "^")
                                    {
                                        NewElement.FillColor = GraphData[InnerIndex - 1].GraphElements[Count].FillColor;
                                    }
                                    else
                                    {
                                        NewElement.FillColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(Split[j + 4]));
                                    }

                                    if (Split[j + 3] == "^")
                                    {
                                        NewElement.BorderColor = GraphData[InnerIndex - 1].GraphElements[Count].BorderColor;
                                    }
                                    else
                                    {
                                        NewElement.BorderColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(Split[j + 3]));
                                    }

                                    NewData.GraphElements.Add(NewElement);
                                    Count++;
                                    j += 5;
                                }
                            }
                            GraphData[InnerIndex] = NewData;
                            InnerIndex++;
                        }
                    }
                    UpdateVisualData(GraphData);
                }
            }
            catch
            {
                MessageBox.Show("Could not load cfg");
            }
        }

        #endregion

        #region CommonElements

        double SumOfList(GraphColunm Values, int _CurrentMode)
        {
            if (Values.GraphElements == null)
                return 0;

            double Moment = 0;
            for (int i = 0; i < Values.GraphElements.Count; i++)
                Moment += SumOfListSimple(Values.GraphElements[i].Value, _CurrentMode);
            return Moment;
        }

        double SumOfListSimple(double[] Values, int _CurrentMode)
        {
            return Values[_CurrentMode];
        }

        double SumOfRange(GraphColunm _Input, int _From, int _To, int _CurrentMode)
        {
            if (_To > _Input.GraphElements.Count)
                _To = _Input.GraphElements.Count;
            double ReturnVal = 0;
            for (int i = _From; i < _To; i++)
                ReturnVal += _Input.GraphElements[i].Value[_CurrentMode];
            return ReturnVal;
        }

        void MakeNewItemPanel(string _Name, string _URL, string _XPath, SolidColorBrush _BorderColor, SolidColorBrush _FillColor, bool _Minimized)
        {
            ItemDesign NewDesign = new ItemDesign();

            if (_Minimized)
            {
                NewDesign.MainItemGrid.Height = 30;
                if (this.Height + 34 < 1000)
                    this.Height += 34;
            }
            else
            {
                NewDesign.MainItemGrid.Height = 150;
                if (this.Height + 154 < 1000)
                    this.Height += 154;
            }

            NewDesign.ItemNameTextBox.Text = _Name;
            NewDesign.ItemURLTextBox.Text = _URL;
            NewDesign.ItemXPathTextBox.Text = _XPath;
            NewDesign.ItemBorderColorButton.Background = _BorderColor;
            NewDesign.ItemFillColorButton.Background = _FillColor;
            if (_Minimized)
                NewDesign.ItemMinimizeButton.Content = "^";
            else
                NewDesign.ItemMinimizeButton.Content = "V";

            ItemStack.Children.Add(NewDesign);
        }

        async Task WaitUntil(int Index, int Index2, List<GraphColunm> GraphData)
        {
            if (TimeFrameCombobox.SelectedIndex == Index)
            {
                for (int i = 0; i < DelayTimes[Index]; i++)
                {
                    await Task.Delay(1000);
                    if (TimeFrameCombobox.SelectedIndex != Index)
                    {
                        if (TimeFrameCombobox.SelectedIndex > Index)
                        {
                            break;
                        }
                        UpdateVisualData(GraphData);
                    }
                    if (RefreshModeChange)
                    {
                        UpdateVisualData(GraphData);
                        RefreshModeChange = false;
                    }
                    if (ClearGraphData || FlatlineGraphData)
                        break;
                }
            }
        }

        public async Task FadeIn(UIElement FadeElement)
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

        double Mode1Method(HtmlNode Node)
        {
            return Int32.Parse(Node.InnerText.Replace(",", "").Replace(".", ""));
        }

        double Mode2Method(HtmlNode Node, GraphElement GraphElement)
        {
            return Mode1Method(Node) - SumOfListSimple(GraphElement.Value, 0);
        }

        double[] SplitString(char Delimiter, string Input)
        {
            string[] SplitString = Input.Split(Delimiter);
            double[] DoubleList = new double[TotalChartModes + 1];
            for (int i = 0; i < SplitString.Length - 1; i++)
            {
                DoubleList[i] = Convert.ToDouble(SplitString[i]);
            }
            return DoubleList;
        }

        string DoubleArrayToString(string Delimiter, double[] Input)
        {
            string OutString = "";
            for (int i = 0; i < Input.Length; i++)
                OutString += Input[i] + Delimiter;
            return OutString;
        }

        #endregion

        private void TimeElementsCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshModeChange = true;
        }
    }
}
