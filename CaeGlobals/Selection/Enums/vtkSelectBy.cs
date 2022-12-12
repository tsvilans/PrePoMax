using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeGlobals
{
    public enum vtkSelectBy
    {
        // General
        Off,
        Default,        
        // Mesh based
        Id,
        Node,
        Element,
        Edge,
        Surface,
        EdgeAngle,
        SurfaceAngle,
        Part,
        // Geometry
        Geometry,
        GeometryVertex,
        GeometryEdge,
        GeometrySurface,
        GeometryEdgeAngle,
        GeometrySurfaceAngle,
        GeometryPart,
        // Querry
        QueryNode,
        QueryElement,
        QueryEdge,
        QuerySurface,
        QueryPart,
        //
        Widget
    }
}
