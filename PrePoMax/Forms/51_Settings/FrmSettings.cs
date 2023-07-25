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
using System.IO;
using CaeGlobals;
using PrePoMax.Settings;
using System.Runtime.InteropServices;

namespace PrePoMax.Forms
{
    public partial class FrmSettings : UserControls.PrePoMaxChildForm
    {
        // Variables                                                                                                                
        private string _previousSettings;
        private Dictionary<string, IViewSettings> _viewSettings;
        private double _labelRatio = 2.3;


        // Properties                                                                                                               
        public Dictionary<string, ISettings> Settings 
        {
            get
            {
                Dictionary<string, ISettings> result = new Dictionary<string, ISettings>();
                foreach (var entry in _viewSettings)
                {
                    result.Add(entry.Key, entry.Value.GetBase());
                }
                return result;
            }
            set 
            {
                //create a clone
                _viewSettings = new Dictionary<string, IViewSettings>();
                foreach (var entry in value)
                {
                    if (entry.Value is GeneralSettings ges)
                        _viewSettings.Add(entry.Key, new ViewGeneralSettings(ges.DeepClone()));
                    else if (entry.Value is GraphicsSettings grs)
                        _viewSettings.Add(entry.Key, new ViewGraphicsSettings(grs.DeepClone()));
                    else if (entry.Value is ColorSettings cos)
                        _viewSettings.Add(entry.Key, new ViewColorSettings(cos.DeepClone()));
                    else if (entry.Value is AnnotationSettings wis)
                        _viewSettings.Add(entry.Key, new ViewAnnotationSettings(wis.DeepClone()));
                    else if (entry.Value is MeshingSettings ms)
                        _viewSettings.Add(entry.Key, new ViewMeshingSettings(ms.DeepClone()));
                    else if (entry.Value is PreSettings prs)
                        _viewSettings.Add(entry.Key, new ViewPreSettings(prs.DeepClone()));
                    else if (entry.Value is CalculixSettings cas)
                        _viewSettings.Add(entry.Key, new ViewCalculixSettings(cas.DeepClone()));
                    else if (entry.Value is PostSettings pos)
                        _viewSettings.Add(entry.Key, new ViewPostSettings(pos.DeepClone()));
                    else if (entry.Value is LegendSettings les)
                        _viewSettings.Add(entry.Key, new ViewLegendSettings(les.DeepClone()));
                    else if (entry.Value is StatusBlockSettings sbs)
                        _viewSettings.Add(entry.Key, new ViewStatusBlockSettings(sbs.DeepClone()));
                    else throw new NotSupportedException();
                }
            } 
        }


        // Events                                                                                                                   
        public event Action<Dictionary<string, ISettings>> UpdateSettings;


        // Constructors                                                                                                             
        public FrmSettings()
        {
            InitializeComponent();
            _previousSettings = null;
            _viewSettings = null;
            //
            propertyGrid.SetLabelColumnWidth(_labelRatio);
        }


        // Event hadlers                                                                                                            
        private void lvSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSettings.SelectedItems.Count > 0)
            {
                propertyGrid.SelectedObject = lvSettings.SelectedItems[0].Tag;
                propertyGrid.Select();
                _previousSettings = lvSettings.SelectedItems[0].Text;
            }
        }
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();
            _propertyItemChanged = true;
        }
        private void cmsPropertyGrid_Opening(object sender, CancelEventArgs e)
        {
            if (propertyGrid.SelectedObject is IReset)
                tsmiResetAll.Enabled = true;
            else
                tsmiResetAll.Enabled = false;
        }
        private void tsmiResetAll_Click(object sender, EventArgs e)
        {
            if (propertyGrid.SelectedObject is IReset resetObject)
            {
                resetObject.Reset();
                propertyGrid.Refresh();
                _propertyItemChanged = true;
            }
        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            if (_propertyItemChanged)
            {
                Dictionary<string, ISettings> settings = Settings;
                //
                foreach (var entry in settings)
                {
                    entry.Value.CheckValues();
                }
                //
                UpdateSettings?.Invoke(settings);
                _propertyItemChanged = false;
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                btnApply_Click(null, null);
                //
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Hide();
            }
            catch (Exception ex)
            {
                CaeGlobals.ExceptionTools.Show(this, ex);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }
        private void FrmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        // Methods                                                                                                                  
        public void PrepareForm(Controller controller)
        {
            _propertyItemChanged = false;
            _viewSettings = null;
            lvSettings.Items.Clear();
            propertyGrid.SelectedObject = null;
            //
            if (controller.Settings == null)
            {
                throw new Exception("There are no settings to show.");
            }
            else
            {
                Settings = controller.Settings.ToDictionary();
                //
                ListViewItem lvi;
                foreach (var entry in _viewSettings)
                {
                    lvi = lvSettings.Items.Add(entry.Key);
                    lvi.Tag = entry.Value;
                }
            }
            // Open previously shown settings
            if (_previousSettings != null)
            {
                foreach (ListViewItem item in lvSettings.Items)
                {
                    if (item.Text == _previousSettings)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
            else
            {
                if (lvSettings.Items.Count > 0) lvSettings.Items[0].Selected = true;
            }
            //
            controller.SetSelectByToOff();
        }
        public void SetSettingsToShow(string name)
        {
            _previousSettings = name;
        }

      

        

       
      
    }
}
