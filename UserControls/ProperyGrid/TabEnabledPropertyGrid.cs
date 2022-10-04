using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace UserControls
{
    // http://kiwigis.blogspot.si/2009/05/adding-tab-key-support-to-propertygrid.html
    public class TabEnabledPropertyGrid : PropertyGrid
    {
        // Variables                                                                                                                
        private bool _readOnly;


        // Variables                                                                                                                
        public bool ReadOnly { get { return _readOnly; } set { _readOnly = value; } }


        // Constructors                                                                                                             
        public TabEnabledPropertyGrid() : base() 
        {
            this.LineColor = System.Drawing.SystemColors.Control;
            this.DisabledItemForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            //
            _readOnly = false;
            //
            Site = new MySite(this);
        }
        //
        public void SetParent(Form form)
        {
            // Catch null arguments
            if (form == null)
            {
                throw new ArgumentNullException("form");
            }

            // Set this property to intercept all events
            form.KeyPreview = true;

            // Listen for keydown event
            form.KeyDown += new KeyEventHandler(this.Form_KeyDown);
        }
        public void SetLabelColumnWidth(double labelRatio)
        {
            // get the grid view

            Control view = (Control)typeof(PropertyGrid).GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((PropertyGrid)this);
            //Control view = (Control)typeof(PropertyGrid).GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(grid);

            // set label width
            //FieldInfo fi = view.GetType().GetField("labelWidth", BindingFlags.Instance | BindingFlags.NonPublic);
            //fi.SetValue(view, width);

            FieldInfo fi2 = view.GetType().GetField("labelRatio");
            fi2.SetValue(view, labelRatio);

            // refresh
            view.Invalidate();
        }
        //
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(DateTime.Now.Millisecond + e.KeyCode.ToString());
            // Exit if cursor not in control
            //if (!this.RectangleToScreen(this.ClientRectangle).Contains(Cursor.Position))
            if (!this.ContainsFocus)
            {
                return;
            }

            //if (e.KeyCode == Keys.Return) 
            //{
            //    SendKeys.Send("{Tab}");
            //    SendKeys.Send("{Tab}");
            //    return;
            //}

            // Handle tab key
            if (e.KeyCode != Keys.Tab) { return; }
            e.Handled = true;
            e.SuppressKeyPress = true;

            // Get selected griditem
            GridItem gridItem = this.SelectedGridItem;
            if (gridItem == null) { return; }

            // Create a collection all visible child griditems in propertygrid
            GridItem root = gridItem;
            while (root.GridItemType != GridItemType.Root)
            {
                root = root.Parent;
            }
            List<GridItem> gridItems = new List<GridItem>();
            this.FindItems(root, gridItems);

            // Get position of selected griditem in collection
            int index = gridItems.IndexOf(gridItem);

            int nextIndex = index + 1;  
            if (nextIndex >= gridItems.Count) 
            {
                //this.SelectedGridItem = gridItems[0];
                e.Handled = false;
                e.SuppressKeyPress = false;
                return; 
            }
            // Select next griditem in collection
            this.SelectedGridItem = gridItems[nextIndex];
            SendKeys.Send("{Tab}");
        }
        private void FindItems(GridItem item, List<GridItem> gridItems)
        {
            switch (item.GridItemType)
            {
                case GridItemType.Root:
                case GridItemType.Category:
                    foreach (GridItem i in item.GridItems)
                    {
                        this.FindItems(i, gridItems);
                    }
                    break;
                case GridItemType.Property:
                    gridItems.Add(item);
                    if (item.Expanded)
                    {
                        foreach (GridItem i in item.GridItems)
                        {
                            this.FindItems(i, gridItems);
                        }
                    }
                    break;
                case GridItemType.ArrayValue:
                    break;
            }
        }
        // Overrides
        protected override void OnGotFocus(EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(DateTime.Now.Millisecond + " OnGotFocus");
            //// Get selected griditem
            //GridItem gridItem = this.SelectedGridItem;
            //if (gridItem == null) { return; }

            //// Create a collection all visible child griditems in propertygrid
            //GridItem root = gridItem;
            //while (root.GridItemType != GridItemType.Root)
            //{
            //    root = root.Parent;
            //}
            //List<GridItem> gridItems = new List<GridItem>();
            //this.FindItems(root, gridItems);

            ////this.SelectedGridItem = gridItems[0];
            //this.SelectedGridItem = gridItem;
            
            ////SendKeys.Send("{Tab}");

            base.OnGotFocus(e);
        }
        protected override void OnEnter(EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(DateTime.Now.Millisecond + " OnEnter");
            // Get selected griditem
            GridItem gridItem = this.SelectedGridItem;
            if (gridItem == null) { return; }
            // Create a collection all visible child griditems in propertygrid
            GridItem root = gridItem;
            while (root.GridItemType != GridItemType.Root)
            {
                root = root.Parent;
            }
            List<GridItem> gridItems = new List<GridItem>();
            this.FindItems(root, gridItems);
            //
            this.SelectedGridItem = gridItems[0];
            //
            base.OnEnter(e);
        }
        protected override void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
        {
            if (_readOnly)
            {
                if (e.NewSelection.GridItemType == GridItemType.Property)
                {
                    if (e.NewSelection.Parent != null && e.NewSelection.Parent.GridItemType == GridItemType.Category)
                    {
                        this.SelectedGridItem = e.NewSelection.Parent;
                        return;
                    }
                }
            }
            else base.OnSelectedGridItemChanged(e);
        }
        //
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
        
    }
}
