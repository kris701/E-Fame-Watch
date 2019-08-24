using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Fame_Watch
{
    public class FI : FIVItems
    {
        #region Structs

        public struct ValueCategory
        {
            public string Name;
            public List<FIItems> ValueStack;

            public ValueCategory(string _Name, List<FIItems> _ItemStack)
            {
                Name = _Name;
                ValueStack = _ItemStack;
            }
        }

        struct ItemPoint
        {
            public int X;
            public int Y;

            public ItemPoint(int _X, int _Y)
            {
                X = _X;
                Y = _Y;
            }
        }

        public struct CategoryData
        {
            public string Name;
            public int Offset;

            public CategoryData(string _Name, int _Offset)
            {
                Name = _Name;
                Offset = _Offset;
            }
        }

        public struct ItemData
        {
            public string Name;
            public int Offset;

            public ItemData(string _Name, int _Offset)
            {
                Name = _Name;
                Offset = _Offset;
            }
        }

        #endregion

        /// <summary>
        /// This is the buffer for all the items to be saved or loaded. This should be loaded with items before calling SaveToFile. 
        /// Gets emptied before loading.
        /// </summary>
        public List<ValueCategory> ItemStack = new List<ValueCategory>();

        #region Private Internal Functions

        private string FindAnyRefrence(FIItems Item, int SenderI, int SenderJ)
        {
            if (Item.GetValueLength() > 6)
            {
                for (int i = 0; i <= SenderI; i++)
                {
                    for (int j = 0; j < ItemStack[i].ValueStack.Count; j++)
                    {
                        if (SenderI == i && j > SenderJ)
                            break;

                        if (!(j == SenderJ && SenderI == i))
                            if (ItemStack[i].ValueStack[j].GetValue() == Item.GetValue())
                                return "R[" + i + "." + j + "]";
                    }
                }
            }
            return Item.GetValue();
        }

        private string GetValueFromDataPoint(string DataPoint)
        {
            string[] CutData = DataPoint.Replace("R[", "").Replace("]", "").Split('.');
            int IVal = Int32.Parse(CutData[0]);
            int JVal = Int32.Parse(CutData[1]);
            if (IVal < ItemStack.Count)
            {
                if (JVal < ItemStack[IVal].ValueStack.Count)
                {
                    return ItemStack[IVal].ValueStack[JVal].GetValue();
                }
            }
            return "ERR";
        }

        private string CutStringTillData(string Input)
        {
            return Input.Substring(Input.IndexOf(" = ") + 3);
        }

        private ItemPoint FindIndexOfCategory(string Cat, string Name, int Offset, int ItemOffset)
        {
            int InnerCount = 0;
            int InnerCount2 = 0;
            for (int i = 0; i < ItemStack.Count; i++)
            {
                if (ItemStack[i].Name == Cat)
                {
                    if (InnerCount == Offset)
                    {
                        for (int j = 0; j < ItemStack[i].ValueStack.Count; j++)
                        {
                            if (ItemStack[i].ValueStack[j].GetName() == Name)
                            {
                                if (InnerCount2 == ItemOffset)
                                {
                                    return new ItemPoint(i, j);
                                }
                                InnerCount2++;
                            }
                        }
                    }
                    InnerCount++;
                }
            }
            return new ItemPoint(-1, -1);
        }

        #endregion

        #region Public Function

        /// <summary>
        /// Loads a ItemStack from the file location
        /// </summary>
        /// <param string="FileLoc"></param>
        public void LoadFromFile(string FileLoc)
        {
            try
            {
                if (System.IO.File.Exists(FileLoc))
                {
                    string[] lines = System.IO.File.ReadAllLines(FileLoc);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("( ["))
                        {
                            ValueCategory NewCat = new ValueCategory();
                            NewCat.Name = lines[i].Replace(lines[i].Substring(0, lines[i].IndexOf(" : ") + 3), "").Replace(" ) { ", "");
                            NewCat.ValueStack = new List<FIItems>();
                            ItemStack.Add(NewCat);

                            int InnerI = i + 1;
                            while (lines[InnerI] != "}")
                            {
                                string ValueType = lines[InnerI].Replace(lines[InnerI].Substring(0, lines[InnerI].IndexOf("(") + 1), "").Substring(0, lines[InnerI].Replace(lines[InnerI].Substring(0, lines[InnerI].IndexOf("(") + 1), "").IndexOf(")"));

                                FIItems NewItem = new StringValue("", "");

                                if (ValueType == ValueTypesStrings[(int)ValueTypesEnum.STR])
                                {
                                    NewItem = new StringValue(
                                        lines[InnerI].Replace(lines[InnerI].Substring(0, lines[InnerI].IndexOf(":") + 2), "").Substring(0, lines[InnerI].Replace(lines[InnerI].Substring(0, lines[InnerI].IndexOf(":") + 2), "").IndexOf(" = ")), "");
                                }
                                if (ValueType == ValueTypesStrings[(int)ValueTypesEnum.INT])
                                {
                                    NewItem = new IntValue(
                                        lines[InnerI].Replace(lines[InnerI].Substring(0, lines[InnerI].IndexOf(":") + 2), "").Substring(0, lines[InnerI].Replace(lines[InnerI].Substring(0, lines[InnerI].IndexOf(":") + 2), "").IndexOf(" = ")), 0);
                                }
                                if (ValueType == ValueTypesStrings[(int)ValueTypesEnum.BOL])
                                {
                                    NewItem = new BoolValue(
                                        lines[InnerI].Replace(lines[InnerI].Substring(0, lines[InnerI].IndexOf(":") + 2), "").Substring(0, lines[InnerI].Replace(lines[InnerI].Substring(0, lines[InnerI].IndexOf(":") + 2), "").IndexOf(" = ")), false);
                                }
                                if (ValueType == ValueTypesStrings[(int)ValueTypesEnum.BRS])
                                {
                                    NewItem = new BrushValue(
                                        lines[InnerI].Replace(lines[InnerI].Substring(0, lines[InnerI].IndexOf(":") + 2), "").Substring(0, lines[InnerI].Replace(lines[InnerI].Substring(0, lines[InnerI].IndexOf(":") + 2), "").IndexOf(" = ")), System.Windows.Media.Brushes.White);
                                }
                                if (ValueType == ValueTypesStrings[(int)ValueTypesEnum.DAT])
                                {
                                    NewItem = new DateTimeValue(
                                        lines[InnerI].Replace(lines[InnerI].Substring(0, lines[InnerI].IndexOf(":") + 2), "").Substring(0, lines[InnerI].Replace(lines[InnerI].Substring(0, lines[InnerI].IndexOf(":") + 2), "").IndexOf(" = ")), System.DateTime.Now);
                                }
                                if (ValueType == ValueTypesStrings[(int)ValueTypesEnum.LST])
                                {
                                    NewItem = new ListValue("", "");
                                }
                                else
                                {
                                    if (ValueType == ValueTypesStrings[(int)ValueTypesEnum.ELS])
                                    {
                                        NewItem = new EndListValue("", "");
                                    }
                                    else
                                    {
                                        if (CutStringTillData(lines[InnerI]).StartsWith("R["))
                                        {
                                            NewItem.SetValue(GetValueFromDataPoint(CutStringTillData(lines[InnerI])));
                                        }
                                        else
                                        {
                                            NewItem.SetValue(CutStringTillData(lines[InnerI]));
                                        }
                                    }
                                }

                                if (NewItem.GetName() != "" && NewItem.GetValue() != "")
                                    NewCat.ValueStack.Add(NewItem);

                                InnerI++;
                            }
                            i = InnerI;
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Saves the current ItemStack at a given file location
        /// </summary>
        /// <param string="FileLoc"></param>
        public void SaveToFile(string FileLoc)
        {
            try
            {
                string ListOffset = "";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(FileLoc))
                {
                    for (int i = 0; i < ItemStack.Count; i++)
                    {
                        file.WriteLine("( [" + i + "] : " + ItemStack[i].Name + " ) { ");
                        for (int j = 0; j < ItemStack[i].ValueStack.Count; j++)
                        {
                            if (ItemStack[i].ValueStack[j].GetValueType() == ValueTypesStrings[(int)ValueTypesEnum.LST])
                            {
                                file.WriteLine(" (" + ItemStack[i].ValueStack[j].GetValueType() + ")");
                                ListOffset += " ";
                            }
                            else
                            {
                                if (ItemStack[i].ValueStack[j].GetValueType() == ValueTypesStrings[(int)ValueTypesEnum.ELS])
                                {
                                    file.WriteLine(" (" + ItemStack[i].ValueStack[j].GetValueType() + ")");
                                    ListOffset = ListOffset.Substring(0, ListOffset.Length - 1);
                                }
                                else
                                {
                                    file.WriteLine(ListOffset + " (" + ItemStack[i].ValueStack[j].GetValueType() + ")" + " [" + j + "] : " + ItemStack[i].ValueStack[j].GetName() + " = " + FindAnyRefrence(ItemStack[i].ValueStack[j], i, j));
                                }
                            }
                        }
                        file.WriteLine("}");
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Returns a string if found, else returns an empty string
        /// </summary>
        /// <param CategoryData="CatData"></param>
        /// <param ItemData="ItemDat"></param>
        /// <returns></returns>
        public string FindtItemInItemstack_STR(CategoryData CatData, ItemData ItemDat)
        {
            ItemPoint MomentPoint = FindIndexOfCategory(CatData.Name, ItemDat.Name, CatData.Offset, ItemDat.Offset);
            if (MomentPoint.X != -1)
                if (MomentPoint.Y != -1)
                    return ItemStack[MomentPoint.X].ValueStack[MomentPoint.Y].GetValue();
            return "";
        }

        /// <summary>
        /// Returns a bool if found, else returns false
        /// </summary>
        /// <param CategoryData="CatData"></param>
        /// <param ItemData="ItemDat"></param>
        /// <returns></returns>
        public bool FindtItemInItemstack_BOL(CategoryData CatData, ItemData ItemDat)
        {
            ItemPoint MomentPoint = FindIndexOfCategory(CatData.Name, ItemDat.Name, CatData.Offset, ItemDat.Offset);
            if (MomentPoint.X != -1)
                if (MomentPoint.Y != -1)
                    return Convert.ToBoolean(ItemStack[MomentPoint.X].ValueStack[MomentPoint.Y].GetValue());
            return false;
        }

        /// <summary>
        /// Returns a int if found, else returns 0
        /// </summary>
        /// <param CategoryData="CatData"></param>
        /// <param ItemData="ItemDat"></param>
        /// <returns></returns>
        public int FindtItemInItemstack_INT(CategoryData CatData, ItemData ItemDat)
        {
            ItemPoint MomentPoint = FindIndexOfCategory(CatData.Name, ItemDat.Name, CatData.Offset, ItemDat.Offset);
            if (MomentPoint.X != -1)
                if (MomentPoint.Y != -1)
                    return Int32.Parse(ItemStack[MomentPoint.X].ValueStack[MomentPoint.Y].GetValue());
            return 0;
        }
        
        /// <summary>
        /// Returns a Brush object if found, else returns a white brush.
        /// </summary>
        /// <param CategoryData="CatData"></param>
        /// <param ItemData="ItemDat"></param>
        /// <returns></returns>
        public System.Windows.Media.Brush FindtItemInItemstack_BRS(CategoryData CatData, ItemData ItemDat)
        {
            ItemPoint MomentPoint = FindIndexOfCategory(CatData.Name, ItemDat.Name, CatData.Offset, ItemDat.Offset);
            if (MomentPoint.X != -1)
                if (MomentPoint.Y != -1)
                    return (System.Windows.Media.SolidColorBrush)(new System.Windows.Media.BrushConverter().ConvertFrom(ItemStack[MomentPoint.X].ValueStack[MomentPoint.Y].GetValue()));
            return System.Windows.Media.Brushes.White;
        }

        /// <summary>
        /// Returns a DateTime object if found, else returns Datetime.Now
        /// </summary>
        /// <param CategoryData="CatData"></param>
        /// <param ItemData="ItemDat"></param>
        /// <returns></returns>
        public System.DateTime FindtItemInItemstack_DAT(CategoryData CatData, ItemData ItemDat)
        {
            ItemPoint MomentPoint = FindIndexOfCategory(CatData.Name, ItemDat.Name, CatData.Offset, ItemDat.Offset);
            if (MomentPoint.X != -1)
                if (MomentPoint.Y != -1)
                    return System.DateTime.Parse(ItemStack[MomentPoint.X].ValueStack[MomentPoint.Y].GetValue());
            return System.DateTime.Now;
        }

        /// <summary>
        /// Checks if there are any more of the given categorys. Only returns true if it is found higher than the CategoryData offset
        /// </summary>
        /// <param CategoryData="CatData"></param>
        /// <returns></returns>
        public bool IsAnyMoreOfCat(CategoryData CatData)
        {
            int InnerCount = 0;
            for (int i = 0; i < ItemStack.Count; i++)
            {
                if (ItemStack[i].Name == CatData.Name)
                {
                    if (InnerCount >= CatData.Offset)
                        return true;
                    InnerCount++;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if there are any more items within a category. Searches only at the selected category index. Only returns true if it is found higher than the ItemData offset
        /// </summary>
        /// <param CategoryData="CatData"></param>
        /// <param ItemData="ItemDat"></param>
        /// <returns></returns>
        public bool IsAnyMoreWithinCat(CategoryData CatData, ItemData ItemDat)
        {
            int InnerCount = 0;
            int InnerCount2 = 0;
            for (int i = 0; i < ItemStack.Count; i++)
            {
                if (ItemStack[i].Name == CatData.Name)
                {
                    if (InnerCount == CatData.Offset)
                    {
                        for (int j = 0; j < ItemStack[i].ValueStack.Count; j++)
                        {
                            if (ItemStack[i].ValueStack[j].GetName() == ItemDat.Name)
                            {
                                if (InnerCount2 >= ItemDat.Offset)
                                    return true;
                                InnerCount2++;
                            }
                        }
                    }
                    InnerCount++;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a value exist in a category with a item name. Searches only at the selected category index. Item offset is omitted
        /// </summary>
        /// <param CategoryData="CatData"></param>
        /// <param ItemData="ItemDat"></param>
        /// <param string="Value"></param>
        /// <returns></returns>
        public bool DoesValueExist(CategoryData CatData, ItemData ItemDat, string Value)
        {
            int InnerCount = 0;
            for (int i = 0; i < ItemStack.Count; i++)
            {
                if (ItemStack[i].Name == CatData.Name)
                {
                    if (InnerCount == CatData.Offset)
                    {
                        for (int j = 0; j < ItemStack[i].ValueStack.Count; j++)
                        {
                            if (ItemStack[i].ValueStack[j].GetName() == ItemDat.Name)
                            {
                                if (ItemStack[i].ValueStack[j].GetValue().ToUpper().Contains(Value.ToUpper()))
                                    return true;
                            }
                        }
                    }
                    InnerCount++;
                }
            }
            return false;
        }

        #endregion
    }

    /// <summary>
    /// A class with an interface to save, load and name data types
    /// </summary>
    public class FIVItems
    {
        // String, int, bool, brush, datetime, list start, list end
        public static readonly string[] ValueTypesStrings = { "STR", "INT", "BOL", "BRS", "DAT", "LST", "ELS" };
        public enum ValueTypesEnum { STR, INT, BOL, BRS, DAT, LST, ELS };

        /// <summary>
        /// Interface with value types
        /// </summary>
        public interface FIItems
        {
            int GetValueLength();
            string GetValue();
            string GetName();
            void SetValue(string Input);
            string GetValueType();
        }

        // Value types
        public class StringValue : FIItems
        {
            public ValueTypesEnum ValueTypeString;
            public string Name;
            public string Value;

            public StringValue(string _Name, string _Value)
            {
                ValueTypeString = ValueTypesEnum.STR;
                Name = _Name;
                Value = _Value;
            }

            int FIItems.GetValueLength()
            {
                return Value.Length;
            }

            string FIItems.GetValue()
            {
                return Value;
            }

            string FIItems.GetName()
            {
                return Name;
            }

            void FIItems.SetValue(string Input)
            {
                Value = Input;
            }

            string FIItems.GetValueType()
            {
                return ValueTypesStrings[(int)ValueTypeString];
            }
        }

        public class IntValue : FIItems
        {
            public ValueTypesEnum ValueTypeString;
            public string Name;
            public int Value;

            public IntValue(string _Name, int _Value)
            {
                ValueTypeString = ValueTypesEnum.INT;
                Name = _Name;
                Value = _Value;
            }

            int FIItems.GetValueLength()
            {
                if (Value == 0)
                    return 1;
                return (int)Math.Floor(Math.Log10(Value)) + 1;
            }

            string FIItems.GetValue()
            {
                return Value.ToString();
            }

            string FIItems.GetName()
            {
                return Name;
            }
            void FIItems.SetValue(string Input)
            {
                Value = Int32.Parse(Input);
            }
            string FIItems.GetValueType()
            {
                return ValueTypesStrings[(int)ValueTypeString];
            }
        }

        public class BoolValue : FIItems
        {
            public ValueTypesEnum ValueTypeString;
            public string Name;
            public bool Value;

            public BoolValue(string _Name, bool _Value)
            {
                ValueTypeString = ValueTypesEnum.BOL;
                Name = _Name;
                Value = _Value;
            }

            int FIItems.GetValueLength()
            {
                if (Value == true)
                    return 4;
                return 5;
            }

            string FIItems.GetValue()
            {
                return Value.ToString();
            }

            string FIItems.GetName()
            {
                return Name;
            }
            void FIItems.SetValue(string Input)
            {
                Value = Convert.ToBoolean(Input);
            }
            string FIItems.GetValueType()
            {
                return ValueTypesStrings[(int)ValueTypeString];
            }
        }

        public class BrushValue : FIItems
        {
            public ValueTypesEnum ValueTypeString;
            public string Name;
            public System.Windows.Media.Brush Value;

            public BrushValue(string _Name, System.Windows.Media.Brush _Value)
            {
                ValueTypeString = ValueTypesEnum.BRS;
                Name = _Name;
                Value = _Value;
            }

            int FIItems.GetValueLength()
            {
                return ((System.Windows.Media.SolidColorBrush)Value).Color.ToString().Length;
            }

            string FIItems.GetValue()
            {
                return ((System.Windows.Media.SolidColorBrush)Value).Color.ToString();
            }

            string FIItems.GetName()
            {
                return Name;
            }
            void FIItems.SetValue(string Input)
            {
                Value = (System.Windows.Media.SolidColorBrush)(new System.Windows.Media.BrushConverter().ConvertFrom(Input));
            }
            string FIItems.GetValueType()
            {
                return ValueTypesStrings[(int)ValueTypeString];
            }
        }

        public class DateTimeValue : FIItems
        {
            public ValueTypesEnum ValueTypeString;
            public string Name;
            public System.DateTime Value;

            public DateTimeValue(string _Name, System.DateTime _Value)
            {
                ValueTypeString = ValueTypesEnum.DAT;
                Name = _Name;
                Value = _Value;
            }

            int FIItems.GetValueLength()
            {
                return Value.ToString().Length;
            }

            string FIItems.GetValue()
            {
                return Value.ToString();
            }

            string FIItems.GetName()
            {
                return Name;
            }
            void FIItems.SetValue(string Input)
            {
                Value = System.DateTime.Parse(Input);
            }
            string FIItems.GetValueType()
            {
                return ValueTypesStrings[(int)ValueTypeString];
            }
        }

        public class ListValue : FIItems
        {
            public ValueTypesEnum ValueTypeString;
            public string Name;
            public string Value;

            public ListValue(string _Name, string _Value)
            {
                ValueTypeString = ValueTypesEnum.LST;
                Name = _Name;
                Value = _Value;
            }

            int FIItems.GetValueLength()
            {
                return int.MaxValue;
            }

            string FIItems.GetValue()
            {
                return "";
            }

            string FIItems.GetName()
            {
                return Name;
            }
            void FIItems.SetValue(string Input)
            {

            }
            string FIItems.GetValueType()
            {
                return ValueTypesStrings[(int)ValueTypeString];
            }
        }

        public class EndListValue : FIItems
        {
            public ValueTypesEnum ValueTypeString;
            public string Name;
            public string Value;

            public EndListValue(string _Name, string _Value)
            {
                ValueTypeString = ValueTypesEnum.ELS;
                Name = _Name;
                Value = _Value;
            }

            int FIItems.GetValueLength()
            {
                return int.MaxValue;
            }

            string FIItems.GetValue()
            {
                return "";
            }

            string FIItems.GetName()
            {
                return Name;
            }
            void FIItems.SetValue(string Input)
            {

            }
            string FIItems.GetValueType()
            {
                return ValueTypesStrings[(int)ValueTypeString];
            }
        }
    }
}
