using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrePoMax.Forms
{ 
    class ListViewItemComparer : System.Collections.IComparer
    {
        //  Variables                                                                                                               
        private Dictionary<string, int> _namePosition = new Dictionary<string, int>();        
        private int _col;


        //  Constructors                                                                                                            
        public ListViewItemComparer()
            :this(0, null)
        {
        }
        public ListViewItemComparer(int column, Dictionary<string, int> namePosition)            
        {
            _col = column;
            _namePosition = namePosition;
        }
        public int Compare(object x, object y)
        {
            if (_namePosition == null) return 0;
            //
            int xPos = _namePosition[((ListViewItem)x).Text];
            int yPos = _namePosition[((ListViewItem)y).Text];
            //
            if (xPos > yPos) return 1;
            else if (xPos < yPos) return -1;
            return 0;
        }
    }
}
