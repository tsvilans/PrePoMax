﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CaeMesh
{
    [Serializable]
    public enum vtkCellType
    {
        // Linear cells
        VTK_EMPTY_CELL = 0,
        VTK_VERTEX = 1,
        VTK_POLY_VERTEX = 2,
        VTK_LINE = 3,
        VTK_POLY_LINE = 4,
        VTK_TRIANGLE = 5,
        VTK_TRIANGLE_STRIP = 6,
        VTK_POLYGON = 7,
        VTK_PIXEL = 8,
        VTK_QUAD = 9,
        VTK_TETRA = 10,
        VTK_VOXEL = 11,
        VTK_HEXAHEDRON = 12,
        VTK_WEDGE = 13,
        VTK_PYRAMID = 14,
        VTK_PENTAGONAL_PRISM = 15,
        VTK_HEXAGONAL_PRISM = 16,

        // Quadratic, isoparametric cells
        VTK_QUADRATIC_EDGE = 21,
        VTK_QUADRATIC_TRIANGLE = 22,
        VTK_QUADRATIC_QUAD = 23,
        VTK_QUADRATIC_POLYGON = 36,
        VTK_QUADRATIC_TETRA = 24,
        VTK_QUADRATIC_HEXAHEDRON = 25,
        VTK_QUADRATIC_WEDGE = 26,
        VTK_QUADRATIC_PYRAMID = 27,
        VTK_BIQUADRATIC_QUAD = 28,
        VTK_TRIQUADRATIC_HEXAHEDRON = 29,
        VTK_QUADRATIC_LINEAR_QUAD = 30,
        VTK_QUADRATIC_LINEAR_WEDGE = 31,
        VTK_BIQUADRATIC_QUADRATIC_WEDGE = 32,
        VTK_BIQUADRATIC_QUADRATIC_HEXAHEDRON = 33,
        VTK_BIQUADRATIC_TRIANGLE = 34,

        // Cubic, isoparametric cell
        VTK_CUBIC_LINE = 35,

        // Special class of cells formed by convex group of points
        VTK_CONVEX_POINT_SET = 41,

        // Polyhedron cell (consisting of polygonal faces)
        VTK_POLYHEDRON = 42,

        // Higher order cells in parametric form
        VTK_PARAMETRIC_CURVE = 51,
        VTK_PARAMETRIC_SURFACE = 52,
        VTK_PARAMETRIC_TRI_SURFACE = 53,
        VTK_PARAMETRIC_QUAD_SURFACE = 54,
        VTK_PARAMETRIC_TETRA_REGION = 55,
        VTK_PARAMETRIC_HEX_REGION = 56,

        // Higher order cells
        VTK_HIGHER_ORDER_EDGE = 60,
        VTK_HIGHER_ORDER_TRIANGLE = 61,
        VTK_HIGHER_ORDER_QUAD = 62,
        VTK_HIGHER_ORDER_POLYGON = 63,
        VTK_HIGHER_ORDER_TETRAHEDRON = 64,
        VTK_HIGHER_ORDER_WEDGE = 65,
        VTK_HIGHER_ORDER_PYRAMID = 66,
        VTK_HIGHER_ORDER_HEXAHEDRON = 67,

        VTK_NUMBER_OF_CELL_TYPES
    }
    
}
