using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace UserControls
{
    public partial class CollapsableGroupBox : GroupBox
    {
        // Variables                                                                                                                
        private bool _isCollapsed = false;
        private Rectangle _buttonRectangle;
        private int _actualHeight = 0;
        private int _currentHeight = 0;
        private int _collapseHeight;
        private int _textWidth;
        private int _textHeight;
        private int _halfTextHeight;
        private List<Control> _visibleControls;
        private SolidBrush _drawBrush;
        

        // Properties                                                                                                               
        private int CollapseHeight
        {
            get { return _collapseHeight; }
        }
        private int ActualHeight
        {
            get { return _actualHeight; }
            set { _actualHeight = value; }
        }
        private int CurrentHeight
        {
            get { return _currentHeight; }
            set { _currentHeight = value; }
        }
        public bool IsCollapsed
        {
            get { return _isCollapsed; }
            set
            {
                if (value != _isCollapsed)
                {
                    _isCollapsed = value;
                    if (!_isCollapsed) Height = ActualHeight;
                    else Height = CollapseHeight;
                    //
                    foreach (Control c in _visibleControls) c.Visible = !value;
                    //
                    Invalidate();
                    //
                    if (OnCollapsedChanged != null) OnCollapsedChanged(this);
                }
            }
        }


        // Events                                                                                                                   
        public delegate void CollapseChangeEventHandler(object sender);
        public event CollapseChangeEventHandler OnCollapsedChanged;


        // Constructor                                                                                                              
        public CollapsableGroupBox()
        {
            InitializeComponent();
            //
            _buttonRectangle = new Rectangle(14, 0, 12, 12);
            _visibleControls = new List<Control>();
            _drawBrush = new SolidBrush(ForeColor);
            //
            GetFontSize();
        }


        // Methods                                                                                                                  
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (ActualHeight == 0) ActualHeight = Width;
            //
            DrawGroupBox(pe.Graphics);
            DrawButton(pe.Graphics);            
        }
        void DrawGroupBox(Graphics g)
        {
            Rectangle bounds = new Rectangle(ClientRectangle.X, ClientRectangle.Y + _halfTextHeight,
                                             ClientRectangle.Width, ClientRectangle.Height - _halfTextHeight);
            GroupBoxRenderer.DrawGroupBox(g, bounds, Enabled ? GroupBoxState.Normal : GroupBoxState.Disabled);
            //
            int pad = 21;
            //
            g.DrawLine(SystemPens.Control, pad + 1, _halfTextHeight, _textWidth + pad , _halfTextHeight);
            //
            Point p = new Point(pad, 0);
            TextRenderer.DrawText(g, Text, this.Font, p, ForeColor);
        }
        private void DrawButton(Graphics g)
        {
            Rectangle background = _buttonRectangle;
            //
            background.Inflate(2, 2);
            g.FillRectangle(SystemBrushes.Control, background);
            //g.DrawRectangle(SystemPens.ActiveBorder, background);
            //
            if (IsCollapsed) g.DrawImage(Properties.Resources.Expand, _buttonRectangle);
            else g.DrawImage(Properties.Resources.Collapse, _buttonRectangle);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_buttonRectangle.Contains(e.Location)) CollapsedChanged();                
            base.OnMouseUp(e);
        }
        private void CollapsedChanged()
        {
            IsCollapsed = !IsCollapsed;
            if (OnCollapsedChanged != null) OnCollapsedChanged(this);
        }
        protected override void OnResize(EventArgs e)
        {
            if (ActualHeight == 0) ActualHeight = Height;
            //
            UpdatebuttonPosition();
            //
            base.OnResize(e);
        }
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (_visibleControls.Count == 0)
            {
                foreach (Control c in Controls)
                {
                    if (c.Visible) _visibleControls.Add(c);
                }
            }
            base.OnLayout(levent);
        }
        protected override void OnTextChanged(EventArgs e)
        {
            GetFontSize();
            //
            base.OnTextChanged(e);
        }
        //
        private void GetFontSize()
        {
            Image image = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(image);
            //SizeF textSize = TextRenderer.MeasureText(Text, this.Font);
            SizeF textSize = g.MeasureString(Text, this.Font);
            _textWidth = (int)Math.Ceiling(textSize.Width);
            _textHeight = (int)Math.Ceiling(textSize.Height);
            _halfTextHeight = (int)Math.Ceiling(_textHeight / 2.0);
            //
            _textWidth = _textWidth < 1 ? 1 : _textWidth;
            _collapseHeight = Math.Max(25, _textHeight + 5);
            //
            UpdatebuttonPosition();
        }
        private void UpdatebuttonPosition()
        {
            _buttonRectangle.Y = _halfTextHeight - _buttonRectangle.Height / 2;
            _buttonRectangle.X = 9;
        }
    }
}
