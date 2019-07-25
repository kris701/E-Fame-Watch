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
    #region StructsAndEnums

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

    enum GMCS { TotalValues, AVRChange, DisablePieChart, EnablePieChart, PieChart_UseIndex, ColumnChart_UseIndex, Statistical_UseIndex, General_UseIndex, SetChartLabels, SetGraphMode, DisplayGraphMiddleLine, HideGraphMiddleLine };

    struct GMC
    {
        public GMCS Command;
        public int OverloadInt;
        public int OverloadInt2;
        public int OverloadInt3;
        public int Modifier;
        public int Modifier2;
        public int Modifier3;

        public GMC(GMCS _Command, int _OverloadInt = 0, int _Modifier = 1, int _OverloadInt2 = 0, int _Modifier2 = 1, int _OverloadInt3 = 0, int _Modifier3 = 1)
        {
            Command = _Command;
            OverloadInt = _OverloadInt;
            Modifier = _Modifier;
            OverloadInt2 = _OverloadInt2;
            Modifier2 = _Modifier2;
            OverloadInt3 = _OverloadInt3;
            Modifier3 = _Modifier3;
        }
    }

    struct GraphMode
    {
        public string Name;
        public List<GMC> ValueProcessingList;
        public List<GMC> GraphModeCommandList;

        public GraphMode(string _Name, List<GMC> _GraphModeCommandList, List<GMC> _ValueProcessingList)
        {
            Name = _Name;
            GraphModeCommandList = _GraphModeCommandList;
            ValueProcessingList = _ValueProcessingList;
        }
    }

    #endregion

    public partial class MainWindow : Window
    {
        #region Variabels

        bool Draging = false;
        Point StartDragPoint = new Point();
        static readonly int[] DelayTimes = { 86400, 3600, 60, 1 };
        static readonly int DataCacheSize = 50;

        #region GraphModes

        static readonly List<GraphMode> GraphModes = new List<GraphMode>() {
            new GraphMode("Total", 
                new List<GMC>() {
                    new GMC(GMCS.EnablePieChart),
                    new GMC(GMCS.SetGraphMode, 0),
                    new GMC(GMCS.Statistical_UseIndex, 0),
                    new GMC(GMCS.General_UseIndex, 0),
                    new GMC(GMCS.SetChartLabels, 25, 1, 0, 0, 0, 1),
                    new GMC(GMCS.ColumnChart_UseIndex, 0),
                    new GMC(GMCS.HideGraphMiddleLine)
                }, 
                new List<GMC>() {
                    new GMC(GMCS.TotalValues)
                }),

            new GraphMode("Change (Column)", 
                new List<GMC>() {
                    new GMC(GMCS.DisablePieChart),
                    new GMC(GMCS.SetGraphMode, 1),
                    new GMC(GMCS.Statistical_UseIndex, 0),
                    new GMC(GMCS.General_UseIndex, 1),
                    new GMC(GMCS.SetChartLabels, 0, 1, 0, -1, 2, 2),
                    new GMC(GMCS.ColumnChart_UseIndex, 1),
                    new GMC(GMCS.DisplayGraphMiddleLine)
                }, 
                new List<GMC>() {
                    new GMC(GMCS.AVRChange)
                }),

            new GraphMode("Change (Line)", 
                new List<GMC>() {
                    new GMC(GMCS.DisablePieChart),
                    new GMC(GMCS.SetGraphMode, 2),
                    new GMC(GMCS.Statistical_UseIndex, 0),
                    new GMC(GMCS.General_UseIndex, 1),
                    new GMC(GMCS.SetChartLabels, 0, 1, 0, -1, 2, 2),
                    new GMC(GMCS.ColumnChart_UseIndex, 1),
                    new GMC(GMCS.DisplayGraphMiddleLine)
                }, 
                new List<GMC>() {
                    new GMC(GMCS.AVRChange)
                })
        };

        #endregion

        int CurrentGraphMode = 0;
        bool RefreshModeChange = false;
        bool ClearGraphData = false;
        bool FlatlineGraphData = false;
        bool EqualizeColorsData = false;
        bool IsPieChartEnabled = true;
        int Statistical_UseIndexIs = 0;
        int PieChart_UseIndexIs = 0;
        int ColumnChart_UseIndexIs = 0;
        int General_UseIndexIs = 0;
        int ChartLabelOffsetTop = 0;
        int ChartLabelOffSetTopModifier = 1;
        int ChartLabelOffsetBottom = 0;
        int ChartLabelOffSetBottomModifier = 1;
        int GraphModeColumn = 0;
        int GraphYOffsetModifier = 0;
        int GraphYTransformModifier = 1;
        bool DisplayGraphMiddleLine = false;

        #endregion

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

                GraphLoadingControl NewLoading = new GraphLoadingControl(GraphCanvas.ActualWidth, GraphCanvas.ActualHeight);
                GraphCanvas.Children.Add(new GraphLoadingControl(GraphCanvas.ActualWidth, GraphCanvas.ActualHeight));

                GraphLoadingControl NewLoading2 = null;
                if (IsPieChartEnabled)
                {
                    NewLoading2 = new GraphLoadingControl(PieGraphCanvas.ActualWidth, PieGraphCanvas.ActualHeight);
                    PieGraphCanvas.Children.Add(NewLoading2);
                }

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

                            SenderDesign.URLErrorLabel.Visibility = Visibility.Visible;
                            SenderDesign.XPathErrorLabel.Visibility = Visibility.Visible;

                            if (SenderDesign.ItemURLTextBox.Text != "" && SenderDesign.ItemXPathTextBox.Text != "")
                            {
                                HtmlWeb web = new HtmlWeb();
                                HtmlDocument doc = await web.LoadFromWebAsync(SenderDesign.ItemURLTextBox.Text);

                                SenderDesign.URLErrorLabel.Visibility = Visibility.Hidden;

                                HtmlNode node = doc.DocumentNode.SelectSingleNode(SenderDesign.ItemXPathTextBox.Text.Replace("/tbody", ""));

                                if (node == null)
                                    break;
                                SenderDesign.XPathErrorLabel.Visibility = Visibility.Hidden;

                                double[] NewData = new double[GraphModes.Count];
                                for (int j = 0; j < GraphModes.Count; j++)
                                {
                                    NewData[j] = RunCommandList(j, GraphModes[j].ValueProcessingList ,node, GraphData, i);
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

                if (EqualizeColorsData)
                {
                    EqualizeColorsData = false;
                    for (int i = 1; i < DataCacheSize; i++)
                    {
                        for (int j = 0; j < ItemStack.Children.Count; j++)
                        {
                            int TargetJ = j;
                            while (GraphData[i].GraphElements[TargetJ].Name != GraphData[0].GraphElements[j].Name)
                            {
                                TargetJ++;
                                if (TargetJ > ItemStack.Children.Count - 1)
                                    TargetJ = 0;
                                if (TargetJ == j)
                                    break;
                            }
                            GraphData[i].GraphElements[TargetJ] = new GraphElement(GraphData[i].GraphElements[TargetJ].Value, GraphData[i].GraphElements[TargetJ].Name, GraphData[0].GraphElements[j].FillColor, GraphData[0].GraphElements[j].BorderColor);
                        }
                    }
                }

                UpdateVisualData(GraphData);

                GraphCanvas.Children.Remove(NewLoading);
                if (IsPieChartEnabled)
                {
                    PieGraphCanvas.Children.Remove(NewLoading2);
                }

                SaveSettings(GraphData);

                await WaitUntil(TimeFrameCombobox.SelectedIndex, TimeElementsCombobox.SelectedIndex, GraphData);
            }
        }

        #region UI Events Region
        public MainWindow()
        {
            InitializeComponent();
            this.Opacity = 0;
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, 260, 300) };

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

            RunCommandList(CurrentGraphMode, GraphModes[CurrentGraphMode].GraphModeCommandList, null, null, 0);

            UpdateVisualData(GraphData);

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
            if (this.Clip.Bounds.Width == 260)
            {
                Grid.SetColumnSpan(ItemsScrollBar, 5);
                for (int i = 260; i < 520; i += 10)
                {
                    this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, i, this.Clip.Bounds.Height) };
                    await Task.Delay(1);
                }
                ExpandButton.Content = "<";
                this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, 520, this.Clip.Bounds.Height) };
            }
            else
            {
                for (int i = 520; i > 260; i -= 10)
                {
                    this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, i, this.Clip.Bounds.Height) };
                    await Task.Delay(1);
                }
                ExpandButton.Content = ">";
                this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, 260, this.Clip.Bounds.Height) };
                Grid.SetColumnSpan(ItemsScrollBar, 3);
            }
        }

        private async void ChangeModeButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> MomentList = new List<string>();
            for (int i = 0; i < GraphModes.Count; i++)
                MomentList.Add(GraphModes[i].Name);

            ListSelection inputDialog = new ListSelection(this, MomentList, "Select a mode");
            MainGrid.Children.Add(inputDialog);
            while (true)
            {
                if (inputDialog.SelectionMade)
                {
                    if (inputDialog.SelectedIndex != -1)
                    {
                        CurrentGraphMode = inputDialog.SelectedIndex;
                        RefreshModeChange = true;
                        GraphTopLabel.Content = GraphModes[CurrentGraphMode].Name;
                        RunCommandList(CurrentGraphMode, GraphModes[CurrentGraphMode].GraphModeCommandList, null, null, 0);
                    }
                    MainGrid.Children.Remove(inputDialog);
                    break;
                }
                await Task.Delay(100);
            }
        }

        private async void ResetGraphButton_Click(object sender, RoutedEventArgs e)
        {
            WarningPopup inputDialog = new WarningPopup(this, "Flatninging the graph will set all historic value to the newest one. Are you sure you want to continue?", "Warning");
            MainGrid.Children.Add(inputDialog);
            while (true)
            {
                if (inputDialog.SelectionMade)
                {
                    if (inputDialog.YesBool)
                    {
                        FlatlineGraphData = true;
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
            WarningPopup inputDialog = new WarningPopup(this, "Flatninging the graph will set all historic value to the newest one. Are you sure you want to continue?", "Warning");
            MainGrid.Children.Add(inputDialog);
            while (true)
            {
                if (inputDialog.SelectionMade)
                {
                    if (inputDialog.YesBool)
                    {
                        FlatlineGraphData = true;
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

        private async void EqualizeColorsBotton_Click(object sender, RoutedEventArgs e)
        {
            WarningPopup inputDialog = new WarningPopup(this, "Equalizing colors will reset all historic colors. Are you sure you want to continue?", "Warning");
            MainGrid.Children.Add(inputDialog);
            while (true)
            {
                if (inputDialog.SelectionMade)
                {
                    if (inputDialog.YesBool)
                    {
                        EqualizeColorsData = true;
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

        private async void MinimizeItemStackButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemStack.Children.Count > 0)
            {
                if (this.Clip.Bounds.Height == 295)
                {
                    double TargetHeight = 0;
                    for (int i = 0; i < ItemStack.Children.Count; i++)
                    {
                        ItemDesign SenderDesign = ItemStack.Children[i] as ItemDesign;
                        TargetHeight += SenderDesign.Height;
                    }
                    if (TargetHeight > 1000)
                        TargetHeight = 1000;
                    for (int i = (int)this.Clip.Bounds.Height; i < 295 + TargetHeight; i += 10)
                    {
                        this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.Clip.Bounds.Width, i) };
                        await Task.Delay(1);
                    }
                    this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.Clip.Bounds.Width, 295 + TargetHeight) };
                }
                else
                {
                    for (int i = (int)this.Clip.Bounds.Height; i > 295; i -= 10)
                    {
                        this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.Clip.Bounds.Width, i) };
                        await Task.Delay(1);
                    }
                    this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.Clip.Bounds.Width, 295) };
                }
            }
        }

        private async void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            await FadeOut(this);
            Application.Current.Shutdown();
        }

        private void TimeElementsCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshModeChange = true;
        }

        #endregion

        #region Update Visual Region

        void UpdateVisualData(List<GraphColunm> Values)
        {
            //Avr change label

            double AvrChange = 0;
            for (int i = 0; i < (int)TimeElementsCombobox.SelectedValue - 1; i++)
            {
                AvrChange += SumOfList(Values[i], General_UseIndexIs) - SumOfList(Values[i + 1], General_UseIndexIs);
            }
            AvrChange = AvrChange / ((int)TimeElementsCombobox.SelectedValue - 1);

            AvrChangeLabel.Foreground = GetColorFromPosNegNeuValue(AvrChange, Brushes.LightGreen, Brushes.Pink, Brushes.White);
            AvrChangeLabel.Content = "Avr Change: " + Math.Round(AvrChange, 2);

            //Total Label
            double HighestVal = 0;
            for (int i = 0; i < (int)TimeElementsCombobox.SelectedValue; i++)
            {
                double Moment = Math.Abs(SumOfList(Values[i], General_UseIndexIs));

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

            if (DisplayGraphMiddleLine)
                GraphCanvasMiddleLine.Visibility = Visibility.Visible;
            else
                GraphCanvasMiddleLine.Visibility = Visibility.Hidden;

            GraphTopValLabel.Content = ChartLabelOffSetTopModifier * (TotalValue + ChartLabelOffsetTop);
            GraphTopValLabel.ToolTip = new ToolTip { Content = GraphTopValLabel.Content };
            GraphBottomValLabel.Content = ChartLabelOffSetBottomModifier * (TotalValue + ChartLabelOffsetBottom);
            GraphBottomValLabel.ToolTip = new ToolTip { Content = GraphBottomValLabel.Content };

            double TransformX = GraphCanvas.ActualWidth / (int)TimeElementsCombobox.SelectedValue;
            double TransformY = GraphCanvas.ActualHeight / (TotalValue * GraphYTransformModifier);
            double YOffset = 0;
            if (GraphYOffsetModifier != 0)
                YOffset = GraphCanvas.ActualHeight / GraphYOffsetModifier;

            for (int i = 0; i < (int)TimeElementsCombobox.SelectedValue; i++)
            {
                if (Values[i].GraphElements != null)
                {
                    double PosDirOffsetY = 0;
                    double NegDirOffsetY = 0;
                    for (int j = 0; j < Values[i].GraphElements.Count; j++)
                    {
                        double AddYA = 0;
                        double AddYB = 0;
                        double AddYC = 0;
                        double AddYD = 0;

                        Polygon NewPolygon = new Polygon();
                        ToolTip tooltip = new ToolTip();

                        if (GraphModeColumn == 0)
                        {
                            NewPolygon.Stroke = Values[i].GraphElements[j].BorderColor;
                            NewPolygon.Fill = Values[i].GraphElements[j].FillColor;

                            AddYA = PosDirOffsetY;
                            AddYB = PosDirOffsetY + Values[i].GraphElements[j].Value[ColumnChart_UseIndexIs];
                            AddYC = PosDirOffsetY + Values[i].GraphElements[j].Value[ColumnChart_UseIndexIs];
                            AddYD = PosDirOffsetY;

                            PosDirOffsetY += Values[i].GraphElements[j].Value[ColumnChart_UseIndexIs];

                            tooltip = new ToolTip { Content = Values[i].GraphElements[j].Name + ": " + Values[i].GraphElements[j].Value[ColumnChart_UseIndexIs] + " ( " + SumOfList(Values[i], ColumnChart_UseIndexIs) + " )" };
                        }
                        if (GraphModeColumn == 1)
                        {
                            NewPolygon.Stroke = Values[i].GraphElements[j].BorderColor;
                            NewPolygon.Fill = Values[i].GraphElements[j].FillColor;
                            if (Values[i].GraphElements[j].Value[ColumnChart_UseIndexIs] > 0)
                            {
                                AddYA = PosDirOffsetY;
                                AddYB = PosDirOffsetY + Values[i].GraphElements[j].Value[ColumnChart_UseIndexIs];
                                AddYC = PosDirOffsetY + Values[i].GraphElements[j].Value[ColumnChart_UseIndexIs];
                                AddYD = PosDirOffsetY;

                                PosDirOffsetY += Values[i].GraphElements[j].Value[ColumnChart_UseIndexIs];
                            }
                            else
                            {
                                AddYA = NegDirOffsetY;
                                AddYB = NegDirOffsetY + Values[i].GraphElements[j].Value[ColumnChart_UseIndexIs];
                                AddYC = NegDirOffsetY + Values[i].GraphElements[j].Value[ColumnChart_UseIndexIs];
                                AddYD = NegDirOffsetY;

                                NegDirOffsetY += Values[i].GraphElements[j].Value[ColumnChart_UseIndexIs];
                            }
                            tooltip = new ToolTip { Content = Values[i].GraphElements[j].Name + ": " + Values[i].GraphElements[j].Value[ColumnChart_UseIndexIs] + " ( " + SumOfList(Values[i], ColumnChart_UseIndexIs) + " )" };
                        }
                        if (GraphModeColumn == 2)
                        {
                            NewPolygon.Stroke = Brushes.White;
                            NewPolygon.Fill = Brushes.Transparent;
                            AddYA = SumOfRange(Values[i], 0, Values[i].GraphElements.Count, ColumnChart_UseIndexIs);
                            AddYB = AddYA;
                            AddYC = SumOfRange(Values[i + 1], 0, Values[i].GraphElements.Count, ColumnChart_UseIndexIs);
                            AddYD = AddYC;

                            tooltip = new ToolTip { Content = SumOfRange(Values[i], 0, Values[i].GraphElements.Count, ColumnChart_UseIndexIs) };
                        }

                        NewPolygon.ToolTip = tooltip;
                        NewPolygon.StrokeThickness = 2;
                        NewPolygon.ClipToBounds = true;
                        NewPolygon.Points = new PointCollection() {
                            new Point(FitValue((i * TransformX) + 1, 0, GraphCanvas.ActualWidth), FitValue((GraphCanvas.ActualHeight - AddYA * TransformY - YOffset) - 1, 0, GraphCanvas.ActualHeight)),
                            new Point(FitValue((i * TransformX) + 1, 0, GraphCanvas.ActualWidth), FitValue((GraphCanvas.ActualHeight - AddYB * TransformY - YOffset) + 1, 0, GraphCanvas.ActualHeight)),
                            new Point(FitValue(((i + 1) * TransformX) - 1, 0, GraphCanvas.ActualWidth), FitValue((GraphCanvas.ActualHeight - AddYC * TransformY - YOffset) + 1, 0, GraphCanvas.ActualHeight)),
                            new Point(FitValue(((i + 1) * TransformX) - 1, 0, GraphCanvas.ActualWidth), FitValue((GraphCanvas.ActualHeight - AddYD * TransformY - YOffset) - 1, 0, GraphCanvas.ActualHeight))
                        };

                        GraphCanvas.Children.Add(NewPolygon);
                    }
                }
            }
        }

        double FitValue(double Value, double Min, double Max)
        {
            if (Value < Min)
                Value = Min;
            if (Value > Max)
                Value = Max;
            return Value;
        }

        void UpdatePieChart(List<GraphColunm> Values, double TotalValue)
        {
            PieGraphCanvas.Children.Clear();

            if (IsPieChartEnabled)
            {
                PieGraphLabel.Visibility = Visibility.Hidden;
                double OffSetAngle = 359;
                List<GraphElement> SortedList = new List<GraphElement>(Values[0].GraphElements);

                SortedList.Sort((s1, s2) => s1.Value[PieChart_UseIndexIs].CompareTo(s2.Value[PieChart_UseIndexIs]));

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
            else
                PieGraphLabel.Visibility = Visibility.Visible;
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
                                    SenderDesign.ItemValueLabel.Content = "Value: " + Values[0].GraphElements[i].Value[Statistical_UseIndexIs];

                                    if (IsPieChartEnabled)
                                    {
                                        double ShareVal = (((double)Values[0].GraphElements[i].Value[Statistical_UseIndexIs] / (double)TotalValue) * 100);
                                        SenderDesign.ItemShareLabel.Content = "Share: " + (int)ShareVal + "%";
                                    }
                                    else
                                    {
                                        SenderDesign.ItemShareLabel.Content = "Share: Disabled";
                                    }

                                    double ChangeVal = Values[0].GraphElements[i].Value[0] - Values[1].GraphElements[i].Value[0];
                                    SenderDesign.ItemChangeLabel.Foreground = GetColorFromPosNegNeuValue(ChangeVal, Brushes.LightGreen, Brushes.Pink, Brushes.White);
                                    SenderDesign.ItemChangeLabel.Content = "Change: " + ChangeVal;

                                    double AvrChange = 0;
                                    for (int j = 1; j < (int)TimeElementsCombobox.SelectedValue - 1; j++)
                                    {
                                        if (Values[j].GraphElements.Count > 0)
                                            if (Values[j + 1].GraphElements.Count > 0)
                                                AvrChange += Values[j].GraphElements[i].Value[Statistical_UseIndexIs] - Values[j + 1].GraphElements[i].Value[Statistical_UseIndexIs];
                                    }
                                    AvrChange = AvrChange / ((int)TimeElementsCombobox.SelectedValue - 1);

                                    SenderDesign.ItemAvrChangeLabel.Foreground = GetColorFromPosNegNeuValue(AvrChange, Brushes.LightGreen, Brushes.Pink, Brushes.White);
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
                    file.WriteLine(Application.Current.MainWindow.Left + ";" + Application.Current.MainWindow.Top + ";");
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

                    Application.Current.MainWindow.Left = Int32.Parse(lines[0].Split(';')[0]);
                    Application.Current.MainWindow.Top = Int32.Parse(lines[0].Split(';')[1]);

                    TimeFrameCombobox.SelectedIndex = Int32.Parse(lines[1].Split(';')[0]);
                    TimeElementsCombobox.SelectedIndex = Int32.Parse(lines[1].Split(';')[1]);

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
                            if (InnerIndex > GraphData.Count - 1)
                                break;
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
                Moment += Values.GraphElements[i].Value[_CurrentMode];
            return Moment;
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
            ItemDesign NewDesign = new ItemDesign(this);

            if (_Minimized)
            {
                NewDesign.Height = 34;
                if (this.Clip.Bounds.Height + 34 < 1000)
                    this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.Clip.Bounds.Width, this.Clip.Bounds.Height + 34) };
            }
            else
            {
                NewDesign.Height = 190;
                if (this.Clip.Bounds.Height + 184 < 1000)
                    this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.Clip.Bounds.Width, this.Clip.Bounds.Height + 184) };
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
                    Index = TimeFrameCombobox.SelectedIndex;
                }
                if (RefreshModeChange)
                {
                    UpdateVisualData(GraphData);
                    RefreshModeChange = false;
                }
                if (ClearGraphData || FlatlineGraphData || EqualizeColorsData)
                    break;
            }
        }

        public async Task FadeIn(UIElement FadeElement)
        {
            for (int i = 0; i < 100; i += 5)
            {
                FadeElement.Opacity = ((double)i / (double)100);
                await Task.Delay(10);
            }
            FadeElement.Opacity = 1;
        }

        public async Task FadeOut(UIElement FadeElement)
        {
            for (int i = 100; i >= 0; i -= 5)
            {
                FadeElement.Opacity = ((double)i / (double)100);
                await Task.Delay(10);
            }
            FadeElement.Opacity = 0;
        }

        double[] SplitString(char Delimiter, string Input)
        {
            string[] SplitString = Input.Split(Delimiter);
            double[] DoubleList = new double[SplitString.Length];
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

        double RunCommandList(int GraphModeIndex, List<GMC> _Commands, HtmlNode Node, List<GraphColunm> GraphData, int GraphDataIndex)
        {
            double ReturnValue = 0;
            try
            {
                for (int i = 0; i < _Commands.Count; i++)
                {
                    if (_Commands[i].Command == GMCS.TotalValues)
                        ReturnValue = Int32.Parse(Node.InnerText.Replace(",", "").Replace(".", ""));

                    if (_Commands[i].Command == GMCS.AVRChange)
                        if (GraphData[1].GraphElements.Count != 0)
                            ReturnValue = Int32.Parse(Node.InnerText.Replace(",", "").Replace(".", "")) - GraphData[1].GraphElements[GraphDataIndex].Value[0];

                    if (_Commands[i].Command == GMCS.EnablePieChart)
                        IsPieChartEnabled = true;

                    if (_Commands[i].Command == GMCS.DisablePieChart)
                        IsPieChartEnabled = false;

                    if (_Commands[i].Command == GMCS.Statistical_UseIndex)
                    {
                        Statistical_UseIndexIs = _Commands[i].OverloadInt;
                    }

                    if (_Commands[i].Command == GMCS.PieChart_UseIndex)
                    {
                        PieChart_UseIndexIs = _Commands[i].OverloadInt;
                    }

                    if (_Commands[i].Command == GMCS.ColumnChart_UseIndex)
                    {
                        ColumnChart_UseIndexIs = _Commands[i].OverloadInt;
                    }

                    if (_Commands[i].Command == GMCS.General_UseIndex)
                    {
                        General_UseIndexIs = _Commands[i].OverloadInt;
                    }

                    if (_Commands[i].Command == GMCS.SetChartLabels)
                    {
                        ChartLabelOffsetTop = _Commands[i].OverloadInt;
                        ChartLabelOffSetTopModifier = _Commands[i].Modifier;
                        ChartLabelOffsetBottom = _Commands[i].OverloadInt2;
                        ChartLabelOffSetBottomModifier = _Commands[i].Modifier2;
                        GraphYOffsetModifier = _Commands[i].OverloadInt3;
                        GraphYTransformModifier = _Commands[i].Modifier3;
                    }

                    if (_Commands[i].Command == GMCS.SetGraphMode)
                    {
                        GraphModeColumn = _Commands[i].OverloadInt;
                    }

                    if (_Commands[i].Command == GMCS.DisplayGraphMiddleLine)
                    {
                        DisplayGraphMiddleLine = true;
                    }

                    if (_Commands[i].Command == GMCS.HideGraphMiddleLine)
                    {
                        DisplayGraphMiddleLine = false;
                    }
                }
            }
            catch
            {
                ReturnValue = 0;
            }
            return ReturnValue;
        }

        public async Task ExpandWindow(bool ExpandOrRectract)
        {
            if (ExpandOrRectract)
            {
                bool Expand = false;
                if (this.Clip.Bounds.Width == 260)
                {
                    Expand = true;
                }

                if (Expand)
                {
                    for (int i = 260; i < 520; i += 20)
                    {
                        this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, i, this.Clip.Bounds.Height) };
                        await Task.Delay(1);
                    }
                    this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, 520, this.Clip.Bounds.Height) };
                }
            }
            else
            {
                bool Retract = false;
                if (this.Clip.Bounds.Width == 520)
                {
                    Retract = true;
                }

                if (Retract)
                {
                    for (int i = 520; i >= 260; i -= 20)
                    {
                        this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, i, this.Clip.Bounds.Height) };
                        await Task.Delay(1);
                    }
                    this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, 260, this.Clip.Bounds.Height) };
                }
            }
        }

        Brush GetColorFromPosNegNeuValue(double Value, Brush PositiveColor, Brush NegativeColor, Brush NeutralColor)
        {
            if (Value > 0)
                return PositiveColor;
            if (Value < 0)
                return NegativeColor;
            return NeutralColor;
        }

        #endregion
    }
}
