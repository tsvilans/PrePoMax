using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeGlobals;
using System.Windows.Forms;

namespace PrePoMax.Forms
{
    class FrmFieldOutput : UserControls.FrmPropertyListView, IFormBase
    {
        // Variables                                                                                                                
        private string[] _fieldOutputNames;
        private string _fieldOutputToEditName;
        private ViewFieldOutput _viewFieldOutput;
        private Controller _controller;


        // Properties                                                                                                               
        public FieldOutput FieldOutput
        {
            get { return _viewFieldOutput.Base; }
            set
            {
                var clone = value.DeepClone();
                if (clone == null) _viewFieldOutput = null;
                else if (clone is NodalFieldOutput nfo) _viewFieldOutput = new ViewNodalFieldOutput(nfo);
                else if (clone is ElementFieldOutput efo) _viewFieldOutput = new ViewElementFieldOutput(efo);
                else if (clone is ContactFieldOutput cfo) _viewFieldOutput = new ViewContactFieldOutput(cfo);
                else throw new NotImplementedException();
            }
        }


        // Constructors                                                                                                             
        public FrmFieldOutput(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewFieldOutput = null;
        }
        private void InitializeComponent()
        {
            this.gbType.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbType
            // 
            this.gbType.Size = new System.Drawing.Size(310, 89);
            // 
            // lvTypes
            // 
            this.lvTypes.Size = new System.Drawing.Size(298, 61);
            // 
            // gbProperties
            // 
            this.gbProperties.Location = new System.Drawing.Point(12, 107);
            this.gbProperties.Size = new System.Drawing.Size(310, 313);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Size = new System.Drawing.Size(298, 285);
            // 
            // FrmFieldOutput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Name = "FrmFieldOutput";
            this.Text = "Edit Field Output";
            this.gbType.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnListViewTypeSelectedIndexChanged()
        {
            if (lvTypes.SelectedItems != null && lvTypes.SelectedItems.Count > 0)
            {
                propertyGrid.SelectedObject = lvTypes.SelectedItems[0].Tag;
                propertyGrid.Select();
            }
        }
        protected override void OnApply(bool onOkAddNew)
        {
            if (propertyGrid.SelectedObject == null) throw new CaeException("No item selected.");
            //
            _viewFieldOutput = (ViewFieldOutput)propertyGrid.SelectedObject;
            // Check if the name exists
            CheckName(_fieldOutputToEditName, _viewFieldOutput.Name, _fieldOutputNames, "field output");
            // Create
            if (_fieldOutputToEditName == null)
            {
                _controller.AddFieldOutputCommand(_stepName, FieldOutput);
            }
            // Replace
            else if (_propertyItemChanged)
            {
                _controller.ReplaceFieldOutputCommand(_stepName, _fieldOutputToEditName, FieldOutput);
            }
        }
        protected override bool OnPrepareForm(string stepName, string fieldOutputToEditName)
        {
            this.btnOkAddNew.Visible = fieldOutputToEditName == null;
            //
            _propertyItemChanged = false;
            _stepName = null;
            _fieldOutputNames = null;
            _fieldOutputToEditName = null;
            _viewFieldOutput = null;
            lvTypes.Items.Clear();
            propertyGrid.SelectedObject = null;
            //
            _stepName = stepName;
            _fieldOutputNames = _controller.GetFieldOutputNamesForStep(_stepName);
            _fieldOutputToEditName = fieldOutputToEditName;
            //
            if (_fieldOutputNames == null)
                throw new CaeException("The field output names must be defined first.");
            //
            PopulateListOfFieldOutputs();
            //
            if (fieldOutputToEditName == null)
            {
                lvTypes.Enabled = true;
                _viewFieldOutput = null;
            }
            else
            {
                FieldOutput = _controller.GetFieldOutput(_stepName, fieldOutputToEditName); // to clone
                //
                int selectedId;
                if (_viewFieldOutput.Base is NodalFieldOutput) selectedId = 0;
                else if (_viewFieldOutput.Base is ElementFieldOutput) selectedId = 1;
                else if (_viewFieldOutput.Base is ContactFieldOutput) selectedId = 2;
                else throw new NotSupportedException();
                //
                lvTypes.Items[selectedId].Tag = _viewFieldOutput;
                _preselectIndex = selectedId;
            }
            //
            _controller.SetSelectByToOff();
            //
            return true;
        }


        // Methods                                                                                                                  
        private void PopulateListOfFieldOutputs()
        {
            // Populate list view
            ListViewItem item;
            // Node
            item = new ListViewItem("Node Output");
            ViewNodalFieldOutput vnfo = new ViewNodalFieldOutput(new NodalFieldOutput(GetFieldOutputName("N"),
                                                                                      NodalFieldVariable.U));
            item.Tag = vnfo;
            lvTypes.Items.Add(item);
            // Element
            item = new ListViewItem("Element Output");
            ViewElementFieldOutput vefo = new ViewElementFieldOutput(new ElementFieldOutput(GetFieldOutputName("E"),
                                                                                            ElementFieldVariable.S |
                                                                                            ElementFieldVariable.E));
            item.Tag = vefo;
            lvTypes.Items.Add(item);
            // Contact
            item = new ListViewItem("Contact Output");
            Step step = _controller.Model.StepCollection.GetStep(_stepName);
            ViewContactFieldOutput vcfo = new ViewContactFieldOutput(new ContactFieldOutput(GetFieldOutputName("C"),
                                                                                            ContactFieldVariable.CDIS |
                                                                                            ContactFieldVariable.CSTR));
            item.Tag = vcfo;
            lvTypes.Items.Add(item);
        }
        private string GetFieldOutputName(string prefix)
        {
            return _fieldOutputNames.GetNextNumberedKey(prefix + "F_Output");
        }
    }
}
