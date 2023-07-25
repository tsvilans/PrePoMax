using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeMesh;
using System.Reflection;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace PrePoMax.Forms
{
    public partial class FrmGetValue : Form
    {
        // Variables                                                                                                                
        private ViewDoubleValue _viewValue;
        private double _labelRatio = 2.0; // larger value means wider second column
        


        // Properties                                                                                                               
        public double Value 
        {
            get { return _viewValue.Value; }
            set { _viewValue.Value = value; } 
        }
        public byte NumOfDigits { get { return _viewValue.NumOfDigits; } set { _viewValue.NumOfDigits = value; } }
        public double MaxValue { get { return _viewValue.MaxValue; } set { _viewValue.MaxValue = value; } }
        public double MinValue { get { return _viewValue.MinValue; } set { _viewValue.MinValue = value; } }


        // Constructors                                                                                                             
        public FrmGetValue()
        {
            InitializeComponent();
            //
            _viewValue = new ViewDoubleValue();
            //
            propertyGrid.SetLabelColumnWidth(_labelRatio);
        }


        // Event hadlers                                                                                                            
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
     

        // Methods                                                                                                                  
        public void PrepareForm(string title, string valueName, string valueDescription,
                                double value, OrderedDictionary<string, double> presetValues,
                                TypeConverter typeConverter = null)
        {
            Text = title;
            //
            _viewValue.Value = value;
            _viewValue.PresetValues = presetValues;
            //
            _viewValue.SetDisplayName(valueName);
            _viewValue.SetDescription(valueDescription);
            if (typeConverter != null) _viewValue.SetTypeConverter(typeConverter);
            else _viewValue.SetTypeConverter(new StringDoubleConverter());
            //
            propertyGrid.SelectedObject = _viewValue;
            propertyGrid.Select();
            //
        }
      
    }
}
