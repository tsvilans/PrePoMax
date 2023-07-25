using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;


namespace vtkControl
{
    class vtkMaxTextWithArrowWidget : vtkMaxTextWidget
    {
        // Variables                                                                                                                
        string _name;
        private bool _anchorAlreadySet;
        vtkCoordinate _worldAnchorPoint;
        vtkCoordinate _worldPositionPoint;
        //
        vtkPolyData _leaderPolyData;
        vtkPolyDataMapper2D _leaderMapper2D;
        vtkActor2D _leaderActor2D;
        //
        vtkPolyData _headPolyData;
        vtkGlyph3D _headGlyph;
        vtkActor2D _headActor2D;


        // Constructors                                                                                                             
        public vtkMaxTextWithArrowWidget(string name)
        {
            _name = name;
            //
            _anchorAlreadySet = false;
            _scaleBySectors = true;
            
            vtkDoubleArray vecs;

            _worldAnchorPoint = vtkCoordinate.New();
            _worldAnchorPoint.SetCoordinateSystemToWorld();
            _worldAnchorPoint.SetValue(0, 0, 0);

            _worldPositionPoint = vtkCoordinate.New();
            _worldPositionPoint.SetCoordinateSystemToWorld();

            // Leader - This is the leader (line) from the attachment point to the caption                              
            _leaderPolyData = vtkPolyData.New();
            vtkPoints pts = vtkPoints.New();
            pts.SetNumberOfPoints(2);
            pts.SetPoint(0, 0, 0, 0);
            pts.SetPoint(0, 100, 100, 0);
            _leaderPolyData.SetPoints(pts);

            vtkCellArray leader = vtkCellArray.New();
            leader.InsertNextCell(2);
            leader.InsertCellPoint(0);
            leader.InsertCellPoint(1); //at the attachment point
            _leaderPolyData.SetLines(leader);

            // mapper/actor combination
            _leaderMapper2D = vtkPolyDataMapper2D.New();
            _leaderMapper2D.SetInput(_leaderPolyData);

            _leaderActor2D = vtkActor2D.New();
            _leaderActor2D.SetMapper(_leaderMapper2D);
            _leaderActor2D.GetProperty().SetColor(0, 0, 0);


            // Arrow                                                                                                    
            vtkArrowSource arrowSource = vtkArrowSource.New();
            arrowSource.SetTipLength(1);
            arrowSource.SetTipRadius(0.2);
            arrowSource.SetShaftRadius(0.3);

            vtkTransform transform = vtkTransform.New();
            transform.Translate(-1, 0, 0); // so that tip is at the coordinates point

            // Transform the polydata
            vtkTransformPolyDataFilter translatedArrow = vtkTransformPolyDataFilter.New();
            translatedArrow.SetTransform(transform);
            translatedArrow.SetInputConnection(arrowSource.GetOutputPort());

            // Glyph - This is for glyphing the head of the leader: A single point with a vector for glyph orientation  
            _headPolyData = vtkPolyData.New();
            pts = vtkPoints.New();
            pts.SetNumberOfPoints(1);
            pts.SetPoint(0, 0, 0, 0);
            _headPolyData.SetPoints(pts);
            vecs = vtkDoubleArray.New();
            vecs.SetNumberOfComponents(3);
            vecs.SetNumberOfTuples(1);
            vecs.SetTuple3(0, 1, 1, 1);
            _headPolyData.GetPointData().SetVectors(vecs);

            _headGlyph = vtkGlyph3D.New();
            _headGlyph.SetSourceConnection(translatedArrow.GetOutputPort());
            _headGlyph.SetInput(_headPolyData);
            _headGlyph.SetScaleModeToDataScalingOff();
            _headGlyph.SetScaleFactor(20);
            _headGlyph.Update();

            // Mapper/actor combination
            vtkPolyDataMapper2D arrowMapper2D = vtkPolyDataMapper2D.New();
            arrowMapper2D.SetInput(_headGlyph.GetOutput());

            _headActor2D = vtkActor2D.New();
            _headActor2D.SetMapper(arrowMapper2D);
            _headActor2D.GetProperty().SetColor(0, 0, 0);
        }


