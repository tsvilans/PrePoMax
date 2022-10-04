namespace UserControls
{
    partial class ModelTree
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelTree));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Solid-Part-1");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Solid-Part-2");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Solid-Part-4");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Solid-Part-5");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Comp1", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Solid-Part-6");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Solid-Part-7");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Comp2", new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7});
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Solid-Part-10");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Parts", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode5,
            treeNode8,
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Mesh refinements");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Parts");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Node sets");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Element sets");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Surfaces");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Reference points");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Mesh", new System.Windows.Forms.TreeNode[] {
            treeNode12,
            treeNode13,
            treeNode14,
            treeNode15,
            treeNode16});
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Materials");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Sections");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Constraints");
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Surface interactions");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("Contact pairs");
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("Contacts", new System.Windows.Forms.TreeNode[] {
            treeNode21,
            treeNode22});
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("Amplitudes");
            System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("Initial conditions");
            System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("Steps");
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("Model", new System.Windows.Forms.TreeNode[] {
            treeNode17,
            treeNode18,
            treeNode19,
            treeNode20,
            treeNode23,
            treeNode24,
            treeNode25,
            treeNode26});
            System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("Analyses");
            System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("Parts");
            System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("Node sets");
            System.Windows.Forms.TreeNode treeNode31 = new System.Windows.Forms.TreeNode("Element sets");
            System.Windows.Forms.TreeNode treeNode32 = new System.Windows.Forms.TreeNode("Surfaces");
            System.Windows.Forms.TreeNode treeNode33 = new System.Windows.Forms.TreeNode("Mesh", new System.Windows.Forms.TreeNode[] {
            treeNode29,
            treeNode30,
            treeNode31,
            treeNode32});
            System.Windows.Forms.TreeNode treeNode34 = new System.Windows.Forms.TreeNode("Field outputs");
            System.Windows.Forms.TreeNode treeNode35 = new System.Windows.Forms.TreeNode("History outputs");
            System.Windows.Forms.TreeNode treeNode36 = new System.Windows.Forms.TreeNode("Results", new System.Windows.Forms.TreeNode[] {
            treeNode34,
            treeNode35});
            this.cmsTree = new System.Windows.Forms.ContextMenuStrip();
            this.tsmiCreate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQuery = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDuplicate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPropagate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceCompoundPart = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCompoundPart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSwapPartGeometries = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceMesh = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiMeshingParameters = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPreviewEdgeMesh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateMesh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceCopyPart = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCopyGeometryToResults = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceEditCalculiXKeywords = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiEditCalculiXKeywords = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceMergeParts = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiMergeParts = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceConvertToPart = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiConvertToPart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceMaterialLibrary = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiMaterialLibrary = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceSearchContactPairs = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSearchContactPairs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceSwapMergeMasterSlave = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSwapMasterSlave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMergeByMasterSlave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceHideShow = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHide = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetTransparency = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceColorContours = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiColorContoursOff = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorContoursOn = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceAnalysis = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRun = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiResults = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiKill = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceActive = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiActivate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeactivate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceExpandColapse = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpaceDelete = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.ilIcons = new System.Windows.Forms.ImageList();
            this.ilStatusIcons = new System.Windows.Forms.ImageList();
            this.tcGeometryModelResults = new System.Windows.Forms.TabControl();
            this.tpGeometry = new System.Windows.Forms.TabPage();
            this.stbGeometry = new UserControls.SearchTextBox();
            this.cltvGeometry = new UserControls.CodersLabTreeView();
            this.tpModel = new System.Windows.Forms.TabPage();
            this.stbModel = new UserControls.SearchTextBox();
            this.cltvModel = new UserControls.CodersLabTreeView();
            this.tpResults = new System.Windows.Forms.TabPage();
            this.stbResults = new UserControls.SearchTextBox();
            this.cltvResults = new UserControls.CodersLabTreeView();
            this.cmsTree.SuspendLayout();
            this.tcGeometryModelResults.SuspendLayout();
            this.tpGeometry.SuspendLayout();
            this.tpModel.SuspendLayout();
            this.tpResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmsTree
            // 
            this.cmsTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreate,
            this.tsmiEdit,
            this.tsmiQuery,
            this.tsmiDuplicate,
            this.tsmiPropagate,
            this.tsmiPreview,
            this.tsmiSpaceCompoundPart,
            this.tsmiCompoundPart,
            this.tsmiSwapPartGeometries,
            this.tsmiSpaceMesh,
            this.tsmiMeshingParameters,
            this.tsmiPreviewEdgeMesh,
            this.tsmiCreateMesh,
            this.tsmiSpaceCopyPart,
            this.tsmiCopyGeometryToResults,
            this.tsmiSpaceEditCalculiXKeywords,
            this.tsmiEditCalculiXKeywords,
            this.tsmiSpaceMergeParts,
            this.tsmiMergeParts,
            this.tsmiSpaceConvertToPart,
            this.tsmiConvertToPart,
            this.tsmiSpaceMaterialLibrary,
            this.tsmiMaterialLibrary,
            this.tsmiSpaceSearchContactPairs,
            this.tsmiSearchContactPairs,
            this.tsmiSpaceSwapMergeMasterSlave,
            this.tsmiSwapMasterSlave,
            this.tsmiMergeByMasterSlave,
            this.tsmiSpaceHideShow,
            this.tsmiHide,
            this.tsmiShow,
            this.tsmiShowOnly,
            this.tsmiSetTransparency,
            this.tsmiSpaceColorContours,
            this.tsmiColorContoursOff,
            this.tsmiColorContoursOn,
            this.tsmiSpaceAnalysis,
            this.tsmiRun,
            this.tsmiMonitor,
            this.tsmiResults,
            this.tsmiKill,
            this.tsmiSpaceActive,
            this.tsmiActivate,
            this.tsmiDeactivate,
            this.tsmiSpaceExpandColapse,
            this.tsmiExpandAll,
            this.tsmiCollapseAll,
            this.tsmiSpaceDelete,
            this.tsmiDelete});
            this.cmsTree.Name = "cmsTree";
            this.cmsTree.Size = new System.Drawing.Size(212, 864);
            // 
            // tsmiCreate
            // 
            this.tsmiCreate.Name = "tsmiCreate";
            this.tsmiCreate.Size = new System.Drawing.Size(211, 22);
            this.tsmiCreate.Text = "Create";
            this.tsmiCreate.Click += new System.EventHandler(this.tsmiCreate_Click);
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.Name = "tsmiEdit";
            this.tsmiEdit.Size = new System.Drawing.Size(211, 22);
            this.tsmiEdit.Text = "Edit";
            this.tsmiEdit.Click += new System.EventHandler(this.tsmiEdit_Click);
            // 
            // tsmiQuery
            // 
            this.tsmiQuery.Image = global::UserControls.Properties.Resources.Query;
            this.tsmiQuery.Name = "tsmiQuery";
            this.tsmiQuery.Size = new System.Drawing.Size(211, 22);
            this.tsmiQuery.Text = "Query";
            this.tsmiQuery.Click += new System.EventHandler(this.tsmiQuery_Click);
            // 
            // tsmiDuplicate
            // 
            this.tsmiDuplicate.Name = "tsmiDuplicate";
            this.tsmiDuplicate.Size = new System.Drawing.Size(211, 22);
            this.tsmiDuplicate.Text = "Duplicate";
            this.tsmiDuplicate.Click += new System.EventHandler(this.tsmiDuplicate_Click);
            // 
            // tsmiPropagate
            // 
            this.tsmiPropagate.Image = global::UserControls.Properties.Resources.Step;
            this.tsmiPropagate.Name = "tsmiPropagate";
            this.tsmiPropagate.Size = new System.Drawing.Size(211, 22);
            this.tsmiPropagate.Text = "Propagate";
            this.tsmiPropagate.Click += new System.EventHandler(this.tsmiPropagate_Click);
            // 
            // tsmiPreview
            // 
            this.tsmiPreview.Image = global::UserControls.Properties.Resources.Preview_load;
            this.tsmiPreview.Name = "tsmiPreview";
            this.tsmiPreview.Size = new System.Drawing.Size(211, 22);
            this.tsmiPreview.Text = "Preview";
            this.tsmiPreview.Click += new System.EventHandler(this.tsmiPreview_Click);
            // 
            // tsmiSpaceCompoundPart
            // 
            this.tsmiSpaceCompoundPart.Name = "tsmiSpaceCompoundPart";
            this.tsmiSpaceCompoundPart.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiCompoundPart
            // 
            this.tsmiCompoundPart.Name = "tsmiCompoundPart";
            this.tsmiCompoundPart.Size = new System.Drawing.Size(211, 22);
            this.tsmiCompoundPart.Text = "Create Compound Part";
            this.tsmiCompoundPart.Click += new System.EventHandler(this.tsmiCompoundPart_Click);
            // 
            // tsmiSwapPartGeometries
            // 
            this.tsmiSwapPartGeometries.Name = "tsmiSwapPartGeometries";
            this.tsmiSwapPartGeometries.Size = new System.Drawing.Size(211, 22);
            this.tsmiSwapPartGeometries.Text = "Swap Part Geometries";
            this.tsmiSwapPartGeometries.Click += new System.EventHandler(this.tsmiSwapPartGeometries_Click);
            // 
            // tsmiSpaceMesh
            // 
            this.tsmiSpaceMesh.Name = "tsmiSpaceMesh";
            this.tsmiSpaceMesh.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiMeshingParameters
            // 
            this.tsmiMeshingParameters.Name = "tsmiMeshingParameters";
            this.tsmiMeshingParameters.Size = new System.Drawing.Size(211, 22);
            this.tsmiMeshingParameters.Text = "Meshing Parameters";
            this.tsmiMeshingParameters.Click += new System.EventHandler(this.tsmiMeshingParameters_Click);
            // 
            // tsmiPreviewEdgeMesh
            // 
            this.tsmiPreviewEdgeMesh.Name = "tsmiPreviewEdgeMesh";
            this.tsmiPreviewEdgeMesh.Size = new System.Drawing.Size(211, 22);
            this.tsmiPreviewEdgeMesh.Text = "Preview Edge Mesh";
            this.tsmiPreviewEdgeMesh.Click += new System.EventHandler(this.tsmiPreviewEdgeMesh_Click);
            // 
            // tsmiCreateMesh
            // 
            this.tsmiCreateMesh.Image = global::UserControls.Properties.Resources.Part;
            this.tsmiCreateMesh.Name = "tsmiCreateMesh";
            this.tsmiCreateMesh.Size = new System.Drawing.Size(211, 22);
            this.tsmiCreateMesh.Text = "Create Mesh";
            this.tsmiCreateMesh.Click += new System.EventHandler(this.tsmiCreateMesh_Click);
            // 
            // tsmiSpaceCopyPart
            // 
            this.tsmiSpaceCopyPart.Name = "tsmiSpaceCopyPart";
            this.tsmiSpaceCopyPart.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiCopyGeometryToResults
            // 
            this.tsmiCopyGeometryToResults.Name = "tsmiCopyGeometryToResults";
            this.tsmiCopyGeometryToResults.Size = new System.Drawing.Size(211, 22);
            this.tsmiCopyGeometryToResults.Text = "Copy Geometry to Results";
            this.tsmiCopyGeometryToResults.Click += new System.EventHandler(this.tsmiCopyGeometryPartToResults_Click);
            // 
            // tsmiSpaceEditCalculiXKeywords
            // 
            this.tsmiSpaceEditCalculiXKeywords.Name = "tsmiSpaceEditCalculiXKeywords";
            this.tsmiSpaceEditCalculiXKeywords.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiEditCalculiXKeywords
            // 
            this.tsmiEditCalculiXKeywords.Name = "tsmiEditCalculiXKeywords";
            this.tsmiEditCalculiXKeywords.Size = new System.Drawing.Size(211, 22);
            this.tsmiEditCalculiXKeywords.Text = "Edit CalculiX Keywords";
            this.tsmiEditCalculiXKeywords.Click += new System.EventHandler(this.tsmiEditCalculiXKeywords_Click);
            // 
            // tsmiSpaceMergeParts
            // 
            this.tsmiSpaceMergeParts.Name = "tsmiSpaceMergeParts";
            this.tsmiSpaceMergeParts.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiMergeParts
            // 
            this.tsmiMergeParts.Name = "tsmiMergeParts";
            this.tsmiMergeParts.Size = new System.Drawing.Size(211, 22);
            this.tsmiMergeParts.Text = "Merge Parts";
            this.tsmiMergeParts.Click += new System.EventHandler(this.tsmiMergeParts_Click);
            // 
            // tsmiSpaceConvertToPart
            // 
            this.tsmiSpaceConvertToPart.Name = "tsmiSpaceConvertToPart";
            this.tsmiSpaceConvertToPart.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiConvertToPart
            // 
            this.tsmiConvertToPart.Name = "tsmiConvertToPart";
            this.tsmiConvertToPart.Size = new System.Drawing.Size(211, 22);
            this.tsmiConvertToPart.Text = "Convert to Part";
            this.tsmiConvertToPart.Click += new System.EventHandler(this.tsmiConvertToPart_Click);
            // 
            // tsmiSpaceMaterialLibrary
            // 
            this.tsmiSpaceMaterialLibrary.Name = "tsmiSpaceMaterialLibrary";
            this.tsmiSpaceMaterialLibrary.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiMaterialLibrary
            // 
            this.tsmiMaterialLibrary.Image = global::UserControls.Properties.Resources.Library;
            this.tsmiMaterialLibrary.Name = "tsmiMaterialLibrary";
            this.tsmiMaterialLibrary.Size = new System.Drawing.Size(211, 22);
            this.tsmiMaterialLibrary.Text = "Material Library";
            this.tsmiMaterialLibrary.Click += new System.EventHandler(this.tsmiMaterialLibrary_Click);
            // 
            // tsmiSpaceSearchContactPairs
            // 
            this.tsmiSpaceSearchContactPairs.Name = "tsmiSpaceSearchContactPairs";
            this.tsmiSpaceSearchContactPairs.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiSearchContactPairs
            // 
            this.tsmiSearchContactPairs.Name = "tsmiSearchContactPairs";
            this.tsmiSearchContactPairs.Size = new System.Drawing.Size(211, 22);
            this.tsmiSearchContactPairs.Text = "Search Contact Pairs";
            this.tsmiSearchContactPairs.Click += new System.EventHandler(this.tsmiSearchContactPairs_Click);
            // 
            // tsmiSpaceSwapMergeMasterSlave
            // 
            this.tsmiSpaceSwapMergeMasterSlave.Name = "tsmiSpaceSwapMergeMasterSlave";
            this.tsmiSpaceSwapMergeMasterSlave.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiSwapMasterSlave
            // 
            this.tsmiSwapMasterSlave.Name = "tsmiSwapMasterSlave";
            this.tsmiSwapMasterSlave.Size = new System.Drawing.Size(211, 22);
            this.tsmiSwapMasterSlave.Text = "Swap Master/Slave";
            this.tsmiSwapMasterSlave.Click += new System.EventHandler(this.tsmiSwapMasterSlave_Click);
            // 
            // tsmiMergeByMasterSlave
            // 
            this.tsmiMergeByMasterSlave.Name = "tsmiMergeByMasterSlave";
            this.tsmiMergeByMasterSlave.Size = new System.Drawing.Size(211, 22);
            this.tsmiMergeByMasterSlave.Text = "Merge by Master/Slave";
            this.tsmiMergeByMasterSlave.Click += new System.EventHandler(this.tsmiMergeByMasterSlave_Click);
            // 
            // tsmiSpaceHideShow
            // 
            this.tsmiSpaceHideShow.Name = "tsmiSpaceHideShow";
            this.tsmiSpaceHideShow.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiHide
            // 
            this.tsmiHide.Image = global::UserControls.Properties.Resources.Hide;
            this.tsmiHide.Name = "tsmiHide";
            this.tsmiHide.Size = new System.Drawing.Size(211, 22);
            this.tsmiHide.Text = "Hide";
            this.tsmiHide.Click += new System.EventHandler(this.tsmiHideShow_Click);
            // 
            // tsmiShow
            // 
            this.tsmiShow.Image = global::UserControls.Properties.Resources.Show;
            this.tsmiShow.Name = "tsmiShow";
            this.tsmiShow.Size = new System.Drawing.Size(211, 22);
            this.tsmiShow.Text = "Show";
            this.tsmiShow.Click += new System.EventHandler(this.tsmiHideShow_Click);
            // 
            // tsmiShowOnly
            // 
            this.tsmiShowOnly.Image = global::UserControls.Properties.Resources.Show;
            this.tsmiShowOnly.Name = "tsmiShowOnly";
            this.tsmiShowOnly.Size = new System.Drawing.Size(211, 22);
            this.tsmiShowOnly.Text = "Show Only";
            this.tsmiShowOnly.Click += new System.EventHandler(this.tsmiHideShow_Click);
            // 
            // tsmiSetTransparency
            // 
            this.tsmiSetTransparency.Name = "tsmiSetTransparency";
            this.tsmiSetTransparency.Size = new System.Drawing.Size(211, 22);
            this.tsmiSetTransparency.Text = "Set Transparency";
            this.tsmiSetTransparency.Click += new System.EventHandler(this.tsmiSetTransparency_Click);
            // 
            // tsmiSpaceColorContours
            // 
            this.tsmiSpaceColorContours.Name = "tsmiSpaceColorContours";
            this.tsmiSpaceColorContours.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiColorContoursOff
            // 
            this.tsmiColorContoursOff.Image = global::UserControls.Properties.Resources.Deformed;
            this.tsmiColorContoursOff.Name = "tsmiColorContoursOff";
            this.tsmiColorContoursOff.Size = new System.Drawing.Size(211, 22);
            this.tsmiColorContoursOff.Text = "Color Contours off";
            this.tsmiColorContoursOff.Click += new System.EventHandler(this.tsmiResultColorContoutsVisibility_Click);
            // 
            // tsmiColorContoursOn
            // 
            this.tsmiColorContoursOn.Image = global::UserControls.Properties.Resources.Color_contours;
            this.tsmiColorContoursOn.Name = "tsmiColorContoursOn";
            this.tsmiColorContoursOn.Size = new System.Drawing.Size(211, 22);
            this.tsmiColorContoursOn.Text = "Color Contours on";
            this.tsmiColorContoursOn.Click += new System.EventHandler(this.tsmiResultColorContoutsVisibility_Click);
            // 
            // tsmiSpaceAnalysis
            // 
            this.tsmiSpaceAnalysis.Name = "tsmiSpaceAnalysis";
            this.tsmiSpaceAnalysis.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiRun
            // 
            this.tsmiRun.Name = "tsmiRun";
            this.tsmiRun.Size = new System.Drawing.Size(211, 22);
            this.tsmiRun.Text = "Run";
            this.tsmiRun.Click += new System.EventHandler(this.tsmiRun_Click);
            // 
            // tsmiMonitor
            // 
            this.tsmiMonitor.Name = "tsmiMonitor";
            this.tsmiMonitor.Size = new System.Drawing.Size(211, 22);
            this.tsmiMonitor.Text = "Monitor";
            this.tsmiMonitor.Click += new System.EventHandler(this.tsmiMonitor_Click);
            // 
            // tsmiResults
            // 
            this.tsmiResults.Name = "tsmiResults";
            this.tsmiResults.Size = new System.Drawing.Size(211, 22);
            this.tsmiResults.Text = "Results";
            this.tsmiResults.Click += new System.EventHandler(this.tsmiResults_Click);
            // 
            // tsmiKill
            // 
            this.tsmiKill.Name = "tsmiKill";
            this.tsmiKill.Size = new System.Drawing.Size(211, 22);
            this.tsmiKill.Text = "Kill";
            this.tsmiKill.Click += new System.EventHandler(this.tsmiKill_Click);
            // 
            // tsmiSpaceActive
            // 
            this.tsmiSpaceActive.Name = "tsmiSpaceActive";
            this.tsmiSpaceActive.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiActivate
            // 
            this.tsmiActivate.Name = "tsmiActivate";
            this.tsmiActivate.Size = new System.Drawing.Size(211, 22);
            this.tsmiActivate.Text = "Activate";
            this.tsmiActivate.Click += new System.EventHandler(this.ActivateDeactivate_Click);
            // 
            // tsmiDeactivate
            // 
            this.tsmiDeactivate.Image = global::UserControls.Properties.Resources.Unactive;
            this.tsmiDeactivate.Name = "tsmiDeactivate";
            this.tsmiDeactivate.Size = new System.Drawing.Size(211, 22);
            this.tsmiDeactivate.Text = "Deactivate";
            this.tsmiDeactivate.Click += new System.EventHandler(this.ActivateDeactivate_Click);
            // 
            // tsmiSpaceExpandColapse
            // 
            this.tsmiSpaceExpandColapse.Name = "tsmiSpaceExpandColapse";
            this.tsmiSpaceExpandColapse.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiExpandAll
            // 
            this.tsmiExpandAll.Name = "tsmiExpandAll";
            this.tsmiExpandAll.Size = new System.Drawing.Size(211, 22);
            this.tsmiExpandAll.Text = "Expand All";
            this.tsmiExpandAll.Click += new System.EventHandler(this.tsmiExpandAll_Click);
            // 
            // tsmiCollapseAll
            // 
            this.tsmiCollapseAll.Name = "tsmiCollapseAll";
            this.tsmiCollapseAll.Size = new System.Drawing.Size(211, 22);
            this.tsmiCollapseAll.Text = "Collapse All";
            this.tsmiCollapseAll.Click += new System.EventHandler(this.tsmiCollapseAll_Click);
            // 
            // tsmiSpaceDelete
            // 
            this.tsmiSpaceDelete.Name = "tsmiSpaceDelete";
            this.tsmiSpaceDelete.Size = new System.Drawing.Size(208, 6);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(211, 22);
            this.tsmiDelete.Text = "Delete";
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // ilIcons
            // 
            this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilIcons.Images.SetKeyName(0, "Empty.ico");
            this.ilIcons.Images.SetKeyName(1, "Hide.ico");
            this.ilIcons.Images.SetKeyName(2, "Show.ico");
            this.ilIcons.Images.SetKeyName(3, "Unactive.ico");
            this.ilIcons.Images.SetKeyName(4, "Geometry.ico");
            this.ilIcons.Images.SetKeyName(5, "GeomPart.ico");
            this.ilIcons.Images.SetKeyName(6, "Mesh_refinement.ico");
            this.ilIcons.Images.SetKeyName(7, "Mesh.ico");
            this.ilIcons.Images.SetKeyName(8, "BasePart.ico");
            this.ilIcons.Images.SetKeyName(9, "Node_set.ico");
            this.ilIcons.Images.SetKeyName(10, "Element_set.ico");
            this.ilIcons.Images.SetKeyName(11, "Surface.ico");
            this.ilIcons.Images.SetKeyName(12, "Reference_point.ico");
            this.ilIcons.Images.SetKeyName(13, "Material.ico");
            this.ilIcons.Images.SetKeyName(14, "Section.ico");
            this.ilIcons.Images.SetKeyName(15, "Constraints.ico");
            this.ilIcons.Images.SetKeyName(16, "Contact.ico");
            this.ilIcons.Images.SetKeyName(17, "SurfaceInteraction.ico");
            this.ilIcons.Images.SetKeyName(18, "ContactPair.ico");
            this.ilIcons.Images.SetKeyName(19, "Amplitude.ico");
            this.ilIcons.Images.SetKeyName(20, "InitialConditions.ico");
            this.ilIcons.Images.SetKeyName(21, "Step.ico");
            this.ilIcons.Images.SetKeyName(22, "History_output.ico");
            this.ilIcons.Images.SetKeyName(23, "Field_output.ico");
            this.ilIcons.Images.SetKeyName(24, "Bc.ico");
            this.ilIcons.Images.SetKeyName(25, "Load.ico");
            this.ilIcons.Images.SetKeyName(26, "Defined_field.ico");
            this.ilIcons.Images.SetKeyName(27, "Analysis.ico");
            this.ilIcons.Images.SetKeyName(28, "NoResult.ico");
            this.ilIcons.Images.SetKeyName(29, "Running.ico");
            this.ilIcons.Images.SetKeyName(30, "OK.ico");
            this.ilIcons.Images.SetKeyName(31, "Warning.ico");
            this.ilIcons.Images.SetKeyName(32, "Dots.ico");
            this.ilIcons.Images.SetKeyName(33, "Dots_t.ico");
            // 
            // ilStatusIcons
            // 
            this.ilStatusIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilStatusIcons.ImageStream")));
            this.ilStatusIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilStatusIcons.Images.SetKeyName(0, "Unactive.ico");
            this.ilStatusIcons.Images.SetKeyName(1, "Warning.ico");
            // 
            // tcGeometryModelResults
            // 
            this.tcGeometryModelResults.Controls.Add(this.tpGeometry);
            this.tcGeometryModelResults.Controls.Add(this.tpModel);
            this.tcGeometryModelResults.Controls.Add(this.tpResults);
            this.tcGeometryModelResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcGeometryModelResults.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcGeometryModelResults.Location = new System.Drawing.Point(0, 0);
            this.tcGeometryModelResults.Margin = new System.Windows.Forms.Padding(0);
            this.tcGeometryModelResults.Name = "tcGeometryModelResults";
            this.tcGeometryModelResults.SelectedIndex = 0;
            this.tcGeometryModelResults.Size = new System.Drawing.Size(239, 498);
            this.tcGeometryModelResults.TabIndex = 0;
            this.tcGeometryModelResults.SelectedIndexChanged += new System.EventHandler(this.tcGeometryModelResults_SelectedIndexChanged);
            this.tcGeometryModelResults.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tcGeometryModelResults_Deselecting);
            // 
            // tpGeometry
            // 
            this.tpGeometry.Controls.Add(this.stbGeometry);
            this.tpGeometry.Controls.Add(this.cltvGeometry);
            this.tpGeometry.Location = new System.Drawing.Point(4, 24);
            this.tpGeometry.Name = "tpGeometry";
            this.tpGeometry.Size = new System.Drawing.Size(231, 470);
            this.tpGeometry.TabIndex = 2;
            this.tpGeometry.Text = "Geometry";
            this.tpGeometry.UseVisualStyleBackColor = true;
            // 
            // stbGeometry
            // 
            this.stbGeometry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stbGeometry.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stbGeometry.Location = new System.Drawing.Point(0, 0);
            this.stbGeometry.Name = "stbGeometry";
            this.stbGeometry.ReadOnly = false;
            this.stbGeometry.Size = new System.Drawing.Size(270, 22);
            this.stbGeometry.TabIndex = 1;
            this.stbGeometry.TextChanged += new System.Action<object, System.EventArgs>(this.stbGeometry_TextChanged);
            // 
            // cltvGeometry
            // 
            this.cltvGeometry.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cltvGeometry.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cltvGeometry.ChangeHighlightOnFocusLost = false;
            this.cltvGeometry.DisableMouse = false;
            this.cltvGeometry.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cltvGeometry.HideSelection = false;
            this.cltvGeometry.HighlightForeErrorColor = System.Drawing.Color.Red;
            this.cltvGeometry.ImageIndex = 0;
            this.cltvGeometry.ImageList = this.ilIcons;
            this.cltvGeometry.Location = new System.Drawing.Point(0, 21);
            this.cltvGeometry.Name = "cltvGeometry";
            treeNode1.ImageKey = "Dots.ico";
            treeNode1.Name = "Node0";
            treeNode1.SelectedImageKey = "Dots.ico";
            treeNode1.Text = "Solid-Part-1";
            treeNode2.ImageKey = "Dots.ico";
            treeNode2.Name = "Node1";
            treeNode2.SelectedImageKey = "Dots.ico";
            treeNode2.Text = "Solid-Part-2";
            treeNode3.ImageKey = "Dots.ico";
            treeNode3.Name = "Node3";
            treeNode3.SelectedImageKey = "Dots.ico";
            treeNode3.Text = "Solid-Part-4";
            treeNode4.ImageKey = "Dots.ico";
            treeNode4.Name = "Node4";
            treeNode4.SelectedImageKey = "Dots.ico";
            treeNode4.Text = "Solid-Part-5";
            treeNode5.ImageKey = "Dots_t.ico";
            treeNode5.Name = "Node2";
            treeNode5.SelectedImageKey = "Dots_t.ico";
            treeNode5.Text = "Comp1";
            treeNode6.ImageKey = "Dots.ico";
            treeNode6.Name = "Node6";
            treeNode6.SelectedImageKey = "Dots.ico";
            treeNode6.Text = "Solid-Part-6";
            treeNode7.ImageKey = "Dots.ico";
            treeNode7.Name = "Node7";
            treeNode7.SelectedImageKey = "Dots.ico";
            treeNode7.Text = "Solid-Part-7";
            treeNode8.ImageKey = "Dots_t.ico";
            treeNode8.Name = "Node5";
            treeNode8.SelectedImageKey = "Dots_t.ico";
            treeNode8.Text = "Comp2";
            treeNode9.ImageKey = "Dots.ico";
            treeNode9.Name = "Node8";
            treeNode9.SelectedImageKey = "Dots.ico";
            treeNode9.Text = "Solid-Part-10";
            treeNode10.ImageKey = "Geometry.ico";
            treeNode10.Name = "Parts";
            treeNode10.SelectedImageKey = "Geometry.ico";
            treeNode10.Text = "Parts";
            treeNode10.ToolTipText = "Parts";
            treeNode11.ImageKey = "Mesh_refinement.ico";
            treeNode11.Name = "Mesh refinements";
            treeNode11.SelectedImageKey = "Mesh_refinement.ico";
            treeNode11.Text = "Mesh refinements";
            treeNode11.ToolTipText = "Mesh refinements";
            this.cltvGeometry.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode10,
            treeNode11});
            this.cltvGeometry.SelectedImageIndex = 0;
            this.cltvGeometry.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.cltvGeometry.SelectionMode = UserControls.TreeViewSelectionMode.MultiSelect;
            this.cltvGeometry.Size = new System.Drawing.Size(231, 450);
            this.cltvGeometry.TabIndex = 0;
            this.cltvGeometry.SelectionsChanged += new System.EventHandler(this.cltv_SelectionsChanged);
            this.cltvGeometry.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.cltv_BeforeCollapse);
            this.cltvGeometry.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.cltv_AfterCollapse);
            this.cltvGeometry.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.cltv_BeforeExpand);
            this.cltvGeometry.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.cltv_AfterExpand);
            this.cltvGeometry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cltv_KeyDown);
            this.cltvGeometry.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.cltv_MouseDoubleClick);
            this.cltvGeometry.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cltv_MouseDown);
            this.cltvGeometry.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cltv_MouseUp);
            // 
            // tpModel
            // 
            this.tpModel.Controls.Add(this.stbModel);
            this.tpModel.Controls.Add(this.cltvModel);
            this.tpModel.Location = new System.Drawing.Point(4, 24);
            this.tpModel.Name = "tpModel";
            this.tpModel.Size = new System.Drawing.Size(231, 470);
            this.tpModel.TabIndex = 0;
            this.tpModel.Text = "FE Model";
            this.tpModel.UseVisualStyleBackColor = true;
            // 
            // stbModel
            // 
            this.stbModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stbModel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stbModel.Location = new System.Drawing.Point(0, 0);
            this.stbModel.Name = "stbModel";
            this.stbModel.ReadOnly = false;
            this.stbModel.Size = new System.Drawing.Size(270, 22);
            this.stbModel.TabIndex = 2;
            this.stbModel.TextChanged += new System.Action<object, System.EventArgs>(this.stbModel_TextChanged);
            // 
            // cltvModel
            // 
            this.cltvModel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cltvModel.BackColor = System.Drawing.SystemColors.Window;
            this.cltvModel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cltvModel.ChangeHighlightOnFocusLost = false;
            this.cltvModel.DisableMouse = false;
            this.cltvModel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cltvModel.HideSelection = false;
            this.cltvModel.HighlightForeErrorColor = System.Drawing.Color.Red;
            this.cltvModel.ImageIndex = 0;
            this.cltvModel.ImageList = this.ilIcons;
            this.cltvModel.Location = new System.Drawing.Point(0, 21);
            this.cltvModel.Margin = new System.Windows.Forms.Padding(0);
            this.cltvModel.Name = "cltvModel";
            treeNode12.ImageKey = "BasePart.ico";
            treeNode12.Name = "Parts";
            treeNode12.SelectedImageKey = "BasePart.ico";
            treeNode12.Text = "Parts";
            treeNode12.ToolTipText = "Parts";
            treeNode13.ImageKey = "Node_set.ico";
            treeNode13.Name = "Node sets";
            treeNode13.SelectedImageKey = "Node_set.ico";
            treeNode13.Text = "Node sets";
            treeNode13.ToolTipText = "Node sets";
            treeNode14.ImageKey = "Element_set.ico";
            treeNode14.Name = "Element sets";
            treeNode14.SelectedImageKey = "Element_set.ico";
            treeNode14.Text = "Element sets";
            treeNode14.ToolTipText = "Element sets";
            treeNode15.ImageKey = "Surface.ico";
            treeNode15.Name = "Surfaces";
            treeNode15.SelectedImageKey = "Surface.ico";
            treeNode15.Text = "Surfaces";
            treeNode15.ToolTipText = "Surfaces";
            treeNode16.ImageKey = "Reference_point.ico";
            treeNode16.Name = "Reference points";
            treeNode16.SelectedImageKey = "Reference_point.ico";
            treeNode16.Text = "Reference points";
            treeNode16.ToolTipText = "Reference points";
            treeNode17.ImageKey = "Mesh.ico";
            treeNode17.Name = "Mesh";
            treeNode17.SelectedImageKey = "Mesh.ico";
            treeNode17.Text = "Mesh";
            treeNode17.ToolTipText = "Mesh";
            treeNode18.ImageKey = "Material.ico";
            treeNode18.Name = "Materials";
            treeNode18.SelectedImageKey = "Material.ico";
            treeNode18.Text = "Materials";
            treeNode18.ToolTipText = "Materials";
            treeNode19.ImageKey = "Section.ico";
            treeNode19.Name = "Sections";
            treeNode19.SelectedImageKey = "Section.ico";
            treeNode19.Text = "Sections";
            treeNode19.ToolTipText = "Sections";
            treeNode20.ImageKey = "Constraints.ico";
            treeNode20.Name = "Constraints";
            treeNode20.SelectedImageKey = "Constraints.ico";
            treeNode20.Text = "Constraints";
            treeNode20.ToolTipText = "Constraints";
            treeNode21.ImageKey = "SurfaceInteraction.ico";
            treeNode21.Name = "Surface interactions";
            treeNode21.SelectedImageKey = "SurfaceInteraction.ico";
            treeNode21.Text = "Surface interactions";
            treeNode21.ToolTipText = "Surface interactions";
            treeNode22.ImageKey = "ContactPair.ico";
            treeNode22.Name = "Contact pairs";
            treeNode22.SelectedImageKey = "ContactPair.ico";
            treeNode22.Text = "Contact pairs";
            treeNode22.ToolTipText = "Contact pairs";
            treeNode23.ImageKey = "Contact.ico";
            treeNode23.Name = "Contacts";
            treeNode23.SelectedImageIndex = 16;
            treeNode23.Text = "Contacts";
            treeNode23.ToolTipText = "Contacts";
            treeNode24.ImageKey = "Amplitude.ico";
            treeNode24.Name = "Amplitudes";
            treeNode24.SelectedImageKey = "Amplitude.ico";
            treeNode24.Text = "Amplitudes";
            treeNode24.ToolTipText = "Amplitudes";
            treeNode25.ImageKey = "InitialConditions.ico";
            treeNode25.Name = "Initial conditions";
            treeNode25.SelectedImageKey = "InitialConditions.ico";
            treeNode25.Text = "Initial conditions";
            treeNode25.ToolTipText = "Initial conditions";
            treeNode26.ImageKey = "Step.ico";
            treeNode26.Name = "Steps";
            treeNode26.SelectedImageKey = "Step.ico";
            treeNode26.Text = "Steps";
            treeNode26.ToolTipText = "Steps";
            treeNode27.ImageKey = "Dots.ico";
            treeNode27.Name = "Model";
            treeNode27.Text = "Model";
            treeNode27.ToolTipText = "Model";
            treeNode28.ImageKey = "Analysis.ico";
            treeNode28.Name = "Analyses";
            treeNode28.SelectedImageKey = "Analysis.ico";
            treeNode28.Text = "Analyses";
            this.cltvModel.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode27,
            treeNode28});
            this.cltvModel.SelectedImageIndex = 0;
            this.cltvModel.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.cltvModel.SelectionMode = UserControls.TreeViewSelectionMode.MultiSelect;
            this.cltvModel.Size = new System.Drawing.Size(231, 450);
            this.cltvModel.TabIndex = 0;
            this.cltvModel.SelectionsChanged += new System.EventHandler(this.cltv_SelectionsChanged);
            this.cltvModel.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.cltv_BeforeCollapse);
            this.cltvModel.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.cltv_AfterCollapse);
            this.cltvModel.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.cltv_BeforeExpand);
            this.cltvModel.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.cltv_AfterExpand);
            this.cltvModel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cltv_KeyDown);
            this.cltvModel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.cltv_MouseDoubleClick);
            this.cltvModel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cltv_MouseDown);
            this.cltvModel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cltv_MouseUp);
            // 
            // tpResults
            // 
            this.tpResults.Controls.Add(this.stbResults);
            this.tpResults.Controls.Add(this.cltvResults);
            this.tpResults.Location = new System.Drawing.Point(4, 24);
            this.tpResults.Name = "tpResults";
            this.tpResults.Padding = new System.Windows.Forms.Padding(3);
            this.tpResults.Size = new System.Drawing.Size(231, 470);
            this.tpResults.TabIndex = 1;
            this.tpResults.Text = "Results";
            this.tpResults.UseVisualStyleBackColor = true;
            // 
            // stbResults
            // 
            this.stbResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stbResults.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stbResults.Location = new System.Drawing.Point(0, 0);
            this.stbResults.Name = "stbResults";
            this.stbResults.ReadOnly = false;
            this.stbResults.Size = new System.Drawing.Size(270, 22);
            this.stbResults.TabIndex = 3;
            this.stbResults.TextChanged += new System.Action<object, System.EventArgs>(this.stbResults_TextChanged);
            // 
            // cltvResults
            // 
            this.cltvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cltvResults.BackColor = System.Drawing.SystemColors.Window;
            this.cltvResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cltvResults.ChangeHighlightOnFocusLost = false;
            this.cltvResults.DisableMouse = false;
            this.cltvResults.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cltvResults.HighlightForeErrorColor = System.Drawing.Color.Red;
            this.cltvResults.ImageIndex = 0;
            this.cltvResults.ImageList = this.ilIcons;
            this.cltvResults.Location = new System.Drawing.Point(0, 21);
            this.cltvResults.Margin = new System.Windows.Forms.Padding(0);
            this.cltvResults.Name = "cltvResults";
            treeNode29.ImageKey = "BasePart.ico";
            treeNode29.Name = "Parts";
            treeNode29.SelectedImageKey = "BasePart.ico";
            treeNode29.Text = "Parts";
            treeNode29.ToolTipText = "Parts";
            treeNode30.ImageKey = "Node_set.ico";
            treeNode30.Name = "Node sets";
            treeNode30.SelectedImageKey = "Node_set.ico";
            treeNode30.Text = "Node sets";
            treeNode30.ToolTipText = "Node sets";
            treeNode31.ImageKey = "Element_set.ico";
            treeNode31.Name = "Element sets";
            treeNode31.SelectedImageKey = "Element_set.ico";
            treeNode31.Text = "Element sets";
            treeNode31.ToolTipText = "Element sets";
            treeNode32.ImageKey = "Surface.ico";
            treeNode32.Name = "Surfaces";
            treeNode32.SelectedImageKey = "Surface.ico";
            treeNode32.Text = "Surfaces";
            treeNode32.ToolTipText = "Surfaces";
            treeNode33.ImageKey = "Mesh.ico";
            treeNode33.Name = "Mesh";
            treeNode33.SelectedImageKey = "Mesh.ico";
            treeNode33.Text = "Mesh";
            treeNode33.ToolTipText = "Mesh";
            treeNode34.ImageKey = "Field_output.ico";
            treeNode34.Name = "Field outputs";
            treeNode34.SelectedImageKey = "Field_output.ico";
            treeNode34.StateImageKey = "(none)";
            treeNode34.Text = "Field outputs";
            treeNode34.ToolTipText = "Field outputs";
            treeNode35.ImageKey = "History_output.ico";
            treeNode35.Name = "History outputs";
            treeNode35.SelectedImageKey = "History_output.ico";
            treeNode35.Text = "History outputs";
            treeNode35.ToolTipText = "History outputs";
            treeNode36.ImageKey = "Dots.ico";
            treeNode36.Name = "Results";
            treeNode36.SelectedImageKey = "Dots_t.ico";
            treeNode36.Text = "Results";
            treeNode36.ToolTipText = "Results";
            this.cltvResults.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode33,
            treeNode36});
            this.cltvResults.SelectedImageIndex = 0;
            this.cltvResults.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.cltvResults.SelectionMode = UserControls.TreeViewSelectionMode.MultiSelect;
            this.cltvResults.Size = new System.Drawing.Size(231, 450);
            this.cltvResults.TabIndex = 0;
            this.cltvResults.SelectionsChanged += new System.EventHandler(this.cltv_SelectionsChanged);
            this.cltvResults.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.cltv_BeforeCollapse);
            this.cltvResults.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.cltv_AfterCollapse);
            this.cltvResults.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.cltv_BeforeExpand);
            this.cltvResults.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.cltv_AfterExpand);
            this.cltvResults.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cltv_KeyDown);
            this.cltvResults.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.cltv_MouseDoubleClick);
            this.cltvResults.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cltv_MouseDown);
            this.cltvResults.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cltv_MouseUp);
            // 
            // ModelTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcGeometryModelResults);
            this.Name = "ModelTree";
            this.Size = new System.Drawing.Size(239, 498);
            this.cmsTree.ResumeLayout(false);
            this.tcGeometryModelResults.ResumeLayout(false);
            this.tpGeometry.ResumeLayout(false);
            this.tpModel.ResumeLayout(false);
            this.tpResults.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void CltvModel_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void CltvGeometry_SelectionsChanged(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        private CodersLabTreeView cltvModel;
        private System.Windows.Forms.ContextMenuStrip cmsTree;
        private System.Windows.Forms.ToolStripMenuItem tsmiExpandAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiCollapseAll;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceDelete;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreate;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceExpandColapse;
        private System.Windows.Forms.ToolStripMenuItem tsmiRun;
        private System.Windows.Forms.ToolStripMenuItem tsmiMonitor;
        private System.Windows.Forms.ToolStripMenuItem tsmiKill;
        private System.Windows.Forms.ToolStripMenuItem tsmiResults;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceAnalysis;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeactivate;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceActive;
        private System.Windows.Forms.ToolStripMenuItem tsmiActivate;
        private System.Windows.Forms.TabControl tcGeometryModelResults;
        private System.Windows.Forms.TabPage tpModel;
        private System.Windows.Forms.TabPage tpResults;
        private CodersLabTreeView cltvResults;
        private System.Windows.Forms.ImageList ilStatusIcons;
        private System.Windows.Forms.ImageList ilIcons;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceHideShow;
        private System.Windows.Forms.ToolStripMenuItem tsmiHide;
        private System.Windows.Forms.ToolStripMenuItem tsmiShow;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceMesh;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateMesh;
        private System.Windows.Forms.ToolStripMenuItem tsmiMeshingParameters;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceCopyPart;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyGeometryToResults;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceColorContours;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorContoursOff;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorContoursOn;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceMaterialLibrary;
        private System.Windows.Forms.ToolStripMenuItem tsmiMaterialLibrary;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceMergeParts;
        private System.Windows.Forms.ToolStripMenuItem tsmiMergeParts;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceConvertToPart;
        private System.Windows.Forms.ToolStripMenuItem tsmiConvertToPart;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowOnly;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetTransparency;
        private System.Windows.Forms.ToolStripMenuItem tsmiPreviewEdgeMesh;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceEditCalculiXKeywords;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditCalculiXKeywords;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceCompoundPart;
        private System.Windows.Forms.ToolStripMenuItem tsmiCompoundPart;
        private System.Windows.Forms.ToolStripMenuItem tsmiDuplicate;
        private System.Windows.Forms.ToolStripMenuItem tsmiPropagate;
        private System.Windows.Forms.ToolStripMenuItem tsmiSwapPartGeometries;
        private System.Windows.Forms.TabPage tpGeometry;
        private SearchTextBox stbGeometry;
        private CodersLabTreeView cltvGeometry;
        private SearchTextBox stbModel;
        private SearchTextBox stbResults;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceSwapMergeMasterSlave;
        private System.Windows.Forms.ToolStripMenuItem tsmiSwapMasterSlave;
        private System.Windows.Forms.ToolStripMenuItem tsmiMergeByMasterSlave;
        private System.Windows.Forms.ToolStripSeparator tsmiSpaceSearchContactPairs;
        private System.Windows.Forms.ToolStripMenuItem tsmiSearchContactPairs;
        private System.Windows.Forms.ToolStripMenuItem tsmiQuery;
        private System.Windows.Forms.ToolStripMenuItem tsmiPreview;
    }
}
