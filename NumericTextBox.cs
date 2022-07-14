using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPApp
{
    public class NumericTextBox : TextBox
    {
        public NumericTextBox()
        {
            this.DefaultStyleKey = typeof(TextBox);
            TextChanging += NumericOnlyTextBox_TextChanging;
            Paste += NumericOnlyTextBox_Paste;
        }

        private void NumericOnlyTextBox_Paste(object sender, TextControlPasteEventArgs e)
        {
            //do not allow pasting (or code it yourself)
            e.Handled = true;
            return;
        }

        private void NumericOnlyTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            var matchInt = Regex.IsMatch(sender.Text, "^\\d*$");
            bool passesTest = matchInt;


            if (!passesTest && sender.Text != "")
            {
                int pos = sender.SelectionStart - 1;
                sender.Text = sender.Text.Remove(pos, 1);
                sender.SelectionStart = pos;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InputScope scope = new InputScope();
            InputScopeName scopeName = new InputScopeName();
            scopeName.NameValue = InputScopeNameValue.NumberFullWidth;
            scope.Names.Add(scopeName);
        }


    }
}