        // Private methods                                                                                                          
        private void RecomputeLeader()
        {
            if (_position == null) return;              // if border is changed before _positionInPixels is initialized

            double[] end = RecomputeLeaderEnd();        // this is arrow head
            double[] start = RecomputeLeaderStart();    // this is the start of the arrow
            RecomputeHead(start, end);
        }
        private double[] RecomputeLeaderStart()
        {
            vtkPoints pts = _leaderPolyData.GetPoints();

            double[] head = pts.GetPoint(1);
            double[] start;
            if (head[0] < _position[0])                         // head is left from border
                start = new double[] { _position[0], _position[1] + _size[1] / 2 };
            else if (head[0] > _position[0] + _size[0])         // head is right from border
                start = new double[] { _position[0] + _size[0], _position[1] + _size[1] / 2 };
            else if (head[1] < _position[1])                    // head is below the border
                start = new double[] { _position[0] + _size[0] / 2, _position[1] };
            else                                                // head is over the border
                start = new double[] { _position[0] + _size[0] / 2, _position[1] + _size[1] };

            pts.SetPoint(0, start[0], start[1], 0);

            return start;
        }
        private double[] RecomputeLeaderEnd()
        {
            vtkPoints pts = _leaderPolyData.GetPoints();

            double[] attach = _worldAnchorPoint.GetComputedWorldValue(_renderer);
            _renderer.SetWorldPoint(attach[0], attach[1], attach[2], 1.0);
            _renderer.WorldToDisplay();
            double[] display = _renderer.GetDisplayPoint();
            pts.SetPoint(1, display[0], display[1], 0);

            return display;
        }
        private void RecomputeHead(double[] start, double[] end)
        {
            try
            {
                _headPolyData.GetPoints().SetPoint(0, end[0], end[1], 0);
                _headPolyData.GetPointData().GetVectors().SetTuple3(0, end[0] - start[0], end[1] - start[1], 0);
                _headPolyData.Modified();
            }
            catch { }
        }
        private double[] ProjectPointOnPlane(double[] point, double[] pointOnPlane, double[] normal)
        {
            //The projection of a point q = (x, y, z) onto a plane given by a point p = (a, b, c) and a normal n = (d, e, f) is
            //q_proj = q - dot(q - p, n) * n

            double[] tmp = new double[3];
            tmp[0] = point[0] - pointOnPlane[0];
            tmp[1] = point[1] - pointOnPlane[1];
            tmp[2] = point[2] - pointOnPlane[2];

            double dot = tmp[0] * normal[0] + tmp[1] * normal[1] + tmp[2] * normal[2];

            tmp[0] = point[0] - dot * normal[0];
            tmp[1] = point[1] - dot * normal[1];
            tmp[2] = point[2] - dot * normal[2];

            return tmp;
        }


        // Public methods                                                                                                           
        public override void OnRenderWindowModified()
        {
            if (!_visibility) return;

            base.OnRenderWindowModified();
            RecomputeLeader();
        }
        public override void CameraModified()
        {
            if (!_visibility) return;

            base.CameraModified();
            SetPositionFromWorldPosition();
            RecomputeLeader();
        }
        //
        public override void MiddleButtonPress(int x, int y)
        {
            if (!_visibility) return;
            
            base.MiddleButtonPress(x, y);
            RecomputeLeader();
        }
        public override void MiddleButtonRelease(int x, int y)
        {
            if (!_visibility) return;

            base.MiddleButtonRelease(x, y);
            SetPositionFromWorldPosition();
            RecomputeLeader();
        }
        public override bool MouseMove(int x, int y)
        {
            if (!_visibility) return false;

            if (base.MouseMove(x, y))
            {
                SetWorldPositionFromPosition();
                RecomputeLeader();
                return true;
            }
            return false;
        }
        public override void MousePan(int dx, int dy)
        {
            if (!_visibility) return;

            base.MousePan(dx, dy);
            SetPosition(_position[0] + dx, _position[1] + dy);
            RecomputeLeader();
        }
        public override void MouseRotate(double azimuthAngle, double ElevationAngle)
        {
            if (!_visibility) return;

            base.MouseRotate(azimuthAngle, ElevationAngle);
            SetPositionFromWorldPosition();
            RecomputeLeader();
        }
        public override void MouseWheelScrolled()
        {
            if (!_visibility) return;

            base.MouseWheelScrolled();
            SetPositionFromWorldPosition();
            RecomputeLeader();
        }
        //
        public override void VisibilityOn()
        {
            if (_visibility == false)
            {
                SetWorldPositionFromPosition();
                // Arrow first
                if (_leaderActor2D != null) _renderer.AddActor(_leaderActor2D);
                if (_headActor2D != null) _renderer.AddActor(_headActor2D);
                // Box last
                base.VisibilityOn();
            }
        }
        public override void VisibilityOff()
        {
            if (_visibility == true)
            {
                base.VisibilityOff();
                if (_leaderActor2D != null) _renderer.RemoveActor(_leaderActor2D);
                if (_headActor2D != null) _renderer.RemoveActor(_headActor2D);
            }
        }
        //
        public void ResetInitialPosition()
        {
            _anchorAlreadySet = false;
        }


