﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vtkControl
{
    [Serializable]
    public enum vtkSelectItem
    {   
        None,
        Node,
        Element,
        Surface,
        Part
    }
}
