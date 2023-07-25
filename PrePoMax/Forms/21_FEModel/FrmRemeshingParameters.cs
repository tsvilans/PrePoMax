using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeMesh;
using CaeGlobals;

namespace PrePoMax.Forms
{
    class FrmRemeshingParameters : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        bool _highlightEnabled;
        private ViewRemeshingParameters _viewRemeshingParameters;
        private RemeshingParameters _prevRemeshingParameters;
        private Button btnPreview;
        private Controller _controller;


        // Properties                                                                                                               
        public RemeshingParameters RemeshingParameters
        {
            get { return _viewRemeshingParameters.GetBase(); }
            set { _viewRemeshingParameters = new ViewRemeshingParameters(value.DeepClone()); }
        }


        // Constructors                                                                                                             
        public FrmRemeshingParameters(Controller controller) 
        {
            InitializeComponent();
            //
            _controller = controller;
            _highlightEnabled = true;
            _viewRemeshingParameters = null;
            _prevRemeshingParameters = null;
            //
            SelectionClear = _controller.Selection.Clear;
        }
        private void InitializeComponent()
        {
            this.btnPreview = new System.Windows.Forms.Button();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Text = "Close";
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(160, 376);
            this.btnOkAddNew.Text = "Apply";
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreview.Location = new System.Drawing.Point(79, 376);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 17;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // FrmRemeshingParameters
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Controls.Add(this.btnPreview);
            this.Name = "FrmRemeshingParameters";
            this.Text = "Remeshing Parameters";
            this.VisibleChanged += new System.EventHandler(this.FrmRemeshingParameters_VisibleChanged);
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.Controls.SetChildIndex(this.btnPreview, 0);
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Event handlers                                                                                                           
        private void FrmRemeshingParameters_VisibleChanged(object sender, EventArgs e)
        {
            // Limit selection to the first selected part
            _controller.Selection.LimitSelectionToFirstPart = Visible;
            btnPreview.Enabled = true;
        }
        async private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                HighlightElementSet();
                //
                btnPreview.Enabled = false;
                //
                CheckRemeshingParameters();
                //
                await Task.Run(() => _controller.RemeshElements(RemeshingParameters, true));
                //
                if (RemeshingParameters.RegionType == RegionTypeEnum.Selection &&
                    RemeshingParameters.CreationData != null)
                {
                    _controller.Selection = RemeshingParameters.CreationData.DeepClone();
                }
            }
            catch (Exception ex)
            {
                btnPreview.Enabled = true;
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                btnPreview.Enabled = true;
            }
        }


        // Overrides                                                                                                                
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (property == nameof(_viewRemeshingParameters.RegionType))
            {
                ShowHideSelectionForm();
                //
                HighlightElementSet();
            }
            else if (property == nameof(_viewRemeshingParameters.ElementSetName))
            {
                HighlightElementSet();
            }
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            _highlightEnabled = false;
            //
            CheckRemeshingParameters();
            //
            _controller.RemeshElementsCommand(RemeshingParameters);
            // If all is successful close the ItemSetSelectionForm - except for OKAddNew
            if (onOkAddNew) _prevRemeshingParameters = RemeshingParameters.DeepClone();
            else ItemSetDataEditor.SelectionForm.Hide();
        }
        protected override void OnHideOrClose()
        {
            _prevRemeshingParameters = null;
            // Close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
            //
            base.OnHideOrClose();
        }
        protected override bool OnPrepareForm(string stepName, string itemToEditName)
        {
            this.btnOK.Visible = false;
            //
            _highlightEnabled = true;
            _propertyItemChanged = false;
            _viewRemeshingParameters = null;
            propertyGrid.SelectedObject = null;
            //
            string[] elementSetNames = _controller.GetUserElementSetNames();
            // Create new
            if (_prevRemeshingParameters == null)
            {
                MeshingParameters meshingParameters =
                    _controller.GetPartDefaultMeshingParameters(_controller.Model.Mesh.Parts.Keys.ToArray(), false);
                RemeshingParameters = new RemeshingParameters("", RegionTypeEnum.Selection, meshingParameters);
            }
            else RemeshingParameters = _prevRemeshingParameters;
            //
            _controller.Selection.Clear();                
            //
            _viewRemeshingParameters.PopulateDropDownLists(elementSetNames);
            //
            propertyGrid.SelectedObject = _viewRemeshingParameters;
            propertyGrid.Focus();
            // Get start point grid item and select it
            GridItem gi = propertyGrid.EnumerateAllItems().First((item) =>
                          item.PropertyDescriptor != null &&
                          item.PropertyDescriptor.Name == nameof(_viewRemeshingParameters.RegionType));
            gi.Select();
            //
            ShowHideSelectionForm();
            //
            HighlightElementSet();
            //
            return true;
        }


        // Methods                                                                                                                  
        private void CheckRemeshingParameters()
        {
            _viewRemeshingParameters = (ViewRemeshingParameters)propertyGrid.SelectedObject;
            //
            if (_viewRemeshingParameters == null) throw new CaeException("No remeshing was selected.");
            //
            if (RemeshingParameters.RegionType == RegionTypeEnum.Selection &&
                (RemeshingParameters.CreationIds == null || RemeshingParameters.CreationIds.Length == 0))
                throw new CaeException("The element set for remeshing selection must contain at least one item.");
        }
        private void HighlightElementSet()
        {
            try
            {
                _controller.ClearSelectionHistory();
                //
                if (_viewRemeshingParameters == null) { }
                else 
                {
                    if (RemeshingParameters.RegionType == RegionTypeEnum.ElementSetName)
                    {
                        _controller.Highlight3DObjects(new object[] { RemeshingParameters.RegionName });
                    }
                    else if (RemeshingParameters.RegionType == RegionTypeEnum.Selection)
                    {
                        SetSelectItem();
                        //
                        if (RemeshingParameters.CreationData != null)
                        {
                            _controller.Selection = RemeshingParameters.CreationData.DeepClone();
                            _controller.HighlightSelection();
                        }
                    }
                    else throw new NotSupportedException();
                }
            }
            catch { }
        }
        //
        private void ShowHideSelectionForm()
        {
            if (RemeshingParameters != null && RemeshingParameters.RegionType == RegionTypeEnum.Selection)
                ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            else
                ItemSetDataEditor.SelectionForm.Hide();
            //
            SetSelectItem();
        }
        private void SetSelectItem()
        {
            if (RemeshingParameters != null && RemeshingParameters.RegionType == RegionTypeEnum.Selection)
                _controller.SetSelectItemToElement();
            else
                _controller.SetSelectByToOff();
        }
        public void SelectionChanged(int[] ids)
        {
            if (RemeshingParameters != null && RemeshingParameters.RegionType == RegionTypeEnum.Selection)
            {
                RemeshingParameters.CreationIds = ids;
                RemeshingParameters.CreationData = _controller.Selection.DeepClone();
                //
                propertyGrid.Refresh();
                //
                _propertyItemChanged = true;
            }
        }
        

        // IFormHighlight
        public void Highlight()
        {
            if(_highlightEnabled) HighlightElementSet();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            if (RemeshingParameters == null || RemeshingParameters.CreationData == null) return true;
            return RemeshingParameters.CreationData.IsGeometryBased();
        }

      
    }


}
