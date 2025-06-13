using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDownloader.Extensions
{
    public static class ComboBoxExtension
    {
        /// <summary>
        /// Populates a ComboBox with values from a specified enumeration type and sets the default selected item
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type to populate the ComboBox with</typeparam>
        /// <param name="comboBox">Target ComboBox control to populate</param>
        /// <param name="defaultValue">Default enum value to pre-select</param>
        /// <remarks>
        /// Only initializes empty ComboBoxes. Creates ComboBoxItems with Content and Tag properties
        /// </remarks>
        public static Exception? PopulateComboBoxWithEnum<TEnum>(this ComboBox comboBox, TEnum? selectedValueByDefault) where TEnum : Enum
        {
            try
            {
                if (comboBox.Items.Count == 0)
                {
                    foreach (TEnum item in Enum.GetValues(typeof(TEnum)))
                    {
                        var newOption = new ComboBoxItem { Content = item.ToString(), Tag = Convert.ToInt32(item) };
                        comboBox.Items.Add(newOption);

                        if (selectedValueByDefault is not null && item.Equals(selectedValueByDefault))
                        {
                            comboBox.SelectedIndex = comboBox.Items.Count - 1;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
