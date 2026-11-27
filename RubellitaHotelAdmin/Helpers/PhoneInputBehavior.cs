using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RubellitaHotelAdmin.Helpers
{
    /// <summary>Обмежує введення в полі телефону: лише цифри та тире, не більше потрібної кількості цифр.</summary>
    public static class PhoneInputBehavior
    {
        private static readonly HashSet<TextBox> _updatingBoxes = new HashSet<TextBox>();

        public static readonly DependencyProperty MaxDigitsProperty =
            DependencyProperty.RegisterAttached(
                "MaxDigits",
                typeof(int),
                typeof(PhoneInputBehavior),
                new PropertyMetadata(9, OnMaxDigitsChanged));

        public static int GetMaxDigits(DependencyObject obj) => (int)obj.GetValue(MaxDigitsProperty);
        public static void SetMaxDigits(DependencyObject obj, int value) => obj.SetValue(MaxDigitsProperty, value);

        private static void OnMaxDigitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
                textBox.PreviewKeyDown += TextBox_PreviewKeyDown;

                textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                textBox.PreviewTextInput += TextBox_PreviewTextInput;

                textBox.TextChanged -= TextBox_TextChanged;
                textBox.TextChanged += TextBox_TextChanged;

                DataObject.RemovePastingHandler(textBox, TextBox_Pasting);
                DataObject.AddPastingHandler(textBox, TextBox_Pasting);

                EnforceDigitLimit(textBox, (int)e.NewValue);
            }
        }

        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox == null) return;

            bool isDigitKey = (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9);
            if (!isDigitKey) return;

            if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) != ModifierKeys.None) return;

            int maxDigits = GetMaxDigits(textBox);
            int selectedDigits = (textBox.SelectedText ?? string.Empty).Count(char.IsDigit);
            int currentDigits = textBox.Text.Count(char.IsDigit) - selectedDigits;

            if (currentDigits + 1 > maxDigits)
            {
                e.Handled = true;
            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox == null) return;
            if (_updatingBoxes.Contains(textBox)) return;

            int maxDigits = GetMaxDigits(textBox);
            EnforceDigitLimit(textBox, maxDigits);
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox == null) return;

            if (!e.Text.All(c => char.IsDigit(c) || c == '-'))
            {
                e.Handled = true;
                return;
            }

            int addedDigits = e.Text.Count(char.IsDigit);
            if (addedDigits == 0) return;

            int maxDigits = GetMaxDigits(textBox);
            int selectedDigits = (textBox.SelectedText ?? string.Empty).Count(char.IsDigit);
            int currentDigits = textBox.Text.Count(char.IsDigit) - selectedDigits;

            if (currentDigits + addedDigits > maxDigits)
            {
                e.Handled = true;
            }
        }

        private static void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox == null) return;
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            string pasted = (string)e.DataObject.GetData(typeof(string));
            if (!pasted.All(c => char.IsDigit(c) || c == '-'))
            {
                e.CancelCommand();
                return;
            }

            int maxDigits = GetMaxDigits(textBox);
            int selectedDigits = (textBox.SelectedText ?? string.Empty).Count(char.IsDigit);
            int currentDigits = textBox.Text.Count(char.IsDigit) - selectedDigits;
            int pastedDigits = pasted.Count(char.IsDigit);

            if (currentDigits + pastedDigits > maxDigits)
            {
                e.CancelCommand();
            }
        }

        private static void EnforceDigitLimit(TextBox textBox, int maxDigits)
        {
            if (maxDigits < 0) maxDigits = 0;

            int count = 0;
            var sb = new StringBuilder();
            foreach (char c in textBox.Text ?? string.Empty)
            {
                if (char.IsDigit(c))
                {
                    if (count >= maxDigits) continue;
                    count++;
                    sb.Append(c);
                }
                else if (c == '-' && count < maxDigits)
                {
                    sb.Append(c);
                }
            }

            string trimmed = sb.ToString();
            if (textBox.Text != trimmed)
            {
                _updatingBoxes.Add(textBox);
                try
                {
                    int caret = textBox.CaretIndex;
                    textBox.Text = trimmed;
                    textBox.CaretIndex = Math.Min(caret, textBox.Text.Length);

                    var binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    binding?.UpdateSource();
                }
                finally
                {
                    _updatingBoxes.Remove(textBox);
                }
            }
        }
    }
}
