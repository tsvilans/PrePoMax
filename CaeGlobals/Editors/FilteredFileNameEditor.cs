using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    public class FilteredFileNameEditor : System.Windows.Forms.Design.FileNameEditor
    {
        public static string Filter = "";


        public FilteredFileNameEditor()
        {
        }


        protected override void InitializeDialog(System.Windows.Forms.OpenFileDialog openFileDialog)
        {
            base.InitializeDialog(openFileDialog);
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = Filter;
        }
    }
}
