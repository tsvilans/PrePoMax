using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeModel;

namespace PrePoMax.Forms
{
    class FrmPartProperties : UserControls.FrmProperties, IFormBase
    {
        // Variables                                                                                                                
        private HashSet<string> _partNames;
        private string _partToEditName;
        private ViewPartProperties _viewPartProperties;
        private ViewGeometryModelResults _currentView;
        private Controller _controller;

        // Properties                                                                                                               
        public CaeMesh.PartProperties PartProperties
        {
            get { return _viewPartProperties.GetBase(); }
            set
            {
                _viewPartProperties = new ViewPartProperties(value, _currentView,
                    _controller.Model.Properties.ModelSpace.GetUnavailableElementTypeNames());
            }
        }
        public ViewGeometryModelResults View { get { return _currentView; } set { _currentView = value; } }


        // Constructors                                                                                                             
        public FrmPartProperties(Controller controller)
        {
            InitializeComponent();

            _controller = controller;
            _viewPartProperties = null;
            _currentView = ViewGeometryModelResults.Geometry;
            _partNames = new HashSet<string>();

            _addNew = false;

            propertyGrid.SetLabelColumnWidth(1.9);
        }
        private void InitializeComponent()
        {
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 376);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 376);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 376);
            this.btnOkAddNew.Size = new System.Drawing.Size(75, 23);
            this.btnOkAddNew.Text = "Apply";
            // 
            // FrmPartProperties
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmPartProperties";
            this.Text = "Edit Part";
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnApply(bool onOkAddNew)
        {
            _viewPartProperties = (ViewPartProperties)propertyGrid.SelectedObject;
            // Check if the name exists
            CheckName(_partToEditName, _viewPartProperties.Name, _partNames, "part");
            //
            if (_partToEditName == null)
            {
                // Create
                throw new NotSupportedException();
            }
            else
            {
                // Replace
                if (_propertyItemChanged)
                {
                    if (_currentView == ViewGeometryModelResults.Geometry)
                        _controller.ReplaceGeometryPartPropertiesCommand(_partToEditName, PartProperties);
                    else if (_currentView == ViewGeometryModelResults.Model)
                        _controller.ReplaceModelPartPropertiesCommand(_partToEditName, PartProperties);
                    else if (_currentView == ViewGeometryModelResults.Results)
                        _controller.ReplaceResultPartProperties(_partToEditName, PartProperties);
                    else throw new NotSupportedException();
                    //
                    _partToEditName = PartProperties.Name;    // to enable next apply
                }
            }
        }
        protected override bool OnPrepareForm(string stepName, string partToEditName)
        {
            // Disable selection
            _controller.SetSelectByToOff();
            //
            _propertyItemChanged = false;
            _partNames.Clear();
            _partToEditName = null;
            _viewPartProperties = null;
            //
            if (_currentView == ViewGeometryModelResults.Geometry || _currentView == ViewGeometryModelResults.Model)
            {
                _partNames.UnionWith(_controller.GetGeometryPartNames());
                _partNames.UnionWith(_controller.GetAllMeshEntityNames());
            }
            else if (_currentView == ViewGeometryModelResults.Results)
                _partNames.UnionWith(_controller.GetResultPartNames());
            else
                throw new NotSupportedException();
            //
            _partToEditName = partToEditName;
            //
            if (_partToEditName == null)
            {
                throw new NotSupportedException();
            }
            else
            {
                if (_currentView == ViewGeometryModelResults.Geometry)
                    PartProperties = _controller.GetGeometryPart(_partToEditName).GetProperties();   // to clone
                else if (_currentView == ViewGeometryModelResults.Model)
                    PartProperties = _controller.GetModelPart(_partToEditName).GetProperties();    // to clone
                else if (_currentView == ViewGeometryModelResults.Results)
                    PartProperties = _controller.GetResultPart(_partToEditName).GetProperties(); // to clone
                else
                    throw new NotSupportedException();
            }
            //
            propertyGrid.SelectedObject = _viewPartProperties;
            propertyGrid.Select();
            //
            return true;
        }


        // Methods                                                                                                                  
       

        
    }
}
