using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace E_Fame_Watch
{
    // General Interface
    public class GI
    {
        public struct ThemeInfo
        {
            public string Name;
            public string URI;

            public ThemeInfo(string _Name, string _URI)
            {
                Name = _Name;
                URI = _URI;
            }
        }

        public struct GraphElement
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

        public struct GraphColunm
        {
            public DateTime TimeTable;
            public List<GraphElement> GraphElements;

            public GraphColunm(DateTime _TimeTable, List<GraphElement> _GraphElements)
            {
                TimeTable = _TimeTable;
                GraphElements = _GraphElements;
            }
        }

        //Graph Mode CommandS : GMCS
        public enum GMCS { TotalValues, AVRChange, DisablePieChart, EnablePieChart, PieChart_UseIndex, ColumnChart_UseIndex, Statistical_UseIndex, General_UseIndex, SetChartLabels, SetGraphMode, DisplayGraphMiddleLine, HideGraphMiddleLine };

        //Graph Mode Command : GMC
        public struct GMC
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

        public struct GraphMode
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

        //Graph Mode Command Values : GMCValues
        public struct GMCValues
        {
            public bool IsPieChartEnabled;
            public int Statistical_UseIndexIs;
            public int PieChart_UseIndexIs;
            public int ColumnChart_UseIndexIs;
            public int General_UseIndexIs;
            public int ChartLabelOffsetTop;
            public int ChartLabelOffSetTopModifier;
            public int ChartLabelOffsetBottom;
            public int ChartLabelOffSetBottomModifier;
            public int GraphModeColumn;
            public int GraphYOffsetModifier;
            public int GraphYTransformModifier;
            public bool DisplayGraphMiddleLine;

            public GMCValues(
                bool _IsPieChartEnabled = true, 
                int _Statistical_UseIndexIs = 0,
                int _PieChart_UseIndexIs = 0,
                int _ColumnChart_UseIndexIs = 0,
                int _General_UseIndexIs = 0,
                int _ChartLabelOffsetTop = 0,
                int _ChartLabelOffSetTopModifier = 1,
                int _ChartLabelOffsetBottom = 0,
                int _ChartLabelOffSetBottomModifier = 1,
                int _GraphModeColumn = 0,
                int _GraphYOffsetModifier = 0,
                int _GraphYTransformModifier = 1,
                bool _DisplayGraphMiddleLine = false)
            {
                IsPieChartEnabled = _IsPieChartEnabled;
                Statistical_UseIndexIs = _Statistical_UseIndexIs;
                PieChart_UseIndexIs = _PieChart_UseIndexIs;
                ColumnChart_UseIndexIs = _ColumnChart_UseIndexIs;
                General_UseIndexIs = _General_UseIndexIs;
                ChartLabelOffsetTop = _ChartLabelOffsetTop;
                ChartLabelOffSetTopModifier = _ChartLabelOffSetTopModifier;
                ChartLabelOffsetBottom = _ChartLabelOffsetBottom;
                ChartLabelOffSetBottomModifier = _ChartLabelOffSetBottomModifier;
                GraphModeColumn = _GraphModeColumn;
                GraphYOffsetModifier = _GraphYOffsetModifier;
                GraphYTransformModifier = _GraphYTransformModifier;
                DisplayGraphMiddleLine = _DisplayGraphMiddleLine;
            }
        }

        public static Brush GetColorFromPosNegNeuValue(double Value, Brush PositiveColor, Brush NegativeColor, Brush NeutralColor)
        {
            if (Value > 0)
                return PositiveColor;
            if (Value < 0)
                return NegativeColor;
            return NeutralColor;
        }


        public static async Task ExpandWindow(MainWindow SenderWindow,bool ExpandOrRectract)
        {
            if (ExpandOrRectract)
            {
                bool Expand = false;
                if (SenderWindow.Clip.Bounds.Width == 260)
                {
                    Expand = true;
                }

                if (Expand)
                {
                    for (int i = 260; i < 520; i += 20)
                    {
                        SenderWindow.Clip = new RectangleGeometry { Rect = new Rect(0, 0, i, SenderWindow.Clip.Bounds.Height) };
                        await Task.Delay(1);
                    }
                    SenderWindow.Clip = new RectangleGeometry { Rect = new Rect(0, 0, 520, SenderWindow.Clip.Bounds.Height) };
                }
            }
            else
            {
                bool Retract = false;
                if (SenderWindow.Clip.Bounds.Width == 520)
                {
                    Retract = true;
                }

                if (Retract)
                {
                    for (int i = 520; i >= 260; i -= 20)
                    {
                        SenderWindow.Clip = new RectangleGeometry { Rect = new Rect(0, 0, i, SenderWindow.Clip.Bounds.Height) };
                        await Task.Delay(1);
                    }
                    SenderWindow.Clip = new RectangleGeometry { Rect = new Rect(0, 0, 260, SenderWindow.Clip.Bounds.Height) };
                }
            }
        }


        public static async Task FadeIn(UIElement FadeElement)
        {
            for (int i = 0; i <= 100; i += 5)
            {
                FadeElement.Opacity = ((double)i / (double)100);
                await Task.Delay(10);
            }
            FadeElement.Opacity = 1;
        }

        public static async Task FadeOut(UIElement FadeElement)
        {
            for (int i = 100; i >= 0; i -= 5)
            {
                FadeElement.Opacity = ((double)i / (double)100);
                await Task.Delay(10);
            }
            FadeElement.Opacity = 0;
        }

        public static void MakeNewItemPanel(MainWindow SenderWindow, string _Name, string _URL, string _XPath, SolidColorBrush _BorderColor, SolidColorBrush _FillColor, bool _Minimized)
        {
            ItemDesign NewDesign = new ItemDesign(SenderWindow);

            if (_Minimized)
            {
                NewDesign.Height = 34;
                if (SenderWindow.Clip.Bounds.Height + 34 < 1000)
                    SenderWindow.Clip = new RectangleGeometry { Rect = new Rect(0, 0, SenderWindow.Clip.Bounds.Width, SenderWindow.Clip.Bounds.Height + 34) };
            }
            else
            {
                NewDesign.Height = 190;
                if (SenderWindow.Clip.Bounds.Height + 184 < 1000)
                    SenderWindow.Clip = new RectangleGeometry { Rect = new Rect(0, 0, SenderWindow.Clip.Bounds.Width, SenderWindow.Clip.Bounds.Height + 184) };
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

            SenderWindow.ItemStack.Children.Add(NewDesign);
        }
    }
}
