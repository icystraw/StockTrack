using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StockTrack
{
    class Utilities
    {
        public static void ChangeDate(DatePicker dp, int delta)
        {
            if (dp.SelectedDate != null)
            {
                if (delta > 0)
                {
                    dp.SelectedDate = ((DateTime)dp.SelectedDate).AddDays(1);
                }
                if (delta < 0)
                {
                    dp.SelectedDate = ((DateTime)dp.SelectedDate).AddDays(-1);
                }
            }
        }

        public static void ChangeTextFieldNumber(TextBox t, double modification)
        {
            string originalText = t.Text.Trim();
            List<string> textSections = new List<string>();
            bool? isLetter = null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < originalText.Length; i++)
            {
                if (Char.IsLetter(originalText[i]) != isLetter)
                {
                    isLetter = Char.IsLetter(originalText[i]);
                    if (sb.Length > 0) textSections.Add(sb.ToString());
                    sb = new StringBuilder();
                }
                sb.Append(originalText[i]);
            }
            if (sb.Length > 0) textSections.Add(sb.ToString());
            for (int i = textSections.Count - 1; i >= 0; i--)
            {
                try
                {
                    textSections[i] = (Convert.ToDouble(textSections[i]) + modification).ToString();
                    break;
                }
                catch
                {
                    continue;
                }
            }
            sb = new StringBuilder();
            foreach (string textSection in textSections)
            {
                sb.Append(textSection);
            }
            t.Text = sb.ToString();
            t.Focus();
            t.SelectAll();
        }
    }
}
