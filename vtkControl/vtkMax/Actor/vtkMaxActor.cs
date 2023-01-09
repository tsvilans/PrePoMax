using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;
using CaeGlobals;

namespace vtkControl
{
    public class vtkMaxActor
    {
        // Variables                                                                                                                
        private string _name;
        private vtkMaxExtreemeNode _minNode;
        private vtkMaxExtreemeNode _maxNode;
        //
        private vtkMapper _geometryMapper;
        // Actors
        private vtkActor _geometry;
        private vtkActor _elementEdges;
        private vtkActor _modelEdges;
        //
        private vtkProperty _geometryProperty;
        private vtkProperty _elementEdgesProperty;
        private vtkProperty _modelEdgesProperty;
        //
        private vtkPointData _geometryPointData;
        private vtkCellLocator _cellLocator;          // for surface picking
        private vtkCellLocator _frustumCellLocator;   // for volume picking
        private vtkPointData _frustumPointData;
        private vtkMaxActor _sectionViewActor;
        private List<vtkMaxActor> _copies;
        //
        public vtkMaxActorRepresentation _actorRepresentation;
        private bool _visible;
        private bool _backfaceCulling;
        private System.Drawing.Color _color;
        private System.Drawing.Color _backfaceColor;
        private System.Drawing.Color[] _colorTable;
        private double _ambient;
        private double _diffuse;
        private bool _colorContours;
        private bool _sectionViewPossible;
        private bool _drawOnGeometry;
        private bool _useSecondaryHighightColor;