        // Public setters                                                                                                           
        public override void SetInteractor(vtkRenderer renderer, vtkRenderWindowInteractor renderWindowInteractor)
        {
            // Arrow first
            renderer.AddActor(_leaderActor2D);
            renderer.AddActor(_headActor2D);
            // Box last
            base.SetInteractor(renderer, renderWindowInteractor);
        }
        public override void RemoveInteractor()
        {
            _renderer.RemoveActor(_leaderActor2D);
            _renderer.RemoveActor(_headActor2D);
            //
            base.RemoveInteractor();
        }
        public void SetAnchorPoint(double x, double y, double z)
        {
            _worldAnchorPoint.SetValue(x, y, z);

            if (!_anchorAlreadySet)     // change the position only the first time the anchor point is set
            {
                _renderer.SetWorldPoint(x, y, z, 1.0);
                _renderer.WorldToDisplay();
                double[] displayAnchor = _renderer.GetDisplayPoint();

                int[] windowSize = _renderer.GetSize();

                double dx = -80;
                double dy = 50;

                if (displayAnchor[0] + dx + _size[0] > windowSize[0]) dx = -(_size[0] + dx);     // if there is no room to the left, go to the right
                if (displayAnchor[1] + dy + _size[1] > windowSize[1]) dy = -(_size[1] + dy);     // if there is no room to the top, go down

                _renderer.SetDisplayPoint(displayAnchor[0] + dx, displayAnchor[1] + dy, 0);
                _renderer.DisplayToWorld();
                double[] positionWorld = _renderer.GetWorldPoint();

                double[] normal = _renderer.GetActiveCamera().GetViewPlaneNormal();
                double[] pointOnPlane = _worldAnchorPoint.GetValue();
                double[] projectedOnPlane = ProjectPointOnPlane(positionWorld, pointOnPlane, normal);
                _worldPositionPoint.SetValue(projectedOnPlane[0], projectedOnPlane[1], projectedOnPlane[2]);

                SetPositionFromWorldPosition();

                _anchorAlreadySet = true;
            }
            RecomputeLeader();
            //OnBorderRepresentationModified();
        }
        private void SetPositionFromWorldPosition()
        {
            double[] positionWorld = _worldPositionPoint.GetValue();
            // Get display projection from the world position
            _renderer.SetWorldPoint(positionWorld[0], positionWorld[1], positionWorld[2], 1.0);
            _renderer.WorldToDisplay();
            double[] displayPosition = _renderer.GetDisplayPoint();
            //
            if (false)
            {
                CaeGlobals.Vec3D center = new CaeGlobals.Vec3D(displayPosition[0], displayPosition[1], 0);
                CaeGlobals.Vec3D size = new CaeGlobals.Vec3D(_size[0], _size[1], 0);
                RecomputeLeaderStart();
                vtkPoints pts = _leaderPolyData.GetPoints();
                CaeGlobals.Vec3D arrow = new CaeGlobals.Vec3D(pts.GetPoint(1));

                CaeGlobals.Vec3D direction = center - arrow;
                double minDist = size.Len / 2 + 20;
                if (direction.Len < minDist)
                {
                    direction.Normalize();
                    center = arrow + direction * minDist;
                    displayPosition[0] = center.X;
                    displayPosition[1] = center.Y;
                }
            }
            //
            displayPosition[0] -= _size[0] / 2;
            displayPosition[1] -= _size[1] / 2;
            //
            SetPosition(displayPosition[0], displayPosition[1]);
        }
        private void SetWorldPositionFromPosition()
        {
            if (_position == null) return;      // if borederRepresentation is changed before _positionInPixels is initialized
            //
            vtkCamera camera = _renderer.GetActiveCamera();
            // Get world projection from display position
            _renderer.SetDisplayPoint(_position[0] + _size[0] / 2, _position[1] + _size[1] / 2, 0);
            _renderer.DisplayToWorld();
            double[] positionWorld = _renderer.GetWorldPoint();
            // Project world position on the plane through anchor point
            double[] normal = camera.GetViewPlaneNormal();
            double[] pointOnPlane = _worldAnchorPoint.GetValue();
            double[] projectedOnPlane = ProjectPointOnPlane(positionWorld, pointOnPlane, normal);
            // Set world position
            _worldPositionPoint.SetValue(projectedOnPlane[0], projectedOnPlane[1], projectedOnPlane[2]);
        }
        //
        public void OffsetPosition(double x, double y)
        {
            _position[0] += x;
            _position[1] += y;
            //
            SetWorldPositionFromPosition();
            //
            SetPositionFromWorldPosition(); // must be here
            RecomputeLeader();              // must be here
        }
        
        
        // Public getters                                                                                                           
        public string GetName()
        {
            return _name;
        }
        public CaeMesh.BoundingBox GetAnchorPointBox()
        {
            vtkPoints pts = _leaderPolyData.GetPoints();
            double[] head = pts.GetPoint(1);
            //
            double offset = 15; //px
            CaeMesh.BoundingBox box = new CaeMesh.BoundingBox();
            box.MinX = head[0] - offset;
            box.MaxX = head[0] + offset;
            box.MinY = head[1] - offset;
            box.MaxY = head[1] + offset;
            box.MinZ = head[2] - offset;
            box.MaxZ = head[2] + offset;
            box.MinZ = 0;
            box.MaxZ = 1;
            //
            return box;
        }
    }
}
