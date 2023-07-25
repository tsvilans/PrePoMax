using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using PrePoMax.Forms;
using CaeGlobals;
using UserControls;
using CaeJob;
using System.Reflection;
using CaeModel;
using CaeMesh;
using CaeResults;
using vtkControl;

namespace PrePoMax
{
    public partial class FrmMain : MainMouseWheelManagedForm
    {
        // Variables                                                                                                                
        #region Variables ##########################################################################################################
        //
        FrmSplash splash;
        //
        private vtkControl.vtkControl _vtk;
        private ModelTree _modelTree;
        private Controller _controller;
        private string[] _args;
        private string[] outputLines;
        private AdvisorControl _advisorControl;
        private KeyboardHook _keyboardHook;
        //
        private Point _formLocation;
        private List<Form> _allForms;
        private FrmSectionView _frmSectionView;
        private FrmExplodedView _frmExplodedView;
        private FrmSelectEntity _frmSelectEntity;
        private FrmSelectGeometry _frmSelectGeometry;
        private FrmSelectItemSet _frmSelectItemSet;
        private FrmNewModel _frmNewModel;
        private FrmAnalyzeGeometry _frmAnalyzeGeometry;
        private FrmMeshingParameters _frmMeshingParameters;
        private FrmMeshRefinement _frmMeshRefinement;
        private FrmModelProperties _frmModelProperties;
        private FrmCalculixKeywordEditor _frmCalculixKeywordEditor;
        private FrmPartProperties _frmPartProperties;
        private FrmBoundaryLayer _frmBoundaryLayer;
        private FrmRemeshingParameters _frmRemeshingParameters;
        private FrmTranslate _frmTranslate;
        private FrmScale _frmScale;
        private FrmRotate _frmRotate;
        private FrmNodeSet _frmNodeSet;
        private FrmElementSet _frmElementSet;
        private FrmSurface _frmSurface;
        private FrmReferencePoint _frmReferencePoint;
        private FrmMaterial _frmMaterial;
        private FrmSection _frmSection;
        private FrmConstraint _frmConstraint;
        private FrmSurfaceInteraction _frmSurfaceInteraction;
        private FrmContactPair _frmContactPair;
        private FrmSearchContactPairs _frmSearchContactPairs;
        private FrmInitialCondition _frmInitialCondition;
        private FrmAmplitude _frmAmplitude;
        private FrmStep _frmStep;
        private FrmHistoryOutput _frmHistoryOutput;
        private FrmFieldOutput _frmFieldOutput;
        private FrmBC _frmBoundaryCondition;
        private FrmLoad _frmLoad;
        private FrmDefinedField _frmDefinedField;
        private FrmSettings _frmSettings;
        private FrmQuery _frmQuery;
        private FrmFind _frmFind;
        private FrmAnalysis _frmAnalysis;
        private FrmMonitor _frmMonitor;
        private FrmAnimation _frmAnimation;
        private FrmResultFieldOutput _frmResultFieldOutput;
        private FrmViewResultHistoryOutput _frmViewResultHistoryOutput;
        private FrmResultHistoryOutput _frmResultHistoryOutput;
        private FrmTransformation _frmTransformation;
        //
        #endregion  ################################################################################################################

        #region Properties #########################################################################################################
        public Controller Controller { get { return _controller; } set { _controller = value; } }
        public ViewGeometryModelResults GetCurrentView()
        {
            return _controller.CurrentView;
        }
        public void SetCurrentView(ViewGeometryModelResults view)
        {
            // This gets called from: _controller.CurrentView
            InvokeIfRequired(() =>
            {
                if (view == ViewGeometryModelResults.Geometry)
                {
                    _modelTree.SetGeometryTab();
                    if (_controller.Model != null) UpdateUnitSystem(_controller.Model.UnitSystem);
                }
                else if (view == ViewGeometryModelResults.Model)
                {
                    _modelTree.SetModelTab();
                    if (_controller.Model != null) UpdateUnitSystem(_controller.Model.UnitSystem);
                }
                else if (view == ViewGeometryModelResults.Results)
                {
                    _modelTree.SetResultsTab();
                    if (_controller.CurrentResult != null) UpdateUnitSystem(_controller.CurrentResult.UnitSystem);
                    InitializeResultWidgetPositions();
                }
                else throw new NotSupportedException();
                //
                if (_advisorControl != null)
                {
                    ViewType viewType = GetViewType(view);
                    //
                    _advisorControl.PrepareControls(viewType);
                }
                //
                SetMenuAndToolStripVisibility();
                // This calls the saved action
                SetCurrentEdgesVisibilities(_controller.CurrentEdgesVisibility);    // highlights selected buttons
                //
                this.ActiveControl = null;
            });
        }
        public void SetCurrentEdgesVisibilities(vtkControl.vtkEdgesVisibility edgesVisibility)
        {
            InvokeIfRequired(() =>
            {
                // Highlight selected buttons
                tsbShowWireframeEdges.Checked = edgesVisibility == vtkControl.vtkEdgesVisibility.Wireframe;
                tsbShowElementEdges.Checked = edgesVisibility == vtkControl.vtkEdgesVisibility.ElementEdges;
                tsbShowModelEdges.Checked = edgesVisibility == vtkControl.vtkEdgesVisibility.ModelEdges;
                tsbShowNoEdges.Checked = edgesVisibility == vtkControl.vtkEdgesVisibility.NoEdges;
                //
                _vtk.EdgesVisibility = edgesVisibility;
                //
                UpdateHighlight();
            });
        }
        public bool ScreenUpdating { get { return _modelTree.ScreenUpdating; } set { _modelTree.ScreenUpdating = value; } }
        public bool DisableSelectionsChanged
        {
            get { return _modelTree.DisableSelectionsChanged; }
            set { _modelTree.DisableSelectionsChanged = value; }
        }
        public bool RenderingOn { get { return _vtk.RenderingOn; } set { _vtk.RenderingOn = value; } }        
        private ViewType GetViewType(ViewGeometryModelResults view)
        {
            ViewType viewType;
            if (view == ViewGeometryModelResults.Geometry) viewType = ViewType.Geometry;
            else if (view == ViewGeometryModelResults.Model) viewType = ViewType.Model;
            else if (view == ViewGeometryModelResults.Results) viewType = ViewType.Results;
            else throw new NotSupportedException();
            return viewType;
        }

        #endregion  ################################################################################################################


        // Constructors                                                                                                             
        public FrmMain(string[] args)
        {
            // Initialize               
            InitializeComponent();
            //SettingsContainer settings = new SettingsContainer();
            //settings.LoadFromFile();
            //if (settings.General.Maximized)
            //{
            //    Rectangle resolution = Screen.FromControl(this).Bounds;
            //    this.Location = new Point(0, 0);
            //    this.Size = new Size(resolution.Width, resolution.Height);
            //    this.WindowState = FormWindowState.Maximized;
            //}
            _vtk = null;
            _controller = null;
            _modelTree = null;
            _args = args;
            //
            MessageBoxes.ParentForm = this;
        }