        // Properties                                                                                                               
        public string Name { get { return _name; } set { _name = value; } }
        public vtkMaxExtreemeNode MinNode { get { return _minNode; } set { _minNode = value; } }
        public vtkMaxExtreemeNode MaxNode { get { return _maxNode; } set { _maxNode = value; } }
        //
        public vtkMapper GeometryMapper
        {
            get { return _geometryMapper; }
            set
            {
                _geometryMapper = value;
                if (_geometry != null) _geometry.SetMapper(_geometryMapper);
            }
        }
        // Actors
        public vtkActor Geometry
        {
            get { return _geometry; }
            set
            {
                _geometry = value;
                if (_geometry != null)
                {
                    _geometryMapper = _geometry.GetMapper();
                    _geometryProperty = _geometry.GetProperty();
                }
            }
        }
        public vtkActor ElementEdges
        {
            get { return _elementEdges; }
            set
            {
                _elementEdges = value;
                if (_elementEdges != null) _elementEdgesProperty = _elementEdges.GetProperty();
            }
        }
        public vtkActor ModelEdges
        {
            get { return _modelEdges; }
            set
            {
                _modelEdges = value;
                if (_modelEdges != null) _modelEdgesProperty = _modelEdges.GetProperty();
            }
        }
        // Properties
        public vtkProperty GeometryProperty
        {
            get { return _geometryProperty; }
            set
            {
                _geometryProperty = value;
                if (_geometry != null) _geometry.SetProperty(_geometryProperty);
            }
        }
        public vtkProperty ElementEdgesProperty { get { return _elementEdgesProperty; } set { _elementEdgesProperty = value; } }
        public vtkProperty ModelEdgesProperty { get { return _modelEdgesProperty; } set { _modelEdgesProperty = value; } }
        //
        public vtkPointData GeometryPointData { get { return _geometryPointData; } }
        public vtkCellLocator CellLocator { get { return _cellLocator; } set { _cellLocator = value; } }
        public vtkCellLocator FrustumCellLocator { get { return _frustumCellLocator; } set { _frustumCellLocator = value; } }
        public vtkPointData FrustumPointData { get { return _frustumPointData; } }
        public vtkMaxActor SectionViewActor { get { return _sectionViewActor; } set { _sectionViewActor = value; } }
        public List<vtkMaxActor> Copies { get { return _copies; } }
        public vtkMaxActorRepresentation ActorRepresentation
        {
            get { return _actorRepresentation; }
            set { _actorRepresentation = value; }
        }
        public bool VtkMaxActorVisible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                _geometry.SetVisibility(_visible ? 1 : 0);    // to base vtkActor
            }
        }
        public bool BackfaceCulling { get { return _backfaceCulling; } set { _backfaceCulling = value; } }
        public bool Pickable { get { return _geometry.GetPickable() == 1; } }
        public System.Drawing.Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                UpdateColor();
            }
        }
        public System.Drawing.Color BackfaceColor
        {
            get { return _backfaceColor; }
            set
            {
                _backfaceColor = value;
                UpdateColor();
            }
        }
        public System.Drawing.Color[] ColorTable
        {
            get { return _colorTable; }
            set
            {
                _colorTable = value;
                UpdateColor();
            }
        }
        public double Ambient
        {
            get { return _ambient; }
            set
            {
                _ambient = value;
                UpdateColor();
            }
        }
        public double Diffuse
        {
            get { return _diffuse; }
            set
            {
                _diffuse = value;
                UpdateColor();
            }
        }
        public bool ColorContours { get { return _colorContours; } set { _colorContours = value; } }
        public bool SectionViewPossible { get { return _sectionViewPossible; } set { _sectionViewPossible = value; } }
        public bool DrawOnGeometry { get { return _drawOnGeometry; } set { _drawOnGeometry = value; } }
        public bool UseSecondaryHighightColor 
        { 
            get { return _useSecondaryHighightColor; } 
            set { _useSecondaryHighightColor = value; } 
        }


        // Constructors                                                                                                             
        public vtkMaxActor()
        {
            _name = null;
            _minNode = null;
            _maxNode = null;
            //
            _geometry = new vtkActor();
            _elementEdges = null;
            _modelEdges = null;
            //
            _geometryProperty = _geometry.GetProperty();
            _elementEdgesProperty = null;
            _modelEdgesProperty = null;
            //
            _geometryMapper = _geometry.GetMapper();
            //
            _cellLocator = null;
            _frustumCellLocator = null;
            _sectionViewActor = null;
            _copies = new List<vtkMaxActor>();
            _actorRepresentation = vtkMaxActorRepresentation.Unknown;
            _visible = true;
            _backfaceCulling = true;
            _color = System.Drawing.Color.Empty;
            _backfaceColor = System.Drawing.Color.Empty;
            _colorTable = null;
            _ambient = 0.5;
            _diffuse = 0.5;
            _colorContours = false;
            _sectionViewPossible = true;
            _drawOnGeometry = false;
            _useSecondaryHighightColor = false;
            //
            UpdateColor();
        }
        public vtkMaxActor(vtkUnstructuredGrid source)
            : this ()
        {
            // Unstructured grid
            vtkUnstructuredGrid uGridActor = vtkUnstructuredGrid.New();
            vtkUnstructuredGrid uGridEdges = vtkUnstructuredGrid.New();
            // Add the points
            uGridActor.SetPoints(source.GetPoints());
            uGridEdges.SetPoints(source.GetPoints());
            // Add the cells
            AddCellDataToGrid(source, uGridActor, uGridEdges);
            // Update grid
            uGridActor.Update();
            uGridEdges.Update();
            // Actor
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInput(uGridActor);
            _geometryMapper = mapper;
            _geometry.SetMapper(_geometryMapper);
            _geometry.PickableOff();
            _geometryProperty = _geometry.GetProperty();
            // Edges
            _elementEdges = GetActorEdgesFromGridByVisualizationSurfaceExtraction(uGridEdges);
            if (_elementEdges != null) _elementEdgesProperty = _elementEdges.GetProperty();
        }
        public vtkMaxActor(vtkMaxActorData data)
            : this(data, false, false)
        {
        }
        public vtkMaxActor(vtkMaxActorData data, vtkMapper mapper)
            : this()
        {
            _name = data.Name;
            _geometryMapper = mapper;
            _geometry.SetMapper(_geometryMapper);
            _geometry.SetPickable(data.Pickable ? 1 : 0);
            _geometry.GetProperty().SetLineWidth(data.LineWidth);
            //
            _geometryProperty = _geometry.GetProperty();
            //
            _actorRepresentation = data.ActorRepresentation;
            _backfaceCulling = data.BackfaceCulling;
            _color = data.Color;
            _backfaceColor = data.BackfaceColor;
            _colorTable = data.ColorTable;
            _ambient = data.Ambient;
            _diffuse = data.Diffuse;
            _colorContours = data.ColorContours;
            _sectionViewPossible = data.SectionViewPossible;
            _drawOnGeometry = data.DrawOnGeometry;
            //
            UpdateColor();
        }
        public vtkMaxActor(vtkMaxActorData data, bool extractVisualizationSurface, bool createNodalActor)
            : this()
        {
            this._name = data.Name;
            //
            if ((data.Geometry.Cells.CellNodeIds == null && data.Geometry.Nodes.Coor != null) || createNodalActor)
            {
                CreateNodalActor(data);
            }
            else
            {
                if (extractVisualizationSurface) CreateFromDataByVisualizationSurfaceExtraction(data);
                else
                {
                    CreatePolyFromData(data);
                }
                //
                if (data.Pickable && _geometryMapper.GetInputAsDataSet().GetCellData().GetGlobalIds() != null)
                {
                    _geometry.SetPickable(1);
                    _cellLocator = vtkCellLocator.New();
                    _cellLocator.SetDataSet(_geometryMapper.GetInputAsDataSet());
                    _cellLocator.LazyEvaluationOn();
                }
                else
                {
                    _geometry.SetPickable(0);
                }
            }
            // Property
            _geometryProperty.SetLineWidth(data.LineWidth);
            _geometryProperty.SetPointSize(data.NodeSize);
            if (_modelEdgesProperty != null) _modelEdgesProperty.SetPointSize(data.NodeSize);
            if (_elementEdgesProperty != null) _elementEdgesProperty.SetPointSize(data.NodeSize);
            //
            _actorRepresentation = data.ActorRepresentation;
            _backfaceCulling = data.BackfaceCulling;
            _color = data.Color;
            _backfaceColor = data.BackfaceColor;
            _colorTable = data.ColorTable;
            _ambient = data.Ambient;
            _diffuse = data.Diffuse;
            _colorContours = data.ColorContours;
            _sectionViewPossible = data.SectionViewPossible;
            _drawOnGeometry = data.DrawOnGeometry;
            _useSecondaryHighightColor = data.UseSecondaryHighightColor;
            //
            UpdateColor();
        }
        public vtkMaxActor(vtkMaxActor sourceActor)
          : this()
        {
            this._name = sourceActor.Name;           
            //
            if (sourceActor.MinNode != null) _minNode = new vtkMaxExtreemeNode(sourceActor.MinNode);
            if (sourceActor.MaxNode != null) _maxNode = new vtkMaxExtreemeNode(sourceActor.MaxNode);
            //
            // Geometry                                                         
            // Polydata
            vtkPolyData polydata = vtkPolyData.New();
            polydata.DeepCopy(sourceActor.Geometry.GetMapper().GetInput());
            // Mapper
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInput(polydata);
            _geometryMapper = mapper;
            // Actor
            _geometry.SetMapper(_geometryMapper);
            _geometry.GetProperty().DeepCopy(sourceActor.GeometryProperty);
            _geometryProperty = _geometry.GetProperty();
            //_geometry.GetProperty().SetLineWidth(sourceActor.Geometry.GetProperty().GetLineWidth());
            _geometry.SetVisibility(sourceActor.Geometry.GetVisibility());
            _geometry.SetPickable(sourceActor.Geometry.GetPickable());
            // Cell Locator
            if (sourceActor.CellLocator != null)
            {
                _cellLocator = vtkCellLocator.New();
                _cellLocator.SetDataSet(_geometryMapper.GetInputAsDataSet());
                _cellLocator.LazyEvaluationOn();
            }
            // Frustum Locator
            if (sourceActor.FrustumCellLocator != null)
            {
                vtkUnstructuredGrid grid = vtkUnstructuredGrid.New();
                grid.DeepCopy(sourceActor.FrustumCellLocator.GetDataSet());
                //
                _frustumCellLocator = vtkCellLocator.New();
                _frustumCellLocator.SetDataSet(grid);
                _frustumCellLocator.LazyEvaluationOn();
            }
            // Element edges                                                    
            if (sourceActor.ElementEdges != null)
            {
                // Polydata
                polydata = vtkPolyData.New();
                polydata.DeepCopy(sourceActor.ElementEdges.GetMapper().GetInput());
                // Mapper
                mapper = vtkDataSetMapper.New();
                mapper.SetInput(polydata);
                // Actor
                _elementEdges = new vtkActor();
                _elementEdges.SetMapper(mapper);
                _elementEdges.GetProperty().DeepCopy(sourceActor.ElementEdges.GetProperty());
                _elementEdges.SetVisibility(sourceActor.ElementEdges.GetVisibility());
                _elementEdges.SetPickable(sourceActor.ElementEdges.GetPickable());
                //
                _elementEdgesProperty = _elementEdges.GetProperty();
            }
            // Model edges                                                      
            if (sourceActor.ModelEdges != null)
            {
                // Polydata
                polydata = vtkPolyData.New();
                polydata.DeepCopy(sourceActor.ModelEdges.GetMapper().GetInput());
                // Mapper
                mapper = vtkDataSetMapper.New();
                mapper.SetInput(polydata);
                // Actor
                _modelEdges = vtkActor.New();
                _modelEdges.SetMapper(mapper);
                _modelEdges.GetProperty().DeepCopy(sourceActor.ModelEdges.GetProperty());
                _modelEdges.SetVisibility(sourceActor.ModelEdges.GetVisibility());
                _modelEdges.SetPickable(sourceActor.ModelEdges.GetPickable());
                //
                _modelEdgesProperty = _modelEdges.GetProperty();
            }
            // Section view
            _sectionViewActor = sourceActor.SectionViewActor;
            // transformed copies
            _copies = new List<vtkMaxActor>(sourceActor.Copies);
            //
            _actorRepresentation = sourceActor.ActorRepresentation;
            VtkMaxActorVisible = sourceActor.VtkMaxActorVisible;
            _backfaceCulling = sourceActor.BackfaceCulling;
            _color = sourceActor.Color;
            _backfaceColor = sourceActor.BackfaceColor;
            _colorTable = sourceActor.ColorTable;
            _ambient = sourceActor.Ambient;
            _diffuse = sourceActor.Diffuse;
            _colorContours = sourceActor.ColorContours;
            _sectionViewPossible = sourceActor.SectionViewPossible;
            _drawOnGeometry = sourceActor.DrawOnGeometry;
            _useSecondaryHighightColor = sourceActor.UseSecondaryHighightColor;
            //
            UpdateColor();
        }


        // Methods                                                                                                                  
        public void CropWithCylinder(double r, string fileName)
        {
            vtkTransform transformCylinder = vtkTransform.New();
            transformCylinder.Identity();
            //transformCylinder.Translate(-.4, .4, 0);
            //transformCylinder.RotateZ(30);
            //transformCylinder.RotateY(60);
            transformCylinder.RotateX(90);
            //transformCylinder.Inverse();
            vtkCylinder cylinder = vtkCylinder.New();
            cylinder.SetTransform(transformCylinder);
            cylinder.SetRadius(r);
            //
            vtkClipPolyData clipper = vtkClipPolyData.New();
            clipper.SetInput(_geometryMapper.GetInput());
            clipper.SetClipFunction(cylinder);
            clipper.GenerateClippedOutputOn();
            clipper.GenerateClipScalarsOn();
            clipper.SetValue(0);
            //
            vtkDecimatePro decimate = vtkDecimatePro.New();
            decimate.SetInput(clipper.GetClippedOutput());
            decimate.SetTargetReduction(0.05);
            decimate.PreserveTopologyOn();
            decimate.Update();
            //
            vtkSTLWriter stlWriter = vtkSTLWriter.New();
            stlWriter.SetFileName(fileName);
            stlWriter.SetInput(decimate.GetOutput());
            stlWriter.Write();
        }
        public void CropWithCube(double a, string fileName)
        {
            vtkBox box = vtkBox.New();
            box.SetBounds(0, a, 0, a, -100 * a, 200 * a);
            //
            vtkClipPolyData clipper = vtkClipPolyData.New();
            clipper.SetInput(_geometryMapper.GetInput());
            clipper.SetClipFunction(box);
            clipper.GenerateClippedOutputOn();
            clipper.GenerateClipScalarsOn();
            clipper.SetValue(0);
            //
            vtkDecimatePro decimate = vtkDecimatePro.New();
            decimate.SetInput(clipper.GetClippedOutput());
            decimate.SetTargetReduction(0.05);
            decimate.PreserveTopologyOn();
            decimate.Update();
            //
            vtkSTLWriter stlWriter = vtkSTLWriter.New();
            stlWriter.SetFileName(fileName);
            stlWriter.SetInput(decimate.GetOutput());
            stlWriter.Write();
        }
        public void Smooth(double a, string fileName)
        {
            vtkWindowedSincPolyDataFilter smoother = vtkWindowedSincPolyDataFilter.New();
            vtkSmoothPolyDataFilter smoothFilter = vtkSmoothPolyDataFilter.New();
            //
            if (true)
            {
                int smoothingIterations = 20;
                double passBand = 0.1;
                double featureAngle = 360.0;
                //
                smoother.SetInput(_geometryMapper.GetInput());
                smoother.NormalizeCoordinatesOn();
                smoother.SetNumberOfIterations(smoothingIterations);
                //smoother.BoundarySmoothingOff();
                smoother.BoundarySmoothingOn();
                smoother.FeatureEdgeSmoothingOn();
                //smoother.FeatureEdgeSmoothingOn();
                smoother.SetFeatureAngle(featureAngle);
                smoother.SetPassBand(passBand);
                smoother.NonManifoldSmoothingOn();
                //smoother.NormalizeCoordinatesOn();
                smoother.Update();
            }
            if (false)
            {
                smoothFilter.SetInput(_geometryMapper.GetInput());
                smoothFilter.SetNumberOfIterations(15);
                smoothFilter.SetRelaxationFactor(0.1);
                smoothFilter.FeatureEdgeSmoothingOff();
                smoothFilter.BoundarySmoothingOn();
                smoothFilter.Update();
            }
            //
            vtkDecimatePro decimate = vtkDecimatePro.New();
            decimate.SetInput(smoother.GetOutput());
            decimate.PreserveTopologyOff();
            decimate.SetTargetReduction(0.95);
            decimate.SplittingOn();
            decimate.BoundaryVertexDeletionOn();
            decimate.PreserveTopologyOff();
            decimate.Update();
            //
            //
            vtkSTLWriter stlWriter = vtkSTLWriter.New();
            stlWriter.SetFileName(fileName);
            stlWriter.SetInput(smoother.GetOutput());
            stlWriter.Write();
        }
        private void AddCellDataToGrid(vtkUnstructuredGrid source, vtkUnstructuredGrid uGridActor, vtkUnstructuredGrid uGridEdges)
        {
            List<int> actorCellIds = new List<int>();

            int cellType;
            bool isEdge;
            vtkIdList list = vtkIdList.New();

            uGridActor.ShallowCopy(source);

            vtkUnsignedCharArray types = source.GetCellTypesArray();

            for (int i = 0; i < types.GetNumberOfTuples(); i++)
            {
                cellType = (int)types.GetTuple1(i);
                isEdge = cellType == (int)vtkCellType.VTK_LINE || cellType == (int)vtkCellType.VTK_QUADRATIC_EDGE;

                if (!isEdge)
                {
                    source.GetCellPoints(i, list);
                    uGridEdges.InsertNextCell(cellType, list);
                }
            }
        }
        private vtkActor GetActorEdgesFromGridByVisualizationSurfaceExtraction(vtkUnstructuredGrid uGridEdges)
        {
            if (uGridEdges.GetNumberOfCells() <= 0) return null;

            // extract visualization surface of the unstructured grid
            vtkUnstructuredGridGeometryFilter filter = vtkUnstructuredGridGeometryFilter.New();
            filter.SetInput(uGridEdges);
            filter.Update();

            // extract edges of the visualization surface
            vtkExtractEdges extractEdges = vtkExtractEdges.New();
            extractEdges.SetInput(uGridEdges);
            extractEdges.Update();

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(extractEdges.GetOutput());

            vtkActor edges = vtkActor.New();
            edges.SetMapper(mapper);
            edges.PickableOff();

            return edges;
        }
        //                          
        public void CreateNodalActor(vtkMaxActorData data)
        {
            // Create the geometry of a point (the coordinate)
            vtkPoints points = vtkPoints.New();
            // Create the topology of the point (a vertex)
            vtkCellArray vertices = vtkCellArray.New();
            for (int i = 0; i < data.Geometry.Nodes.Coor.GetLength(0); i++)
            {
                points.InsertNextPoint(data.Geometry.Nodes.Coor[i][0], data.Geometry.Nodes.Coor[i][1], data.Geometry.Nodes.Coor[i][2]);
                vertices.InsertNextCell(1);
                vertices.InsertCellPoint(i);
            }
            // Create a polydata object
            vtkPolyData pointsPolyData = vtkPolyData.New();
            // Set the points and vertices created as the geometry and topology of the polydata
            pointsPolyData.SetPoints(points);
            pointsPolyData.SetVerts(vertices);
            // Mapper
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(pointsPolyData);
            _geometryMapper = mapper;
            // Actor
            _geometry.SetMapper(_geometryMapper);
            //
            _geometryProperty = _geometry.GetProperty();
            _geometryProperty.SetColor(data.Color.R / 255d, data.Color.G / 255d, data.Color.B / 255d);
        }
        private void CreateFromDataByVisualizationSurfaceExtraction(vtkMaxActorData data)
        {
            // Unstructured grid
            vtkUnstructuredGrid uGridActor;
            vtkUnstructuredGrid uGridEdges;
            //
            bool addBackFace = data.Layer == vtkRendererLayer.Selection;
            addBackFace = false;
            //
            AddNodeAndCellDataToGrid(data.Geometry, out uGridActor, out uGridEdges, false, addBackFace);
            // Extract visualization surface of the unstructured grid - filter only keeps PedigreeIds and not GlobalIds
            vtkUnstructuredGridGeometryFilter filter = vtkUnstructuredGridGeometryFilter.New();
            filter.SetInput(uGridActor);
            filter.PassThroughPointIdsOn();
            filter.PassThroughCellIdsOn();
            filter.Update();
            // Mapper
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInput(filter.GetOutput());
            _geometryMapper = mapper;
            // Actor
            _geometry.SetMapper(_geometryMapper);
            //
            _geometryProperty = _geometry.GetProperty();
            _geometryProperty.SetColor(_color.R / 255d, _color.G / 255d, _color.B / 255d);
            _geometryProperty.SetOpacity(_color.A / 255d);
            //
            if (data.Pickable) _geometry.PickableOn();
            else _geometry.PickableOff();
            // Edges
            if (data.CanHaveElementEdges)
            {
                _elementEdges = GetActorEdgesFromGridByVisualizationSurfaceExtraction(uGridEdges);
                _elementEdgesProperty = _elementEdges.GetProperty();
            }
        }
        private void CreatePolyFromData(vtkMaxActorData data)
        {
            // Poly data actor
            vtkPolyData polyActor;
            vtkPolyData polyEdges;
            //
            int[] numOfCellPolys;
            AddNodeAndCellDataToPoly(data.Geometry, out polyActor, out polyEdges, out numOfCellPolys, true);
            // Add node scalars
            if (data.Geometry.Nodes.Values != null)
            {
                vtkFloatArray scalars = vtkFloatArray.New();
                scalars.SetName(Globals.ScalarArrayName);
                scalars.SetNumberOfValues(data.Geometry.Nodes.Values.Length);
                for (int i = 0; i < data.Geometry.Nodes.Values.Length; i++)
                {
                    scalars.SetValue(i, data.Geometry.Nodes.Values[i]);
                }
                // Set scalars
                polyActor.GetPointData().SetScalars(scalars);
                // Set extreme nodes data
                if (data.Geometry.ExtremeNodes.Ids != null)
                {
                    _minNode = new vtkMaxExtreemeNode(data.Geometry.ExtremeNodes.Ids[0], data.Geometry.ExtremeNodes.Coor[0],
                                                      data.Geometry.ExtremeNodes.Values[0]);
                    _maxNode = new vtkMaxExtreemeNode(data.Geometry.ExtremeNodes.Ids[1], data.Geometry.ExtremeNodes.Coor[1],
                                                      data.Geometry.ExtremeNodes.Values[1]);
                }
            }
            // Add cell scalars            
            if (data.Geometry.Cells.Values != null)
            {
                // Set scalars
                float value;
                vtkFloatArray cellData = vtkFloatArray.New();                
                for (int i = 0; i < data.Geometry.Cells.Values.Length; i++)
                {
                    value = data.Geometry.Cells.Values[i];
                    for (int j = 0; j < numOfCellPolys[i]; j++) cellData.InsertNextValue(value);
                }
                polyActor.GetCellData().SetScalars(cellData);                
            }

            // Actor                                                                                                                
            // Polydata
            vtkPolyData polydata = polyActor;
            
            // Smoothing
            //vtkLoopSubdivisionFilter subdivisionFilter = vtkLoopSubdivisionFilter.New();
            //subdivisionFilter.SetInput(polydata);
            //subdivisionFilter.SetNumberOfSubdivisions(2);
            //subdivisionFilter.Update();
            //polydata = subdivisionFilter.GetOutput();

            polydata.GetPointData().CopyScalarsOn();
            // Normals
            if (data.SmoothShaded) polydata.GetPointData().SetNormals(ComputeNormals(polydata));
            // Mapper
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInput(polydata);
            _geometryMapper = mapper;
            // Actor
            _geometry.SetMapper(_geometryMapper);
            _geometryPointData = polydata.GetPointData();
            //
            if (data.Pickable) _geometry.PickableOn();
            else _geometry.PickableOff();
            //
            _geometryProperty = _geometry.GetProperty();
            // Frustum locator
            //if (data.Layer == vtkRendererLayer.Base && data.Pickable && data.CellLocator != null)
            if (data.Layer == vtkRendererLayer.Base && data.CellLocator != null)
            {
                vtkUnstructuredGrid uGridCellLocator;
                //
                AddNodeAndCellDataToGrid(data.CellLocator, out uGridCellLocator);
                //
                _frustumCellLocator = vtkCellLocator.New();
                _frustumCellLocator.SetDataSet(uGridCellLocator);
                _frustumCellLocator.LazyEvaluationOn();
                _frustumCellLocator.GetDataSet().GetPointData().SetGlobalIds((vtkUnsignedLongArray)uGridCellLocator.GetPointData().GetGlobalIds());
                _frustumCellLocator.GetDataSet().GetCellData().SetGlobalIds((vtkUnsignedLongArray)uGridCellLocator.GetCellData().GetGlobalIds());
                // Add scalars
                if (data.CellLocator.Nodes.Values != null)
                {
                    vtkFloatArray scalars = vtkFloatArray.New();
                    scalars.SetName(Globals.ScalarArrayName);
                    scalars.SetNumberOfValues(data.CellLocator.Nodes.Values.Length);
                    for (int i = 0; i < data.CellLocator.Nodes.Values.Length; i++)
                    {
                        scalars.SetValue(i, data.CellLocator.Nodes.Values[i]);
                    }
                    // Set scalars
                    _frustumPointData = _frustumCellLocator.GetDataSet().GetPointData();
                    _frustumPointData.SetScalars(scalars);
                }
            }
            // Element edges                                                                                                        
            if (data.CanHaveElementEdges)
            {
                // Mapper
                mapper = vtkDataSetMapper.New();
                mapper.SetInput(polyEdges);
                // Actor
                _elementEdges = new vtkActor();
                _elementEdges.SetMapper(mapper);
                _elementEdges.PickableOff();
                //
                _elementEdgesProperty = _elementEdges.GetProperty();
                _elementEdgesProperty.SetLighting(true);
            }
            // Model edges                                                                                                          
            if (data.ModelEdges != null)
            {
                AddNodeAndCellDataToPoly(data.ModelEdges, out polyActor, out polyEdges, out numOfCellPolys, false);
                // Polydata
                polydata = polyActor;
                // Mapper
                mapper = vtkDataSetMapper.New();
                mapper.SetInput(polydata);
                // Actor
                _modelEdges = vtkActor.New();
                _modelEdges.SetMapper(mapper);
                _modelEdges.PickableOff();
                //
                _modelEdgesProperty = _modelEdges.GetProperty();
                _modelEdgesProperty.SetLighting(true);
                //
                if (!data.CanHaveElementEdges)
                {
                    // Recreate data
                    polydata.DeepCopy(polydata);
                    // Mapper
                    mapper = vtkDataSetMapper.New();
                    mapper.SetInput(polydata);
                    // Actor
                    _elementEdges = new vtkActor();
                    _elementEdges.SetMapper(mapper);
                    _elementEdges.PickableOff();
                    //
                    _elementEdgesProperty = _elementEdges.GetProperty();
                    _elementEdgesProperty.SetLighting(true);
                }
            }
        }
        //
        private void AddNodeAndCellDataToGrid(PartExchangeData data, out vtkUnstructuredGrid uGridActor)
        {           
            vtkUnstructuredGrid uGridEdges;
            AddNodeAndCellDataToGrid(data, out uGridActor, out uGridEdges, false, false);
        }
        private void AddNodeAndCellDataToGrid(PartExchangeData data, out vtkUnstructuredGrid uGridActor,
                                              out vtkUnstructuredGrid uGridEdges, 
                                              bool extractEdges, bool addBackFace)
        {
            // Unstructured grid
            uGridActor = vtkUnstructuredGrid.New();
            uGridEdges = vtkUnstructuredGrid.New();

            // Add the points
            vtkPoints points = vtkPoints.New();
            points.SetNumberOfPoints(data.Nodes.Coor.Length);

            for (int i = 0; i < data.Nodes.Coor.Length; i++)
            {
                points.SetPoint(i, data.Nodes.Coor[i][0], data.Nodes.Coor[i][1], data.Nodes.Coor[i][2]);
            }
            uGridActor.SetPoints(points);
            uGridEdges.SetPoints(points);

            // Add the cells
            AddCellDataToGrid(data, ref uGridActor, ref uGridEdges, extractEdges, addBackFace);

            // Add node ids to points in the grid
            if (data.Nodes.Ids != null)
            {
                vtkUnsignedLongArray pointIds = vtkUnsignedLongArray.New();
                pointIds.SetNumberOfValues(data.Nodes.Ids.Length);
                for (int i = 0; i < data.Nodes.Ids.Length; i++)
                {
                    pointIds.SetValue(i, (uint)data.Nodes.Ids[i]);
                }
                uGridActor.GetPointData().SetGlobalIds(pointIds);
            }

            // Add cell ids to cells in the grid
            if (data.Cells.Ids != null)
            {
                vtkUnsignedLongArray cellIds = vtkUnsignedLongArray.New();
                cellIds.SetNumberOfValues(data.Cells.Ids.Length);
                for (int i = 0; i < data.Cells.Ids.Length; i++)
                {
                    cellIds.SetValue(i, (uint)data.Cells.Ids[i]);
                }
                uGridActor.GetCellData().SetGlobalIds(cellIds);
            }

            // Update grid
            uGridActor.Update();
            uGridEdges.Update();                        
        }

        private static void AddNodeAndCellDataToPoly(PartExchangeData data, out vtkPolyData polyActor, out vtkPolyData polyEdges,
                                                     out int[] numOfCellPolys, bool extractEdges)
        {
            polyActor = vtkPolyData.New();
            polyEdges = vtkPolyData.New();
            // Add the points
            vtkPoints points = vtkPoints.New();
            points.SetNumberOfPoints(data.Nodes.Coor.Length);
            for (int i = 0; i < data.Nodes.Coor.Length; i++)
            {
                points.SetPoint(i, data.Nodes.Coor[i][0], data.Nodes.Coor[i][1], data.Nodes.Coor[i][2]);
            }
            polyActor.SetPoints(points);
            polyEdges.SetPoints(points);
            // Add the cells
            AddCellDataToPoly(data, ref polyActor, ref polyEdges, out numOfCellPolys, extractEdges);
            // Add node ids to points in the grid
            if (data.Nodes.Ids != null)
            {
                vtkUnsignedLongArray pointIds = vtkUnsignedLongArray.New();
                pointIds.SetNumberOfValues(data.Nodes.Ids.Length);
                for (int i = 0; i < data.Nodes.Ids.Length; i++)
                {
                    pointIds.SetValue(i, (uint)data.Nodes.Ids[i]);
                }
                polyActor.GetPointData().SetGlobalIds(pointIds);
            }
            // Add cell ids to cells in the grid
            if (data.Cells.Ids != null)
            {
                uint id;
                vtkUnsignedLongArray cellIds = vtkUnsignedLongArray.New();
                //cellIds.SetNumberOfValues(data.Cells.Ids.Length);
                for (int i = 0; i < data.Cells.Ids.Length; i++)
                {
                    id = (uint)data.Cells.Ids[i];
                    for (int j = 0; j < numOfCellPolys[i]; j++) cellIds.InsertNextValue(id);
                }
                polyActor.GetCellData().SetGlobalIds(cellIds);
            }
            // Update grid
            polyActor.Update();
            polyEdges.Update();
        }
        private void AddCellDataToGrid(PartExchangeData data, ref vtkUnstructuredGrid uGridActor, ref vtkUnstructuredGrid uGridEdges, 
                                       bool extractEdges, bool addBackFace)
        {
            int[] nodeIds;
            int cellType;
            bool isEdge;
            bool isSurface;
            vtkIdList nodeList = vtkIdList.New();
            vtkIdList edgeNodeList = vtkIdList.New();
            List<int> actorCellIds = new List<int>();

            for (int i = 0; i < data.Cells.CellNodeIds.GetLength(0); i++)
            {
                nodeIds = data.Cells.CellNodeIds[i];
                cellType = data.Cells.Types[i];
                isEdge = cellType == (int)vtkCellType.VTK_LINE || cellType == (int)vtkCellType.VTK_QUADRATIC_EDGE;
                isSurface = cellType == (int)vtkCellType.VTK_TRIANGLE || cellType == (int)vtkCellType.VTK_QUAD
                                        || cellType == (int)vtkCellType.VTK_QUADRATIC_TRIANGLE || cellType == (int)vtkCellType.VTK_QUADRATIC_QUAD;

                // Node list
                nodeList.SetNumberOfIds(nodeIds.Length);
                for (int j = 0; j < nodeIds.Length; j++) nodeList.SetId(j, nodeIds[j]);

                // Cell edges                                                                                                       
                if (!isEdge && uGridEdges != null)
                {
                    if (!extractEdges) uGridEdges.InsertNextCell(cellType, nodeList);
                    else
                    {
                        if (cellType == (int)vtkCellType.VTK_TRIANGLE)
                        {
                            edgeNodeList.SetNumberOfIds(2);
                            for (int j = 0; j < 3; j++)
                            {
                                edgeNodeList.SetId(0, nodeIds[j]);
                                edgeNodeList.SetId(1, nodeIds[(j + 1) % 3]);
                                uGridEdges.InsertNextCell((int)vtkCellType.VTK_LINE, edgeNodeList);
                            }
                        }
                        else if (cellType == (int)vtkCellType.VTK_QUAD)
                        {
                            edgeNodeList.SetNumberOfIds(2);
                            for (int j = 0; j < 4; j++)
                            {
                                edgeNodeList.SetId(0, nodeIds[j]);
                                edgeNodeList.SetId(1, nodeIds[(j + 1) % 4]);
                                uGridEdges.InsertNextCell((int)vtkCellType.VTK_LINE, edgeNodeList);
                            }
                        }
                        else if (cellType == (int)vtkCellType.VTK_QUADRATIC_TRIANGLE)
                        {
                            edgeNodeList.SetNumberOfIds(3);
                            for (int j = 0; j < 3; j++)
                            {
                                edgeNodeList.SetId(0, nodeIds[j]);
                                edgeNodeList.SetId(1, nodeIds[(j + 1) % 3]);
                                edgeNodeList.SetId(2, nodeIds[j + 3]);
                                uGridEdges.InsertNextCell((int)vtkCellType.VTK_QUADRATIC_EDGE, edgeNodeList);
                            }
                        }
                        else if (cellType == (int)vtkCellType.VTK_QUADRATIC_QUAD)
                        {
                            edgeNodeList.SetNumberOfIds(3);
                            for (int j = 0; j < 4; j++)
                            {
                                edgeNodeList.SetId(0, nodeIds[j]);
                                edgeNodeList.SetId(1, nodeIds[(j + 1) % 4]);
                                edgeNodeList.SetId(2, nodeIds[j + 4]);
                                uGridEdges.InsertNextCell((int)vtkCellType.VTK_QUADRATIC_EDGE, edgeNodeList);
                            }
                        }
                    }
                }

                // Cells                                                                                                            
                uGridActor.InsertNextCell(cellType, nodeList);
                if (data.Cells.Ids != null) actorCellIds.Add(data.Cells.Ids[i]);

                // Cells backface
                if (isSurface && addBackFace) 
                {
                    nodeList.SetNumberOfIds(nodeIds.Length);
                    if (cellType == (int)vtkCellType.VTK_TRIANGLE)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[2]);
                        nodeList.SetId(2, nodeIds[1]);
                    }
                    else if (cellType == (int)vtkCellType.VTK_QUAD)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[3]);
                        nodeList.SetId(2, nodeIds[2]);
                        nodeList.SetId(3, nodeIds[1]);
                    }
                    else if (cellType == (int)vtkCellType.VTK_QUADRATIC_TRIANGLE)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[2]);
                        nodeList.SetId(2, nodeIds[1]);
                        nodeList.SetId(3, nodeIds[5]);
                        nodeList.SetId(4, nodeIds[4]);
                        nodeList.SetId(5, nodeIds[3]);
                    }
                    else if (cellType == (int)vtkCellType.VTK_QUADRATIC_QUAD)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[3]);
                        nodeList.SetId(2, nodeIds[2]);
                        nodeList.SetId(3, nodeIds[1]);
                        nodeList.SetId(4, nodeIds[7]);
                        nodeList.SetId(5, nodeIds[6]);
                        nodeList.SetId(6, nodeIds[5]);
                        nodeList.SetId(7, nodeIds[4]);
                    }
                    else throw new NotSupportedException();

                    uGridActor.InsertNextCell(cellType, nodeList);
                    if (data.Cells.Ids != null) actorCellIds.Add(data.Cells.Ids[i]);
                }
            }

            if (data.Cells.Ids != null) data.Cells.Ids = actorCellIds.ToArray();
        }
        private static void AddCellDataToPoly_(PartExchangeData data, ref vtkPolyData polyActor, ref vtkPolyData polyEdges,
                                              out int[] numOfCellPolys, bool extractEdges)
        {
            double d1;
            double d2;
            //
            int[] nodeIds;
            double[][] nodeCoor;
            int cellType;
            bool isVertex;
            bool isEdge;
            bool isSurface;
            vtkIdList nodeList = vtkIdList.New();
            vtkIdList edgeNodeList = vtkIdList.New();
            //
            numOfCellPolys = null;
            if (data.Cells.Ids != null)
            {
                numOfCellPolys = new int[data.Cells.Ids.Length];
            }
            //
            int vertexType = (int)vtkCellType.VTK_VERTEX;
            int lineType = (int)vtkCellType.VTK_LINE;
            int quadraticEdgeType = (int)vtkCellType.VTK_QUADRATIC_EDGE;
            int triangleType = (int)vtkCellType.VTK_TRIANGLE;
            int quadType = (int)vtkCellType.VTK_QUAD;
            int quadraticTriangleType = (int)vtkCellType.VTK_QUADRATIC_TRIANGLE;
            int quadraticQuadType = (int)vtkCellType.VTK_QUADRATIC_QUAD;
            //
            polyActor.Allocate(10000, 10000);
            if (extractEdges) polyEdges.Allocate(1000, 1000);
            //
            Dictionary<int, HashSet<int[]>> cellTypeUniqueCells = new Dictionary<int, HashSet<int[]>>();

            for (int i = 0; i < data.Cells.CellNodeIds.Length; i++)
            {
                nodeIds = data.Cells.CellNodeIds[i];                
                cellType = data.Cells.Types[i];
                isVertex = cellType == vertexType;
                isEdge = cellType == lineType || cellType == quadraticEdgeType;
                isSurface = cellType == triangleType || cellType == quadType || cellType == quadraticTriangleType ||
                            cellType == quadraticQuadType;
                // Cell edges                                                                                                       
                if (extractEdges)
                {
                    if (isEdge)
                    {
                        edgeNodeList.SetNumberOfIds(1);
                        //if (cellType == lineType || cellType == quadraticEdgeType)
                        {
                            edgeNodeList.SetId(0, nodeIds[0]);
                            polyEdges.InsertNextCell(vertexType, edgeNodeList);
                            edgeNodeList.SetId(0, nodeIds[1]);
                            polyEdges.InsertNextCell(vertexType, edgeNodeList);
                        }
                    }
                    else if (isSurface)
                    {
                        edgeNodeList.SetNumberOfIds(2);
                        if (cellType == triangleType)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                edgeNodeList.SetId(0, nodeIds[j]);
                                edgeNodeList.SetId(1, nodeIds[(j + 1) % 3]);
                                polyEdges.InsertNextCell(lineType, edgeNodeList);
                            }
                        }
                        else if (cellType == quadType)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                edgeNodeList.SetId(0, nodeIds[j]);
                                edgeNodeList.SetId(1, nodeIds[(j + 1) % 4]);
                                polyEdges.InsertNextCell(lineType, edgeNodeList);
                            }
                        }
                        else if (cellType == quadraticTriangleType)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                edgeNodeList.SetId(0, nodeIds[j]);
                                edgeNodeList.SetId(1, nodeIds[j + 3]);
                                polyEdges.InsertNextCell(lineType, edgeNodeList);
                                edgeNodeList.SetId(0, nodeIds[j + 3]);
                                edgeNodeList.SetId(1, nodeIds[(j + 1) % 3]);
                                polyEdges.InsertNextCell(lineType, edgeNodeList);
                            }
                        }
                        else if (cellType == quadraticQuadType)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                edgeNodeList.SetId(0, nodeIds[j]);
                                edgeNodeList.SetId(1, nodeIds[j + 4]);
                                polyEdges.InsertNextCell(lineType, edgeNodeList);
                                edgeNodeList.SetId(0, nodeIds[j + 4]);
                                edgeNodeList.SetId(1, nodeIds[(j + 1) % 4]);
                                polyEdges.InsertNextCell(lineType, edgeNodeList);
                            }
                        }
                    }
                }
                // Cells                                                                                                            
                if (isVertex)
                {
                    nodeList.SetNumberOfIds(1);
                    //if (cellType == vertexType)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        polyActor.InsertNextCell(vertexType, nodeList);
                        if (numOfCellPolys != null) numOfCellPolys[i] = 1;
                    }
                }
                else if (isEdge)
                {
                    nodeList.SetNumberOfIds(2);
                    if (cellType == lineType)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[1]);
                        polyActor.InsertNextCell(lineType, nodeList);
                        if (numOfCellPolys != null) numOfCellPolys[i] = 1;
                    }
                    else if (cellType == quadraticEdgeType)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[2]);
                        polyActor.InsertNextCell(lineType, nodeList);
                        nodeList.SetId(0, nodeIds[2]);
                        nodeList.SetId(1, nodeIds[1]);
                        polyActor.InsertNextCell(lineType, nodeList);
                        if (numOfCellPolys != null) numOfCellPolys[i] = 2;
                    }
                }
                else if (isSurface)
                {
                    nodeList.SetNumberOfIds(3);
                    if (cellType == triangleType)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[1]);
                        nodeList.SetId(2, nodeIds[2]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        if (numOfCellPolys != null) numOfCellPolys[i] = 1;
                    }
                    else if (cellType == quadType)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[1]);
                        nodeList.SetId(2, nodeIds[2]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[2]);
                        nodeList.SetId(2, nodeIds[3]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        if (numOfCellPolys != null) numOfCellPolys[i] = 2;
                    }
                    else if (cellType == quadraticTriangleType)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[3]);
                        nodeList.SetId(2, nodeIds[5]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        nodeList.SetId(0, nodeIds[3]);
                        nodeList.SetId(1, nodeIds[4]);
                        nodeList.SetId(2, nodeIds[5]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        nodeList.SetId(0, nodeIds[3]);
                        nodeList.SetId(1, nodeIds[1]);
                        nodeList.SetId(2, nodeIds[4]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        nodeList.SetId(0, nodeIds[5]);
                        nodeList.SetId(1, nodeIds[4]);
                        nodeList.SetId(2, nodeIds[2]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        if (numOfCellPolys != null) numOfCellPolys[i] = 4;
                    }
                    else if (cellType == quadraticQuadType)
                    {
                        nodeCoor = data.Nodes.Coor;
                        d1 = Vec3D.GetDirVec(nodeCoor[nodeIds[4]], nodeCoor[nodeIds[6]]).Len2;
                        d2 = Vec3D.GetDirVec(nodeCoor[nodeIds[5]], nodeCoor[nodeIds[7]]).Len2;
                        if (d1 < d2)
                        {
                            nodeList.SetId(0, nodeIds[7]);
                            nodeList.SetId(1, nodeIds[0]);
                            nodeList.SetId(2, nodeIds[4]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[7]);
                            nodeList.SetId(1, nodeIds[4]);
                            nodeList.SetId(2, nodeIds[6]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[7]);
                            nodeList.SetId(1, nodeIds[6]);
                            nodeList.SetId(2, nodeIds[3]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[5]);
                            nodeList.SetId(1, nodeIds[4]);
                            nodeList.SetId(2, nodeIds[1]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[5]);
                            nodeList.SetId(1, nodeIds[6]);
                            nodeList.SetId(2, nodeIds[4]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[5]);
                            nodeList.SetId(1, nodeIds[2]);
                            nodeList.SetId(2, nodeIds[6]);
                            polyActor.InsertNextCell(triangleType, nodeList);                            
                        }
                        else
                        {
                            nodeList.SetId(0, nodeIds[4]);
                            nodeList.SetId(1, nodeIds[7]);
                            nodeList.SetId(2, nodeIds[0]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[4]);
                            nodeList.SetId(1, nodeIds[5]);
                            nodeList.SetId(2, nodeIds[7]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[4]);
                            nodeList.SetId(1, nodeIds[1]);
                            nodeList.SetId(2, nodeIds[5]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[6]);
                            nodeList.SetId(1, nodeIds[5]);
                            nodeList.SetId(2, nodeIds[2]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[6]);
                            nodeList.SetId(1, nodeIds[7]);
                            nodeList.SetId(2, nodeIds[5]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[6]);
                            nodeList.SetId(1, nodeIds[3]);
                            nodeList.SetId(2, nodeIds[7]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                        }
                        if (numOfCellPolys != null) numOfCellPolys[i] = 6;
                    }
                    else throw new NotSupportedException();
                }
                else throw new NotSupportedException();
            }
            //
            polyActor.Squeeze();
            if (extractEdges) polyEdges.Squeeze();
        }
        private static void AddCellDataToPoly(PartExchangeData data, ref vtkPolyData polyActor, ref vtkPolyData polyEdges,
                                              out int[] numOfCellPolys, bool extractEdges)
        {
            double d1;
            double d2;
            //
            int[] nodeIds;
            double[][] nodeCoor;
            int cellType;
            bool isVertex;
            bool isEdge;
            bool isSurface;
            vtkIdList nodeList = vtkIdList.New();
            vtkIdList edgeNodeList = vtkIdList.New();
            //
            numOfCellPolys = null;
            if (data.Cells.Ids != null)
            {
                numOfCellPolys = new int[data.Cells.Ids.Length];
            }
            //
            int vertexType = (int)vtkCellType.VTK_VERTEX;
            int lineType = (int)vtkCellType.VTK_LINE;
            int polyLineType = (int)vtkCellType.VTK_POLY_LINE;
            int quadraticEdgeType = (int)vtkCellType.VTK_QUADRATIC_EDGE;
            int triangleType = (int)vtkCellType.VTK_TRIANGLE;
            int quadType = (int)vtkCellType.VTK_QUAD;
            int quadraticTriangleType = (int)vtkCellType.VTK_QUADRATIC_TRIANGLE;
            int quadraticQuadType = (int)vtkCellType.VTK_QUADRATIC_QUAD;
            //
            polyActor.Allocate(10000, 10000);
            if (extractEdges) polyEdges.Allocate(1000, 1000);
            //
            int id1;
            int id2;
            int id3;
            HashSet<int> uniqueVertexIds = new HashSet<int>();
            HashSet<int[]> uniqueEdgeCells = new HashSet<int[]>();
            HashSet<int[]> uniquePolyEdgeCells = new HashSet<int[]>();

            for (int i = 0; i < data.Cells.CellNodeIds.Length; i++)
            {
                nodeIds = data.Cells.CellNodeIds[i];
                cellType = data.Cells.Types[i];
                isVertex = cellType == vertexType;
                isEdge = cellType == lineType || cellType == quadraticEdgeType;
                isSurface = cellType == triangleType || cellType == quadType || cellType == quadraticTriangleType ||
                            cellType == quadraticQuadType;
                // Cell edges                                                                                                       
                if (extractEdges)
                {
                    if (isEdge)
                    {
                        uniqueVertexIds.UnionWith(nodeIds);
                    }
                    else if (isSurface)
                    {
                        if (cellType == triangleType)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                id1 = nodeIds[j];
                                id2 = nodeIds[(j + 1) % 3];
                                uniqueEdgeCells.Add(new int[] { Math.Min(id1, id2), Math.Max(id1, id2) });
                            }
                        }
                        else if (cellType == quadType)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                id1 = nodeIds[j];
                                id2 = nodeIds[(j + 1) % 4];
                                uniqueEdgeCells.Add(new int[] { Math.Min(id1, id2), Math.Max(id1, id2) });
                            }
                        }
                        else if (cellType == quadraticTriangleType)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                id1 = nodeIds[j];
                                id2 = nodeIds[j + 3];
                                id3 = nodeIds[(j + 1) % 3];
                                if (id1 < id3) uniquePolyEdgeCells.Add(new int[] { id1, id2, id3 });
                                else uniquePolyEdgeCells.Add(new int[] { id3, id2, id1 });
                            }
                        }
                        else if (cellType == quadraticQuadType)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                id1 = nodeIds[j];
                                id2 = nodeIds[j + 4];
                                id3 = nodeIds[(j + 1) % 4];
                                if (id1 < id3) uniquePolyEdgeCells.Add(new int[] { id1, id2, id3 });
                                else uniquePolyEdgeCells.Add(new int[] { id3, id2, id1 });
                            }
                        }
                    }
                }
                // Cells                                                                                                            
                if (isVertex)
                {
                    nodeList.SetNumberOfIds(1);
                    //if (cellType == vertexType)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        polyActor.InsertNextCell(vertexType, nodeList);
                        if (numOfCellPolys != null) numOfCellPolys[i] = 1;
                    }
                }
                else if (isEdge)
                {
                    nodeList.SetNumberOfIds(2);
                    if (cellType == lineType)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[1]);
                        polyActor.InsertNextCell(lineType, nodeList);
                        if (numOfCellPolys != null) numOfCellPolys[i] = 1;
                    }
                    else if (cellType == quadraticEdgeType)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[2]);
                        polyActor.InsertNextCell(lineType, nodeList);
                        nodeList.SetId(0, nodeIds[2]);
                        nodeList.SetId(1, nodeIds[1]);
                        polyActor.InsertNextCell(lineType, nodeList);
                        if (numOfCellPolys != null) numOfCellPolys[i] = 2;
                    }
                }
                else if (isSurface)
                {
                    nodeList.SetNumberOfIds(3);
                    if (cellType == triangleType)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[1]);
                        nodeList.SetId(2, nodeIds[2]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        if (numOfCellPolys != null) numOfCellPolys[i] = 1;
                    }
                    else if (cellType == quadType)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[1]);
                        nodeList.SetId(2, nodeIds[2]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[2]);
                        nodeList.SetId(2, nodeIds[3]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        if (numOfCellPolys != null) numOfCellPolys[i] = 2;
                    }
                    else if (cellType == quadraticTriangleType)
                    {
                        nodeList.SetId(0, nodeIds[0]);
                        nodeList.SetId(1, nodeIds[3]);
                        nodeList.SetId(2, nodeIds[5]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        nodeList.SetId(0, nodeIds[3]);
                        nodeList.SetId(1, nodeIds[4]);
                        nodeList.SetId(2, nodeIds[5]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        nodeList.SetId(0, nodeIds[3]);
                        nodeList.SetId(1, nodeIds[1]);
                        nodeList.SetId(2, nodeIds[4]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        nodeList.SetId(0, nodeIds[5]);
                        nodeList.SetId(1, nodeIds[4]);
                        nodeList.SetId(2, nodeIds[2]);
                        polyActor.InsertNextCell(triangleType, nodeList);
                        if (numOfCellPolys != null) numOfCellPolys[i] = 4;
                    }
                    else if (cellType == quadraticQuadType)
                    {
                        nodeCoor = data.Nodes.Coor;
                        d1 = Vec3D.GetDirVec(nodeCoor[nodeIds[4]], nodeCoor[nodeIds[6]]).Len2;
                        d2 = Vec3D.GetDirVec(nodeCoor[nodeIds[5]], nodeCoor[nodeIds[7]]).Len2;
                        if (d1 < d2)
                        {
                            nodeList.SetId(0, nodeIds[7]);
                            nodeList.SetId(1, nodeIds[0]);
                            nodeList.SetId(2, nodeIds[4]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[7]);
                            nodeList.SetId(1, nodeIds[4]);
                            nodeList.SetId(2, nodeIds[6]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[7]);
                            nodeList.SetId(1, nodeIds[6]);
                            nodeList.SetId(2, nodeIds[3]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[5]);
                            nodeList.SetId(1, nodeIds[4]);
                            nodeList.SetId(2, nodeIds[1]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[5]);
                            nodeList.SetId(1, nodeIds[6]);
                            nodeList.SetId(2, nodeIds[4]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[5]);
                            nodeList.SetId(1, nodeIds[2]);
                            nodeList.SetId(2, nodeIds[6]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                        }
                        else
                        {
                            nodeList.SetId(0, nodeIds[4]);
                            nodeList.SetId(1, nodeIds[7]);
                            nodeList.SetId(2, nodeIds[0]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[4]);
                            nodeList.SetId(1, nodeIds[5]);
                            nodeList.SetId(2, nodeIds[7]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[4]);
                            nodeList.SetId(1, nodeIds[1]);
                            nodeList.SetId(2, nodeIds[5]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[6]);
                            nodeList.SetId(1, nodeIds[5]);
                            nodeList.SetId(2, nodeIds[2]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[6]);
                            nodeList.SetId(1, nodeIds[7]);
                            nodeList.SetId(2, nodeIds[5]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                            nodeList.SetId(0, nodeIds[6]);
                            nodeList.SetId(1, nodeIds[3]);
                            nodeList.SetId(2, nodeIds[7]);
                            polyActor.InsertNextCell(triangleType, nodeList);
                        }
                        if (numOfCellPolys != null) numOfCellPolys[i] = 6;
                    }
                    else throw new NotSupportedException();
                }
                else throw new NotSupportedException();
            }
            //
            if (extractEdges)
            {
                edgeNodeList.SetNumberOfIds(1);
                foreach (var vertexId in uniqueVertexIds)
                {
                    edgeNodeList.SetId(0, vertexId);
                    polyEdges.InsertNextCell(vertexType, edgeNodeList);
                }
                //
                edgeNodeList.SetNumberOfIds(2);
                foreach (var edgeCell in uniqueEdgeCells)
                {
                    edgeNodeList.SetId(0, edgeCell[0]);
                    edgeNodeList.SetId(1, edgeCell[1]);
                    polyEdges.InsertNextCell(lineType, edgeNodeList);
                }
                //
                edgeNodeList.SetNumberOfIds(3);
                foreach (var polyEdgeCell in uniquePolyEdgeCells)
                {
                    edgeNodeList.SetId(0, polyEdgeCell[0]);
                    edgeNodeList.SetId(1, polyEdgeCell[1]);
                    edgeNodeList.SetId(2, polyEdgeCell[2]);
                    polyEdges.InsertNextCell(polyLineType, edgeNodeList);
                }
            }
            //
            polyActor.Squeeze();
            if (extractEdges) polyEdges.Squeeze();
        }

        // Color and visuals
        public void UpdateColor()
        {
            double opacity = _color.A / 255d;
            // Custom coloring of elements: sections, materials...
            if (_colorTable != null)    
            {
                // Create color table
                vtkLookupTable cellLut = null;  //make it global!!!
                cellLut = vtkLookupTable.New();
                cellLut.SetNumberOfTableValues(_colorTable.Length + 1); // +1 for the missing color
                cellLut.Build();
                cellLut.SetTableValue(0, 1, 1, 1, 1);                 // White for missing color
                for (int i = 0; i < _colorTable.Length; i++)
                    cellLut.SetTableValue(i + 1, _colorTable[i].R / 255d, _colorTable[i].G / 255d, _colorTable[i].B / 255d, 1);
                //cellLut.SetValueRange(0, _colorTable.Length - 1);
                //
                _geometryMapper.SetLookupTable(cellLut);
                _geometryMapper.SetScalarRange(-1, _colorTable.Length - 1);
                //_geometryMapper.SetUseLookupTableScalarRange(1);

                //mapper.SetScalarRange(-1, data.ColorTable.Length - 1);
                //mapper.SetScalarModeToUseCellData();
                //mapper.SetInterpolateScalarsBeforeMapping(1);
                //mapper.SetColorModeToMapScalars();
                //
                _geometryProperty.SetAmbientColor(0.8, 0.8, 0.8);  // also reset part color highlight
                _geometryProperty.SetAmbient(0.2);
                _geometryProperty.SetDiffuse(0.201); // must be a little larger
                //
                _geometryProperty.SetOpacity(opacity);
                // This changes the base color and forces graphics update !?
                //_geometryProperty.SetSpecularColor(1, 1, 1);
                _geometryProperty.SetSpecular(0.6);
                _geometryProperty.SetSpecularPower(100);
            }
            else
            {
                _geometryProperty.SetAmbient(_ambient);
                _geometryProperty.SetDiffuse(_diffuse);
                //
                _geometryProperty.SetOpacity(opacity);
                //
                if (!ColorContours)
                {
                    _geometryProperty.SetColor(_color.R / 255d, _color.G / 255d, _color.B / 255d);
                    // This changes the base color and forces graphics update !?
                    //_geometryProperty.SetSpecularColor(1, 1, 1);
                    _geometryProperty.SetSpecular(0.6);
                    _geometryProperty.SetSpecularPower(100);
                }
                else
                {
                    // This changes the base color and forces graphics update !?
                    _geometryProperty.SetAmbientColor(1, 1, 1);  // also reset part color highlight
                }
            }
            //
            if (_modelEdges != null)
            {
                _modelEdgesProperty.SetColor(0, 0, 0);
                _modelEdgesProperty.SetLighting(false);
                _modelEdgesProperty.SetLineWidth(0.8f);
                _modelEdgesProperty.SetOpacity(opacity * 0.7);
            }
            //if (opacity < 1) opacity = Math.Min(0.2, opacity);
            if (_elementEdges != null)
            {
                _elementEdgesProperty.SetColor(0, 0, 0);
                _elementEdgesProperty.SetLighting(false);
                _elementEdgesProperty.SetLineWidth(0.5f);
                _elementEdgesProperty.SetOpacity(opacity * 0.4);
            }
            // Backface property
            if (!_backfaceColor.IsEmpty && !_backfaceCulling)
            {
                vtkProperty property = vtkProperty.New();
                //
                property.SetAmbient(_ambient);
                property.SetDiffuse(_diffuse);
                //
                opacity = _backfaceColor.A / 255d;
                property.SetOpacity(opacity);
                //
                if (!ColorContours)
                {
                    property.SetColor(_backfaceColor.R / 255d, _backfaceColor.G / 255d, _backfaceColor.B / 255d);
                    // This changes the base color and forces graphics update !?
                    //property.SetSpecularColor(1, 1, 1);
                    property.SetSpecular(0.6);
                    property.SetSpecularPower(100);
                }
                else
                {
                    // This changes the base color and forces graphics update !?
                    property.SetAmbientColor(1, 1, 1);  // also reset part color highlight
                }
                _geometry.SetBackfaceProperty(property);
            }
        }
        public void UpdateVisibility()
        {
            // Update even if VtkMaxActorVisible == _visible to set the visibility for the geometry actor
            VtkMaxActorVisible = _visible;
        }
        public void Highlight()
        {
            _geometryProperty.SetAmbientColor(1, 0, 0);
        }
        private vtkDataArray ComputeNormals(vtkPointSet pointSet)
        {
            // Recompute polydata normals
            vtkPolyDataNormals normalGenerator = vtkPolyDataNormals.New();
            normalGenerator.SetInput(pointSet);
            normalGenerator.SplittingOff(); // prevents changes in topology
            //normalGenerator.SetFeatureAngle(0.001 * Math.PI / 180);
            normalGenerator.ComputePointNormalsOn();
            normalGenerator.ComputeCellNormalsOff();
            normalGenerator.Update();
            return normalGenerator.GetOutput().GetPointData().GetNormals();
        }


        // Animation                                                                                                                
        public void SetAnimationFrame(vtkMaxActorData data, int frameNumber)
        {
            if (data.Geometry.NodesAnimation != null)
            {
                if (frameNumber < 0 || frameNumber >= data.Geometry.NodesAnimation.Length)
                    throw new Exception("The animation frame can not be set.");
                //
                vtkMaxActorAnimationData animationData = 
                    new vtkMaxActorAnimationData(data.Geometry.NodesAnimation[frameNumber].Coor, 
                                                 data.ModelEdges.NodesAnimation[frameNumber].Coor,
                                                 data.Geometry.NodesAnimation[frameNumber].Values,
                                                 data.Geometry.ExtremeNodesAnimation[frameNumber],
                                                 data.CellLocator.NodesAnimation[frameNumber].Coor,
                                                 data.CellLocator.NodesAnimation[frameNumber].Values);
                // Actor points
                if (animationData.Points != null)
                {
                    vtkPointSet pointSet = _geometryMapper.GetInput() as vtkPointSet;
                    pointSet.SetPoints(animationData.Points);
                    (_elementEdges.GetMapper().GetInput() as vtkPointSet).SetPoints(animationData.Points);
                    // Normals
                    if (pointSet.GetPointData().GetNormals() != null && animationData.PointNormals == null)
                        animationData.PointNormals = ComputeNormals(pointSet);
                    pointSet.GetPointData().SetNormals(animationData.PointNormals);
                }
                // Actor edges points
                if (animationData.ModelEdgesPoints != null && _modelEdges != null)
                    (_modelEdges.GetMapper().GetInput() as vtkPointSet).SetPoints(animationData.ModelEdgesPoints);
                // Values
                if (animationData.Values != null) _geometryMapper.GetInput().GetPointData().SetScalars(animationData.Values);
                // Min/max
                if (animationData.MinNode != null) _minNode = animationData.MinNode;
                if (animationData.MaxNode != null) _maxNode = animationData.MaxNode;
                // Locator points
                if (animationData.LocatorPoints != null)
                {
                    vtkPointSet pointSet = _frustumCellLocator.GetDataSet() as vtkPointSet;
                    pointSet.SetPoints(animationData.LocatorPoints);
                }
                // Locator values
                if (animationData.LocatorValues != null)
                    _frustumCellLocator.GetDataSet().GetPointData().SetScalars(animationData.LocatorValues);
            }
        }

        // Section cut                                                                                                              
        public void AddClippingPlane(vtkPlane sectionViewPlane)
        {
            _geometryMapper.AddClippingPlane(sectionViewPlane);
            if (ElementEdges != null) ElementEdges.GetMapper().AddClippingPlane(sectionViewPlane);
            if (ModelEdges != null) ModelEdges.GetMapper().AddClippingPlane(sectionViewPlane);
            //
            if (ActorRepresentation == vtkMaxActorRepresentation.SolidAsShell) _geometryProperty.BackfaceCullingOff();
        }
        public void RemoveAllClippingPlanes()
        {
            _geometryMapper.RemoveAllClippingPlanes();
            if (ElementEdges != null) ElementEdges.GetMapper().RemoveAllClippingPlanes();
            if (ModelEdges != null) ModelEdges.GetMapper().RemoveAllClippingPlanes();
        }
        // Exports
        public double[][][] GetStlTriangles()
        {
            vtkCell cell;
            vtkPoints points;
            long numOfPoints;
            long numCells = _geometryMapper.GetInput().GetNumberOfCells();
            double[][] trianlge;
            List<double[][]> trianlges = new List<double[][]>();
            //
            for (int i = 0; i < numCells; i++)
            {
                cell = _geometryMapper.GetInput().GetCell(i);
                points = cell.GetPoints();
                numOfPoints = points.GetNumberOfPoints();
                if (numOfPoints == 3)
                {
                    trianlge = new double[3][];
                    for (int j = 0; j < 3; j++)
                    {
                        trianlge[j] = points.GetPoint(j);
                    }
                    trianlges.Add(trianlge);
                }
                else throw new NotSupportedException();
            }
            //
            return trianlges.ToArray(); ;
        }
    }
}
