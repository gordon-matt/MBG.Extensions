using System.Windows.Forms;

namespace MBG.Extensions.WinForms
{
    public static class CheckedListBoxExtensions
    {
        public static void SetItemsChecked(this CheckedListBox checkedListBox, bool value)
        {
            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                checkedListBox.SetItemChecked(i, value);
            }
        }
        public static void SetItemsCheckState(this CheckedListBox checkedListBox, CheckState value)
        {
            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                checkedListBox.SetItemCheckState(i, value);
            }
        }
    }
}