﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    public abstract class ViewSurfaceInteractionProperty
    {
        // Variables                                                                                                                
        private DynamicCustomTypeDescriptor _dctd;


        // Properties                                                                                                               
        public abstract string Name { get; }
        //
        [Browsable(false)]
        public DynamicCustomTypeDescriptor DynamicCustomTypeDescriptor { get { return _dctd; } set { _dctd = value; } }
        public abstract CaeModel.SurfaceInteractionProperty Base { get; }
    }
}
