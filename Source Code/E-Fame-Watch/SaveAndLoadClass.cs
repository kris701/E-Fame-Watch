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
                FI SaveConfigs = new FI();
                SaveConfigs.ItemStack.Add(
                    new FI.ValueCategory("MainWindow Data", new List<FI.FIItems>() {
                    new FI.IntValue("Location X", (int)Application.Current.MainWindow.Left),
                    new FI.IntValue("Location Y", (int)Application.Current.MainWindow.Top),
                    new FI.IntValue("TimeFrame Index", SenderWindow.TimeFrameCombobox.SelectedIndex),
                    new FI.IntValue("Time Elements Index", SenderWindow.TimeElementsCombobox.SelectedIndex),
                    new FI.IntValue("Current Mode", CurrentModeIndex),
                    new FI.IntValue("Theme Index", CurrentThemeIndex)
                    }));

                for (int i = 0; i < SenderWindow.ItemStack.Children.Count; i++)
                {
                    ItemDesign SenderDesign = SenderWindow.ItemStack.Children[i] as ItemDesign;
                    SaveConfigs.ItemStack.Add(
                    new FI.ValueCategory("Item", new List<FI.FIItems>() {
                        new FI.StringValue("Name", SenderDesign.ItemNameTextBox.Text),
                        new FI.StringValue("URL", SenderDesign.ItemURLTextBox.Text),
                        new FI.StringValue("XPath", SenderDesign.ItemXPathTextBox.Text),
                        new FI.BrushValue("BorderColor", (SolidColorBrush)SenderDesign.ItemBorderColorButton.Background),
                        new FI.BrushValue("FillColor", (SolidColorBrush)SenderDesign.ItemFillColorButton.Background)
                    }));
                }

                for (int i = 0; i < DataCacheSize; i++)
                {
                    FI.ValueCategory MomentCat = new FI.ValueCategory("HistoricData", new List<FI.FIItems>());
                    MomentCat.ValueStack.Add(new FI.DateTimeValue("Timestamp", GraphData[i].TimeTable));

                    for (int j = 0; j < GraphData[i].GraphElements.Count; j++)
                    {
                        MomentCat.ValueStack.Add(new FI.ListValue("", ""));
                        MomentCat.ValueStack.Add(new FI.StringValue("Name", GraphData[i].GraphElements[j].Name));
                        MomentCat.ValueStack.Add(new FI.IntValue("Value1", (int)GraphData[i].GraphElements[j].Value[0]));
                        MomentCat.ValueStack.Add(new FI.IntValue("Value2", (int)GraphData[i].GraphElements[j].Value[1]));
                        MomentCat.ValueStack.Add(new FI.BrushValue("BorderColor", GraphData[i].GraphElements[j].BorderColor));
                        MomentCat.ValueStack.Add(new FI.BrushValue("FillColor", GraphData[i].GraphElements[j].FillColor));
                        MomentCat.ValueStack.Add(new FI.EndListValue("", ""));
                    }

                    SaveConfigs.ItemStack.Add(MomentCat);
                }

                SaveConfigs.SaveToFile("cfg.txt");
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
                FI SaveConfigs = new FI();
                SaveConfigs.LoadFromFile("cfg.txt");
                FI.CategoryData CurrentCategory = new FI.CategoryData("MainWindow Data", 0);
                Application.Current.MainWindow.Left = SaveConfigs.FindtItemInItemstack_INT(CurrentCategory, new FI.ItemData("Location X", 0));
                Application.Current.MainWindow.Top = SaveConfigs.FindtItemInItemstack_INT(CurrentCategory, new FI.ItemData("Location Y", 0));
                SenderWindow.TimeFrameCombobox.SelectedIndex = SaveConfigs.FindtItemInItemstack_INT(CurrentCategory, new FI.ItemData("TimeFrame Index", 0));
                SenderWindow.TimeElementsCombobox.SelectedIndex = SaveConfigs.FindtItemInItemstack_INT(CurrentCategory, new FI.ItemData("Time Elements Index", 0));
                SenderWindow.CurrentGraphMode = SaveConfigs.FindtItemInItemstack_INT(CurrentCategory, new FI.ItemData("Current Mode", 0));
                SenderWindow.CurrentThemeIndex = SaveConfigs.FindtItemInItemstack_INT(CurrentCategory, new FI.ItemData("Theme Index", 0));

                CurrentCategory = new FI.CategoryData("Item", 0);
                while (SaveConfigs.IsAnyMoreOfCat(CurrentCategory))
                {
                    GI.MakeNewItemPanel(
                        SenderWindow,
                        SaveConfigs.FindtItemInItemstack_STR(CurrentCategory, new FI.ItemData("Name", 0)),
                        SaveConfigs.FindtItemInItemstack_STR(CurrentCategory, new FI.ItemData("URL", 0)),
                        SaveConfigs.FindtItemInItemstack_STR(CurrentCategory, new FI.ItemData("XPath", 0)),
                        (SolidColorBrush)SaveConfigs.FindtItemInItemstack_BRS(CurrentCategory, new FI.ItemData("BorderColor", 0)),
                        (SolidColorBrush)SaveConfigs.FindtItemInItemstack_BRS(CurrentCategory, new FI.ItemData("FillColor", 0)),
                        true
                    );

                    CurrentCategory.Offset++;
                }

                CurrentCategory = new FI.CategoryData("HistoricData", 0);
                while (SaveConfigs.IsAnyMoreOfCat(CurrentCategory))
                {
                    GI.GraphColunm NewData = new GI.GraphColunm();
                    NewData.TimeTable = SaveConfigs.FindtItemInItemstack_DAT(CurrentCategory, new FI.ItemData("Timestamp", 0));
                    NewData.GraphElements = new List<GI.GraphElement>();

                    FI.ItemData MomentItem = new FI.ItemData("Name", 0);
                    while (SaveConfigs.IsAnyMoreWithinCat(CurrentCategory, MomentItem))
                    {
                        GI.GraphElement NewElement = new GI.GraphElement();
                        NewElement.Name = SaveConfigs.FindtItemInItemstack_STR(CurrentCategory, MomentItem);
                        double[] MomentValues = new double[2];
                        MomentValues[0] = SaveConfigs.FindtItemInItemstack_INT(CurrentCategory, new FI.ItemData("Value1", MomentItem.Offset));
                        MomentValues[1] = SaveConfigs.FindtItemInItemstack_INT(CurrentCategory, new FI.ItemData("Value2", MomentItem.Offset));
                        NewElement.Value = MomentValues;
                        NewElement.BorderColor = (SolidColorBrush)SaveConfigs.FindtItemInItemstack_BRS(CurrentCategory, new FI.ItemData("BorderColor", MomentItem.Offset));
                        NewElement.FillColor = (SolidColorBrush)SaveConfigs.FindtItemInItemstack_BRS(CurrentCategory, new FI.ItemData("FillColor", MomentItem.Offset));
                        NewData.GraphElements.Add(NewElement);

                        MomentItem.Offset++;
                    }

                    GraphData[CurrentCategory.Offset] = NewData;
                    CurrentCategory.Offset++;
                }
            }
            catch
            {
                MessageBox.Show("Could not load cfg");
                Application.Current.Shutdown();
            }
        }
    }
}
