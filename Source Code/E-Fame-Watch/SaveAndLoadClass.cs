using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace E_Fame_Watch
{
    class SaveAndLoadClass
    {
        public static void SaveSettings(List<GI.GraphColunm> GraphData, MainWindow SenderWindow, int DataCacheSize, int CurrentModeIndex, int CurrentThemeIndex)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("cfg.txt"))
                {
                    file.WriteLine(Application.Current.MainWindow.Left + ";" + Application.Current.MainWindow.Top + ";");
                    file.WriteLine(SenderWindow.TimeFrameCombobox.SelectedIndex + ";" + SenderWindow.TimeElementsCombobox.SelectedIndex + ";");
                    file.WriteLine(CurrentModeIndex + ";" + CurrentThemeIndex + ";");
                    for (int i = 0; i < SenderWindow.ItemStack.Children.Count; i++)
                    {
                        ItemDesign SenderDesign = SenderWindow.ItemStack.Children[i] as ItemDesign;
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

                                if (GraphData[i].GraphElements[j].BorderColor.ToString() == GraphData[i - 1].GraphElements[j].BorderColor.ToString())
                                {
                                    WriteString += "^;";
                                }
                                else
                                {
                                    WriteString += GraphData[i].GraphElements[j].BorderColor + ";";
                                }

                                if (GraphData[i].GraphElements[j].FillColor.ToString() == GraphData[i - 1].GraphElements[j].FillColor.ToString())
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

        public static void LoadSettings(List<GI.GraphColunm> GraphData, MainWindow SenderWindow, int DataCacheSize, int GraphModeCount)
        {
            try
            {
                if (System.IO.File.Exists("cfg.txt"))
                {
                    GraphData.Clear();
                    for (int i = 0; i < DataCacheSize; i++)
                    {
                        GraphData.Add(new GI.GraphColunm(DateTime.Now, new List<GI.GraphElement>()));
                    }

                    string[] lines = System.IO.File.ReadAllLines("cfg.txt");

                    Application.Current.MainWindow.Left = Int32.Parse(lines[0].Split(';')[0]);
                    Application.Current.MainWindow.Top = Int32.Parse(lines[0].Split(';')[1]);

                    SenderWindow.TimeFrameCombobox.SelectedIndex = Int32.Parse(lines[1].Split(';')[0]);
                    SenderWindow.TimeElementsCombobox.SelectedIndex = Int32.Parse(lines[1].Split(';')[1]);

                    SenderWindow.CurrentGraphMode = Int32.Parse(lines[2].Split(';')[0]);
                    SenderWindow.CurrentThemeIndex = Int32.Parse(lines[2].Split(';')[1]);

                    int InnerIndex = 0;

                    for (int i = 3; i < lines.Length; i++)
                    {
                        string[] Split = lines[i].Split(';');
                        if (Split[0] == "I")
                        {
                            GI.MakeNewItemPanel(SenderWindow, Split[1], Split[2], Split[3], (SolidColorBrush)(new BrushConverter().ConvertFrom(Split[4])), (SolidColorBrush)(new BrushConverter().ConvertFrom(Split[5])), true);
                        }
                        if (Split[0] == "D")
                        {
                            GI.GraphColunm NewData = new GI.GraphColunm();
                            NewData.TimeTable = DateTime.Parse(Split[1]);
                            NewData.GraphElements = new List<GI.GraphElement>();
                            int Count = 0;
                            for (int j = 2; j < Split.Length; j++)
                            {
                                if (Split[j] == "S")
                                {
                                    GI.GraphElement NewElement = new GI.GraphElement();
                                    if (Split[j + 2] == "^")
                                    {
                                        NewElement.Value = GraphData[InnerIndex - 1].GraphElements[Count].Value;
                                    }
                                    else
                                    {
                                        NewElement.Value = SplitString(':', Split[j + 2], GraphModeCount);
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
                }
            }
            catch
            {
                MessageBox.Show("Could not load cfg");
            }
        }

        private static double[] SplitString(char Delimiter, string Input, int DoubleListSize)
        {
            string[] SplitString = Input.Split(Delimiter);
            double[] DoubleList = new double[DoubleListSize];
            for (int i = 0; i < DoubleListSize; i++)
            {
                if (i > DoubleList.Length - 1)
                    break;
                DoubleList[i] = Convert.ToDouble(SplitString[i]);
            }
            return DoubleList;
        }

        private static string DoubleArrayToString(string Delimiter, double[] Input)
        {
            string OutString = "";
            for (int i = 0; i < Input.Length; i++)
                OutString += Input[i] + Delimiter;
            return OutString;
        }
    }
}
