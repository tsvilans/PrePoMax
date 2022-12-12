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
using FileInOut.Output.Calculix;
using CaeGlobals;

namespace PrePoMax.Forms
{
    public partial class FrmCalculixKeywordEditor : UserControls.PrePoMaxChildForm
    {
        // dll routines
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);


        // Variables                                                                                                                
        List<CalculixKeyword> _keywords;
        OrderedDictionary<int[], CalculixUserKeyword> _userKeywords;
        int _selectedKeywordFirstLine;
        int _selectedKeywordNumOfLines;


        // Properties                                                                                                               
        public List<CalculixKeyword> Keywords { get { return _keywords; } set { _keywords = value; } }
        public OrderedDictionary<int[], CalculixUserKeyword> UserKeywords { get { return _userKeywords; } set { _userKeywords = value; } }


        // Events                                                                                                                   


        // Constructors                                                                                                             
        public FrmCalculixKeywordEditor()
        {
            InitializeComponent();
        }


        // Event handlers                                                                                                           
        private void cltvKeywordsTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Set tree selection color
            bool userNode = false;
            if (cltvKeywordsTree.SelectedNode != null)
            {
                if (cltvKeywordsTree.SelectedNode.Tag is CalculixUserKeyword)
                {
                    userNode = true;
                }
            }
            // De-reference text changed event
            rtbKeyword.TextChanged -= rtbKeyword_TextChanged;
            // Clear keyword textbox
            rtbKeyword.Clear();
            // Add keyword data to keyword textbox
            rtbKeyword.Tag = e.Node.Tag;
            if (e.Node.Tag != null)
            {
                rtbKeyword.AppendText(FileInOut.Output.CalculixFileWriter.GetShortKeywordData(e.Node.Tag as CalculixKeyword));
                FormatInp(rtbKeyword);
                rtbKeyword.ReadOnly = !userNode;
            }
            // Re-reference text changed event
            rtbKeyword.TextChanged += rtbKeyword_TextChanged;
            //
            UpdateKeywordTextBox();
        }
        //
        private void rtbKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                ((RichTextBox)sender).Paste(DataFormats.GetFormat("Text"));
                e.Handled = true;
            }
        }
        private void rtbKeyword_TextChanged(object sender, EventArgs e)
        {
            UpdateKeywordTextBoxDelayed();
        }
        private void btnAddKeyword_Click(object sender, EventArgs e)
        {
            if (cltvKeywordsTree.SelectedNode != null)
            {
                CalculixUserKeyword keyword = new CalculixUserKeyword("User keyword");
                TreeNode node = cltvKeywordsTree.SelectedNode.Nodes.Add("User keyword");
                //
                cltvKeywordsTree.Focus();   // must be first
                AddUserKeywordToTreeNode(keyword, node);
                cltvKeywordsTree.SelectedNode.Expand();
                cltvKeywordsTree.SelectedNode = node;
            }
            UpdateKeywordTextBoxDelayed();
        }
        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (cltvKeywordsTree.SelectedNode != null)
            {
                TreeNode child = cltvKeywordsTree.SelectedNode;
                if (child.Tag is CalculixUserKeyword)
                {
                    TreeNode parent = cltvKeywordsTree.SelectedNode.Parent;
                    if (parent != null)
                    {
                        int index = parent.Nodes.IndexOf(child);
                        if (index > 0)
                        {
                            parent.Nodes.RemoveAt(index);
                            parent.Nodes.Insert(index - 1, child);
                        }
                    }
                    else
                    {
                        int index = cltvKeywordsTree.Nodes.IndexOf(child);
                        if (index > 0)
                        {
                            cltvKeywordsTree.Nodes.RemoveAt(index);
                            cltvKeywordsTree.Nodes.Insert(index - 1, child);
                        }
                    }

                    cltvKeywordsTree.SelectedNode = child;
                    cltvKeywordsTree.Focus();
                }
            }

            UpdateKeywordTextBox();
        }
        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (cltvKeywordsTree.SelectedNode != null)
            {
                TreeNode child = cltvKeywordsTree.SelectedNode;
                if (child.Tag is CalculixUserKeyword)
                {
                    TreeNode parent = cltvKeywordsTree.SelectedNode.Parent;
                    if (parent != null)
                    {
                        int index = parent.Nodes.IndexOf(child);
                        if (index < parent.Nodes.Count - 1)
                        {
                            parent.Nodes.RemoveAt(index);
                            parent.Nodes.Insert(index + 1, child);
                        }
                    }
                    else
                    {
                        int index = cltvKeywordsTree.Nodes.IndexOf(child);
                        if (index < cltvKeywordsTree.Nodes.Count - 1)
                        {
                            cltvKeywordsTree.Nodes.RemoveAt(index);
                            cltvKeywordsTree.Nodes.Insert(index + 1, child);
                        }
                    }

                    cltvKeywordsTree.SelectedNode = child;
                    cltvKeywordsTree.Focus();
                }
            }

            UpdateKeywordTextBox();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (cltvKeywordsTree.SelectedNode != null)
            {
                TreeNode child = cltvKeywordsTree.SelectedNode;
                if (child.Tag is CalculixUserKeyword)
                {
                    TreeNode parent = cltvKeywordsTree.SelectedNode.Parent;

                    if (parent != null) parent.Nodes.Remove(child);
                    else cltvKeywordsTree.Nodes.Remove(child);

                    cltvKeywordsTree.Focus();
                }
            }

            UpdateKeywordTextBox();
        }
        //
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            UpdateKeywordTextBox();
            timerUpdate.Stop();
        }
        //
        private void btnOK_Click(object sender, EventArgs e)
        {
            _userKeywords = new OrderedDictionary<int[], CalculixUserKeyword>("User CalculiX keywords");
            //
            FindUserKeywords(cltvKeywordsTree.Nodes[0], _userKeywords);
            //
            this.DialogResult = DialogResult.OK;
        }
        private void FindUserKeywords(TreeNode node, OrderedDictionary<int[], CalculixUserKeyword> userKeywords)
        {
            if (node.Tag != null && node.Tag is CalculixUserKeyword calculixUserKeyword)
            {
                List<int> indices = new List<int>();
                GetNodeIdices(node, indices);
                userKeywords.Add(indices.ToArray(), calculixUserKeyword);
            }
            //
            foreach (TreeNode childNode in node.Nodes)
            {
                FindUserKeywords(childNode, userKeywords);
            }
        }
        private void GetNodeIdices(TreeNode node, List<int> indices)
        {
            TreeNode parent = node.Parent;
            if (parent != null)
            {
                GetNodeIdices(parent, indices);
                //
                indices.Add(parent.Nodes.IndexOf(node));
            }
        }


        // Methods                                                                                                                  
        public void PrepareForm()
        {
            TreeNode node = new TreeNode();
            node.Text = "CalculiX inp file";
            cltvKeywordsTree.Nodes.Add(node);
            // Build the keyword tree
            int index;
            foreach (CalculixKeyword keyword in _keywords)
            {
                index = node.Nodes.Add(new TreeNode());
                AddKeywordToTreeNode(keyword, node.Nodes[index]);
            }
            // Output tree to the inp read-only textbox
            WriteTreeToTextBox();
            // Apply formating
            FormatInp(rtbInpFile);
            // Expant first tree node
            cltvKeywordsTree.Nodes[0].Expand();
            // Add user keywords
            if (_userKeywords != null)
            {
                foreach (var entry in _userKeywords)
                {
                    AddUserKeywordToTreeByIndex(entry.Key, entry.Value.DeepClone());
                }
            }
        }
        // Add keywords to tree
        private void AddKeywordToTreeNode(CalculixKeyword keyword, TreeNode node)
        {
            string nodeText;
            if (keyword is CalTitle) nodeText = (keyword as CalTitle).Title;
            else nodeText = keyword.GetKeywordString();

            nodeText = GetFirstLineFromMultiline(nodeText);

            node.Text = nodeText;
            node.Name = node.Text;
            node.Tag = keyword;

            foreach (var childKeyword in keyword.Keywords)
            {
                TreeNode childNode = new TreeNode();
                node.Nodes.Add(childNode);
                AddKeywordToTreeNode(childKeyword, childNode);
            }
        }
        private void AddUserKeywordToTreeByIndex(int[] indices, CalculixKeyword keyword)
        {
            bool deactivated = false;
            TreeNode node = cltvKeywordsTree.Nodes[0];
            //
            for (int i = 0; i < indices.Length - 1; i++)
            {
                node.Expand();
                if (indices[i] < node.Nodes.Count)
                {
                    node = node.Nodes[indices[i]];
                    if (node.Tag != null && node.Tag is CalDeactivated) deactivated = true;
                }
                else return;
            }
            //
            TreeNode child = node.Nodes.Insert(indices[indices.Length - 1], "");
            // User keyword should not be deactivated in the user editor to enable editing
            AddUserKeywordToTreeNode(keyword, child);
            //
            node.Expand();
        }
        private void AddUserKeywordToTreeNode(CalculixKeyword keyword, TreeNode node)
        {
            node.Text = GetFirstLineFromMultiline(keyword.GetKeywordString());
            node.Tag = keyword;
            Font font = new Font(cltvKeywordsTree.Font, FontStyle.Bold);
            node.NodeFont = font;
            node.ForeColor = Color.Red;
        }
        // Write keywords from tree to inp textbox
        private void WriteTreeToTextBox()
        {
            _selectedKeywordFirstLine = -1;
            _selectedKeywordNumOfLines = -1;
            //
            string[] newLines = WriteTreeNodeToLines(cltvKeywordsTree.Nodes[0], 0).ToArray();
            string[] oldLines = rtbInpFile.Lines;
            bool equal = true;
            //
            if (newLines.Length == oldLines.Length)
            {
                for (int i = 0; i < newLines.Length; i++)
                {
                    if (newLines[i] != oldLines[i])
                    {
                        equal = false;
                        break;
                    }
                }
            }
            else equal = false;
            //
            //if (!equal)
            {
                rtbInpFile.Lines = newLines;
                FormatInp(rtbInpFile);
            }
        }
        private List<string> WriteTreeNodeToLines(TreeNode node, int lineCount)
        {
            List<string> lines = new List<string>();
            //
            if (node.Tag != null && node.Tag is CalculixKeyword)
            {
                if (cltvKeywordsTree.SelectedNodes.Contains(node)) _selectedKeywordFirstLine = lineCount;
                lines.AddRange(WriteKeywordToLines(node.Tag as CalculixKeyword));
                if (cltvKeywordsTree.SelectedNodes.Contains(node)) _selectedKeywordNumOfLines = lines.Count;
            }
            //
            foreach (TreeNode childNode in node.Nodes)
            {
                lines.AddRange(WriteTreeNodeToLines(childNode, lineCount + lines.Count));
            }
            //
            return lines;
        }
        private string[] WriteKeywordToLines(CalculixKeyword keyword)
        {
            string text = FileInOut.Output.CalculixFileWriter.GetShortKeywordData(keyword);
            return text.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
        // Update keyword text box
        private void UpdateKeywordTextBoxDelayed(int interval = 200)
        {
            if (timerUpdate.Enabled) timerUpdate.Stop();
            timerUpdate.Interval = interval;
            timerUpdate.Start();
        }
        private void UpdateKeywordTextBox()
        {
            try
            {
                // De-reference text changed event
                rtbKeyword.TextChanged -= rtbKeyword_TextChanged;
                // rtbKeyword.Tag is set by clicking on the tree node
                if (!rtbKeyword.ReadOnly && rtbKeyword.Tag != null && cltvKeywordsTree.SelectedNode != null)
                {
                    CalculixUserKeyword userKeyword = rtbKeyword.Tag as CalculixUserKeyword;
                    //
                    if (userKeyword != null)
                    {
                        userKeyword.Data = "";
                        int count = 0;
                        // Get only lines that contain data
                        foreach (var line in rtbKeyword.Lines)
                        {
                            if (line.Length > 0)
                            {
                                if (count > 0) userKeyword.Data += Environment.NewLine;
                                userKeyword.Data += line;
                                count++;
                            }
                        }
                        // Change the name of the Selected tree node
                        LockWindowUpdate(cltvKeywordsTree.Handle);
                        if (rtbKeyword.Lines.Length > 0 && rtbKeyword.Lines[0].Length > 0) cltvKeywordsTree.SelectedNode.Text = rtbKeyword.Lines[0];
                        else cltvKeywordsTree.SelectedNode.Text = "User keyword";
                        LockWindowUpdate(IntPtr.Zero);
                        //
                        FormatInp(rtbKeyword);
                    }
                }
                //
                WriteTreeToTextBox();
                //
                SelectKeywordLinesAndScrollToSelection();
                // Re-reference text changed event
                rtbKeyword.TextChanged += rtbKeyword_TextChanged;
            }
            catch
            { }
        }
        private void SelectKeywordLinesAndScrollToSelection()
        {
            if (_selectedKeywordFirstLine == -1 || _selectedKeywordNumOfLines == -1) return;
            // Apply selection
            rtbInpFile.SelectionStart = rtbInpFile.GetFirstCharIndexFromLine(_selectedKeywordFirstLine);
            int lastSelectedChar = rtbInpFile.GetFirstCharIndexFromLine(_selectedKeywordFirstLine + _selectedKeywordNumOfLines);
            if (lastSelectedChar == -1) lastSelectedChar = rtbInpFile.TextLength;       //if last line value = -1
            rtbInpFile.SelectionLength = lastSelectedChar - rtbInpFile.SelectionStart;
            // Apply formatting
            rtbInpFile.SelectionBackColor = Color.FromArgb(255, 210, 210);
            rtbInpFile.SelectionColor = Color.Red;
            Font font = new System.Drawing.Font(rtbInpFile.Font, FontStyle.Bold);
            rtbInpFile.SelectionFont = font;
            // Scroll to carret
            int firstVisibleChar = rtbInpFile.GetCharIndexFromPosition(new Point(0, 0));
            int firstVisibleLine = rtbInpFile.GetLineFromCharIndex(firstVisibleChar);
            //
            int lastVisibleChar = rtbInpFile.GetCharIndexFromPosition(new Point(0, rtbInpFile.Height));
            int lastVisibleLine = rtbInpFile.GetLineFromCharIndex(lastVisibleChar);
            //
            int numOfVisibleLines = lastVisibleLine - firstVisibleLine;
            int deltaLines = (numOfVisibleLines - _selectedKeywordNumOfLines) / 2;
            //
            if (_selectedKeywordFirstLine < firstVisibleLine || _selectedKeywordFirstLine + _selectedKeywordNumOfLines > lastVisibleLine)
            {
                firstVisibleLine = _selectedKeywordFirstLine - deltaLines;
                if (firstVisibleLine < 0) firstVisibleLine = 0;
                rtbInpFile.SelectionStart = rtbInpFile.GetFirstCharIndexFromLine(firstVisibleLine);
                rtbInpFile.ScrollToCaret();
            }
        }
        // Formatting
        private void FormatInp(RichTextBox rtb)
        {
            int start = rtb.SelectionStart;
            int len = rtb.SelectionLength;

            //Point point = new Point();
            //GetCaretPos(out point);

            try
            {
                int c0;
                int c1;
                string line;
                string[] lines = rtb.Lines;

                LockWindowUpdate(rtb.Handle);

                // Reset all color settings
                c0 = 0;
                c1 = rtb.Text.Length;
                rtb.SelectionStart = c0;
                rtb.SelectionLength = c1 - c0;
                rtb.SelectionBackColor = rtb.BackColor;
                rtb.SelectionColor = rtb.ForeColor;
                Font font = new System.Drawing.Font(rtbInpFile.Font, FontStyle.Regular);
                rtb.SelectionFont = font;

                for (int i = 0; i < lines.Length; i++)
                {
                    line = lines[i];

                    if (line.StartsWith("**") || line.StartsWith("*") || line.StartsWith("..."))
                    {
                        c0 = rtb.GetFirstCharIndexFromLine(i);          // very slow
                        c1 = rtb.GetFirstCharIndexFromLine(i + 1);      // very slow
                        if (c1 < 0) c1 = rtb.Text.Length;

                        if (line.StartsWith("**"))
                        {
                            rtb.SelectionStart = c0;
                            rtb.SelectionLength = c1 - c0;
                            rtb.SelectionBackColor = Color.FromArgb(230, 255, 230);
                            rtb.SelectionColor = Color.Green;
                        }
                        else if (line.StartsWith("*"))
                        {
                            rtb.SelectionStart = c0;
                            rtb.SelectionLength = c1 - c0;
                            rtb.SelectionColor = Color.Blue;
                        }
                        else if (line.StartsWith("..."))
                        {
                            rtb.SelectionStart = c0;
                            rtb.SelectionLength = c1 - c0;
                            rtb.SelectionColor = Color.Gray;
                        }
                    }
                   
                }
            }
            catch { }
            finally 
            { 
                LockWindowUpdate(IntPtr.Zero);

                rtb.SelectionStart = start;
                rtb.SelectionLength = len;
            }
            
        }
        //
        private string GetFirstLineFromMultiline(string multiLinedata)
        {
            string[] tmp = multiLinedata.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (tmp.Length > 0) return tmp[0];
            else return "User keyword";
        }

      
    }
}
