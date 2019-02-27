﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing;

namespace PrePoMax.Settings
{
    [Serializable]
    public class ViewPreSettings : ViewSettings, IReset
    {
        // Variables                                                                                                                
        private PreSettings _preSettings;


        // Properties                                                                                                               
        [CategoryAttribute("Selection")]
        [OrderedDisplayName(0, 10, "Highlight color")]
        [DescriptionAttribute("Select the highlight color.")]
        public Color HighlightColor
        {
            get { return _preSettings.HighlightColor; }
            set { _preSettings.HighlightColor = value; }
        }

        [CategoryAttribute("Selection")]
        [OrderedDisplayName(1, 10, "Mouse highlight color")]
        [DescriptionAttribute("Select the mouse highlight color.")]
        public Color MouseHighlightColor
        {
            get { return _preSettings.MouseHighlightColor; }
            set { _preSettings.MouseHighlightColor = value; }
        }

        [CategoryAttribute("Symbols")]
        [OrderedDisplayName(0, 10, "Constraint color")]
        [DescriptionAttribute("Select the constraint symbol color.")]
        public Color ConstraintSymbolColor
        {
            get { return _preSettings.ConstraintSymbolColor; }
            set { _preSettings.ConstraintSymbolColor = value; }
        }

        [CategoryAttribute("Symbols")]
        [OrderedDisplayName(1, 10, "Boundary condition color")]
        [DescriptionAttribute("Select the boundary condition symbol color.")]
        public Color BoundaryConditionSymbolColor
        {
            get { return _preSettings.BoundaryConditionSymbolColor; }
            set { _preSettings.BoundaryConditionSymbolColor = value; }
        }
        [CategoryAttribute("Symbols")]
        [OrderedDisplayName(2, 10, "Load color")]
        [DescriptionAttribute("Select the load symbol color.")]
        public Color LoadSymbolColor
        {
            get { return _preSettings.LoadSymbolColor; }
            set { _preSettings.LoadSymbolColor = value; }
        }
        [CategoryAttribute("Symbols")]
        [OrderedDisplayName(3, 10, "Symbol size")]
        [DescriptionAttribute("Select the symbol size.")]
        public int SymbolSize
        {
            get { return _preSettings.SymbolSize; }
            set { _preSettings.SymbolSize = value; }
        }
        [CategoryAttribute("Symbols")]
        [OrderedDisplayName(4, 10, "Node symbol size")]
        [DescriptionAttribute("Select the node symbol size.")]
        public int NodeSymbolSize
        {
            get { return _preSettings.NodeSymbolSize; }
            set { _preSettings.NodeSymbolSize = value; }
        }


        // Constructors                                                                                                             
        public ViewPreSettings(PreSettings preSettings)
        {
            _preSettings = preSettings;
        }


        // Methods                                                                                                                  
        public override ISettings GetBase()
        {
            return _preSettings;
        }

        public void Reset()
        {
            _preSettings.Reset();
        }
    }

}
