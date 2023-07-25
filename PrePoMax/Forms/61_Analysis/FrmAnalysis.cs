using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeJob;
using System.Reflection;
using System.IO;
using CaeGlobals;

namespace PrePoMax.Forms
{
    public partial class FrmAnalysis : UserControls.PrePoMaxChildForm, IFormBase
    {
        // Variables                                                                                                                
        private string[] _jobNames;
        private string _jobToEditName;
        private ViewJob _viewJob;
        private Controller _controller;
        private double _labelRatio = 3;


        // Properties                                                                                                               
        public AnalysisJob Job 
        {
            get { return _viewJob.GetBase(); }
            set { _viewJob = new ViewJob(value.DeepClone()); }
        }


        // Constructors                                                                                                             
        public FrmAnalysis(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewJob = null;
            //
            propertyGrid.SetLabelColumnWidth(_labelRatio);
        }


        // Event hadlers                                                                                                            
        private void FrmAnalysis_VisibleChanged(object sender, EventArgs e)
        {
            // This must be here and not in PrepareForm since PrepareForm is called by GetDefaultJob
            if (Visible) _controller.SetSelectByToOff();
        }
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();
            _propertyItemChanged = true;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                _viewJob = (ViewJob)propertyGrid.SelectedObject;
                // Check if the name exists
                UserControls.FrmProperties.CheckName(_jobToEditName, _viewJob.Name, _jobNames, "analysis");
                //
                if (_jobToEditName == null)
                {
                    // Create
                    _controller.AddJobCommand(Job);
                }
                else
                {
                    // Replace
                    if (_propertyItemChanged) _controller.ReplaceJobCommand(_jobToEditName, Job);
                }
                Hide();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }
        private void FrmAnalysis_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }


        // Methods                                                                                                                  
        public bool PrepareForm(string stepName, string jobToEditName)
        {
            _propertyItemChanged = false;
            _jobNames = null;
            _jobToEditName = null;
            _viewJob = null;
            propertyGrid.SelectedObject = null;
            //
            _jobNames = _controller.GetJobNames();
            _jobToEditName = jobToEditName;
            //
            if (_jobToEditName == null)
            {
                Job = new AnalysisJob(GetJobName(), _controller.Settings.Calculix.CalculixExe, GetJobName(),
                                      _controller.Settings.GetWorkDirectory());
            }
            else
            {
                Job = _controller.GetJob(_jobToEditName); // to clone
            }
            //
            propertyGrid.SelectedObject = _viewJob;
            propertyGrid.Select();            
            //
            return true;
        }
        private string GetJobName()
        {
            int max = 0;
            int tmp;
            string[] parts;
            foreach (var jobName in _jobNames)
            {
                parts = jobName.Split('-');
                if (int.TryParse(parts.Last(), out tmp))
                {
                    if (tmp > max) max = tmp;
                }
            }
            max++;
            //
            return "Analysis-" + max.ToString();
        }

     
    }
}
