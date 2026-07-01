using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cinema_Management_App.Extensions
{
    public static class NumericInputBehavior
    {
        public static readonly DependencyProperty IsNumericOnlyProperty =
            DependencyProperty.RegisterAttached(
                "IsNumericOnly",
                typeof(bool),
                typeof(NumericInputBehavior),
                new PropertyMetadata(false, OnIsNumericOnlyChanged));

        public static void SetIsNumericOnly(UIElement element, bool value)
        {
            element.SetValue(IsNumericOnlyProperty, value);
        }

        public static bool GetIsNumericOnly(UIElement element)
        {
            return (bool)element.GetValue(IsNumericOnlyProperty);
        }

        private static void OnIsNumericOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    DataObject.AddPastingHandler(textBox, TextBox_Pasting);
                }
                else
                {
                    textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                    DataObject.RemovePastingHandler(textBox, TextBox_Pasting);
                }
            }
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private static void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (new Regex("[^0-9]+").IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.TextChanged -= TextBox_TextChanged;

                string currentText = textBox.Text;

                if (string.IsNullOrEmpty(currentText))
                {
                    textBox.Text = "0";
                    textBox.CaretIndex = 1;
                }
                else if (currentText.Length > 1 && currentText.StartsWith("0"))
                {
                    string newText = currentText.TrimStart('0');
                    if (string.IsNullOrEmpty(newText))
                    {
                        newText = "0";
                    }

                    textBox.Text = newText;
                    textBox.CaretIndex = textBox.Text.Length;
                }

                textBox.TextChanged += TextBox_TextChanged;
            }
        }
    }
}