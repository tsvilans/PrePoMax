using CaeGlobals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeModel;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PrePoMax.Forms
{
    public partial class FrmMaterialLibrary : Form
    {
        // Variables                                                                                                                
        private Controller _controller;
        private bool _modelChanged;
        private UnitSystem _libraryUnitSystem;
        private FrmMaterial _frmMaterial;
        private int _yPadding;
        static bool _collapsed = true;

        // Properties                                                                                                               


        // Constructors                                                                                                             
        public FrmMaterialLibrary(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _modelChanged = false;
            //
            _libraryUnitSystem = new UnitSystem(UnitSystemType.MM_TON_S_C);
            _controller.Model.UnitSystem.SetConverterUnits();
        }


        // Event handlers                                                                                                           
        private void FrmMaterialLibrary_Load(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model != null)
                {
                    ListViewItem item;
                    foreach (var entry in _controller.Model.Materials)
                    {
                        item = lvModelMaterials.Items.Add(entry.Value.Name);
                        item.Name = entry.Value.Name;
                        item.Tag = entry.Value; // do not clone, to determine if the material changed
                    }
                    if (lvModelMaterials.Items.Count > 0) lvModelMaterials.Items[0].Selected = true;
                }
                // Load material libraries
                string fileName = Path.Combine(Application.StartupPath, Globals.MaterialLibraryFileName);
                LoadMaterialLibraryFromFile(fileName);
                //
                foreach (var materialLibraryFile in _controller.Settings.General.GetMaterialLibraryFiles())
                {
                    LoadMaterialLibraryFromFile(materialLibraryFile);
                }
                //
                TreeNode materialNode;
                GetNodeContainingFirstMaterial(cltvLibrary.Nodes[0], out materialNode);
                if (materialNode != null) cltvLibrary.SelectedNode = materialNode;
                else cltvLibrary.SelectedNode = cltvLibrary.Nodes[0];
                //
                _frmMaterial = new FrmMaterial(_controller);
                _frmMaterial.Text = "Preview Material Properties";
                _frmMaterial.VisibleChanged += _frmMaterial_VisibleChanged;
                _frmMaterial.PrepareFormForPreview();
                //
                cltvLibrary_AfterSelect(null, null);
                //
                _yPadding = gbLibraries.Bottom - gbLibraryMaterials.Top;
                //
                gbLibraries.IsCollapsed = _collapsed;
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void _frmMaterial_VisibleChanged(object sender, EventArgs e)
        {
            if (_frmMaterial.Visible) { }
            else
            {
                if (cbPreview.Checked) cbPreview.Checked = false;
            }
        }
        private void gbLibraries_OnCollapsedChanged(object sender)
        {
            int newPadding = gbLibraries.Bottom - gbLibraryMaterials.Top;
            if (newPadding != _yPadding)
            {
                int delta = newPadding - _yPadding;
                gbLibraryMaterials.Top += delta;
                gbModelMaterials.Top += delta;
                btnCopyToModel.Top += delta;
                btnCopyToLibrary.Top += delta;
                //
                gbLibraryMaterials.Height -= delta;
                gbModelMaterials.Height -= delta;
            }
            _collapsed = gbLibraries.IsCollapsed;
        }
        // Libraries
        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Material library files | *.lib";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        MaterialLibraryItem materialLibrary = new MaterialLibraryItem("Materials");
                        SaveMaterialLibraryToFile(saveFileDialog.FileName, materialLibrary);
                        //
                        LoadMaterialLibraryFromFile(saveFileDialog.FileName);
                        SetControlStates();
                    }
                }
            }
            catch
            { }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Material library files | *.lib";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadMaterialLibraryFromFile(openFileDialog.FileName);
                        _controller.AddMaterialLibraryFile(openFileDialog.FileName);
                        SetControlStates();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is CaeException ce) ExceptionTools.Show(this, ce);
            }
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvLibraries.SelectedItems.Count == 1)
            {
                _controller.RemoveMaterialLibraryFile(lvLibraries.SelectedItems[0].Text);
                //
                int selectedId = lvLibraries.SelectedIndices[0];
                lvLibraries.SelectedIndices.Clear();
                lvLibraries.Items.RemoveAt(selectedId);
                //
                if (selectedId < lvLibraries.Items.Count) lvLibraries.Items[selectedId].Selected = true;
                else if (lvLibraries.Items.Count > 0) lvLibraries.Items[lvLibraries.Items.Count - 1].Selected = true;
                else if (lvLibraries.Items.Count == 0) SetControlStates();
            }
        }
        private void lvLibraries_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lvLibraries.SelectedItems.Count == 1)
                {
                    MaterialLibraryItem mli = (MaterialLibraryItem)lvLibraries.SelectedItems[0].Tag;
                    ClearTree();
                    FillTree(mli, cltvLibrary.Nodes[0]);
                    //
                    cltvLibrary_AfterSelect(null, null);
                }
            }
            catch { }
        }
        private void LibraryChanged()
        {
            if (lvLibraries.SelectedItems.Count == 1)
            {
                MaterialLibraryItem materialLibrary = (MaterialLibraryItem)lvLibraries.SelectedItems[0].Tag;
                materialLibrary.Items.Clear();
                TreeNodesToItemList(cltvLibrary.Nodes[0], materialLibrary);
                //
                if (!lvLibraries.SelectedItems[0].Text.EndsWith("*")) lvLibraries.SelectedItems[0].Text += "*";
            }
        }
        private bool AnyLibraryChanged()
        {
            foreach (ListViewItem item in lvLibraries.Items)
            {
                if (item.Text.EndsWith("*")) return true;
            }
            return false;
        }
        private void SetControlStates()
        {
            bool enabled = true;
            if (lvLibraries.Items.Count == 0)
            {
                ClearTree();
                tbCategoryName.Text = "";
                //
                enabled = false;
            }
            //
            gbLibraryMaterials.Enabled = enabled;
            btnCopyToModel.Enabled = enabled;
            btnCopyToLibrary.Enabled = enabled;
        }
        private void ClearTree()
        {
            cltvLibrary.BeginUpdate();
            cltvLibrary.Nodes[0].Nodes.Clear();
            cltvLibrary.EndUpdate();
        }
        //
        private void cltvLibrary_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //try
            //{
            //    if (cltvLibrary.SelectedNode != null)
            //    {
            //        cltvLibrary.SelectedNode.EnsureVisible();
            //        tbCategoryName.Text = cltvLibrary.SelectedNode.Text;
            //        //
            //        if (cltvLibrary.SelectedNode.Tag != null)
            //        {
            //            if (_frmMaterial != null) _frmMaterial.Material = (Material)cltvLibrary.SelectedNode.Tag;
            //        }
            //    }
            //}
            //catch
            //{ }
        }
        private void cltvLibrary_MouseDown(object sender, MouseEventArgs e)
        {
            
        }
        private void cltvLibrary_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (cltvLibrary.SelectedNode != null)
                {
                    cltvLibrary.SelectedNode.EnsureVisible();
                    tbCategoryName.Text = cltvLibrary.SelectedNode.Text;
                    //
                    if (cltvLibrary.SelectedNode.Tag != null)
                    {
                        if (_frmMaterial != null)
                        {
                            // Convert material unit system
                            Material previewMaterial = (Material)cltvLibrary.SelectedNode.Tag.DeepClone();
                            previewMaterial.ConvertUnits(_controller.Model.UnitSystem, _libraryUnitSystem, _controller.Model.UnitSystem);
                            _frmMaterial.Material = previewMaterial;
                        }
                        
                    }
                }
            }
            catch
            { }
        }
        private void cltvLibrary_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (cltvLibrary.SelectedNode != null && cltvLibrary.SelectedNode.Tag != null)
            {
                btnCopyToModel_Click(null, null);
            }
                
        }
        //
        private void lvModelMaterials_MouseDown(object sender, MouseEventArgs e)
        {
            //try
            //{
            //    if (lvModelMaterials.HitTest(e.Location).Item == null)
            //    {
            //        //lvModelMaterials.SelectedItems.Clear();
            //    }
            //}
            //catch
            //{ }
        }
        private void lvModelMaterials_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (lvModelMaterials.SelectedItems != null && lvModelMaterials.SelectedItems.Count == 1 &&
                   lvModelMaterials.SelectedItems[0].Tag != null)
                {
                    if (_frmMaterial.Material != null) _frmMaterial.Material = (Material)lvModelMaterials.SelectedItems[0].Tag;
                }
            }
            catch
            { }
        }
        private void lvModelMaterials_Leave(object sender, EventArgs e)
        {
            //if (lvModelMaterials.SelectedItems.Count == 1)
            //{
            //    lvModelMaterials.SelectedItems[0].BackColor = SystemColors.Highlight;
            //    lvModelMaterials.SelectedItems[0].ForeColor = Color.White;
            //}
        }
        private void lvModelMaterials_Enter(object sender, EventArgs e)
        {
            //if (lvModelMaterials.SelectedItems.Count == 1)
            //{
            //    lvModelMaterials.SelectedItems[0].BackColor = Color.White;
            //    lvModelMaterials.SelectedItems[0].ForeColor = Color.Black;
            //}
        }
        //
        private void tbCategoryName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;  // no beep
            }
        }
        private void tbCategoryName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnRename_Click(null, null);
        }
        //
        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode parentNode;
                if (cltvLibrary.SelectedNode != null) parentNode = cltvLibrary.SelectedNode;
                else parentNode = cltvLibrary.Nodes[0];

                if (parentNode.Tag == null)
                {
                    TreeNode node = parentNode.Nodes.Add("NewCategory");
                    node.Name = "NewCategory";

                    parentNode.Expand();
                    cltvLibrary.SelectedNode = node;
                    cltvLibrary.SelectedNode.EnsureVisible();
                    ApplyFormatingRecursive(node);
                    cltvLibrary.Focus();

                    tbCategoryName.Text = node.Name;
                    tbCategoryName.Focus();

                    LibraryChanged();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnDeleteFromLibrary_Click(object sender, EventArgs e)
        {
            TreeNode parent = cltvLibrary.SelectedNode.Parent;
            if (cltvLibrary.SelectedNode != null && parent != null)
            {
                int selectedId = cltvLibrary.SelectedNode.Index;                
                //
                parent.Nodes.Remove(cltvLibrary.SelectedNode);
                LibraryChanged();
                //
                if (selectedId == parent.Nodes.Count) selectedId--;
                if (selectedId >= 0) cltvLibrary.SelectedNode = parent.Nodes[selectedId];
                else cltvLibrary.SelectedNode = parent;
            }
        }
        private void btnRename_Click(object sender, EventArgs e)
        {
            try
            {
                if (cltvLibrary.SelectedNode != null && cltvLibrary.SelectedNode.Text != tbCategoryName.Text && cltvLibrary.SelectedNode.Parent != null)
                {
                    if (!cltvLibrary.SelectedNode.Parent.Nodes.ContainsKey(tbCategoryName.Text))
                    {
                        Material test = new Material(tbCategoryName.Text); // test the name
                        cltvLibrary.SelectedNode.Text = tbCategoryName.Text;
                        cltvLibrary.SelectedNode.Name = tbCategoryName.Text;
                        if (cltvLibrary.SelectedNode.Tag != null) ((Material)cltvLibrary.SelectedNode.Tag).Name = tbCategoryName.Text;
                        //
                        LibraryChanged();
                    }
                    else throw new CaeException("The node '" + cltvLibrary.SelectedNode.Parent.Text + 
                                                "' already contains the node named '" + tbCategoryName.Text + "'.");
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void btnDeleteFromModel_Click(object sender, EventArgs e)
        {
            if (lvModelMaterials.SelectedItems.Count == 1)
            {
                int selectedIndex = lvModelMaterials.SelectedIndices[0];
                lvModelMaterials.Items.Remove(lvModelMaterials.SelectedItems[0]);
                _modelChanged = true;
                //
                if (lvModelMaterials.Items.Count > 0)
                {
                    // Select the same index
                    if (selectedIndex < lvModelMaterials.Items.Count) lvModelMaterials.Items[selectedIndex].Selected = true;
                    // Select the last item
                    else lvModelMaterials.Items[selectedIndex - 1].Selected = true;
                }
                //
                lvModelMaterials.Focus();
            }
        }
        //
        private void btnCopyToLibrary_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvModelMaterials.SelectedItems.Count == 1 && cltvLibrary.SelectedNode != null)
                {
                    TreeNode categoryNode;
                    if (cltvLibrary.SelectedNode.Tag == null) categoryNode = cltvLibrary.SelectedNode;  // Category
                    else categoryNode = cltvLibrary.SelectedNode.Parent;                                // Material
                    //
                    if (categoryNode == null)
                        throw new CaeException("Please select a library category to which the material should be added.");
                    //
                    string materialName = lvModelMaterials.SelectedItems[0].Text;
                    int count = 1;
                    while (categoryNode.Nodes.ContainsKey(materialName))
                    {
                        materialName = lvModelMaterials.SelectedItems[0].Text + "_Model-" + count;
                        count++;
                    }
                    //
                    ListViewItem libraryMaterialItem = lvModelMaterials.SelectedItems[0];
                    Material libraryMaterial = (Material)libraryMaterialItem.Tag.DeepClone();
                    libraryMaterial.Name = materialName;
                    //
                    TreeNode newMaterialNode = categoryNode.Nodes.Add(libraryMaterial.Name);
                    newMaterialNode.Name = newMaterialNode.Text;
                    // Convert material unit system
                    libraryMaterial.ConvertUnits(_controller.Model.UnitSystem, _controller.Model.UnitSystem, _libraryUnitSystem);
                    newMaterialNode.Tag = libraryMaterial;
                    //
                    categoryNode.Expand();
                    cltvLibrary.SelectedNode = newMaterialNode;
                    //
                    LibraryChanged();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                _controller.Model.UnitSystem.SetConverterUnits();
            }
        }
        private void btnCopyToModel_Click(object sender, EventArgs e)
        {
            try
            {
                if (cltvLibrary.SelectedNode != null && cltvLibrary.SelectedNode.Tag != null)
                {
                    string materialName = cltvLibrary.SelectedNode.Text;
                    int count = 1;
                    while (lvModelMaterials.Items.ContainsKey(materialName))
                    {
                        materialName = cltvLibrary.SelectedNode.Text + "_Library-" + count;
                        count++;
                    }
                    ListViewItem modelMaterialItem = lvModelMaterials.Items.Add(materialName);
                    modelMaterialItem.Name = modelMaterialItem.Text;
                    // Convert material unit system
                    Material modelMaterial = (Material)cltvLibrary.SelectedNode.Tag.DeepClone();
                    modelMaterial.Name = modelMaterialItem.Name;
                    modelMaterial.ConvertUnits(_controller.Model.UnitSystem, _libraryUnitSystem, _controller.Model.UnitSystem);
                    modelMaterialItem.Tag = modelMaterial;
                    //
                    lvModelMaterials_Enter(null, null);
                    // Deselect
                    modelMaterialItem.Selected = true;
                    lvModelMaterials_Leave(null, null);
                    //
                    lvModelMaterials.Focus();
                    //
                    _modelChanged = true;
                }
                else throw new CaeException("Please select the material in the library materials " +
                                            "to be copied to the model materials.");
            }
            catch (Exception ex)
            {                
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                _controller.Model.UnitSystem.SetConverterUnits();
            }
        }
        //
        private void cbPreview_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPreview.Checked)
            {
                _frmMaterial.Location = new Point(Location.X + Width - 12, Location.Y);
                _frmMaterial.Show(this);
            }
            else _frmMaterial.Hide();
        }
        //
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvLibraries.SelectedItems.Count == 1)
                {
                    SaveMaterialLibrary(lvLibraries.SelectedItems[0]);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (_controller.Model != null && _modelChanged)
                {
                    List<Material> newMaterials = new List<Material>();
                    List<Material> changedMaterials = new List<Material>();
                    List<string> deletedMaterials = new List<string>();
                    Material material;
                    //
                    foreach (ListViewItem item in lvModelMaterials.Items)
                    {
                        if (_controller.Model.Materials.TryGetValue(item.Name, out material))
                        {
                            if ((Material)item.Tag != material) changedMaterials.Add((Material)item.Tag);
                        }
                        else newMaterials.Add((Material)item.Tag);
                    }
                    //
                    foreach (var entry in _controller.Model.Materials)
                    {
                        if (!lvModelMaterials.Items.ContainsKey(entry.Key)) deletedMaterials.Add(entry.Key);
                    }
                    //
                    if (deletedMaterials.Count > 0) _controller.RemoveMaterialsCommand(deletedMaterials.ToArray());
                    foreach (Material changedMaterial in changedMaterials) _controller.ReplaceMaterialCommand(changedMaterial.Name, changedMaterial);
                    foreach (Material newMaterial in newMaterials) _controller.AddMaterialCommand(newMaterial);
                }
                Close();
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
        //
        private void FrmMaterialLibrary_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (e.CloseReason == CloseReason.UserClosing && AnyLibraryChanged())
                {
                    DialogResult response = MessageBox.Show("Save all material libraries before closing?", "Warning",
                                                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (response == DialogResult.Yes) SaveAllMaterialLibraries();
                    else if (response == DialogResult.Cancel) e.Cancel = true;
                }
            }
            catch
            { }
        }


        // Methods                                                                                                                  
        private void SaveAllMaterialLibraries()
        {
            foreach (ListViewItem item in lvLibraries.Items) SaveMaterialLibrary(item);
        }
        private void SaveMaterialLibrary(ListViewItem item)
        {
            if (item.Text.EndsWith("*"))
            {
                MaterialLibraryItem materialLibrary = (MaterialLibraryItem)item.Tag;
                SaveMaterialLibraryToFile(item.Name, materialLibrary);
                item.Text = item.Name;
            }
        }
        private void SaveMaterialLibraryToFile(string fileName, MaterialLibraryItem materialLibrary)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            string json = JsonConvert.SerializeObject(materialLibrary, Formatting.Indented, settings);
            File.WriteAllText(fileName, json);
        }
        private void TreeNodesToItemList(TreeNode node, MaterialLibraryItem materialLibraryItem)
        {
            materialLibraryItem.Expanded = node.IsExpanded;
            //
            foreach (TreeNode childNode in node.Nodes)
            {
                MaterialLibraryItem childItem = new MaterialLibraryItem(childNode.Name);
                if (childNode.Tag != null) childItem.Tag = (Material)childNode.Tag.DeepClone();
                materialLibraryItem.Items.Add(childItem);
                //
                TreeNodesToItemList(childNode, childItem);
            }
        }
        //
        private void LoadMaterialLibraryFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                foreach (ListViewItem item in lvLibraries.Items)
                {
                    if (item.Name == fileName) throw new CaeException("The selected material library is already open.");
                }
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                string contents = File.ReadAllText(fileName);
                //
                MaterialLibraryItem materialLibrary = JsonConvert.DeserializeObject<MaterialLibraryItem>(contents, settings);
                //
                lvLibraries.SelectedIndices.Clear();
                ListViewItem libraryItem = lvLibraries.Items.Add(fileName);
                libraryItem.Name = libraryItem.Text;
                libraryItem.Tag = materialLibrary;
                libraryItem.Selected = true;
            }
        }
        private void FillTree(MaterialLibraryItem materialLibraryItem, TreeNode node)
        {
            node.TreeView.BeginUpdate();
            //
            ItemListToTreeNodes(materialLibraryItem, node);
            ApplyFormatingRecursive(cltvLibrary.Nodes[0]);
            //
            node.TreeView.EndUpdate();
        }
        private void ItemListToTreeNodes(MaterialLibraryItem materialLibraryItem, TreeNode node)
        {
            TreeNode childNode;
            foreach (MaterialLibraryItem childItem in materialLibraryItem.Items)
            {
                childNode = node.Nodes.Add(childItem.Name);
                childNode.Name = childItem.Name;
                //
                if (childItem.Tag != null) childNode.Tag = childItem.Tag.DeepClone();
                else ItemListToTreeNodes(childItem, childNode);
            }
            //
            if (materialLibraryItem.Expanded) node.Expand();
        }
        //
        private void ApplyFormatingRecursive(TreeNode node)
        {
            if (node.Tag == null) node.ForeColor = SystemColors.Highlight;
            else node.ForeColor = Color.Black;
            //
            foreach (TreeNode childNode in node.Nodes)
            {
                ApplyFormatingRecursive(childNode);
            }
        }
        //
        private void GetNodeContainingFirstMaterial(TreeNode node, out TreeNode firstNodeWithMaterial)
        {
            firstNodeWithMaterial = null;
            //
            if (node.Tag != null)
            {
                firstNodeWithMaterial = node;
                return;
            }
            //
            foreach (TreeNode childNode in node.Nodes)
            {
                GetNodeContainingFirstMaterial(childNode, out firstNodeWithMaterial);
                if (firstNodeWithMaterial != null) return;
            }
        }

        
    }
}
