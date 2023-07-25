using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserControls
{
    public enum NumericTextBoxEnum
    {
        Integer,
        Real
    }
    public partial class NumericTextBox : TextBox
    {
        // Variables                                                                                                                
        private const int WM_PASTE = 0x0302;
        private NumericTextBoxEnum _numericType;

        // Properties                                                                                                               
        public double Value
        {
            get
            {
                double value = 0;
                if (double.TryParse(this.Text, out value)) return value;
                else return 0;
            }
        }
        public NumericTextBoxEnum NumericType
        {
            get { return _numericType; }
            set
            {
                _numericType = value;
                UpdateValueOnTypeChanged();
            }
        }


        // Constructors                                                                                                             
        public NumericTextBox()
        {
            InitializeComponent();
            //
            _numericType = NumericTextBoxEnum.Real;
        }


        // Methods                                                                                                                  
        protected override void WndProc(ref Message m)
        {
            if (m.Msg != WM_PASTE)
            {
                // Handle all other messages normally
                base.WndProc(ref m);
            }
            else
            {
                // Some simplified example code that complete replaces the
                // text box content only if the clipboard contains a valid double.
                // I'll leave improvement of this behavior as an exercise :)
                double value;
                if (double.TryParse(Clipboard.GetText(), out value))
                {
                    Text = value.ToString();
                }
            }
        }
        private void NumTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                if ( _numericType == NumericTextBoxEnum.Integer ||
                    (_numericType == NumericTextBoxEnum.Real && e.KeyChar != '.'))
                {
                    // Cancel key press
                    e.Handled = true;
                }
            }
            // Only allow one decimal point
            if (_numericType == NumericTextBoxEnum.Real && (e.KeyChar == '.') && Text.IndexOf('.') > -1)
            {
                // Cancel key press
                e.Handled = true;
            }
        }
        private void UpdateValueOnTypeChanged()
        {
            if (_numericType == NumericTextBoxEnum.Integer)
            {
                int separatorPos = Text.IndexOf('.');
                if (separatorPos > -1)
                {
                    Text = Text.Substring(0, Text.Length - separatorPos);
                }
            }
        }
    }
}