        // Event handling                                                                                                           
        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (TestWriteAccess() == false)
            {
                MessageBoxes.ShowError("PrePoMax has no write access for the folder: " + Application.StartupPath +
                                       Environment.NewLine + Environment.NewLine +
                                       "To run PrePoMax, move the base PrePoMax folder to another, non-protected folder.");
                Close();
                return;
            }
            //
            Text = Globals.ProgramName;
            this.TopMost = true;
            splash = new FrmSplash { TopMost = true };
            var task = Task.Run(() => splash.ShowDialog());            
            //
            try
            {
                // Edit annotation text box
                panelControl.Controls.Remove(aeAnnotationTextEditor);
                this.Controls.Add(aeAnnotationTextEditor);
                // Vtk
                _vtk = new vtkControl.vtkControl();
                panelControl.Parent.Controls.Add(_vtk);
                panelControl.SendToBack();
                // Menu
                tsmiColorAnnotations.DropDown.Closing += DropDown_Closing;
                // Tree
                _modelTree = new ModelTree();
                _modelTree.Name = "modelTree";
                //_modelTree.Location = new Point(0, 0);
                splitContainer1.Panel1.Controls.Add(this._modelTree);
                _modelTree.Dock = DockStyle.Fill;
                //_modelTree.Size = splitContainer1.Panel1.ClientSize;
                //_modelTree.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                //_modelTree.Dock = DockStyle.None;
                _modelTree.TabIndex = 0;
                _modelTree.RegenerateTreeCallBack = RegenerateTreeCallback;
                //
                _modelTree.GeometryMeshResultsEvent += ModelTree_ViewEvent;
                _modelTree.SelectEvent += ModelTree_Select;
                _modelTree.ClearSelectionEvent += Clear3DSelection;
                _modelTree.CreateEvent += ModelTree_CreateEvent;
                _modelTree.EditEvent += ModelTree_Edit;
                _modelTree.QueryEvent += ModelTree_Query;
                _modelTree.DuplicateEvent += ModelTree_DuplicateEvent;
                _modelTree.PropagateEvent += ModelTree_PropagateEvent;
                _modelTree.PreviewEvent += ModelTree_PreviewEvent;
                _modelTree.CreateCompoundPart += CreateAndImportCompoundPart;
                _modelTree.SwapPartGeometries += SwapPartGeometries;
                _modelTree.PreviewEdgeMesh += PreviewEdgeMeshesAsync;
                _modelTree.CreateMeshEvent += CreatePartMeshes;
                _modelTree.CopyGeometryToResultsEvent += CopyGeometryPartsToResults;
                _modelTree.EditCalculixKeywords += EditCalculiXKeywords;
                _modelTree.MergeParts += MergeModelParts;
                _modelTree.MergeResultParts += MergeResultParts;
                _modelTree.ConvertElementSetsToMeshParts += ConvertElementSetsToMeshParts;
                _modelTree.MaterialLibrary += ShowMaterialLibrary;
                _modelTree.SearchContactPairs += SearchContactPairs;
                _modelTree.SwapMasterSlave += ModelTree_SwapMasterSlave;
                _modelTree.MergeByMasterSlave += ModelTree_MergeByMasterSlave;
                _modelTree.HideShowEvent += ModelTree_HideShowEvent;
                _modelTree.SetTransparencyEvent += ModelTree_SetTransparencyEvent;
                _modelTree.ColorContoursVisibilityEvent += ModelTree_ColorContoursVisibilityEvent;
                _modelTree.RunEvent += RunAnalysis;
                _modelTree.CheckModelEvent += CheckModel;
                _modelTree.MonitorEvent += MonitorAnalysis;
                _modelTree.ResultsEvent += ResultsAnalysis;
                _modelTree.KillEvent += KillAnalysis;
                _modelTree.ActivateDeactivateEvent += ModelTree_ActivateDeactivateEvent;
                _modelTree.DeleteEvent += ModelTree_Delete;
                _modelTree.FieldDataSelectEvent += ModelTree_FieldDataSelectEvent;
                _modelTree.RenderingOff += () => _vtk.RenderingOn = false;
                _modelTree.RenderingOn += () => _vtk.RenderingOn = true;
                // Strip menus
                tsFile.Location = new Point(0, 0);
                tsViews.Location = new Point(tsFile.Left + tsFile.Width, 0);
                tsModel.Location = new Point(tsViews.Left + tsViews.Width, 0);
                tsDeformationFactor.Location = new Point(0, tsFile.Height);
                tsResults.Location = new Point(tsDeformationFactor.Left + tsDeformationFactor.Width, tsFile.Height);
                tscbSymbolsForStep.SelectedIndexChanged += tscbSymbolsForStep_SelectedIndexChanged;
                // Controller
                _controller = new Controller(this);
                // Vtk
                _vtk.OnMouseLeftButtonUpSelection += SelectPointOrArea;
                _vtk.Controller_GetAnnotationText += _controller.GetAnnotationText;
                _vtk.Controller_GetNodeActorData = _controller.GetNodeActorData;
                _vtk.Controller_GetCellActorData = _controller.GetCellActorData;
                _vtk.Controller_GetCellFaceActorData = _controller.GetCellFaceActorData;
                _vtk.Controller_GetEdgeActorData = _controller.GetEdgeActorData;
                _vtk.Controller_GetSurfaceEdgesActorDataFromElementId = _controller.GetSurfaceEdgesActorDataFromElementId;
                _vtk.Controller_GetSurfaceEdgesActorDataFromNodeAndElementIds =
                    _controller.GetSurfaceEdgesActorDataFromNodeAndElementIds;
                _vtk.Controller_GetPartActorData = _controller.GetPartActorData;
                _vtk.Controller_GetGeometryActorData = _controller.GetGeometryActorData;
                _vtk.Controller_GetGeometryVertexActorData = _controller.GetGeometryVertexActorData;
                _vtk.Controller_ActorsPicked = SelectBaseParts;
                _vtk.Form_ShowColorBarSettings = ShowColorBarSettings;
                _vtk.Form_ShowLegendSettings = ShowLegendSettings;
                _vtk.Form_ShowStatusBlockSettings = ShowStatusBlockSettings;
                _vtk.Form_EndEditArrowWidget = EndEditArrowAnnotation;
                _vtk.Form_WidgetPicked = AnnotationPicked;
                // Forms
                _formLocation = new Point(100, 100);
                _allForms = new List<Form>();
                //
                _frmSelectEntity = new FrmSelectEntity(_controller);
                AddFormToAllForms(_frmSelectEntity);
                //
                _frmSelectGeometry = new FrmSelectGeometry(_controller);
                AddFormToAllForms(_frmSelectGeometry);
                //
                _frmSelectItemSet = new FrmSelectItemSet(_controller);
                AddFormToAllForms(_frmSelectItemSet);
                //
                _frmSectionView = new FrmSectionView(_controller);
                AddFormToAllForms(_frmSectionView);
                //
                _frmExplodedView = new FrmExplodedView(_controller);
                _frmExplodedView.Clear3D = Clear3DSelection;
                AddFormToAllForms(_frmExplodedView);
                //
                _frmNewModel = new FrmNewModel(_controller);
                AddFormToAllForms(_frmNewModel);
                //
                _frmAnalyzeGeometry = new FrmAnalyzeGeometry(_controller);
                AddFormToAllForms(_frmAnalyzeGeometry);
                //
                _frmMeshingParameters = new FrmMeshingParameters(_controller);
                _frmMeshingParameters.PreviewEdgeMeshesAsync = PreviewEdgeMeshesAsync;
                AddFormToAllForms(_frmMeshingParameters);
                //
                _frmMeshRefinement = new FrmMeshRefinement(_controller);
                _frmMeshRefinement.PreviewEdgeMeshesAsync = PreviewEdgeMeshesAsync;
                AddFormToAllForms(_frmMeshRefinement);
                //
                _frmModelProperties = new FrmModelProperties(_controller);
                AddFormToAllForms(_frmModelProperties);
                //
                _frmPartProperties = new FrmPartProperties(_controller);
                AddFormToAllForms(_frmPartProperties);
                //
                _frmBoundaryLayer = new FrmBoundaryLayer(_controller);
                AddFormToAllForms(_frmBoundaryLayer);
                //
                _frmRemeshingParameters = new FrmRemeshingParameters(_controller);
                AddFormToAllForms(_frmRemeshingParameters);
                //
                _frmTranslate = new FrmTranslate(_controller);
                AddFormToAllForms(_frmTranslate);
                //
                _frmScale = new FrmScale(_controller);
                _frmScale.ScaleGeometryPartsAsync = ScaleGeometryPartsAsync;
                AddFormToAllForms(_frmScale);
                //
                _frmRotate = new FrmRotate(_controller);
                AddFormToAllForms(_frmRotate);
                //
                _frmNodeSet = new FrmNodeSet(_controller);
                AddFormToAllForms(_frmNodeSet);
                //
                _frmElementSet = new FrmElementSet(_controller);
                AddFormToAllForms(_frmElementSet);
                //
                _frmSurface = new FrmSurface(_controller);                
                AddFormToAllForms(_frmSurface);
                //
                _frmReferencePoint = new FrmReferencePoint(_controller);
                AddFormToAllForms(_frmReferencePoint);
                //
                _frmMaterial = new FrmMaterial(_controller);
                AddFormToAllForms(_frmMaterial);
                //
                _frmSection = new FrmSection(_controller);
                AddFormToAllForms(_frmSection);
                //
                _frmConstraint = new FrmConstraint(_controller);
                AddFormToAllForms(_frmConstraint);
                //
                _frmSurfaceInteraction = new FrmSurfaceInteraction(_controller);
                AddFormToAllForms(_frmSurfaceInteraction);
                //
                _frmContactPair = new FrmContactPair(_controller);
                AddFormToAllForms(_frmContactPair);
                //
                _frmSearchContactPairs = new FrmSearchContactPairs(_controller);
                AddFormToAllForms(_frmSearchContactPairs);
                //
                _frmInitialCondition = new FrmInitialCondition(_controller);
                AddFormToAllForms(_frmInitialCondition);
                //
                _frmAmplitude = new FrmAmplitude(_controller);
                AddFormToAllForms(_frmAmplitude);
                //
                _frmStep = new FrmStep(_controller);
                AddFormToAllForms(_frmStep);
                //
                _frmHistoryOutput = new FrmHistoryOutput(_controller);
                AddFormToAllForms(_frmHistoryOutput);
                //
                _frmFieldOutput = new FrmFieldOutput(_controller);
                AddFormToAllForms(_frmFieldOutput);
                //
                _frmBoundaryCondition = new FrmBC(_controller);
                AddFormToAllForms(_frmBoundaryCondition);
                //
                _frmLoad = new FrmLoad(_controller);
                AddFormToAllForms(_frmLoad);
                //
                _frmDefinedField = new FrmDefinedField(_controller);
                AddFormToAllForms(_frmDefinedField);
                //
                _frmAnalysis = new FrmAnalysis(_controller);
                AddFormToAllForms(_frmAnalysis);
                //
                _frmMonitor = new FrmMonitor(_controller);
                _frmMonitor.KillJob += KillAnalysis;
                _frmMonitor.Results += ResultsAnalysis;
                AddFormToAllForms(_frmMonitor);
                //
                _frmSettings = new FrmSettings();
                _frmSettings.UpdateSettings += UpdateSettings;
                AddFormToAllForms(_frmSettings);
                //
                _frmQuery = new FrmQuery();
                _frmQuery.Form_WriteDataToOutput = WriteDataToOutput;
                _frmQuery.Form_RemoveAnnotations = tsbRemoveAnnotations_Click;
                AddFormToAllForms(_frmQuery);
                //
                _frmFind = new FrmFind();
                _frmFind.Form_RemoveAnnotations = tsbRemoveAnnotations_Click;
                AddFormToAllForms(_frmFind);
                //
                _frmAnimation = new FrmAnimation();
                _frmAnimation.Form_ControlsEnable = DisableEnableControlsForAnimation;
                AddFormToAllForms(_frmAnimation);
                //
                _frmResultFieldOutput = new FrmResultFieldOutput(_controller);
                AddFormToAllForms(_frmResultFieldOutput);
                //
                _frmViewResultHistoryOutput = new FrmViewResultHistoryOutput(_controller);
                AddFormToAllForms(_frmViewResultHistoryOutput);
                //
                _frmResultHistoryOutput = new FrmResultHistoryOutput(_controller);
                AddFormToAllForms(_frmResultHistoryOutput);
                //
                _frmTransformation = new FrmTransformation(_controller);
                AddFormToAllForms(_frmTransformation);
                //
                _vtk.Hide();
                _vtk.Enabled = false;
                // Deformation toolstrip
                InitializeDeformationComboBoxes();
                InitializeComplexComboBoxes();
                // Converters
                tstbDeformationFactor.UnitConverter = new StringDoubleConverter();
                tstbAngle.UnitConverter = new StringAngleDegConverter();
                // Create the Keyboard Hook
                _keyboardHook = new KeyboardHook();
                // Capture the events
                _keyboardHook.KeyDown += KeyboardHook_KeyDown;
                // Install the hook
                _keyboardHook.Install();
            }
            catch
            {
                // If no error the splash is closed latter
                splash.BeginInvoke((MethodInvoker)delegate () { splash.Close(); });
            }
            finally
            {
                this.TopMost = false;
            }
            //
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                tsmiTest.Visible = false;
                tsmiCropWithCylinder.Visible = false;
                tsmiCropWithCube.Visible = false;
            }
        }
        //
        private void FrmMain_Shown(object sender, EventArgs e)
        {
            // Set vtk control size
            UpdateVtkControlSize();
            //
            _vtk.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            // Set pass through control for the mouse wheel event
            this.PassThroughControl = _vtk;
            // Timer to delay the rendering of the vtk so that menus get rendered first
            Timer timer = new Timer();
            timer.Interval = 50;
            timer.Tick += new EventHandler(async (object s, EventArgs ea) =>
            {
                timer.Stop();
                // Set form size
                _controller.Settings.General.ApplyFormSize(this);
                // Vtk
                _vtk.Enabled = true;
                // Reduce flicker
                _vtk.Left += _vtk.Width;
                _vtk.Visible = true;
                _controller.Redraw();
                _vtk.SetZoomFactor(1000);    // set starting zoom larger that the object
                Application.DoEvents();
                _vtk.Left -= _vtk.Width;
                _vtk.Visible = false;
                // Close splash 
                splash.BeginInvoke((MethodInvoker)delegate () { splash.Close(); });
                // At the end when vtk is loaded open the file
                string fileName = null;
                UnitSystemType unitSystemType = UnitSystemType.Undefined;
                //
                try
                {
                    // Try to recover unsaved progess due to crushed PrePoMax
                    if (File.Exists(_controller.GetHistoryFileNameBin()))
                    {
                        if (MessageBoxes.ShowWarningQuestion("A recovery file from a previous PrePoMax session exists. " +
                                                             "Would you like to try to recover it?") == DialogResult.OK)
                        {
                            fileName = _controller.GetHistoryFileNameBin();
                        }
                    }
                    if (fileName == null)
                    {
                        // Open file from exe arguments
                        if (_args != null && _args.Length >= 1)
                        {
                            fileName = _args[0];
                            unitSystemType = _controller.Settings.General.UnitSystemType;
                            for (int i = 1; i < _args.Length; i++)
                            {
                                if (_args[i].ToUpper() == "-US" && i + 1 < _args.Length)
                                {
                                    if (!Enum.TryParse(_args[i + 1].ToUpper(), out unitSystemType))
                                        throw new CaeException("The unit system type " + _args[i + 1] + " is not supported.");
                                }
                            }
                        }
                        // Check for open last file
                        else if (_controller.Settings.General.OpenLastFile) fileName = _controller.OpenedFileName;
                    }
                    //
                    if (File.Exists(fileName))
                    {
                        fileName = Path.GetFullPath(fileName);  // change local file name to global
                        string extension = Path.GetExtension(fileName).ToLower();
                        HashSet<string> importExtensions = GetFileImportExtensions();
                        //
                        if (extension == ".pmx" || extension == ".pmh" || extension == ".frd")
                            await Task.Run(() => OpenAsync(fileName, _controller.Open));
                        else if (importExtensions.Contains(extension))
                        {
                            // Create new model
                            if (New(ModelSpaceEnum.ThreeD, unitSystemType))
                            {
                                // Import
                                await _controller.ImportFileAsync(fileName, false);
                                // Set to null, otherwise the previous OpenedFileName gets overwriten on Save
                                _controller.OpenedFileName = null; 
                            }
                        }
                        else MessageBoxes.ShowError("The file name extension is not supported.");
                        //
                        _vtk.SetFrontBackView(false, true);
                    }
                    else
                    {
                        _controller.CurrentView = ViewGeometryModelResults.Geometry;
                        //
                        UpdateRecentFilesThreadSafe(_controller.Settings.General.GetRecentFiles());
                    }
                }
                catch (Exception ex)
                {
                    ExceptionTools.Show(this, ex);
                    _controller.ModelChanged = false;   // hide messagebox
                    tsmiNew_Click(null, null);
                }
                finally
                {                    
                }
            });
            timer.Start();
        }
        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && _frmAnimation.Visible) _frmAnimation.UpdateAnimation();
        }
        private async void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = false;   // close the form
                DialogResult response = DialogResult.None;
                // No write access
                if (_controller == null) return;
                //
                foreach (var entry in _controller.Jobs)
                {
                    if (entry.Value.JobStatus == JobStatus.Running)
                    {
                        response = MessageBoxes.ShowWarningQuestion("There is an analysis running." +
                                                                    " Closing will kill the analysis. Close anyway?");
                        if (response == DialogResult.Cancel) e.Cancel = true;
                        else if (response == DialogResult.OK) _controller.KillAllJobs();
                        break;
                    }
                }
                //
                if (tsslState.Text != Globals.ReadyText)
                {
                    response = MessageBoxes.ShowWarningQuestion("There is a task running. Close anyway?");
                    if (response == DialogResult.Cancel) e.Cancel = true;
                    else if (response == DialogResult.OK && _controller.SavingFile)
                    {
                        while (_controller.SavingFile) System.Threading.Thread.Sleep(100);
                    }
                }
                else if (_controller.ModelChanged)
                {
                    response = MessageBox.Show("Save file before closing?",
                                               "Warning",
                                               MessageBoxButtons.YesNoCancel,
                                               MessageBoxIcon.Warning);
                    if (response == DialogResult.Yes)
                    {
                        e.Cancel = true;                                // stop the form from closing before saving
                        await Task.Run(() => _controller.Save());       // save
                        Close();                                        // close the control
                    }
                    else if (response == DialogResult.Cancel) e.Cancel = true;
                }
                // Save form size and location and delete history files
                if (e.Cancel == false && _controller != null)
                {
                    _controller.Settings.General.SaveFormSize(this);
                    _controller.Settings.SaveToFile();
                    //
                    _controller.DeleteHistoryFiles();
                    //
                    _vtk.Clear();
                    _vtk.Dispose();
                    _vtk = null;
                }
            }
            catch
            { }
        }
        private void FrmMain_Move(object sender, EventArgs e)
        {
            if (_allForms == null) return;

            foreach (Form form in _allForms)
            {
                if (form.Visible)
                {
                    //form.Location = new Point(Left + _formLocation.X, Top + _formLocation.Y);
                }
            }
        }
        private void UpdateVtkControlSize()
        {
            // Update vtk control size
            _vtk.Location = panelControl.Location;
            _vtk.Top += 1;
            _vtk.Left += 1;
            //
            _vtk.Size = panelControl.Size;
            _vtk.Width -= 2;
            _vtk.Height -= 2;
        }
        private bool TestWriteAccess()
        {
            try
            {
                string fileName = Tools.GetNonExistentRandomFileName(Application.StartupPath, ".test");
                File.WriteAllText(fileName, "");
                //
                File.Delete(fileName);
                //
                return true;
            }
            catch
            {
                return false;
            }
        }
        // Forms
        private void itemForm_VisibleChanged(object sender, EventArgs e)
        {
            Form form = sender as Form;
            int count = 0;
            // One or two forms can be open
            foreach (var aForm in _allForms)
            {
                // Do not count the Query form
                if (aForm.Visible) count++;
            }
            // Disable model tree mouse and keyboard actions for the form
            bool unactive;
            if (count > 0) unactive = true;
            else unactive = false;
            //
            _modelTree.DisableMouse = unactive;
            menuStripMain.DisableMouseButtons = unactive;
            tsFile.DisableMouseButtons = unactive;
            tsModel.Enabled = !unactive; // changing the symbols clears the selection - unwanted during selection
            // This gets also called from item selection form: by angle, by edge ...
            if (form.Visible == false)
            {
                UpdateHighlightFromTree();
                SaveFormLoaction(form);
                //
                _controller.SetSelectByToDefault();
                //
                this.Focus();
            }
        }
        private void itemForm_Move(object sender, EventArgs e)
        {
            Form form = sender as Form;
            //Size screenSize =  Screen.GetWorkingArea(form).Size;
            //if (form.Left < 0) form.Left = 0;
            //else if (form.Left + form.Width > screenSize.Width) form.Left = screenSize.Width - form.Width;
            //if (form.Top < 0) form.Top = 0;
            //else if (form.Top + form.Height > screenSize.Height) form.Top = screenSize.Height - form.Height;
            SaveFormLoaction(form);
        }
        // Keyboard
        private void KeyboardHook_KeyDown(KeyboardHook.VKeys vKey)
        {
            if (this == ActiveForm)
            {
                Keys key = (Keys)vKey;
                //
                if (key == Keys.Escape)
                {
                    if (!_vtk.IsRubberBandActive) CloseAllForms();
                }
                else if (Control.ModifierKeys == Keys.Control)
                {
                    //if (key == Keys.I) tsmiImportFile_Click(null, null);
                    //else if (key == Keys.N) tsmiNew_Click(null, null);
                    //else if (key == Keys.O) tsmiOpen_Click(null, null);
                    //else if (key == Keys.S) tsmiSave_Click(null, null);
                    //else if (key == Keys.X) tsmiExit_Click(null, null);
                }
                // Model tree
                else if (_modelTree.ActiveControl == null || !_modelTree.ActiveControl.Focused)
                {
                    Control focusedControl = FindFocusedControl(this);
                    // Check for toolstrip
                    if (focusedControl != null && focusedControl.Parent is ToolStripFocus) { }
                    // Check for annotation editor
                    else if (aeAnnotationTextEditor.Visible) { }
                    // Forward to tree
                    else _modelTree.cltv_KeyDown(this, new KeyEventArgs(key));
                }
            }
        }
        public static Control FindFocusedControl(Control control)
        {
            var container = control as IContainerControl;
            while (container != null)
            {
                control = container.ActiveControl;
                container = control as IContainerControl;
            }
            return control;
        }
        // Timer
        private void timerOutput_Tick(object sender, EventArgs e)
        {
            tbOutput.Lines = outputLines;
            tbOutput.SelectionStart = tbOutput.Text.Length;
            tbOutput.ScrollToCaret();
            timerOutput.Stop();
        }

        #region ModelTree Events ###################################################################################################
        //
        internal void ModelTree_ViewEvent(ViewType viewType)
        {
            try
            {
                if ((viewType == ViewType.Geometry && GetCurrentView() == ViewGeometryModelResults.Geometry) ||
                    (viewType == ViewType.Model && GetCurrentView() == ViewGeometryModelResults.Model) ||
                    (viewType == ViewType.Results && GetCurrentView() == ViewGeometryModelResults.Results)) return;
                //
                CloseAllForms();
                _controller.SelectBy = vtkSelectBy.Default;
                //
                if (viewType == ViewType.Geometry) _controller.CurrentView = ViewGeometryModelResults.Geometry;
                else if (viewType == ViewType.Model) _controller.CurrentView = ViewGeometryModelResults.Model;
                else if (viewType == ViewType.Results) _controller.CurrentView = ViewGeometryModelResults.Results;
                else throw new NotSupportedException();
                //
                if (_advisorControl != null) _advisorControl.PrepareControls(viewType);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void ModelTree_Select(NamedClass[] items)
        {
            try
            {
                _controller.Highlight3DObjects(items);
            }
            catch
            {
            }
        }       
        //
        private void ModelTree_CreateEvent(string nodeName, string stepName)
        {
            if (_controller.Model.Geometry != null && _controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
                if (nodeName == _modelTree.MeshingParametersName) tsmiCreateMeshingParameters_Click(null, null);
                else if (nodeName == _modelTree.MeshRefinementsName) tsmiCreateMeshRefinement_Click(null, null);
            }
            else if (_controller.Model.Mesh != null && _controller.CurrentView == ViewGeometryModelResults.Model)
            {
                // _controller.Model.Mesh defines the unit system and must
                if (nodeName == _modelTree.NodeSetsName) tsmiCreateNodeSet_Click(null, null);
                else if (nodeName == _modelTree.ElementSetsName) tsmiCreateElementSet_Click(null, null);
                else if (nodeName == _modelTree.SurfacesName) tsmiCreateSurface_Click(null, null);
                else if (nodeName == _modelTree.ReferencePointsName) tsmiCreateRP_Click(null, null);
                else if (nodeName == _modelTree.MaterialsName) tsmiCreateMaterial_Click(null, null);
                else if (nodeName == _modelTree.SectionsName) tsmiCreateSection_Click(null, null);
                else if (nodeName == _modelTree.ConstraintsName) tsmiCreateConstraint_Click(null, null);
                else if (nodeName == _modelTree.SurfaceInteractionsName) tsmiCreateSurfaceInteraction_Click(null, null);
                else if (nodeName == _modelTree.ContactPairsName) tsmiCreateContactPair_Click(null, null);
                else if (nodeName == _modelTree.AmplitudesName) tsmiCreateAmplitude_Click(null, null);
                else if (nodeName == _modelTree.InitialConditionsName) tsmiCreateInitialCondition_Click(null, null);
                else if (nodeName == _modelTree.StepsName) tsmiCreateStep_Click(null, null);
                else if (nodeName == _modelTree.HistoryOutputsName && stepName != null) CreateHistoryOutput(stepName);
                else if (nodeName == _modelTree.FieldOutputsName && stepName != null) CreateFieldOutput(stepName);
                else if (nodeName == _modelTree.BoundaryConditionsName && stepName != null) CreateBoundaryCondition(stepName);
                else if (nodeName == _modelTree.LoadsName && stepName != null) CreateLoad(stepName);
                else if (nodeName == _modelTree.DefinedFieldsName && stepName != null) CreateDefinedField(stepName);
                else if (nodeName == _modelTree.AnalysesName) tsmiCreateAnalysis_Click(null, null);
            }
            else if (_controller.CurrentResult != null && _controller.CurrentResult.Mesh != null &&
                     _controller.CurrentView == ViewGeometryModelResults.Results)
            {
                if (nodeName == _modelTree.FieldOutputsName) tsmiCreateResultFieldOutput_Click(null, null);
                else if (nodeName == _modelTree.HistoryOutputsName) tsmiCreateResultHistoryOutput_Click(null, null);
            }
        }
        private void ModelTree_Edit(NamedClass namedClass, string stepName)
        {
            // Geometry
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
                if (namedClass is GeometryPart) EditGeometryPart(namedClass.Name);
                else if (namedClass is MeshingParameters) EditMeshingParameters(namedClass.Name);
                else if (namedClass is FeMeshRefinement) EditMeshRefinement(namedClass.Name);
            }
            // Model
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                if (namedClass is EmptyNamedClass) // empty named class is used to trasfer the name only
                {
                    if (namedClass.Name == typeof(FeModel).ToString()) tsmiEditModel_Click(null, null);
                }
                else if (namedClass is MeshPart) EditModelPart(namedClass.Name);
                else if (namedClass is FeNodeSet) EditNodeSet(namedClass.Name);
                else if (namedClass is FeElementSet) EditElementSet(namedClass.Name);
                else if (namedClass is FeSurface) EditSurface(namedClass.Name);
                else if (namedClass is FeReferencePoint) EditRP(namedClass.Name);
                else if (namedClass is Material) EditMaterial(namedClass.Name);
                else if (namedClass is Section) EditSection(namedClass.Name);
                else if (namedClass is CaeModel.Constraint) EditConstraint(namedClass.Name);
                else if (namedClass is SurfaceInteraction) EditSurfaceInteraction(namedClass.Name);
                else if (namedClass is ContactPair) EditContactPair(namedClass.Name);
                else if (namedClass is Amplitude) EditAmplitude(namedClass.Name);
                else if (namedClass is InitialCondition) EditInitialCondition(namedClass.Name);
                else if (namedClass is Step) EditStep(namedClass.Name);
                else if (namedClass is HistoryOutput) EditHistoryOutput(stepName, namedClass.Name);
                else if (namedClass is FieldOutput) EditFieldOutput(stepName, namedClass.Name);
                else if (namedClass is BoundaryCondition) EditBoundaryCondition(stepName, namedClass.Name);
                else if (namedClass is Load) EditLoad(stepName, namedClass.Name);
                else if (namedClass is DefinedField) EditDefinedField(stepName, namedClass.Name);
                else if (namedClass is AnalysisJob) EditAnalysis(namedClass.Name);
            }
            // Results
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                if (namedClass is ResultPart || namedClass is GeometryPart) EditResultPart(namedClass.Name);
                else if (namedClass is ResultFieldOutput rfo) EditResultFieldOutput(rfo.Name);
                else if (namedClass is HistoryResultData hd) ViewResultHistoryOutputData(hd);
                else if (namedClass is FieldData fd) ShowLegendSettings();
            }
        }
        private void ModelTree_Query()
        {
            tsmiQuery_Click(null, null);
        }
        private void ModelTree_DuplicateEvent(NamedClass[] items)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            { }
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                ApplyActionOnItems<FeNodeSet>(items, DuplicateNodeSets);
                ApplyActionOnItems<FeElementSet>(items, DuplicateElementSets);
                //
                ApplyActionOnItems<Material>(items, DuplicateMaterials);
                //
                ApplyActionOnItems<CaeModel.Constraint>(items, DuplicateConstraints);
                //
                ApplyActionOnItems<SurfaceInteraction>(items, DuplicateSurfaceInteractions);
                //
                ApplyActionOnItems<Step>(items, DuplicateSteps);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            { }
        }
        private void ModelTree_PropagateEvent(NamedClass[] items, string[] stepNames)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                ApplyActionOnItemsInStep<HistoryOutput>(items, stepNames, PropagateHistoryOutput);
                ApplyActionOnItemsInStep<FieldOutput>(items, stepNames, PropagateFieldOutput);
                ApplyActionOnItemsInStep<BoundaryCondition>(items, stepNames, PropagateBoundaryCondition);
                ApplyActionOnItemsInStep<Load>(items, stepNames, PropagateLoad);
                ApplyActionOnItemsInStep<DefinedField>(items, stepNames, PropagateDefinedField);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
            }
        }
        private void ModelTree_PreviewEvent(NamedClass[] items, string[] stepNames)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                ApplyActionOnItems<InitialCondition>(items, PreviewInitialCondition);
                ApplyActionOnItemsInStep<Load>(items, stepNames, PreviewLoad);
                ApplyActionOnItemsInStep<DefinedField>(items, stepNames, PreviewDefinedField);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
            }
        }
        //
        private void ModelTree_SwapMasterSlave(NamedClass[] items)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            { }
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                ApplyActionOnItems<CaeModel.Constraint>(items, SwapMasterSlaveConstraints);
                ApplyActionOnItems<ContactPair>(items, SwapMasterSlaveContactPairs);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            { }
        }
        private void ModelTree_MergeByMasterSlave(NamedClass[] items)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            { }
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                ApplyActionOnItems<CaeModel.Constraint>(items, MergeByMasterSlaveConstraints);
                ApplyActionOnItems<ContactPair>(items, MergeByMasterSlaveContactPairs);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            { }
        }
        //
        private void ModelTree_HideShowEvent(NamedClass[] items, HideShowOperation operation, string[] stepNames)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
                HideShowItems<GeometryPart>(items, operation, HideGeometryParts, ShowGeometryParts, ShowOnlyGeometryParts);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                HideShowItems<MeshPart>(items, operation, HideModelParts, ShowModelParts, ShowOnlyModelParts);
                HideShowItems<FeReferencePoint>(items, operation, HideRPs, ShowRPs, ShowOnlyRPs);
                HideShowItems<CaeModel.Constraint>(items, operation, HideConstraints, ShowConstraints, ShowOnlyConstraints);
                HideShowItems<ContactPair>(items, operation, HideContactPairs, ShowContactPairs, ShowOnlyContactPairs);
                HideShowStepItems<BoundaryCondition>(items, operation, stepNames, HideBoundaryConditions, 
                                                              ShowBoundaryConditions, ShowOnlyBoundaryConditions);
                HideShowStepItems<Load>(items, operation, stepNames, HideLoads, ShowLoads, ShowOnlyLoads);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                HideShowItems<ResultPart>(items, operation, HideResultParts, ShowResultParts, ShowOnlyResultParts);
                HideShowItems<GeometryPart>(items, operation, HideResultParts, ShowResultParts, ShowOnlyResultParts);
            }
        }
        private void ModelTree_SetTransparencyEvent(string[] partNames)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) SetTransparencyForGeometryParts(partNames);
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) SetTransparencyForModelParts(partNames);
            else if (_controller.CurrentView == ViewGeometryModelResults.Results) SetTransparencyForResultParts(partNames);
        }
        private void ModelTree_ColorContoursVisibilityEvent(NamedClass[] items, bool colorContours)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < items.Length; i++)
            {
                names.Add(items[i].Name);
            }
            //
            if (names.Count > 0)
            {
                if (colorContours) ColorContoursOnResultPart(names.ToArray());
                else ColorContoursOffResultPart(names.ToArray());
            }
        }
        //
        private void ModelTree_Delete(NamedClass[] items, string[] parentNames)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
                ApplyActionOnItems<MeshingParameters>(items, DeleteMeshingParameters);
                ApplyActionOnItems<FeMeshRefinement>(items, DeleteMeshRefinements);
                // At last delete the parts
                ApplyActionOnItems<GeometryPart>(items, DeleteGeometryParts);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                ApplyActionOnItems<FeNodeSet>(items, DeleteNodeSets);
                ApplyActionOnItems<FeElementSet>(items, DeleteElementSets);
                ApplyActionOnItems<FeSurface>(items, DeleteSurfaces);
                ApplyActionOnItems<FeReferencePoint>(items, DeleteRPs);
                ApplyActionOnItems<Material>(items, DeleteMaterials);
                ApplyActionOnItems<Section>(items, DeleteSections);
                ApplyActionOnItems<CaeModel.Constraint>(items, DeleteConstraints);
                ApplyActionOnItems<SurfaceInteraction>(items, DeleteSurfaceInteractions);
                ApplyActionOnItems<ContactPair>(items, DeleteContactPairs);
                ApplyActionOnItems<Amplitude>(items, DeleteAmplitudes);
                ApplyActionOnItems<InitialCondition>(items, DeleteInitialConditions);
                // First delete step items and then steps
                DeleteParentItems<HistoryOutput>(items, parentNames, DeleteHistoryOutputs);
                DeleteParentItems<FieldOutput>(items, parentNames, DeleteFieldOutputs);
                DeleteParentItems<BoundaryCondition>(items, parentNames, DeleteBoundaryConditions);
                DeleteParentItems<Load>(items, parentNames, DeleteLoads);
                DeleteParentItems<DefinedField>(items, parentNames, DeleteDefinedFields);
                ApplyActionOnItems<Step>(items, DeleteSteps);
                //
                ApplyActionOnItems<AnalysisJob>(items, DeleteAnalyses);
                // At last delete the parts
                ApplyActionOnItems<MeshPart>(items, DeleteModelParts);
            }
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                ApplyActionOnItems<ResultPart>(items, DeleteResultParts);
                ApplyActionOnItems<GeometryPart>(items, DeleteResultParts);
                // First delete components and then field outputs
                DeleteParentItems<FieldData>(items, parentNames, DeleteResultFieldOutputComponents);
                ApplyActionOnItems<Field>(items, DeleteResultFieldOutputs);
                ApplyActionOnItems<ResultFieldOutput>(items, DeleteResultFieldOutputs);
                //
                DeleteResultHistoryResultCompoments(items);
                DeleteParentItems<HistoryResultField>(items, parentNames, DeleteResultHistoryResultFields);
                ApplyActionOnItems<HistoryResultSet>(items, RemoveResultHistoryResultSets);
                
            }
        }
        private void ModelTree_ActivateDeactivateEvent(NamedClass[] items, bool activate, string[] stepNames)
        {
            _controller.ActivateDeactivateMultipleCommand(items, activate, stepNames);
        }
        private void ModelTree_FieldDataSelectEvent(string[] obj)
        {
            try
            {
                SetFieldData(obj[0], obj[1], GetCurrentFieldOutputStepId(), GetCurrentFieldOutputStepIncrementId());
            }
            catch { }
        }
        //                                                                                                                          
        private void HideShowItems<T>(NamedClass[] items, HideShowOperation operation, Action<string[]> Hide, 
                                      Action<string[]> Show, Action<string[]> ShowOnly)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is T) names.Add(items[i].Name);
            }
            if (names.Count > 0)
            {
                if (operation == HideShowOperation.Hide) Hide(names.ToArray());
                else if (operation == HideShowOperation.Show) Show(names.ToArray());
                else if (operation == HideShowOperation.ShowOnly) ShowOnly(names.ToArray());
                else throw new NotSupportedException();
            }
        }
        private void HideShowStepItems<T>(NamedClass[] items, HideShowOperation operation, string[] stepNames, 
                                          Action<string, string[]> Hide,
                                          Action<string, string[]> Show, 
                                          Action<string, string[]> ShowOnly)
        {
            Dictionary<string, List<string>> stepItems = new Dictionary<string, List<string>>();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is T)
                {
                    if (stepItems.ContainsKey(stepNames[i])) stepItems[stepNames[i]].Add(items[i].Name);
                    else stepItems.Add(stepNames[i], new List<string>() { items[i].Name });
                }
            }
            if (stepItems.Count > 0)
            {
                foreach (var entry in stepItems)
                {
                    if (operation == HideShowOperation.Hide) Hide(entry.Key, entry.Value.ToArray());
                    else if (operation == HideShowOperation.Show) Show(entry.Key, entry.Value.ToArray());
                    else if (operation == HideShowOperation.ShowOnly) ShowOnly(entry.Key, entry.Value.ToArray());
                    else throw new NotSupportedException();
                }
            }
        }
        private void ApplyActionOnItems<T>(NamedClass[] items, Action<string[]> Action)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is T) names.Add(items[i].Name);
            }
            if (names.Count > 0) Action(names.ToArray());
        }
        private void ApplyActionOnItemsInStep<T>(NamedClass[] items, string[] steps, Action<string, string> Action)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is T) Action(steps[i], items[i].Name);
            }
        }
        private void DeleteParentItems<T>(NamedClass[] items, string[] parentNames, Action<string, string[]> Delete)
        {
            List<string> itemList;
            Dictionary<string, List<string>> parentItems = new Dictionary<string, List<string>>();
            //
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is T)
                {
                    if (parentItems.TryGetValue(parentNames[i], out itemList)) itemList.Add(items[i].Name);
                    else parentItems.Add(parentNames[i], new List<string>() { items[i].Name });
                }
            }
            if (parentItems.Count > 0)
            {
                foreach (var entry in parentItems)
                {
                    Delete(entry.Key, entry.Value.ToArray());
                }
            }
        }
        //                                                                                                                          
        
        #endregion #################################################################################################################


        // Menus                                                                                                                    
        private void SetMenuAndToolStripVisibility()
        {
            InvokeIfRequired(() =>
            {
                //                      Disable                                                         
                // Main menu
                foreach (ToolStripMenuItem item in menuStripMain.Items) item.Enabled = false;
                tsmiFile.Enabled = true;
                tsmiTools.Enabled = true;
                tsmiHelp.Enabled = true;
                // File menu
                foreach (ToolStripItem item in tsmiFile.DropDownItems) item.Enabled = false;
                // Tools menu
                tsmiQuery.Enabled = false;
                // Toolbar File
                tsbImport.Enabled = false;
                tsbSave.Enabled = false;
                // Toolbar View
                tsViews.DisableMouseButtons = true;
                // Toolbar Model
                tslSymbols.Enabled = false;
                tscbSymbolsForStep.Enabled = false;
                // Toolbar Results
                tsDeformationFactor.Enabled = false;
                tsResults.Enabled = false;
                // Vtk
                bool vtkVisible = false;
                // Tree
                SetStateWorking("Rendering...");
                //                      Enable                                                          
                if (_controller.ModelInitialized || _controller.ResultsInitialized)
                {
                    // Main menu
                    tsmiEdit.Enabled = true;
                    // File menu
                    foreach (ToolStripItem item in tsmiFile.DropDownItems) item.Enabled = true;
                    tsmiImportFile.Enabled = _controller.ModelInitialized;
                    // Tools menu
                    tsmiQuery.Enabled = true;
                    // Toolbar File
                    tsbImport.Enabled = _controller.ModelInitialized;
                    tsbSave.Enabled = true;                    
                }
                else
                {
                    // File menu
                    tsmiNew.Enabled = true;
                    tsmiOpen.Enabled = true;
                    tsmiOpenRecent.Enabled = true;
                    tsmiExit.Enabled = true;
                }
                //
                bool setGeometryView = _controller.CurrentView == ViewGeometryModelResults.Geometry;
                bool setModelView = _controller.CurrentView == ViewGeometryModelResults.Model;
                bool setResultsView = _controller.CurrentView == ViewGeometryModelResults.Results;
                bool setEmptyView = (setGeometryView && !_controller.ModelInitialized) ||
                                    (setModelView && !_controller.ModelInitialized) ||
                                    (setResultsView && !_controller.ResultsInitialized);
                
                // Only for individual views !!!
                if (setEmptyView) { }
                else if (setGeometryView)
                {
                    // Main menu
                    tsmiView.Enabled = true;
                    tsmiGeometry.Enabled = true;
                    tsmiMesh.Enabled = true;
                    // Toolbar View
                    tsViews.DisableMouseButtons = false;
                    // Vtk
                    vtkVisible = true;
                }
                else if (setModelView)
                {
                    // Main menu
                    tsmiView.Enabled = true;
                    tsmiModel.Enabled = true;
                    tsmiProperty.Enabled = true;
                    tsmiInteraction.Enabled = true;
                    tsmiInitialCondition.Enabled = true;
                    tsmiAmplitude.Enabled = true;
                    tsmiStepMenu.Enabled = true;
                    tsmiBC.Enabled = true;
                    tsmiLoad.Enabled = true;
                    tsmiAnalysis.Enabled = true;
                    // Toolbar View
                    tsViews.DisableMouseButtons = false;
                    // Toolbar Model
                    tslSymbols.Enabled = true;
                    tscbSymbolsForStep.Enabled = true;
                    // Vtk
                    vtkVisible = true;
                }
                else if (setResultsView)
                {
                    // Main menu
                    tsmiView.Enabled = true;
                    tsmiResults.Enabled = true;
                    // Toolbar View
                    tsViews.DisableMouseButtons = false;
                    // Toolbar Results
                    tsDeformationFactor.Enabled = true;
                    tsResults.Enabled = true;
                    // Vtk
                    vtkVisible = true;
                }
                // Tree
                SetStateReady("Rendering...");
                //                      Buttons                                                         
                tsbSectionView.Checked = _controller.IsSectionViewActive();
                tsbExplodedView.Checked = _controller.IsExplodedViewActive();
                tsbTransformation.Checked = _controller.AreTransformationsActive();
                //                      Icons                                                           
                UpdateResultsTypeIconStates();
                //
                UpdateComplexControlStates();
                //
                _vtk.Visible = vtkVisible;
            });
        }

        #region File menu ##########################################################################################################
        internal void tsmiNew_Click(object sender, EventArgs e)
        {
            New(ModelSpaceEnum.Undefined, UnitSystemType.Undefined);
        }
        private bool New(ModelSpaceEnum modelSpace, UnitSystemType unitSystemType)
        {
            try
            {
                if ((_controller.ModelChanged || _controller.ModelInitialized || _controller.ResultsInitialized) &&
                    MessageBoxes.ShowWarningQuestion("OK to close the current model?") != DialogResult.OK) return false;
                //
                _controller.DeInitialize();
                SetMenuAndToolStripVisibility();
                //
                bool update = false;
                // The model space and the unit system are undefined
                if (modelSpace == ModelSpaceEnum.Undefined || unitSystemType == UnitSystemType.Undefined)
                {
                    if (SelectNewModelProperties(true))
                    {
                        _controller.New();
                        SetNewModelProperties();
                        update = true;
                    }
                }
                else
                {
                    _controller.New();
                    _controller.SetNewModelPropertiesCommand(modelSpace, unitSystemType);
                    update = true;
                }
                //
                if (update)
                {
                    SetMenuAndToolStripVisibility();
                    //
                    _controller.ModelChanged = false; // must be here since adding a unit system changes the model
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            return true;
        }
        private async void tsmiOpen_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    // Debugger attached
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        openFileDialog.Filter = "All files|*.pmx;*.pmh;*.frd;*.dat;*.foam" +
                                                "|PrePoMax files|*.pmx" +
                                                "|PrePoMax history|*.pmh" +
                                                "|Calculix result files|*.frd" +
                                                "|Calculix dat files|*.dat" +       // added .dat file
                                                "|OpenFoam files|*.foam";

                    }
                    // No dedugger
                    else
                    {
                        openFileDialog.Filter = "All files|*.pmx;*.pmh;*.frd;*.foam" +
                                                "|PrePoMax files|*.pmx" +
                                                "|PrePoMax history|*.pmh" +
                                                "|Calculix result files|*.frd" +
                                                "|OpenFoam files|*.foam";
                    }

                    //
                    openFileDialog.FileName = "";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (CheckBeforeOpen(openFileDialog.FileName)) await OpenAsync(openFileDialog.FileName, _controller.Open);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
                _controller.ModelChanged = false;   // hide messagebox
                tsmiNew_Click(null, null);
            }
        }
        private bool CheckBeforeOpen(string fileName)
        {
            if (!File.Exists(fileName)) return false;
            //
            if (_controller.ModelChanged)
            {
                string extension = Path.GetExtension(fileName).ToLower();
                if (extension == ".pmx")
                {
                    if (MessageBoxes.ShowWarningQuestion("OK to close the current model?") != DialogResult.OK)
                        return false;
                }
                else if ((extension == ".frd" || extension == ".foam") && _controller.AllResults.ContainsResult(fileName))
                {
                    if (MessageBoxes.ShowWarningQuestion("OK to reopen the existing results?") != DialogResult.OK)
                        return false;
                }
            }
            return true;
        }
        private async Task OpenAsync(string fileName, Action<string> ActionOnFile, bool resetCamera = true, Action callback = null)
        {
            bool stateSet = false;
            try
            {
                if (SetStateWorking(Globals.OpeningText))
                {
                    stateSet = true;
                    await Task.Run(() => Open(fileName, ActionOnFile, resetCamera));
                    callback?.Invoke();                    
                }
                else MessageBoxes.ShowWarning("Another task is already running.");
                // If the model space or the unit system are undefined
                if (_controller.ModelInitialized) IfNeededSelectAndSetNewModelProperties();
                if (_controller.ResultsInitialized) SelectResultsUnitSystem();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                if (stateSet) SetStateReady(Globals.OpeningText);
            }
        }
        public void Open(string fileName, Action<string> ActionOnFile, bool resetCamera = true)
        {
            ActionOnFile(fileName);
            //
            if (_controller.CurrentResult != null && _controller.CurrentResult.Mesh != null)
            {
                SetResultNames();
                // Reset the previous step and increment
                SetAllStepAndIncrementIds(true);
                // Set last increment
                SetDefaultStepAndIncrementIds();
                // Show the selection in the results tree
                SelectFirstComponentOfFirstFieldOutput();
            }
            //
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) _controller.DrawGeometry(resetCamera);
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) _controller.DrawModel(resetCamera);
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                // Set the representation which also calls Draw
                _controller.ViewResultsType = ViewResultsTypeEnum.ColorContours;  // Draw
                //
                if (resetCamera) tsmiFrontView_Click(null, null);
            }
            else throw new NotSupportedException();
            //
            SetMenuAndToolStripVisibility();
        }
        internal void tsmiImportFile_Click(object sender, EventArgs e)
        {
            ImportFile(false);
        }        
        internal async void tsmiSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_controller.ModelInitialized && !_controller.ResultsInitialized)
                    throw new CaeException("There is no model or results to save.");
                //
                SetStateWorking(Globals.SavingText);
                await Task.Run(() => _controller.Save());
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.SavingText);
            }
        }
        private async void tsmiSaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_controller.ModelInitialized && !_controller.ResultsInitialized)
                    throw new CaeException("There is no model or results to save.");
                //
                SetStateWorking(Globals.SavingAsText);
                await Task.Run(() => _controller.SaveAs());
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.SavingAsText);
            }
        }
        //
        private void tsmiExportToStep_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.CurrentView = ViewGeometryModelResults.Geometry;
                //
                if (_controller.Model.Geometry != null && _controller.Model.Geometry.Parts != null)
                {
                    SelectMultipleEntities("Parts", _controller.GetCADGeometryParts(), SaveCADPartsAsStep);
                }
                else throw new CaeException("No geometry to export.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiExportToBrep_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.CurrentView = ViewGeometryModelResults.Geometry;
                //
                if (_controller.Model.Geometry != null && _controller.Model.Geometry.Parts != null)
                {
                    SelectMultipleEntities("Parts", _controller.GetCADGeometryParts(), SaveCADPartsAsBrep);
                }
                else throw new CaeException("No geometry to export.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiExportToStereolitography_Click(object sender, EventArgs e)
        {
            try
            {
                ViewGeometryModelResults currentView = GetCurrentView();
                if (currentView == ViewGeometryModelResults.Geometry)
                {
                    if (_controller.Model.Geometry != null && _controller.Model.Geometry.Parts != null)
                    {
                        SelectMultipleEntities("Parts", _controller.GetGeometryParts(), SavePartsAsStl);
                    }
                    else throw new CaeException("No geometry parts to export.");
                }
                else if (currentView == ViewGeometryModelResults.Model)
                {
                    if (_controller.Model.Mesh != null && _controller.Model.Mesh.Parts != null)
                    {
                        SelectMultipleEntities("Parts", _controller.GetModelParts(), SavePartsAsStl);
                    }
                    else throw new CaeException("No mesh parts to export.");
                }
                else MessageBoxes.ShowError("Deformed mesh can only be exported while results are drawn.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiExportToCalculix_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.CurrentView = ViewGeometryModelResults.Model;
                //
                if (CheckValiditiy())
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Calculix files | *.inp";
                        if (_controller.OpenedFileName != null)
                            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_controller.OpenedFileName) + ".inp";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            // the filter adds the extension to the file name
                            SetStateWorking(Globals.ExportingText);
                            _controller.ExportToCalculix(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        private void tsmiExportToAbaqus_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.CurrentView = ViewGeometryModelResults.Model;
                //
                if (CheckValiditiy())
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Abaqus files | *.inp";
                        if (_controller.OpenedFileName != null)
                            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_controller.OpenedFileName) + ".inp";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            // the filter adds the extension to the file name
                            SetStateWorking(Globals.ExportingText);
                            //_controller.ExportToCalculix(saveFileDialog.FileName);
                            _controller.ExportToAbaqus(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        private void tsmiExportToMmgMesh_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.CurrentView = ViewGeometryModelResults.Model;
                //
                if (_controller.Model.Geometry != null && _controller.Model.Geometry.Parts != null)
                {
                    SelectMultipleEntities("Parts", _controller.GetGeometryParts(), SavePartsAsMmgMesh);
                }
                else throw new CaeException("No geometry to export.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiExportToDeformedInp_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.CurrentView = ViewGeometryModelResults.Results;
                //
                if (_controller.CurrentResult.Mesh != null && _controller.CurrentResult.Mesh.Parts != null)
                {
                    SelectMultipleEntities("Parts", _controller.GetResultParts(), SaveDeformedPartsAsInp);
                }
                else MessageBoxes.ShowError("There is no mesh to export.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }

        }
        private void tsmiExportToDeformedStl_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.CurrentView = ViewGeometryModelResults.Results;
                //
                if (_controller.CurrentResult.Mesh != null && _controller.CurrentResult.Mesh.Parts != null)
                {
                    SelectMultipleEntities("Parts", _controller.GetResultParts(), SavePartsAsStl);
                }
                else MessageBoxes.ShowError("There is no mesh to export.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private async void ImportFile(bool onlyMaterials)
        {
            try
            {
                if (!_controller.ModelInitialized)
                    throw new CaeException("There is no model to import into. First create a new model.");
                // If the model space or the unit system are undefined
                IfNeededSelectAndSetNewModelProperties();
                //
                string filter = GetFileImportFilter(onlyMaterials);
                string[] files = GetFileNamesToImport(filter);
                //
                if (files != null && files.Length > 0)
                {
                    _controller.ClearErrors();
                    //
                    SetStateWorking(Globals.ImportingText);
                    foreach (var file in files)
                    {
                        await _controller.ImportFileAsync(file, onlyMaterials);
                    }
                    SetFrontBackView(true, true);   // animate must be true in order for the scale bar to work correctly
                    //
                    int numErrors = _controller.GetNumberOfErrors();
                    if (numErrors > 0)
                    {
                        _controller.OutputErrors();
                        string message = "There were errors while importing the file/files.";
                        WriteDataToOutput(message);
                        AutoClosingMessageBox.ShowError(message, 3000);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ImportingText);
            }
        }
        private async void SaveCADPartsAsStep(string[] partNames)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Step files | *.stp";
                    if (_controller.OpenedFileName != null)
                        saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_controller.OpenedFileName) + ".stp";
                    //
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // The filter adds the extension to the file name
                        SetStateWorking(Globals.ExportingText);
                        //
                        await Task.Run(() => _controller.ExportCADGeometryPartsAsStep(partNames, saveFileDialog.FileName));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        private async void SaveCADPartsAsBrep(string[] partNames)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Brep files | *.brep";
                    if (_controller.OpenedFileName != null)
                        saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_controller.OpenedFileName) + ".brep";
                    //
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // The filter adds the extension to the file name
                        SetStateWorking(Globals.ExportingText);
                        //
                        await Task.Run(() => _controller.ExportCADGeometryPartsAsBrep(partNames, saveFileDialog.FileName));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        private async void SavePartsAsStl(string[] partNames)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Stereolitography files | *.stl";
                    if (_controller.OpenedFileName != null)
                        saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_controller.OpenedFileName) + ".stl";
                    //
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // The filter adds the extension to the file name
                        SetStateWorking(Globals.ExportingText);
                        //
                        await Task.Run(() => _controller.ExportToStl(partNames, saveFileDialog.FileName));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        private async void SavePartsAsMmgMesh(string[] partNames)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Mmg files | *.mesh";
                    if (_controller.OpenedFileName != null)
                        saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_controller.OpenedFileName) + ".mesh";
                    //
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // The filter adds the extension to the file name
                        SetStateWorking(Globals.ExportingText);
                        //
                        await Task.Run(() => _controller.ExportGeometryPartsAsMmgMesh(partNames, saveFileDialog.FileName));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        private async void SaveDeformedPartsAsInp(string[] partNames)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Calculix files | *.inp";
                    if (_controller.OpenedFileName != null)
                        saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_controller.OpenedFileName) + ".inp";
                    //
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // The filter adds the extension to the file name
                        SetStateWorking(Globals.ExportingText);
                        //
                        await Task.Run(() => _controller.ExportDeformedPartsToCalculix(partNames, saveFileDialog.FileName));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        //
        private void tsmiCloseCurrentResult_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.AllResults.Count <= 1) tsmiCloseAllResults_Click(null, null);
                else
                {
                    _controller.RemoveCurrentResult();
                    SetResultNames();
                    if (tscbResultNames.SelectedItem != null) SetResult(tscbResultNames.SelectedItem.ToString());
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiCloseAllResults_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.ClearResults(); // calls this.ClearResults();
                //
                if (_controller.CurrentView == ViewGeometryModelResults.Results) Clear3D();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Recent
        public void UpdateRecentFilesThreadSafe(string[] fileNames)
        {
            InvokeIfRequired(UpdateRecentFiles, fileNames);
        }
        public void UpdateRecentFiles(string[] fileNames)
        {
            try
            {
                if (fileNames != null)
                {
                    tsmiOpenRecent.DropDownItems.Clear();
                    //
                    ToolStripMenuItem menuItem;
                    foreach (var fileName in fileNames)
                    {
                        menuItem = new ToolStripMenuItem(fileName.Replace("&", "&&"));
                        menuItem.Name = fileName;
                        menuItem.Click += tsmiRecentFile_Click;
                        tsmiOpenRecent.DropDownItems.Add(menuItem);
                    }
                    if (fileNames.Length > 0)
                    {
                        ToolStripSeparator separator = new ToolStripSeparator();
                        tsmiOpenRecent.DropDownItems.Add(separator);
                    }
                    menuItem = new ToolStripMenuItem("Clear Recent Files");
                    menuItem.Click += tsmiClearRecentFiles_Click;
                    tsmiOpenRecent.DropDownItems.Add(menuItem);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiRecentFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = ((ToolStripMenuItem)sender).Name;
                if (CheckBeforeOpen(fileName)) OpenAsync(fileName, _controller.Open);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiClearRecentFiles_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.ClearRecentFiles();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        #endregion  ################################################################################################################

        #region Edit menu ##########################################################################################################
        private async void tsmiUndo_Click(object sender, EventArgs e)
        {
            try
            {
                SetStateWorking(Globals.UndoingText);
                _modelTree.ScreenUpdating = false;
                //
                await Task.Run(() => _controller.UndoHistory());                
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.UndoingText);
                _modelTree.ScreenUpdating = true;
                _modelTree.RegenerateTree(_controller.Model, _controller.Jobs, _controller.CurrentResult);
                //
                SetMenuAndToolStripVisibility();
                //
                SetZoomToFit(true);
            }
        }
        private void tsmiRedo_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.RedoHistory();
                //
                SetMenuAndToolStripVisibility();
                //
                SetZoomToFit(true);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiViewHistory_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(_controller.GetHistoryFileNameTxt());
        }
        private async void tsmiRegenerate_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllForms();
                Clear3D();
                Application.DoEvents();
                SetStateWorking(Globals.RegeneratingText);
                _modelTree.ScreenUpdating = false;
                await Task.Run(() => _controller.RegenerateHistoryCommands());
                //
                SetMenuAndToolStripVisibility();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.RegeneratingText);
                _modelTree.ScreenUpdating = true;
                _modelTree.RegenerateTree(_controller.Model, _controller.Jobs, _controller.CurrentResult);
                //
                SetMenuAndToolStripVisibility();
                //
                SetZoomToFit(true);
            }
        }
        private async void tsmiRegenerteUsingOtherFiles_Click(object sender, EventArgs e)
        {
            try
            {
                CloseAllForms();
                Clear3D();
                Application.DoEvents();
                SetStateWorking(Globals.RegeneratingText);
                _modelTree.ScreenUpdating = false;
                await Task.Run(() => _controller.RegenerateHistoryCommandsWithDialogs(true));
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.RegeneratingText);
                _modelTree.ScreenUpdating = true;
                _modelTree.RegenerateTree(_controller.Model, _controller.Jobs, _controller.CurrentResult);
                //
                SetMenuAndToolStripVisibility();
                //
                SetZoomToFit(true);
            }
        }
        public void EnableDisableUndoRedo(string undo, string redo)
        {
            InvokeIfRequired(EnableDisable, undo, redo);
        }
        private void EnableDisable(string undo, string redo)
        {
            if (undo == null)
            {
                tsmiUndo.Text = "Undo";
                tsmiUndo.Enabled = false;
            }
            else
            {
                tsmiUndo.Text = "Undo - " + undo;
                tsmiUndo.Enabled = true;
            }

            if (redo == null)
            {
                tsmiRedo.Text = "Redo";
                tsmiRedo.Enabled = false;
            }
            else
            {
                tsmiRedo.Text = "Redo - " + redo;
                tsmiRedo.Enabled = true;
            }
        }

        #endregion  ################################################################################################################

        #region View menu ##########################################################################################################
        private void tsmiFrontView_Click(object sender, EventArgs e)
        {
            _vtk.SetFrontBackView(true, true);
        }
        private void tsmiBackView_Click(object sender, EventArgs e)
        {
            _vtk.SetFrontBackView(true, false);
        }
        private void tsmiTopView_Click(object sender, EventArgs e)
        {
            _vtk.SetTopBottomView(true, true);
        }
        private void tsmiBottomView_Click(object sender, EventArgs e)
        {
            _vtk.SetTopBottomView(true, false);
        }
        private void tsmiLeftView_Click(object sender, EventArgs e)
        {
            _vtk.SetLeftRightView(true, true);
        }
        private void tsmiRightView_Click(object sender, EventArgs e)
        {
            _vtk.SetLeftRightView(true, false);
        }
        //
        private void tsmiNormalView_Click(object sender, EventArgs e)
        {
            _vtk.SetNormalView(true);
        }
        private void tsmiVerticalView_Click(object sender, EventArgs e)
        {
            _vtk.SetVerticalView(true, true);
        }
        //
        private void tsmiIsometricView_Click(object sender, EventArgs e)
        {
            _vtk.SetIsometricView(true, true);
        }
        private void tsmiZoomToFit_Click(object sender, EventArgs e)
        {
            SetZoomToFit(true);
        }
        //
        private void tsmiShowWireframeEdges_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.CurrentEdgesVisibility = vtkControl.vtkEdgesVisibility.Wireframe;
            }
            catch { }
        }
        private void tsmiShowElementEdges_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.CurrentEdgesVisibility = vtkControl.vtkEdgesVisibility.ElementEdges;
            }
            catch { }
        }
        private void tsmiShowModelEdges_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.CurrentEdgesVisibility = vtkControl.vtkEdgesVisibility.ModelEdges;
            }
            catch { }
        }
        private void tsmiShowNoEdges_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.CurrentEdgesVisibility = vtkControl.vtkEdgesVisibility.NoEdges;
            }
            catch { }
        }
        //
        private void tsmiSectionView_Click(object sender, EventArgs e)
        {
            try
            {
                SinglePointDataEditor.ParentForm = _frmSectionView;
                SinglePointDataEditor.Controller = _controller;
                //
                ShowForm(_frmSectionView, tsmiSectionView.Text, null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiExplodedView_Click(object sender, EventArgs e)
        {
            try
            {
                ExplodedViewParameters parameters = _controller.GetCurrentExplodedViewParameters();
                _frmExplodedView.SetExplodedViewParameters(parameters);
                //
                ShowForm(_frmExplodedView, _frmExplodedView.Text, null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void TurnExplodedViewOnOff(bool animate)
        {
            try
            {
                SetStateWorking(Globals.ExplodePartsText);
                _controller.TurnExplodedViewOnOff(animate);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExplodePartsText);
            }
        }
        
        // Hide/Show
        private void tsmiHideAllParts_Click(object sender, EventArgs e)
        {
            try
            {
                string[] partNames;
                if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
                {
                    partNames = _controller.GetGeometryPartNames();
                    _controller.HideGeometryPartsCommand(partNames);
                }
                else if (_controller.CurrentView == ViewGeometryModelResults.Model)
                {
                    partNames = _controller.GetModelPartNames();
                    _controller.HideModelPartsCommand(partNames);
                }
                else if (_controller.CurrentView == ViewGeometryModelResults.Results)
                {
                    partNames = _controller.GetResultPartNames();
                    _controller.HideResultParts(partNames);
                }
            }
            catch { }
        }
        private void tsmiShowAllParts_Click(object sender, EventArgs e)
        {
            try
            {
                string[] partNames;
                if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
                {
                    partNames = _controller.GetGeometryPartNames();
                    _controller.ShowGeometryPartsCommand(partNames);
                }
                else if (_controller.CurrentView == ViewGeometryModelResults.Model)
                {
                    partNames = _controller.GetModelPartNames();
                    _controller.ShowModelPartsCommand(partNames);
                }
                else if (_controller.CurrentView == ViewGeometryModelResults.Results)
                {
                    partNames = _controller.GetResultPartNames();
                    _controller.ShowResultParts(partNames);
                }
            }
            catch { }
        }
        private void tsmiInvertVisibleParts_Click(object sender, EventArgs e)
        {
            try
            {
                BasePart[] parts;
                List<string> partNamesToHide = new List<string>();
                List<string> partNamesToShow = new List<string>();

                if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
                {
                    parts = _controller.GetGeometryParts();
                    foreach (var part in parts)
                    {
                        if (part.Visible) partNamesToHide.Add(part.Name);
                        else partNamesToShow.Add(part.Name);
                    }
                    if (partNamesToHide.Count > 0) _controller.HideGeometryPartsCommand(partNamesToHide.ToArray());
                    if (partNamesToShow.Count > 0) _controller.ShowGeometryPartsCommand(partNamesToShow.ToArray());
                }
                else if (_controller.CurrentView == ViewGeometryModelResults.Model)
                {
                    parts = _controller.GetModelParts();
                    foreach (var part in parts)
                    {
                        if (part.Visible) partNamesToHide.Add(part.Name);
                        else partNamesToShow.Add(part.Name);
                    }
                    if (partNamesToHide.Count > 0) _controller.HideModelPartsCommand(partNamesToHide.ToArray());
                    if (partNamesToShow.Count > 0) _controller.ShowModelPartsCommand(partNamesToShow.ToArray());
                }
                else if (_controller.CurrentView == ViewGeometryModelResults.Results)
                {
                    parts = _controller.GetResultParts();
                    foreach (var part in parts)
                    {
                        if (part.Visible) partNamesToHide.Add(part.Name);
                        else partNamesToShow.Add(part.Name);
                    }
                    if (partNamesToHide.Count > 0) _controller.HideResultParts(partNamesToHide.ToArray());
                    if (partNamesToShow.Count > 0) _controller.ShowResultParts(partNamesToShow.ToArray());
                }
            }
            catch { }
        }
        // Annotate
        private void DropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                e.Cancel = true;
        }
        private void tsmiAnnotateFaceOrientations_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateParts_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateMaterials_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateSections_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateSectionThicknesses_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateAllSymbols_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem tsmi)
            {
                // Turn off
                if (tsmi.Checked)
                {
                    ClearAnnotationStatus();  // first check and then clear
                    _controller.AnnotateWithColor = AnnotateWithColorEnum.None;
                }
                // Turn on
                else
                {
                    ClearAnnotationStatus();
                    //
                    tsmi.Checked = true;
                    tsmiAnnotateReferencePoints.Checked = true;
                    tsmiAnnotateConstraints.Checked = true;
                    tsmiAnnotateContactPairs.Checked = true;
                    tsmiAnnotateBCs.Checked = true;
                    tsmiAnnotateLoads.Checked = true;
                    //
                    _controller.AnnotateWithColor = AnnotateWithColorEnum.ReferencePoints |
                                                    AnnotateWithColorEnum.Constraints |
                                                    AnnotateWithColorEnum.ContactPairs |
                                                    AnnotateWithColorEnum.BoundaryConditions |
                                                    AnnotateWithColorEnum.Loads;
                }
            }
        }
        private void tsmiAnnotateReferencePoints_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateConstraints_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateContactPairs_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateBCs_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        private void tsmiAnnotateLoads_Click(object sender, EventArgs e)
        {
            _controller.AnnotateWithColor = ChangeAnnotationStatus(sender);
        }
        //
        private AnnotateWithColorEnum ChangeAnnotationStatus(object sender)
        {
            if (sender is ToolStripMenuItem tsmi)
            {
                // Turn off
                if (tsmi.Checked) tsmi.Checked = false;
                // Turn on
                else
                {
                    // Only one possibility - Face orientations or parts
                    if (tsmi == tsmiAnnotateFaceOrientations || tsmi == tsmiAnnotateParts ||
                        tsmi == tsmiAnnotateMaterials || tsmi == tsmiAnnotateSections ||  
                        tsmi == tsmiAnnotateSectionThicknesses)
                    {
                        ClearAnnotationStatus();
                        tsmi.Checked = true;
                    }
                    // Symbols
                    else
                    {
                        tsmiAnnotateFaceOrientations.Checked = false;
                        tsmiAnnotateParts.Checked = false;
                        tsmiAnnotateMaterials.Checked = false;
                        tsmiAnnotateSections.Checked = false;
                        tsmiAnnotateSectionThicknesses.Checked = false;
                        //
                        tsmi.Checked = true;
                    }
                }
                //
                if (tsmiAnnotateFaceOrientations.Checked) return AnnotateWithColorEnum.FaceOrientation;
                else if (tsmiAnnotateParts.Checked) return AnnotateWithColorEnum.Parts;
                else if (tsmiAnnotateMaterials.Checked) return AnnotateWithColorEnum.Materials;
                else if (tsmiAnnotateSections.Checked) return AnnotateWithColorEnum.Sections;
                else if (tsmiAnnotateSectionThicknesses.Checked) return AnnotateWithColorEnum.SectionThicknesses;
                else
                {
                    AnnotateWithColorEnum status = AnnotateWithColorEnum.None;
                    if (tsmiAnnotateReferencePoints.Checked) status |= AnnotateWithColorEnum.ReferencePoints;
                    if (tsmiAnnotateConstraints.Checked) status |= AnnotateWithColorEnum.Constraints;
                    if (tsmiAnnotateContactPairs.Checked) status |= AnnotateWithColorEnum.ContactPairs;
                    if (tsmiAnnotateBCs.Checked) status |= AnnotateWithColorEnum.BoundaryConditions;
                    if (tsmiAnnotateLoads.Checked) status |= AnnotateWithColorEnum.Loads;
                    //
                    tsmiAnnotateAllSymbols.Checked = status.HasFlag(AnnotateWithColorEnum.ReferencePoints |
                                                                    AnnotateWithColorEnum.Constraints |
                                                                    AnnotateWithColorEnum.ContactPairs |
                                                                    AnnotateWithColorEnum.BoundaryConditions |
                                                                    AnnotateWithColorEnum.Loads);
                    //
                    return status;
                }
            }
            return AnnotateWithColorEnum.None;
        }
        //
        private void tsmiResultsUndeformed_Click(object sender, EventArgs e)
        {
            SetUndeformedModelType(ViewResultsTypeEnum.Undeformed, UndeformedModelTypeEnum.None);
        }
        private void tsmiResultsDeformed_Click(object sender, EventArgs e)
        {
            SetUndeformedModelType(ViewResultsTypeEnum.Deformed, UndeformedModelTypeEnum.None);
        }
        private void tsmiResultsColorContours_Click(object sender, EventArgs e)
        {
            SetUndeformedModelType(ViewResultsTypeEnum.ColorContours, UndeformedModelTypeEnum.None);
        }
        private void tsmiResultsDeformedColorWireframe_Click(object sender, EventArgs e)
        {
            SetUndeformedModelType(ViewResultsTypeEnum.ColorContours, UndeformedModelTypeEnum.WireframeBody);
        }

        private void tsmiResultsDeformedColorSolid_Click(object sender, EventArgs e)
        {
            SetUndeformedModelType(ViewResultsTypeEnum.ColorContours, UndeformedModelTypeEnum.SolidBody);
        }
        private void SetUndeformedModelType(ViewResultsTypeEnum viewResultsType, UndeformedModelTypeEnum undeformedModelType)
        {
            _controller.SetUndeformedModelType(undeformedModelType);
            //
            if (GetCurrentView() == ViewGeometryModelResults.Results)
            {
                if (_frmAnimation.Visible) _frmAnimation.Hide();
                _controller.ViewResultsType = viewResultsType;
            }
            //
            UpdateResultsTypeIconStates();
        }
        private void UpdateResultsTypeIconStates()
        {
            tsbResultsUndeformed.Checked = false;
            tsbResultsDeformed.Checked = false;
            tsbResultsColorContours.Checked = false;
            tsbResultsUndeformedWireframe.Checked = false;
            tsbResultsUndeformedSolid.Checked = false;
            //
            if (GetCurrentView() == ViewGeometryModelResults.Results)
            {
                if (_controller.ViewResultsType == ViewResultsTypeEnum.Undeformed) tsbResultsUndeformed.Checked = true;
                else if (_controller.ViewResultsType == ViewResultsTypeEnum.Deformed) tsbResultsDeformed.Checked = true;
                else if (_controller.ViewResultsType == ViewResultsTypeEnum.ColorContours)
                {
                    if (_controller.GetUndeformedModelType() == UndeformedModelTypeEnum.None)
                        tsbResultsColorContours.Checked = true;
                    else if (_controller.GetUndeformedModelType() == UndeformedModelTypeEnum.WireframeBody)
                        tsbResultsUndeformedWireframe.Checked = true;
                    else if (_controller.GetUndeformedModelType() == UndeformedModelTypeEnum.SolidBody)
                        tsbResultsUndeformedSolid.Checked = true;
                }
            }
        }
        #endregion

        #region Geometry ###########################################################################################################
        // Part
        internal void tsmiEditGeometryPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Parts", _controller.GetGeometryParts(), EditGeometryPart);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        // Sub menu Transform
        private void tsmiScaleGeometryParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), ScaleGeometryParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        // End Transform
        private void tsmiCopyGeometryPartsToResults_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), CopyGeometryPartsToResults);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideGeometryParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), HideGeometryParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowGeometryParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), ShowGeometryParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowOnlyGeometryParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), ShowOnlyGeometryParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSetTransparencyForGeometryParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), SetTransparencyForGeometryParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteGeometryParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryPartsWithoutSubParts(), DeleteGeometryParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        // End Part
        // CAD Part
        private async void tsmiFlipFaceNormalCAD_Click(object sender, EventArgs e)
        {
            try
            {
                // Must be outside the await part otherwise couses screen flickering
                AnnotateWithColorEnum _prevShowWithColor = _controller.AnnotateWithColor;
                _controller.AnnotateWithColor = AnnotateWithColorEnum.FaceOrientation;
                //
                await Task.Run(() =>
                {
                    _frmSelectGeometry.HideFormOnOK = false;
                    _frmSelectGeometry.SelectionFilter = SelectGeometryEnum.Surface;
                    _frmSelectGeometry.OnOKCallback = FlipFaces;
                    //
                    InvokeIfRequired(() => { ShowForm(_frmSelectGeometry, "Select faces to flip", null); });
                    while (_frmSelectGeometry.Visible) System.Threading.Thread.Sleep(100);
                });
                //
                _controller.AnnotateWithColor = _prevShowWithColor;
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.FlippingNormalsText);
            }
        }
        private async void tsmiSplitAFaceUsingTwoPoints_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    GeometrySelection surfaceSelection;
                    GeometrySelection verticesSelection;
                    while (true)
                    {
                        // Get a surface to split
                        _frmSelectGeometry.MaxNumberOfSelectedItems = 1;
                        _frmSelectGeometry.SelectionFilter = SelectGeometryEnum.Surface;
                        //
                        InvokeIfRequired(() => { ShowForm(_frmSelectGeometry, "Select a face to split", null); });
                        while (_frmSelectGeometry.Visible) System.Threading.Thread.Sleep(100);
                        //
                        if (_frmSelectGeometry.DialogResult == DialogResult.OK)
                        {
                            surfaceSelection = _frmSelectGeometry.GeometrySelection.DeepClone();
                            // Get two vertices to split the surface
                            _frmSelectGeometry.MaxNumberOfSelectedItems = 2;
                            _frmSelectGeometry.SelectionFilter = SelectGeometryEnum.Vertex;
                            //
                            InvokeIfRequired(() => { ShowForm(_frmSelectGeometry, "Select splitting vertices", null); });
                            while (_frmSelectGeometry.Visible) System.Threading.Thread.Sleep(100);
                            //
                            if (_frmSelectGeometry.DialogResult == DialogResult.OK)
                            {
                                verticesSelection = _frmSelectGeometry.GeometrySelection.DeepClone();
                                //
                                SetStateWorking(Globals.SplittingFacesText);
                                _controller.SplitAFaceUsingTwoPointsCommand(surfaceSelection, verticesSelection);
                                SetStateReady(Globals.SplittingFacesText);
                            }
                            else break;
                        }
                        else break;
                    }
                });
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.SplittingFacesText);
            }
        }
        // End CAD Part
        // Stl Part
        private void tsmiFindEdgesByAngleForGeometryParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetNonCADGeometryParts(), FindEdgesByAngleForGeometryParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiFlipStlPartSurfacesNormal_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetNonCADGeometryParts(), FlipStlPartSurfacesNormal);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSmoothPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Parts", _controller.GetNonCADGeometryParts(), _controller.SmoothGeometryPart);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiCropWithCylinder_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Parts", _controller.GetGeometryParts(), _controller.CropGeometryPartWithCylinder);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiCropWithCube_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Parts", _controller.GetGeometryParts(), _controller.CropGeometryPartWithCube);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        // End Stl Part
        private void tsmiCreateAndImportCompoundPart_Click(object sender, EventArgs e)
        {
            try
            {
                Clear3DSelection();
                SelectMultipleEntities("Parts", _controller.GetCADGeometryParts(), CreateAndImportCompoundPart, 2);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiRegenerateCompoundPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetCompoundParts(), RegenerateCompoundParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSwapGeometryPartGeometries_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), SwapPartGeometries, 2, 2);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        // Analyze geometry
        private void tsmiGeometryAnalyze_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryParts(), AnalyzeGeometry);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }

        }
        //                                                                                                                          
        // Part
        private void EditGeometryPart(string partName)
        {
            _frmPartProperties.View = ViewGeometryModelResults.Geometry;
            ShowForm(_frmPartProperties, "Edit Part", partName);
        }
        // Sub menu Transform
        private void ScaleGeometryParts(string[] partNames)
        {
            SinglePointDataEditor.ParentForm = _frmScale;
            SinglePointDataEditor.Controller = _controller;
            // Set all part names for scaling
            _frmScale.PartNames = partNames;
            //
            ShowForm(_frmScale, "Scale parts: " + partNames.ToShortString(), null);
        }
        public async Task ScaleGeometryPartsAsync(string[] partNames, double[] scaleCenter, double[] scaleFactors, bool copy)
        {
            bool stateSet = false;
            try
            {
                if (SetStateWorking(Globals.TransformingText))
                {
                    stateSet = true;
                    //
                    if (partNames != null && partNames.Length > 0)
                    {
                        await Task.Run(() => _controller.ScaleGeometryPartsCommand(partNames, scaleCenter, scaleFactors, copy));
                    }
                }
                else MessageBoxes.ShowWarning("Another task is already running.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                if (stateSet) SetStateReady(Globals.TransformingText);
            }
        }
        // End Transform
        private void CopyGeometryPartsToResults(string[] partNames)
        {
            CloseAllForms();
            _controller.CopyGeometryPartsToResults(partNames);
        }
        private void HideGeometryParts(string[] partNames)
        {
            _controller.HideGeometryPartsCommand(partNames);            
        }
        private void ShowGeometryParts(string[] partNames)
        {
            _controller.ShowGeometryPartsCommand(partNames);
        }
        private void ShowOnlyGeometryParts(string[] partNames)
        {
            // If sub part is selected add the whole compound part
            HashSet<string> partsToShow = new HashSet<string>(partNames);
            foreach (var entry in _controller.Model.Geometry.Parts)
            {
                if (entry.Value is CompoundGeometryPart cgp)
                {
                    if (partNames.Contains(cgp.Name))
                    {
                        partsToShow.Add(cgp.Name);
                        partsToShow.UnionWith(cgp.SubPartNames);
                    }
                }
            }
            //
            HashSet<string> allNames = new HashSet<string>(_controller.Model.Geometry.Parts.Keys);
            allNames.ExceptWith(partsToShow);
            _controller.HideGeometryPartsCommand(allNames.ToArray());
            _controller.ShowGeometryPartsCommand(partsToShow.ToArray());
        }
        private void SetTransparencyForGeometryParts(string[] partNames)
        {
            if (_controller.Model.Geometry == null) return;
            //
            using (FrmGetValue frmGetValue = new FrmGetValue())
            {
                frmGetValue.NumOfDigits = 0;
                frmGetValue.MinValue = 25;
                frmGetValue.MaxValue = 255;
                SetFormLoaction(frmGetValue);
                OrderedDictionary<string, double> presetValues =
                    new OrderedDictionary<string, double>("Preset Transparency values", StringComparer.OrdinalIgnoreCase);
                presetValues.Add("Semi-transparent", 128);
                presetValues.Add("Opaque", 255);
                string desc = "Enter the transparency between 0 and 255.\n" + "(0 - transparent; 255 - opaque)";
                frmGetValue.PrepareForm("Set Transparency: " + partNames.ToShortString(), "Transparency", desc, 128, presetValues);
                if (frmGetValue.ShowDialog() == DialogResult.OK)
                {
                    _controller.SetTransparencyForGeometryPartsCommand(partNames, (byte)frmGetValue.Value);
                }
                SaveFormLoaction(frmGetValue);
            }
        }
        private void DeleteGeometryParts(string[] partNames)
        {
            GeometryPart[] parts = _controller.GetGeometryPartsWithoutSubParts();
            HashSet<string> deletablePartNames = new HashSet<string>();
            foreach (GeometryPart part in parts) deletablePartNames.Add(part.Name);
            deletablePartNames.IntersectWith(partNames);
            if (deletablePartNames.Count > 0)
            {
                partNames = deletablePartNames.ToArray();
                if (MessageBoxes.ShowWarningQuestion("OK to delete selected parts?") == DialogResult.OK)
                {
                    _controller.RemoveGeometryPartsCommand(partNames.ToArray());
                }
            }
            else MessageBoxes.ShowError("Selected parts belong to a compound part and cannot be deleted:" + Environment.NewLine +
                                        partNames.ToRows());
        }
        // End Part
        // CAD Part
        private void FlipFaces(GeometrySelection geometrySelection)
        {
            SetStateWorking(Globals.FlippingNormalsText);
            _controller.FlipFaceOrientationsCommand(geometrySelection);
            SetStateReady(Globals.FlippingNormalsText);
        }
        // End CAD Part
        // Stl Part
        private void FlipStlPartSurfacesNormal(string[] partNames)
        {
            _controller.FlipStlPartSurfacesNormalCommand(partNames);
        }
        private void FindEdgesByAngleForGeometryParts(string[] partNames)
        {
            using (FrmGetValue frmGetValue = new FrmGetValue())
            {
                SetUpFrmGetValueForEdgeAngle(frmGetValue, partNames);
                //
                if (frmGetValue.ShowDialog() == DialogResult.OK)
                {
                    _controller.FindEdgesByAngleForGeometryPartsCommand(partNames, frmGetValue.Value);
                }
                SaveFormLoaction(frmGetValue);
            }
        }
        private void SetUpFrmGetValueForEdgeAngle(FrmGetValue frmGetValue, string[] partNames)
        {
            frmGetValue.NumOfDigits = 2;
            frmGetValue.MinValue = 0;
            frmGetValue.MaxValue = 90;
            SetFormLoaction(frmGetValue);
            OrderedDictionary<string, double> presetValues =
                new OrderedDictionary<string, double>("Preset Transparency Values", StringComparer.OrdinalIgnoreCase);
            presetValues.Add("Default", CaeMesh.Globals.EdgeAngle);
            string desc = "Enter the face angle for model edges detection.";
            frmGetValue.PrepareForm("Find model edges: " + partNames.ToShortString(), "Angle", desc,
                                    CaeMesh.Globals.EdgeAngle, presetValues, new StringAngleDegConverter());
        }
        // End Stl Part
        private async void CreateAndImportCompoundPart(string[] partNames)
        {
            try
            {
                SetStateWorking(Globals.CreatingCompoundText, true);
                //
                GeometryPart part;
                HashSet<PartType> stlPartTypes = new HashSet<PartType>();
                HashSet<PartType> cadPartTypes = new HashSet<PartType>();
                //
                string[] allPartNames = _controller.GetMeshablePartNames(partNames);
                foreach (var partName in allPartNames)
                {
                    part = (GeometryPart)_controller.Model.Geometry.Parts[partName];
                    if (part.CADFileData == null) stlPartTypes.Add(part.PartType);
                    else cadPartTypes.Add(part.PartType);
                }
                if (stlPartTypes.Count + cadPartTypes.Count != 1)
                    throw new CaeException("Compound part can be made from only CAD or only stl based geometry parts " + 
                                           "of the same type.");
                await Task.Run(() => _controller.CreateAndImportCompoundPartCommand(partNames));
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.CreatingCompoundText);
            }
        }
        private async void RegenerateCompoundParts(string[] compoundPartNames)
        {
            try
            {
                SetStateWorking(Globals.RegeneratingCompoundText, true);
                //
                string missingPartName = null;
                string errorCompoundPartName = null;
                CompoundGeometryPart part;
                foreach (var compoundPartName in compoundPartNames)
                {
                    part = (CompoundGeometryPart)_controller.Model.Geometry.Parts[compoundPartName];
                    if (part.CreatedFromPartNames != null && part.CreatedFromPartNames.Length > 1)
                    {
                        foreach (var createdFromPartName in part.CreatedFromPartNames)
                        {
                            if (!_controller.Model.Geometry.Parts.ContainsKey(createdFromPartName))
                            {
                                missingPartName = createdFromPartName;
                                errorCompoundPartName = compoundPartName;
                                break;
                            }
                        }
                        //
                        if (missingPartName != null) break;
                    }
                }
                if (missingPartName != null)
                    throw new CaeException("The part '" + missingPartName + "' that was used to create a compound part '" +
                                           errorCompoundPartName + "' is missing.");
                //
                await Task.Run(() => _controller.RegenerateCompoundPartsCommand(compoundPartNames));
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.RegeneratingCompoundText);
            }
        }
        private void SwapPartGeometries(string[] partNames)
        {
            GeometryPart[] parts = _controller.GetGeometryPartsWithoutSubParts();
            GeometryPart part1 = _controller.GetGeometryPart(partNames[0]);
            GeometryPart part2 = _controller.GetGeometryPart(partNames[1]);
            if (parts.Contains(part1) && parts.Contains(part2))
            {
                if (part1 is CompoundGeometryPart || part2 is CompoundGeometryPart)
                    MessageBoxes.ShowError("Compound parts cannot be swaped.");
                else
                    _controller.SwapPartGeometriesCommand(partNames[0], partNames[1]);
            }
            else MessageBoxes.ShowError("Compound subparts cannot be swaped.");
        }
        // Analyze geometry
        private void AnalyzeGeometry(string[] partNames)
        {
            if (!_frmAnalyzeGeometry.Visible)
            {
                CloseAllForms();
                SetFormLoaction((Form)_frmAnalyzeGeometry);
                _frmAnalyzeGeometry.PartNamesToAnalyze = partNames;
                _frmAnalyzeGeometry.Show();
            }
        }

        #endregion  ################################################################################################################

        #region Mesh ###############################################################################################################
        private void tsmiCreateMeshingParameters_Click(object sender, EventArgs e)
        {
            try
            {
                CreateMeshingParameters();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditMeshingParameters_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Meshing Parameters", _controller.GetMeshingParameters(), EditMeshingParameters);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteMeshingParameters_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Meshing Parameters", _controller.GetMeshingParameters(), DeleteMeshingParameters);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void tsmiCreateMeshRefinement_Click(object sender, EventArgs e)
        {
            try
            {
                CreateMeshRefinement();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditMeshRefinement_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Mesh Refinements", _controller.GetMeshRefinements(), EditMeshRefinement);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteMeshRefinement_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Mesh Refinements", _controller.GetMeshRefinements(), DeleteMeshRefinements);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void tsmiPreviewEdgeMesh_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryPartsWithoutSubParts(), PreviewEdgeMeshes);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        internal void tsmiCreateMesh_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryPartsWithoutSubParts(), CreatePartMeshes);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }            
        }
        //
        private async void PreviewEdgeMeshes(string[] partNames)
        {
            await PreviewEdgeMeshesAsync(partNames, null, null);
        }
        private async Task PreviewEdgeMeshesAsync(string[] partNames, MeshingParameters meshingParameters,
                                                  FeMeshRefinement meshRefinement)
        {
            bool stateSet = false;
            try
            {
                if (SetStateWorking(Globals.PreviewText))
                {
                    stateSet = true;                    
                    //
                    await Task.Run(() =>
                    {
                        foreach (var partName in partNames)
                        {
                            _controller.PreviewEdgeMesh(partName, meshingParameters, meshRefinement);
                        }
                    });
                }
                else MessageBoxes.ShowWarning("Another task is already running.");
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                if (stateSet) SetStateReady(Globals.PreviewText);
            }
        }
        // Meshing parameters
        private void CreateMeshingParameters()
        {
            if (_controller.Model.Geometry == null) return;
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmMeshingParameters;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmMeshingParameters, "Create Meshing Parameters", null);
        }
        private void EditMeshingParameters(string meshingParametersName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmMeshingParameters;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmMeshingParameters, "Edit Meshing Parameters", meshingParametersName);
        }
        private void DeleteMeshingParameters(string[] meshingParametersNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected meshing parameters?" + Environment.NewLine
                                                 + meshingParametersNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveMeshingParametersCommand(meshingParametersNames);
            }
        }
        // Mesh refinement
        private void CreateMeshRefinement()
        {
            if (_controller.Model.Geometry == null) return;
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmMeshRefinement;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmMeshRefinement, "Create Mesh Refinement", null);
        }
        private void EditMeshRefinement(string meshRefinementName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmMeshRefinement;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmMeshRefinement, "Edit Mesh Refinement", meshRefinementName);
        }
        private void DeleteMeshRefinements(string[] meshRefinementNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected mesh refinements?" + Environment.NewLine
                                                 + meshRefinementNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveMeshRefinementsCommand(meshRefinementNames);
            }
        }
        // Create mesh
        private async void CreatePartMeshes(string[] partNames)
        {
            try
            {
                List<string> errors = new List<string>();
                SetStateWorking(Globals.MeshingText, true);
                MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
                Keys modifierKeys = Keys.Control;
                _modelTree.ClearTreeSelection(ViewType.Model);                
                //
                CloseAllForms();
                //
                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                //
                GeometryPart part;
                foreach (var partName in partNames)
                {
                    try
                    {
                        part = _controller.GetGeometryPart(partName);
                        //
                        await Task.Run(() => _controller.CreateMeshCommand(partName));
                        // Check for the cancel button click
                        if (IsStateWorking())
                        {
                            _modelTree.SelectBasePart(e, modifierKeys, part, true);
                        }
                        else
                        {
                            errors.Add("Mesh generation canceled.");
                            break;
                        }
                    }
                    catch
                    {
                        errors.Add("Mesh generation failed for part " + partName +
                                   ". Check the geometry and/or adjust the meshing parameters.");
                    }
                }
                watch.Stop();
                if (partNames.Length > 1)
                {
                    WriteDataToOutput("");
                    WriteDataToOutput("Elapsed time [s]: " + watch.Elapsed.TotalSeconds.ToString());
                }
                //
                _controller.UpdateExplodedView(true);
                //
                if (errors.Count > 0)
                {
                    WriteDataToOutput("");
                    foreach (var error in errors) WriteDataToOutput(error);
                    MessageBoxes.ShowError("Errors occurred during meshing. Please check the output window.");
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.MeshingText);
            }
        }
        // Advisor
        internal void CreateDefaultMesh(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryPartsWithoutSubParts(), CreateDefaultMeshes);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        internal void CreateUserDefinedMesh(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetGeometryPartsWithoutSubParts(), CreateUserDefinedMeshes);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void CreateDefaultMeshes(string[] partNames)
        {
            CreatePartMeshes(partNames);
        }
        private async void CreateUserDefinedMeshes(string[] partNames)
        {
            await Task.Run(() =>
            {
                InvokeIfRequired(() => { CreateMeshingParameters(); });
                //
                bool firstTime = true;
                do
                {
                    System.Threading.Thread.Sleep(250);
                    if (firstTime)
                    {
                        Selection selection = new Selection();
                        selection.Add(new SelectionNodeIds(vtkSelectOperation.None, false,
                                                           _controller.Model.Geometry.GetPartIdsByNames(partNames)));
                        selection.SelectItem = vtkSelectItem.Part;
                        selection.CurrentView = (int)ViewGeometryModelResults.Geometry;
                        _controller.Selection = selection;
                        _controller.SetSelectByToOff();
                        //
                        InvokeIfRequired(() => {
                            //_controller.HighlightSelection();
                            SelectionChanged(_controller.Model.Geometry.GetPartIdsByNames(partNames));
                        });
                        //
                        firstTime = false;
                    }
                }
                while (_frmMeshingParameters.Visible);

            });
            if (_frmMeshingParameters.DialogResult == DialogResult.OK) CreatePartMeshes(partNames);
        }

        #endregion  ################################################################################################################

        #region Model edit  ########################################################################################################
        private void tsmiEditModel_Click(object sender, EventArgs e)
        {
            try
            {
                EditModel();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditCalculiXKeywords_Click(object sender, EventArgs e)
        {
            try
            {
                EditCalculiXKeywords();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        // Tools
        private void tsmiCreateBoundaryLayer_Click(object sender, EventArgs e)
        {
            try
            {
                CreateBoundaryLayer();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiFindEdgesByAngleForModelParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetNonCADModelParts(), FindEdgesByAngleForModelParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiRemeshElements_Click(object sender, EventArgs e)
        {
            try
            {
                RemeshElements();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private async void tsmiUpdateNodalCoordinatesFromFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_controller.ModelInitialized || _controller.Model.Mesh == null) return;
                //
                string fileName = GetFileNameToImport("Abaqus/Calculix inp files|*.inp");
                //
                SetStateWorking(Globals.ImportingText);
                await Task.Run(() => _controller.UpdateNodalCoordinatesFromFileCommand(fileName));
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ImportingText);
            }
        }
        //                                                                                                                          
        private void EditModel()
        {
            if (_controller.Model.UnitSystem.UnitSystemType != UnitSystemType.Undefined)
                ShowForm(_frmModelProperties, "Edit Model", null);

        }
        private void EditCalculiXKeywords()
        {
            // This is also called from the model tree - needs try, catch
            try
            {
                if (CheckValiditiy())
                {
                    _frmCalculixKeywordEditor = new FrmCalculixKeywordEditor();
                    _frmCalculixKeywordEditor.Keywords = _controller.GetCalculixModelKeywords();
                    _frmCalculixKeywordEditor.UserKeywords = _controller.GetCalculixUserKeywords();
                    //
                    if (_frmCalculixKeywordEditor.Keywords != null)
                    {
                        _frmCalculixKeywordEditor.PrepareForm(); // must be here to check for errors
                        if (_frmCalculixKeywordEditor.ShowDialog() == DialogResult.OK)
                        {
                            _controller.SetCalculixUserKeywordsCommand(_frmCalculixKeywordEditor.UserKeywords);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        // Tools
        private void CreateBoundaryLayer()
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmBoundaryLayer;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmBoundaryLayer, "Create Boundary Layer", null);
        }
        private void FindEdgesByAngleForModelParts(string[] partNames)
        {
            using (FrmGetValue frmGetValue = new FrmGetValue())
            {
                SetUpFrmGetValueForEdgeAngle(frmGetValue, partNames);
                //
                if (frmGetValue.ShowDialog() == DialogResult.OK)
                {
                    _controller.FindEdgesByAngleForModelPartsCommand(partNames, frmGetValue.Value);
                }
                SaveFormLoaction(frmGetValue);
            }
        }
        private void RemeshElements()
        {
            if (_controller.Model == null || _controller.Model.Mesh == null) return;
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmRemeshingParameters;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmRemeshingParameters, "Remeshing Parameters", null);
        }
        private void UpdateNodalCoordinatesFromFile()
        {

        }

        #endregion  ################################################################################################################

        #region Node menu  #########################################################################################################
        private void tsmiRenumberAllNodes_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                //
                using (FrmGetValue frmGetValue = new FrmGetValue())
                {
                    frmGetValue.NumOfDigits = 0;
                    frmGetValue.MinValue = 1;
                    SetFormLoaction(frmGetValue);
                    string desc = "Enter the starting node id for the node renumbering.";
                    frmGetValue.PrepareForm("Renumber Nodes", "Start node id", desc, 1, null);
                    if (frmGetValue.ShowDialog() == DialogResult.OK)
                    {
                        _controller.RenumberNodesCommand((int)frmGetValue.Value);
                    }
                    SaveFormLoaction(frmGetValue);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        #endregion  ################################################################################################################

        #region Element menu  ######################################################################################################
        private void tsmiRenumberAllElements_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                //
                using (FrmGetValue frmGetValue = new FrmGetValue())
                {
                    frmGetValue.NumOfDigits = 0;
                    frmGetValue.MinValue = 1;
                    SetFormLoaction(frmGetValue);
                    string desc = "Enter the starting element id for the element renumbering.";
                    frmGetValue.PrepareForm("Renumber Elements", "Start element id", desc, 1, null);
                    if (frmGetValue.ShowDialog() == DialogResult.OK)
                    {
                        _controller.RenumberElementsCommand((int)frmGetValue.Value);
                    }
                    SaveFormLoaction(frmGetValue);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        #endregion  ################################################################################################################

        #region Model part menu  ###################################################################################################
        private void tsmiEditPart_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Parts", _controller.GetModelParts(), EditModelPart);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        // Transform
        private void tsmiTranslateModelParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), TranslateModelParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiScaleModelParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), ScaleModelParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiRotateModelParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), RotateModelParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void tsmiMergeParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), MergeModelParts, 2);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), HideModelParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), ShowModelParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowOnlyParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), ShowOnlyModelParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSetTransparencyForParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), SetTransparencyForModelParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetModelParts(), DeleteModelParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //                                                                                                                          
        private void EditModelPart(string partName)
        {
            _frmPartProperties.View = ViewGeometryModelResults.Model; 
            ShowForm(_frmPartProperties, "Edit Part", partName);
        }
        // Transform
        private void TranslateModelParts(string[] partNames)
        {
            SinglePointDataEditor.ParentForm = _frmTranslate;
            SinglePointDataEditor.Controller = _controller;
            // Set all part names for translation
            _frmTranslate.PartNames = partNames;    
            //
            ShowForm(_frmTranslate, "Translate parts: " + partNames.ToShortString(), null);
        }
        private void ScaleModelParts(string[] partNames)
        {
            SinglePointDataEditor.ParentForm = _frmScale;
            SinglePointDataEditor.Controller = _controller;
            // Set all part names for scaling
            _frmScale.PartNames = partNames;    
            //
            ShowForm(_frmScale, "Scale parts: " + partNames.ToShortString(), null);
        }
        private void RotateModelParts(string[] partNames)
        {
            SinglePointDataEditor.ParentForm = _frmRotate;
            SinglePointDataEditor.Controller = _controller;
            // Set all part names for rotation
            _frmRotate.PartNames = partNames;    
            //
            ShowForm(_frmRotate, "Rotate parts: " + partNames.ToShortString(), null);
        }
        //
        private void MergeModelParts(string[] partNames)
        {
            if (_controller.AreModelPartsMergable(partNames))
            {
                if (MessageBoxes.ShowWarningQuestion("OK to merge selected parts?") == DialogResult.OK)
                {
                    _controller.MergeModelPartsCommand(partNames);
                }
            }
            else MessageBoxes.ShowError("Selected parts are of a different type and thus can not be merged.");
        }
        private void HideModelParts(string[] partNames)
        {
            _controller.HideModelPartsCommand(partNames);
        }
        private void ShowModelParts(string[] partNames)
        {
            _controller.ShowModelPartsCommand(partNames);
        }
        private void ShowOnlyModelParts(string[] partNames)
        {
            HashSet<string> allNames = new HashSet<string>(_controller.Model.Mesh.Parts.Keys);
            allNames.ExceptWith(partNames);
            _controller.HideModelPartsCommand(allNames.ToArray());
            _controller.ShowModelPartsCommand(partNames);
        }
        private void SetTransparencyForModelParts(string[] partNames)
        {
            if (_controller.Model.Mesh == null) return;
            //
            using (FrmGetValue frmGetValue = new FrmGetValue())
            {
                frmGetValue.NumOfDigits = 0;
                frmGetValue.MinValue = 25;
                frmGetValue.MaxValue = 255;
                SetFormLoaction(frmGetValue);
                OrderedDictionary<string, double> presetValues
                    = new OrderedDictionary<string, double>("Preset Transparency Values", StringComparer.OrdinalIgnoreCase);
                presetValues.Add("Semi-transparent", 128);
                presetValues.Add("Opaque", 255);
                string desc = "Enter the transparency between 0 and 255.\n" + "(0 - transparent; 255 - opaque)";
                frmGetValue.PrepareForm("Set Transparency: " + partNames.ToShortString(), "Transparency", desc, 128, presetValues);
                if (frmGetValue.ShowDialog() == DialogResult.OK)
                {
                    _controller.SetTransparencyForModelPartsCommand(partNames, (byte)frmGetValue.Value);
                }
                SaveFormLoaction(frmGetValue);
            }
        }
        private void DeleteModelParts(string[] partNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected parts?" + Environment.NewLine
                                                 + partNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveModelPartsCommand(partNames);
            }
        }
        
        #endregion  ################################################################################################################

        #region Node set menu  #####################################################################################################
        private void tsmiCreateNodeSet_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                // Data editor
                ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
                ItemSetDataEditor.ParentForm = _frmNodeSet;
                _frmSelectItemSet.SetOnlyGeometrySelection(false);
                ShowForm(_frmNodeSet, "Create Node Set", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditNodeSet_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Node Sets", _controller.GetUserNodeSets(), EditNodeSet);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDuplicateNodeSet_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Node Sets", _controller.GetUserNodeSets(), DuplicateNodeSets);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteNodeSet_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Node Sets", _controller.GetUserNodeSets(), DeleteNodeSets);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditNodeSet(string nodeSetName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmNodeSet;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmNodeSet, "Edit Node Set", nodeSetName);
        }
        private void DuplicateNodeSets(string[] nodeSetNames)
        {
            _controller.DuplicateNodeSetsCommand(nodeSetNames);
        }
        private void DeleteNodeSets(string[] nodeSetNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected node sets?" + Environment.NewLine
                                                 + nodeSetNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveNodeSetsCommand(nodeSetNames);
            }
        }

        #endregion  ################################################################################################################

        #region Element set menu  ##################################################################################################
        private void tsmiCreateElementSet_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                // Data editor
                ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
                ItemSetDataEditor.ParentForm = _frmElementSet;
                _frmSelectItemSet.SetOnlyGeometrySelection(false);
                ShowForm(_frmElementSet, "Create Element Set", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditElementSet_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Element Sets", _controller.GetUserElementSets(), EditElementSet);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDuplicateElementSet_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Element Sets", _controller.GetUserElementSets(), DuplicateElementSets);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiConvertElementSetsToMeshParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Element Sets", _controller.GetUserElementSets(), ConvertElementSetsToMeshParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteElementSet_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Element Sets", _controller.GetUserElementSets(), DeleteElementSets);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditElementSet(string elementSetName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmElementSet;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmElementSet, "Edit Element Set", elementSetName);
        }
        private void DuplicateElementSets(string[] elementSetNames)
        {
            _controller.DuplicateElementSetsCommand(elementSetNames);
        }
        private void ConvertElementSetsToMeshParts(string[] elementSetNames)
        {
            _controller.ConvertElementSetsToMeshPartsCommand(elementSetNames);
        }
        private void DeleteElementSets(string[] elementSetNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected element sets?" + Environment.NewLine
                                                 + elementSetNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveElementSetsCommand(elementSetNames);
            }
        }

        #endregion  ################################################################################################################

        #region Surface menu  ######################################################################################################
        private void tsmiCreateSurface_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                // Data editor
                ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
                ItemSetDataEditor.ParentForm = _frmSurface;
                _frmSelectItemSet.SetOnlyGeometrySelection(false);
                ShowForm(_frmSurface, "Create Surface", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditSurface_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Surfaces", _controller.GetAllSurfaces(), EditSurface);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteSurface_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Surfaces", _controller.GetAllSurfaces(), DeleteSurfaces);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void EditSurface(string surfaceName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmSurface;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmSurface, "Edit Surface", surfaceName);
        }
        private void DeleteSurfaces(string[] surfaceNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected surfaces?" + Environment.NewLine
                                                 + surfaceNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveSurfacesCommand(surfaceNames);
            }
        }

        #endregion  ################################################################################################################

        #region Reference point  ###################################################################################################
        private void tsmiCreateRP_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                //
                ShowForm(_frmReferencePoint, "Create Reference Point", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditRP_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Reference points", _controller.GetAllReferencePoints(), EditRP);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteRP_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Reference points", _controller.GetAllReferencePoints(), DeleteRPs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        private void EditRP(string referencePointName)
        {
            ShowForm(_frmReferencePoint, "Edit Reference Point", referencePointName);
        }
        private void HideRPs(string[] constraintNames)
        {
            _controller.HideReferencePointsCommand(constraintNames);
        }
        private void ShowRPs(string[] constraintNames)
        {
            _controller.ShowReferencePointsCommand(constraintNames);
        }
        private void ShowOnlyRPs(string[] referencePointNames)
        {
            HashSet<string> allNames = new HashSet<string>(_controller.Model.Mesh.ReferencePoints.Keys);
            allNames.ExceptWith(referencePointNames);
            _controller.HideReferencePointsCommand(allNames.ToArray());
            _controller.ShowReferencePointsCommand(referencePointNames);
        }
        private void DeleteRPs(string[] referencePointNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected reference points?" + Environment.NewLine
                                                 + referencePointNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveReferencePointsCommand(referencePointNames);
            }
        }

        #endregion  ################################################################################################################

        #region Material menu  #####################################################################################################
        internal void tsmiCreateMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                ShowForm(_frmMaterial, "Create Material", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        internal void CreateSimpleMaterial(object sender, EventArgs e)
        {
            _frmMaterial.UseSimpleEditor = true;
            tsmiCreateMaterial_Click(sender, e);
        }
        private void tsmiEditMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Materials", _controller.GetAllMaterials(), EditMaterial);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDuplicateMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Materials", _controller.GetAllMaterials(), DuplicateMaterials);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private  void tsmiImportMaterial_Click(object sender, EventArgs e)
        {
            ImportFile(true);
        }
        private void tsmiExportMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Materials", _controller.GetAllMaterials(), ExportMaterials);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Materials", _controller.GetAllMaterials(), DeleteMaterials);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
           
        }
        //
        private void EditMaterial(string materialName)
        {
            ShowForm(_frmMaterial, "Edit Material", materialName);
        }
        private void DuplicateMaterials(string[] materialNames)
        {
            _controller.DuplicateMaterialsCommand(materialNames);
        }
        private async void ExportMaterials(string[] materialNames)
        {
            try
            {
                _controller.CurrentView = ViewGeometryModelResults.Model;
                //
                saveFileDialog.Filter = "Calculix files | *.inp";
                //
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // The filter adds the extension to the file name
                    SetStateWorking(Globals.ExportingText);
                    //
                    await Task.Run(() => _controller.ExportMaterials(materialNames, saveFileDialog.FileName));
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                SetStateReady(Globals.ExportingText);
            }
        }
        private void DeleteMaterials(string[] materialNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected materials?" + Environment.NewLine
                                                 + materialNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveMaterialsCommand(materialNames);
            }
        }
        //
        internal void tsmiMaterialLibrary_Click(object sender, EventArgs e)
        {
            try
            {
                ShowMaterialLibrary();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void ShowMaterialLibrary()
        {
            if (_controller.Model.Mesh != null)
            {
                FrmMaterialLibrary fml = new FrmMaterialLibrary(_controller);
                CloseAllForms();
                SetFormLoaction((Form)fml);
                fml.ShowDialog();                
            }
        }

        #endregion  ################################################################################################################

        #region Section menu  ######################################################################################################
        internal void tsmiCreateSection_Click(object sender, EventArgs e)
        {
            try
            {
                CreateSection();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditSection_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Sections", _controller.GetAllSections(), EditSection);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Sections", _controller.GetAllSections(), DeleteSections);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void CreateSection()
        {
            if (_controller.Model.Mesh == null) return;
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmSection;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmSection, "Create Section", null);
        }
        private void EditSection(string sectionName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmSection;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmSection, "Edit Section", sectionName);
        }
        private void DeleteSections(string[] sectionNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected sections?" + Environment.NewLine
                                                 + sectionNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveSectionsCommand(sectionNames);
            }
        }

        #endregion  ################################################################################################################

        #region Interaction menu  ##################################################################################################

        #region Constraint menu  ###################################################################################################
        private void tsmiCreateConstraint_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                // Data editor
                ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
                ItemSetDataEditor.ParentForm = _frmConstraint;
                _frmSelectItemSet.SetOnlyGeometrySelection(false);
                ShowForm(_frmConstraint, "Create Constraint", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditConstraint_Click(object sender, EventArgs e)
        {
             try
             {
                 SelectOneEntity("Constraints", _controller.GetAllConstraints(), EditConstraint);
             }
             catch (Exception ex)
             {
                 ExceptionTools.Show(this, ex);
             }
        }
        private void tsmiDuplicateConstraint_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Constraints", _controller.GetAllConstraints(), DuplicateConstraints);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSwapMasterSlaveConstraint_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Constaints", _controller.GetAllConstraints(), SwapMasterSlaveConstraints);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiMergeByMasterSlaveConstraint_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Constaints", _controller.GetAllConstraints(), MergeByMasterSlaveConstraints);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideConstraint_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Constaints", _controller.GetAllConstraints(), HideConstraints);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowConstraint_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Constaints", _controller.GetAllConstraints(), ShowConstraints);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteConstraint_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Constraints", _controller.GetAllConstraints(), DeleteConstraints);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditConstraint(string constraintName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmConstraint;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmConstraint, "Edit Constraint", constraintName);
        }
        private void DuplicateConstraints(string[] constraintNames)
        {
            _controller.DuplicateConstraintsCommnad(constraintNames);
        }
        private void SwapMasterSlaveConstraints(string[] constraintNames)
        {
            _controller.SwapMasterSlaveConstraintsCommand(constraintNames);
        }
        private void MergeByMasterSlaveConstraints(string[] constraintNames)
        {
            _controller.MergeByMasterSlaveConstraintsCommand(constraintNames);
        }
        private void HideConstraints(string[] constraintNames)
        {
            _controller.HideConstraintsCommand(constraintNames);
        }
        private void ShowConstraints(string[] constraintNames)
        {
            _controller.ShowConstraintsCommand(constraintNames);
        }
        private void ShowOnlyConstraints(string[] constraintNames)
        {
            HashSet<string> allNames = new HashSet<string>(_controller.Model.Constraints.Keys);
            allNames.ExceptWith(constraintNames);
            _controller.HideConstraintsCommand(allNames.ToArray());
            _controller.ShowConstraintsCommand(constraintNames);
        }
        private void DeleteConstraints(string[] constraintNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected constraints?" + Environment.NewLine
                                                 + constraintNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveConstraintsCommand(constraintNames);
            }
        }

        #endregion  ################################################################################################################
        
        #region Surface interaction menu  ##########################################################################################
        private void tsmiCreateSurfaceInteraction_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                ShowForm(_frmSurfaceInteraction, "Create surface interaction", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditSurfaceInteraction_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Surface interactions", _controller.GetAllSurfaceInteractions(), EditSurfaceInteraction);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDuplicateSurfaceInteraction_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Surface interactions", _controller.GetAllSurfaceInteractions(),
                                       DuplicateSurfaceInteractions);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteSurfaceInteraction_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Surface interactions", _controller.GetAllSurfaceInteractions(), DeleteSurfaceInteractions);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }

        }
        //
        private void EditSurfaceInteraction(string surfaceInteractionName)
        {
            ShowForm(_frmSurfaceInteraction, "Edit surface interaction", surfaceInteractionName);
        }
        private void DuplicateSurfaceInteractions(string[] surfaceInteractionNames)
        {
            _controller.DuplicateSurfaceInteractionsCommand(surfaceInteractionNames);
        }
        private void DeleteSurfaceInteractions(string[] surfaceInteractionNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected surface interactions?" + Environment.NewLine
                                                 + surfaceInteractionNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveSurfaceInteractionsCommand(surfaceInteractionNames);
            }
        }



        #endregion  ################################################################################################################

        #region Contact pair menu  #################################################################################################
        private void tsmiCreateContactPair_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                // Data editor
                ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
                ItemSetDataEditor.ParentForm = _frmContactPair;
                _frmSelectItemSet.SetOnlyGeometrySelection(false);
                ShowForm(_frmContactPair, "Create Contact Pair", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditContactPair_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Contact pairs", _controller.GetAllContactPairs(), EditContactPair);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSwapMasterSlaveContactPair_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Contact pairs", _controller.GetAllContactPairs(), SwapMasterSlaveContactPairs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiMergeByMasterSlaveContactPair_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Contact pairs", _controller.GetAllContactPairs(), MergeByMasterSlaveContactPairs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideContactPair_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Contact pairs", _controller.GetAllContactPairs(), HideContactPairs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowContactPair_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Contact pairs", _controller.GetAllContactPairs(), ShowContactPairs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteContactPair_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Contact pairs", _controller.GetAllContactPairs(), DeleteContactPairs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditContactPair(string contactPairName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmContactPair;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmContactPair, "Edit Contact Pair", contactPairName);
        }
        private void SwapMasterSlaveContactPairs(string[] contactPairNames)
        {
            _controller.SwapMasterSlaveContactPairsCommand(contactPairNames);
        }
        private void MergeByMasterSlaveContactPairs(string[] contactPairNames)
        {
            _controller.MergeByMasterSlaveContactPairsCommand(contactPairNames);
        }
        private void HideContactPairs(string[] contactPairNames)
        {
            _controller.HideContactPairsCommand(contactPairNames);
        }
        private void ShowContactPairs(string[] contactPairNames)
        {
            _controller.ShowContactPairsCommand(contactPairNames);
        }
        private void ShowOnlyContactPairs(string[] contactPairNames)
        {
            HashSet<string> allNames = new HashSet<string>(_controller.Model.ContactPairs.Keys);
            allNames.ExceptWith(contactPairNames);
            _controller.HideContactPairsCommand(allNames.ToArray());
            _controller.ShowContactPairsCommand(contactPairNames);
        }
        private void DeleteContactPairs(string[] contactPairNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected contact pairs?" + Environment.NewLine
                                                 + contactPairNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveContactPairsCommand(contactPairNames);
            }
        }
        #endregion  ################################################################################################################

        private void tsmiSearchContactPairs_Click(object sender, EventArgs e)
        {
            try
            {
                SearchContactPairs();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void SearchContactPairs()
        {
            if (_controller.Model == null || _controller.Model.Mesh == null) return;
            //
            if (!_frmSearchContactPairs.Visible)
            {
                CloseAllForms();
                SetFormLoaction(_frmSearchContactPairs);
                _frmSearchContactPairs.PrepareForm();
                _frmSearchContactPairs.Show(this);
            }
        }

        #endregion  ################################################################################################################

        #region Amplitude menu  ####################################################################################################
        private void tsmiCreateAmplitude_Click(object sender, EventArgs e)
        {
            try
            {
                CreateAmplitude();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditAmplitude_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Amplitudes", _controller.GetAllAmplitudes(), EditAmplitude);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteAmplitude_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Amplitudes", _controller.GetAllAmplitudes(), DeleteAmplitudes);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void CreateAmplitude()
        {
            if (_controller.Model.Mesh == null) return;
            ShowForm(_frmAmplitude, "Create Amplitude", null);
        }
        private void EditAmplitude(string amplitudeName)
        {
            ShowForm(_frmAmplitude, "Edit Amplitude", amplitudeName);
        }
        private void DeleteAmplitudes(string[] amplitudeNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected amplitudes?" + Environment.NewLine
                                                 + amplitudeNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveAmplitudesCommand(amplitudeNames);
            }
        }

        #endregion  ################################################################################################################

        #region Initial condition menu  ############################################################################################
        private void tsmiCreateInitialCondition_Click(object sender, EventArgs e)
        {
            try
            {
                CreateInitialCondition();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditInitialCondition_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Initial Conditions", _controller.GetAllInitialConditions(), EditInitialCondition);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiPreviewInitialCondition_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Initial Conditions", _controller.GetAllInitialConditions(), PreviewInitialCondition);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteInitialCondition_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Initial Conditions", _controller.GetAllInitialConditions(), DeleteInitialConditions);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void CreateInitialCondition()
        {
            if (_controller.Model.Mesh == null) return;
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmInitialCondition;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmInitialCondition, "Create Initial Condition", null);
        }
        private void EditInitialCondition(string initialConditionName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmInitialCondition;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmInitialCondition, "Edit Initial Condition", initialConditionName);
        }
        private void PreviewInitialCondition(string[] initialConditionNames)
        {
            foreach (var name in initialConditionNames) PreviewInitialCondition(name);
        }
        private void PreviewInitialCondition(string initialConditionName)
        {
            _controller.PreviewInitialCondition(initialConditionName);
            //
            if (_controller.CurrentResult != null && _controller.CurrentResult.Mesh != null)
            {
                SetResultNames();
                // Reset the previous step and increment
                SetAllStepAndIncrementIds();
                // Set last increment
                SetDefaultStepAndIncrementIds();
                // Show the selection in the results tree
                SelectFirstComponentOfFirstFieldOutput();
            }
            // Set the representation which also calls Draw
            _controller.ViewResultsType = ViewResultsTypeEnum.ColorContours;  // Draw
            //
            SetMenuAndToolStripVisibility();
        }
        private void DeleteInitialConditions(string[] initialConditionNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected initial conditions?" + Environment.NewLine
                                                 + initialConditionNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveInitialConditionsCommand(initialConditionNames);
            }
        }

        #endregion  ################################################################################################################

        #region Step menu  #########################################################################################################
        internal void tsmiCreateStep_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                //
                int selectedIndex = -1;
                if (e is EventArgs<int> ea) selectedIndex = ea.Value;
                _frmStep.PreselectListViewItem(selectedIndex);
                //
                ShowForm(_frmStep, "Create Step", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditStep_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), EditStep);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDuplicateStep_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Steps", _controller.GetAllSteps(), DuplicateSteps);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteStep_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Steps", _controller.GetAllSteps(), DeleteSteps);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        private void EditStep(string stepName)
        {
            ShowForm(_frmStep, "Edit Step", stepName);
        }
        private void DuplicateSteps(string[] stepNames)
        {
            _controller.DuplicateStepsCommnad(stepNames);
        }
        private void DeleteSteps(string[] stepNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected steps?" + Environment.NewLine
                                                 + stepNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveStepsCommnad(stepNames);
            }
        }

        #endregion  ################################################################################################################

        #region History output menu  ###############################################################################################
        private void tsmiCreateHistoryOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), CreateHistoryOutput);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditHistoryOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndEditHistoryOutput);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiPropagateHistoryOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndPropagateHistoryOutput);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteHistoryOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndDeleteHistoryOutputs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void SelectAndEditHistoryOutput(string stepName)
        {
            SelectOneEntityInStep("History outputs", _controller.GetAllHistoryOutputs(stepName), stepName, EditHistoryOutput);
        }
        private void SelectAndPropagateHistoryOutput(string stepName)
        {
            SelectOneEntityInStep("History outputs", _controller.GetAllHistoryOutputs(stepName), stepName, PropagateHistoryOutput);
        }
        private void SelectAndDeleteHistoryOutputs(string stepName)
        {
            SelectMultipleEntitiesInStep("History outputs", _controller.GetAllHistoryOutputs(stepName),
                                         stepName, DeleteHistoryOutputs);
        }
        //
        private void CreateHistoryOutput(string stepName)
        {
            if (_controller.Model.Mesh == null) return;
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmHistoryOutput;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmHistoryOutput, "Create History Output", stepName, null);
        }
        private void EditHistoryOutput(string stepName, string historyOutputName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmHistoryOutput;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmHistoryOutput, "Edit History Output", stepName, historyOutputName);
        }
        private void PropagateHistoryOutput(string stepName, string historyOutputName)
        {
            bool exists = false;
            string[] nextStepNames = _controller.Model.StepCollection.GetNextStepNames(stepName);
            //
            foreach (var nextStepName in nextStepNames)
            {
                if (_controller.Model.StepCollection.GetStep(nextStepName).HistoryOutputs.ContainsKey(historyOutputName))
                {
                    exists = true;
                    break;
                }
            }
            //
            bool propagate = true;
            if (exists)
            {
                if (MessageBoxes.ShowWarningQuestion("OK to overwrite the existing history output " + historyOutputName
                                                     + "?") == DialogResult.Cancel) propagate = false;
            }
            if (propagate) _controller.PropagateHistoryOutputCommand(stepName, historyOutputName);
        }
        private void DeleteHistoryOutputs(string stepName, string[] historyOutputNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected history outputs from step " + stepName + "?"
                                                 + Environment.NewLine + historyOutputNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveHistoryOutputsForStepCommand(stepName, historyOutputNames);
            }
        }
        #endregion  ################################################################################################################

        #region Field output menu  #################################################################################################
        private void tsmiCreateFieldOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), CreateFieldOutput);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditFieldOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndEditFieldOutput);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiPropagateFieldOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndPropagateFieldOutput);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteFieldOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndDeleteFieldOutputs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void SelectAndEditFieldOutput(string stepName)
        {
            SelectOneEntityInStep("Field outputs", _controller.GetAllFieldOutputs(stepName), stepName, EditFieldOutput);
        }
        private void SelectAndPropagateFieldOutput(string stepName)
        {
            SelectOneEntityInStep("Field outputs", _controller.GetAllFieldOutputs(stepName), stepName, PropagateFieldOutput);
        }
        private void SelectAndDeleteFieldOutputs(string stepName)
        {
            SelectMultipleEntitiesInStep("Field outputs", _controller.GetAllFieldOutputs(stepName), stepName, DeleteFieldOutputs);
        }
        //
        private void CreateFieldOutput(string stepName)
        {
            if (_controller.Model.Mesh == null) return;
            //
            ShowForm(_frmFieldOutput, "Create Field Output", stepName, null);
        }
        private void EditFieldOutput(string stepName, string fieldOutputName)
        {
            ShowForm(_frmFieldOutput, "Edit Field Output", stepName, fieldOutputName);
        }
        private void PropagateFieldOutput(string stepName, string fieldOutputName)
        {
            bool exists = false;
            string[] nextStepNames = _controller.Model.StepCollection.GetNextStepNames(stepName);
            //
            foreach (var nextStepName in nextStepNames)
            {
                if (_controller.Model.StepCollection.GetStep(nextStepName).FieldOutputs.ContainsKey(fieldOutputName))
                {
                    exists = true;
                    break;
                }
            }
            //
            bool propagate = true;
            if (exists)
            {
                if (MessageBoxes.ShowWarningQuestion("OK to overwrite the existing filed output " + fieldOutputName
                                                     + "?") == DialogResult.Cancel) propagate = false;
            }
            if (propagate) _controller.PropagateFieldOutputCommand(stepName, fieldOutputName);
        }
        private void DeleteFieldOutputs(string stepName, string[] fieldOutputNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected field outputs from step " + stepName + "?"
                                                 + Environment.NewLine + fieldOutputNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveFieldOutputsForStepCommand(stepName, fieldOutputNames);
            }
        }

        #endregion  ################################################################################################################

        #region Boundary condition menu  ###########################################################################################
        internal void tsmiCreateBC_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = -1;
                if (e is EventArgs<int> ea) selectedIndex = ea.Value;
                _frmBoundaryCondition.PreselectListViewItem(selectedIndex);
                //
                SelectOneEntity("Steps", _controller.GetAllSteps(), CreateBoundaryCondition);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditBC_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndEditBoundaryCondition);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiPropagateBC_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndPropagateBoundaryCondition);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideBC_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndHideBoundaryConditions);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowBC_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndShowBoundaryConditions);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteBC_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndDeleteBoundaryCondition);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void SelectAndEditBoundaryCondition(string stepName)
        {
            SelectOneEntityInStep("Boundary conditions", _controller.GetStepBoundaryConditions(stepName), stepName,
                                  EditBoundaryCondition);
        }
        private void SelectAndPropagateBoundaryCondition(string stepName)
        {
            SelectOneEntityInStep("Boundary conditions", _controller.GetStepBoundaryConditions(stepName), stepName,
                                  PropagateBoundaryCondition);
        }
        private void SelectAndHideBoundaryConditions(string stepName)
        {
            SelectMultipleEntitiesInStep("Boundary conditions", _controller.GetStepBoundaryConditions(stepName),
                                         stepName, HideBoundaryConditions);
        }
        private void SelectAndShowBoundaryConditions(string stepName)
        {
            SelectMultipleEntitiesInStep("Boundary conditions", _controller.GetStepBoundaryConditions(stepName),
                                         stepName, ShowBoundaryConditions);
        }
        private void SelectAndDeleteBoundaryCondition(string stepName)
        {
            SelectMultipleEntitiesInStep("Boundary conditions", _controller.GetStepBoundaryConditions(stepName),
                                         stepName, DeleteBoundaryConditions);
        }
        //
        private void CreateBoundaryCondition(string stepName)
        {
            if (_controller.Model.Mesh == null) return;
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmBoundaryCondition;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmBoundaryCondition, "Create Boundary Condition", stepName, null);
        }
        private void EditBoundaryCondition(string stepName, string boundaryConditionName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmBoundaryCondition;
            _frmSelectItemSet.SetOnlyGeometrySelection(true);
            ShowForm(_frmBoundaryCondition, "Edit Boundary Condition", stepName, boundaryConditionName);
        }
        private void PropagateBoundaryCondition(string stepName, string boundaryConditionName)
        {
            bool exists = false;
            string[] nextStepNames = _controller.Model.StepCollection.GetNextStepNames(stepName);
            //
            foreach (var nextStepName in nextStepNames)
            {
                if (_controller.Model.StepCollection.GetStep(nextStepName).BoundaryConditions.ContainsKey(boundaryConditionName))
                {
                    exists = true;
                    break;
                }
            }
            //
            bool propagate = true;
            if (exists)
            {
                if (MessageBoxes.ShowWarningQuestion("OK to overwrite the existing boundary condition " + boundaryConditionName
                                                     + "?") == DialogResult.Cancel) propagate = false;
            }
            if (propagate) _controller.PropagateBoundaryConditionCommand(stepName, boundaryConditionName);
        }
        private void HideBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            _controller.HideBoundaryConditionCommand(stepName, boundaryConditionNames);
        }
        private void ShowBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            _controller.ShowBoundaryConditionCommand(stepName, boundaryConditionNames);
        }
        private void ShowOnlyBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            HashSet<string> allNames =
                new HashSet<string>(_controller.Model.StepCollection.GetStep(stepName).BoundaryConditions.Keys);
            allNames.ExceptWith(boundaryConditionNames);
            _controller.HideBoundaryConditionCommand(stepName, allNames.ToArray());
            _controller.ShowBoundaryConditionCommand(stepName, boundaryConditionNames);
        }
        private void DeleteBoundaryConditions(string stepName, string[] boundaryConditionNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected boundary conditions from step " + stepName + "?"
                                                 + Environment.NewLine + boundaryConditionNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveBoundaryConditionsCommand(stepName, boundaryConditionNames);
            }
        }

        #endregion  ################################################################################################################

        #region Load menu  #########################################################################################################
        internal void tsmiCreateLoad_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = -1;
                if (e is EventArgs<int> ea) selectedIndex = ea.Value;
                _frmLoad.PreselectListViewItem(selectedIndex);
                //
                SelectOneEntity("Steps", _controller.GetAllSteps(), CreateLoad);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditLoad_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndEditLoad);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiPropagateLoad_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndPropagateLoad);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiPreviewLoad_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndPreviewLoad);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideLoad_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndHideLoads);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowLoad_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndShowLoads);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteLoad_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndDeleteLoad);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void SelectAndEditLoad(string stepName)
        {
            SelectOneEntityInStep("Loads", _controller.GetStepLoads(stepName), stepName, EditLoad);
        }
        private void SelectAndPropagateLoad(string stepName)
        {
            SelectOneEntityInStep("Loads", _controller.GetStepLoads(stepName), stepName, PropagateLoad);
        }
        private void SelectAndPreviewLoad(string stepName)
        {
            SelectOneEntityInStep("Loads", _controller.GetStepLoads(stepName), stepName, PreviewLoad);
        }
        private void SelectAndHideLoads(string stepName)
        {
            SelectMultipleEntitiesInStep("Loads", _controller.GetStepLoads(stepName), stepName, HideLoads);
        }
        private void SelectAndShowLoads(string stepName)
        {
            SelectMultipleEntitiesInStep("Loads", _controller.GetStepLoads(stepName), stepName, ShowLoads);
        }
        private void SelectAndDeleteLoad(string stepName)
        {
            SelectMultipleEntitiesInStep("Loads", _controller.GetStepLoads(stepName), stepName, DeleteLoads);
        }
        //
        private void CreateLoad(string stepName)
        {
            if (_controller.Model.Mesh == null) return;
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmLoad;
            //
            SinglePointDataEditor.ParentForm = _frmLoad;
            SinglePointDataEditor.Controller = _controller;
            //
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmLoad, "Create Load", stepName, null);
        }
        private void EditLoad(string stepName, string loadName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmLoad;
            //
            SinglePointDataEditor.ParentForm = _frmLoad;
            SinglePointDataEditor.Controller = _controller;
            //
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmLoad, "Edit Load", stepName, loadName);
        }
        private void PropagateLoad(string stepName, string loadName)
        {
            bool exists = false;
            string[] nextStepNames = _controller.Model.StepCollection.GetNextStepNames(stepName);
            //
            foreach (var nextStepName in nextStepNames)
            {
                if (_controller.Model.StepCollection.GetStep(nextStepName).Loads.ContainsKey(loadName))
                {
                    exists = true;
                    break;
                }
            }
            //
            bool propagate = true;
            if (exists)
            {
                if (MessageBoxes.ShowWarningQuestion("OK to overwrite the existing load " + loadName
                                                     + "?") == DialogResult.Cancel) propagate = false;
            }
            if (propagate) _controller.PropagateLoadCommand(stepName, loadName);
        }
        private void PreviewLoad(string stepName, string loadName)
        {
            _controller.PreviewLoad(stepName, loadName);
            //
            if (_controller.CurrentResult != null && _controller.CurrentResult.Mesh != null)
            {
                SetResultNames();
                // Reset the previous step and increment
                SetAllStepAndIncrementIds();
                // Set last increment
                SetDefaultStepAndIncrementIds();
                // Show the selection in the results tree
                SelectFirstComponentOfFirstFieldOutput();
            }
            // Set the representation which also calls Draw
            _controller.ViewResultsType = ViewResultsTypeEnum.ColorContours;  // Draw
            //
            SetMenuAndToolStripVisibility();
        }
        private void HideLoads(string stepName, string[] loadNames)
        {
            _controller.HideLoadsCommand(stepName, loadNames);
        }
        private void ShowLoads(string stepName, string[] loadNames)
        {
            _controller.ShowLoadsCommand(stepName, loadNames);
        }
        private void ShowOnlyLoads(string stepName, string[] loadNames)
        {
            HashSet<string> allNames = new HashSet<string>(_controller.Model.StepCollection.GetStep(stepName).Loads.Keys);
            allNames.ExceptWith(loadNames);
            _controller.HideLoadsCommand(stepName, allNames.ToArray());
            _controller.ShowLoadsCommand(stepName, loadNames);
        }
        private void DeleteLoads(string stepName, string[] loadNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected loads from step " + stepName + "?"
                                                 + Environment.NewLine + loadNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveLoadsCommand(stepName, loadNames);
            }
        }

        #endregion  ################################################################################################################

        #region Defined field menu #################################################################################################
        private void tsmiCreateDefinedField_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), CreateDefinedField);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditDefinedField_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndEditDefinedField);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiPropagateDefinedField_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndPropagateDefinedField);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiPreviewDefinedField_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndPreviewDefinedField);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteDefinedField_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Steps", _controller.GetAllSteps(), SelectAndDeleteDefinedFields);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void SelectAndEditDefinedField(string stepName)
        {
            SelectOneEntityInStep("Defined fields", _controller.GetAllDefinedFields(stepName), stepName, EditDefinedField);
        }
        private void SelectAndPropagateDefinedField(string stepName)
        {
            SelectOneEntityInStep("Defined fields", _controller.GetAllDefinedFields(stepName), stepName, PropagateDefinedField);
        }
        private void SelectAndPreviewDefinedField(string stepName)
        {
            SelectOneEntityInStep("Defined fields", _controller.GetAllDefinedFields(stepName), stepName, PreviewDefinedField);
        }
        private void SelectAndDeleteDefinedFields(string stepName)
        {
            SelectMultipleEntitiesInStep("Defined fields", _controller.GetAllDefinedFields(stepName),
                                         stepName, DeleteDefinedFields);
        }
        //
        private void CreateDefinedField(string stepName)
        {
            if (_controller.Model.Mesh == null) return;
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmDefinedField;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmDefinedField, "Create Defined Field", stepName, null);
        }
        private void EditDefinedField(string stepName, string definedFieldName)
        {
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;
            ItemSetDataEditor.ParentForm = _frmDefinedField;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmDefinedField, "Edit Defined Field", stepName, definedFieldName);
        }
        private void PropagateDefinedField(string stepName, string definedFieldName)
        {
            bool exists = false;
            string[] nextStepNames = _controller.Model.StepCollection.GetNextStepNames(stepName);
            //
            foreach (var nextStepName in nextStepNames)
            {
                if (_controller.Model.StepCollection.GetStep(nextStepName).DefinedFields.ContainsKey(definedFieldName))
                {
                    exists = true;
                    break;
                }
            }
            //
            bool propagate = true;
            if (exists)
            {
                if (MessageBoxes.ShowWarningQuestion("OK to overwrite the existing defined field " + definedFieldName
                                                     + "?") == DialogResult.Cancel) propagate = false;
            }
            if (propagate) _controller.PropagateDefinedFieldCommand(stepName, definedFieldName);
        }
        private void PreviewDefinedField(string stepName, string definedFieldName)
        {
            _controller.PreviewDefinedField(stepName, definedFieldName);
            //
            if (_controller.CurrentResult != null && _controller.CurrentResult.Mesh != null)
            {
                SetResultNames();
                // Reset the previous step and increment
                SetAllStepAndIncrementIds();
                // Set last increment
                SetDefaultStepAndIncrementIds();
                // Show the selection in the results tree
                SelectFirstComponentOfFirstFieldOutput();
            }
            // Set the representation which also calls Draw
            _controller.ViewResultsType = ViewResultsTypeEnum.ColorContours;  // Draw
            //
            SetMenuAndToolStripVisibility();
        }
        private void DeleteDefinedFields(string stepName, string[] definedFieldNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected defined fields from step " + stepName + "?"
                                                 + Environment.NewLine + definedFieldNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveDefinedFieldsForStepCommand(stepName, definedFieldNames);
            }
        }
        
        #endregion  ################################################################################################################

        #region Tools ##############################################################################################################
        private void tsmiSettings_Click(object sender, EventArgs e)
        {
            try
            {
                //string settingsName = Globals.GraphicsSettingsName;
                //if (_controller.CurrentView == ViewGeometryMeshResults.Results) settingsName = Globals.PostSettingsName;

                if (!_frmSettings.Visible)
                {
                    CloseAllForms();
                    SetFormLoaction(_frmSettings);
                    _frmSettings.PrepareForm(_controller);
                    _frmSettings.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
           
        }
        private void tsmiQuery_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_frmQuery.Visible)
                {
                    ClearSelection();
                    //
                    CloseAllForms();
                    SetFormLoaction(_frmQuery);
                    _frmQuery.PrepareForm(_controller);
                    _frmQuery.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_frmFind.Visible)
                {
                    ClearSelection();
                    //
                    CloseAllForms();
                    SetFormLoaction(_frmFind);
                    _frmFind.PrepareForm(_controller);
                    _frmFind.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void ShowColorBarSettings()
        {
            _frmSettings.SetSettingsToShow(Globals.PreSettingsName);
            tsmiSettings_Click(null, null);
        }
        private void ShowAnnotationSettings()
        {
            _frmSettings.SetSettingsToShow(Globals.AnnotationSettingsName);
            tsmiSettings_Click(null, null);
        }
        private void ShowLegendSettings()
        {
            _frmSettings.SetSettingsToShow(Globals.LegendSettingsName);
            tsmiSettings_Click(null, null);
        }
        private void ShowStatusBlockSettings()
        {
            _frmSettings.SetSettingsToShow(Globals.StatusBlockSettingsName);
            tsmiSettings_Click(null, null);
        }
        // Annotations
        private void StartEditArrowAnnotation(string name, Rectangle rectangle)
        {
            if (AnnotationContainer.IsAnnotationNameReserved(name)) return;
            //
            AnnotationBase annotation = _controller.Annotations.GetCurrentAnnotation(name);
            string text = annotation.GetAnnotationText();
            rectangle.Offset(-4, 4);
            //
            Point vtkLocation = this.PointToClient(_vtk.PointToScreen(_vtk.Location));
            Point location = new Point(vtkLocation.X + rectangle.X,
                                       vtkLocation.Y + (_vtk.Height - rectangle.Y - rectangle.Height));
            Rectangle vtkArea = new Rectangle(vtkLocation, _vtk.Size);
            //
            aeAnnotationTextEditor.Location = location;
            aeAnnotationTextEditor.Size = rectangle.Size;
            aeAnnotationTextEditor.MinSize = rectangle.Size;
            aeAnnotationTextEditor.ParentArea = vtkArea;
            aeAnnotationTextEditor.Text = text;
            aeAnnotationTextEditor.BringToFront();
            aeAnnotationTextEditor.Visible = true;
            aeAnnotationTextEditor.Tag = annotation;
            //
            _vtk.SelectBy = vtkSelectBy.Widget;
            //_vtk.DisableInteractor = true;
        }
        private void EndEditArrowAnnotation()
        {
            if (aeAnnotationTextEditor.Visible)
            {
                AnnotationBase annotation = (AnnotationBase)aeAnnotationTextEditor.Tag;
                string nonOverridenText = annotation.GetNotOverridenAnnotationText();
                //
                nonOverridenText = nonOverridenText.Replace("\r\n", "\n");
                string newText = aeAnnotationTextEditor.Text.Replace("\r\n", "\n");
                //
                if (newText.Length > 0 && newText != nonOverridenText)
                    annotation.OverridenText = aeAnnotationTextEditor.Text;
                else
                    annotation.OverridenText = null;
                //
                aeAnnotationTextEditor.Visible = false;
                //
                //_vtk.DisableInteractor = false;
                _vtk.SelectBy = vtkSelectBy.Default;
                //
                _controller.Annotations.DrawAnnotations();  // redraw in both cases
            }
        }
        public override void LeftMouseDownOnForm(Control sender)
        {
            if (aeAnnotationTextEditor.Visible && !aeAnnotationTextEditor.IsOrContainsControl(sender))
                EndEditArrowAnnotation();
        }
        public void AnnotationPicked(MouseEventArgs e, Keys modifierKeys, string annotationName, Rectangle annotationRectangle)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 2)
            {
                StartEditArrowAnnotation(annotationName, annotationRectangle);
            }
            else if (e.Button == MouseButtons.Right)
            {
                tsmiDeleteAnnotation.Tag = new object[] { annotationName, annotationRectangle };
                //
                bool reserved = AnnotationContainer.IsAnnotationNameReserved(annotationName);
                tsmiEditAnnotation.Enabled = !reserved;
                tsmiResetAnnotation.Enabled = !reserved;
                tsmiDeleteAnnotation.Enabled = !reserved;
                cmsAnnotation.Show(_vtk, new Point(e.X, _vtk.Height-  e.Y));
            }
        }
        private void tsmiEditAnnotation_Click(object sender, EventArgs e)
        {
            try
            {
                object[] tag = (object[])tsmiDeleteAnnotation.Tag;
                if (tag[0] is string annotationName && tag[1] is Rectangle annotationRectangle)
                {
                    StartEditArrowAnnotation(annotationName, annotationRectangle);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiResetAnnotation_Click(object sender, EventArgs e)
        {
            try
            {
                object[] tag = (object[])tsmiDeleteAnnotation.Tag;
                if (tag[0] is string annotationName)
                {
                    if (AnnotationContainer.IsAnnotationNameReserved(annotationName)) return;
                    //
                    _controller.Annotations.ResetAnnotation(annotationName);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiAnnotationSettings_Click(object sender, EventArgs e)
        {
            try
            {
                ShowAnnotationSettings();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteAnnotation_Click(object sender, EventArgs e)
        {
            try
            {
                object[] tag = (object[])tsmiDeleteAnnotation.Tag;
                if (tag[0] is string annotationName)
                {
                    if (AnnotationContainer.IsAnnotationNameReserved(annotationName)) return;
                    //
                    if (MessageBoxes.ShowWarningQuestion("OK to delete selected annotation?") == DialogResult.OK)
                    {
                        _controller.Annotations.RemoveCurrentArrowAnnotation(annotationName);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        // Settings
        private void UpdateSettings(Dictionary<string, ISettings> items)
        {
            _controller.Settings = new SettingsContainer(items);
        }

        // Unit system
        private void tsslUnitSystem_Click(object sender, EventArgs e)
        {
            try
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    if (GetCurrentView() == ViewGeometryModelResults.Geometry ||
                        GetCurrentView() == ViewGeometryModelResults.Model)
                    {
                        IfNeededSelectAndSetNewModelProperties();
                    }
                    else SelectResultsUnitSystem();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void IfNeededSelectAndSetNewModelProperties()
        {
            try
            {
                UnitSystemType unitSystemType = _controller.Model.UnitSystem.UnitSystemType;
                ModelSpaceEnum modelSpace = _controller.Model.Properties.ModelSpace;
                // If needed
                if (modelSpace == ModelSpaceEnum.Undefined || unitSystemType == UnitSystemType.Undefined)
                {
                    // Select and set
                    if (SelectNewModelProperties(false)) SetNewModelProperties();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private bool SelectNewModelProperties(bool cancelPossible)
        {
            DialogResult dialogResult = DialogResult.Cancel;
            //
            InvokeIfRequired(() =>
            {
                try
                {                    
                    // Disable the form during regenerate - check that the state is ready
                    if (tsslState.Text != Globals.RegeneratingText)
                    {
                        CloseAllForms();
                        SetFormLoaction(_frmNewModel);
                        //
                        if (_frmNewModel.PrepareForm("", "New Model"))
                        {
                            _frmNewModel.SetCancelPossible(cancelPossible);
                            dialogResult = _frmNewModel.ShowDialog(this);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionTools.Show(this, ex);
                }
            });
            //
            return dialogResult == DialogResult.OK;
        }
        private void SetNewModelProperties()
        {
            InvokeIfRequired(() =>
            {
                try
                {
                    if (_frmNewModel.ModelSpace.IsTwoD())
                    {
                        if ((_controller.Model.Geometry != null && !_controller.Model.Geometry.BoundingBox.Is2D())
                            || (_controller.Model.Mesh != null && !_controller.Model.Mesh.BoundingBox.Is2D()))
                            throw new CaeException("Use of the 2D model space is not possible. The geometry or the mesh " +
                                                   "do not contain 2D geometry in x-y plane.");
                    }
                    _controller.SetNewModelPropertiesCommand(_frmNewModel.ModelSpace, _frmNewModel.UnitSystem.UnitSystemType);
                }
                catch (Exception ex)
                {
                    ExceptionTools.Show(this, ex);
                }
            });
        }
        public void SelectResultsUnitSystem()
        {
            InvokeIfRequired(() =>
            {
                try
                {
                    // Disable unit system selection during regenerate - check that the state is ready
                    if (tsslState.Text != Globals.RegeneratingText)
                    {
                        UnitSystemType unitSystemType = _controller.CurrentResult.UnitSystem.UnitSystemType;
                        //
                        if (unitSystemType == UnitSystemType.Undefined)
                        {
                            CloseAllForms();
                            SetFormLoaction(_frmNewModel);
                            //
                            if (_frmNewModel.PrepareForm("", "Results"))
                            {
                                if (_frmNewModel.ShowDialog(this) == DialogResult.OK)
                                {
                                    _controller.SetResultsUnitSystem(_frmNewModel.UnitSystem.UnitSystemType);
                                }
                                else throw new NotSupportedException();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionTools.Show(this, ex);
                }
            });
        }
        public void UpdateUnitSystem(UnitSystem unitSystem)
        {
            tsslUnitSystem.Text = "Unit system: " + unitSystem.UnitSystemType.GetDescription();
            //
            SetScaleWidgetUnit(unitSystem);
        }
        #endregion  ################################################################################################################

        #region Analysis menu  #####################################################################################################
        private void tsmiCreateAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model.Mesh == null) return;
                ShowForm(_frmAnalysis, "Create Analysis", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Analyses", _controller.GetAllJobs(), EditAnalysis);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        internal void tsmiRunAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Analyses", _controller.GetAllJobs(), RunAnalysis);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiCheckModel_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Analyses", _controller.GetAllJobs(), CheckModel);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiMonitorAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Analyses", _controller.GetAllJobs(), MonitorAnalysis);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        internal void tsmiResultsAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Analyses", _controller.GetAllJobs(), ResultsAnalysis);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiKillAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Analyses", _controller.GetAllJobs(), KillAnalysis);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Analyses", _controller.GetAllJobs(), DeleteAnalyses);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditAnalysis(string jobName)
        {
            ShowForm(_frmAnalysis, "Edit Analysis", jobName);
        }
        private void RunAnalysis(string jobName)
        {
            RunAnalysis(jobName, false);
        }
        private void CheckModel(string jobName)
        {
            RunAnalysis(jobName, true);
        }
        private void RunAnalysis(string jobName, bool onlyCheckModel)
        {
            // Check validity
            if (CheckValiditiy())
            {
                string workDirectory = _controller.Settings.GetWorkDirectory();
                //
                if (workDirectory == null || !Directory.Exists(workDirectory))
                    throw new Exception("The work directory of the analysis does not exist.");
                //
                AnalysisJob job = _controller.GetJob(jobName);
                if (job.JobStatus != JobStatus.Running)
                {
                    string inputFileName = Path.Combine(workDirectory, jobName + ".inp");
                    if (File.Exists(inputFileName))
                    {
                        if (MessageBoxes.ShowWarningQuestion("Overwrite existing analysis files?") != DialogResult.OK) return;
                    }
                    //
                    if (_controller.PrepareAndRunJob(inputFileName, job, onlyCheckModel)) MonitorAnalysis(jobName);
                }
                else MessageBoxes.ShowError("The analysis is already running or in queue.");
            }
        }
        private void MonitorAnalysis(string jobName)
        {
            try
            {
                CloseAllForms();
                SetFormLoaction(_frmMonitor);
                _frmMonitor.PrepareForm(jobName);
                _frmMonitor.ShowDialog(this);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private async void ResultsAnalysis(string jobName)
        {
            AnalysisJob job = _controller.GetJob(jobName);
            if (job.JobStatus == JobStatus.OK || job.JobStatus == JobStatus.Running ||
                job.JobStatus == JobStatus.FailedWithResults)
            {
                string resultsFile = job.Name + ".frd";
                //
                await OpenAsync(Path.Combine(job.WorkDirectory, resultsFile), _controller.Open, false,
                    () => { if (_controller.CurrentResult != null && _controller.CurrentResult.Mesh != null)
                            _frmMonitor.DialogResult = DialogResult.OK; }); // this hides the monitor window
            }
            else
            {
                MessageBoxes.ShowError("The analysis did not complete.");
            }
        }
        private void KillAnalysis(string jobName)
        {
            if (_controller.GetJob(jobName).JobStatus == JobStatus.Running)
            {
                if (MessageBoxes.ShowWarningQuestion("OK to kill selected analysis?") == DialogResult.OK)
                {
                    _controller.KillJob(jobName);
                }
            }
            else
            {
                MessageBoxes.ShowError("The analysis is not running.");
            }
        }
        private void DeleteAnalyses(string[] jobNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected analyses?" + Environment.NewLine
                                                 + jobNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveJobsCommand(jobNames);
            }
        }
        public void UpdateAnalysisProgress()
        {
            _frmMonitor.UpdateProgress();
        }
        //
        
        public AnalysisJob GetDefaultJob()
        {
            try
            {
                List<AnalysisJob> analysisJob = new List<AnalysisJob>();
                InvokeIfRequired(GetDefaultJob, analysisJob);
                return analysisJob[0];
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
                return null;
            }
        }
        private void GetDefaultJob(List<AnalysisJob> defaultJob)
        {
            _frmAnalysis.PrepareForm(null, null);
            AnalysisJob job = _frmAnalysis.Job;
            //
            if (_controller.OpenedFileName != null)
            {
                job.Name = NamedClass.GetErrorFreeName(Path.GetFileNameWithoutExtension(_controller.OpenedFileName));
            }
            //
            defaultJob.Add(job);
        }

        #endregion  ################################################################################################################

        #region Results  ###########################################################################################################
        public void ViewResultHistoryOutputData(HistoryResultData historyData)
        {
            try
            {
                if (!_frmViewResultHistoryOutput.Visible)
                {
                    CloseAllForms();
                    SetFormLoaction(_frmViewResultHistoryOutput);
                    //
                    string[] columnNames;
                    object[][] rowBasedData;
                    _controller.GetHistoryOutputData(historyData, out columnNames, out rowBasedData);
                    //
                    _frmViewResultHistoryOutput.SetData(columnNames, rowBasedData);
                    _frmViewResultHistoryOutput.Show();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiTransformation_Click(object sender, EventArgs e)
        {
            try
            {
                SinglePointDataEditor.ParentForm = _frmTransformation;
                SinglePointDataEditor.Controller = _controller;
                //
                ShowForm(_frmTransformation, "Create Transformation", null);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private async void tsmiAppendResults_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Calculix result files|*.frd";
                    openFileDialog.Multiselect = false;
                    //
                    openFileDialog.FileName = "";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        foreach (var fileName in openFileDialog.FileNames)
                        {
                            // Do not use: if (CheckBeforeOpen(fileName))
                            if (File.Exists(fileName)) await OpenAsync(fileName, _controller.AppendResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
                _controller.ModelChanged = false;   // hide messagebox
                tsmiNew_Click(null, null);
            }
        }

        #endregion  ################################################################################################################

        #region Result part menu  ##################################################################################################
        private void tsmiEditResultParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Parts", _controller.GetResultParts(), EditResultPart);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiHideResultParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), HideResultParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowResultParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), ShowResultParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiShowOnlyResultParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), ShowOnlyResultParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiSetTransparencyForResultParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), SetTransparencyForResultParts);
                Clear3DSelection();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiColorContoursOff_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts<ResultPart>(), ColorContoursOffResultPart);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiColorContoursOn_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts<ResultPart>(), ColorContoursOnResultPart);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }

            
        }
        private void tsmiDeleteResultParts_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Parts", _controller.GetResultParts(), DeleteResultParts);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void EditResultPart(string partName)
        {
            _frmPartProperties.View = ViewGeometryModelResults.Results;
            ShowForm(_frmPartProperties, "Edit Part", partName);
        }
        private void MergeResultParts(string[] partNames)
        {
            if (_controller.AreResultPartsMergable(partNames))
            {
                if (MessageBoxes.ShowWarningQuestion("OK to merge selected parts?") == DialogResult.OK)
                {
                    _controller.MergeResultParts(partNames);
                }
            }
            else MessageBoxes.ShowError("Selected parts are of a different type and thus can not be merged.");
        }
        private void HideResultParts(string[] partNames)
        {
            _controller.HideResultParts(partNames);
        }
        private void ShowResultParts(string[] partNames)
        {
            _controller.ShowResultParts(partNames);
        }
        private void ShowOnlyResultParts(string[] partNames)
        {
            HashSet<string> allNames = new HashSet<string>(_controller.CurrentResult.Mesh.Parts.Keys);
            allNames.ExceptWith(partNames);
            _controller.ShowResultParts(partNames);
            _controller.HideResultParts(allNames.ToArray());
        }
        private void SetTransparencyForResultParts(string[] partNames)
        {
            if (_controller.CurrentResult == null || _controller.CurrentResult.Mesh == null) return;
            //
            using (FrmGetValue frmGetValue = new FrmGetValue())
            {
                frmGetValue.NumOfDigits = 0;
                frmGetValue.MinValue = 25;
                frmGetValue.MaxValue = 255;
                SetFormLoaction(frmGetValue);
                OrderedDictionary<string, double> presetValues =
                    new OrderedDictionary<string, double>("Preset Transparency Values", StringComparer.OrdinalIgnoreCase);
                presetValues.Add("Semi-transparent", 128);
                presetValues.Add("Opaque", 255);
                string desc = "Enter the transparency between 0 and 255.\n" + "(0 - transparent; 255 - opaque)";
                frmGetValue.PrepareForm("Set Transparency: " + partNames.ToShortString(), "Transparency", desc, 128, presetValues);
                if (frmGetValue.ShowDialog() == DialogResult.OK)
                {
                    _controller.SetTransparencyForResultParts(partNames, (byte)frmGetValue.Value);
                }
                SaveFormLoaction(frmGetValue);
            }
        }
        private void ColorContoursOffResultPart(string[] partNames)
        {
            _controller.SetResultPartsColorContoursVisibility(partNames, false);
        }
        private void ColorContoursOnResultPart(string[] partNames)
        {
            _controller.SetResultPartsColorContoursVisibility(partNames, true);
        }
        private void DeleteResultParts(string[] partNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected parts?" + Environment.NewLine + 
                                                 partNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveResultParts(partNames);
            }
        }

        #endregion  ################################################################################################################

        #region Result field output  ###############################################################################################
        private void tsmiCreateResultFieldOutput_Click(object sender, EventArgs e)
        {
            try
            {
                CreateResultFieldOutput();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiEditResultFieldOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectOneEntity("Field Outputs", _controller.GetResultFieldOutputs(), EditResultFieldOutput);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteResultFieldOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("Field Outputs", _controller.GetVisibleResultFieldOutputsAsNamedItems(),
                                       DeleteResultFieldOutputs);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void CreateResultFieldOutput()
        {
            if (_controller.CurrentResult == null || _controller.CurrentResult.Mesh == null) return;
            if (_controller.CurrentResult.GetAllFiledNameComponentNames().Count == 0) return;
            //
            ShowForm(_frmResultFieldOutput, "Create Field Output", null);
        }
        private void EditResultFieldOutput(string resultFieldOutputName)
        {
            ShowForm(_frmResultFieldOutput, "Edit Field Output", resultFieldOutputName);
        }
        public void DeleteResultFieldOutputs(string[] fieldOutputNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected field outputs?" + Environment.NewLine
                                                 + fieldOutputNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveResultFieldOutputs(fieldOutputNames);
            }
        }
        public void DeleteResultFieldOutputComponents(string fieldOutputName, string[] componentNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected components from field output " + fieldOutputName + "?"
                                                 + Environment.NewLine + componentNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveResultFieldOutputComponents(fieldOutputName, componentNames);
            }
        }

        #endregion  ################################################################################################################

        #region Result history output  #############################################################################################
        private void tsmiCreateResultHistoryOutput_Click(object sender, EventArgs e)
        {
            try
            {
                CreateResultHistoryOutput();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tsmiDeleteResultHistoryOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SelectMultipleEntities("History Outputs", _controller.GetResultHistoryOutputsAsNamedItems(),
                                       RemoveResultHistoryResultSets);
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void CreateResultHistoryOutput()
        {
            if (_controller.CurrentResult == null || _controller.CurrentResult.Mesh == null) return;
            if (_controller.CurrentResult.GetAllFiledNameComponentNames().Count == 0) return;
            // Data editor
            ItemSetDataEditor.SelectionForm = _frmSelectItemSet;            
            ItemSetDataEditor.ParentForm = _frmResultHistoryOutput;
            _frmSelectItemSet.SetOnlyGeometrySelection(false);
            ShowForm(_frmResultHistoryOutput, "Create History Output", null);
        }
        public void RemoveResultHistoryResultSets(string[] historyResultSetNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected history outputs?" + Environment.NewLine
                                                 + historyResultSetNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveResultHistoryResultSets(historyResultSetNames);
            }
        }
        public void DeleteResultHistoryResultFields(string historyResultSetName, string[] historyResultFieldNames)
        {
            if (MessageBoxes.ShowWarningQuestion("OK to delete selected fields from history output " + historyResultSetName + "?"
                                                 + Environment.NewLine + historyResultFieldNames.ToRows()) == DialogResult.OK)
            {
                _controller.RemoveResultHistoryResultFields(historyResultSetName, historyResultFieldNames);
            }
        }
        public void DeleteResultHistoryResultCompoments(NamedClass[] items)
        {
            Dictionary<string, List<string>> parentItemNames;
            Dictionary<string, Dictionary<string, List<string>>> parentParentItemNames =
                new Dictionary<string, Dictionary<string, List<string>>>();
            //
            foreach (var item in items)
            {
                if (item is CaeResults.HistoryResultData hrd)
                {
                    if (parentParentItemNames.TryGetValue(hrd.SetName, out parentItemNames))
                    {
                        parentItemNames[hrd.FieldName].Add(hrd.ComponentName);
                    }
                    else
                    {
                        parentParentItemNames.Add(hrd.SetName, new Dictionary<string, List<string>>()
                                                  { { hrd.FieldName, new List<string>() { hrd.ComponentName} } });
                    }
                }
            }
            //
            string[] itemNames;
            foreach (var parentParentEntry in parentParentItemNames)
            {
                foreach (var parentEntry in parentParentEntry.Value)
                {
                    itemNames = parentEntry.Value.ToArray();
                    if (MessageBoxes.ShowWarningQuestion("OK to delete selected components from history field " +
                                                         parentEntry.Key + "?" + Environment.NewLine +
                                                         itemNames.ToRows()) == DialogResult.OK)
                    {
                        _controller.RemoveResultHistoryResultCompoments(parentParentEntry.Key,
                                                                        parentEntry.Key,
                                                                        itemNames);
                    }
                }
            }
        }
        #endregion  ################################################################################################################
       

        #region Help menu  #########################################################################################################
        private void tsmiHomePage_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Globals.HomePage);
        }
        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            FrmSplash frmSplash = new FrmSplash();
            frmSplash.ShowHelp = true;
            frmSplash.ShowDialog();
        }
        #endregion  ################################################################################################################

        #region Selection methods  #################################################################################################
        private void SelectOneEntity(string title, NamedClass[] entities, Action<string> OperateOnEntity)
        {
            if (entities == null || entities.Length == 0) return;
            // Only one entity exists
            if (entities.Length == 1)
            {
                OperateOnEntity(entities[0].Name);
            }
            // Multiple entities exists
            else
            {
                string[] preSelectedEntityNames = _modelTree.IntersectSelectionWithList(entities);
                //
                SetFormLoaction(_frmSelectEntity);
                _frmSelectEntity.PrepareForm(title, false, entities, preSelectedEntityNames, null);
                _frmSelectEntity.OneEntitySelected = OperateOnEntity;
                _frmSelectEntity.Show();
            }
        }
        private void SelectOneEntityInStep(string title, NamedClass[] entities, string stepName,
                                           Action<string, string> OperateOnEntityInStep)
        {
            if (entities == null || entities.Length == 0) return;
            // Only one entity exists
            if (entities.Length == 1)
            {
                OperateOnEntityInStep(stepName, entities[0].Name);
            }
            // Multiple entities exists
            else
            {
                string[] preSelectedEntityNames = _modelTree.IntersectSelectionWithList(entities);
                //
                SetFormLoaction(_frmSelectEntity);
                _frmSelectEntity.PrepareForm(title, false, entities, preSelectedEntityNames, stepName);
                _frmSelectEntity.OneEntitySelectedInStep = OperateOnEntityInStep;
                _frmSelectEntity.Show();
            }
        }
        private void SelectMultipleEntities(string title, NamedClass[] entities, Action<string[]> OperateOnMultpleEntities,
                                            int minNumberOfEntities = 1, int maxNumberOfEntities = int.MaxValue)
        {
            if (entities == null || entities.Length == 0 || entities.Length < minNumberOfEntities) return;
            // Only one entity exists
            if (entities.Length == 1)
            {
                if (minNumberOfEntities == 1) OperateOnMultpleEntities(entities.GetNames());
            }
            // Multiple entities exists
            else
            {
                string[] preSelectedEntityNames = _modelTree.IntersectSelectionWithList(entities);
                //
                SetFormLoaction(_frmSelectEntity);
                _frmSelectEntity.PrepareForm(title, true, entities, preSelectedEntityNames, null);
                _frmSelectEntity.MultipleEntitiesSelected = OperateOnMultpleEntities;
                _frmSelectEntity.MinNumberOfEntities = minNumberOfEntities;
                _frmSelectEntity.MaxNumberOfEntities = maxNumberOfEntities;
                _frmSelectEntity.Show();
            }
        }
        private void SelectMultipleEntitiesInStep(string title, NamedClass[] entities, string stepName,
                                                  Action<string, string[]> OperateOnMultpleEntitiesInStep)
        {
            if (entities == null || entities.Length == 0) return;
            // Only one entity exists
            if (entities.Length == 1)
            {
                OperateOnMultpleEntitiesInStep(stepName, entities.GetNames());
            }
            // Multiple entities exists
            else
            {
                string[] preSelectedEntityNames = _modelTree.IntersectSelectionWithList(entities);
                //
                _frmSelectEntity.Location = new Point(Left + _formLocation.X, Top + _formLocation.Y);
                _frmSelectEntity.PrepareForm(title, true, entities, preSelectedEntityNames, stepName);
                _frmSelectEntity.MultipleEntitiesSelectedInStep = OperateOnMultpleEntitiesInStep;
                _frmSelectEntity.Show();
            }
        }

        #endregion  ################################################################################################################

        #region Mouse selection methods  ###########################################################################################
        public void SelectPointOrArea(double[] pickedPoint, double[] selectionDirection, double[][] planeParameters,
                                      vtkSelectOperation selectOperation, string[] pickedPartNames)
        {
            _controller.SelectPointOrArea(pickedPoint, selectionDirection, planeParameters, selectOperation, pickedPartNames);
            //
            int[] ids = _controller.GetSelectionIds();
            // Must be here since it calls Clear which calls SelectionChanged
            if (_frmSectionView != null && _frmSectionView.Visible) _frmSectionView.PickedIds(ids);
            if (_frmTranslate != null && _frmTranslate.Visible) _frmTranslate.PickedIds(ids);
            if (_frmScale != null && _frmScale.Visible) _frmScale.PickedIds(ids);
            if (_frmRotate != null && _frmRotate.Visible) _frmRotate.PickedIds(ids);
            if (_frmReferencePoint != null && _frmReferencePoint.Visible) _frmReferencePoint.PickedIds(ids);
            if (_frmQuery != null && _frmQuery.Visible) _frmQuery.PickedIds(ids);
            if (_frmTransformation != null && _frmTransformation.Visible) _frmTransformation.PickedIds(ids);
            //
            SelectionChanged(ids);
        }
        public void SelectionChanged(int[] ids = null)
        {
            if (ids == null) ids = _controller.GetSelectionIds();
            //
            if (_frmMeshingParameters != null && _frmMeshingParameters.Visible) _frmMeshingParameters.SelectionChanged(ids);
            if (_frmMeshRefinement != null && _frmMeshRefinement.Visible) _frmMeshRefinement.SelectionChanged(ids);
            if (_frmSelectGeometry != null && _frmSelectGeometry.Visible) _frmSelectGeometry.SelectionChanged(ids);
            //
            if (_frmBoundaryLayer != null && _frmBoundaryLayer.Visible) _frmBoundaryLayer.SelectionChanged(ids);
            if (_frmRemeshingParameters != null && _frmRemeshingParameters.Visible) _frmRemeshingParameters.SelectionChanged(ids);            
            if (_frmNodeSet != null && _frmNodeSet.Visible) _frmNodeSet.SelectionChanged(ids);
            if (_frmElementSet != null && _frmElementSet.Visible) _frmElementSet.SelectionChanged(ids);
            if (_frmSurface != null && _frmSurface.Visible) _frmSurface.SelectionChanged(ids);
            if (_frmSection != null && _frmSection.Visible) _frmSection.SelectionChanged(ids);
            if (_frmConstraint != null && _frmConstraint.Visible) _frmConstraint.SelectionChanged(ids);
            if (_frmContactPair != null && _frmContactPair.Visible) _frmContactPair.SelectionChanged(ids);
            if (_frmInitialCondition != null && _frmInitialCondition.Visible) _frmInitialCondition.SelectionChanged(ids);
            //
            if (_frmHistoryOutput != null && _frmHistoryOutput.Visible) _frmHistoryOutput.SelectionChanged(ids);
            if (_frmBoundaryCondition != null && _frmBoundaryCondition.Visible) _frmBoundaryCondition.SelectionChanged(ids);
            if (_frmLoad != null && _frmLoad.Visible) _frmLoad.SelectionChanged(ids);
            if (_frmDefinedField != null && _frmDefinedField.Visible) _frmDefinedField.SelectionChanged(ids);
            //
            if (_frmResultHistoryOutput != null && _frmResultHistoryOutput.Visible) _frmResultHistoryOutput.SelectionChanged(ids);
        }
        public void SetSelectBy(vtkSelectBy selectBy)
        {
            InvokeIfRequired(() => _vtk.SelectBy = selectBy);
        }
        public void SetSelectItem(vtkSelectItem selectItem)
        {
            InvokeIfRequired(() => _vtk.SelectItem = selectItem);
        }
        public void GetGeometryPickProperties(double[] point, out int elementId, out int[] edgeNodeIds,
                                              out int[] cellFaceNodeIds, string[] selectionPartNames = null)
        {
            elementId = -1;
            edgeNodeIds = null;
            cellFaceNodeIds = null;
            try
            {
                if (selectionPartNames != null) _vtk.SetSelectableActorsFilter(selectionPartNames);
                _vtk.GetGeometryPickProperties(point, out elementId, out edgeNodeIds, out cellFaceNodeIds);
            }
            catch { }
            finally
            {
                if (selectionPartNames != null) _vtk.SetSelectableActorsFilter(null);
            }
        }
        public double GetSelectionPrecision()
        {
            return _vtk.GetSelectionPrecision();
        }
        public void GetPointAndCellIdsInsideFrustum(double[][] planeParameters, string[] selectionPartNames,
                                                    out int[] pointIds, out int[] cellIds)
        {
            cellIds = null;
            pointIds = null;
            try
            {
                if (selectionPartNames != null) _vtk.SetSelectableActorsFilter(selectionPartNames);
                _vtk.GetPointAndCellIdsInsideFrustum(planeParameters, out pointIds, out cellIds);
            }
            catch { }
            finally
            {
                if (selectionPartNames != null) _vtk.SetSelectableActorsFilter(null);
            }
        }

        #endregion  ################################################################################################################

        private void AddFormToAllForms(Form form)
        {
            form.StartPosition = FormStartPosition.Manual;
            form.Icon = Icon;
            form.Owner = this;
            form.Move += itemForm_Move;
            form.VisibleChanged += itemForm_VisibleChanged;
            _allForms.Add(form);
        }
        private void ShowForm(IFormBase form, string text, string itemToEditName)
        {
            ShowForm(form, text, null, itemToEditName);
        }
        private void ShowForm(IFormBase form, string text, string stepName, string itemToEditName)
        {
            if (!form.Visible)
            {
                CloseAllForms();
                SetFormLoaction((Form)form);
                form.Text = text;
                if (itemToEditName != null) form.Text += ": " + itemToEditName;
                if (form.PrepareForm(stepName, itemToEditName)) form.Show();
                else
                {
                    if (stepName != null)
                    {
                        string itemName = text.Replace("Create ", "").Replace("Edit ", "").ToLower();
                        MessageBoxes.ShowWarning("Creating/editing of a " + itemName + " in the step: "
                                                 + stepName + " is not supported.");
                    }
                }
            }
        }
        private void SetFormLoaction(Form form)
        {
            Rectangle screenBounds;
            bool intersects = false;
            Rectangle formSize = form.ClientRectangle;
            Point location = new Point(Left + _formLocation.X, Top + _formLocation.Y);
            Rectangle locationRect = new Rectangle(location, new Size(1, 1));
            //
            foreach (var screen in Screen.AllScreens)
            {
                screenBounds = screen.Bounds;
                if (screenBounds.IntersectsWith(locationRect))
                {
                    intersects = true;
                    // Size
                    if (formSize.Width > screenBounds.Width) formSize.Width = screenBounds.Width;
                    if (formSize.Height > screenBounds.Height) formSize.Height = screenBounds.Height;
                    // Location X
                    if (location.X < screenBounds.Left) location.X = screenBounds.Left;
                    else if (location.X + formSize.Width > screenBounds.Left + screenBounds.Width)
                        location.X = screenBounds.Left + screenBounds.Width - formSize.Width;
                    // Location Y
                    if (location.Y < screenBounds.Top) location.Y = screenBounds.Top;
                    else if (location.Y + formSize.Height > screenBounds.Top + screenBounds.Height)
                        location.Y = screenBounds.Top + screenBounds.Height - formSize.Height;
                }
            }
            //
            if (!intersects) location = Location;
            //
            form.Location = location;
        }
        private void SaveFormLoaction(Form form)
        {
            _formLocation.X = form.Location.X - Left;
            _formLocation.Y = form.Location.Y - Top;
        }
        public void CloseAllForms()
        {
            InvokeIfRequired(() =>
            {
                if (_allForms != null)
                {
                    foreach (var form in _allForms)
                    {
                        if (form.Visible) form.Hide();
                    }
                }
           });
        }

        // Toolbars                                                                                                                 
        #region File toolbar #######################################################################################################
        private void tsbNew_Click(object sender, EventArgs e)
        {
            tsmiNew_Click(null, null);
        }
        
        private void tsbOpen_Click(object sender, EventArgs e)
        {
            tsmiOpen_Click(null, null);
        }
        private void tsbImport_Click(object sender, EventArgs e)
        {
            tsmiImportFile_Click(null, null);
        }
        private void tsbSave_Click(object sender, EventArgs e)
        {
            tsmiSave_Click(null, null);
        }
        #endregion  ################################################################################################################

        #region View toolbar  ######################################################################################################
        private void tsbFrontView_Click(object sender, EventArgs e)
        {
            tsmiFrontView_Click(null, null);
        }
        private void tsbBackView_Click(object sender, EventArgs e)
        {
            tsmiBackView_Click(null, null);
        }
        private void tsbTopView_Click(object sender, EventArgs e)
        {
            tsmiTopView_Click(null, null);
        }
        private void tsbBottomView_Click(object sender, EventArgs e)
        {
            tsmiBottomView_Click(null, null);
        }
        private void tsbLeftView_Click(object sender, EventArgs e)
        {
            tsmiLeftView_Click(null, null);
        }
        private void tsbRightView_Click(object sender, EventArgs e)
        {
            tsmiRightView_Click(null, null);
        }
        //
        private void tsbNormalView_Click(object sender, EventArgs e)
        {
            tsmiNormalView_Click(null, null);
        }
        private void tsbVerticalView_Click(object sender, EventArgs e)
        {
            tsmiVerticalView_Click(null, null);
        }
        //
        private void tsbIsometric_Click(object sender, EventArgs e)
        {
            tsmiIsometricView_Click(null, null);
        }
        //
        private void tsbZoomToFit_Click(object sender, EventArgs e)
        {
            tsmiZoomToFit_Click(null, null);
        }
        //
        private void tsbShowWireframeEdges_Click(object sender, EventArgs e)
        {
            tsmiShowWireframeEdges_Click(null, null);
        }
        private void tsbShowElementEdges_Click(object sender, EventArgs e)
        {
            tsmiShowElementEdges_Click(null, null);
        }
        private void tsbShowModelEdges_Click(object sender, EventArgs e)
        {
            tsmiShowModelEdges_Click(null, null);
        }
        private void tsbShowNoEdges_Click(object sender, EventArgs e)
        {
            tsmiShowNoEdges_Click(null, null);
        }
        //
        private void tsbSectionView_Click(object sender, EventArgs e)
        {
            tsmiSectionView_Click(null, null);
        }
        private void tsbExplodedView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) TurnExplodedViewOnOff(true);
            else if (e.Button == MouseButtons.Right) tsmiExplodedView_Click(null, null);
        }
        private void tsbExplodedView_DoubleClick(object sender, EventArgs e)
        {
            tsmiExplodedView_Click(null, null);
        }
        //
        private void tsbHideAllParts_Click(object sender, EventArgs e)
        {
            tsmiHideAllParts_Click(sender, e);
        }
        private void tsbShowAllParts_Click(object sender, EventArgs e)
        {
            tsmiShowAllParts_Click(sender, e);
        }
        private void tsbInvertVisibleParts_Click(object sender, EventArgs e)
        {
            tsmiInvertVisibleParts_Click(sender, e);
        }
        //
        private void tsbQuery_Click(object sender, EventArgs e)
        {
            tsmiQuery_Click(sender, e);
        }
        private void tsbRemoveAnnotations_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Annotations.GetCurrentAnnotationNames().Length > 0)
                {
                    if (MessageBoxes.ShowWarningQuestion("OK to delete current view annotations?") == DialogResult.OK)
                    {
                        _controller.Annotations.RemoveCurrentArrowAnnotations();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void tscbSymbolsForStep_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // If BC or load from one step-1 is selected its selection requires the step-1 to be selected.
            // Changing og the step symols is not possible -> Clear selection
            _controller.ClearAllSelection();
            //
            _controller.DrawSymbolsForStep(tscbSymbolsForStep.SelectedItem.ToString(), false);
        }
        public void UpadteSymbolsForStepList()
        {
            InvokeIfRequired(() =>
            {
                ClearSymbolsDropDown();
                tscbSymbolsForStep.Items.AddRange(_controller.GetStepNames());
                tscbSymbolsForStep.SelectedIndex = tscbSymbolsForStep.Items.Count - 1;
            });
        }
        public void UpadteOneStepInSymbolsForStepList(string oldStepName, string newStepName)
        {
            InvokeIfRequired(() =>
            {
                int index = -1;
                for (int i = 0; i < tscbSymbolsForStep.Items.Count; i++)
                {
                    if (tscbSymbolsForStep.Items[i].ToString() == oldStepName)
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                {
                    tscbSymbolsForStep.Items[index] = newStepName;
                }
            });
        }
        public void RemoveOneStepInSymbolsForStepList(string stepName)
        {
            InvokeIfRequired(() =>
            {
                int selectedIndex = tscbSymbolsForStep.SelectedIndex;
                //
                int index = -1;
                for (int i = 0; i < tscbSymbolsForStep.Items.Count; i++)
                {
                    if (tscbSymbolsForStep.Items[i].ToString() == stepName)
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                {
                    tscbSymbolsForStep.Items.RemoveAt(index);
                    if (index == selectedIndex) tscbSymbolsForStep.SelectedIndex = tscbSymbolsForStep.Items.Count - 1;
                }
            });
        }
        public void SelectOneStepInSymbolsForStepList(string stepName)
        {
            InvokeIfRequired(() =>
            {
                int index = -1;
                for (int i = 0; i < tscbSymbolsForStep.Items.Count; i++)
                {
                    if (tscbSymbolsForStep.Items[i].ToString() == stepName)
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                {
                    tscbSymbolsForStep.SelectedIndexChanged -= tscbSymbolsForStep_SelectedIndexChanged;
                    tscbSymbolsForStep.SelectedIndex = index;
                    tscbSymbolsForStep.SelectedIndexChanged += tscbSymbolsForStep_SelectedIndexChanged;
                    //
                    _controller.DrawSymbolsForStep(tscbSymbolsForStep.SelectedItem.ToString(), false); // do not highlight!
                }
            });
        }

        #endregion  ################################################################################################################

        #region Deformation toolbar  ###############################################################################################
        private void tscbResultNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ResizeResultNamesComboBox();    // must be here
                //
                string currentResultName = _controller.AllResults.GetCurrentResultName();
                string newResultName = tscbResultNames.SelectedItem.ToString();
                if (newResultName != currentResultName)
                {
                    SetResult(newResultName);
                    UpdateComplexControlStates();
                }
                this.ActiveControl = null;
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }      
        private void tscbDeformationVariable_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _controller.Redraw();
                this.ActiveControl = null;
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tscbDeformationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Enable scale factor text box if needed
                UpdateScaleFactorTextBoxState();
                //
                _controller.Redraw();
                this.ActiveControl = null;
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tstbDeformationFactor_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    _controller.Redraw();
                    this.ActiveControl = null;
                    // No beep
                    e.SuppressKeyPress = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tstbDeformationFactor_EnabledChanged(object sender, EventArgs e)
        {
            if (tstbDeformationFactor.Enabled) UpdateScaleFactorTextBoxState();
        }
        // Complex
        private void tscbComplex_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Enable angle text box if needed
                UpdateAngleTextBoxState();
                //
                _controller.Redraw();
                this.ActiveControl = null;
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void tstbAngle_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    tstbAngle.Text = Tools.GetPhase360(tstbAngle.Value).ToString();
                    //
                    _controller.Redraw();
                    this.ActiveControl = null;
                    // No beep
                    e.SuppressKeyPress = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        public void SetResultNames()
        {
            InvokeIfRequired(() =>
            {
                tscbResultNames.Items.Clear();
                string[] allResultNames = _controller.AllResults.GetResultNames();
                if (allResultNames != null && allResultNames.Length > 0)
                {
                    foreach (var name in allResultNames) tscbResultNames.Items.Add(name);
                    // Drop down width
                    int maxWidth = GetMaxStringWidth(allResultNames, tscbResultNames.Font);
                    tscbResultNames.DropDownWidth = maxWidth;
                    //
                    string currentResultName = _controller.AllResults.GetCurrentResultName();
                    if (currentResultName != null) tscbResultNames.SelectedItem = currentResultName;
                }
            });
        }
        public void SetResult(string resultName)
        {
            // Clear
            Clear3D();
            // Set results
            _controller.AllResults.SetCurrentResult(resultName);
            // Regenerate tree
            RegenerateTree();
            // Get first component of the first field for the last increment in the last step
            if (_controller.ResultsInitialized) _controller.CurrentFieldData =
                    _controller.AllResults.CurrentResult.GetFirstComponentOfTheFirstFieldAtDefaultIncrement();
            //
            if (_controller.CurrentResult != null && _controller.CurrentResult.Mesh != null)
            {
                // Reset the previous step and increment
                SetAllStepAndIncrementIds();
                // Set last increment
                SetDefaultStepAndIncrementIds();
                // Show the selection in the results tree
                SelectFirstComponentOfFirstFieldOutput();
                //
                _controller.ViewResultsType = ViewResultsTypeEnum.ColorContours;  // Draw
                //
                SetMenuAndToolStripVisibility();
                //tsmiZoomToFit_Click(null, null);    // different results have different views
                SetCurrentEdgesVisibilities(_controller.CurrentEdgesVisibility);
            }
        }
        private void ResizeResultNamesComboBox()
        {
            string[] allResultNames = new string[] { tscbResultNames.SelectedItem.ToString() };
            int currentWidth = GetMaxStringWidth(allResultNames, tscbResultNames.Font);
            // Control width
            currentWidth += 20; // to account for the drop down arrow
            if (currentWidth < 125) currentWidth = 125;
            else if (currentWidth > 400) currentWidth = 400;
            tscbResultNames.Size = new Size(currentWidth, tscbResultNames.Height);
            //
            Application.DoEvents();
        }
        private int GetMaxStringWidth(IEnumerable<string> items, Font font)
        {
            int maxWidth = 0;
            using (Graphics graphics = CreateGraphics())
            {
                foreach (string item in items)
                {
                    SizeF area = graphics.MeasureString(item, font);
                    maxWidth = Math.Max((int)area.Width, maxWidth);
                }
            }
            return maxWidth;
        }
        private void InitializeDeformationComboBoxes()
        {
            tscbDeformationVariable.Items.Clear();
            string[] variableNames = FeResults.GetPossibleDeformationFieldOutputNames();
            tscbDeformationVariable.Items.AddRange(variableNames);
            tscbDeformationVariable.SelectedIndex = 0;  // Displacements
            //
            tscbDeformationType.Items.Clear();
            Type type = typeof(DeformationScaleFactorTypeEnum);
            string[] typeNames = Enum.GetNames(type);
            for (int i = 0; i < typeNames.Length; i++)
            {
                typeNames[i] = ((DeformationScaleFactorTypeEnum)Enum.Parse(type, typeNames[i])).GetDisplayedName();
            }
            tscbDeformationType.Items.AddRange(typeNames);
            tscbDeformationType.SelectedIndex = 2;      // Automatic

            //if (controller.Results != null)
            //    vps.PopulateDropDownList(controller.Results.GetExistingDeformationFieldOutputNames());
            //else
            //    vps.PopulateDropDownList(CaeResults.FeResults.GetPossibleDeformationFieldOutputNames());
        }
        private void UpdateScaleFactorTextBoxState()
        {
            tslDeformationFactor.Enabled = GetDeformationType() == DeformationScaleFactorTypeEnum.UserDefined;
            tstbDeformationFactor.Enabled = tslDeformationFactor.Enabled;
        }
        public string GetDeformationVariable()
        {
            if (InvokeRequired) return (string)Invoke(new Func<string>(GetDeformationVariable));
            //
            return tscbDeformationVariable.SelectedItem.ToString();
        }
        public DeformationScaleFactorTypeEnum GetDeformationType()
        {
            // Invoke
            if (InvokeRequired)
                return (DeformationScaleFactorTypeEnum)Invoke(new Func<DeformationScaleFactorTypeEnum>(GetDeformationType));
            //
            string displayName = tscbDeformationType.SelectedItem.ToString();
            DeformationScaleFactorTypeEnum[] scaleFactorTypes =
                (DeformationScaleFactorTypeEnum[])Enum.GetValues(typeof(DeformationScaleFactorTypeEnum));
            //
            for (int i = 0; i < scaleFactorTypes.Length; i++)
            {
                if (displayName == scaleFactorTypes[i].GetDisplayedName()) return scaleFactorTypes[i];
            }
            //
            throw new NotSupportedException();
        }
        public float GetDeformationFactor()
        {
            return (float)tstbDeformationFactor.Value;
        }
        // Complex
        private void InitializeComplexComboBoxes()
        {
            tscbComplex.Items.Clear();
            //
            Type type = typeof(ComplexResultTypeEnum);
            string[] typeNames = Enum.GetNames(type);
            for (int i = 0; i < typeNames.Length; i++)
            {
                typeNames[i] = ((ComplexResultTypeEnum)Enum.Parse(type, typeNames[i])).GetDisplayedName();
            }
            tscbComplex.Items.AddRange(typeNames);
            tscbComplex.SelectedIndex = 0;  // Real
            //
            UpdateComplexControlStates();
        }
        private void UpdateComplexControlStates()
        {
            bool visible;
            if (_controller.ContainsComplexResults)
            {
                visible = true;
                //
                bool enabled = true;
                //if (_controller.CurrentFieldData != null)
                //{
                //    Field field = _controller.CurrentResult.GetField(_controller.CurrentFieldData);
                //    if (field != null) enabled = field.Complex;
                //    else enabled = false;
                //}
                //else enabled = false;
                //
                tslComplex.Enabled = enabled;
                tscbComplex.Enabled = enabled;
                if (enabled) UpdateAngleTextBoxState();
                else
                {
                    tslAngle.Enabled = false;
                    tstbAngle.Enabled = false;
                }
            }
            else
            {
                visible = false;
            }
            //
            SetComplexControlsVisibility(visible);
        }
        private void SetComplexControlsVisibility(bool visible)
        {
            tslComplex.Visible = visible;
            tscbComplex.Visible = visible;
            tslAngle.Visible = visible;
            tstbAngle.Visible = visible;
        }
        private void UpdateAngleTextBoxState()
        {
            bool enabled = GetComplexResultType() == ComplexResultTypeEnum.RealAtAngle;
            tslAngle.Enabled = enabled;
            tstbAngle.Enabled = enabled;
        }
        public ComplexResultTypeEnum GetComplexResultType()
        {
            // Invoke
            if (InvokeRequired)
                return (ComplexResultTypeEnum)Invoke(new Func<ComplexResultTypeEnum>(GetComplexResultType));
            //
            string displayName = tscbComplex.SelectedItem.ToString();
            ComplexResultTypeEnum[] complexResultTypes = (ComplexResultTypeEnum[])Enum.GetValues(typeof(ComplexResultTypeEnum));
            //
            for (int i = 0; i < complexResultTypes.Length; i++)
            {
                if (displayName == complexResultTypes[i].GetDisplayedName()) return complexResultTypes[i];
            }
            //
            throw new NotSupportedException();
        }
        public double GetComplexAngleDeg()
        {
            return tstbAngle.Value;
        }

        #endregion  ################################################################################################################

        #region Results field toolbar  #############################################################################################

        private void tsbResultsUndeformed_Click(object sender, EventArgs e)
        {
            tsmiResultsUndeformed_Click(null, null);
        }
        private void tsbResultsDeformed_Click(object sender, EventArgs e)
        {
            tsmiResultsDeformed_Click(null, null);
        }
        private void tsbResultsColorContours_Click(object sender, EventArgs e)
        {
            tsmiResultsColorContours_Click(null, null);
        }
        private void tsbResultsUndeformedWireframe_Click(object sender, EventArgs e)
        {
            tsmiResultsDeformedColorWireframe_Click(null, null);
        }
        private void tsbResultsUndeformedSolid_Click(object sender, EventArgs e)
        {
            tsmiResultsDeformedColorSolid_Click(null, null);
        }
        private void tsbTransformation_Click(object sender, EventArgs e)
        {
            tsmiTransformation_Click(null, null);
        }
        //
        private void FieldOutput_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                FieldData current = _controller.CurrentFieldData;
                SetFieldData(current.Name, current.Component, GetCurrentFieldOutputStepId(), GetCurrentFieldOutputStepIncrementId());
            }
            catch
            { }
        }
        private void tsbPreviousStepIncrement_Click(object sender, EventArgs e)
        {
            try
            {
                if (tscbStepAndIncrement.Enabled && tscbStepAndIncrement.SelectedIndex > 0) 
                    tscbStepAndIncrement.SelectedIndex--;
            }
            catch
            { }
        }
        private void tsbNextStepIncrement_Click(object sender, EventArgs e)
        {
            try
            {
                if (tscbStepAndIncrement.Enabled && tscbStepAndIncrement.SelectedIndex < tscbStepAndIncrement.Items.Count - 1) 
                    tscbStepAndIncrement.SelectedIndex++;
            }
            catch
            { }
        }
        private void tsbFirstStepIncrement_Click(object sender, EventArgs e)
        {
            try
            {
                if (tscbStepAndIncrement.Enabled) tscbStepAndIncrement.SelectedIndex = 0;
            }
            catch
            { }
        }
        private void tsbLastStepIncrement_Click(object sender, EventArgs e)
        {
            try
            {
                if (tscbStepAndIncrement.Enabled) tscbStepAndIncrement.SelectedIndex = tscbStepAndIncrement.Items.Count - 1;
            }
            catch
            { }
        }
        private void tsbAnimate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.ViewResultsType != ViewResultsTypeEnum.Undeformed &&
                    _controller.GetResultStepIDs().Length > 0 && !_frmAnimation.Visible)
                {
                    CloseAllForms();
                    DisableEnableControlsForAnimation(false);
                    SetFormLoaction(_frmAnimation);
                    _frmAnimation.PrepareForm(this, _controller);
                    if (_frmAnimation.DialogResult == DialogResult.Abort)
                        DisableEnableControlsForAnimation(true);
                    else
                        _frmAnimation.Show();
                }
            }
            catch
            { }
        }
        //
        private void DisableEnableControlsForAnimation(bool enable)
        {
            // _modelTree.DisableMouse = !enable; this is done in the itemForm_VisibleChanged
            menuStripMain.DisableMouseButtons = !enable;
            tsFile.DisableMouseButtons = !enable;
            //
            tsDeformationFactor.DisableMouseButtons = !enable;
            tscbDeformationVariable.Enabled = enable;   // must be here
            tscbDeformationType.Enabled = enable;       // must be here
            tstbDeformationFactor.Enabled = enable;     // must be here
            //
            if (enable) UpdateComplexControlStates();
            else
            {
                tslComplex.Enabled = false;
                tscbComplex.Enabled = false;
                tslAngle.Enabled = false;
                tstbAngle.Enabled = false;
            }
            //
            tsResults.DisableMouseButtons = !enable;
            tscbStepAndIncrement.Enabled = enable;      // must be here despite the tsResults.DisableMouseButtons = !enable;
            //
            tsbShowAllParts.Enabled = enable;
            tsbHideAllParts.Enabled = enable;
            tsbInvertVisibleParts.Enabled = enable;
        }


        #endregion  ################################################################################################################

        #region Status strip  ######################################################################################################
        public void SetState(string text, bool working)
        {
            InvokeIfRequired(() =>
            {
                tsslState.Text = text;
                //
                if (working) tspbProgress.Style = ProgressBarStyle.Marquee;
                else tspbProgress.Style = ProgressBarStyle.Blocks;
                // Rendering
                if (text == Globals.ExplodePartsText) _vtk.RenderingOn = true;
                else _vtk.RenderingOn = !working;
                //
                _vtk.Enabled = !working;
                //
                _modelTree.DisableMouse = working;
                menuStripMain.DisableMouseButtons = working;
                tsFile.DisableMouseButtons = working;
                tsViews.DisableMouseButtons = working;
                tsDeformationFactor.DisableMouseButtons = working;
                tsResults.DisableMouseButtons = working;
                //
                //this.DisableAllMouseEvents = working;
                tspbProgress.Visible = working;
            });
        }
        public void SetStateReady(string currentText)
        {
            if (tsslState.Text == currentText) // check that the same command is being canceled
            {
                SetState(Globals.ReadyText, false);
                tsslCancel.Visible = false;
            }
        }
        public bool SetStateWorking(string text, bool showCancelButton = false)
        {
            if (tsslState.Text == Globals.ReadyText) // check that the state is ready
            {
                SetState(text, true);
                tsslCancel.Visible = showCancelButton;
                return true;
            }
            return false;
        }
        private bool IsStateWorking()
        {
            return tsslState.Text != Globals.ReadyText;
        }
        public bool IsStateOpening()
        {
            return tsslState.Text == Globals.OpeningText;
        }

        private void tsslCancel_MouseDown(object sender, MouseEventArgs e)
        {
            tsslCancel.BorderStyle = Border3DStyle.Sunken;
        }
        private void tsslCancel_MouseUp(object sender, MouseEventArgs e)
        {
            tsslCancel.BorderStyle = Border3DStyle.Raised;
        }
        private void tsslCancel_MouseLeave(object sender, EventArgs e)
        {
            tsslCancel.BorderStyle = Border3DStyle.Raised;
        }
        private void tsslCancel_Click(object sender, EventArgs e)
        {
            _controller.StopNetGenJob();
        }
        #endregion  ################################################################################################################


        // Methods                                                                                                                  
        #region Methods
        public void SetTitle(string title)
        {
            InvokeIfRequired(() => this.Text = title);
        }
        private bool CheckValiditiy()
        {
            string[] invalidItems = _controller.CheckAndUpdateModelValidity();
            if (invalidItems.Length > 0)
            {
                string text = "The model contains active invlaid items:" + Environment.NewLine;
                foreach (var item in invalidItems) text += Environment.NewLine + item;
                text += Environment.NewLine + Environment.NewLine + "Continue?";
                return MessageBoxes.ShowWarningQuestion(text) == DialogResult.OK;
            }
            return true;
        }

        public double[] GetBoundingBox()
        {
            // xmin, xmax, ymin, ymax, zmin, zmax
            return _vtk.GetBoundingBox();
        }
        public double[] GetBondingBoxSize()
        {
            return _vtk.GetBoundingBoxSize();
        }
        public string GetFileNameToImport(bool onlyMaterials)
        {
            return GetFileNameToImport(GetFileImportFilter(onlyMaterials));
        }
        public string GetFileNameToImport(string filter)
        {
            string fileName = null;
            InvokeIfRequired(() =>
            {
                openFileDialog.Filter = filter;
                openFileDialog.FileName = "";
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;
                }
            });
            return fileName;
        }
        public string GetFileNameToSaveAs()
        {
            string fileName = null;
            InvokeIfRequired(() =>
            {
                saveFileDialog.Filter = "PrePoMax files | *.pmx";
                //
                fileName = Path.GetFileName(_controller.OpenedFileName);
                saveFileDialog.FileName = fileName;
                //
                saveFileDialog.OverwritePrompt = true;
                //
                if (saveFileDialog.ShowDialog() == DialogResult.OK) fileName = saveFileDialog.FileName;
                else fileName = null;
            });
            return fileName;
        }
        public string[] GetFileNamesToImport(string filter)
        {
            string[] fileNames = null;
            InvokeIfRequired(() =>
            {
                // create new dialog to enable multiFilter
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Multiselect = true;
                    openFileDialog.Filter = filter;
                    openFileDialog.FileName = "";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        fileNames = openFileDialog.FileNames;
                    }
                }
            });
            return fileNames;
        }
        private string GetFileImportFilter(bool onlyMaterials)
        {
            if (onlyMaterials) return "Abaqus/Calculix inp files|*.inp";
            //
            string filter = "All supported files|*.stp;*.step;*.igs;*.iges;*.brep;*.stl;*.unv;*.vol;*.inp;*.mesh"
                            + "|Step files|*.stp;*.step"
                            + "|Iges files|*.igs;*.iges"
                            + "|Brep files|*.brep"
                            + "|Stereolitography files|*.stl"
                            + "|Universal files|*.unv"
                            + "|Netgen files|*.vol"
                            + "|Abaqus/Calculix inp files|*.inp"
                            + "|Mmg mesh files|*.mesh";
            return filter;
        }
        private HashSet<string> GetFileImportExtensions()
        {
            string[] tmp = GetFileImportFilter(false).Split(new string[] { "*", "\"", ";", "|" },
                                                            StringSplitOptions.RemoveEmptyEntries);
            HashSet<string> extensions = new HashSet<string>();
            foreach (var entry in tmp)
            {
                if (entry.StartsWith(".")) extensions.Add(entry);
            }
            return extensions;
        }

        #region Clear  #############################################################################################################
        public void ClearControls()
        {
            InvokeIfRequired(() =>
            {
                _vtk.Clear();
                _vtk.RemoveAllArrowWidgets();
                _modelTree.Clear();
                outputLines = new string[0];
                tbOutput.Text = "";
                ClearResults();
                ClearAnnotationStatus();
            });
        }
        private void ClearSymbolsDropDown()
        {
            tscbSymbolsForStep.Items.Clear();
            tscbSymbolsForStep.Items.Add("None");
            tscbSymbolsForStep.Items.Add("Model");
            tscbSymbolsForStep.SelectedIndex = 1;
        }
        public void ClearResults()
        {
            InvokeIfRequired(() =>
            {
                tscbResultNames.Items.Clear();
                tscbResultNames.Size = new Size(125, tscbResultNames.Height);
                tscbStepAndIncrement.Items.Clear();
                _modelTree.ClearResults();
                //
                SetMenuAndToolStripVisibility();
            });
        }
        public void Clear3D()
        {
            InvokeIfRequired(_vtk.Clear);
        }
        public void ClearButKeepParts(string[] partNames)
        {
            InvokeIfRequired(_vtk.ClearButKeepParts, partNames);
        }
        public void ClearSelection()
        {
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
        }
        public void Clear3DSelection()
        {
            InvokeIfRequired(() => _vtk.ClearSelection());
        }
        public void ClearActiveTreeSelection()
        {
            InvokeIfRequired(_modelTree.ClearActiveTreeSelection);
        }
        private void ClearAnnotationStatus()
        {
            tsmiAnnotateFaceOrientations.Checked = false;
            tsmiAnnotateParts.Checked = false;
            tsmiAnnotateMaterials.Checked = false;
            tsmiAnnotateSections.Checked = false;
            tsmiAnnotateSectionThicknesses.Checked = false;
            tsmiAnnotateReferencePoints.Checked = false;
            tsmiAnnotateConstraints.Checked = false;
            tsmiAnnotateContactPairs.Checked = false;
            tsmiAnnotateBCs.Checked = false;
            tsmiAnnotateLoads.Checked = false;
            tsmiAnnotateAllSymbols.Checked = false;
            //
            HideColorBar();
        }

        #endregion  ################################################################################################################

        #region vtkControl  ########################################################################################################
        // vtkControl
        public void SetFrontBackView(bool animate, bool front)
        {
            InvokeIfRequired(_vtk.SetFrontBackView, animate, front);
        }
        public void SetZoomToFit(bool animate)
        {
            InvokeIfRequired(_vtk.SetZoomToFit, animate);
        }
        public double[] GetViewPlaneNormal()
        {
            if (this.InvokeRequired)
            {
                return (double[])this.Invoke((MethodInvoker)delegate () { _vtk.GetViewPlaneNormal(); });
            }
            else
            {
                return _vtk.GetViewPlaneNormal();
            }
        }
        public void AdjustCameraDistanceAndClipping()
        {
            InvokeIfRequired(_vtk.AdjustCameraDistanceAndClipping);
        }
        public void UpdateScalarsAndRedraw()
        {
            InvokeIfRequired(_vtk.UpdateScalarsAndRedraw);
        }
        public void UpdateScalarsAndCameraAndRedraw()
        {
            InvokeIfRequired(_vtk.UpdateScalarsAndCameraAndRedraw);
        }
        // Section view
        public void ApplySectionView(double[] point, double[] normal)
        {
            InvokeIfRequired(_vtk.ApplySectionView, point, normal);
            InvokeIfRequired(() => { tsbSectionView.Checked = true; });
        }
        public void UpdateSectionView(double[] point, double[] normal)
        {
            InvokeIfRequired(_vtk.UpdateSectionView, point, normal);
            InvokeIfRequired(() => { tsbSectionView.Checked = true; });
        }
        public void RemoveSectionView()
        {
            InvokeIfRequired(_vtk.RemoveSectionView);
            InvokeIfRequired(() => { tsbSectionView.Checked = false; });
        }
        // Exploded view
        public void PreviewExplodedView(Dictionary<string, double[]> partOffsets, bool animate)
        {
            InvokeIfRequired(_vtk.PreviewExplodedView, partOffsets, animate);
        }
        public void RemovePreviewedExplodedView(string[] partNames)
        {
            InvokeIfRequired(_vtk.RemovePreviewedExplodedView, partNames);
        }
        public void SetExplodedViewStatus(bool status)
        {
            InvokeIfRequired(() => { tsbExplodedView.Checked = status; });
        }
        // Transformations
        public void AddSymmetry(int symmetryPlane, double[] symmetryPoint)
        {
            InvokeIfRequired(_vtk.AddSymmetry, symmetryPlane, symmetryPoint);
        }
        public void AddLinearPattern(double[] displacement, int numOfItems)
        {
            InvokeIfRequired(_vtk.AddLinearPattern, displacement, numOfItems);
        }
        public void AddCircularPattern(double[] axisPoint, double[] axisNormal, double angle, int numOfItems)
        {
            InvokeIfRequired(_vtk.AddCircularPattern, axisPoint, axisNormal, angle, numOfItems);
        }
        public void ApplyTransformations()
        {
            InvokeIfRequired(_vtk.ApplyTransforms);
        }
        public void SetTransformationsStatus(bool status)
        {
            InvokeIfRequired(() => { tsbTransformation.Checked = status; });
        }
        //
        public void Add3DNodes(vtkControl.vtkMaxActorData actorData)
        {
            InvokeIfRequired(_vtk.AddPoints, actorData);
        }
        public void Add3DCells(vtkControl.vtkMaxActorData cellData)
        {
            InvokeIfRequired(_vtk.AddCells, cellData);
        }
        public void AddScalarFieldOn3DCells(vtkControl.vtkMaxActorData actorData, bool update)
        {
            InvokeIfRequired(_vtk.AddScalarFieldOnCells, actorData, update);
        }
        public bool AddAnimatedScalarFieldOn3DCells(vtkControl.vtkMaxActorData actorData)
        {
            return _vtk.AddAnimatedScalarFieldOnCells(actorData);
        }
        public void UpdateActorSurfaceScalarField(string actorName, float[] values, NodesExchangeData extremeNodes,
                                                  float[] frustumCellLocatorValues, bool update)
        {
            InvokeIfRequired(_vtk.UpdateActorScalarField, actorName, values, extremeNodes, frustumCellLocatorValues, update);
        }
        public void UpdateActorColorContoursVisibility(string[] actorNames, bool colorContour)
        {
            InvokeIfRequired(_vtk.UpdateActorsColorContoursVisibility, actorNames, colorContour);
        }
        public void AddSphereActor(vtkControl.vtkMaxActorData actorData, double symbolSize)
        {
            InvokeIfRequired(_vtk.AddSphereActor, actorData, symbolSize);
        }
        public void AddOrientedDisplacementConstraintActor(vtkControl.vtkMaxActorData actorData, double symbolSize)
        {
            InvokeIfRequired(_vtk.AddOrientedDisplacementConstraintActor, actorData, symbolSize);
        }
        public void AddOrientedRotationalConstraintActor(vtkControl.vtkMaxActorData actorData, double symbolSize)
        {
            InvokeIfRequired(_vtk.AddOrientedRotationalConstraintActor, actorData, symbolSize);
        }
        public void AddOrientedArrowsActor(vtkControl.vtkMaxActorData actorData, double symbolSize, bool invert = false, 
                                           double relativeSize = 1)
        {
            InvokeIfRequired(_vtk.AddOrientedArrowsActor, actorData, symbolSize, invert, relativeSize);
        }
        public void AddOrientedDoubleArrowsActor(vtkControl.vtkMaxActorData actorData, double symbolSize)
        {
            InvokeIfRequired(_vtk.AddOrientedDoubleArrowsActor, actorData, symbolSize);
        }
        public void AddOrientedSpringActor(vtkControl.vtkMaxActorData actorData, double symbolSize, bool invert = false)
        {
            InvokeIfRequired(_vtk.AddOrientedSpringActor, actorData, symbolSize, invert);
        }
        public void AddOrientedThermosActor(vtkControl.vtkMaxActorData actorData, double symbolSize, bool invert = false)
        {
            InvokeIfRequired(_vtk.AddOrientedThermoActor, actorData, symbolSize, invert);
        }
        public void AddOrientedFluxActor(vtkControl.vtkMaxActorData actorData, double symbolSize, bool center, bool invert)
        {
            InvokeIfRequired(_vtk.AddOrientedFluxActor, actorData, symbolSize, center, invert);
        }
        //
        public bool ContainsActor(string actorName)
        {
            return _vtk.ContainsActor(actorName);
        }
        public void HighlightActor(string actorName)
        {
            InvokeIfRequired(_vtk.HighlightActor, actorName);
        }
        public void UpdateActor(string oldName, string newName, Color newColor)
        {
            InvokeIfRequired(_vtk.UpdateActor, oldName, newName, newColor);
        }
        public void HideActors(string[] actorNames, bool updateColorContours)
        {
            InvokeIfRequired(_vtk.HideActors, actorNames, updateColorContours);
        }
        public void ShowActors(string[] actorNames, bool updateColorContours)
        {
            InvokeIfRequired(_vtk.ShowActors, actorNames, updateColorContours);
        }
        // Settings                                             
        public void SetCoorSysVisibility(bool visibility)
        {
            InvokeIfRequired(_vtk.SetCoorSysVisibility, visibility);
        }
        // Scale bar
        public void SetScaleWidgetVisibility(bool visibility)
        {
            InvokeIfRequired(_vtk.SetScaleWidgetVisibility, visibility);
        }
        private void SetScaleWidgetUnit(UnitSystem unitSystem)
        {
            string unit = "";
            if (unitSystem.UnitSystemType != UnitSystemType.Undefined) unit = unitSystem.LengthUnitAbbreviation;
            //
            InvokeIfRequired(_vtk.SetScaleWidgetUnit, unit);
        }
        // Scalar bar
        public void InitializeResultWidgetPositions()
        {
            InvokeIfRequired(_vtk.InitializeResultWidgetPositions);
        }
        public void SetScalarBarColorSpectrum(vtkControl.vtkMaxColorSpectrum colorSpectrum)
        {
            InvokeIfRequired(_vtk.SetScalarBarColorSpectrum, colorSpectrum);
        }
        public void SetScalarBarNumberFormat(string numberFormat)
        {
            InvokeIfRequired(_vtk.SetScalarBarNumberFormat, numberFormat);
        }
        public void SetScalarBarText(string fieldName, string componentName, string unitAbbreviation, string complexComponent,
                                     string minMaxType)
        {
            InvokeIfRequired(_vtk.SetScalarBarText, fieldName, componentName, unitAbbreviation, complexComponent, minMaxType);
        }
        public void DrawLegendBackground(bool drawBackground)
        {
            InvokeIfRequired(_vtk.DrawScalarBarBackground, drawBackground);
        }
        public void DrawLegendBorder(bool drawBorder)
        {
            InvokeIfRequired(_vtk.DrawScalarBarBorder, drawBorder);
        }
        // Color bar
        public void InitializeColorBarWidgetPosition()
        {
            InvokeIfRequired(_vtk.InitializeColorBarWidgetPosition);
        }
        public void SetColorBarColorsAndLabels(Color[] colors, string[] labels)
        {
            InvokeIfRequired(_vtk.SetColorBarColorsAndLabels, colors, labels);
        }
        public void AddColorBarColorsAndLabels(Color[] colors, string[] labels)
        {
            InvokeIfRequired(_vtk.AddColorBarColorsAndLabels, colors, labels);
        }
        public void DrawColorBarBackground(bool drawBackground)
        {
            InvokeIfRequired(_vtk.DrawColorBarBackground, drawBackground);
        }
        public void DrawColorBarBorder(bool drawBorder)
        {
            InvokeIfRequired(_vtk.DrawColorBarBorder, drawBorder);
        }
        public void HideColorBar()
        {
            InvokeIfRequired(_vtk.HideColorBar);
        }
        // Status bar
        public void DrawStatusBlockBackground(bool drawBackground)
        {
            InvokeIfRequired(_vtk.DrawStatusBlockBackground, drawBackground);
        }
        public void DrawStatusBlockBorder(bool drawBorder)
        {
            InvokeIfRequired(_vtk.DrawStatusBlockBorder, drawBorder);
        }
        public void SetStatusBlock(string name, DateTime dateTime, float analysisTime, string unit, string deformationVariable,
                                   float scaleFactor, vtkControl.vtkMaxFieldDataType fieldType, int stepNumber, int incrementNumber)
        {
            InvokeIfRequired(_vtk.SetStatusBlock, name, dateTime, analysisTime, unit, deformationVariable, scaleFactor,
                             fieldType, stepNumber, incrementNumber);
        }
        // General
        public void SetBackground(bool gradient, Color topColor, Color bottomColor, bool redraw)
        {
            InvokeIfRequired(_vtk.SetBackground, gradient, topColor, bottomColor, redraw);
        }
        public void SetLighting(double ambient, double diffuse, bool redraw)
        {
            InvokeIfRequired(_vtk.SetLighting, ambient, diffuse, redraw);
        }
        public void SetSmoothing(bool pointSmoothing, bool lineSmoothing, bool redraw)
        {
            InvokeIfRequired(_vtk.SetSmoothing, pointSmoothing, lineSmoothing, redraw);
        }
        // Highlight
        public void SetHighlightColor(Color primaryHighlightColor, Color secondaryHighlightColor)
        {
            InvokeIfRequired(_vtk.SetHighlightColor, primaryHighlightColor, secondaryHighlightColor);
        }
        public void SetMouseHighlightColor(Color mousehighlightColor)
        {
            InvokeIfRequired(_vtk.SetMouseHighlightColor, mousehighlightColor);
        }
        // Symbols
        public void SetDrawSymbolEdges(bool drawSilhouettes)
        {
            InvokeIfRequired(_vtk.SetDrawSymbolEdges, drawSilhouettes);
        }
        //
        public void CropPartWithCylinder(string partName, double r, string fileName)
        {
            InvokeIfRequired(_vtk.CropPartWithCylinder, partName, r, fileName);
        }
        public void CropPartWithCube(string partName, double a, string fileName)
        {
            InvokeIfRequired(_vtk.CropPartWithCube, partName, a, fileName);
        }
        public void SmoothPart(string partName, double a, string fileName)
        {
            InvokeIfRequired(_vtk.SmoothPart, partName, a, fileName);
        }
        // User pick
        public void ActivateUserPick()
        {
            _vtk.UserPick = true;
        }
        public void DeactivateUserPick()
        {
            _vtk.UserPick = false;
        }
        #endregion  ################################################################################################################

        #region Results  ###########################################################################################################
        // Results

        public void SetFieldData(string name, string component, int stepId, int stepIncrementId)
        {
            FieldData fieldData = new FieldData(name, component, stepId, stepIncrementId);
            FieldData currentData = _controller.CurrentFieldData;
            // In case the currentData is null exit
            if (currentData == null) return;
            //
            if (!fieldData.Equals(currentData)) // update results only if field data changed
            {
                // Stop and update animation data only if field data changed
                if (_frmAnimation.Visible) _frmAnimation.Hide();
                //
                if (fieldData.Name == currentData.Name && fieldData.Component == currentData.Component)
                {
                    // Step id or increment id changed                                              

                    // Find the choosen data; also contains info about type of step ...
                    fieldData = _controller.CurrentResult.GetFieldData(fieldData.Name,
                                                                       fieldData.Component,
                                                                       fieldData.StepId,
                                                                       fieldData.StepIncrementId,
                                                                       true);
                    // Update controller field data
                    _controller.CurrentFieldData = fieldData;
                    // Draw deformation or field data
                    if (_controller.ViewResultsType != ViewResultsTypeEnum.Undeformed) _controller.DrawResults(false);
                }
                else
                {
                    // Field of field component changed                                                 

                    // Update controller field data; this is used for the SetStepAndIncrementIds to detect missing ids
                    _controller.CurrentFieldData = fieldData;
                    // Find the existing choosen data; also contains info about type of step ...
                    fieldData = _controller.CurrentResult.GetFieldData(fieldData.Name,
                                                                       fieldData.Component,
                                                                       fieldData.StepId,
                                                                       fieldData.StepIncrementId,
                                                                       true);
                    // Update controller field data
                    _controller.CurrentFieldData = fieldData;
                    // Draw field data
                    if (_controller.ViewResultsType == ViewResultsTypeEnum.ColorContours) _controller.UpdatePartsScalarFields();
                }
                //
                UpdateComplexControlStates();
                // Move focus from step and step increment dropdown menus
                this.ActiveControl = null;
            }
        }
        public void SetAllStepAndIncrementIds(bool reset = false)
        {
            InvokeIfRequired(() =>
            {
                // Save current step and increment id
                string currentStepIncrement = (string)tscbStepAndIncrement.SelectedItem;
                string[] prevStepIncrementIds = null;
                if (!reset && currentStepIncrement != null)
                {
                    prevStepIncrementIds = currentStepIncrement.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                }
                // Set all increments
                tscbStepAndIncrement.SelectedIndexChanged -= FieldOutput_SelectionChanged;  // detach event
                tscbStepAndIncrement.Items.Clear();
                Dictionary<int, int[]> allIds = _controller.CurrentResult.GetAllExistingIncrementIds();
                int lastStepId = 1;
                int lastIncrementId = 0;
                foreach (var entry in allIds)
                {
                    foreach (int incrementId in entry.Value)
                    {
                        tscbStepAndIncrement.Items.Add(entry.Key.ToString() + ", " + incrementId);
                        lastIncrementId = incrementId;
                    }
                    lastStepId = entry.Key;
                }
                tscbStepAndIncrement.SelectedIndexChanged += FieldOutput_SelectionChanged;  // reattach event
                // Reselect previous step and increment
                if (prevStepIncrementIds != null)
                {
                    int stepId = Math.Min(int.Parse(prevStepIncrementIds[0]), lastStepId);
                    int incrementId = Math.Min(int.Parse(prevStepIncrementIds[1]), lastIncrementId);
                    SetStepAndIncrementIds(stepId, incrementId);
                }
                else SetDefaultStepAndIncrementIds();
            });
        }
        public void SetStepAndIncrementIds(int stepId, int incrementId)
        {
            InvokeIfRequired(() =>
            {
                string stepIncrement = stepId + ", " + incrementId;
                // Set the combobox
                if (tscbStepAndIncrement.Items.Contains(stepIncrement))
                {
                    tscbStepAndIncrement.SelectedIndexChanged -= FieldOutput_SelectionChanged;
                    tscbStepAndIncrement.SelectedItem = stepIncrement;
                    // Set the step and increment if the combobox set was successful
                    CaeResults.FieldData data = _controller.CurrentFieldData;
                    data.StepId = stepId;
                    data.StepIncrementId = incrementId;
                    _controller.CurrentFieldData = data;   // to correctly update the increment time
                    tscbStepAndIncrement.SelectedIndexChanged += FieldOutput_SelectionChanged;
                }
                else SetDefaultStepAndIncrementIds();
            });
        }
        public void SetDefaultStepAndIncrementIds()
        {
            string[] tmp;
            FieldData fieldData = _controller.CurrentFieldData;
            if (fieldData.StepId == -1 && fieldData.StepIncrementId == -1) return;
            else SetStepAndIncrementIds(fieldData.StepId, fieldData.StepIncrementId);
            //
            return;
            //
            if (_controller.CurrentFieldData.StepType == CaeResults.StepTypeEnum.Frequency)
            {
                string firstStepIncrement = (string)tscbStepAndIncrement.Items[tscbStepAndIncrement.Items.Count - 1];
            }
            else
            {
                string lastStepIncrement = (string)tscbStepAndIncrement.Items[tscbStepAndIncrement.Items.Count - 1];
                tmp = lastStepIncrement.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                
            }
        }
        public int GetCurrentFieldOutputStepId()
        {
            string selectedStepIncrement = (string)tscbStepAndIncrement.SelectedItem;
            string[] tmp = selectedStepIncrement.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            return int.Parse(tmp[0]);
        }
        public int GetCurrentFieldOutputStepIncrementId()
        {
            string selectedStepIncrement = (string)tscbStepAndIncrement.SelectedItem;
            string[] tmp = selectedStepIncrement.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            return int.Parse(tmp[1]);
        }
        public void SetAnimationAcceleration(bool animationAcceleration)
        {
            InvokeIfRequired(_vtk.SetAnimationAcceleration, animationAcceleration);
        }
        public void SetAnimationFrameData(float[] time, int[] stepId, int[] stepIncrementId, float[] scale,
                                          double[] allFramesScalarRange, vtkMaxAnimationType animationType)
        {
            InvokeIfRequired(_vtk.SetAnimationFrameData, time, stepId, stepIncrementId, scale, allFramesScalarRange,
                             animationType);
        }
        public void SetAnimationFrame(int frameNum, bool scalarRangeFromAllFrames)
        {
            InvokeIfRequired(_vtk.SetAnimationFrame, frameNum, scalarRangeFromAllFrames);
        }
        public void SaveAnimationAsAVI(string fileName, int[] firstLastFrame, int step, int fps, bool scalarRangeFromAllFrames,
                                       bool swing, bool encoderOptions)
        {
            InvokeIfRequired(_vtk.SaveAnimationAsAVI, fileName, firstLastFrame, step, fps, scalarRangeFromAllFrames,
                             swing, encoderOptions);
        }
        public void SaveAnimationAsImages(string fileName, int[] firstLastFrame, int step, bool scalarRangeFromAllFrames,
                                          bool swing)
        {
            InvokeIfRequired(_vtk.SaveAnimationAsImages, fileName, firstLastFrame, step, scalarRangeFromAllFrames, swing);
        }
        // Widgets
        public void AddArrowWidget(string name, string text, string numberFormat, double[] anchorPoint,
                                   bool drawBackground, bool drawBorder, bool visible)
        {
            InvokeIfRequired(_vtk.AddArrowWidget, name, text, numberFormat, anchorPoint, drawBackground, drawBorder, visible);
        }
        public void RemoveAllArrowWidgets()
        {
            InvokeIfRequired(_vtk.RemoveAllArrowWidgets);
        }
        public void RemoveArrowWidgets(string[] widgetNames)
        {
            InvokeIfRequired(_vtk.RemoveArrowWidgets, widgetNames);
        }

        #endregion  ################################################################################################################

        #region Tree  ##############################################################################################################
        // Tree
        public void RegenerateTreeCallback()
        {
            RegenerateTree();
        }
        public void RegenerateTree(bool remeshing = false)
        {
            InvokeIfRequired(_modelTree.RegenerateTree, _controller.Model, _controller.Jobs, _controller.CurrentResult, remeshing);
            InvokeIfRequired(UpadteSymbolsForStepList);
        }
        public void AddTreeNode(ViewGeometryModelResults view, NamedClass item, string parentName)
        {
            ViewType viewType = GetViewType(view);
            //
            InvokeIfRequired(_modelTree.AddTreeNode, viewType, item, parentName);
            if (item is Step) UpadteSymbolsForStepList();
        }
        public void UpdateTreeNode(ViewGeometryModelResults view, string oldItemName, NamedClass item, string parentName,
                                   bool updateSelection = true)
        {
            ViewType viewType = GetViewType(view);
            //
            InvokeIfRequired(_modelTree.UpdateTreeNode, viewType, oldItemName, item, parentName, updateSelection);
            if (item is Step) UpadteOneStepInSymbolsForStepList(oldItemName, item.Name);
        }
        public void SwapTreeNode(ViewGeometryModelResults view, string firstItemName, NamedClass firstItem,
                                string secondItemName, NamedClass secondItem, string parentName)
        {
            ViewType viewType = GetViewType(view);
            //
            InvokeIfRequired(_modelTree.SwapTreeNodes, viewType, firstItemName, firstItem, secondItemName,
                             secondItem, parentName);
            //if (item is Step) UpadteOneStepInSymbolsForStepList(oldItemName, item.Name);
        }
        public void RemoveTreeNode<T>(ViewGeometryModelResults view, string nodeName, string parentName) where T : NamedClass
        {
            ViewType viewType = GetViewType(view);
            //
            InvokeIfRequired(_modelTree.RemoveTreeNode<T>, viewType, nodeName, parentName);
            if (typeof(T) == typeof(Step)) RemoveOneStepInSymbolsForStepList(nodeName);
        }
        public bool[][] GetTreeExpandCollapseState()
        {
            if (InvokeRequired)
            {
                return (bool[][])Invoke((Func<bool[][]>)delegate
                    { return _modelTree.GetAllTreesExpandCollapseState(out string[][] afterNodeNames); });
            }
            else
            {
                return _modelTree.GetAllTreesExpandCollapseState(out string[][] afterNodeNames);
            }
        }
        public void SetTreeExpandCollapseState(bool[][] states)
        {
            InvokeIfRequired(_modelTree.SetAllTreeExpandCollapseState, states);
        }
        public void UpdateHighlight()
        {
            if (_allForms == null) return;
            //
            List<IFormHighlight> highlightForms = new List<IFormHighlight>();
            foreach (var aForm in _allForms)
            {
                // Do not count the Query form
                if (aForm.Visible && (aForm is IFormHighlight ihf)) highlightForms.Add(ihf);
            }
            if (highlightForms.Count == 0) UpdateHighlightFromTree();
            else if (highlightForms.Count == 1) highlightForms[0].Highlight();
            else throw new NotSupportedException();
        }
        public void UpdateHighlightFromTree()
        {
            InvokeIfRequired(_modelTree.UpdateHighlight);
        }
        public void SelectBaseParts(string[] partNames)
        {
            MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
            InvokeIfRequired(SelectBaseParts, e, Keys.None, partNames);
        }
        public void SelectBaseParts(MouseEventArgs e, Keys modifierKeys, string[] partNames)
        {
            if ((partNames != null && partNames.Length == 0) || 
                (partNames != null && partNames.Length > 0 && partNames[0] == null))
            {
                if (modifierKeys != Keys.Shift && modifierKeys != Keys.Control) _controller.ClearAllSelection();
            }
            else
            {
                //
                if (e.Clicks == 2) _modelTree.EditSelectedPart();
                else
                {
                    int count = 0;
                    BasePart[] parts;
                    int numOfSelectedTreeNodes = 0;
                    //
                    if (GetCurrentView() == ViewGeometryModelResults.Geometry)
                        parts = _controller.GetGeometryPartsForSelection(partNames);
                    else if (GetCurrentView() == ViewGeometryModelResults.Model)
                        parts = _controller.GetModelParts(partNames);
                    else if (GetCurrentView() == ViewGeometryModelResults.Results)
                        parts = _controller.GetResultParts(partNames);
                    else throw new NotSupportedException();
                    //
                    foreach (var part in parts)
                    {
                        numOfSelectedTreeNodes = _modelTree.SelectBasePart(e, modifierKeys, part, false);
                        count++;
                        //
                        if (count == 1 && modifierKeys == Keys.None) modifierKeys |= Keys.Shift;
                    }
                    _modelTree.UpdateHighlight();
                    //
                    if (numOfSelectedTreeNodes > 0 && e.Button == MouseButtons.Right)
                    {
                        _modelTree.ShowContextMenu(_vtk, e.X, _vtk.Height - e.Y);
                    }
                }
            }
        }
        public void SelectFirstComponentOfFirstFieldOutput()
        {
            InvokeIfRequired(_modelTree.SelectFirstComponentOfFirstFieldOutput);
        }
        //
        public void SetNumberOfModelUserKeywords(int numOfUserKeywords)
        {
            InvokeIfRequired(_modelTree.SetNumberOfUserKeywords, numOfUserKeywords);
        }
        
        #endregion  ################################################################################################################

        // Output
        public void WriteDataToOutput(string data)
        {
            if (data == null) return;
            // 20 chars is an empty line with date
            if (data.Length == 0 && (outputLines.Length > 0 && outputLines.Last().Length == 20)) return;

            InvokeIfRequired(() =>
            {
                data = data.Replace("\r\n", "\n");
                data = data.Replace('\r', '\n');
                string[] lines = data.Split('\n');

                foreach (var line in lines) WriteLineToOutputWithDate(line);

                timerOutput.Start();
            });
            
            
        }
        private void WriteLineToOutputWithDate(string data)
        {
            int numColDate = 20;
            int numCol = 150 + numColDate;
            int numRows = 100;      // number of displayed lines

            List<string> lines = new List<string>(outputLines);
            List<string> wrappedLines = new List<string>();

            while (numColDate + data.Length > numCol)
            {
                wrappedLines.Add(data.Substring(0, numCol - numColDate) + "...");
                data = data.Substring(numCol - numColDate);
            }
            wrappedLines.Add(data);

            foreach (var wrappedLine in wrappedLines)
            {
                lines.Add(DateTime.Now.ToString("MM/dd/yy HH:mm:ss").PadRight(numColDate) + wrappedLine);
            }

            int firstLine = Math.Max(0, lines.Count - numRows);
            int numLines = Math.Min(lines.Count, numRows);

            outputLines = new string[numLines];
            Array.Copy(lines.ToArray(), firstLine, outputLines, 0, numLines);
        }

        #region Invoke  ############################################################################################################
        // Invoke
        public void InvokeIfRequired(Action action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { action(); });
            }
            else
            {
                action();
            }
        }
        public object InvokeIfRequired(Func<object> function)
        {
            if (this.InvokeRequired)
            {
                return (object)this.Invoke((MethodInvoker)delegate () { function(); });
            }
            else
            {
                return function();
            }
        }
        public void InvokeIfRequired<T>(Action<T> action, T parameter)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { action(parameter); });
            }
            else
            {
                action(parameter);
            }
        }
        public void InvokeIfRequired<T1,T2>(Action<T1, T2> action, T1 parameter1, T2 parameter2)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { action(parameter1, parameter2); });
            }
            else
            {
                action(parameter1, parameter2);
            }
        }
        public void InvokeIfRequired<T1, T2, T3>(Action<T1, T2, T3> action, T1 parameter1, T2 parameter2, T3 parameter3)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { action(parameter1, parameter2, parameter3); });
            }
            else
            {
                action(parameter1, parameter2, parameter3);
            }
        }
        public void InvokeIfRequired<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 parameter1, T2 parameter2, T3 parameter3, 
                                                     T4 parameter4)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { action(parameter1, parameter2, parameter3, parameter4); });
            }
            else
            {
                action(parameter1, parameter2, parameter3, parameter4);
            }
        }
        public void InvokeIfRequired<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 parameter1, T2 parameter2,
                                     T3 parameter3, T4 parameter4, T5 parameter5)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { action(parameter1, parameter2, parameter3, parameter4, parameter5); });
            }
            else
            {
                action(parameter1, parameter2, parameter3, parameter4, parameter5);
            }
        }
        public void InvokeIfRequired<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 parameter1, T2 parameter2, 
                                     T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { action(parameter1, parameter2, parameter3, parameter4, 
                                                         parameter5, parameter6); });
            }
            else
            {
                action(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6);
            }
        }
        public void InvokeIfRequired<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 parameter1, 
                                     T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () { action(parameter1, parameter2, parameter3, parameter4, parameter5,
                                                         parameter6, parameter7); });
            }
            else
            {
                action(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7);
            }
        }
        public void InvokeIfRequired<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 parameter1,
                                     T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7,
                                     T8 parameter8)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () {
                    action(parameter1, parameter2, parameter3, parameter4, parameter5,
                    parameter6, parameter7, parameter8);
                });
            }
            else
            {
                action(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8);
            }
        }
        public void InvokeIfRequired<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action,
                                     T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6,
                                     T7 parameter7, T8 parameter8, T9 parameter9)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate () {
                    action(parameter1, parameter2, parameter3, parameter4, parameter5,
                    parameter6, parameter7, parameter8, parameter9);
                });
            }
            else
            {
                action(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9);
            }
        }



































        #endregion  ################################################################################################################

        #endregion

        private void tsmiTest_Click(object sender, EventArgs e)
        {
            try
            {
                _vtk.SwithchLights();
                //_controller.TestCreateSurface();
            }
            catch
            {

            }

            return;
            

            ImportedPressure pressure = (ImportedPressure)_controller.GetStep("Step-1").Loads["Imported_pressure-1"];
            pressure.ImportPressure();
            //
            PartExchangeData allData = new PartExchangeData();
            _controller.Model.Mesh.GetAllNodesAndCells(out allData.Nodes.Ids, out allData.Nodes.Coor, out allData.Cells.Ids,
                                                       out allData.Cells.CellNodeIds, out allData.Cells.Types);
            //
            FeSurface surface = _controller.Model.Mesh.Surfaces[pressure.SurfaceName];
            FeNodeSet nodeSet = _controller.Model.Mesh.NodeSets[surface.NodeSetName];
            HashSet<int> nodeIds = new HashSet<int>(nodeSet.Labels);
            //
            double[] distance;
            double value;
            float[] distancesAll = new float[allData.Nodes.Coor.Length];
            float[] distances1 = new float[allData.Nodes.Coor.Length];
            float[] distances2 = new float[allData.Nodes.Coor.Length];
            float[] distances3 = new float[allData.Nodes.Coor.Length];
            float[] values = new float[allData.Nodes.Coor.Length];
            //
            for (int i = 0; i < values.Length; i++)
            {
                if (nodeIds.Contains(allData.Nodes.Ids[i]))
                {
                    pressure.GetPressureAndDistanceForPoint(allData.Nodes.Coor[i], out distance, out value);
                    distances1[i] = (float)distance[0];
                    distances2[i] = (float)distance[1];
                    distances3[i] = (float)distance[2];
                    distancesAll[i] = (float)Math.Sqrt(distance[0] * distance[0] +
                                                       distance[1] * distance[1] +
                                                       distance[2] * distance[2]);
                    values[i] = (float)value;
                }
                else
                {
                    distances1[i] = float.NaN;
                    distances2[i] = float.NaN;
                    distances3[i] = float.NaN;
                    distancesAll[i] = float.NaN;
                    values[i] = float.NaN;
                }
            }
            //
            Dictionary<int, int> nodeIdsLookUp = new Dictionary<int, int>();
            for (int i = 0; i < allData.Nodes.Coor.Length; i++) nodeIdsLookUp.Add(allData.Nodes.Ids[i], i);
            CaeResults.FeResults outResults = new CaeResults.FeResults("Imported_pressure-1");
            //outResults.FileName = "Imported_pressure-1";
            outResults.SetMesh(_controller.Model.Mesh, nodeIdsLookUp);
            // Add distances
            CaeResults.FieldData fieldData = new CaeResults.FieldData(CaeResults.FOFieldNames.Distance);
            fieldData.GlobalIncrementId = 1;
            fieldData.StepType = CaeResults.StepTypeEnum.Static;
            fieldData.Time = 1;
            fieldData.MethodId = 1;
            fieldData.StepId = 1;
            fieldData.StepIncrementId = 1;
            // Distances
            CaeResults.Field field = new CaeResults.Field(fieldData.Name);
            field.AddComponent(CaeResults.FOComponentNames.All, distancesAll);
            field.AddComponent(CaeResults.FOComponentNames.D1, distances1);
            field.AddComponent(CaeResults.FOComponentNames.D2, distances2);
            field.AddComponent(CaeResults.FOComponentNames.D3, distances3);
            outResults.AddField(fieldData, field);
            // Add values
            fieldData = new CaeResults.FieldData(fieldData);
            fieldData.Name = CaeResults.FOFieldNames.Imported;
            //
            field = new CaeResults.Field(fieldData.Name);
            field.AddComponent(CaeResults.FOComponentNames.PRESS, values);
            outResults.AddField(fieldData, field);
            // Unit system
            outResults.UnitSystem = new UnitSystem(_controller.Model.UnitSystem.UnitSystemType);
            _controller.SetResults(outResults);
            //
            if (_controller.CurrentResult != null && _controller.CurrentResult.Mesh != null)
            {
                SetResultNames();
                // Reset the previous step and increment
                SetAllStepAndIncrementIds();
                // Set last increment
                SetDefaultStepAndIncrementIds();
                // Show the selection in the results tree
                SelectFirstComponentOfFirstFieldOutput();
            }
            // Set the representation which also calls Draw
            _controller.ViewResultsType = ViewResultsTypeEnum.ColorContours;  // Draw
            //
            SetMenuAndToolStripVisibility();
            
            
            
            //
            //if (timerTest.Enabled) timerTest.Stop();
            //else timerTest.Start();
        }

        private void timerTest_Tick(object sender, EventArgs e)
        {
            //TestAnimation();
            //TestSelection1();
            TestSelection2();
        }
        private void TestAnimation()
        {
            try
            {
                timerTest.Interval = 10;
                //timerTest.Stop();

                string[] names = new string[] { CaeResults.FOFieldNames.Stress, CaeResults.FOFieldNames.Disp };
                string[] components = new string[] { "SZZ", "D2" };

                CaeResults.FieldData currentData = _controller.CurrentFieldData;
                int i = 0;
                if (currentData.Component == components[i]) i++;

                int len = names.Length;

                SetFieldData(names[i % len], components[i % len], 1, 1);
            }
            catch
            { }
        }
        private void TestSelection1()
        {
            try
            {
                timerTest.Interval = 100;
                //
                string[] allPartNames = _controller.GetGeometryPartNames();
                HashSet<string> selectedPartNames = new HashSet<string>();
                //
                Random rand = new Random();
                for (int i = 0; i < 100; i++)
                {
                    selectedPartNames.Add(allPartNames[(int)(rand.NextDouble() * allPartNames.Length - 1)]);
                }
                Clear3DSelection();
                _controller.HighlightGeometryParts(selectedPartNames.ToArray());
                //
                Application.DoEvents();
            }
            catch
            { }
        }
        private void TestSelection2()
        {
            try
            {
                timerTest.Interval = 50;
                //
                Random rand = new Random();
                int x1 = _vtk.Width / 4 + rand.Next(_vtk.Width / 2);
                int y1 = _vtk.Height / 4 + rand.Next(_vtk.Height / 2);

                //x1 = 523;
                //y1 = 421;

                Clear3DSelection();
                _vtk.Pick(x1, y1, false, 0, 0);
                //
                Application.DoEvents();
            }
            catch
            { }
        }
        internal void tsmiAdvisor_Click(object sender, EventArgs e)
        {
            if (!_controller.ModelInitialized) return;
            // Change the wizzard check state
            tsmiAdvisor.Checked = !tsmiAdvisor.Checked;
            // Add wizard panel
            if (tsmiAdvisor.Checked == true)
            {
                Control parent = panelControl.Parent;
                if (parent == splitContainer2.Panel1)
                {
                    // First remove the vtk comtrol and panel border
                    parent.Controls.Remove(_vtk);
                    parent.Controls.Remove(panelControl);
                    // Split container
                    SplitContainer splitContainer = new SplitContainer();
                    splitContainer.FixedPanel = FixedPanel.Panel2;
                    splitContainer.Dock = DockStyle.Fill;
                    parent.Controls.Add(splitContainer);
                    // Set the Panel 2 size - min 100 max 300
                    splitContainer.SplitterDistance = Math.Max(100, Math.Max(parent.Width - 300, (int)(parent.Width * 0.8)));
                    // Panel 1 - LEFT
                    splitContainer.Panel1.Controls.Add(_vtk);
                    splitContainer.Panel1.Controls.Add(panelControl);
                    panelControl.SendToBack();
                    // Update vtk control size
                    UpdateVtkControlSize();
                    // Panel 2 - RIGHT
                    _advisorControl = AdvisorCreator.CreateControl(this);
                    //
                    splitContainer.Panel2.Controls.Add(_advisorControl);
                    _advisorControl.Dock = DockStyle.Fill;
                    _advisorControl.UpdateDesign();
                    _advisorControl.AutoScroll = true;
                }
            }
            // Remove wizard panel
            else
            {
                Control parent = panelControl.Parent;
                if (parent is SplitterPanel && parent != splitContainer2.Panel1)
                {
                    // First remove the vtk comtrol and panel border
                    parent.Controls.Remove(_vtk);
                    parent.Controls.Remove(panelControl);
                    // Remove added split container
                    splitContainer2.Panel1.Controls.Clear();
                    _advisorControl = null;
                    // Add controls back
                    splitContainer2.Panel1.Controls.Add(panelControl);
                    splitContainer2.Panel1.Controls.Add(_vtk);
                    panelControl.SendToBack();
                    // Update vtk control size
                    UpdateVtkControlSize();
                }
            }
        }

       
    }
}
