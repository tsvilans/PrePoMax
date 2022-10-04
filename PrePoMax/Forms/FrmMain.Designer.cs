namespace PrePoMax
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.tsFile = new UserControls.ToolStripFocus();
            this.tsbNew = new System.Windows.Forms.ToolStripButton();
            this.tsbOpen = new System.Windows.Forms.ToolStripButton();
            this.tsbImport = new System.Windows.Forms.ToolStripButton();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.tsViews = new UserControls.ToolStripFocus();
            this.tsbZoomToFit = new System.Windows.Forms.ToolStripButton();
            this.toolStripViewSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbFrontView = new System.Windows.Forms.ToolStripButton();
            this.tsbBackView = new System.Windows.Forms.ToolStripButton();
            this.tsbTopView = new System.Windows.Forms.ToolStripButton();
            this.tsbBottomView = new System.Windows.Forms.ToolStripButton();
            this.tsbLeftView = new System.Windows.Forms.ToolStripButton();
            this.tsbRightView = new System.Windows.Forms.ToolStripButton();
            this.tsbNormalView = new System.Windows.Forms.ToolStripButton();
            this.tsbVerticalView = new System.Windows.Forms.ToolStripButton();
            this.tsbIsometric = new System.Windows.Forms.ToolStripButton();
            this.toolStripViewSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbShowWireframeEdges = new System.Windows.Forms.ToolStripButton();
            this.tsbShowElementEdges = new System.Windows.Forms.ToolStripButton();
            this.tsbShowModelEdges = new System.Windows.Forms.ToolStripButton();
            this.tsbShowNoEdges = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSectionView = new System.Windows.Forms.ToolStripButton();
            this.tsbExplodedView = new System.Windows.Forms.ToolStripButton();
            this.toolStripViewSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbQuery = new System.Windows.Forms.ToolStripButton();
            this.tsbRemoveAnnotations = new System.Windows.Forms.ToolStripButton();
            this.toolStripViewSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbShowAllParts = new System.Windows.Forms.ToolStripButton();
            this.tsbHideAllParts = new System.Windows.Forms.ToolStripButton();
            this.tsbInvertVisibleParts = new System.Windows.Forms.ToolStripButton();
            this.tsModel = new UserControls.ToolStripFocus();
            this.tslSymbols = new System.Windows.Forms.ToolStripLabel();
            this.tscbSymbolsForStep = new System.Windows.Forms.ToolStripComboBox();
            this.tsDeformationFactor = new UserControls.ToolStripFocus();
            this.tslResultName = new System.Windows.Forms.ToolStripLabel();
            this.tscbResultNames = new System.Windows.Forms.ToolStripComboBox();
            this.tslDeformationVariable = new System.Windows.Forms.ToolStripLabel();
            this.tscbDeformationVariable = new System.Windows.Forms.ToolStripComboBox();
            this.tslDeformationType = new System.Windows.Forms.ToolStripLabel();
            this.tscbDeformationType = new System.Windows.Forms.ToolStripComboBox();
            this.tslDeformationFactor = new System.Windows.Forms.ToolStripLabel();
            this.tstbDeformationFactor = new UserControls.NumericToolStripTextBox();
            this.tsResults = new UserControls.ToolStripFocus();
            this.tsbResultsUndeformed = new System.Windows.Forms.ToolStripButton();
            this.tsbResultsDeformed = new System.Windows.Forms.ToolStripButton();
            this.tsbResultsColorContours = new System.Windows.Forms.ToolStripButton();
            this.tsbResultsUndeformedWireframe = new System.Windows.Forms.ToolStripButton();
            this.tsbResultsUndeformedSolid = new System.Windows.Forms.ToolStripButton();
            this.toolStripResultsSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbTransformation = new System.Windows.Forms.ToolStripButton();
            this.toolStripResultsSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbFirstStepIncrement = new System.Windows.Forms.ToolStripButton();
            this.tsbPreviousStepIncrement = new System.Windows.Forms.ToolStripButton();
            this.tslStepIncrement = new System.Windows.Forms.ToolStripLabel();
            this.tscbStepAndIncrement = new System.Windows.Forms.ToolStripComboBox();
            this.tsbNextStepIncrement = new System.Windows.Forms.ToolStripButton();
            this.tsbLastStepIncrement = new System.Windows.Forms.ToolStripButton();
            this.tsbAnimate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStripMain = new UserControls.MenuStripFocus();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNew = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImportFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerFile1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerFile2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExport = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportToStep = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportToBrep = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportToStereolitography = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerExport1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExportToCalculix = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportToAbaqus = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportToMmgMesh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerExport2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExportToDeformedInp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportToDeformedStl = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerFile3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCloseCurrentResult = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCloseAllResults = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerEdit1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiViewHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRegenerate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRegenerateUsingOtherFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRegenerateForRemeshing = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiView = new System.Windows.Forms.ToolStripMenuItem();
            this.standardViewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFrontView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBackView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTopView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBottomView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLeftView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRightView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNormalView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiVerticalView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIsometricView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiZoomToFit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerView1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiShowWireframeEdges = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowElementEdges = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowModelEdges = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowNoEdges = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerView2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSectionView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExplodedView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerView3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiShowAllParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHideAllParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiInvertVisibleParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerView4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiResultsUndeformed = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiResultsDeformed = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiResultsColorContours = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerView5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiColorAnnotations = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAnnotateFaceOrientations = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerColorAnnotations1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAnnotateParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAnnotateMaterials = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAnnotateSections = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAnnotateSectionThicknesses = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerColorAnnotations2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAnnotateAllSymbols = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAnnotateReferencePoints = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAnnotateConstraints = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAnnotateContactPairs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAnnotateBCs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAnnotateLoads = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGeometry = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGeometryPart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditGeometryPart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTransformGeometryParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScaleGeometryParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerGeomPart1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCopyGeometryPartsToResults = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerGeomPart2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHideGeometryParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowGeometryParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowOnlyGeometryParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetTransparencyForGeometryParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerGeomPart3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteGeometryParts = new System.Windows.Forms.ToolStripMenuItem();
            this.cADPartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFlipFaceNormalCAD = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSplitAFaceUsingTwoPoints = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiStlPart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFindEdgesByAngleForGeometryParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFlipStlPartSurfacesNormal = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSmoothPart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCropWithCylinder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCropWithCube = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerGeometry1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCreateAndImportCompoundPart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRegenerateCompoundPart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSwapGeometryPartGeometries = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerGeometry2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiGeometryAnalyze = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMesh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMeshingParameters = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMeshRefinement = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateMeshRefinement = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditMeshRefinement = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerMeshRefinement1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteMeshRefinement = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPreviewEdgeMesh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateMesh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiModel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditModel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditCalculiXKeywords = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiToolsParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateBoundaryLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFindEdgesByAngleForModelParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemeshElements = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUpdateNodalCoordinatesFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerModel1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiNode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRenumberAllNodes = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiElement = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRenumberAllElements = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditPart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTransformModelParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTranslateModelParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScaleModelParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRotateModelParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMergeParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerPart2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHideParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowOnlyParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetTransparencyForParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerPart3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNodeSet = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateNodeSet = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditNodeSet = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDuplicateNodeSet = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerNodeSet1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteNodeSet = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiElementSet = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateElementSet = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditElementSet = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDuplicateElementSet = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiConvertElementSetsToMeshParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerElementSet1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteElementSet = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSurface = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateSurface = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditSurface = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerSurface1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteSurface = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReferencePointTool = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateRP = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditRP = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRP = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteRP = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiProperty = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMaterial = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateMaterial = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditMaterial = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDuplicateMaterial = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerMaterial1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiImportMaterial = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportMaterial = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerMaterial2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteMaterial = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMaterialLibrary = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSection = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateSection = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditSection = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerSection1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiInteraction = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiConstraint = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateConstraint = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditConstraint = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerConstraint1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSwapMasterSlaveConstraint = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMergeByMasterSlaveConstraint = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerConstraint2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHideConstraint = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowConstraint = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerConstraint3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteConstraint = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiContact = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSurfaceInteraction = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateSurfaceInteraction = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditSurfaceInteraction = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDuplicateSurfaceInteraction = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerSurfaceInteraction1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteSurfaceInteraction = new System.Windows.Forms.ToolStripMenuItem();
            this.contactPairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateContactPair = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditContactPair = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerContactPair1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSwapMasterSlaveContactPair = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMergeByMasterSlaveContactPair = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerContactPair2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHideContactPair = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowContactPair = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerContactPair3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteContactPair = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerInteraction1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSearchContactPairs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAmplitude = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateAmplitude = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditAmplitude = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerAmplitude1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteAmplitude = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiInitialCondition = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateInitialCondition = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditInitialCondition = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPreviewInitialCondition = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerInitialCondition1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteInitialCondition = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiStepMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiStep = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateStep = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditStep = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDuplicateStep = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerStep2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteStep = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerStep1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHistoryOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateHistoryOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditHistoryOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPropagateHistoryOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHistoryOutputDivider1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteHistoryOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFieldOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateFieldOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditFieldOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPropagateFieldOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerFieldOutput1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteFieldOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBC = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateBC = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditBC = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPropagateBC = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerBC1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHideBC = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowBC = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerBC2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteBC = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPreviewLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPropagateLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerLoad1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHideLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerLoad2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDefinedField = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateDefinedField = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditDefinedField = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPropagateDefinedField = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPreviewDefinedField = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerDefinedField1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteDefinedField = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerAnalysis1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRunAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMonitorAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiResultsAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiKillAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerAnalysis2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiResults = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiResultPart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditResultPart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerResultPart1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHideResultParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowResultParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowOnlyResultParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetTransparencyForResultParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerResultPart2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiColorContoursOff = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorContoursOn = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerResultPart3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteResultParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerResults1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiResultFieldOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeleteResultFieldOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiResultHistoryOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateResultHistoryOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerResultHistoryOutput1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteResultHistoryOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerResults2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiTransformation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTools = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQuery = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAdvisor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHomePage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAdvisorHelp1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTest = new System.Windows.Forms.ToolStripMenuItem();
            this.panelControl = new System.Windows.Forms.Panel();
            this.aeAnnotationTextEditor = new UserControls.AnnotationEditor();
            this.cmsAnnotation = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiEditAnnotation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiResetAnnotation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerAnnotation1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAnnotationSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDividerAnnotation2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDeleteAnnotation = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.tspbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.tsslState = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslCancel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslEmpty = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslUnitSystem = new System.Windows.Forms.ToolStripStatusLabel();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.timerTest = new System.Windows.Forms.Timer(this.components);
            this.timerOutput = new System.Windows.Forms.Timer(this.components);
            this.tsFile.SuspendLayout();
            this.tsViews.SuspendLayout();
            this.tsModel.SuspendLayout();
            this.tsDeformationFactor.SuspendLayout();
            this.tsResults.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.panelControl.SuspendLayout();
            this.cmsAnnotation.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsFile
            // 
            this.tsFile.BackColor = System.Drawing.SystemColors.Control;
            this.tsFile.DisableMouseButtons = false;
            this.tsFile.Dock = System.Windows.Forms.DockStyle.None;
            this.tsFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbNew,
            this.tsbOpen,
            this.tsbImport,
            this.tsbSave});
            this.tsFile.Location = new System.Drawing.Point(3, 50);
            this.tsFile.Name = "tsFile";
            this.tsFile.Size = new System.Drawing.Size(104, 25);
            this.tsFile.TabIndex = 5;
            this.tsFile.Text = "File";
            // 
            // tsbNew
            // 
            this.tsbNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNew.Image = global::PrePoMax.Properties.Resources.New;
            this.tsbNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNew.Name = "tsbNew";
            this.tsbNew.Size = new System.Drawing.Size(23, 22);
            this.tsbNew.Text = "New model";
            this.tsbNew.Click += new System.EventHandler(this.tsbNew_Click);
            // 
            // tsbOpen
            // 
            this.tsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpen.Image = global::PrePoMax.Properties.Resources.Open;
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(23, 22);
            this.tsbOpen.Text = "Open file";
            this.tsbOpen.Click += new System.EventHandler(this.tsbOpen_Click);
            // 
            // tsbImport
            // 
            this.tsbImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbImport.Image = global::PrePoMax.Properties.Resources.Import;
            this.tsbImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbImport.Name = "tsbImport";
            this.tsbImport.Size = new System.Drawing.Size(23, 22);
            this.tsbImport.Text = "Import file";
            this.tsbImport.ToolTipText = "Import file";
            this.tsbImport.Click += new System.EventHandler(this.tsbImport_Click);
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = global::PrePoMax.Properties.Resources.Save;
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(23, 22);
            this.tsbSave.Text = "Save to file";
            this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
            // 
            // tsViews
            // 
            this.tsViews.BackColor = System.Drawing.SystemColors.Control;
            this.tsViews.DisableMouseButtons = false;
            this.tsViews.Dock = System.Windows.Forms.DockStyle.None;
            this.tsViews.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbZoomToFit,
            this.toolStripViewSeparator1,
            this.tsbFrontView,
            this.tsbBackView,
            this.tsbTopView,
            this.tsbBottomView,
            this.tsbLeftView,
            this.tsbRightView,
            this.tsbNormalView,
            this.tsbVerticalView,
            this.tsbIsometric,
            this.toolStripViewSeparator2,
            this.tsbShowWireframeEdges,
            this.tsbShowElementEdges,
            this.tsbShowModelEdges,
            this.tsbShowNoEdges,
            this.toolStripSeparator1,
            this.tsbSectionView,
            this.tsbExplodedView,
            this.toolStripViewSeparator3,
            this.tsbQuery,
            this.tsbRemoveAnnotations,
            this.toolStripViewSeparator4,
            this.tsbShowAllParts,
            this.tsbHideAllParts,
            this.tsbInvertVisibleParts});
            this.tsViews.Location = new System.Drawing.Point(107, 50);
            this.tsViews.Name = "tsViews";
            this.tsViews.Size = new System.Drawing.Size(525, 25);
            this.tsViews.TabIndex = 6;
            this.tsViews.Text = "Views";
            // 
            // tsbZoomToFit
            // 
            this.tsbZoomToFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbZoomToFit.Image = global::PrePoMax.Properties.Resources.ZoomToFit;
            this.tsbZoomToFit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbZoomToFit.Name = "tsbZoomToFit";
            this.tsbZoomToFit.Size = new System.Drawing.Size(23, 22);
            this.tsbZoomToFit.Text = "Zoom to fit";
            this.tsbZoomToFit.Click += new System.EventHandler(this.tsbZoomToFit_Click);
            // 
            // toolStripViewSeparator1
            // 
            this.toolStripViewSeparator1.Name = "toolStripViewSeparator1";
            this.toolStripViewSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbFrontView
            // 
            this.tsbFrontView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFrontView.Image = global::PrePoMax.Properties.Resources.Front;
            this.tsbFrontView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFrontView.Name = "tsbFrontView";
            this.tsbFrontView.Size = new System.Drawing.Size(23, 22);
            this.tsbFrontView.Text = "Front view";
            this.tsbFrontView.Click += new System.EventHandler(this.tsbFrontView_Click);
            // 
            // tsbBackView
            // 
            this.tsbBackView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbBackView.Image = global::PrePoMax.Properties.Resources.Back;
            this.tsbBackView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbBackView.Name = "tsbBackView";
            this.tsbBackView.Size = new System.Drawing.Size(23, 22);
            this.tsbBackView.Text = "Back view";
            this.tsbBackView.Click += new System.EventHandler(this.tsbBackView_Click);
            // 
            // tsbTopView
            // 
            this.tsbTopView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTopView.Image = global::PrePoMax.Properties.Resources.Top;
            this.tsbTopView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbTopView.Name = "tsbTopView";
            this.tsbTopView.Size = new System.Drawing.Size(23, 22);
            this.tsbTopView.Text = "Top view";
            this.tsbTopView.Click += new System.EventHandler(this.tsbTopView_Click);
            // 
            // tsbBottomView
            // 
            this.tsbBottomView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbBottomView.Image = global::PrePoMax.Properties.Resources.Bottom;
            this.tsbBottomView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbBottomView.Name = "tsbBottomView";
            this.tsbBottomView.Size = new System.Drawing.Size(23, 22);
            this.tsbBottomView.Text = "Bottom view";
            this.tsbBottomView.Click += new System.EventHandler(this.tsbBottomView_Click);
            // 
            // tsbLeftView
            // 
            this.tsbLeftView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLeftView.Image = global::PrePoMax.Properties.Resources.Left;
            this.tsbLeftView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLeftView.Name = "tsbLeftView";
            this.tsbLeftView.Size = new System.Drawing.Size(23, 22);
            this.tsbLeftView.Text = "Left view";
            this.tsbLeftView.Click += new System.EventHandler(this.tsbLeftView_Click);
            // 
            // tsbRightView
            // 
            this.tsbRightView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRightView.Image = global::PrePoMax.Properties.Resources.Right;
            this.tsbRightView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRightView.Name = "tsbRightView";
            this.tsbRightView.Size = new System.Drawing.Size(23, 22);
            this.tsbRightView.Text = "Right view";
            this.tsbRightView.Click += new System.EventHandler(this.tsbRightView_Click);
            // 
            // tsbNormalView
            // 
            this.tsbNormalView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNormalView.Image = global::PrePoMax.Properties.Resources.Normal;
            this.tsbNormalView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNormalView.Name = "tsbNormalView";
            this.tsbNormalView.Size = new System.Drawing.Size(23, 22);
            this.tsbNormalView.Text = "Normal view";
            this.tsbNormalView.Click += new System.EventHandler(this.tsbNormalView_Click);
            // 
            // tsbVerticalView
            // 
            this.tsbVerticalView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbVerticalView.Image = global::PrePoMax.Properties.Resources.Vertical;
            this.tsbVerticalView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbVerticalView.Name = "tsbVerticalView";
            this.tsbVerticalView.Size = new System.Drawing.Size(23, 22);
            this.tsbVerticalView.Text = "Vertical view";
            this.tsbVerticalView.Click += new System.EventHandler(this.tsbVerticalView_Click);
            // 
            // tsbIsometric
            // 
            this.tsbIsometric.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbIsometric.Image = global::PrePoMax.Properties.Resources.Isometric;
            this.tsbIsometric.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbIsometric.Name = "tsbIsometric";
            this.tsbIsometric.Size = new System.Drawing.Size(23, 22);
            this.tsbIsometric.Text = "Isometric view";
            this.tsbIsometric.Click += new System.EventHandler(this.tsbIsometric_Click);
            // 
            // toolStripViewSeparator2
            // 
            this.toolStripViewSeparator2.Name = "toolStripViewSeparator2";
            this.toolStripViewSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbShowWireframeEdges
            // 
            this.tsbShowWireframeEdges.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbShowWireframeEdges.Image = global::PrePoMax.Properties.Resources.Wireframe;
            this.tsbShowWireframeEdges.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbShowWireframeEdges.Name = "tsbShowWireframeEdges";
            this.tsbShowWireframeEdges.Size = new System.Drawing.Size(23, 22);
            this.tsbShowWireframeEdges.Text = "Wireframe";
            this.tsbShowWireframeEdges.Click += new System.EventHandler(this.tsbShowWireframeEdges_Click);
            // 
            // tsbShowElementEdges
            // 
            this.tsbShowElementEdges.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbShowElementEdges.Image = global::PrePoMax.Properties.Resources.ElementEdges;
            this.tsbShowElementEdges.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbShowElementEdges.Name = "tsbShowElementEdges";
            this.tsbShowElementEdges.Size = new System.Drawing.Size(23, 22);
            this.tsbShowElementEdges.Text = "Show element edges";
            this.tsbShowElementEdges.Click += new System.EventHandler(this.tsbShowElementEdges_Click);
            // 
            // tsbShowModelEdges
            // 
            this.tsbShowModelEdges.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbShowModelEdges.Image = global::PrePoMax.Properties.Resources.ModelEdges;
            this.tsbShowModelEdges.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbShowModelEdges.Name = "tsbShowModelEdges";
            this.tsbShowModelEdges.Size = new System.Drawing.Size(23, 22);
            this.tsbShowModelEdges.Text = "Show model edges";
            this.tsbShowModelEdges.Click += new System.EventHandler(this.tsbShowModelEdges_Click);
            // 
            // tsbShowNoEdges
            // 
            this.tsbShowNoEdges.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbShowNoEdges.Image = global::PrePoMax.Properties.Resources.NoEdges;
            this.tsbShowNoEdges.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbShowNoEdges.Name = "tsbShowNoEdges";
            this.tsbShowNoEdges.Size = new System.Drawing.Size(23, 22);
            this.tsbShowNoEdges.Text = "No edges";
            this.tsbShowNoEdges.Click += new System.EventHandler(this.tsbShowNoEdges_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbSectionView
            // 
            this.tsbSectionView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSectionView.Image = global::PrePoMax.Properties.Resources.SectionView;
            this.tsbSectionView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSectionView.Name = "tsbSectionView";
            this.tsbSectionView.Size = new System.Drawing.Size(23, 22);
            this.tsbSectionView.Text = "Section view";
            this.tsbSectionView.Click += new System.EventHandler(this.tsbSectionView_Click);
            // 
            // tsbExplodedView
            // 
            this.tsbExplodedView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbExplodedView.Image = global::PrePoMax.Properties.Resources.Explode;
            this.tsbExplodedView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbExplodedView.Name = "tsbExplodedView";
            this.tsbExplodedView.Size = new System.Drawing.Size(23, 22);
            this.tsbExplodedView.Text = "Exploded view";
            this.tsbExplodedView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tsbExplodedView_MouseUp);
            // 
            // toolStripViewSeparator3
            // 
            this.toolStripViewSeparator3.Name = "toolStripViewSeparator3";
            this.toolStripViewSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbQuery
            // 
            this.tsbQuery.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbQuery.Image = global::PrePoMax.Properties.Resources.Query;
            this.tsbQuery.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbQuery.Name = "tsbQuery";
            this.tsbQuery.Size = new System.Drawing.Size(23, 22);
            this.tsbQuery.Text = "Query";
            this.tsbQuery.Click += new System.EventHandler(this.tsbQuery_Click);
            // 
            // tsbRemoveAnnotations
            // 
            this.tsbRemoveAnnotations.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRemoveAnnotations.Image = global::PrePoMax.Properties.Resources.Remove_annotations;
            this.tsbRemoveAnnotations.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRemoveAnnotations.Name = "tsbRemoveAnnotations";
            this.tsbRemoveAnnotations.Size = new System.Drawing.Size(23, 22);
            this.tsbRemoveAnnotations.Text = "Remove annotations";
            this.tsbRemoveAnnotations.Click += new System.EventHandler(this.tsbRemoveAnnotations_Click);
            // 
            // toolStripViewSeparator4
            // 
            this.toolStripViewSeparator4.Name = "toolStripViewSeparator4";
            this.toolStripViewSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbShowAllParts
            // 
            this.tsbShowAllParts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbShowAllParts.Image = global::PrePoMax.Properties.Resources.Show;
            this.tsbShowAllParts.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbShowAllParts.Name = "tsbShowAllParts";
            this.tsbShowAllParts.Size = new System.Drawing.Size(23, 22);
            this.tsbShowAllParts.Text = "Show all parts";
            this.tsbShowAllParts.Click += new System.EventHandler(this.tsbShowAllParts_Click);
            // 
            // tsbHideAllParts
            // 
            this.tsbHideAllParts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbHideAllParts.Image = global::PrePoMax.Properties.Resources.Hide;
            this.tsbHideAllParts.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHideAllParts.Name = "tsbHideAllParts";
            this.tsbHideAllParts.Size = new System.Drawing.Size(23, 22);
            this.tsbHideAllParts.Text = "Hide all parts";
            this.tsbHideAllParts.Click += new System.EventHandler(this.tsbHideAllParts_Click);
            // 
            // tsbInvertVisibleParts
            // 
            this.tsbInvertVisibleParts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbInvertVisibleParts.Image = global::PrePoMax.Properties.Resources.InvertHideShow;
            this.tsbInvertVisibleParts.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbInvertVisibleParts.Name = "tsbInvertVisibleParts";
            this.tsbInvertVisibleParts.Size = new System.Drawing.Size(23, 22);
            this.tsbInvertVisibleParts.Text = "Invert visible parts";
            this.tsbInvertVisibleParts.Click += new System.EventHandler(this.tsbInvertVisibleParts_Click);
            // 
            // tsModel
            // 
            this.tsModel.DisableMouseButtons = false;
            this.tsModel.Dock = System.Windows.Forms.DockStyle.None;
            this.tsModel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslSymbols,
            this.tscbSymbolsForStep});
            this.tsModel.Location = new System.Drawing.Point(3, 0);
            this.tsModel.Name = "tsModel";
            this.tsModel.Size = new System.Drawing.Size(187, 25);
            this.tsModel.TabIndex = 9;
            // 
            // tslSymbols
            // 
            this.tslSymbols.Name = "tslSymbols";
            this.tslSymbols.Size = new System.Drawing.Size(52, 22);
            this.tslSymbols.Text = "Symbols";
            // 
            // tscbSymbolsForStep
            // 
            this.tscbSymbolsForStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbSymbolsForStep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tscbSymbolsForStep.Items.AddRange(new object[] {
            "None"});
            this.tscbSymbolsForStep.Name = "tscbSymbolsForStep";
            this.tscbSymbolsForStep.Size = new System.Drawing.Size(121, 25);
            this.tscbSymbolsForStep.ToolTipText = "Select how symbols are displayed.";
            // 
            // tsDeformationFactor
            // 
            this.tsDeformationFactor.BackColor = System.Drawing.SystemColors.Control;
            this.tsDeformationFactor.DisableMouseButtons = false;
            this.tsDeformationFactor.Dock = System.Windows.Forms.DockStyle.None;
            this.tsDeformationFactor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslResultName,
            this.tscbResultNames,
            this.tslDeformationVariable,
            this.tscbDeformationVariable,
            this.tslDeformationType,
            this.tscbDeformationType,
            this.tslDeformationFactor,
            this.tstbDeformationFactor});
            this.tsDeformationFactor.Location = new System.Drawing.Point(3, 25);
            this.tsDeformationFactor.Name = "tsDeformationFactor";
            this.tsDeformationFactor.Size = new System.Drawing.Size(622, 25);
            this.tsDeformationFactor.TabIndex = 8;
            // 
            // tslResultName
            // 
            this.tslResultName.Name = "tslResultName";
            this.tslResultName.Size = new System.Drawing.Size(39, 22);
            this.tslResultName.Text = "Result";
            // 
            // tscbResultNames
            // 
            this.tscbResultNames.AutoToolTip = true;
            this.tscbResultNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbResultNames.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tscbResultNames.Name = "tscbResultNames";
            this.tscbResultNames.Size = new System.Drawing.Size(121, 25);
            this.tscbResultNames.SelectedIndexChanged += new System.EventHandler(this.tscbResultNames_SelectedIndexChanged);
            // 
            // tslDeformationVariable
            // 
            this.tslDeformationVariable.Name = "tslDeformationVariable";
            this.tslDeformationVariable.Size = new System.Drawing.Size(48, 22);
            this.tslDeformationVariable.Text = "Variable";
            // 
            // tscbDeformationVariable
            // 
            this.tscbDeformationVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbDeformationVariable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tscbDeformationVariable.Name = "tscbDeformationVariable";
            this.tscbDeformationVariable.Size = new System.Drawing.Size(121, 25);
            this.tscbDeformationVariable.ToolTipText = "Select the deformation variable";
            this.tscbDeformationVariable.SelectedIndexChanged += new System.EventHandler(this.tscbDeformationVariable_SelectedIndexChanged);
            // 
            // tslDeformationType
            // 
            this.tslDeformationType.Name = "tslDeformationType";
            this.tslDeformationType.Size = new System.Drawing.Size(31, 22);
            this.tslDeformationType.Text = "Type";
            // 
            // tscbDeformationType
            // 
            this.tscbDeformationType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbDeformationType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tscbDeformationType.Name = "tscbDeformationType";
            this.tscbDeformationType.Size = new System.Drawing.Size(121, 25);
            this.tscbDeformationType.ToolTipText = "Select the deformation type";
            this.tscbDeformationType.SelectedIndexChanged += new System.EventHandler(this.tscbDeformationType_SelectedIndexChanged);
            // 
            // tslDeformationFactor
            // 
            this.tslDeformationFactor.Name = "tslDeformationFactor";
            this.tslDeformationFactor.Size = new System.Drawing.Size(40, 22);
            this.tslDeformationFactor.Text = "Factor";
            // 
            // tstbDeformationFactor
            // 
            this.tstbDeformationFactor.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tstbDeformationFactor.Name = "tstbDeformationFactor";
            this.tstbDeformationFactor.ShortcutsEnabled = false;
            this.tstbDeformationFactor.Size = new System.Drawing.Size(50, 25);
            this.tstbDeformationFactor.Text = "10";
            this.tstbDeformationFactor.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tstbDeformationFactor.ToolTipText = "Enter the deformation scale factor";
            this.tstbDeformationFactor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tstbDeformationFactor_KeyDown);
            this.tstbDeformationFactor.EnabledChanged += new System.EventHandler(this.tstbDeformationFactor_EnabledChanged);
            // 
            // tsResults
            // 
            this.tsResults.BackColor = System.Drawing.SystemColors.Control;
            this.tsResults.DisableMouseButtons = false;
            this.tsResults.Dock = System.Windows.Forms.DockStyle.None;
            this.tsResults.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbResultsUndeformed,
            this.tsbResultsDeformed,
            this.tsbResultsColorContours,
            this.tsbResultsUndeformedWireframe,
            this.tsbResultsUndeformedSolid,
            this.toolStripResultsSeparator1,
            this.tsbTransformation,
            this.toolStripResultsSeparator2,
            this.tsbFirstStepIncrement,
            this.tsbPreviousStepIncrement,
            this.tslStepIncrement,
            this.tscbStepAndIncrement,
            this.tsbNextStepIncrement,
            this.tsbLastStepIncrement,
            this.tsbAnimate,
            this.toolStripSeparator2});
            this.tsResults.Location = new System.Drawing.Point(3, 75);
            this.tsResults.Name = "tsResults";
            this.tsResults.Size = new System.Drawing.Size(450, 25);
            this.tsResults.TabIndex = 7;
            this.tsResults.Text = "Results";
            // 
            // tsbResultsUndeformed
            // 
            this.tsbResultsUndeformed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbResultsUndeformed.Image = global::PrePoMax.Properties.Resources.Undeformed;
            this.tsbResultsUndeformed.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbResultsUndeformed.Name = "tsbResultsUndeformed";
            this.tsbResultsUndeformed.Size = new System.Drawing.Size(23, 22);
            this.tsbResultsUndeformed.Text = "Undeformed";
            this.tsbResultsUndeformed.Click += new System.EventHandler(this.tsbResultsUndeformed_Click);
            // 
            // tsbResultsDeformed
            // 
            this.tsbResultsDeformed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbResultsDeformed.Image = global::PrePoMax.Properties.Resources.Deformed;
            this.tsbResultsDeformed.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbResultsDeformed.Name = "tsbResultsDeformed";
            this.tsbResultsDeformed.Size = new System.Drawing.Size(23, 22);
            this.tsbResultsDeformed.Text = "Deformed";
            this.tsbResultsDeformed.Click += new System.EventHandler(this.tsbResultsDeformed_Click);
            // 
            // tsbResultsColorContours
            // 
            this.tsbResultsColorContours.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbResultsColorContours.Image = global::PrePoMax.Properties.Resources.Color_contours;
            this.tsbResultsColorContours.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbResultsColorContours.Name = "tsbResultsColorContours";
            this.tsbResultsColorContours.Size = new System.Drawing.Size(23, 22);
            this.tsbResultsColorContours.Text = "Deformed with color contours";
            this.tsbResultsColorContours.Click += new System.EventHandler(this.tsbResultsColorContours_Click);
            // 
            // tsbResultsUndeformedWireframe
            // 
            this.tsbResultsUndeformedWireframe.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbResultsUndeformedWireframe.Image = global::PrePoMax.Properties.Resources.Undeformed_Wireframe;
            this.tsbResultsUndeformedWireframe.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbResultsUndeformedWireframe.Name = "tsbResultsUndeformedWireframe";
            this.tsbResultsUndeformedWireframe.Size = new System.Drawing.Size(23, 22);
            this.tsbResultsUndeformedWireframe.Text = "Show undeformed wireframe model";
            this.tsbResultsUndeformedWireframe.Click += new System.EventHandler(this.tsbResultsUndeformedWireframe_Click);
            // 
            // tsbResultsUndeformedSolid
            // 
            this.tsbResultsUndeformedSolid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbResultsUndeformedSolid.Image = global::PrePoMax.Properties.Resources.Undeformed_Solid;
            this.tsbResultsUndeformedSolid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbResultsUndeformedSolid.Name = "tsbResultsUndeformedSolid";
            this.tsbResultsUndeformedSolid.Size = new System.Drawing.Size(23, 22);
            this.tsbResultsUndeformedSolid.Text = "Show undeformed solid model";
            this.tsbResultsUndeformedSolid.Click += new System.EventHandler(this.tsbResultsUndeformedSolid_Click);
            // 
            // toolStripResultsSeparator1
            // 
            this.toolStripResultsSeparator1.Name = "toolStripResultsSeparator1";
            this.toolStripResultsSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbTransformation
            // 
            this.tsbTransformation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTransformation.Image = global::PrePoMax.Properties.Resources.Transformations;
            this.tsbTransformation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbTransformation.Name = "tsbTransformation";
            this.tsbTransformation.Size = new System.Drawing.Size(23, 22);
            this.tsbTransformation.Text = "Transformation";
            this.tsbTransformation.Click += new System.EventHandler(this.tsbTransformation_Click);
            // 
            // toolStripResultsSeparator2
            // 
            this.toolStripResultsSeparator2.Name = "toolStripResultsSeparator2";
            this.toolStripResultsSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbFirstStepIncrement
            // 
            this.tsbFirstStepIncrement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFirstStepIncrement.Image = global::PrePoMax.Properties.Resources.First;
            this.tsbFirstStepIncrement.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFirstStepIncrement.Name = "tsbFirstStepIncrement";
            this.tsbFirstStepIncrement.Size = new System.Drawing.Size(23, 22);
            this.tsbFirstStepIncrement.Text = "First increment";
            this.tsbFirstStepIncrement.Click += new System.EventHandler(this.tsbFirstStepIncrement_Click);
            // 
            // tsbPreviousStepIncrement
            // 
            this.tsbPreviousStepIncrement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPreviousStepIncrement.Image = global::PrePoMax.Properties.Resources.Previous;
            this.tsbPreviousStepIncrement.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPreviousStepIncrement.Name = "tsbPreviousStepIncrement";
            this.tsbPreviousStepIncrement.Size = new System.Drawing.Size(23, 22);
            this.tsbPreviousStepIncrement.Text = "Previous increment";
            this.tsbPreviousStepIncrement.ToolTipText = "Previous increment";
            this.tsbPreviousStepIncrement.Click += new System.EventHandler(this.tsbPreviousStepIncrement_Click);
            // 
            // tslStepIncrement
            // 
            this.tslStepIncrement.Name = "tslStepIncrement";
            this.tslStepIncrement.Size = new System.Drawing.Size(90, 22);
            this.tslStepIncrement.Text = "Step, Increment";
            // 
            // tscbStepAndIncrement
            // 
            this.tscbStepAndIncrement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbStepAndIncrement.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tscbStepAndIncrement.Name = "tscbStepAndIncrement";
            this.tscbStepAndIncrement.Size = new System.Drawing.Size(75, 25);
            this.tscbStepAndIncrement.ToolTipText = "Select increment";
            // 
            // tsbNextStepIncrement
            // 
            this.tsbNextStepIncrement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNextStepIncrement.Image = global::PrePoMax.Properties.Resources.Next;
            this.tsbNextStepIncrement.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNextStepIncrement.Name = "tsbNextStepIncrement";
            this.tsbNextStepIncrement.Size = new System.Drawing.Size(23, 22);
            this.tsbNextStepIncrement.Text = "Next increment";
            this.tsbNextStepIncrement.ToolTipText = "Next increment";
            this.tsbNextStepIncrement.Click += new System.EventHandler(this.tsbNextStepIncrement_Click);
            // 
            // tsbLastStepIncrement
            // 
            this.tsbLastStepIncrement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLastStepIncrement.Image = global::PrePoMax.Properties.Resources.Last;
            this.tsbLastStepIncrement.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLastStepIncrement.Name = "tsbLastStepIncrement";
            this.tsbLastStepIncrement.Size = new System.Drawing.Size(23, 22);
            this.tsbLastStepIncrement.Text = "Last increment";
            this.tsbLastStepIncrement.Click += new System.EventHandler(this.tsbLastStepIncrement_Click);
            // 
            // tsbAnimate
            // 
            this.tsbAnimate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAnimate.Image = global::PrePoMax.Properties.Resources.Animate;
            this.tsbAnimate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAnimate.Name = "tsbAnimate";
            this.tsbAnimate.Size = new System.Drawing.Size(23, 22);
            this.tsbAnimate.Text = "Animate";
            this.tsbAnimate.Click += new System.EventHandler(this.tsbAnimate_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // menuStripMain
            // 
            this.menuStripMain.DisableMouseButtons = false;
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.tsmiEdit,
            this.tsmiView,
            this.tsmiGeometry,
            this.tsmiMesh,
            this.tsmiModel,
            this.tsmiProperty,
            this.tsmiInteraction,
            this.tsmiAmplitude,
            this.tsmiInitialCondition,
            this.tsmiStepMenu,
            this.tsmiAnalysis,
            this.tsmiResults,
            this.tsmiTools,
            this.tsmiHelp});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(1264, 24);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStripMain";
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNew,
            this.tsmiOpen,
            this.tsmiOpenRecent,
            this.tsmiImportFile,
            this.tsmiDividerFile1,
            this.tsmiSave,
            this.tsmiSaveAs,
            this.tsmiDividerFile2,
            this.tsmiExport,
            this.tsmiDividerFile3,
            this.tsmiCloseCurrentResult,
            this.tsmiCloseAllResults,
            this.tsmiExit});
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.Size = new System.Drawing.Size(37, 20);
            this.tsmiFile.Text = "File";
            // 
            // tsmiNew
            // 
            this.tsmiNew.Image = global::PrePoMax.Properties.Resources.New;
            this.tsmiNew.Name = "tsmiNew";
            this.tsmiNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.tsmiNew.Size = new System.Drawing.Size(219, 22);
            this.tsmiNew.Text = "New                          ";
            this.tsmiNew.Click += new System.EventHandler(this.tsmiNew_Click);
            // 
            // tsmiOpen
            // 
            this.tsmiOpen.Image = global::PrePoMax.Properties.Resources.Open;
            this.tsmiOpen.Name = "tsmiOpen";
            this.tsmiOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.tsmiOpen.Size = new System.Drawing.Size(219, 22);
            this.tsmiOpen.Text = "Open";
            this.tsmiOpen.Click += new System.EventHandler(this.tsmiOpen_Click);
            // 
            // tsmiOpenRecent
            // 
            this.tsmiOpenRecent.Name = "tsmiOpenRecent";
            this.tsmiOpenRecent.Size = new System.Drawing.Size(219, 22);
            this.tsmiOpenRecent.Text = "Open Recent";
            // 
            // tsmiImportFile
            // 
            this.tsmiImportFile.Image = global::PrePoMax.Properties.Resources.Import;
            this.tsmiImportFile.Name = "tsmiImportFile";
            this.tsmiImportFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.tsmiImportFile.Size = new System.Drawing.Size(219, 22);
            this.tsmiImportFile.Text = "Import";
            this.tsmiImportFile.Click += new System.EventHandler(this.tsmiImportFile_Click);
            // 
            // tsmiDividerFile1
            // 
            this.tsmiDividerFile1.Name = "tsmiDividerFile1";
            this.tsmiDividerFile1.Size = new System.Drawing.Size(216, 6);
            // 
            // tsmiSave
            // 
            this.tsmiSave.Image = global::PrePoMax.Properties.Resources.Save;
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tsmiSave.Size = new System.Drawing.Size(219, 22);
            this.tsmiSave.Text = "Save";
            this.tsmiSave.Click += new System.EventHandler(this.tsmiSave_Click);
            // 
            // tsmiSaveAs
            // 
            this.tsmiSaveAs.Name = "tsmiSaveAs";
            this.tsmiSaveAs.Size = new System.Drawing.Size(219, 22);
            this.tsmiSaveAs.Text = "Save As";
            this.tsmiSaveAs.Click += new System.EventHandler(this.tsmiSaveAs_Click);
            // 
            // tsmiDividerFile2
            // 
            this.tsmiDividerFile2.Name = "tsmiDividerFile2";
            this.tsmiDividerFile2.Size = new System.Drawing.Size(216, 6);
            // 
            // tsmiExport
            // 
            this.tsmiExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiExportToStep,
            this.tsmiExportToBrep,
            this.tsmiExportToStereolitography,
            this.tsmiDividerExport1,
            this.tsmiExportToCalculix,
            this.tsmiExportToAbaqus,
            this.tsmiExportToMmgMesh,
            this.tsmiDividerExport2,
            this.tsmiExportToDeformedInp,
            this.tsmiExportToDeformedStl});
            this.tsmiExport.Image = global::PrePoMax.Properties.Resources.Export;
            this.tsmiExport.Name = "tsmiExport";
            this.tsmiExport.Size = new System.Drawing.Size(219, 22);
            this.tsmiExport.Text = "Export";
            // 
            // tsmiExportToStep
            // 
            this.tsmiExportToStep.Name = "tsmiExportToStep";
            this.tsmiExportToStep.Size = new System.Drawing.Size(222, 22);
            this.tsmiExportToStep.Text = "Step *.stp";
            this.tsmiExportToStep.Click += new System.EventHandler(this.tsmiExportToStep_Click);
            // 
            // tsmiExportToBrep
            // 
            this.tsmiExportToBrep.Name = "tsmiExportToBrep";
            this.tsmiExportToBrep.Size = new System.Drawing.Size(222, 22);
            this.tsmiExportToBrep.Text = "Brep *.brep";
            this.tsmiExportToBrep.Click += new System.EventHandler(this.tsmiExportToBrep_Click);
            // 
            // tsmiExportToStereolitography
            // 
            this.tsmiExportToStereolitography.Name = "tsmiExportToStereolitography";
            this.tsmiExportToStereolitography.Size = new System.Drawing.Size(222, 22);
            this.tsmiExportToStereolitography.Text = "Stereolitography *.stl";
            this.tsmiExportToStereolitography.Click += new System.EventHandler(this.tsmiExportToStereolitography_Click);
            // 
            // tsmiDividerExport1
            // 
            this.tsmiDividerExport1.Name = "tsmiDividerExport1";
            this.tsmiDividerExport1.Size = new System.Drawing.Size(219, 6);
            // 
            // tsmiExportToCalculix
            // 
            this.tsmiExportToCalculix.Name = "tsmiExportToCalculix";
            this.tsmiExportToCalculix.Size = new System.Drawing.Size(222, 22);
            this.tsmiExportToCalculix.Text = "Calculix *.inp";
            this.tsmiExportToCalculix.Click += new System.EventHandler(this.tsmiExportToCalculix_Click);
            // 
            // tsmiExportToAbaqus
            // 
            this.tsmiExportToAbaqus.Name = "tsmiExportToAbaqus";
            this.tsmiExportToAbaqus.Size = new System.Drawing.Size(222, 22);
            this.tsmiExportToAbaqus.Text = "Abaqus *.inp (experimental)";
            this.tsmiExportToAbaqus.Click += new System.EventHandler(this.tsmiExportToAbaqus_Click);
            // 
            // tsmiExportToMmgMesh
            // 
            this.tsmiExportToMmgMesh.Name = "tsmiExportToMmgMesh";
            this.tsmiExportToMmgMesh.Size = new System.Drawing.Size(222, 22);
            this.tsmiExportToMmgMesh.Text = "Mmg *.mesh";
            this.tsmiExportToMmgMesh.Click += new System.EventHandler(this.tsmiExportToMmgMesh_Click);
            // 
            // tsmiDividerExport2
            // 
            this.tsmiDividerExport2.Name = "tsmiDividerExport2";
            this.tsmiDividerExport2.Size = new System.Drawing.Size(219, 6);
            // 
            // tsmiExportToDeformedInp
            // 
            this.tsmiExportToDeformedInp.Name = "tsmiExportToDeformedInp";
            this.tsmiExportToDeformedInp.Size = new System.Drawing.Size(222, 22);
            this.tsmiExportToDeformedInp.Text = "Deformed mesh *.inp";
            this.tsmiExportToDeformedInp.Click += new System.EventHandler(this.tsmiExportToDeformedInp_Click);
            // 
            // tsmiExportToDeformedStl
            // 
            this.tsmiExportToDeformedStl.Name = "tsmiExportToDeformedStl";
            this.tsmiExportToDeformedStl.Size = new System.Drawing.Size(222, 22);
            this.tsmiExportToDeformedStl.Text = "Deformed visualization *.stl";
            this.tsmiExportToDeformedStl.Click += new System.EventHandler(this.tsmiExportToDeformedStl_Click);
            // 
            // tsmiDividerFile3
            // 
            this.tsmiDividerFile3.Name = "tsmiDividerFile3";
            this.tsmiDividerFile3.Size = new System.Drawing.Size(216, 6);
            // 
            // tsmiCloseCurrentResult
            // 
            this.tsmiCloseCurrentResult.Name = "tsmiCloseCurrentResult";
            this.tsmiCloseCurrentResult.Size = new System.Drawing.Size(219, 22);
            this.tsmiCloseCurrentResult.Text = "Close Current Result";
            this.tsmiCloseCurrentResult.Click += new System.EventHandler(this.tsmiCloseCurrentResult_Click);
            // 
            // tsmiCloseAllResults
            // 
            this.tsmiCloseAllResults.Name = "tsmiCloseAllResults";
            this.tsmiCloseAllResults.Size = new System.Drawing.Size(219, 22);
            this.tsmiCloseAllResults.Text = "Close All Results";
            this.tsmiCloseAllResults.Click += new System.EventHandler(this.tsmiCloseAllResults_Click);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.tsmiExit.Size = new System.Drawing.Size(219, 22);
            this.tsmiExit.Text = "Exit";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiUndo,
            this.tsmiRedo,
            this.tsmiDividerEdit1,
            this.tsmiViewHistory,
            this.tsmiRegenerate,
            this.tsmiRegenerateUsingOtherFiles,
            this.tsmiRegenerateForRemeshing});
            this.tsmiEdit.Name = "tsmiEdit";
            this.tsmiEdit.Size = new System.Drawing.Size(39, 20);
            this.tsmiEdit.Text = "Edit";
            // 
            // tsmiUndo
            // 
            this.tsmiUndo.Name = "tsmiUndo";
            this.tsmiUndo.Size = new System.Drawing.Size(225, 22);
            this.tsmiUndo.Text = "Undo";
            this.tsmiUndo.Click += new System.EventHandler(this.tsmiUndo_Click);
            // 
            // tsmiRedo
            // 
            this.tsmiRedo.Name = "tsmiRedo";
            this.tsmiRedo.Size = new System.Drawing.Size(225, 22);
            this.tsmiRedo.Text = "Redo";
            this.tsmiRedo.Click += new System.EventHandler(this.tsmiRedo_Click);
            // 
            // tsmiDividerEdit1
            // 
            this.tsmiDividerEdit1.Name = "tsmiDividerEdit1";
            this.tsmiDividerEdit1.Size = new System.Drawing.Size(222, 6);
            // 
            // tsmiViewHistory
            // 
            this.tsmiViewHistory.Name = "tsmiViewHistory";
            this.tsmiViewHistory.Size = new System.Drawing.Size(225, 22);
            this.tsmiViewHistory.Text = "View History";
            this.tsmiViewHistory.Click += new System.EventHandler(this.tsmiViewHistory_Click);
            // 
            // tsmiRegenerate
            // 
            this.tsmiRegenerate.Name = "tsmiRegenerate";
            this.tsmiRegenerate.Size = new System.Drawing.Size(225, 22);
            this.tsmiRegenerate.Text = "Regenerate";
            this.tsmiRegenerate.Click += new System.EventHandler(this.tsmiRegenerate_Click);
            // 
            // tsmiRegenerateUsingOtherFiles
            // 
            this.tsmiRegenerateUsingOtherFiles.Name = "tsmiRegenerateUsingOtherFiles";
            this.tsmiRegenerateUsingOtherFiles.Size = new System.Drawing.Size(225, 22);
            this.tsmiRegenerateUsingOtherFiles.Text = "Regenerate Using Other Files";
            this.tsmiRegenerateUsingOtherFiles.Click += new System.EventHandler(this.tsmiRegenerteUsingOtherFiles_Click);
            // 
            // tsmiRegenerateForRemeshing
            // 
            this.tsmiRegenerateForRemeshing.Name = "tsmiRegenerateForRemeshing";
            this.tsmiRegenerateForRemeshing.Size = new System.Drawing.Size(225, 22);
            this.tsmiRegenerateForRemeshing.Text = "Regenerate for Remeshing";
            this.tsmiRegenerateForRemeshing.Click += new System.EventHandler(this.tsmiRegenerateForRemeshing_Click);
            // 
            // tsmiView
            // 
            this.tsmiView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.standardViewsToolStripMenuItem,
            this.tsmiZoomToFit,
            this.tsmiDividerView1,
            this.tsmiShowWireframeEdges,
            this.tsmiShowElementEdges,
            this.tsmiShowModelEdges,
            this.tsmiShowNoEdges,
            this.tsmiDividerView2,
            this.tsmiSectionView,
            this.tsmiExplodedView,
            this.tsmiDividerView3,
            this.tsmiShowAllParts,
            this.tsmiHideAllParts,
            this.tsmiInvertVisibleParts,
            this.tsmiDividerView4,
            this.tsmiResultsUndeformed,
            this.tsmiResultsDeformed,
            this.tsmiResultsColorContours,
            this.tsmiDividerView5,
            this.tsmiColorAnnotations});
            this.tsmiView.Name = "tsmiView";
            this.tsmiView.Size = new System.Drawing.Size(44, 20);
            this.tsmiView.Text = "View";
            // 
            // standardViewsToolStripMenuItem
            // 
            this.standardViewsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFrontView,
            this.tsmiBackView,
            this.tsmiTopView,
            this.tsmiBottomView,
            this.tsmiLeftView,
            this.tsmiRightView,
            this.tsmiNormalView,
            this.tsmiVerticalView,
            this.tsmiIsometricView});
            this.standardViewsToolStripMenuItem.Name = "standardViewsToolStripMenuItem";
            this.standardViewsToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.standardViewsToolStripMenuItem.Text = "Standard Views";
            // 
            // tsmiFrontView
            // 
            this.tsmiFrontView.Image = global::PrePoMax.Properties.Resources.Front;
            this.tsmiFrontView.Name = "tsmiFrontView";
            this.tsmiFrontView.Size = new System.Drawing.Size(151, 22);
            this.tsmiFrontView.Text = "Front View";
            this.tsmiFrontView.Click += new System.EventHandler(this.tsmiFrontView_Click);
            // 
            // tsmiBackView
            // 
            this.tsmiBackView.Image = global::PrePoMax.Properties.Resources.Back;
            this.tsmiBackView.Name = "tsmiBackView";
            this.tsmiBackView.Size = new System.Drawing.Size(151, 22);
            this.tsmiBackView.Text = "Back View";
            this.tsmiBackView.Click += new System.EventHandler(this.tsmiBackView_Click);
            // 
            // tsmiTopView
            // 
            this.tsmiTopView.Image = global::PrePoMax.Properties.Resources.Top;
            this.tsmiTopView.Name = "tsmiTopView";
            this.tsmiTopView.Size = new System.Drawing.Size(151, 22);
            this.tsmiTopView.Text = "Top View";
            this.tsmiTopView.Click += new System.EventHandler(this.tsmiTopView_Click);
            // 
            // tsmiBottomView
            // 
            this.tsmiBottomView.Image = global::PrePoMax.Properties.Resources.Bottom;
            this.tsmiBottomView.Name = "tsmiBottomView";
            this.tsmiBottomView.Size = new System.Drawing.Size(151, 22);
            this.tsmiBottomView.Text = "Bottom View";
            this.tsmiBottomView.Click += new System.EventHandler(this.tsmiBottomView_Click);
            // 
            // tsmiLeftView
            // 
            this.tsmiLeftView.Image = global::PrePoMax.Properties.Resources.Left;
            this.tsmiLeftView.Name = "tsmiLeftView";
            this.tsmiLeftView.Size = new System.Drawing.Size(151, 22);
            this.tsmiLeftView.Text = "Left View";
            this.tsmiLeftView.Click += new System.EventHandler(this.tsmiLeftView_Click);
            // 
            // tsmiRightView
            // 
            this.tsmiRightView.Image = global::PrePoMax.Properties.Resources.Right;
            this.tsmiRightView.Name = "tsmiRightView";
            this.tsmiRightView.Size = new System.Drawing.Size(151, 22);
            this.tsmiRightView.Text = "Right View";
            this.tsmiRightView.Click += new System.EventHandler(this.tsmiRightView_Click);
            // 
            // tsmiNormalView
            // 
            this.tsmiNormalView.Image = global::PrePoMax.Properties.Resources.Normal;
            this.tsmiNormalView.Name = "tsmiNormalView";
            this.tsmiNormalView.Size = new System.Drawing.Size(151, 22);
            this.tsmiNormalView.Text = "Normal View";
            this.tsmiNormalView.Click += new System.EventHandler(this.tsmiNormalView_Click);
            // 
            // tsmiVerticalView
            // 
            this.tsmiVerticalView.Image = global::PrePoMax.Properties.Resources.Vertical;
            this.tsmiVerticalView.Name = "tsmiVerticalView";
            this.tsmiVerticalView.Size = new System.Drawing.Size(151, 22);
            this.tsmiVerticalView.Text = "Vertical View";
            this.tsmiVerticalView.Click += new System.EventHandler(this.tsmiVerticalView_Click);
            // 
            // tsmiIsometricView
            // 
            this.tsmiIsometricView.Image = global::PrePoMax.Properties.Resources.Isometric;
            this.tsmiIsometricView.Name = "tsmiIsometricView";
            this.tsmiIsometricView.Size = new System.Drawing.Size(151, 22);
            this.tsmiIsometricView.Text = "Isometric View";
            this.tsmiIsometricView.Click += new System.EventHandler(this.tsmiIsometricView_Click);
            // 
            // tsmiZoomToFit
            // 
            this.tsmiZoomToFit.Image = global::PrePoMax.Properties.Resources.ZoomToFit;
            this.tsmiZoomToFit.Name = "tsmiZoomToFit";
            this.tsmiZoomToFit.Size = new System.Drawing.Size(239, 22);
            this.tsmiZoomToFit.Text = "Zoom to Fit";
            this.tsmiZoomToFit.Click += new System.EventHandler(this.tsmiZoomToFit_Click);
            // 
            // tsmiDividerView1
            // 
            this.tsmiDividerView1.Name = "tsmiDividerView1";
            this.tsmiDividerView1.Size = new System.Drawing.Size(236, 6);
            // 
            // tsmiShowWireframeEdges
            // 
            this.tsmiShowWireframeEdges.Image = global::PrePoMax.Properties.Resources.Wireframe;
            this.tsmiShowWireframeEdges.Name = "tsmiShowWireframeEdges";
            this.tsmiShowWireframeEdges.Size = new System.Drawing.Size(239, 22);
            this.tsmiShowWireframeEdges.Text = "Wireframe";
            this.tsmiShowWireframeEdges.Click += new System.EventHandler(this.tsmiShowWireframeEdges_Click);
            // 
            // tsmiShowElementEdges
            // 
            this.tsmiShowElementEdges.Image = global::PrePoMax.Properties.Resources.ElementEdges;
            this.tsmiShowElementEdges.Name = "tsmiShowElementEdges";
            this.tsmiShowElementEdges.Size = new System.Drawing.Size(239, 22);
            this.tsmiShowElementEdges.Text = "Show Element Edges";
            this.tsmiShowElementEdges.Click += new System.EventHandler(this.tsmiShowElementEdges_Click);
            // 
            // tsmiShowModelEdges
            // 
            this.tsmiShowModelEdges.Image = global::PrePoMax.Properties.Resources.ModelEdges;
            this.tsmiShowModelEdges.Name = "tsmiShowModelEdges";
            this.tsmiShowModelEdges.Size = new System.Drawing.Size(239, 22);
            this.tsmiShowModelEdges.Text = "Show Model Edges";
            this.tsmiShowModelEdges.Click += new System.EventHandler(this.tsmiShowModelEdges_Click);
            // 
            // tsmiShowNoEdges
            // 
            this.tsmiShowNoEdges.Image = global::PrePoMax.Properties.Resources.NoEdges;
            this.tsmiShowNoEdges.Name = "tsmiShowNoEdges";
            this.tsmiShowNoEdges.Size = new System.Drawing.Size(239, 22);
            this.tsmiShowNoEdges.Text = "No Edges";
            this.tsmiShowNoEdges.Click += new System.EventHandler(this.tsmiShowNoEdges_Click);
            // 
            // tsmiDividerView2
            // 
            this.tsmiDividerView2.Name = "tsmiDividerView2";
            this.tsmiDividerView2.Size = new System.Drawing.Size(236, 6);
            // 
            // tsmiSectionView
            // 
            this.tsmiSectionView.Image = global::PrePoMax.Properties.Resources.SectionView;
            this.tsmiSectionView.Name = "tsmiSectionView";
            this.tsmiSectionView.Size = new System.Drawing.Size(239, 22);
            this.tsmiSectionView.Text = "Section View";
            this.tsmiSectionView.Click += new System.EventHandler(this.tsmiSectionView_Click);
            // 
            // tsmiExplodedView
            // 
            this.tsmiExplodedView.Image = global::PrePoMax.Properties.Resources.Explode;
            this.tsmiExplodedView.Name = "tsmiExplodedView";
            this.tsmiExplodedView.Size = new System.Drawing.Size(239, 22);
            this.tsmiExplodedView.Text = "Exploded View";
            this.tsmiExplodedView.Click += new System.EventHandler(this.tsmiExplodedView_Click);
            // 
            // tsmiDividerView3
            // 
            this.tsmiDividerView3.Name = "tsmiDividerView3";
            this.tsmiDividerView3.Size = new System.Drawing.Size(236, 6);
            // 
            // tsmiShowAllParts
            // 
            this.tsmiShowAllParts.Image = global::PrePoMax.Properties.Resources.Show;
            this.tsmiShowAllParts.Name = "tsmiShowAllParts";
            this.tsmiShowAllParts.Size = new System.Drawing.Size(239, 22);
            this.tsmiShowAllParts.Text = "Show All";
            this.tsmiShowAllParts.Click += new System.EventHandler(this.tsmiShowAllParts_Click);
            // 
            // tsmiHideAllParts
            // 
            this.tsmiHideAllParts.Image = global::PrePoMax.Properties.Resources.Hide;
            this.tsmiHideAllParts.Name = "tsmiHideAllParts";
            this.tsmiHideAllParts.Size = new System.Drawing.Size(239, 22);
            this.tsmiHideAllParts.Text = "Hide All";
            this.tsmiHideAllParts.Click += new System.EventHandler(this.tsmiHideAllParts_Click);
            // 
            // tsmiInvertVisibleParts
            // 
            this.tsmiInvertVisibleParts.Image = global::PrePoMax.Properties.Resources.InvertHideShow;
            this.tsmiInvertVisibleParts.Name = "tsmiInvertVisibleParts";
            this.tsmiInvertVisibleParts.Size = new System.Drawing.Size(239, 22);
            this.tsmiInvertVisibleParts.Text = "Invert Visible Parts";
            this.tsmiInvertVisibleParts.Click += new System.EventHandler(this.tsmiInvertVisibleParts_Click);
            // 
            // tsmiDividerView4
            // 
            this.tsmiDividerView4.Name = "tsmiDividerView4";
            this.tsmiDividerView4.Size = new System.Drawing.Size(236, 6);
            // 
            // tsmiResultsUndeformed
            // 
            this.tsmiResultsUndeformed.Image = global::PrePoMax.Properties.Resources.Undeformed;
            this.tsmiResultsUndeformed.Name = "tsmiResultsUndeformed";
            this.tsmiResultsUndeformed.Size = new System.Drawing.Size(239, 22);
            this.tsmiResultsUndeformed.Text = "Undeformed";
            this.tsmiResultsUndeformed.Click += new System.EventHandler(this.tsmiResultsUndeformed_Click);
            // 
            // tsmiResultsDeformed
            // 
            this.tsmiResultsDeformed.Image = global::PrePoMax.Properties.Resources.Deformed;
            this.tsmiResultsDeformed.Name = "tsmiResultsDeformed";
            this.tsmiResultsDeformed.Size = new System.Drawing.Size(239, 22);
            this.tsmiResultsDeformed.Text = "Deformed";
            this.tsmiResultsDeformed.Click += new System.EventHandler(this.tsmiResultsDeformed_Click);
            // 
            // tsmiResultsColorContours
            // 
            this.tsmiResultsColorContours.Image = global::PrePoMax.Properties.Resources.Color_contours;
            this.tsmiResultsColorContours.Name = "tsmiResultsColorContours";
            this.tsmiResultsColorContours.Size = new System.Drawing.Size(239, 22);
            this.tsmiResultsColorContours.Text = "Deformed With Color Contours";
            this.tsmiResultsColorContours.Click += new System.EventHandler(this.tsmiResultsColorContours_Click);
            // 
            // tsmiDividerView5
            // 
            this.tsmiDividerView5.Name = "tsmiDividerView5";
            this.tsmiDividerView5.Size = new System.Drawing.Size(236, 6);
            // 
            // tsmiColorAnnotations
            // 
            this.tsmiColorAnnotations.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAnnotateFaceOrientations,
            this.tsmiDividerColorAnnotations1,
            this.tsmiAnnotateParts,
            this.tsmiAnnotateMaterials,
            this.tsmiAnnotateSections,
            this.tsmiAnnotateSectionThicknesses,
            this.tsmiDividerColorAnnotations2,
            this.tsmiAnnotateAllSymbols,
            this.tsmiAnnotateReferencePoints,
            this.tsmiAnnotateConstraints,
            this.tsmiAnnotateContactPairs,
            this.tsmiAnnotateBCs,
            this.tsmiAnnotateLoads});
            this.tsmiColorAnnotations.Name = "tsmiColorAnnotations";
            this.tsmiColorAnnotations.Size = new System.Drawing.Size(239, 22);
            this.tsmiColorAnnotations.Text = "Color Annotations";
            // 
            // tsmiAnnotateFaceOrientations
            // 
            this.tsmiAnnotateFaceOrientations.Name = "tsmiAnnotateFaceOrientations";
            this.tsmiAnnotateFaceOrientations.Size = new System.Drawing.Size(178, 22);
            this.tsmiAnnotateFaceOrientations.Text = "Face Orientations";
            this.tsmiAnnotateFaceOrientations.Click += new System.EventHandler(this.tsmiAnnotateFaceOrientations_Click);
            // 
            // tsmiDividerColorAnnotations1
            // 
            this.tsmiDividerColorAnnotations1.Name = "tsmiDividerColorAnnotations1";
            this.tsmiDividerColorAnnotations1.Size = new System.Drawing.Size(175, 6);
            // 
            // tsmiAnnotateParts
            // 
            this.tsmiAnnotateParts.Name = "tsmiAnnotateParts";
            this.tsmiAnnotateParts.Size = new System.Drawing.Size(178, 22);
            this.tsmiAnnotateParts.Text = "Parts";
            this.tsmiAnnotateParts.Click += new System.EventHandler(this.tsmiAnnotateParts_Click);
            // 
            // tsmiAnnotateMaterials
            // 
            this.tsmiAnnotateMaterials.Name = "tsmiAnnotateMaterials";
            this.tsmiAnnotateMaterials.Size = new System.Drawing.Size(178, 22);
            this.tsmiAnnotateMaterials.Text = "Materials";
            this.tsmiAnnotateMaterials.Click += new System.EventHandler(this.tsmiAnnotateMaterials_Click);
            // 
            // tsmiAnnotateSections
            // 
            this.tsmiAnnotateSections.Name = "tsmiAnnotateSections";
            this.tsmiAnnotateSections.Size = new System.Drawing.Size(178, 22);
            this.tsmiAnnotateSections.Text = "Sections";
            this.tsmiAnnotateSections.Click += new System.EventHandler(this.tsmiAnnotateSections_Click);
            // 
            // tsmiAnnotateSectionThicknesses
            // 
            this.tsmiAnnotateSectionThicknesses.Name = "tsmiAnnotateSectionThicknesses";
            this.tsmiAnnotateSectionThicknesses.Size = new System.Drawing.Size(178, 22);
            this.tsmiAnnotateSectionThicknesses.Text = "Section Thicknesses";
            this.tsmiAnnotateSectionThicknesses.Click += new System.EventHandler(this.tsmiAnnotateSectionThicknesses_Click);
            // 
            // tsmiDividerColorAnnotations2
            // 
            this.tsmiDividerColorAnnotations2.Name = "tsmiDividerColorAnnotations2";
            this.tsmiDividerColorAnnotations2.Size = new System.Drawing.Size(175, 6);
            // 
            // tsmiAnnotateAllSymbols
            // 
            this.tsmiAnnotateAllSymbols.Name = "tsmiAnnotateAllSymbols";
            this.tsmiAnnotateAllSymbols.Size = new System.Drawing.Size(178, 22);
            this.tsmiAnnotateAllSymbols.Text = "All Symbols";
            this.tsmiAnnotateAllSymbols.Click += new System.EventHandler(this.tsmiAnnotateAllSymbols_Click);
            // 
            // tsmiAnnotateReferencePoints
            // 
            this.tsmiAnnotateReferencePoints.Name = "tsmiAnnotateReferencePoints";
            this.tsmiAnnotateReferencePoints.Size = new System.Drawing.Size(178, 22);
            this.tsmiAnnotateReferencePoints.Text = "Reference Points";
            this.tsmiAnnotateReferencePoints.Click += new System.EventHandler(this.tsmiAnnotateReferencePoints_Click);
            // 
            // tsmiAnnotateConstraints
            // 
            this.tsmiAnnotateConstraints.Name = "tsmiAnnotateConstraints";
            this.tsmiAnnotateConstraints.Size = new System.Drawing.Size(178, 22);
            this.tsmiAnnotateConstraints.Text = "Constraints";
            this.tsmiAnnotateConstraints.Click += new System.EventHandler(this.tsmiAnnotateConstraints_Click);
            // 
            // tsmiAnnotateContactPairs
            // 
            this.tsmiAnnotateContactPairs.Name = "tsmiAnnotateContactPairs";
            this.tsmiAnnotateContactPairs.Size = new System.Drawing.Size(178, 22);
            this.tsmiAnnotateContactPairs.Text = "Contact Pairs";
            this.tsmiAnnotateContactPairs.Click += new System.EventHandler(this.tsmiAnnotateContactPairs_Click);
            // 
            // tsmiAnnotateBCs
            // 
            this.tsmiAnnotateBCs.Name = "tsmiAnnotateBCs";
            this.tsmiAnnotateBCs.Size = new System.Drawing.Size(178, 22);
            this.tsmiAnnotateBCs.Text = "BCs";
            this.tsmiAnnotateBCs.Click += new System.EventHandler(this.tsmiAnnotateBCs_Click);
            // 
            // tsmiAnnotateLoads
            // 
            this.tsmiAnnotateLoads.Name = "tsmiAnnotateLoads";
            this.tsmiAnnotateLoads.Size = new System.Drawing.Size(178, 22);
            this.tsmiAnnotateLoads.Text = "Loads";
            this.tsmiAnnotateLoads.Click += new System.EventHandler(this.tsmiAnnotateLoads_Click);
            // 
            // tsmiGeometry
            // 
            this.tsmiGeometry.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiGeometryPart,
            this.cADPartToolStripMenuItem,
            this.tsmiStlPart,
            this.tsmiDividerGeometry1,
            this.tsmiCreateAndImportCompoundPart,
            this.tsmiRegenerateCompoundPart,
            this.tsmiSwapGeometryPartGeometries,
            this.tsmiDividerGeometry2,
            this.tsmiGeometryAnalyze});
            this.tsmiGeometry.Name = "tsmiGeometry";
            this.tsmiGeometry.Size = new System.Drawing.Size(71, 20);
            this.tsmiGeometry.Text = "Geometry";
            // 
            // tsmiGeometryPart
            // 
            this.tsmiGeometryPart.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEditGeometryPart,
            this.tsmiTransformGeometryParts,
            this.tsmiDividerGeomPart1,
            this.tsmiCopyGeometryPartsToResults,
            this.tsmiDividerGeomPart2,
            this.tsmiHideGeometryParts,
            this.tsmiShowGeometryParts,
            this.tsmiShowOnlyGeometryParts,
            this.tsmiSetTransparencyForGeometryParts,
            this.tsmiDividerGeomPart3,
            this.tsmiDeleteGeometryParts});
            this.tsmiGeometryPart.Name = "tsmiGeometryPart";
            this.tsmiGeometryPart.Size = new System.Drawing.Size(221, 22);
            this.tsmiGeometryPart.Text = "Part";
            // 
            // tsmiEditGeometryPart
            // 
            this.tsmiEditGeometryPart.Name = "tsmiEditGeometryPart";
            this.tsmiEditGeometryPart.Size = new System.Drawing.Size(211, 22);
            this.tsmiEditGeometryPart.Text = "Edit";
            this.tsmiEditGeometryPart.Click += new System.EventHandler(this.tsmiEditGeometryPart_Click);
            // 
            // tsmiTransformGeometryParts
            // 
            this.tsmiTransformGeometryParts.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiScaleGeometryParts});
            this.tsmiTransformGeometryParts.Name = "tsmiTransformGeometryParts";
            this.tsmiTransformGeometryParts.Size = new System.Drawing.Size(211, 22);
            this.tsmiTransformGeometryParts.Text = "Transform";
            // 
            // tsmiScaleGeometryParts
            // 
            this.tsmiScaleGeometryParts.Name = "tsmiScaleGeometryParts";
            this.tsmiScaleGeometryParts.Size = new System.Drawing.Size(101, 22);
            this.tsmiScaleGeometryParts.Text = "Scale";
            this.tsmiScaleGeometryParts.Click += new System.EventHandler(this.tsmiScaleGeometryParts_Click);
            // 
            // tsmiDividerGeomPart1
            // 
            this.tsmiDividerGeomPart1.Name = "tsmiDividerGeomPart1";
            this.tsmiDividerGeomPart1.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiCopyGeometryPartsToResults
            // 
            this.tsmiCopyGeometryPartsToResults.Name = "tsmiCopyGeometryPartsToResults";
            this.tsmiCopyGeometryPartsToResults.Size = new System.Drawing.Size(211, 22);
            this.tsmiCopyGeometryPartsToResults.Text = "Copy Geometry to Results";
            this.tsmiCopyGeometryPartsToResults.Click += new System.EventHandler(this.tsmiCopyGeometryPartsToResults_Click);
            // 
            // tsmiDividerGeomPart2
            // 
            this.tsmiDividerGeomPart2.Name = "tsmiDividerGeomPart2";
            this.tsmiDividerGeomPart2.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiHideGeometryParts
            // 
            this.tsmiHideGeometryParts.Image = global::PrePoMax.Properties.Resources.Hide;
            this.tsmiHideGeometryParts.Name = "tsmiHideGeometryParts";
            this.tsmiHideGeometryParts.Size = new System.Drawing.Size(211, 22);
            this.tsmiHideGeometryParts.Text = "Hide";
            this.tsmiHideGeometryParts.Click += new System.EventHandler(this.tsmiHideGeometryParts_Click);
            // 
            // tsmiShowGeometryParts
            // 
            this.tsmiShowGeometryParts.Image = global::PrePoMax.Properties.Resources.Show;
            this.tsmiShowGeometryParts.Name = "tsmiShowGeometryParts";
            this.tsmiShowGeometryParts.Size = new System.Drawing.Size(211, 22);
            this.tsmiShowGeometryParts.Text = "Show";
            this.tsmiShowGeometryParts.Click += new System.EventHandler(this.tsmiShowGeometryParts_Click);
            // 
            // tsmiShowOnlyGeometryParts
            // 
            this.tsmiShowOnlyGeometryParts.Image = global::PrePoMax.Properties.Resources.Show;
            this.tsmiShowOnlyGeometryParts.Name = "tsmiShowOnlyGeometryParts";
            this.tsmiShowOnlyGeometryParts.Size = new System.Drawing.Size(211, 22);
            this.tsmiShowOnlyGeometryParts.Text = "Show Only";
            this.tsmiShowOnlyGeometryParts.Click += new System.EventHandler(this.tsmiShowOnlyGeometryParts_Click);
            // 
            // tsmiSetTransparencyForGeometryParts
            // 
            this.tsmiSetTransparencyForGeometryParts.Name = "tsmiSetTransparencyForGeometryParts";
            this.tsmiSetTransparencyForGeometryParts.Size = new System.Drawing.Size(211, 22);
            this.tsmiSetTransparencyForGeometryParts.Text = "Set Transparency";
            this.tsmiSetTransparencyForGeometryParts.Click += new System.EventHandler(this.tsmiSetTransparencyForGeometryParts_Click);
            // 
            // tsmiDividerGeomPart3
            // 
            this.tsmiDividerGeomPart3.Name = "tsmiDividerGeomPart3";
            this.tsmiDividerGeomPart3.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiDeleteGeometryParts
            // 
            this.tsmiDeleteGeometryParts.Name = "tsmiDeleteGeometryParts";
            this.tsmiDeleteGeometryParts.Size = new System.Drawing.Size(211, 22);
            this.tsmiDeleteGeometryParts.Text = "Delete";
            this.tsmiDeleteGeometryParts.Click += new System.EventHandler(this.tsmiDeleteGeometryParts_Click);
            // 
            // cADPartToolStripMenuItem
            // 
            this.cADPartToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFlipFaceNormalCAD,
            this.tsmiSplitAFaceUsingTwoPoints});
            this.cADPartToolStripMenuItem.Name = "cADPartToolStripMenuItem";
            this.cADPartToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.cADPartToolStripMenuItem.Text = "CAD Part";
            // 
            // tsmiFlipFaceNormalCAD
            // 
            this.tsmiFlipFaceNormalCAD.Name = "tsmiFlipFaceNormalCAD";
            this.tsmiFlipFaceNormalCAD.Size = new System.Drawing.Size(226, 22);
            this.tsmiFlipFaceNormalCAD.Text = "Flip Face Normal";
            this.tsmiFlipFaceNormalCAD.Click += new System.EventHandler(this.tsmiFlipFaceNormalCAD_Click);
            // 
            // tsmiSplitAFaceUsingTwoPoints
            // 
            this.tsmiSplitAFaceUsingTwoPoints.Name = "tsmiSplitAFaceUsingTwoPoints";
            this.tsmiSplitAFaceUsingTwoPoints.Size = new System.Drawing.Size(226, 22);
            this.tsmiSplitAFaceUsingTwoPoints.Text = "Split a Face Using Two Points";
            this.tsmiSplitAFaceUsingTwoPoints.Click += new System.EventHandler(this.tsmiSplitAFaceUsingTwoPoints_Click);
            // 
            // tsmiStlPart
            // 
            this.tsmiStlPart.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFindEdgesByAngleForGeometryParts,
            this.tsmiFlipStlPartSurfacesNormal,
            this.tsmiSmoothPart,
            this.tsmiCropWithCylinder,
            this.tsmiCropWithCube});
            this.tsmiStlPart.Name = "tsmiStlPart";
            this.tsmiStlPart.Size = new System.Drawing.Size(221, 22);
            this.tsmiStlPart.Text = "Stl Part";
            // 
            // tsmiFindEdgesByAngleForGeometryParts
            // 
            this.tsmiFindEdgesByAngleForGeometryParts.Name = "tsmiFindEdgesByAngleForGeometryParts";
            this.tsmiFindEdgesByAngleForGeometryParts.Size = new System.Drawing.Size(218, 22);
            this.tsmiFindEdgesByAngleForGeometryParts.Text = "Find Model Edges by Angle";
            this.tsmiFindEdgesByAngleForGeometryParts.Click += new System.EventHandler(this.tsmiFindEdgesByAngleForGeometryParts_Click);
            // 
            // tsmiFlipStlPartSurfacesNormal
            // 
            this.tsmiFlipStlPartSurfacesNormal.Name = "tsmiFlipStlPartSurfacesNormal";
            this.tsmiFlipStlPartSurfacesNormal.Size = new System.Drawing.Size(218, 22);
            this.tsmiFlipStlPartSurfacesNormal.Text = "Flip Part Surfaces Normal";
            this.tsmiFlipStlPartSurfacesNormal.Click += new System.EventHandler(this.tsmiFlipStlPartSurfacesNormal_Click);
            // 
            // tsmiSmoothPart
            // 
            this.tsmiSmoothPart.Name = "tsmiSmoothPart";
            this.tsmiSmoothPart.Size = new System.Drawing.Size(218, 22);
            this.tsmiSmoothPart.Text = "Smooth Part";
            this.tsmiSmoothPart.Click += new System.EventHandler(this.tsmiSmoothPart_Click);
            // 
            // tsmiCropWithCylinder
            // 
            this.tsmiCropWithCylinder.Name = "tsmiCropWithCylinder";
            this.tsmiCropWithCylinder.Size = new System.Drawing.Size(218, 22);
            this.tsmiCropWithCylinder.Text = "Crop With Cylinder";
            this.tsmiCropWithCylinder.Click += new System.EventHandler(this.tsmiCropWithCylinder_Click);
            // 
            // tsmiCropWithCube
            // 
            this.tsmiCropWithCube.Name = "tsmiCropWithCube";
            this.tsmiCropWithCube.Size = new System.Drawing.Size(218, 22);
            this.tsmiCropWithCube.Text = "Crop With Cube";
            this.tsmiCropWithCube.Click += new System.EventHandler(this.tsmiCropWithCube_Click);
            // 
            // tsmiDividerGeometry1
            // 
            this.tsmiDividerGeometry1.Name = "tsmiDividerGeometry1";
            this.tsmiDividerGeometry1.Size = new System.Drawing.Size(218, 6);
            // 
            // tsmiCreateAndImportCompoundPart
            // 
            this.tsmiCreateAndImportCompoundPart.Name = "tsmiCreateAndImportCompoundPart";
            this.tsmiCreateAndImportCompoundPart.Size = new System.Drawing.Size(221, 22);
            this.tsmiCreateAndImportCompoundPart.Text = "Create Compound Part";
            this.tsmiCreateAndImportCompoundPart.Click += new System.EventHandler(this.tsmiCreateAndImportCompoundPart_Click);
            // 
            // tsmiRegenerateCompoundPart
            // 
            this.tsmiRegenerateCompoundPart.Name = "tsmiRegenerateCompoundPart";
            this.tsmiRegenerateCompoundPart.Size = new System.Drawing.Size(221, 22);
            this.tsmiRegenerateCompoundPart.Text = "Regenerate Compound Part";
            this.tsmiRegenerateCompoundPart.Click += new System.EventHandler(this.tsmiRegenerateCompoundPart_Click);
            // 
            // tsmiSwapGeometryPartGeometries
            // 
            this.tsmiSwapGeometryPartGeometries.Name = "tsmiSwapGeometryPartGeometries";
            this.tsmiSwapGeometryPartGeometries.Size = new System.Drawing.Size(221, 22);
            this.tsmiSwapGeometryPartGeometries.Text = "Swap Part Geometries";
            this.tsmiSwapGeometryPartGeometries.Click += new System.EventHandler(this.tsmiSwapGeometryPartGeometries_Click);
            // 
            // tsmiDividerGeometry2
            // 
            this.tsmiDividerGeometry2.Name = "tsmiDividerGeometry2";
            this.tsmiDividerGeometry2.Size = new System.Drawing.Size(218, 6);
            // 
            // tsmiGeometryAnalyze
            // 
            this.tsmiGeometryAnalyze.Name = "tsmiGeometryAnalyze";
            this.tsmiGeometryAnalyze.Size = new System.Drawing.Size(221, 22);
            this.tsmiGeometryAnalyze.Text = "Analyze";
            this.tsmiGeometryAnalyze.Click += new System.EventHandler(this.tsmiGeometryAnalyze_Click);
            // 
            // tsmiMesh
            // 
            this.tsmiMesh.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMeshingParameters,
            this.tsmiMeshRefinement,
            this.tsmiPreviewEdgeMesh,
            this.tsmiCreateMesh});
            this.tsmiMesh.Name = "tsmiMesh";
            this.tsmiMesh.Size = new System.Drawing.Size(48, 20);
            this.tsmiMesh.Text = "Mesh";
            // 
            // tsmiMeshingParameters
            // 
            this.tsmiMeshingParameters.Name = "tsmiMeshingParameters";
            this.tsmiMeshingParameters.Size = new System.Drawing.Size(182, 22);
            this.tsmiMeshingParameters.Text = "Meshing Parameters";
            this.tsmiMeshingParameters.Click += new System.EventHandler(this.tsmiMeshingParameters_Click);
            // 
            // tsmiMeshRefinement
            // 
            this.tsmiMeshRefinement.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateMeshRefinement,
            this.tsmiEditMeshRefinement,
            this.tsmiDividerMeshRefinement1,
            this.tsmiDeleteMeshRefinement});
            this.tsmiMeshRefinement.Image = global::PrePoMax.Properties.Resources.Mesh_refinement;
            this.tsmiMeshRefinement.Name = "tsmiMeshRefinement";
            this.tsmiMeshRefinement.Size = new System.Drawing.Size(182, 22);
            this.tsmiMeshRefinement.Text = "Mesh Refinement";
            // 
            // tsmiCreateMeshRefinement
            // 
            this.tsmiCreateMeshRefinement.Name = "tsmiCreateMeshRefinement";
            this.tsmiCreateMeshRefinement.Size = new System.Drawing.Size(108, 22);
            this.tsmiCreateMeshRefinement.Text = "Create";
            this.tsmiCreateMeshRefinement.Click += new System.EventHandler(this.tsmiCreateMeshRefinement_Click);
            // 
            // tsmiEditMeshRefinement
            // 
            this.tsmiEditMeshRefinement.Name = "tsmiEditMeshRefinement";
            this.tsmiEditMeshRefinement.Size = new System.Drawing.Size(108, 22);
            this.tsmiEditMeshRefinement.Text = "Edit";
            this.tsmiEditMeshRefinement.Click += new System.EventHandler(this.tsmiEditMeshRefinement_Click);
            // 
            // tsmiDividerMeshRefinement1
            // 
            this.tsmiDividerMeshRefinement1.Name = "tsmiDividerMeshRefinement1";
            this.tsmiDividerMeshRefinement1.Size = new System.Drawing.Size(105, 6);
            // 
            // tsmiDeleteMeshRefinement
            // 
            this.tsmiDeleteMeshRefinement.Name = "tsmiDeleteMeshRefinement";
            this.tsmiDeleteMeshRefinement.Size = new System.Drawing.Size(108, 22);
            this.tsmiDeleteMeshRefinement.Text = "Delete";
            this.tsmiDeleteMeshRefinement.Click += new System.EventHandler(this.tsmiDeleteMeshRefinement_Click);
            // 
            // tsmiPreviewEdgeMesh
            // 
            this.tsmiPreviewEdgeMesh.Name = "tsmiPreviewEdgeMesh";
            this.tsmiPreviewEdgeMesh.Size = new System.Drawing.Size(182, 22);
            this.tsmiPreviewEdgeMesh.Text = "Preview Edge Mesh";
            this.tsmiPreviewEdgeMesh.Click += new System.EventHandler(this.tsmiPreviewEdgeMesh_Click);
            // 
            // tsmiCreateMesh
            // 
            this.tsmiCreateMesh.Image = global::PrePoMax.Properties.Resources.Part;
            this.tsmiCreateMesh.Name = "tsmiCreateMesh";
            this.tsmiCreateMesh.Size = new System.Drawing.Size(182, 22);
            this.tsmiCreateMesh.Text = "Create Mesh";
            this.tsmiCreateMesh.Click += new System.EventHandler(this.tsmiCreateMesh_Click);
            // 
            // tsmiModel
            // 
            this.tsmiModel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEditModel,
            this.tsmiEditCalculiXKeywords,
            this.tsmiToolsParts,
            this.tsmiDividerModel1,
            this.tsmiNode,
            this.tsmiElement,
            this.tsmiPart,
            this.tsmiNodeSet,
            this.tsmiElementSet,
            this.tsmiSurface,
            this.tsmiReferencePointTool});
            this.tsmiModel.Name = "tsmiModel";
            this.tsmiModel.Size = new System.Drawing.Size(53, 20);
            this.tsmiModel.Text = "Model";
            // 
            // tsmiEditModel
            // 
            this.tsmiEditModel.Name = "tsmiEditModel";
            this.tsmiEditModel.Size = new System.Drawing.Size(194, 22);
            this.tsmiEditModel.Text = "Edit";
            this.tsmiEditModel.Click += new System.EventHandler(this.tsmiEditModel_Click);
            // 
            // tsmiEditCalculiXKeywords
            // 
            this.tsmiEditCalculiXKeywords.Name = "tsmiEditCalculiXKeywords";
            this.tsmiEditCalculiXKeywords.Size = new System.Drawing.Size(194, 22);
            this.tsmiEditCalculiXKeywords.Text = "Edit CalculiX Keywords";
            this.tsmiEditCalculiXKeywords.Click += new System.EventHandler(this.tsmiEditCalculiXKeywords_Click);
            // 
            // tsmiToolsParts
            // 
            this.tsmiToolsParts.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateBoundaryLayer,
            this.tsmiFindEdgesByAngleForModelParts,
            this.tsmiRemeshElements,
            this.tsmiUpdateNodalCoordinatesFromFile});
            this.tsmiToolsParts.Name = "tsmiToolsParts";
            this.tsmiToolsParts.Size = new System.Drawing.Size(194, 22);
            this.tsmiToolsParts.Text = "Tools";
            // 
            // tsmiCreateBoundaryLayer
            // 
            this.tsmiCreateBoundaryLayer.Name = "tsmiCreateBoundaryLayer";
            this.tsmiCreateBoundaryLayer.Size = new System.Drawing.Size(266, 22);
            this.tsmiCreateBoundaryLayer.Text = "Create Boundary Layer";
            this.tsmiCreateBoundaryLayer.Click += new System.EventHandler(this.tsmiCreateBoundaryLayer_Click);
            // 
            // tsmiFindEdgesByAngleForModelParts
            // 
            this.tsmiFindEdgesByAngleForModelParts.Name = "tsmiFindEdgesByAngleForModelParts";
            this.tsmiFindEdgesByAngleForModelParts.Size = new System.Drawing.Size(266, 22);
            this.tsmiFindEdgesByAngleForModelParts.Text = "Find Model Edges By Angle";
            this.tsmiFindEdgesByAngleForModelParts.Click += new System.EventHandler(this.tsmiFindEdgesByAngleForModelParts_Click);
            // 
            // tsmiRemeshElements
            // 
            this.tsmiRemeshElements.Name = "tsmiRemeshElements";
            this.tsmiRemeshElements.Size = new System.Drawing.Size(266, 22);
            this.tsmiRemeshElements.Text = "Remesh Elements";
            this.tsmiRemeshElements.Click += new System.EventHandler(this.tsmiRemeshElements_Click);
            // 
            // tsmiUpdateNodalCoordinatesFromFile
            // 
            this.tsmiUpdateNodalCoordinatesFromFile.Name = "tsmiUpdateNodalCoordinatesFromFile";
            this.tsmiUpdateNodalCoordinatesFromFile.Size = new System.Drawing.Size(266, 22);
            this.tsmiUpdateNodalCoordinatesFromFile.Text = "Update Nodal Coordinates From File";
            this.tsmiUpdateNodalCoordinatesFromFile.Click += new System.EventHandler(this.tsmiUpdateNodalCoordinatesFromFile_Click);
            // 
            // tsmiDividerModel1
            // 
            this.tsmiDividerModel1.Name = "tsmiDividerModel1";
            this.tsmiDividerModel1.Size = new System.Drawing.Size(191, 6);
            // 
            // tsmiNode
            // 
            this.tsmiNode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiRenumberAllNodes});
            this.tsmiNode.Name = "tsmiNode";
            this.tsmiNode.Size = new System.Drawing.Size(194, 22);
            this.tsmiNode.Text = "Node";
            // 
            // tsmiRenumberAllNodes
            // 
            this.tsmiRenumberAllNodes.Name = "tsmiRenumberAllNodes";
            this.tsmiRenumberAllNodes.Size = new System.Drawing.Size(146, 22);
            this.tsmiRenumberAllNodes.Text = "Renumber All";
            this.tsmiRenumberAllNodes.Click += new System.EventHandler(this.tsmiRenumberAllNodes_Click);
            // 
            // tsmiElement
            // 
            this.tsmiElement.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiRenumberAllElements});
            this.tsmiElement.Name = "tsmiElement";
            this.tsmiElement.Size = new System.Drawing.Size(194, 22);
            this.tsmiElement.Text = "Element";
            // 
            // tsmiRenumberAllElements
            // 
            this.tsmiRenumberAllElements.Name = "tsmiRenumberAllElements";
            this.tsmiRenumberAllElements.Size = new System.Drawing.Size(146, 22);
            this.tsmiRenumberAllElements.Text = "Renumber All";
            this.tsmiRenumberAllElements.Click += new System.EventHandler(this.tsmiRenumberAllElements_Click);
            // 
            // tsmiPart
            // 
            this.tsmiPart.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEditPart,
            this.tsmiTransformModelParts,
            this.tsmiMergeParts,
            this.tsmiDividerPart2,
            this.tsmiHideParts,
            this.tsmiShowParts,
            this.tsmiShowOnlyParts,
            this.tsmiSetTransparencyForParts,
            this.tsmiDividerPart3,
            this.tsmiDeleteParts});
            this.tsmiPart.Image = global::PrePoMax.Properties.Resources.Part;
            this.tsmiPart.Name = "tsmiPart";
            this.tsmiPart.Size = new System.Drawing.Size(194, 22);
            this.tsmiPart.Text = "Part";
            // 
            // tsmiEditPart
            // 
            this.tsmiEditPart.Name = "tsmiEditPart";
            this.tsmiEditPart.Size = new System.Drawing.Size(162, 22);
            this.tsmiEditPart.Text = "Edit";
            this.tsmiEditPart.Click += new System.EventHandler(this.tsmiEditPart_Click);
            // 
            // tsmiTransformModelParts
            // 
            this.tsmiTransformModelParts.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTranslateModelParts,
            this.tsmiScaleModelParts,
            this.tsmiRotateModelParts});
            this.tsmiTransformModelParts.Name = "tsmiTransformModelParts";
            this.tsmiTransformModelParts.Size = new System.Drawing.Size(162, 22);
            this.tsmiTransformModelParts.Text = "Transform";
            // 
            // tsmiTranslateModelParts
            // 
            this.tsmiTranslateModelParts.Name = "tsmiTranslateModelParts";
            this.tsmiTranslateModelParts.Size = new System.Drawing.Size(120, 22);
            this.tsmiTranslateModelParts.Text = "Translate";
            this.tsmiTranslateModelParts.Click += new System.EventHandler(this.tsmiTranslateModelParts_Click);
            // 
            // tsmiScaleModelParts
            // 
            this.tsmiScaleModelParts.Name = "tsmiScaleModelParts";
            this.tsmiScaleModelParts.Size = new System.Drawing.Size(120, 22);
            this.tsmiScaleModelParts.Text = "Scale";
            this.tsmiScaleModelParts.Click += new System.EventHandler(this.tsmiScaleModelParts_Click);
            // 
            // tsmiRotateModelParts
            // 
            this.tsmiRotateModelParts.Name = "tsmiRotateModelParts";
            this.tsmiRotateModelParts.Size = new System.Drawing.Size(120, 22);
            this.tsmiRotateModelParts.Text = "Rotate";
            this.tsmiRotateModelParts.Click += new System.EventHandler(this.tsmiRotateModelParts_Click);
            // 
            // tsmiMergeParts
            // 
            this.tsmiMergeParts.Name = "tsmiMergeParts";
            this.tsmiMergeParts.Size = new System.Drawing.Size(162, 22);
            this.tsmiMergeParts.Text = "Merge";
            this.tsmiMergeParts.Click += new System.EventHandler(this.tsmiMergeParts_Click);
            // 
            // tsmiDividerPart2
            // 
            this.tsmiDividerPart2.Name = "tsmiDividerPart2";
            this.tsmiDividerPart2.Size = new System.Drawing.Size(159, 6);
            // 
            // tsmiHideParts
            // 
            this.tsmiHideParts.Image = ((System.Drawing.Image)(resources.GetObject("tsmiHideParts.Image")));
            this.tsmiHideParts.Name = "tsmiHideParts";
            this.tsmiHideParts.Size = new System.Drawing.Size(162, 22);
            this.tsmiHideParts.Text = "Hide";
            this.tsmiHideParts.Click += new System.EventHandler(this.tsmiHideParts_Click);
            // 
            // tsmiShowParts
            // 
            this.tsmiShowParts.Image = ((System.Drawing.Image)(resources.GetObject("tsmiShowParts.Image")));
            this.tsmiShowParts.Name = "tsmiShowParts";
            this.tsmiShowParts.Size = new System.Drawing.Size(162, 22);
            this.tsmiShowParts.Text = "Show";
            this.tsmiShowParts.Click += new System.EventHandler(this.tsmiShowParts_Click);
            // 
            // tsmiShowOnlyParts
            // 
            this.tsmiShowOnlyParts.Image = ((System.Drawing.Image)(resources.GetObject("tsmiShowOnlyParts.Image")));
            this.tsmiShowOnlyParts.Name = "tsmiShowOnlyParts";
            this.tsmiShowOnlyParts.Size = new System.Drawing.Size(162, 22);
            this.tsmiShowOnlyParts.Text = "Show Only";
            this.tsmiShowOnlyParts.Click += new System.EventHandler(this.tsmiShowOnlyParts_Click);
            // 
            // tsmiSetTransparencyForParts
            // 
            this.tsmiSetTransparencyForParts.Name = "tsmiSetTransparencyForParts";
            this.tsmiSetTransparencyForParts.Size = new System.Drawing.Size(162, 22);
            this.tsmiSetTransparencyForParts.Text = "Set Transparency";
            this.tsmiSetTransparencyForParts.Click += new System.EventHandler(this.tsmiSetTransparencyForParts_Click);
            // 
            // tsmiDividerPart3
            // 
            this.tsmiDividerPart3.Name = "tsmiDividerPart3";
            this.tsmiDividerPart3.Size = new System.Drawing.Size(159, 6);
            // 
            // tsmiDeleteParts
            // 
            this.tsmiDeleteParts.Name = "tsmiDeleteParts";
            this.tsmiDeleteParts.Size = new System.Drawing.Size(162, 22);
            this.tsmiDeleteParts.Text = "Delete";
            this.tsmiDeleteParts.Click += new System.EventHandler(this.tsmiDeleteParts_Click);
            // 
            // tsmiNodeSet
            // 
            this.tsmiNodeSet.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateNodeSet,
            this.tsmiEditNodeSet,
            this.tsmiDuplicateNodeSet,
            this.tsmiDividerNodeSet1,
            this.tsmiDeleteNodeSet});
            this.tsmiNodeSet.Image = global::PrePoMax.Properties.Resources.Node_set;
            this.tsmiNodeSet.Name = "tsmiNodeSet";
            this.tsmiNodeSet.Size = new System.Drawing.Size(194, 22);
            this.tsmiNodeSet.Text = "Node set";
            // 
            // tsmiCreateNodeSet
            // 
            this.tsmiCreateNodeSet.Name = "tsmiCreateNodeSet";
            this.tsmiCreateNodeSet.Size = new System.Drawing.Size(124, 22);
            this.tsmiCreateNodeSet.Text = "Create";
            this.tsmiCreateNodeSet.Click += new System.EventHandler(this.tsmiCreateNodeSet_Click);
            // 
            // tsmiEditNodeSet
            // 
            this.tsmiEditNodeSet.Name = "tsmiEditNodeSet";
            this.tsmiEditNodeSet.Size = new System.Drawing.Size(124, 22);
            this.tsmiEditNodeSet.Text = "Edit";
            this.tsmiEditNodeSet.Click += new System.EventHandler(this.tsmiEditNodeSet_Click);
            // 
            // tsmiDuplicateNodeSet
            // 
            this.tsmiDuplicateNodeSet.Name = "tsmiDuplicateNodeSet";
            this.tsmiDuplicateNodeSet.Size = new System.Drawing.Size(124, 22);
            this.tsmiDuplicateNodeSet.Text = "Duplicate";
            this.tsmiDuplicateNodeSet.Click += new System.EventHandler(this.tsmiDuplicateNodeSet_Click);
            // 
            // tsmiDividerNodeSet1
            // 
            this.tsmiDividerNodeSet1.Name = "tsmiDividerNodeSet1";
            this.tsmiDividerNodeSet1.Size = new System.Drawing.Size(121, 6);
            // 
            // tsmiDeleteNodeSet
            // 
            this.tsmiDeleteNodeSet.Name = "tsmiDeleteNodeSet";
            this.tsmiDeleteNodeSet.Size = new System.Drawing.Size(124, 22);
            this.tsmiDeleteNodeSet.Text = "Delete";
            this.tsmiDeleteNodeSet.Click += new System.EventHandler(this.tsmiDeleteNodeSet_Click);
            // 
            // tsmiElementSet
            // 
            this.tsmiElementSet.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateElementSet,
            this.tsmiEditElementSet,
            this.tsmiDuplicateElementSet,
            this.tsmiConvertElementSetsToMeshParts,
            this.tsmiDividerElementSet1,
            this.tsmiDeleteElementSet});
            this.tsmiElementSet.Image = global::PrePoMax.Properties.Resources.Element_set;
            this.tsmiElementSet.Name = "tsmiElementSet";
            this.tsmiElementSet.Size = new System.Drawing.Size(194, 22);
            this.tsmiElementSet.Text = "Element set";
            // 
            // tsmiCreateElementSet
            // 
            this.tsmiCreateElementSet.Name = "tsmiCreateElementSet";
            this.tsmiCreateElementSet.Size = new System.Drawing.Size(154, 22);
            this.tsmiCreateElementSet.Text = "Create";
            this.tsmiCreateElementSet.Click += new System.EventHandler(this.tsmiCreateElementSet_Click);
            // 
            // tsmiEditElementSet
            // 
            this.tsmiEditElementSet.Name = "tsmiEditElementSet";
            this.tsmiEditElementSet.Size = new System.Drawing.Size(154, 22);
            this.tsmiEditElementSet.Text = "Edit";
            this.tsmiEditElementSet.Click += new System.EventHandler(this.tsmiEditElementSet_Click);
            // 
            // tsmiDuplicateElementSet
            // 
            this.tsmiDuplicateElementSet.Name = "tsmiDuplicateElementSet";
            this.tsmiDuplicateElementSet.Size = new System.Drawing.Size(154, 22);
            this.tsmiDuplicateElementSet.Text = "Duplicate";
            this.tsmiDuplicateElementSet.Click += new System.EventHandler(this.tsmiDuplicateElementSet_Click);
            // 
            // tsmiConvertElementSetsToMeshParts
            // 
            this.tsmiConvertElementSetsToMeshParts.Name = "tsmiConvertElementSetsToMeshParts";
            this.tsmiConvertElementSetsToMeshParts.Size = new System.Drawing.Size(154, 22);
            this.tsmiConvertElementSetsToMeshParts.Text = "Convert to Part";
            this.tsmiConvertElementSetsToMeshParts.Click += new System.EventHandler(this.tsmiConvertElementSetsToMeshParts_Click);
            // 
            // tsmiDividerElementSet1
            // 
            this.tsmiDividerElementSet1.Name = "tsmiDividerElementSet1";
            this.tsmiDividerElementSet1.Size = new System.Drawing.Size(151, 6);
            // 
            // tsmiDeleteElementSet
            // 
            this.tsmiDeleteElementSet.Name = "tsmiDeleteElementSet";
            this.tsmiDeleteElementSet.Size = new System.Drawing.Size(154, 22);
            this.tsmiDeleteElementSet.Text = "Delete";
            this.tsmiDeleteElementSet.Click += new System.EventHandler(this.tsmiDeleteElementSet_Click);
            // 
            // tsmiSurface
            // 
            this.tsmiSurface.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateSurface,
            this.tsmiEditSurface,
            this.tsmiDividerSurface1,
            this.tsmiDeleteSurface});
            this.tsmiSurface.Image = global::PrePoMax.Properties.Resources.Surface;
            this.tsmiSurface.Name = "tsmiSurface";
            this.tsmiSurface.Size = new System.Drawing.Size(194, 22);
            this.tsmiSurface.Text = "Surface";
            // 
            // tsmiCreateSurface
            // 
            this.tsmiCreateSurface.Name = "tsmiCreateSurface";
            this.tsmiCreateSurface.Size = new System.Drawing.Size(108, 22);
            this.tsmiCreateSurface.Text = "Create";
            this.tsmiCreateSurface.Click += new System.EventHandler(this.tsmiCreateSurface_Click);
            // 
            // tsmiEditSurface
            // 
            this.tsmiEditSurface.Name = "tsmiEditSurface";
            this.tsmiEditSurface.Size = new System.Drawing.Size(108, 22);
            this.tsmiEditSurface.Text = "Edit";
            this.tsmiEditSurface.Click += new System.EventHandler(this.tsmiEditSurface_Click);
            // 
            // tsmiDividerSurface1
            // 
            this.tsmiDividerSurface1.Name = "tsmiDividerSurface1";
            this.tsmiDividerSurface1.Size = new System.Drawing.Size(105, 6);
            // 
            // tsmiDeleteSurface
            // 
            this.tsmiDeleteSurface.Name = "tsmiDeleteSurface";
            this.tsmiDeleteSurface.Size = new System.Drawing.Size(108, 22);
            this.tsmiDeleteSurface.Text = "Delete";
            this.tsmiDeleteSurface.Click += new System.EventHandler(this.tsmiDeleteSurface_Click);
            // 
            // tsmiReferencePointTool
            // 
            this.tsmiReferencePointTool.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateRP,
            this.tsmiEditRP,
            this.tsmiRP,
            this.tsmiDeleteRP});
            this.tsmiReferencePointTool.Image = global::PrePoMax.Properties.Resources.Reference_point;
            this.tsmiReferencePointTool.Name = "tsmiReferencePointTool";
            this.tsmiReferencePointTool.Size = new System.Drawing.Size(194, 22);
            this.tsmiReferencePointTool.Text = "Reference point";
            // 
            // tsmiCreateRP
            // 
            this.tsmiCreateRP.Name = "tsmiCreateRP";
            this.tsmiCreateRP.Size = new System.Drawing.Size(108, 22);
            this.tsmiCreateRP.Text = "Create";
            this.tsmiCreateRP.Click += new System.EventHandler(this.tsmiCreateRP_Click);
            // 
            // tsmiEditRP
            // 
            this.tsmiEditRP.Name = "tsmiEditRP";
            this.tsmiEditRP.Size = new System.Drawing.Size(108, 22);
            this.tsmiEditRP.Text = "Edit";
            this.tsmiEditRP.Click += new System.EventHandler(this.tsmiEditRP_Click);
            // 
            // tsmiRP
            // 
            this.tsmiRP.Name = "tsmiRP";
            this.tsmiRP.Size = new System.Drawing.Size(105, 6);
            // 
            // tsmiDeleteRP
            // 
            this.tsmiDeleteRP.Name = "tsmiDeleteRP";
            this.tsmiDeleteRP.Size = new System.Drawing.Size(108, 22);
            this.tsmiDeleteRP.Text = "Delete";
            this.tsmiDeleteRP.Click += new System.EventHandler(this.tsmiDeleteRP_Click);
            // 
            // tsmiProperty
            // 
            this.tsmiProperty.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMaterial,
            this.tsmiMaterialLibrary,
            this.tsmiSection});
            this.tsmiProperty.Name = "tsmiProperty";
            this.tsmiProperty.Size = new System.Drawing.Size(64, 20);
            this.tsmiProperty.Text = "Property";
            // 
            // tsmiMaterial
            // 
            this.tsmiMaterial.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateMaterial,
            this.tsmiEditMaterial,
            this.tsmiDuplicateMaterial,
            this.tsmiDividerMaterial1,
            this.tsmiImportMaterial,
            this.tsmiExportMaterial,
            this.tsmiDividerMaterial2,
            this.tsmiDeleteMaterial});
            this.tsmiMaterial.Image = global::PrePoMax.Properties.Resources.Material;
            this.tsmiMaterial.Name = "tsmiMaterial";
            this.tsmiMaterial.Size = new System.Drawing.Size(156, 22);
            this.tsmiMaterial.Text = "Material";
            // 
            // tsmiCreateMaterial
            // 
            this.tsmiCreateMaterial.Name = "tsmiCreateMaterial";
            this.tsmiCreateMaterial.Size = new System.Drawing.Size(162, 22);
            this.tsmiCreateMaterial.Text = "Create";
            this.tsmiCreateMaterial.Click += new System.EventHandler(this.tsmiCreateMaterial_Click);
            // 
            // tsmiEditMaterial
            // 
            this.tsmiEditMaterial.Name = "tsmiEditMaterial";
            this.tsmiEditMaterial.Size = new System.Drawing.Size(162, 22);
            this.tsmiEditMaterial.Text = "Edit";
            this.tsmiEditMaterial.Click += new System.EventHandler(this.tsmiEditMaterial_Click);
            // 
            // tsmiDuplicateMaterial
            // 
            this.tsmiDuplicateMaterial.Name = "tsmiDuplicateMaterial";
            this.tsmiDuplicateMaterial.Size = new System.Drawing.Size(162, 22);
            this.tsmiDuplicateMaterial.Text = "Duplicate";
            this.tsmiDuplicateMaterial.Click += new System.EventHandler(this.tsmiDuplicateMaterial_Click);
            // 
            // tsmiDividerMaterial1
            // 
            this.tsmiDividerMaterial1.Name = "tsmiDividerMaterial1";
            this.tsmiDividerMaterial1.Size = new System.Drawing.Size(159, 6);
            // 
            // tsmiImportMaterial
            // 
            this.tsmiImportMaterial.Name = "tsmiImportMaterial";
            this.tsmiImportMaterial.Size = new System.Drawing.Size(162, 22);
            this.tsmiImportMaterial.Text = "Import from .inp";
            this.tsmiImportMaterial.Click += new System.EventHandler(this.tsmiImportMaterial_Click);
            // 
            // tsmiExportMaterial
            // 
            this.tsmiExportMaterial.Name = "tsmiExportMaterial";
            this.tsmiExportMaterial.Size = new System.Drawing.Size(162, 22);
            this.tsmiExportMaterial.Text = "Export to .inp";
            this.tsmiExportMaterial.Click += new System.EventHandler(this.tsmiExportMaterial_Click);
            // 
            // tsmiDividerMaterial2
            // 
            this.tsmiDividerMaterial2.Name = "tsmiDividerMaterial2";
            this.tsmiDividerMaterial2.Size = new System.Drawing.Size(159, 6);
            // 
            // tsmiDeleteMaterial
            // 
            this.tsmiDeleteMaterial.Name = "tsmiDeleteMaterial";
            this.tsmiDeleteMaterial.Size = new System.Drawing.Size(162, 22);
            this.tsmiDeleteMaterial.Text = "Delete";
            this.tsmiDeleteMaterial.Click += new System.EventHandler(this.tsmiDeleteMaterial_Click);
            // 
            // tsmiMaterialLibrary
            // 
            this.tsmiMaterialLibrary.Image = global::PrePoMax.Properties.Resources.Library;
            this.tsmiMaterialLibrary.Name = "tsmiMaterialLibrary";
            this.tsmiMaterialLibrary.Size = new System.Drawing.Size(156, 22);
            this.tsmiMaterialLibrary.Text = "Material Library";
            this.tsmiMaterialLibrary.Click += new System.EventHandler(this.tsmiMaterialLibrary_Click);
            // 
            // tsmiSection
            // 
            this.tsmiSection.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateSection,
            this.tsmiEditSection,
            this.tsmiDividerSection1,
            this.tsmiDelete});
            this.tsmiSection.Image = global::PrePoMax.Properties.Resources.Section;
            this.tsmiSection.Name = "tsmiSection";
            this.tsmiSection.Size = new System.Drawing.Size(156, 22);
            this.tsmiSection.Text = "Section";
            // 
            // tsmiCreateSection
            // 
            this.tsmiCreateSection.Name = "tsmiCreateSection";
            this.tsmiCreateSection.Size = new System.Drawing.Size(108, 22);
            this.tsmiCreateSection.Text = "Create";
            this.tsmiCreateSection.Click += new System.EventHandler(this.tsmiCreateSection_Click);
            // 
            // tsmiEditSection
            // 
            this.tsmiEditSection.Name = "tsmiEditSection";
            this.tsmiEditSection.Size = new System.Drawing.Size(108, 22);
            this.tsmiEditSection.Text = "Edit";
            this.tsmiEditSection.Click += new System.EventHandler(this.tsmiEditSection_Click);
            // 
            // tsmiDividerSection1
            // 
            this.tsmiDividerSection1.Name = "tsmiDividerSection1";
            this.tsmiDividerSection1.Size = new System.Drawing.Size(105, 6);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(108, 22);
            this.tsmiDelete.Text = "Delete";
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // tsmiInteraction
            // 
            this.tsmiInteraction.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiConstraint,
            this.tsmiContact,
            this.tsmiDividerInteraction1,
            this.tsmiSearchContactPairs});
            this.tsmiInteraction.Name = "tsmiInteraction";
            this.tsmiInteraction.Size = new System.Drawing.Size(76, 20);
            this.tsmiInteraction.Text = "Interaction";
            // 
            // tsmiConstraint
            // 
            this.tsmiConstraint.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateConstraint,
            this.tsmiEditConstraint,
            this.tsmiDividerConstraint1,
            this.tsmiSwapMasterSlaveConstraint,
            this.tsmiMergeByMasterSlaveConstraint,
            this.tsmiDividerConstraint2,
            this.tsmiHideConstraint,
            this.tsmiShowConstraint,
            this.tsmiDividerConstraint3,
            this.tsmiDeleteConstraint});
            this.tsmiConstraint.Image = global::PrePoMax.Properties.Resources.Constraints;
            this.tsmiConstraint.Name = "tsmiConstraint";
            this.tsmiConstraint.Size = new System.Drawing.Size(182, 22);
            this.tsmiConstraint.Text = "Constraint";
            // 
            // tsmiCreateConstraint
            // 
            this.tsmiCreateConstraint.Name = "tsmiCreateConstraint";
            this.tsmiCreateConstraint.Size = new System.Drawing.Size(195, 22);
            this.tsmiCreateConstraint.Text = "Create";
            this.tsmiCreateConstraint.Click += new System.EventHandler(this.tsmiCreateConstraint_Click);
            // 
            // tsmiEditConstraint
            // 
            this.tsmiEditConstraint.Name = "tsmiEditConstraint";
            this.tsmiEditConstraint.Size = new System.Drawing.Size(195, 22);
            this.tsmiEditConstraint.Text = "Edit";
            this.tsmiEditConstraint.Click += new System.EventHandler(this.tsmiEditConstraint_Click);
            // 
            // tsmiDividerConstraint1
            // 
            this.tsmiDividerConstraint1.Name = "tsmiDividerConstraint1";
            this.tsmiDividerConstraint1.Size = new System.Drawing.Size(192, 6);
            // 
            // tsmiSwapMasterSlaveConstraint
            // 
            this.tsmiSwapMasterSlaveConstraint.Name = "tsmiSwapMasterSlaveConstraint";
            this.tsmiSwapMasterSlaveConstraint.Size = new System.Drawing.Size(195, 22);
            this.tsmiSwapMasterSlaveConstraint.Text = "Swap Master/Slave";
            this.tsmiSwapMasterSlaveConstraint.Click += new System.EventHandler(this.tsmiSwapMasterSlaveConstraint_Click);
            // 
            // tsmiMergeByMasterSlaveConstraint
            // 
            this.tsmiMergeByMasterSlaveConstraint.Name = "tsmiMergeByMasterSlaveConstraint";
            this.tsmiMergeByMasterSlaveConstraint.Size = new System.Drawing.Size(195, 22);
            this.tsmiMergeByMasterSlaveConstraint.Text = "Merge by Master/Slave";
            this.tsmiMergeByMasterSlaveConstraint.Click += new System.EventHandler(this.tsmiMergeByMasterSlaveConstraint_Click);
            // 
            // tsmiDividerConstraint2
            // 
            this.tsmiDividerConstraint2.Name = "tsmiDividerConstraint2";
            this.tsmiDividerConstraint2.Size = new System.Drawing.Size(192, 6);
            // 
            // tsmiHideConstraint
            // 
            this.tsmiHideConstraint.Image = global::PrePoMax.Properties.Resources.Hide;
            this.tsmiHideConstraint.Name = "tsmiHideConstraint";
            this.tsmiHideConstraint.Size = new System.Drawing.Size(195, 22);
            this.tsmiHideConstraint.Text = "Hide";
            this.tsmiHideConstraint.Click += new System.EventHandler(this.tsmiHideConstraint_Click);
            // 
            // tsmiShowConstraint
            // 
            this.tsmiShowConstraint.Image = global::PrePoMax.Properties.Resources.Show;
            this.tsmiShowConstraint.Name = "tsmiShowConstraint";
            this.tsmiShowConstraint.Size = new System.Drawing.Size(195, 22);
            this.tsmiShowConstraint.Text = "Show";
            this.tsmiShowConstraint.Click += new System.EventHandler(this.tsmiShowConstraint_Click);
            // 
            // tsmiDividerConstraint3
            // 
            this.tsmiDividerConstraint3.Name = "tsmiDividerConstraint3";
            this.tsmiDividerConstraint3.Size = new System.Drawing.Size(192, 6);
            // 
            // tsmiDeleteConstraint
            // 
            this.tsmiDeleteConstraint.Name = "tsmiDeleteConstraint";
            this.tsmiDeleteConstraint.Size = new System.Drawing.Size(195, 22);
            this.tsmiDeleteConstraint.Text = "Delete";
            this.tsmiDeleteConstraint.Click += new System.EventHandler(this.tsmiDeleteConstraint_Click);
            // 
            // tsmiContact
            // 
            this.tsmiContact.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSurfaceInteraction,
            this.contactPairToolStripMenuItem});
            this.tsmiContact.Image = global::PrePoMax.Properties.Resources.Contact;
            this.tsmiContact.Name = "tsmiContact";
            this.tsmiContact.Size = new System.Drawing.Size(182, 22);
            this.tsmiContact.Text = "Contact";
            // 
            // tsmiSurfaceInteraction
            // 
            this.tsmiSurfaceInteraction.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateSurfaceInteraction,
            this.tsmiEditSurfaceInteraction,
            this.tsmiDuplicateSurfaceInteraction,
            this.tsmiDividerSurfaceInteraction1,
            this.tsmiDeleteSurfaceInteraction});
            this.tsmiSurfaceInteraction.Image = global::PrePoMax.Properties.Resources.SurfaceInteraction;
            this.tsmiSurfaceInteraction.Name = "tsmiSurfaceInteraction";
            this.tsmiSurfaceInteraction.Size = new System.Drawing.Size(173, 22);
            this.tsmiSurfaceInteraction.Text = "Surface Interaction";
            // 
            // tsmiCreateSurfaceInteraction
            // 
            this.tsmiCreateSurfaceInteraction.Name = "tsmiCreateSurfaceInteraction";
            this.tsmiCreateSurfaceInteraction.Size = new System.Drawing.Size(124, 22);
            this.tsmiCreateSurfaceInteraction.Text = "Create";
            this.tsmiCreateSurfaceInteraction.Click += new System.EventHandler(this.tsmiCreateSurfaceInteraction_Click);
            // 
            // tsmiEditSurfaceInteraction
            // 
            this.tsmiEditSurfaceInteraction.Name = "tsmiEditSurfaceInteraction";
            this.tsmiEditSurfaceInteraction.Size = new System.Drawing.Size(124, 22);
            this.tsmiEditSurfaceInteraction.Text = "Edit";
            this.tsmiEditSurfaceInteraction.Click += new System.EventHandler(this.tsmiEditSurfaceInteraction_Click);
            // 
            // tsmiDuplicateSurfaceInteraction
            // 
            this.tsmiDuplicateSurfaceInteraction.Name = "tsmiDuplicateSurfaceInteraction";
            this.tsmiDuplicateSurfaceInteraction.Size = new System.Drawing.Size(124, 22);
            this.tsmiDuplicateSurfaceInteraction.Text = "Duplicate";
            this.tsmiDuplicateSurfaceInteraction.Click += new System.EventHandler(this.tsmiDuplicateSurfaceInteraction_Click);
            // 
            // tsmiDividerSurfaceInteraction1
            // 
            this.tsmiDividerSurfaceInteraction1.Name = "tsmiDividerSurfaceInteraction1";
            this.tsmiDividerSurfaceInteraction1.Size = new System.Drawing.Size(121, 6);
            // 
            // tsmiDeleteSurfaceInteraction
            // 
            this.tsmiDeleteSurfaceInteraction.Name = "tsmiDeleteSurfaceInteraction";
            this.tsmiDeleteSurfaceInteraction.Size = new System.Drawing.Size(124, 22);
            this.tsmiDeleteSurfaceInteraction.Text = "Delete";
            this.tsmiDeleteSurfaceInteraction.Click += new System.EventHandler(this.tsmiDeleteSurfaceInteraction_Click);
            // 
            // contactPairToolStripMenuItem
            // 
            this.contactPairToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateContactPair,
            this.tsmiEditContactPair,
            this.tsmiDividerContactPair1,
            this.tsmiSwapMasterSlaveContactPair,
            this.tsmiMergeByMasterSlaveContactPair,
            this.tsmiDividerContactPair2,
            this.tsmiHideContactPair,
            this.tsmiShowContactPair,
            this.tsmiDividerContactPair3,
            this.tsmiDeleteContactPair});
            this.contactPairToolStripMenuItem.Image = global::PrePoMax.Properties.Resources.ContactPair;
            this.contactPairToolStripMenuItem.Name = "contactPairToolStripMenuItem";
            this.contactPairToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.contactPairToolStripMenuItem.Text = "Contact Pair";
            // 
            // tsmiCreateContactPair
            // 
            this.tsmiCreateContactPair.Name = "tsmiCreateContactPair";
            this.tsmiCreateContactPair.Size = new System.Drawing.Size(195, 22);
            this.tsmiCreateContactPair.Text = "Create";
            this.tsmiCreateContactPair.Click += new System.EventHandler(this.tsmiCreateContactPair_Click);
            // 
            // tsmiEditContactPair
            // 
            this.tsmiEditContactPair.Name = "tsmiEditContactPair";
            this.tsmiEditContactPair.Size = new System.Drawing.Size(195, 22);
            this.tsmiEditContactPair.Text = "Edit";
            this.tsmiEditContactPair.Click += new System.EventHandler(this.tsmiEditContactPair_Click);
            // 
            // tsmiDividerContactPair1
            // 
            this.tsmiDividerContactPair1.Name = "tsmiDividerContactPair1";
            this.tsmiDividerContactPair1.Size = new System.Drawing.Size(192, 6);
            // 
            // tsmiSwapMasterSlaveContactPair
            // 
            this.tsmiSwapMasterSlaveContactPair.Name = "tsmiSwapMasterSlaveContactPair";
            this.tsmiSwapMasterSlaveContactPair.Size = new System.Drawing.Size(195, 22);
            this.tsmiSwapMasterSlaveContactPair.Text = "Swap Master/Slave";
            this.tsmiSwapMasterSlaveContactPair.Click += new System.EventHandler(this.tsmiSwapMasterSlaveContactPair_Click);
            // 
            // tsmiMergeByMasterSlaveContactPair
            // 
            this.tsmiMergeByMasterSlaveContactPair.Name = "tsmiMergeByMasterSlaveContactPair";
            this.tsmiMergeByMasterSlaveContactPair.Size = new System.Drawing.Size(195, 22);
            this.tsmiMergeByMasterSlaveContactPair.Text = "Merge by Master/Slave";
            this.tsmiMergeByMasterSlaveContactPair.Click += new System.EventHandler(this.tsmiMergeByMasterSlaveContactPair_Click);
            // 
            // tsmiDividerContactPair2
            // 
            this.tsmiDividerContactPair2.Name = "tsmiDividerContactPair2";
            this.tsmiDividerContactPair2.Size = new System.Drawing.Size(192, 6);
            // 
            // tsmiHideContactPair
            // 
            this.tsmiHideContactPair.Image = global::PrePoMax.Properties.Resources.Hide;
            this.tsmiHideContactPair.Name = "tsmiHideContactPair";
            this.tsmiHideContactPair.Size = new System.Drawing.Size(195, 22);
            this.tsmiHideContactPair.Text = "Hide";
            this.tsmiHideContactPair.Click += new System.EventHandler(this.tsmiHideContactPair_Click);
            // 
            // tsmiShowContactPair
            // 
            this.tsmiShowContactPair.Image = global::PrePoMax.Properties.Resources.Show;
            this.tsmiShowContactPair.Name = "tsmiShowContactPair";
            this.tsmiShowContactPair.Size = new System.Drawing.Size(195, 22);
            this.tsmiShowContactPair.Text = "Show";
            this.tsmiShowContactPair.Click += new System.EventHandler(this.tsmiShowContactPair_Click);
            // 
            // tsmiDividerContactPair3
            // 
            this.tsmiDividerContactPair3.Name = "tsmiDividerContactPair3";
            this.tsmiDividerContactPair3.Size = new System.Drawing.Size(192, 6);
            // 
            // tsmiDeleteContactPair
            // 
            this.tsmiDeleteContactPair.Name = "tsmiDeleteContactPair";
            this.tsmiDeleteContactPair.Size = new System.Drawing.Size(195, 22);
            this.tsmiDeleteContactPair.Text = "Delete";
            this.tsmiDeleteContactPair.Click += new System.EventHandler(this.tsmiDeleteContactPair_Click);
            // 
            // tsmiDividerInteraction1
            // 
            this.tsmiDividerInteraction1.Name = "tsmiDividerInteraction1";
            this.tsmiDividerInteraction1.Size = new System.Drawing.Size(179, 6);
            // 
            // tsmiSearchContactPairs
            // 
            this.tsmiSearchContactPairs.Name = "tsmiSearchContactPairs";
            this.tsmiSearchContactPairs.Size = new System.Drawing.Size(182, 22);
            this.tsmiSearchContactPairs.Text = "Search Contact Pairs";
            this.tsmiSearchContactPairs.Click += new System.EventHandler(this.tsmiSearchContactPairs_Click);
            // 
            // tsmiAmplitude
            // 
            this.tsmiAmplitude.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateAmplitude,
            this.tsmiEditAmplitude,
            this.tsmiDividerAmplitude1,
            this.tsmiDeleteAmplitude});
            this.tsmiAmplitude.Name = "tsmiAmplitude";
            this.tsmiAmplitude.Size = new System.Drawing.Size(75, 20);
            this.tsmiAmplitude.Text = "Amplitude";
            // 
            // tsmiCreateAmplitude
            // 
            this.tsmiCreateAmplitude.Name = "tsmiCreateAmplitude";
            this.tsmiCreateAmplitude.Size = new System.Drawing.Size(108, 22);
            this.tsmiCreateAmplitude.Text = "Create";
            this.tsmiCreateAmplitude.Click += new System.EventHandler(this.tsmiCreateAmplitude_Click);
            // 
            // tsmiEditAmplitude
            // 
            this.tsmiEditAmplitude.Name = "tsmiEditAmplitude";
            this.tsmiEditAmplitude.Size = new System.Drawing.Size(108, 22);
            this.tsmiEditAmplitude.Text = "Edit";
            this.tsmiEditAmplitude.Click += new System.EventHandler(this.tsmiEditAmplitude_Click);
            // 
            // tsmiDividerAmplitude1
            // 
            this.tsmiDividerAmplitude1.Name = "tsmiDividerAmplitude1";
            this.tsmiDividerAmplitude1.Size = new System.Drawing.Size(105, 6);
            // 
            // tsmiDeleteAmplitude
            // 
            this.tsmiDeleteAmplitude.Name = "tsmiDeleteAmplitude";
            this.tsmiDeleteAmplitude.Size = new System.Drawing.Size(108, 22);
            this.tsmiDeleteAmplitude.Text = "Delete";
            this.tsmiDeleteAmplitude.Click += new System.EventHandler(this.tsmiDeleteAmplitude_Click);
            // 
            // tsmiInitialCondition
            // 
            this.tsmiInitialCondition.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateInitialCondition,
            this.tsmiEditInitialCondition,
            this.tsmiPreviewInitialCondition,
            this.tsmiDividerInitialCondition1,
            this.tsmiDeleteInitialCondition});
            this.tsmiInitialCondition.Name = "tsmiInitialCondition";
            this.tsmiInitialCondition.Size = new System.Drawing.Size(102, 20);
            this.tsmiInitialCondition.Text = "Initial condition";
            // 
            // tsmiCreateInitialCondition
            // 
            this.tsmiCreateInitialCondition.Name = "tsmiCreateInitialCondition";
            this.tsmiCreateInitialCondition.Size = new System.Drawing.Size(115, 22);
            this.tsmiCreateInitialCondition.Text = "Create";
            this.tsmiCreateInitialCondition.Click += new System.EventHandler(this.tsmiCreateInitialCondition_Click);
            // 
            // tsmiEditInitialCondition
            // 
            this.tsmiEditInitialCondition.Name = "tsmiEditInitialCondition";
            this.tsmiEditInitialCondition.Size = new System.Drawing.Size(115, 22);
            this.tsmiEditInitialCondition.Text = "Edit";
            this.tsmiEditInitialCondition.Click += new System.EventHandler(this.tsmiEditInitialCondition_Click);
            // 
            // tsmiPreviewInitialCondition
            // 
            this.tsmiPreviewInitialCondition.Image = global::PrePoMax.Properties.Resources.Preview_initial_condition;
            this.tsmiPreviewInitialCondition.Name = "tsmiPreviewInitialCondition";
            this.tsmiPreviewInitialCondition.Size = new System.Drawing.Size(115, 22);
            this.tsmiPreviewInitialCondition.Text = "Preview";
            this.tsmiPreviewInitialCondition.Click += new System.EventHandler(this.tsmiPreviewInitialCondition_Click);
            // 
            // tsmiDividerInitialCondition1
            // 
            this.tsmiDividerInitialCondition1.Name = "tsmiDividerInitialCondition1";
            this.tsmiDividerInitialCondition1.Size = new System.Drawing.Size(112, 6);
            // 
            // tsmiDeleteInitialCondition
            // 
            this.tsmiDeleteInitialCondition.Name = "tsmiDeleteInitialCondition";
            this.tsmiDeleteInitialCondition.Size = new System.Drawing.Size(115, 22);
            this.tsmiDeleteInitialCondition.Text = "Delete";
            this.tsmiDeleteInitialCondition.Click += new System.EventHandler(this.tsmiDeleteInitialCondition_Click);
            // 
            // tsmiStepMenu
            // 
            this.tsmiStepMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiStep,
            this.tsmiDividerStep1,
            this.tsmiHistoryOutput,
            this.tsmiFieldOutput,
            this.tsmiBC,
            this.tsmiLoad,
            this.tsmiDefinedField});
            this.tsmiStepMenu.Name = "tsmiStepMenu";
            this.tsmiStepMenu.Size = new System.Drawing.Size(42, 20);
            this.tsmiStepMenu.Text = "Step";
            // 
            // tsmiStep
            // 
            this.tsmiStep.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateStep,
            this.tsmiEditStep,
            this.tsmiDuplicateStep,
            this.tsmiDividerStep2,
            this.tsmiDeleteStep});
            this.tsmiStep.Image = global::PrePoMax.Properties.Resources.Step;
            this.tsmiStep.Name = "tsmiStep";
            this.tsmiStep.Size = new System.Drawing.Size(153, 22);
            this.tsmiStep.Text = "Step";
            // 
            // tsmiCreateStep
            // 
            this.tsmiCreateStep.Name = "tsmiCreateStep";
            this.tsmiCreateStep.Size = new System.Drawing.Size(124, 22);
            this.tsmiCreateStep.Text = "Create";
            this.tsmiCreateStep.Click += new System.EventHandler(this.tsmiCreateStep_Click);
            // 
            // tsmiEditStep
            // 
            this.tsmiEditStep.Name = "tsmiEditStep";
            this.tsmiEditStep.Size = new System.Drawing.Size(124, 22);
            this.tsmiEditStep.Text = "Edit";
            this.tsmiEditStep.Click += new System.EventHandler(this.tsmiEditStep_Click);
            // 
            // tsmiDuplicateStep
            // 
            this.tsmiDuplicateStep.Name = "tsmiDuplicateStep";
            this.tsmiDuplicateStep.Size = new System.Drawing.Size(124, 22);
            this.tsmiDuplicateStep.Text = "Duplicate";
            this.tsmiDuplicateStep.Click += new System.EventHandler(this.tsmiDuplicateStep_Click);
            // 
            // tsmiDividerStep2
            // 
            this.tsmiDividerStep2.Name = "tsmiDividerStep2";
            this.tsmiDividerStep2.Size = new System.Drawing.Size(121, 6);
            // 
            // tsmiDeleteStep
            // 
            this.tsmiDeleteStep.Name = "tsmiDeleteStep";
            this.tsmiDeleteStep.Size = new System.Drawing.Size(124, 22);
            this.tsmiDeleteStep.Text = "Delete";
            this.tsmiDeleteStep.Click += new System.EventHandler(this.tsmiDeleteStep_Click);
            // 
            // tsmiDividerStep1
            // 
            this.tsmiDividerStep1.Name = "tsmiDividerStep1";
            this.tsmiDividerStep1.Size = new System.Drawing.Size(150, 6);
            // 
            // tsmiHistoryOutput
            // 
            this.tsmiHistoryOutput.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateHistoryOutput,
            this.tsmiEditHistoryOutput,
            this.tsmiPropagateHistoryOutput,
            this.tsmiHistoryOutputDivider1,
            this.tsmiDeleteHistoryOutput});
            this.tsmiHistoryOutput.Image = global::PrePoMax.Properties.Resources.History_output;
            this.tsmiHistoryOutput.Name = "tsmiHistoryOutput";
            this.tsmiHistoryOutput.Size = new System.Drawing.Size(153, 22);
            this.tsmiHistoryOutput.Text = "History Output";
            // 
            // tsmiCreateHistoryOutput
            // 
            this.tsmiCreateHistoryOutput.Name = "tsmiCreateHistoryOutput";
            this.tsmiCreateHistoryOutput.Size = new System.Drawing.Size(128, 22);
            this.tsmiCreateHistoryOutput.Text = "Create";
            this.tsmiCreateHistoryOutput.Click += new System.EventHandler(this.tsmiCreateHistoryOutput_Click);
            // 
            // tsmiEditHistoryOutput
            // 
            this.tsmiEditHistoryOutput.Name = "tsmiEditHistoryOutput";
            this.tsmiEditHistoryOutput.Size = new System.Drawing.Size(128, 22);
            this.tsmiEditHistoryOutput.Text = "Edit";
            this.tsmiEditHistoryOutput.Click += new System.EventHandler(this.tsmiEditHistoryOutput_Click);
            // 
            // tsmiPropagateHistoryOutput
            // 
            this.tsmiPropagateHistoryOutput.Name = "tsmiPropagateHistoryOutput";
            this.tsmiPropagateHistoryOutput.Size = new System.Drawing.Size(128, 22);
            this.tsmiPropagateHistoryOutput.Text = "Propagate";
            this.tsmiPropagateHistoryOutput.Click += new System.EventHandler(this.tsmiPropagateHistoryOutput_Click);
            // 
            // tsmiHistoryOutputDivider1
            // 
            this.tsmiHistoryOutputDivider1.Name = "tsmiHistoryOutputDivider1";
            this.tsmiHistoryOutputDivider1.Size = new System.Drawing.Size(125, 6);
            // 
            // tsmiDeleteHistoryOutput
            // 
            this.tsmiDeleteHistoryOutput.Name = "tsmiDeleteHistoryOutput";
            this.tsmiDeleteHistoryOutput.Size = new System.Drawing.Size(128, 22);
            this.tsmiDeleteHistoryOutput.Text = "Delete";
            this.tsmiDeleteHistoryOutput.Click += new System.EventHandler(this.tsmiDeleteHistoryOutput_Click);
            // 
            // tsmiFieldOutput
            // 
            this.tsmiFieldOutput.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateFieldOutput,
            this.tsmiEditFieldOutput,
            this.tsmiPropagateFieldOutput,
            this.tsmiDividerFieldOutput1,
            this.tsmiDeleteFieldOutput});
            this.tsmiFieldOutput.Image = global::PrePoMax.Properties.Resources.Field_output;
            this.tsmiFieldOutput.Name = "tsmiFieldOutput";
            this.tsmiFieldOutput.Size = new System.Drawing.Size(153, 22);
            this.tsmiFieldOutput.Text = "Field Output";
            // 
            // tsmiCreateFieldOutput
            // 
            this.tsmiCreateFieldOutput.Name = "tsmiCreateFieldOutput";
            this.tsmiCreateFieldOutput.Size = new System.Drawing.Size(128, 22);
            this.tsmiCreateFieldOutput.Text = "Create";
            this.tsmiCreateFieldOutput.Click += new System.EventHandler(this.tsmiCreateFieldOutput_Click);
            // 
            // tsmiEditFieldOutput
            // 
            this.tsmiEditFieldOutput.Name = "tsmiEditFieldOutput";
            this.tsmiEditFieldOutput.Size = new System.Drawing.Size(128, 22);
            this.tsmiEditFieldOutput.Text = "Edit";
            this.tsmiEditFieldOutput.Click += new System.EventHandler(this.tsmiEditFieldOutput_Click);
            // 
            // tsmiPropagateFieldOutput
            // 
            this.tsmiPropagateFieldOutput.Name = "tsmiPropagateFieldOutput";
            this.tsmiPropagateFieldOutput.Size = new System.Drawing.Size(128, 22);
            this.tsmiPropagateFieldOutput.Text = "Propagate";
            this.tsmiPropagateFieldOutput.Click += new System.EventHandler(this.tsmiPropagateFieldOutput_Click);
            // 
            // tsmiDividerFieldOutput1
            // 
            this.tsmiDividerFieldOutput1.Name = "tsmiDividerFieldOutput1";
            this.tsmiDividerFieldOutput1.Size = new System.Drawing.Size(125, 6);
            // 
            // tsmiDeleteFieldOutput
            // 
            this.tsmiDeleteFieldOutput.Name = "tsmiDeleteFieldOutput";
            this.tsmiDeleteFieldOutput.Size = new System.Drawing.Size(128, 22);
            this.tsmiDeleteFieldOutput.Text = "Delete";
            this.tsmiDeleteFieldOutput.Click += new System.EventHandler(this.tsmiDeleteFieldOutput_Click);
            // 
            // tsmiBC
            // 
            this.tsmiBC.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateBC,
            this.tsmiEditBC,
            this.tsmiPropagateBC,
            this.tsmiDividerBC1,
            this.tsmiHideBC,
            this.tsmiShowBC,
            this.tsmiDividerBC2,
            this.tsmiDeleteBC});
            this.tsmiBC.Image = global::PrePoMax.Properties.Resources.Bc;
            this.tsmiBC.Name = "tsmiBC";
            this.tsmiBC.Size = new System.Drawing.Size(153, 22);
            this.tsmiBC.Text = "BC";
            // 
            // tsmiCreateBC
            // 
            this.tsmiCreateBC.Name = "tsmiCreateBC";
            this.tsmiCreateBC.Size = new System.Drawing.Size(128, 22);
            this.tsmiCreateBC.Text = "Create";
            this.tsmiCreateBC.Click += new System.EventHandler(this.tsmiCreateBC_Click);
            // 
            // tsmiEditBC
            // 
            this.tsmiEditBC.Name = "tsmiEditBC";
            this.tsmiEditBC.Size = new System.Drawing.Size(128, 22);
            this.tsmiEditBC.Text = "Edit";
            this.tsmiEditBC.Click += new System.EventHandler(this.tsmiEditBC_Click);
            // 
            // tsmiPropagateBC
            // 
            this.tsmiPropagateBC.Name = "tsmiPropagateBC";
            this.tsmiPropagateBC.Size = new System.Drawing.Size(128, 22);
            this.tsmiPropagateBC.Text = "Propagate";
            this.tsmiPropagateBC.Click += new System.EventHandler(this.tsmiPropagateBC_Click);
            // 
            // tsmiDividerBC1
            // 
            this.tsmiDividerBC1.Name = "tsmiDividerBC1";
            this.tsmiDividerBC1.Size = new System.Drawing.Size(125, 6);
            // 
            // tsmiHideBC
            // 
            this.tsmiHideBC.Image = global::PrePoMax.Properties.Resources.Hide;
            this.tsmiHideBC.Name = "tsmiHideBC";
            this.tsmiHideBC.Size = new System.Drawing.Size(128, 22);
            this.tsmiHideBC.Text = "Hide";
            this.tsmiHideBC.Click += new System.EventHandler(this.tsmiHideBC_Click);
            // 
            // tsmiShowBC
            // 
            this.tsmiShowBC.Image = global::PrePoMax.Properties.Resources.Show;
            this.tsmiShowBC.Name = "tsmiShowBC";
            this.tsmiShowBC.Size = new System.Drawing.Size(128, 22);
            this.tsmiShowBC.Text = "Show";
            this.tsmiShowBC.Click += new System.EventHandler(this.tsmiShowBC_Click);
            // 
            // tsmiDividerBC2
            // 
            this.tsmiDividerBC2.Name = "tsmiDividerBC2";
            this.tsmiDividerBC2.Size = new System.Drawing.Size(125, 6);
            // 
            // tsmiDeleteBC
            // 
            this.tsmiDeleteBC.Name = "tsmiDeleteBC";
            this.tsmiDeleteBC.Size = new System.Drawing.Size(128, 22);
            this.tsmiDeleteBC.Text = "Delete";
            this.tsmiDeleteBC.Click += new System.EventHandler(this.tsmiDeleteBC_Click);
            // 
            // tsmiLoad
            // 
            this.tsmiLoad.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateLoad,
            this.tsmiEditLoad,
            this.tsmiPreviewLoad,
            this.tsmiPropagateLoad,
            this.tsmiDividerLoad1,
            this.tsmiHideLoad,
            this.tsmiShowLoad,
            this.tsmiDividerLoad2,
            this.tsmiDeleteLoad});
            this.tsmiLoad.Image = global::PrePoMax.Properties.Resources.Load;
            this.tsmiLoad.Name = "tsmiLoad";
            this.tsmiLoad.Size = new System.Drawing.Size(153, 22);
            this.tsmiLoad.Text = "Load";
            // 
            // tsmiCreateLoad
            // 
            this.tsmiCreateLoad.Name = "tsmiCreateLoad";
            this.tsmiCreateLoad.Size = new System.Drawing.Size(128, 22);
            this.tsmiCreateLoad.Text = "Create";
            this.tsmiCreateLoad.Click += new System.EventHandler(this.tsmiCreateLoad_Click);
            // 
            // tsmiEditLoad
            // 
            this.tsmiEditLoad.Name = "tsmiEditLoad";
            this.tsmiEditLoad.Size = new System.Drawing.Size(128, 22);
            this.tsmiEditLoad.Text = "Edit";
            this.tsmiEditLoad.Click += new System.EventHandler(this.tsmiEditLoad_Click);
            // 
            // tsmiPreviewLoad
            // 
            this.tsmiPreviewLoad.Image = global::PrePoMax.Properties.Resources.Preview_load;
            this.tsmiPreviewLoad.Name = "tsmiPreviewLoad";
            this.tsmiPreviewLoad.Size = new System.Drawing.Size(128, 22);
            this.tsmiPreviewLoad.Text = "Preview";
            this.tsmiPreviewLoad.Click += new System.EventHandler(this.tsmiPreviewLoad_Click);
            // 
            // tsmiPropagateLoad
            // 
            this.tsmiPropagateLoad.Image = global::PrePoMax.Properties.Resources.Step;
            this.tsmiPropagateLoad.Name = "tsmiPropagateLoad";
            this.tsmiPropagateLoad.Size = new System.Drawing.Size(128, 22);
            this.tsmiPropagateLoad.Text = "Propagate";
            this.tsmiPropagateLoad.Click += new System.EventHandler(this.tsmiPropagateLoad_Click);
            // 
            // tsmiDividerLoad1
            // 
            this.tsmiDividerLoad1.Name = "tsmiDividerLoad1";
            this.tsmiDividerLoad1.Size = new System.Drawing.Size(125, 6);
            // 
            // tsmiHideLoad
            // 
            this.tsmiHideLoad.Image = global::PrePoMax.Properties.Resources.Hide;
            this.tsmiHideLoad.Name = "tsmiHideLoad";
            this.tsmiHideLoad.Size = new System.Drawing.Size(128, 22);
            this.tsmiHideLoad.Text = "Hide";
            this.tsmiHideLoad.Click += new System.EventHandler(this.tsmiHideLoad_Click);
            // 
            // tsmiShowLoad
            // 
            this.tsmiShowLoad.Image = global::PrePoMax.Properties.Resources.Show;
            this.tsmiShowLoad.Name = "tsmiShowLoad";
            this.tsmiShowLoad.Size = new System.Drawing.Size(128, 22);
            this.tsmiShowLoad.Text = "Show";
            this.tsmiShowLoad.Click += new System.EventHandler(this.tsmiShowLoad_Click);
            // 
            // tsmiDividerLoad2
            // 
            this.tsmiDividerLoad2.Name = "tsmiDividerLoad2";
            this.tsmiDividerLoad2.Size = new System.Drawing.Size(125, 6);
            // 
            // tsmiDeleteLoad
            // 
            this.tsmiDeleteLoad.Name = "tsmiDeleteLoad";
            this.tsmiDeleteLoad.Size = new System.Drawing.Size(128, 22);
            this.tsmiDeleteLoad.Text = "Delete";
            this.tsmiDeleteLoad.Click += new System.EventHandler(this.tsmiDeleteLoad_Click);
            // 
            // tsmiDefinedField
            // 
            this.tsmiDefinedField.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateDefinedField,
            this.tsmiEditDefinedField,
            this.tsmiPropagateDefinedField,
            this.tsmiPreviewDefinedField,
            this.tsmiDividerDefinedField1,
            this.tsmiDeleteDefinedField});
            this.tsmiDefinedField.Image = global::PrePoMax.Properties.Resources.Defined_field;
            this.tsmiDefinedField.Name = "tsmiDefinedField";
            this.tsmiDefinedField.Size = new System.Drawing.Size(153, 22);
            this.tsmiDefinedField.Text = "Defined Field";
            // 
            // tsmiCreateDefinedField
            // 
            this.tsmiCreateDefinedField.Name = "tsmiCreateDefinedField";
            this.tsmiCreateDefinedField.Size = new System.Drawing.Size(128, 22);
            this.tsmiCreateDefinedField.Text = "Create";
            this.tsmiCreateDefinedField.Click += new System.EventHandler(this.tsmiCreateDefinedField_Click);
            // 
            // tsmiEditDefinedField
            // 
            this.tsmiEditDefinedField.Name = "tsmiEditDefinedField";
            this.tsmiEditDefinedField.Size = new System.Drawing.Size(128, 22);
            this.tsmiEditDefinedField.Text = "Edit";
            this.tsmiEditDefinedField.Click += new System.EventHandler(this.tsmiEditDefinedField_Click);
            // 
            // tsmiPropagateDefinedField
            // 
            this.tsmiPropagateDefinedField.Name = "tsmiPropagateDefinedField";
            this.tsmiPropagateDefinedField.Size = new System.Drawing.Size(128, 22);
            this.tsmiPropagateDefinedField.Text = "Propagate";
            this.tsmiPropagateDefinedField.Click += new System.EventHandler(this.tsmiPropagateDefinedField_Click);
            // 
            // tsmiPreviewDefinedField
            // 
            this.tsmiPreviewDefinedField.Image = global::PrePoMax.Properties.Resources.Preview_defined_field;
            this.tsmiPreviewDefinedField.Name = "tsmiPreviewDefinedField";
            this.tsmiPreviewDefinedField.Size = new System.Drawing.Size(128, 22);
            this.tsmiPreviewDefinedField.Text = "Preview";
            this.tsmiPreviewDefinedField.Click += new System.EventHandler(this.tsmiPreviewDefinedField_Click);
            // 
            // tsmiDividerDefinedField1
            // 
            this.tsmiDividerDefinedField1.Name = "tsmiDividerDefinedField1";
            this.tsmiDividerDefinedField1.Size = new System.Drawing.Size(125, 6);
            // 
            // tsmiDeleteDefinedField
            // 
            this.tsmiDeleteDefinedField.Name = "tsmiDeleteDefinedField";
            this.tsmiDeleteDefinedField.Size = new System.Drawing.Size(128, 22);
            this.tsmiDeleteDefinedField.Text = "Delete";
            this.tsmiDeleteDefinedField.Click += new System.EventHandler(this.tsmiDeleteDefinedField_Click);
            // 
            // tsmiAnalysis
            // 
            this.tsmiAnalysis.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateAnalysis,
            this.tsmiEditAnalysis,
            this.tsmiDividerAnalysis1,
            this.tsmiRunAnalysis,
            this.tsmiMonitorAnalysis,
            this.tsmiResultsAnalysis,
            this.tsmiKillAnalysis,
            this.tsmiDividerAnalysis2,
            this.tsmiDeleteAnalysis});
            this.tsmiAnalysis.Name = "tsmiAnalysis";
            this.tsmiAnalysis.Size = new System.Drawing.Size(62, 20);
            this.tsmiAnalysis.Text = "Analysis";
            // 
            // tsmiCreateAnalysis
            // 
            this.tsmiCreateAnalysis.Name = "tsmiCreateAnalysis";
            this.tsmiCreateAnalysis.Size = new System.Drawing.Size(117, 22);
            this.tsmiCreateAnalysis.Text = "Create";
            this.tsmiCreateAnalysis.Click += new System.EventHandler(this.tsmiCreateAnalysis_Click);
            // 
            // tsmiEditAnalysis
            // 
            this.tsmiEditAnalysis.Name = "tsmiEditAnalysis";
            this.tsmiEditAnalysis.Size = new System.Drawing.Size(117, 22);
            this.tsmiEditAnalysis.Text = "Edit";
            this.tsmiEditAnalysis.Click += new System.EventHandler(this.tsmiEditAnalysis_Click);
            // 
            // tsmiDividerAnalysis1
            // 
            this.tsmiDividerAnalysis1.Name = "tsmiDividerAnalysis1";
            this.tsmiDividerAnalysis1.Size = new System.Drawing.Size(114, 6);
            // 
            // tsmiRunAnalysis
            // 
            this.tsmiRunAnalysis.Name = "tsmiRunAnalysis";
            this.tsmiRunAnalysis.Size = new System.Drawing.Size(117, 22);
            this.tsmiRunAnalysis.Text = "Run";
            this.tsmiRunAnalysis.Click += new System.EventHandler(this.tsmiRunAnalysis_Click);
            // 
            // tsmiMonitorAnalysis
            // 
            this.tsmiMonitorAnalysis.Name = "tsmiMonitorAnalysis";
            this.tsmiMonitorAnalysis.Size = new System.Drawing.Size(117, 22);
            this.tsmiMonitorAnalysis.Text = "Monitor";
            this.tsmiMonitorAnalysis.Click += new System.EventHandler(this.tsmiMonitorAnalysis_Click);
            // 
            // tsmiResultsAnalysis
            // 
            this.tsmiResultsAnalysis.Name = "tsmiResultsAnalysis";
            this.tsmiResultsAnalysis.Size = new System.Drawing.Size(117, 22);
            this.tsmiResultsAnalysis.Text = "Results";
            this.tsmiResultsAnalysis.Click += new System.EventHandler(this.tsmiResultsAnalysis_Click);
            // 
            // tsmiKillAnalysis
            // 
            this.tsmiKillAnalysis.Name = "tsmiKillAnalysis";
            this.tsmiKillAnalysis.Size = new System.Drawing.Size(117, 22);
            this.tsmiKillAnalysis.Text = "Kill";
            this.tsmiKillAnalysis.Click += new System.EventHandler(this.tsmiKillAnalysis_Click);
            // 
            // tsmiDividerAnalysis2
            // 
            this.tsmiDividerAnalysis2.Name = "tsmiDividerAnalysis2";
            this.tsmiDividerAnalysis2.Size = new System.Drawing.Size(114, 6);
            // 
            // tsmiDeleteAnalysis
            // 
            this.tsmiDeleteAnalysis.Name = "tsmiDeleteAnalysis";
            this.tsmiDeleteAnalysis.Size = new System.Drawing.Size(117, 22);
            this.tsmiDeleteAnalysis.Text = "Delete";
            this.tsmiDeleteAnalysis.Click += new System.EventHandler(this.tsmiDeleteAnalysis_Click);
            // 
            // tsmiResults
            // 
            this.tsmiResults.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiResultPart,
            this.tsmiDividerResults1,
            this.tsmiResultFieldOutput,
            this.tsmiResultHistoryOutput,
            this.tsmiDividerResults2,
            this.tsmiTransformation});
            this.tsmiResults.Name = "tsmiResults";
            this.tsmiResults.Size = new System.Drawing.Size(56, 20);
            this.tsmiResults.Text = "Results";
            // 
            // tsmiResultPart
            // 
            this.tsmiResultPart.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEditResultPart,
            this.tsmiDividerResultPart1,
            this.tsmiHideResultParts,
            this.tsmiShowResultParts,
            this.tsmiShowOnlyResultParts,
            this.tsmiSetTransparencyForResultParts,
            this.tsmiDividerResultPart2,
            this.tsmiColorContoursOff,
            this.tsmiColorContoursOn,
            this.tsmiDividerResultPart3,
            this.tsmiDeleteResultParts});
            this.tsmiResultPart.Image = global::PrePoMax.Properties.Resources.Part;
            this.tsmiResultPart.Name = "tsmiResultPart";
            this.tsmiResultPart.Size = new System.Drawing.Size(154, 22);
            this.tsmiResultPart.Text = "Part";
            // 
            // tsmiEditResultPart
            // 
            this.tsmiEditResultPart.Name = "tsmiEditResultPart";
            this.tsmiEditResultPart.Size = new System.Drawing.Size(173, 22);
            this.tsmiEditResultPart.Text = "Edit";
            this.tsmiEditResultPart.Click += new System.EventHandler(this.tsmiEditResultParts_Click);
            // 
            // tsmiDividerResultPart1
            // 
            this.tsmiDividerResultPart1.Name = "tsmiDividerResultPart1";
            this.tsmiDividerResultPart1.Size = new System.Drawing.Size(170, 6);
            // 
            // tsmiHideResultParts
            // 
            this.tsmiHideResultParts.Image = global::PrePoMax.Properties.Resources.Hide;
            this.tsmiHideResultParts.Name = "tsmiHideResultParts";
            this.tsmiHideResultParts.Size = new System.Drawing.Size(173, 22);
            this.tsmiHideResultParts.Text = "Hide";
            this.tsmiHideResultParts.Click += new System.EventHandler(this.tsmiHideResultParts_Click);
            // 
            // tsmiShowResultParts
            // 
            this.tsmiShowResultParts.Image = global::PrePoMax.Properties.Resources.Show;
            this.tsmiShowResultParts.Name = "tsmiShowResultParts";
            this.tsmiShowResultParts.Size = new System.Drawing.Size(173, 22);
            this.tsmiShowResultParts.Text = "Show";
            this.tsmiShowResultParts.Click += new System.EventHandler(this.tsmiShowResultParts_Click);
            // 
            // tsmiShowOnlyResultParts
            // 
            this.tsmiShowOnlyResultParts.Image = global::PrePoMax.Properties.Resources.Show;
            this.tsmiShowOnlyResultParts.Name = "tsmiShowOnlyResultParts";
            this.tsmiShowOnlyResultParts.Size = new System.Drawing.Size(173, 22);
            this.tsmiShowOnlyResultParts.Text = "Show Only";
            this.tsmiShowOnlyResultParts.Click += new System.EventHandler(this.tsmiShowOnlyResultParts_Click);
            // 
            // tsmiSetTransparencyForResultParts
            // 
            this.tsmiSetTransparencyForResultParts.Name = "tsmiSetTransparencyForResultParts";
            this.tsmiSetTransparencyForResultParts.Size = new System.Drawing.Size(173, 22);
            this.tsmiSetTransparencyForResultParts.Text = "Set Transparency";
            this.tsmiSetTransparencyForResultParts.Click += new System.EventHandler(this.tsmiSetTransparencyForResultParts_Click);
            // 
            // tsmiDividerResultPart2
            // 
            this.tsmiDividerResultPart2.Name = "tsmiDividerResultPart2";
            this.tsmiDividerResultPart2.Size = new System.Drawing.Size(170, 6);
            // 
            // tsmiColorContoursOff
            // 
            this.tsmiColorContoursOff.Image = global::PrePoMax.Properties.Resources.Deformed;
            this.tsmiColorContoursOff.Name = "tsmiColorContoursOff";
            this.tsmiColorContoursOff.Size = new System.Drawing.Size(173, 22);
            this.tsmiColorContoursOff.Text = "Color Contours off";
            this.tsmiColorContoursOff.Click += new System.EventHandler(this.tsmiColorContoursOff_Click);
            // 
            // tsmiColorContoursOn
            // 
            this.tsmiColorContoursOn.Image = global::PrePoMax.Properties.Resources.Color_contours;
            this.tsmiColorContoursOn.Name = "tsmiColorContoursOn";
            this.tsmiColorContoursOn.Size = new System.Drawing.Size(173, 22);
            this.tsmiColorContoursOn.Text = "Color Contours on";
            this.tsmiColorContoursOn.Click += new System.EventHandler(this.tsmiColorContoursOn_Click);
            // 
            // tsmiDividerResultPart3
            // 
            this.tsmiDividerResultPart3.Name = "tsmiDividerResultPart3";
            this.tsmiDividerResultPart3.Size = new System.Drawing.Size(170, 6);
            // 
            // tsmiDeleteResultParts
            // 
            this.tsmiDeleteResultParts.Name = "tsmiDeleteResultParts";
            this.tsmiDeleteResultParts.Size = new System.Drawing.Size(173, 22);
            this.tsmiDeleteResultParts.Text = "Delete";
            this.tsmiDeleteResultParts.Click += new System.EventHandler(this.tsmiDeleteResultParts_Click);
            // 
            // tsmiDividerResults1
            // 
            this.tsmiDividerResults1.Name = "tsmiDividerResults1";
            this.tsmiDividerResults1.Size = new System.Drawing.Size(151, 6);
            // 
            // tsmiResultFieldOutput
            // 
            this.tsmiResultFieldOutput.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDeleteResultFieldOutput});
            this.tsmiResultFieldOutput.Image = global::PrePoMax.Properties.Resources.Field_output;
            this.tsmiResultFieldOutput.Name = "tsmiResultFieldOutput";
            this.tsmiResultFieldOutput.Size = new System.Drawing.Size(154, 22);
            this.tsmiResultFieldOutput.Text = "Field Output";
            // 
            // tsmiDeleteResultFieldOutput
            // 
            this.tsmiDeleteResultFieldOutput.Name = "tsmiDeleteResultFieldOutput";
            this.tsmiDeleteResultFieldOutput.Size = new System.Drawing.Size(107, 22);
            this.tsmiDeleteResultFieldOutput.Text = "Delete";
            this.tsmiDeleteResultFieldOutput.Click += new System.EventHandler(this.tsmiDeleteResultFieldOutput_Click);
            // 
            // tsmiResultHistoryOutput
            // 
            this.tsmiResultHistoryOutput.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateResultHistoryOutput,
            this.tsmiDividerResultHistoryOutput1,
            this.tsmiDeleteResultHistoryOutput});
            this.tsmiResultHistoryOutput.Image = global::PrePoMax.Properties.Resources.History_output;
            this.tsmiResultHistoryOutput.Name = "tsmiResultHistoryOutput";
            this.tsmiResultHistoryOutput.Size = new System.Drawing.Size(154, 22);
            this.tsmiResultHistoryOutput.Text = "History Output";
            // 
            // tsmiCreateResultHistoryOutput
            // 
            this.tsmiCreateResultHistoryOutput.Name = "tsmiCreateResultHistoryOutput";
            this.tsmiCreateResultHistoryOutput.Size = new System.Drawing.Size(108, 22);
            this.tsmiCreateResultHistoryOutput.Text = "Create";
            this.tsmiCreateResultHistoryOutput.Click += new System.EventHandler(this.tsmiCreateResultHistoryOutput_Click);
            // 
            // tsmiDividerResultHistoryOutput1
            // 
            this.tsmiDividerResultHistoryOutput1.Name = "tsmiDividerResultHistoryOutput1";
            this.tsmiDividerResultHistoryOutput1.Size = new System.Drawing.Size(105, 6);
            // 
            // tsmiDeleteResultHistoryOutput
            // 
            this.tsmiDeleteResultHistoryOutput.Name = "tsmiDeleteResultHistoryOutput";
            this.tsmiDeleteResultHistoryOutput.Size = new System.Drawing.Size(108, 22);
            this.tsmiDeleteResultHistoryOutput.Text = "Delete";
            this.tsmiDeleteResultHistoryOutput.Click += new System.EventHandler(this.tsmiDeleteResultHistoryOutput_Click);
            // 
            // tsmiDividerResults2
            // 
            this.tsmiDividerResults2.Name = "tsmiDividerResults2";
            this.tsmiDividerResults2.Size = new System.Drawing.Size(151, 6);
            // 
            // tsmiTransformation
            // 
            this.tsmiTransformation.Image = global::PrePoMax.Properties.Resources.Transformations;
            this.tsmiTransformation.Name = "tsmiTransformation";
            this.tsmiTransformation.Size = new System.Drawing.Size(154, 22);
            this.tsmiTransformation.Text = "Transformation";
            this.tsmiTransformation.Click += new System.EventHandler(this.tsmiTransformation_Click);
            // 
            // tsmiTools
            // 
            this.tsmiTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSettings,
            this.tsmiQuery});
            this.tsmiTools.Name = "tsmiTools";
            this.tsmiTools.Size = new System.Drawing.Size(46, 20);
            this.tsmiTools.Text = "Tools";
            // 
            // tsmiSettings
            // 
            this.tsmiSettings.Name = "tsmiSettings";
            this.tsmiSettings.Size = new System.Drawing.Size(116, 22);
            this.tsmiSettings.Text = "Settings";
            this.tsmiSettings.Click += new System.EventHandler(this.tsmiSettings_Click);
            // 
            // tsmiQuery
            // 
            this.tsmiQuery.Image = global::PrePoMax.Properties.Resources.Query;
            this.tsmiQuery.Name = "tsmiQuery";
            this.tsmiQuery.Size = new System.Drawing.Size(116, 22);
            this.tsmiQuery.Text = "Query";
            this.tsmiQuery.Click += new System.EventHandler(this.tsmiQuery_Click);
            // 
            // tsmiHelp
            // 
            this.tsmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAdvisor,
            this.tsmiHomePage,
            this.tsmiAdvisorHelp1,
            this.tsmiAbout,
            this.tsmiTest});
            this.tsmiHelp.Name = "tsmiHelp";
            this.tsmiHelp.Size = new System.Drawing.Size(44, 20);
            this.tsmiHelp.Text = "Help";
            // 
            // tsmiAdvisor
            // 
            this.tsmiAdvisor.Name = "tsmiAdvisor";
            this.tsmiAdvisor.Size = new System.Drawing.Size(136, 22);
            this.tsmiAdvisor.Text = "Advisor";
            this.tsmiAdvisor.Click += new System.EventHandler(this.tsmiAdvisor_Click);
            // 
            // tsmiHomePage
            // 
            this.tsmiHomePage.Name = "tsmiHomePage";
            this.tsmiHomePage.Size = new System.Drawing.Size(136, 22);
            this.tsmiHomePage.Text = "Home Page";
            this.tsmiHomePage.Click += new System.EventHandler(this.tsmiHomePage_Click);
            // 
            // tsmiAdvisorHelp1
            // 
            this.tsmiAdvisorHelp1.Name = "tsmiAdvisorHelp1";
            this.tsmiAdvisorHelp1.Size = new System.Drawing.Size(133, 6);
            // 
            // tsmiAbout
            // 
            this.tsmiAbout.Name = "tsmiAbout";
            this.tsmiAbout.Size = new System.Drawing.Size(136, 22);
            this.tsmiAbout.Text = "About";
            this.tsmiAbout.Click += new System.EventHandler(this.tsmiAbout_Click);
            // 
            // tsmiTest
            // 
            this.tsmiTest.Name = "tsmiTest";
            this.tsmiTest.Size = new System.Drawing.Size(136, 22);
            this.tsmiTest.Text = "Test";
            this.tsmiTest.Click += new System.EventHandler(this.tsmiTest_Click);
            // 
            // panelControl
            // 
            this.panelControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelControl.Controls.Add(this.aeAnnotationTextEditor);
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl.Location = new System.Drawing.Point(0, 0);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(991, 423);
            this.panelControl.TabIndex = 1;
            // 
            // aeAnnotationTextEditor
            // 
            this.aeAnnotationTextEditor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aeAnnotationTextEditor.Location = new System.Drawing.Point(4, 3);
            this.aeAnnotationTextEditor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.aeAnnotationTextEditor.MinSize = new System.Drawing.Size(0, 0);
            this.aeAnnotationTextEditor.Name = "aeAnnotationTextEditor";
            this.aeAnnotationTextEditor.ParentArea = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.aeAnnotationTextEditor.Size = new System.Drawing.Size(150, 75);
            this.aeAnnotationTextEditor.TabIndex = 6;
            this.aeAnnotationTextEditor.Visible = false;
            // 
            // cmsAnnotation
            // 
            this.cmsAnnotation.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEditAnnotation,
            this.tsmiResetAnnotation,
            this.tsmiDividerAnnotation1,
            this.tsmiAnnotationSettings,
            this.tsmiDividerAnnotation2,
            this.tsmiDeleteAnnotation});
            this.cmsAnnotation.Name = "cmsWidget";
            this.cmsAnnotation.Size = new System.Drawing.Size(117, 104);
            // 
            // tsmiEditAnnotation
            // 
            this.tsmiEditAnnotation.Name = "tsmiEditAnnotation";
            this.tsmiEditAnnotation.Size = new System.Drawing.Size(116, 22);
            this.tsmiEditAnnotation.Text = "Edit";
            this.tsmiEditAnnotation.Click += new System.EventHandler(this.tsmiEditAnnotation_Click);
            // 
            // tsmiResetAnnotation
            // 
            this.tsmiResetAnnotation.Name = "tsmiResetAnnotation";
            this.tsmiResetAnnotation.Size = new System.Drawing.Size(116, 22);
            this.tsmiResetAnnotation.Text = "Reset";
            this.tsmiResetAnnotation.Click += new System.EventHandler(this.tsmiResetAnnotation_Click);
            // 
            // tsmiDividerAnnotation1
            // 
            this.tsmiDividerAnnotation1.Name = "tsmiDividerAnnotation1";
            this.tsmiDividerAnnotation1.Size = new System.Drawing.Size(113, 6);
            // 
            // tsmiAnnotationSettings
            // 
            this.tsmiAnnotationSettings.Name = "tsmiAnnotationSettings";
            this.tsmiAnnotationSettings.Size = new System.Drawing.Size(116, 22);
            this.tsmiAnnotationSettings.Text = "Settings";
            this.tsmiAnnotationSettings.Click += new System.EventHandler(this.tsmiAnnotationSettings_Click);
            // 
            // tsmiDividerAnnotation2
            // 
            this.tsmiDividerAnnotation2.Name = "tsmiDividerAnnotation2";
            this.tsmiDividerAnnotation2.Size = new System.Drawing.Size(113, 6);
            // 
            // tsmiDeleteAnnotation
            // 
            this.tsmiDeleteAnnotation.Name = "tsmiDeleteAnnotation";
            this.tsmiDeleteAnnotation.Size = new System.Drawing.Size(116, 22);
            this.tsmiDeleteAnnotation.Text = "Delete";
            this.tsmiDeleteAnnotation.Click += new System.EventHandler(this.tsmiDeleteAnnotation_Click);
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tspbProgress,
            this.tsslState,
            this.tsslCancel,
            this.tsslEmpty,
            this.tsslUnitSystem});
            this.statusStripMain.Location = new System.Drawing.Point(0, 657);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(1264, 24);
            this.statusStripMain.TabIndex = 2;
            this.statusStripMain.Text = "statusStrip1";
            // 
            // tspbProgress
            // 
            this.tspbProgress.MarqueeAnimationSpeed = 40;
            this.tspbProgress.Name = "tspbProgress";
            this.tspbProgress.Size = new System.Drawing.Size(150, 18);
            // 
            // tsslState
            // 
            this.tsslState.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsslState.Name = "tsslState";
            this.tsslState.Size = new System.Drawing.Size(39, 19);
            this.tsslState.Text = "Ready";
            this.tsslState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslCancel
            // 
            this.tsslCancel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsslCancel.BorderStyle = System.Windows.Forms.Border3DStyle.Raised;
            this.tsslCancel.Name = "tsslCancel";
            this.tsslCancel.Size = new System.Drawing.Size(47, 19);
            this.tsslCancel.Text = "Cancel";
            this.tsslCancel.Visible = false;
            this.tsslCancel.Click += new System.EventHandler(this.tsslCancel_Click);
            this.tsslCancel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsslCancel_MouseDown);
            this.tsslCancel.MouseLeave += new System.EventHandler(this.tsslCancel_MouseLeave);
            this.tsslCancel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tsslCancel_MouseUp);
            // 
            // tsslEmpty
            // 
            this.tsslEmpty.Name = "tsslEmpty";
            this.tsslEmpty.Size = new System.Drawing.Size(928, 19);
            this.tsslEmpty.Spring = true;
            this.tsslEmpty.Text = " ";
            // 
            // tsslUnitSystem
            // 
            this.tsslUnitSystem.Name = "tsslUnitSystem";
            this.tsslUnitSystem.Size = new System.Drawing.Size(130, 19);
            this.tsslUnitSystem.Text = "Unit system: Undefined";
            this.tsslUnitSystem.Click += new System.EventHandler(this.tsslUnitSystem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1264, 533);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(1264, 633);
            this.toolStripContainer1.TabIndex = 4;
            this.toolStripContainer1.Text = "toolStripContainer";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsModel);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsDeformationFactor);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsFile);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsViews);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsResults);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1MinSize = 250;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2MinSize = 250;
            this.splitContainer1.Size = new System.Drawing.Size(1264, 533);
            this.splitContainer1.SplitterDistance = 269;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panelControl);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tbOutput);
            this.splitContainer2.Size = new System.Drawing.Size(991, 533);
            this.splitContainer2.SplitterDistance = 423;
            this.splitContainer2.TabIndex = 2;
            // 
            // tbOutput
            // 
            this.tbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOutput.BackColor = System.Drawing.Color.White;
            this.tbOutput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbOutput.Location = new System.Drawing.Point(0, 0);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutput.Size = new System.Drawing.Size(991, 105);
            this.tbOutput.TabIndex = 0;
            this.tbOutput.Text = "Output text box";
            // 
            // timerTest
            // 
            this.timerTest.Tick += new System.EventHandler(this.timerTest_Tick);
            // 
            // timerOutput
            // 
            this.timerOutput.Tick += new System.EventHandler(this.timerOutput_Tick);
            // 
            // FrmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.menuStripMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PrePoMax";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.Shown += new System.EventHandler(this.FrmMain_Shown);
            this.Move += new System.EventHandler(this.FrmMain_Move);
            this.Resize += new System.EventHandler(this.FrmMain_Resize);
            this.tsFile.ResumeLayout(false);
            this.tsFile.PerformLayout();
            this.tsViews.ResumeLayout(false);
            this.tsViews.PerformLayout();
            this.tsModel.ResumeLayout(false);
            this.tsModel.PerformLayout();
            this.tsDeformationFactor.ResumeLayout(false);
            this.tsDeformationFactor.PerformLayout();
            this.tsResults.ResumeLayout(false);
            this.tsResults.PerformLayout();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.panelControl.ResumeLayout(false);
            this.cmsAnnotation.ResumeLayout(false);
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }





        #endregion

        private UserControls.ToolStripFocus tsFile;
        private UserControls.ToolStripFocus tsViews;
        private UserControls.ToolStripFocus tsModel;
        private UserControls.ToolStripFocus tsDeformationFactor;
        private UserControls.ToolStripFocus tsResults;
        private UserControls.MenuStripFocus menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        private System.Windows.Forms.Panel panelControl;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripStatusLabel tsslState;
        private System.Windows.Forms.ToolStripMenuItem tsmiNew;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerFile3;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerFile1;
        private System.Windows.Forms.ToolStripMenuItem tsmiSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpen;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveAs;
        private System.Windows.Forms.ToolStripMenuItem tsmiView;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripButton tsbTopView;
        private System.Windows.Forms.ToolStripButton tsbFrontView;
        private System.Windows.Forms.ToolStripButton tsbBackView;
        private System.Windows.Forms.ToolStripButton tsbBottomView;
        private System.Windows.Forms.ToolStripButton tsbLeftView;
        private System.Windows.Forms.ToolStripButton tsbRightView;
        private System.Windows.Forms.ToolStripSeparator toolStripViewSeparator1;
        private System.Windows.Forms.ToolStripButton tsbZoomToFit;
        private System.Windows.Forms.ToolStripButton tsbIsometric;
        private System.Windows.Forms.ToolStripMenuItem standardViewsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiFrontView;
        private System.Windows.Forms.ToolStripMenuItem tsmiBackView;
        private System.Windows.Forms.ToolStripMenuItem tsmiTopView;
        private System.Windows.Forms.ToolStripMenuItem tsmiBottomView;
        private System.Windows.Forms.ToolStripMenuItem tsmiLeftView;
        private System.Windows.Forms.ToolStripMenuItem tsmiRightView;
        private System.Windows.Forms.ToolStripMenuItem tsmiIsometricView;
        private System.Windows.Forms.ToolStripButton tsbNew;
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiZoomToFit;
        private System.Windows.Forms.ToolStripSeparator toolStripViewSeparator2;
        private System.Windows.Forms.ToolStripButton tsbShowElementEdges;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowElementEdges;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem tsmiPart;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteParts;
        private System.Windows.Forms.ToolStripButton tsbImport;
        private System.Windows.Forms.ToolStripMenuItem tsmiStep;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerFile2;
        private System.Windows.Forms.ToolStripMenuItem tsmiExport;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportToCalculix;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditPart;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateStep;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditStep;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerStep2;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteStep;
        private System.Windows.Forms.ToolStripMenuItem tsmiProperty;
        private System.Windows.Forms.ToolStripMenuItem tsmiMaterial;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateMaterial;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditMaterial;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerMaterial1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteMaterial;
        private System.Windows.Forms.ToolStripMenuItem tsmiSection;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateSection;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditSection;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerSection1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripMenuItem tsmiLoad;
        private System.Windows.Forms.ToolStripMenuItem tsmiBC;
        private System.Windows.Forms.ToolStripMenuItem tsmiStepMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiFieldOutput;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateFieldOutput;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditFieldOutput;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerFieldOutput1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteFieldOutput;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateLoad;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditLoad;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerLoad1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteLoad;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateBC;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditBC;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerBC1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteBC;
        private System.Windows.Forms.ToolStripMenuItem tsmiModel;
        private System.Windows.Forms.ToolStripMenuItem tsmiSurface;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateSurface;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditSurface;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerSurface1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteSurface;
        private System.Windows.Forms.ToolStripMenuItem tsmiTools;
        private System.Windows.Forms.ToolStripMenuItem tsmiSettings;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerStep1;
        private System.Windows.Forms.ToolStripMenuItem tsmiAnalysis;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateAnalysis;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditAnalysis;
        private System.Windows.Forms.ToolStripMenuItem tsmiRunAnalysis;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerAnalysis2;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteAnalysis;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerAnalysis1;
        private System.Windows.Forms.ToolStripMenuItem tsmiMonitorAnalysis;
        private System.Windows.Forms.ToolStripMenuItem tsmiKillAnalysis;
        private System.Windows.Forms.ToolStripMenuItem tsmiResultsAnalysis;
        private System.Windows.Forms.ToolStripMenuItem tsmiCloseAllResults;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportToAbaqus;
        private System.Windows.Forms.ToolStripButton tsbVerticalView;
        private System.Windows.Forms.ToolStripMenuItem tsmiVerticalView;
        private System.Windows.Forms.ToolStripButton tsbNormalView;
        private System.Windows.Forms.ToolStripMenuItem tsmiNormalView;
        private System.Windows.Forms.ToolStripMenuItem tsmiRegenerate;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiUndo;
        private System.Windows.Forms.ToolStripMenuItem tsmiRedo;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerEdit1;
        private System.Windows.Forms.ToolStripMenuItem tsmiReferencePointTool;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateRP;
        private System.Windows.Forms.ToolStripSeparator tsmiRP;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteRP;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditRP;
        private System.Windows.Forms.ToolStripMenuItem tsmiInteraction;
        private System.Windows.Forms.ToolStripMenuItem tsmiConstraint;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateConstraint;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditConstraint;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerConstraint2;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteConstraint;
        private System.Windows.Forms.ToolStripMenuItem tsmiNode;
        private System.Windows.Forms.ToolStripMenuItem tsmiRenumberAllNodes;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowModelEdges;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowNoEdges;
        private System.Windows.Forms.ToolStripButton tsbShowModelEdges;
        private System.Windows.Forms.ToolStripButton tsbShowNoEdges;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerView1;
        private System.Windows.Forms.ToolStripMenuItem tsmiViewHistory;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.ToolStripMenuItem tsmiQuery;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowWireframeEdges;
        private System.Windows.Forms.ToolStripButton tsbShowWireframeEdges;
        private System.Windows.Forms.ToolStripMenuItem tsmiNodeSet;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateNodeSet;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditNodeSet;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerNodeSet1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteNodeSet;
        private System.Windows.Forms.ToolStripMenuItem tsmiElementSet;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateElementSet;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditElementSet;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerElementSet1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteElementSet;
        private System.Windows.Forms.ToolStripMenuItem tsmiRegenerateUsingOtherFiles;
        private System.Windows.Forms.ToolStripProgressBar tspbProgress;
        private System.Windows.Forms.ToolStripMenuItem tsmiHideParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowParts;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerPart3;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditCalculiXKeywords;
        private System.Windows.Forms.ToolStripLabel tslStepIncrement;
        private System.Windows.Forms.ToolStripComboBox tscbStepAndIncrement;
        private System.Windows.Forms.ToolStripButton tsbPreviousStepIncrement;
        private System.Windows.Forms.ToolStripButton tsbNextStepIncrement;
        private System.Windows.Forms.ToolStripButton tsbFirstStepIncrement;
        private System.Windows.Forms.ToolStripButton tsbLastStepIncrement;
        private System.Windows.Forms.ToolStripMenuItem tsmiGeometry;
        private System.Windows.Forms.ToolStripMenuItem tsmiGeometryPart;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditGeometryPart;
        private System.Windows.Forms.ToolStripMenuItem tsmiHideGeometryParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowGeometryParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteGeometryParts;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerGeomPart1;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerGeomPart2;
        private System.Windows.Forms.ToolStripStatusLabel tsslCancel;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateMesh;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerConstraint1;
        private System.Windows.Forms.ToolStripMenuItem tsmiHideConstraint;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowConstraint;
        private System.Windows.Forms.ToolStripMenuItem tsmiHideBC;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowBC;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerBC2;
        private System.Windows.Forms.ToolStripMenuItem tsmiHideLoad;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowLoad;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerLoad2;
        private System.Windows.Forms.ToolStripMenuItem tsmiRegenerateForRemeshing;
        private System.Windows.Forms.ToolStripButton tsbAnimate;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerGeometry1;
        private System.Windows.Forms.ToolStripMenuItem tsmiMeshingParameters;
        private System.Windows.Forms.ToolStripMenuItem tsmiResults;
        private System.Windows.Forms.ToolStripMenuItem tsmiResultPart;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditResultPart;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerResultPart1;
        private System.Windows.Forms.ToolStripMenuItem tsmiHideResultParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowResultParts;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerResultPart2;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteResultParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyGeometryPartsToResults;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerGeomPart3;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerView2;
        private System.Windows.Forms.ToolStripMenuItem tsmiHideAllParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowAllParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiInvertVisibleParts;
        private System.Windows.Forms.ToolStripSeparator toolStripViewSeparator3;
        private System.Windows.Forms.ToolStripButton tsbHideAllParts;
        private System.Windows.Forms.ToolStripButton tsbShowAllParts;
        private System.Windows.Forms.ToolStripButton tsbInvertVisibleParts;
        private System.Windows.Forms.ToolStripButton tsbResultsUndeformed;
        private System.Windows.Forms.ToolStripButton tsbResultsDeformed;
        private System.Windows.Forms.ToolStripButton tsbResultsColorContours;
        private System.Windows.Forms.ToolStripSeparator toolStripResultsSeparator1;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerView3;
        private System.Windows.Forms.ToolStripMenuItem tsmiResultsUndeformed;
        private System.Windows.Forms.ToolStripMenuItem tsmiResultsDeformed;
        private System.Windows.Forms.ToolStripMenuItem tsmiResultsColorContours;
        private System.Windows.Forms.ToolStripMenuItem tsmiHelp;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbout;
        private System.Windows.Forms.ToolStripMenuItem tsmiHomePage;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorContoursOff;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorContoursOn;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerResultPart3;
        private System.Windows.Forms.ToolStripMenuItem tsmiMaterialLibrary;
        private System.Windows.Forms.ToolStripMenuItem tsmiMergeParts;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerPart2;
        private System.Windows.Forms.ToolStripMenuItem tsmiConvertElementSetsToMeshParts;
        private System.Windows.Forms.ToolStripSeparator toolStripViewSeparator4;
        private System.Windows.Forms.ToolStripLabel tslSymbols;
        private System.Windows.Forms.ToolStripComboBox tscbSymbolsForStep;
        private System.Windows.Forms.ToolStripMenuItem tsmiGeometryAnalyze;
        private System.Windows.Forms.ToolStripMenuItem tsmiHistoryOutput;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateHistoryOutput;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditHistoryOutput;
        private System.Windows.Forms.ToolStripSeparator tsmiHistoryOutputDivider1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteHistoryOutput;
        private System.Windows.Forms.ToolStripMenuItem tsmiTransformModelParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiTranslateModelParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiScaleModelParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiRotateModelParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiSectionView;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerView4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbSectionView;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetTransparencyForGeometryParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetTransparencyForParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetTransparencyForResultParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiTest;
        private System.Windows.Forms.Timer timerTest;
        private System.Windows.Forms.Timer timerOutput;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditModel;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerModel1;
        private System.Windows.Forms.ToolStripMenuItem tsmiPreviewEdgeMesh;
        private System.Windows.Forms.ToolStripMenuItem tsmiMesh;
        private System.Windows.Forms.ToolStripMenuItem tsmiMeshRefinement;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateMeshRefinement;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditMeshRefinement;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerMeshRefinement1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteMeshRefinement;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateAndImportCompoundPart;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportToStep;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportToBrep;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowOnlyGeometryParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowOnlyParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowOnlyResultParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateBoundaryLayer;
        private System.Windows.Forms.ToolStripMenuItem tsmiSurfaceInteraction;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateSurfaceInteraction;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditSurfaceInteraction;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerSurfaceInteraction1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteSurfaceInteraction;
        private System.Windows.Forms.ToolStripMenuItem contactPairToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateContactPair;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditContactPair;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerContactPair1;
        private System.Windows.Forms.ToolStripMenuItem tsmiHideContactPair;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowContactPair;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerContactPair2;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteContactPair;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenRecent;
        private System.Windows.Forms.ToolStripMenuItem tsmiDuplicateMaterial;
        private System.Windows.Forms.ToolStripMenuItem tsmiContact;
        private System.Windows.Forms.ToolStripMenuItem tsmiDuplicateSurfaceInteraction;
        private System.Windows.Forms.ToolStripMenuItem tsmiDuplicateStep;
        private System.Windows.Forms.ToolStripStatusLabel tsslEmpty;
        private System.Windows.Forms.ToolStripStatusLabel tsslUnitSystem;
        private System.Windows.Forms.ToolStripMenuItem tsmiAdvisor;
        private System.Windows.Forms.ToolStripSeparator tsmiAdvisorHelp1;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerResults1;
        private System.Windows.Forms.ToolStripMenuItem tsmiTransformation;
        private System.Windows.Forms.ToolStripSeparator toolStripResultsSeparator2;
        private System.Windows.Forms.ToolStripButton tsbTransformation;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerView5;
        private System.Windows.Forms.ToolStripMenuItem tsmiAnnotateFaceOrientations;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerGeometry2;
        private System.Windows.Forms.ToolStripMenuItem tsmiFlipFaceNormalCAD;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportToMmgMesh;
        private System.Windows.Forms.ToolStripMenuItem tsmiSplitAFaceUsingTwoPoints;
        private System.Windows.Forms.ToolStripMenuItem tsmiCropWithCylinder;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportToStereolitography;
        private System.Windows.Forms.ToolStripMenuItem tsmiDuplicateElementSet;
        private System.Windows.Forms.ToolStripMenuItem tsmiDuplicateNodeSet;
        private System.Windows.Forms.ToolStripMenuItem tsmiAnnotateParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiAnnotateAllSymbols;
        private System.Windows.Forms.ToolStripMenuItem tsmiAnnotateReferencePoints;
        private System.Windows.Forms.ToolStripMenuItem tsmiAnnotateConstraints;
        private System.Windows.Forms.ToolStripMenuItem tsmiAnnotateLoads;
        private System.Windows.Forms.ToolStripMenuItem tsmiAnnotateBCs;
        private System.Windows.Forms.ToolStripMenuItem tsmiAnnotateContactPairs;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorAnnotations;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerColorAnnotations1;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerColorAnnotations2;
        private System.Windows.Forms.ToolStripMenuItem tsmiAnnotateSections;
        private System.Windows.Forms.ToolStripMenuItem tsmiAnnotateMaterials;
        private System.Windows.Forms.ToolStripMenuItem tsmiAnnotateSectionThicknesses;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemeshElements;
        private System.Windows.Forms.ToolStripMenuItem tsmiFindEdgesByAngleForGeometryParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiFindEdgesByAngleForModelParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiToolsParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiPropagateBC;
        private System.Windows.Forms.ToolStripMenuItem tsmiPropagateHistoryOutput;
        private System.Windows.Forms.ToolStripMenuItem tsmiPropagateFieldOutput;
        private System.Windows.Forms.ToolStripMenuItem tsmiPropagateLoad;
        private System.Windows.Forms.ToolStripMenuItem tsmiCropWithCube;
        private System.Windows.Forms.ToolStripMenuItem tsmiSwapGeometryPartGeometries;
        private System.Windows.Forms.ToolStripButton tsbExplodedView;
        private System.Windows.Forms.ToolStripMenuItem tsmiExplodedView;
        private System.Windows.Forms.ToolStripMenuItem tsmiInitialCondition;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateInitialCondition;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditInitialCondition;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerInitialCondition1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteInitialCondition;
        private System.Windows.Forms.ToolStripMenuItem tsmiDefinedField;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateDefinedField;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditDefinedField;
        private System.Windows.Forms.ToolStripMenuItem tsmiPropagateDefinedField;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerDefinedField1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteDefinedField;
        private System.Windows.Forms.ToolStripMenuItem tsmiSearchContactPairs;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerInteraction1;
        private System.Windows.Forms.ToolStripMenuItem tsmiSwapMasterSlaveConstraint;
        private System.Windows.Forms.ToolStripMenuItem tsmiMergeByMasterSlaveConstraint;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerConstraint3;
        private System.Windows.Forms.ToolStripMenuItem tsmiSwapMasterSlaveContactPair;
        private System.Windows.Forms.ToolStripMenuItem tsmiMergeByMasterSlaveContactPair;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerContactPair3;
        private System.Windows.Forms.ToolStripMenuItem tsmiTransformGeometryParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiScaleGeometryParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiElement;
        private System.Windows.Forms.ToolStripMenuItem tsmiRenumberAllElements;
        private System.Windows.Forms.ToolStripMenuItem tsmiStlPart;
        private System.Windows.Forms.ToolStripMenuItem cADPartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiFlipStlPartSurfacesNormal;
        private System.Windows.Forms.ToolStripMenuItem tsmiSmoothPart;
        private System.Windows.Forms.ToolStripMenuItem tsmiRegenerateCompoundPart;
        private System.Windows.Forms.ToolStripMenuItem tsmiResultFieldOutput;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteResultFieldOutput;
        private System.Windows.Forms.ToolStripMenuItem tsmiResultHistoryOutput;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateResultHistoryOutput;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerResultHistoryOutput1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteResultHistoryOutput;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerResults2;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerExport1;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportToDeformedInp;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportToDeformedStl;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerExport2;
        private System.Windows.Forms.ToolStripMenuItem tsmiUpdateNodalCoordinatesFromFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportMaterial;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportMaterial;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerMaterial2;
        private System.Windows.Forms.ToolStripMenuItem tsmiAmplitude;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateAmplitude;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditAmplitude;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerAmplitude1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteAmplitude;
        private UserControls.AnnotationEditor aeAnnotationTextEditor;
        private System.Windows.Forms.ContextMenuStrip cmsAnnotation;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteAnnotation;
        private System.Windows.Forms.ToolStripMenuItem tsmiAnnotationSettings;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerAnnotation1;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditAnnotation;
        private System.Windows.Forms.ToolStripMenuItem tsmiResetAnnotation;
        private System.Windows.Forms.ToolStripSeparator tsmiDividerAnnotation2;
        private System.Windows.Forms.ToolStripLabel tslDeformationVariable;
        private System.Windows.Forms.ToolStripComboBox tscbDeformationVariable;
        private System.Windows.Forms.ToolStripLabel tslDeformationType;
        private System.Windows.Forms.ToolStripComboBox tscbDeformationType;
        private UserControls.NumericToolStripTextBox tstbDeformationFactor;
        private System.Windows.Forms.ToolStripLabel tslDeformationFactor;
        private System.Windows.Forms.ToolStripButton tsbQuery;
        private System.Windows.Forms.ToolStripButton tsbRemoveAnnotations;
        private System.Windows.Forms.ToolStripLabel tslResultName;
        private System.Windows.Forms.ToolStripComboBox tscbResultNames;
        private System.Windows.Forms.ToolStripMenuItem tsmiCloseCurrentResult;
        private System.Windows.Forms.ToolStripButton tsbResultsUndeformedWireframe;
        private System.Windows.Forms.ToolStripButton tsbResultsUndeformedSolid;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsmiPreviewLoad;
        private System.Windows.Forms.ToolStripMenuItem tsmiPreviewInitialCondition;
        private System.Windows.Forms.ToolStripMenuItem tsmiPreviewDefinedField;
    }
}

