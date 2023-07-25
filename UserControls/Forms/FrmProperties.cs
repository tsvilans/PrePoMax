using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using CaeGlobals;

namespace UserControls
{
    public partial class FrmProperties : PrePoMaxChildForm
    {
        // Variables                                                                                                                
        protected Action _controller_SelectionClear;
        protected string _stepName;
        protected bool _hideOnClose;
        protected bool _addNew;


        // Properties                                                                                                               
        public Action SelectionClear { get { return _controller_SelectionClear; } set { _controller_SelectionClear = value; } }


        // Constructors                                                                                                             
        public FrmProperties()
            : this(2.0)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelRatio">Larger value means wider second column. Default = 2.0</param>
        public FrmProperties(double labelRatio)
        {
            InitializeComponent();
            //
            _controller_SelectionClear = null;
            _hideOnClose = true;
            _addNew = true;
            //
            propertyGrid.SetLabelColumnWidth(labelRatio);
        }


        // Event hadlers                                                                                                            
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            try
            {
                if (propertyGrid.SelectedGridItem.Value != null) OnPropertyGridPropertyValueChanged();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void propertyGrid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            try
            {
                OnPropertyGridSelectedGridItemChanged();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void FrmProperties_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible) OnHideOrClose();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                OnApply(false);
                //
                if (_hideOnClose) OnHideOrClose();
                else Close();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnOKAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                OnApply(true);
                //
                if (_addNew) PrepareForm(_stepName, null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        protected virtual void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                OnHideOrClose();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void FrmProperties_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_hideOnClose && e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    //
                    OnHideOrClose();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void FrmProperties_EnabledChanged(object sender, EventArgs e)
        {
            try
            {
                OnEnabledChanged();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }


        // Methods                                                                                                                  
        public virtual bool PrepareForm(string stepName, string itemToEditName)
        {
            return OnPrepareForm(stepName, itemToEditName);
        }
        //
        protected virtual void OnHideOrClose()
        {
            _controller_SelectionClear?.Invoke();
            if (_hideOnClose) Hide();
            else Close();
        }
        protected virtual bool OnPrepareForm(string stepName, string itemToEditName) { return true; }
        protected virtual void OnPropertyGridPropertyValueChanged()
        {
            propertyGrid.Refresh();
            _propertyItemChanged = true;
        }
        protected virtual void OnPropertyGridSelectedGridItemChanged()
        {
        }
        protected virtual void OnEnabledChanged() { }
        protected virtual void OnApply(bool onOkAddNew) { }
        //
        public static void CheckName(string nameToEdit, string name, ICollection<string> existingNames, string messageName)
        {
            // Named to existing name
            if ((nameToEdit == null && existingNames.Contains(name, StringComparer.OrdinalIgnoreCase)) ||
            // Renamed to existing name
            ((nameToEdit != null && name.ToLower() != nameToEdit.ToLower()) &&
             name != nameToEdit && existingNames.Contains(name, StringComparer.OrdinalIgnoreCase)))
            // Exception
            throw new CaeException("The selected "+ messageName + " name already exists. " +
                "Uppercase and lowercase letters are regarded as equal.");
        }
        
    }
}
