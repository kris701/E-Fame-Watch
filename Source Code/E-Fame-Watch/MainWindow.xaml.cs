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
    public partial class MainWindow : Window
    {
        #region Variabels

        bool Draging = false;
        Point StartDragPoint = new Point();
        static readonly int DataCacheSize = 50;
        static readonly List<GI.ThemeInfo> ThemeInfos = new List<GI.ThemeInfo>() {
            new GI.ThemeInfo("Dark Theme","DefaultTheme.xaml"),
            new GI.ThemeInfo("Gray Theme","GrayTheme.xaml")
        };

        #region GraphModes

        GI.GMCValues GMCValues = new GI.GMCValues();

        static readonly List<GI.GraphMode> GraphModes = new List<GI.GraphMode>() {
            new GI.GraphMode("Total", 
                new List<GI.GMC>() {
                    new GI.GMC(GI.GMCS.EnablePieChart),
                    new GI.GMC(GI.GMCS.SetGraphMode, 0),
                    new GI.GMC(GI.GMCS.Statistical_UseIndex, 0),
                    new GI.GMC(GI.GMCS.General_UseIndex, 0),
                    new GI.GMC(GI.GMCS.SetChartLabels, 25, 1, 0, 0, 0, 1),
                    new GI.GMC(GI.GMCS.ColumnChart_UseIndex, 0),
                    new GI.GMC(GI.GMCS.HideGraphMiddleLine)
                }, 
                new List<GI.GMC>() {
                    new GI.GMC(GI.GMCS.TotalValues)
                }),

            new GI.GraphMode("Change (Column)", 
                new List<GI.GMC>() {
                    new GI.GMC(GI.GMCS.DisablePieChart),
                    new GI.GMC(GI.GMCS.SetGraphMode, 1),
                    new GI.GMC(GI.GMCS.Statistical_UseIndex, 0),
                    new GI.GMC(GI.GMCS.General_UseIndex, 1),
                    new GI.GMC(GI.GMCS.SetChartLabels, 0, 1, 0, -1, 2, 2),
                    new GI.GMC(GI.GMCS.ColumnChart_UseIndex, 1),
                    new GI.GMC(GI.GMCS.DisplayGraphMiddleLine)
                }, 
                new List<GI.GMC>() {
                    new GI.GMC(GI.GMCS.AVRChange)
                }),

            new GI.GraphMode("Change (Line)", 
                new List<GI.GMC>() {
                    new GI.GMC(GI.GMCS.DisablePieChart),
                    new GI.GMC(GI.GMCS.SetGraphMode, 2),
                    new GI.GMC(GI.GMCS.Statistical_UseIndex, 0),
                    new GI.GMC(GI.GMCS.General_UseIndex, 1),
                    new GI.GMC(GI.GMCS.SetChartLabels, 0, 1, 0, -1, 2, 2),
                    new GI.GMC(GI.GMCS.ColumnChart_UseIndex, 1),
                    new GI.GMC(GI.GMCS.DisplayGraphMiddleLine)
                }, 
                new List<GI.GMC>() {
                    new GI.GMC(GI.GMCS.AVRChange)
                })
        };

        #endregion

        public int CurrentGraphMode = 0;
        public int CurrentThemeIndex = 0;
        bool RefreshModeChange = false;
        bool ClearGraphData = false;
        bool FlatlineGraphData = false;
        bool EqualizeColorsData = false;
        bool ExitSave = false;
        bool SaveComplete = false;
        bool ErrorInLoading = false;
        bool ManualRefresh = false;

        #endregion

        async Task RunMainLoop(List<GI.GraphColunm> GraphData)
        {
            while (true)
            {
                ManualRefresh = false;

                if (ExitSave)
                {
                    if (!SaveComplete)
                        SaveAndLoadClass.SaveSettings(GraphData, this, DataCacheSize, CurrentGraphMode, CurrentThemeIndex);
                    SaveComplete = true;
                    while (true)
                        await Task.Delay(100);
                }

                if (ClearGraphData)
                {
                    GraphData.Clear();
                    ClearGraphData = false;
                    for (int i = 0; i < DataCacheSize; i++)
                    {
                        GraphData.Add(new GI.GraphColunm(DateTime.Now, new List<GI.GraphElement>()));
                    }
                }

                if (FlatlineGraphData)
                {
                    FlatlineGraphData = false;
                    for (int i = 1; i < DataCacheSize; i++)
                    {
                        GraphData[i] = new GI.GraphColunm(GraphData[0].TimeTable, GraphData[0].GraphElements);
                    }
                }

                GraphLoadingControl NewLoading = new GraphLoadingControl(GraphCanvas, GraphCanvas.ActualWidth, GraphCanvas.ActualHeight);
                GraphCanvas.Children.Add(NewLoading);

                GraphLoadingControl NewLoading2 = null;
                if (GMCValues.IsPieChartEnabled)
                {
                    NewLoading2 = new GraphLoadingControl(PieGraphCanvas, PieGraphCanvas.ActualWidth, PieGraphCanvas.ActualHeight);
                    PieGraphCanvas.Children.Add(NewLoading2);
                }

                if (ItemStack.Children.Count > 0)
                {
                    for (int i = 0; i < ItemStack.Children.Count; i++)
                    {
                        ItemDesign SenderDesign = ItemStack.Children[i] as ItemDesign;
                        SenderDesign.ResetWarningLabels();
                    }

                    GI.GraphColunm NewDataSet = new GI.GraphColunm(DateTime.Now, new List<GI.GraphElement>());
                    try
                    {
                        if (IsTimeOver(DateTime.Now, GraphData[0].TimeTable, TimeFrameCombobox.SelectedIndex))
                        {
                            for (int i = 0; i < ItemStack.Children.Count; i++)
                            {
                                if (ExitSave)
                                    break;

                                ItemDesign SenderDesign = ItemStack.Children[i] as ItemDesign;

                                SenderDesign.URLErrorLabel.Foreground = (Brush)Application.Current.FindResource("StandartItemDesignBadColor");
                                SenderDesign.XPathErrorLabel.Foreground = (Brush)Application.Current.FindResource("StandartItemDesignBadColor");

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
                                        NewData[j] = RunCommandList(j, GraphModes[j].ValueProcessingList, node, GraphData, i);
                                    }

                                    NewDataSet.GraphElements.Add(new GI.GraphElement(NewData, SenderDesign.ItemNameTextBox.Text, (SolidColorBrush)SenderDesign.ItemFillColorButton.Background, (SolidColorBrush)SenderDesign.ItemBorderColorButton.Background));

                                    doc = null;
                                    GC.Collect();
                                }
                            }
                        }

                        if (!ExitSave)
                        {
                            if (IsTimeOver(DateTime.Now, GraphData[0].TimeTable, TimeFrameCombobox.SelectedIndex))
                            {
                                for (int i = GraphData.Count - 1; i > 0; i--)
                                {
                                    GraphData[i] = GraphData[i - 1];
                                }
                                GraphData[0] = NewDataSet;
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
                                        GraphData[i].GraphElements[TargetJ] = new GI.GraphElement(GraphData[i].GraphElements[TargetJ].Value, GraphData[i].GraphElements[TargetJ].Name, GraphData[0].GraphElements[j].FillColor, GraphData[0].GraphElements[j].BorderColor);
                                    }
                                }
                            }

                            UpdateVisualData(GraphData);
                        }

                        SaveAndLoadClass.SaveSettings(GraphData, this, DataCacheSize, CurrentGraphMode, CurrentThemeIndex);
                    }
                    catch
                    {
                        ErrorInLoading = true;
                    }
                }

                NewLoading.DoRemove();
                if (GMCValues.IsPieChartEnabled)
                {
                    NewLoading2.DoRemove();
                }

                if (ErrorInLoading)
                { 
                    await Task.Delay(10000);
                    ErrorInLoading = false;
                }
                else
                {
                    await WaitUntil(TimeFrameCombobox.SelectedIndex, GraphData);
                }
            }
        }

        #region UI Events Region
        public MainWindow()
        {
            InitializeComponent();
            this.Opacity = 0;
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ExitSave = true;
            RefreshModeChange = true;

            e.Cancel = true;

            await GI.FadeOut(this);

            while (!SaveComplete)
            {
                await Task.Delay(100);
            }

            Application.Current.Shutdown();
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, 260, 300) };

            List<GI.GraphColunm> GraphData = new List<GI.GraphColunm>();

            TimeFrameCombobox.Items.Add("Days");
            TimeFrameCombobox.Items.Add("Hours");
            TimeFrameCombobox.Items.Add("Minutes");
            TimeFrameCombobox.Items.Add("Seconds");
            TimeFrameCombobox.SelectedIndex = 0;

            for (int i = 0; i < DataCacheSize; i++)
            {
                GraphData.Add(new GI.GraphColunm(DateTime.Now, new List<GI.GraphElement>()));
            }

            for (int i = 5; i < DataCacheSize + 5; i += 5)
                TimeElementsCombobox.Items.Add(i);
            TimeElementsCombobox.SelectedIndex = 0;

            SaveAndLoadClass.LoadSettings(GraphData, this, DataCacheSize, GraphModes.Count - 1);

            RunCommandList(CurrentGraphMode, GraphModes[CurrentGraphMode].GraphModeCommandList, null, null, 0);
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/Themes/" + ThemeInfos[CurrentThemeIndex].URI, UriKind.Relative) });

            UpdateVisualData(GraphData);

            await GI.FadeIn(this);

            await RunMainLoop(GraphData);
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
            GI.MakeNewItemPanel(this, "Name", "", "", Brushes.White, Brushes.White, false);
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
            WarningPopup inputDialog = new WarningPopup(this, "Flattening the graph will set all historic value to the newest one. Are you sure you want to continue?", "Warning");
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

        private async void ChangeThemeButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> MomentList = new List<string>();
            for (int i = 0; i < ThemeInfos.Count; i++)
                MomentList.Add(ThemeInfos[i].Name);

            ListSelection inputDialog = new ListSelection(this, MomentList, "Select a theme");
            MainGrid.Children.Add(inputDialog);
            while (true)
            {
                if (inputDialog.SelectionMade)
                {
                    if (inputDialog.SelectedIndex != -1)
                    {
                        Application.Current.Resources.MergedDictionaries.Clear();
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/Themes/" + ThemeInfos[inputDialog.SelectedIndex].URI, UriKind.Relative) });
                        CurrentThemeIndex = inputDialog.SelectedIndex;
                        RefreshModeChange = true;
                    }
                    MainGrid.Children.Remove(inputDialog);
                    break;
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
            await GI.FadeOut(this);

            ExitSave = true;
            RefreshModeChange = true;

            while (!SaveComplete)
                await Task.Delay(100);

            Application.Current.Shutdown();
        }

        private void TimeElementsCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshModeChange = true;
        }

        private void GraphRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshModeChange = true;
            ManualRefresh = true;
        }

        #endregion

        #region Update Visual Region

        void UpdateVisualData(List<GI.GraphColunm> Values)
        {
            //Avr change label

            double AvrChange = 0;
            for (int i = 0; i < (int)TimeElementsCombobox.SelectedValue - 1; i++)
            {
                AvrChange += SumOfList(Values[i], GMCValues.General_UseIndexIs) - SumOfList(Values[i + 1], GMCValues.General_UseIndexIs);
            }
            AvrChange = AvrChange / ((int)TimeElementsCombobox.SelectedValue - 1);

            AvrChangeLabel.Foreground = GI.GetColorFromPosNegNeuValue(AvrChange, (Brush)Application.Current.FindResource("StandartFrontGoodColor"), (Brush)Application.Current.FindResource("StandartFrontBadColor"), (Brush)Application.Current.FindResource("StandartFrontColor"));
            AvrChangeLabel.Content = "Avr Change: " + Math.Round(AvrChange, 2);
            AvrChangeLabel.ToolTip = new ToolTip { Content = "AVR(Value - Previous value)" }; ;

            //Total Label
            double HighestVal = 0;
            for (int i = 0; i < (int)TimeElementsCombobox.SelectedValue; i++)
            {
                double Moment = Math.Abs(SumOfList(Values[i], GMCValues.General_UseIndexIs));

                if (Moment > HighestVal)
                    HighestVal = Moment;
            }
            TotalLabel.Content = "Total: " + HighestVal;
            TotalLabel.ToolTip = new ToolTip { Content = "Total value on graph" };

            //Charts
            UpdateColumnChart(Values, HighestVal);

            UpdatePieChart(Values, HighestVal);

            //Individual items data
            UpdateItemsStatisticalData(Values, HighestVal);
        }

        void UpdateColumnChart(List<GI.GraphColunm> Values, double TotalValue)
        {
            GraphCanvas.Children.Clear();

            if (GMCValues.DisplayGraphMiddleLine)
                GraphCanvasMiddleLine.Visibility = Visibility.Visible;
            else
                GraphCanvasMiddleLine.Visibility = Visibility.Hidden;

            GraphTopValLabel.Content = GMCValues.ChartLabelOffSetTopModifier * (TotalValue + GMCValues.ChartLabelOffsetTop);
            GraphTopValLabel.ToolTip = new ToolTip { Content = GraphTopValLabel.Content };
            GraphBottomValLabel.Content = GMCValues.ChartLabelOffSetBottomModifier * (TotalValue + GMCValues.ChartLabelOffsetBottom);
            GraphBottomValLabel.ToolTip = new ToolTip { Content = GraphBottomValLabel.Content };

            double TransformX = GraphCanvas.ActualWidth / (int)TimeElementsCombobox.SelectedValue;
            double TransformY = GraphCanvas.ActualHeight / (TotalValue * GMCValues.GraphYTransformModifier);
            double YOffset = 0;
            if (GMCValues.GraphYOffsetModifier != 0)
                YOffset = GraphCanvas.ActualHeight / GMCValues.GraphYOffsetModifier;

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

                        if (GMCValues.GraphModeColumn == 0)
                        {
                            NewPolygon.Stroke = Values[i].GraphElements[j].BorderColor;
                            NewPolygon.Fill = Values[i].GraphElements[j].FillColor;

                            AddYA = PosDirOffsetY;
                            AddYB = PosDirOffsetY + Values[i].GraphElements[j].Value[GMCValues.ColumnChart_UseIndexIs];
                            AddYC = PosDirOffsetY + Values[i].GraphElements[j].Value[GMCValues.ColumnChart_UseIndexIs];
                            AddYD = PosDirOffsetY;

                            PosDirOffsetY += Values[i].GraphElements[j].Value[GMCValues.ColumnChart_UseIndexIs];

                            tooltip = new ToolTip { Content = Values[i].GraphElements[j].Name + ": " + Values[i].GraphElements[j].Value[GMCValues.ColumnChart_UseIndexIs] + " ( " + SumOfList(Values[i], GMCValues.ColumnChart_UseIndexIs) + " ) ( " + Values[i].TimeTable + " )" };
                        }
                        if (GMCValues.GraphModeColumn == 1)
                        {
                            NewPolygon.Stroke = Values[i].GraphElements[j].BorderColor;
                            NewPolygon.Fill = Values[i].GraphElements[j].FillColor;
                            if (Values[i].GraphElements[j].Value[GMCValues.ColumnChart_UseIndexIs] > 0)
                            {
                                AddYA = PosDirOffsetY;
                                AddYB = PosDirOffsetY + Values[i].GraphElements[j].Value[GMCValues.ColumnChart_UseIndexIs];
                                AddYC = PosDirOffsetY + Values[i].GraphElements[j].Value[GMCValues.ColumnChart_UseIndexIs];
                                AddYD = PosDirOffsetY;

                                PosDirOffsetY += Values[i].GraphElements[j].Value[GMCValues.ColumnChart_UseIndexIs];
                            }
                            else
                            {
                                AddYA = NegDirOffsetY;
                                AddYB = NegDirOffsetY + Values[i].GraphElements[j].Value[GMCValues.ColumnChart_UseIndexIs];
                                AddYC = NegDirOffsetY + Values[i].GraphElements[j].Value[GMCValues.ColumnChart_UseIndexIs];
                                AddYD = NegDirOffsetY;

                                NegDirOffsetY += Values[i].GraphElements[j].Value[GMCValues.ColumnChart_UseIndexIs];
                            }
                            tooltip = new ToolTip { Content = Values[i].GraphElements[j].Name + ": " + Values[i].GraphElements[j].Value[GMCValues.ColumnChart_UseIndexIs] + " ( " + SumOfList(Values[i], GMCValues.ColumnChart_UseIndexIs) + " ) ( " + Values[i].TimeTable + " )" };
                        }
                        if (GMCValues.GraphModeColumn == 2)
                        {
                            NewPolygon.Stroke = Brushes.White;
                            NewPolygon.Fill = Brushes.Transparent;
                            AddYA = SumOfRange(Values[i], 0, Values[i].GraphElements.Count, GMCValues.ColumnChart_UseIndexIs);
                            AddYB = AddYA;
                            AddYC = SumOfRange(Values[i + 1], 0, Values[i].GraphElements.Count, GMCValues.ColumnChart_UseIndexIs);
                            AddYD = AddYC;

                            tooltip = new ToolTip { Content = SumOfRange(Values[i], 0, Values[i].GraphElements.Count, GMCValues.ColumnChart_UseIndexIs) + " ( " + Values[i].TimeTable + " )" };
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

        void UpdatePieChart(List<GI.GraphColunm> Values, double TotalValue)
        {
            PieGraphCanvas.Children.Clear();

            if (GMCValues.IsPieChartEnabled)
            {
                PieGraphLabel.Visibility = Visibility.Hidden;
                double OffSetAngle = 359;
                List<GI.GraphElement> SortedList = new List<GI.GraphElement>(Values[0].GraphElements);

                SortedList.Sort((s1, s2) => s1.Value[GMCValues.PieChart_UseIndexIs].CompareTo(s2.Value[GMCValues.PieChart_UseIndexIs]));

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

                    ToolTip tooltip = new ToolTip { Content = SortedList[j].Name + ": " + (int)(Procent * 100) + "% ( " + Values[0].TimeTable + " )" };
                    Newpath.ToolTip = tooltip;

                    PieGraphCanvas.Children.Add(Newpath);
                }
            }
            else
                PieGraphLabel.Visibility = Visibility.Visible;
        }

        void UpdateItemsStatisticalData(List<GI.GraphColunm> GraphData, double TotalValue)
        {
            if (GraphData.Count > 0)
            {
                for (int i = 0; i < ItemStack.Children.Count; i++)
                {
                    try
                    {
                        ItemDesign SenderDesign = ItemStack.Children[i] as ItemDesign;
                        SenderDesign.UpdateStatisticals(GraphData, TotalValue, GMCValues.Statistical_UseIndexIs, GMCValues.IsPieChartEnabled, (int)TimeElementsCombobox.SelectedValue);
                    }
                    catch { }
                }
            }
        }

        #endregion

        #region Save and Load Region

                #endregion

        #region CommonElements

        double SumOfList(GI.GraphColunm Values, int _CurrentMode)
        {
            if (Values.GraphElements == null)
                return 0;

            double Moment = 0;
            for (int i = 0; i < Values.GraphElements.Count; i++)
                Moment += Values.GraphElements[i].Value[_CurrentMode];
            return Moment;
        }

        double SumOfRange(GI.GraphColunm _Input, int _From, int _To, int _CurrentMode)
        {
            if (_To > _Input.GraphElements.Count)
                _To = _Input.GraphElements.Count;
            double ReturnVal = 0;
            for (int i = _From; i < _To; i++)
                ReturnVal += _Input.GraphElements[i].Value[_CurrentMode];
            return ReturnVal;
        }

        async Task WaitUntil(int Index, List<GI.GraphColunm> GraphData)
        {
            while (!IsTimeOver(DateTime.Now, GraphData[0].TimeTable, Index))
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
                if (ClearGraphData || FlatlineGraphData || EqualizeColorsData || ExitSave || ManualRefresh)
                    break;
            }
        }

        double RunCommandList(int GraphModeIndex, List<GI.GMC> _Commands, HtmlNode Node, List<GI.GraphColunm> GraphData, int GraphDataIndex)
        {
            double ReturnValue = 0;
            try
            {
                for (int i = 0; i < _Commands.Count; i++)
                {
                    if (_Commands[i].Command == GI.GMCS.TotalValues)
                        ReturnValue = Int32.Parse(Node.InnerText.Replace(",", "").Replace(".", ""));

                    if (_Commands[i].Command == GI.GMCS.AVRChange)
                        if (GraphData[1].GraphElements.Count != 0)
                            ReturnValue = Int32.Parse(Node.InnerText.Replace(",", "").Replace(".", "")) - GraphData[1].GraphElements[GraphDataIndex].Value[0];

                    if (_Commands[i].Command == GI.GMCS.EnablePieChart)
                        GMCValues.IsPieChartEnabled = true;

                    if (_Commands[i].Command == GI.GMCS.DisablePieChart)
                        GMCValues.IsPieChartEnabled = false;

                    if (_Commands[i].Command == GI.GMCS.Statistical_UseIndex)
                    {
                        GMCValues.Statistical_UseIndexIs = _Commands[i].OverloadInt;
                    }

                    if (_Commands[i].Command == GI.GMCS.PieChart_UseIndex)
                    {
                        GMCValues.PieChart_UseIndexIs = _Commands[i].OverloadInt;
                    }

                    if (_Commands[i].Command == GI.GMCS.ColumnChart_UseIndex)
                    {
                        GMCValues.ColumnChart_UseIndexIs = _Commands[i].OverloadInt;
                    }

                    if (_Commands[i].Command == GI.GMCS.General_UseIndex)
                    {
                        GMCValues.General_UseIndexIs = _Commands[i].OverloadInt;
                    }

                    if (_Commands[i].Command == GI.GMCS.SetChartLabels)
                    {
                        GMCValues.ChartLabelOffsetTop = _Commands[i].OverloadInt;
                        GMCValues.ChartLabelOffSetTopModifier = _Commands[i].Modifier;
                        GMCValues.ChartLabelOffsetBottom = _Commands[i].OverloadInt2;
                        GMCValues.ChartLabelOffSetBottomModifier = _Commands[i].Modifier2;
                        GMCValues.GraphYOffsetModifier = _Commands[i].OverloadInt3;
                        GMCValues.GraphYTransformModifier = _Commands[i].Modifier3;
                    }

                    if (_Commands[i].Command == GI.GMCS.SetGraphMode)
                    {
                        GMCValues.GraphModeColumn = _Commands[i].OverloadInt;
                    }

                    if (_Commands[i].Command == GI.GMCS.DisplayGraphMiddleLine)
                    {
                        GMCValues.DisplayGraphMiddleLine = true;
                    }

                    if (_Commands[i].Command == GI.GMCS.HideGraphMiddleLine)
                    {
                        GMCValues.DisplayGraphMiddleLine = false;
                    }
                }
            }
            catch
            {
                ReturnValue = 0;
            }
            return ReturnValue;
        }

        bool IsTimeOver(DateTime BaseData, DateTime CompareData, int Index)
        {
            return  (Index == 0 && (BaseData - CompareData).Days >= 1) ||
                    (Index == 1 && (BaseData - CompareData).Hours >= 1) ||
                    (Index == 2 && (BaseData - CompareData).Minutes >= 1) ||
                    (Index == 3 && (BaseData - CompareData).Seconds >= 1);
        }

        #endregion
    }
}
