using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace UserControls
{
    //                                                                                                                              
    // https://stackoverflow.com/questions/23219139/propertygrid-validation                                                         
    //                                                                                                                              
    public class MySite : ISite, IUIService
    {
        private string _name;
        public MySite(PropertyGrid propertyGrid)
        {
            PropertyGrid = propertyGrid;
            //
            _name = "MySite.Name";
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IUIService))
                return this;

            return null;
        }

        // this is part of IUIService
        public DialogResult ShowDialog(Form form)
        {
            // Check the form passed here is the error dialog box. It's type name should be GridErrorDlg.
            DialogResult result;
            //
            if (form.GetType().Name == "GridErrorDlg")
            {
                // Show a message box instead of the default error form
                result = CaeGlobals.MessageBoxes.ShowWarningQuestion(PropertyGrid, form.Controls[0].Text);
                // To reset the property grid data use refresh
                //PropertyGrid.Refresh();
            }
            else
            {
                result = form.ShowDialog(PropertyGrid);
            }
            return result;
        }

        public PropertyGrid PropertyGrid { get; private set; }
        public bool DesignMode { get { return false; } }
        public IContainer Container { get { return null; } }
        public bool CanShowComponentEditor(object component) { return false; }

        // I've left the rest as not implemented, but make sure the whole thing works in your context...
        public IComponent Component
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public IWin32Window GetDialogOwnerWindow()
        {
            throw new NotImplementedException();
        }

        public void SetUIDirty()
        {
            throw new NotImplementedException();
        }

        public bool ShowComponentEditor(object component, IWin32Window parent)
        {
            throw new NotImplementedException();
        }

        public void ShowError(Exception ex, string message)
        {
            throw new NotImplementedException();
        }

        public void ShowError(Exception ex)
        {
            throw new NotImplementedException();
        }

        public void ShowError(string message)
        {
            throw new NotImplementedException();
        }

        public DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message, string caption)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message)
        {
            throw new NotImplementedException();
        }

        public bool ShowToolWindow(Guid toolWindow)
        {
            throw new NotImplementedException();
        }

        public System.Collections.IDictionary Styles
        {
            get { throw new NotImplementedException(); }
        }
    }
}
