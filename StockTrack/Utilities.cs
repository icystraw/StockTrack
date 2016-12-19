using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StockTrack
{
    class Utilities
    {
        public static void EmailOrder(string itemName, double quantity, string orderNo)
        {
            try
            {
                using (StreamReader sr = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + "\\OrderTemplate.html"))
                {
                    String line = sr.ReadToEnd();
                    line = line.Replace("[#date]", DateTime.Now.ToString("dd/MM/yyyy")).Replace("[#itemname]", itemName).Replace("[#orderno]", orderNo).Replace("[#quantity]", quantity.ToString());

                    MailMessage m = new MailMessage("stocktrack@localhost", "stocktrack@localhost");
                    m.Subject = "Purchase Order #" + orderNo;
                    m.IsBodyHtml = true;
                    m.Body = line;
                    m.Headers.Add("X-Unsent", "1");

                    SmtpClient c = new SmtpClient();
                    c.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    c.PickupDirectoryLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    c.Send(m);
                }
            }
            catch
            {
                return;
            }
        }

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
