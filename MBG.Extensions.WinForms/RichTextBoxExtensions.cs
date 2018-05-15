using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

namespace MBG.Extensions.WinForms
{
    public static class RichTextBoxExtensions
    {
        public static void Highlight(this RichTextBox richTextBox, int startIndex, int endIndex)
        {
            richTextBox.Highlight(startIndex, endIndex, Color.Yellow);
        }
        public static void Highlight(this RichTextBox richTextBox, int startIndex, int endIndex, Color color)
        {
            richTextBox.Select(startIndex, endIndex);
            richTextBox.SelectionBackColor = color;
        }

        public static void HighlightAll(this RichTextBox richTextBox, string text)
        {
            richTextBox.HighlightAll(text, Color.Yellow);
        }
        public static void HighlightAll(this RichTextBox richTextBox, string text, Color color)
        {
            Regex regex = new Regex(text, RegexOptions.Compiled);

            foreach (Match match in regex.Matches(richTextBox.Text))
            {
                richTextBox.Highlight(match.Index, match.Length, color);
            }
        }
    }
}