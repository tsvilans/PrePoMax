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
    public partial class UnitAwareToolStripTextBox : ToolStripTextBox
    {
        // Variables                                                                                                                
        private TypeConverter _converter;
        private string _previousValue;


        // Properties                                                                                                               
        public double Value
        {
            get
            {
                double value = double.NaN;
                try
                {
                    if (!double.TryParse(this.Text, out value) && _converter != null)
                    {
                        value = (double)_converter.ConvertFromString(this.Text);
                    }
                    return value;
                }
                catch
                {
                    return value;
                }
            }
        }
        public TypeConverter UnitConverter
        {
            get { return _converter; }
            set
            {
                if (value != _converter)
                {
                    _converter = value;
                    TryConvertText();
                }
            }
        }


        // Constructors                                                                                                             
        public UnitAwareToolStripTextBox()
        {
            InitializeComponent();
            //
            _converter = null;
        }


        // Event handlers                                                                                                           
        private void UnitAwareToolStripTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Convert
                TryConvertText();
                //
                e.SuppressKeyPress = true;  // no beep
            }
        }
        private void UnitAwareToolStripTextBox_Leave(object sender, EventArgs e)
        {
            TryConvertText();
        }
        private void UnitAwareToolStripTextBox_GotFocus(object sender, EventArgs e)
        {
            _previousValue = Text;
        }
        private void UnitAwareToolStripTextBox_LostFocus(object sender, EventArgs e)
        {
            if (Text != _previousValue) Text = _previousValue;
        }
        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (value != base.Text)
                {
                    base.Text = value;
                    TryConvertText();
                }
            }
        }


        // Methods                                                                                                                  
        public void TryConvertText()
        {
            try
            {
                if (_converter != null)
                {
                    // Convert
                    if (Text.Trim().Length == 0) Text = "0";
                    double value = (double)_converter.ConvertFromString(this.Text);
                    Text = _converter.ConvertToString(value);
                    _previousValue = Text;
                }
            }
            catch (Exception ex)
            {
                CaeGlobals.MessageBoxes.ShowError("Entered value: " + this.Text + Environment.NewLine + Environment.NewLine
                                                  + ex.Message);
                this.Focus();
            }
        }
    }
}
