using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using DynamicTypeDescriptor;
using System.Linq.Expressions;

namespace CaeGlobals
{
    [Serializable]
    public static class MessageBoxes
    {
        public static Form ParentForm;
        // MessageBox
        public static void ShowError(string text)
        {
            using (new CenterWinDialog(ParentForm))
            {
                MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void ShowWorkDirectoryError()
        {
            using (new CenterWinDialog(ParentForm))
            {
                MessageBox.Show("The work directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void ShowWarning(string text)
        {
            using (new CenterWinDialog(ParentForm))
            {
                MessageBox.Show(text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public static DialogResult ShowWarningQuestion(string text)
        {
            using (new CenterWinDialog(ParentForm))
            {
                return MessageBox.Show(text, "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
        }
        public static DialogResult ShowWarningQuestion(IWin32Window owner, string text)
        {
            return MessageBox.Show(owner, text, "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        }
        public static DialogResult ShowQuestion(string caption, string text)
        {
            using (new CenterWinDialog(ParentForm))
            {
                return MessageBox.Show(text, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
        }
    }
}
