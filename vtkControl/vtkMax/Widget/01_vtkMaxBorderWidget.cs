using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;
using System.Drawing;
using System.Windows.Forms;

namespace vtkControl
{
    enum vtkMaxWidgetPosition
    {
        FromBottomLeft,
        FromTopLeft,
        HorizontallyRelative
    }
    class vtkMaxBorderWidget
    {
        // Variables                                                                                                                
        protected vtkCoordinate _positionCoordinate;
        //
        protected vtkPolyDataMapper2D _borderMapper;
        protected vtkActor2D _borderActor;
        protected vtkPolyDataMapper2D _backgroundMapper;
        protected vtkActor2D _backgroundActor;
        //
        protected vtkRenderer _renderer;
        protected vtkRenderWindowInteractor _renderWindowInteractor;
        //
        protected bool _visibility;
        protected bool _borderVisibility;
        protected bool _backgroundVisibility;
        protected bool _scaleBySectors;
        protected int _quadrant;
        protected int[] _prewWindowSize;
        protected double[] _position;
        protected double[] _size;
        protected vtkMaxWidgetPosition _widgetPosition;
        //
        protected int[] _clickPos;


        // Events                                                                                                                   
        public event Action<object, MouseEventArgs> MouseDoubleClick;


        // Constructors                                                                                                             
        public vtkMaxBorderWidget()
        {
            _positionCoordinate = vtkCoordinate.New();
            _positionCoordinate.SetCoordinateSystemToDisplay();
            //
            _visibility = true;
            _borderVisibility = true;
            _backgroundVisibility = true;
            _position = null;
            _prewWindowSize = new int[] { 0, 0 };
            _scaleBySectors = true;
            //
            _position = new double[] { 100, 100 };
            _size = new double[] { 100, 100 };
            _widgetPosition = vtkMaxWidgetPosition.FromBottomLeft;
            //
            _clickPos = null;
            // Border                                                                          
            vtkPoints points = vtkPoints.New();
            points.InsertNextPoint(_position[0], _position[1], 0);
            points.InsertNextPoint(_position[0] + _size[0], _position[1], 0);
            points.InsertNextPoint(_position[0] + _size[0], _position[1] + _size[1], 0);
            points.InsertNextPoint(_position[0], _position[1] + _size[1], 0);
            //
            vtkCellArray lines = vtkCellArray.New();
            lines.InsertNextCell(2);
            lines.InsertCellPoint(0);
            lines.InsertCellPoint(1);
            //
            lines.InsertNextCell(2);
            lines.InsertCellPoint(1);
            lines.InsertCellPoint(2);
            //
            lines.InsertNextCell(2);
            lines.InsertCellPoint(2);
            lines.InsertCellPoint(3);
            //
            lines.InsertNextCell(2);
            lines.InsertCellPoint(3);
            lines.InsertCellPoint(0);
            //
            vtkPolyData scalarBarPolyData = vtkPolyData.New();
            scalarBarPolyData.SetPoints(points);
            scalarBarPolyData.SetLines(lines);
            //
            _borderMapper = vtkPolyDataMapper2D.New();
            _borderMapper.SetInput(scalarBarPolyData);
            //
            _borderActor = vtkActor2D.New();
            _borderActor.SetMapper(_borderMapper);
            _borderActor.GetProperty().SetColor(0, 0, 0);
            // Background                                                                   
            vtkCellArray backgroundPolygon = vtkCellArray.New();
            backgroundPolygon.InsertNextCell(4);
            backgroundPolygon.InsertCellPoint(0);
            backgroundPolygon.InsertCellPoint(1);
            backgroundPolygon.InsertCellPoint(2);
            backgroundPolygon.InsertCellPoint(3);
            //
            vtkPolyData backgroundPolyData = vtkPolyData.New();
            backgroundPolyData.SetPoints(_borderMapper.GetInput().GetPoints());
            backgroundPolyData.SetPolys(backgroundPolygon);
            //
            _backgroundMapper = vtkPolyDataMapper2D.New();
            _backgroundMapper.SetInput(backgroundPolyData);
            //
            _backgroundActor = vtkActor2D.New();
            _backgroundActor.SetMapper(_backgroundMapper);
            _backgroundActor.GetProperty().SetColor(1, 1, 1);
        }


        // Event handlers                                                                                                           
        void renderWindow_ModifiedEvt(vtkObject sender, vtkObjectEventArgs e)
        {
            int[] windowSize = _renderer.GetSize();
            if (windowSize[0] != _prewWindowSize[0] || windowSize[1] != _prewWindowSize[1])
            {
                OnRenderWindowModified();
            }
            _renderer.GetSize().CopyTo(_prewWindowSize, 0);
        }
      

        // Public methods                                                                                                           
        public virtual void VisibilityOn()
        {
            if (_visibility == false)
            {
                OnSizeChanged();
                _visibility = true;
                if (_backgroundVisibility == true && _backgroundActor != null) _renderer.AddActor(_backgroundActor);
                if (_borderVisibility == true &&_borderActor != null) _renderer.AddActor(_borderActor);
            }
        }
        public virtual void VisibilityOff()
        {
            if (_visibility == true)
            {
                _visibility = false;
                if (_borderActor != null) _renderer.RemoveActor(_borderActor);
                if (_backgroundActor != null) _renderer.RemoveActor(_backgroundActor);
            }
        }
        public void BorderVisibilityOn()
        {
            if (_borderVisibility == false)
            {
                _borderVisibility = true;
                if (_borderActor != null) _renderer.AddActor(_borderActor);
            }
        }
        public void BorderVisibilityOff()
        {
            if (_borderVisibility == true)
            {
                _borderVisibility = false;
                if (_borderActor != null) _renderer.RemoveActor(_borderActor);
            }
        }
        public virtual void BackgroundVisibilityOn()
        {
            if (_backgroundVisibility == false)
            {
                _backgroundVisibility = true;
                if (_borderVisibility == true) _renderer.RemoveActor(_borderActor); // remove border
                if (_backgroundActor != null) _renderer.AddActor(_backgroundActor);
                if (_borderVisibility == true) _renderer.AddActor(_borderActor); // add border back
            }
        }
        public virtual void BackgroundVisibilityOff()
        {
            if (_backgroundVisibility == true)
            {
                _backgroundVisibility = false;
                if (_backgroundActor != null) _renderer.RemoveActor(_backgroundActor);
            }
        }
        //
        public virtual void OnRenderWindowModified()
        {
            // triggered at window resized
            if (!_visibility) return;

            // set the widget position in relation to the windows borders
            if (_position != null && _prewWindowSize[0] != 0 && _prewWindowSize[0] != 0)
            {
                int[] windowSize = _renderer.GetSize();

                if (windowSize[0] != _prewWindowSize[0] || windowSize[1] != _prewWindowSize[1])
                {
                    if (_widgetPosition == vtkMaxWidgetPosition.HorizontallyRelative)
                    {
                        double center = (_position[0] + _size[0] / 2) / _prewWindowSize[0];
                        _position[0] = windowSize[0] * center - _size[0] / 2;
                    }
                    else
                    {
                        double dx = 0;
                        double dy = 0;

                        if (_quadrant == 2 || _quadrant == 3)
                        {
                            dx = windowSize[0] - _prewWindowSize[0];
                        }
                        if (_quadrant == 3 || _quadrant == 4)
                        {
                            dy = windowSize[1] - _prewWindowSize[1];
                        }

                        if (_scaleBySectors)
                        {
                            _position[0] += dx;
                            _position[1] += dy;

                            OnMovedOrSizeChanged();
                        }
                    }
                }
            }
        }
        public virtual void OnMovedOrSizeChanged()
        {
            // triggered by moving the widget
            if (_renderer == null) return;

            // if this function is called when the renderer is not yet loaded, return
            int[] size = _renderer.GetSize();
            if (size[0] == 0 && size[1] == 0) return;

            double[] max = new double[] { size[0] - _size[0], size[1] - _size[1] };

            if (_widgetPosition == vtkMaxWidgetPosition.FromTopLeft)
            {
                _position[1] = size[1] - _size[1] - _position[1];
            }

            // check if widget fits into window to prevent looping
            if (_size[0] < size[0] && _size[1] < size[1])
            {
                // prevent the widget from going off the window
                if (_position[0] < 0 || _position[0] > max[0] || _position[1] < 0 || _position[1] > max[1])
                {
                    if (_position[0] < 0) _position[0] = 0;
                    else if (_position[0] > max[0]) _position[0] = max[0];

                    if (_position[1] < 0) _position[1] = 0;
                    else if (_position[1] > max[1]) _position[1] = max[1];
                }
            }

            if (_widgetPosition == vtkMaxWidgetPosition.FromTopLeft)
            {
                _position[1] = size[1] - _size[1] - _position[1];
            }

            double[] center = new double[] { _position[0] + _size[0] / 2, _position[1] + _size[1] / 2 };
            
            // Determine the quadrant
            double[] rendererCenter = new double[] { size[0] * 0.5, size[1] * 0.5 }; // * 0.5 for the change to double
            if (center[0] < rendererCenter[0] && center[1] < rendererCenter[1]) _quadrant = 1;
            else if (center[0] > rendererCenter[0] && center[1] < rendererCenter[1]) _quadrant = 2;
            else if (center[0] > rendererCenter[0] && center[1] > rendererCenter[1]) _quadrant = 3;
            else if (center[0] < rendererCenter[0] && center[1] > rendererCenter[1]) _quadrant = 4;

            UpdateBorderGeometry();
        }
        public virtual void OnSizeChanged()
        {
            OnMovedOrSizeChanged();
        }
        //
        public virtual bool LeftButtonPress(MouseEventArgs e)
        {
            if (!_visibility) return false;
            //
            int[] size = _renderer.GetSize();
            double[] position = _position.ToArray();
            if (_widgetPosition == vtkMaxWidgetPosition.FromTopLeft)
            {
                position[1] = size[1] - _size[1] - position[1];
            }
            // Inside click
            if (e.Location.X >= position[0] && e.Location.X <= position[0] + _size[0] &&
                e.Location.Y >= position[1] && e.Location.Y <= position[1] + _size[1])
            {
                //System.Diagnostics.Debug.WriteLine(DateTime.Now.Millisecond);
                // Double click
                if (e.Clicks == 2)
                {
                    MouseDoubleClick?.Invoke(this, e);
                    return true;
                }
                // Single click
                else
                {
                    _clickPos = new int[] { e.Location.X, e.Location.Y };
                    //
                    //_borderActor.GetProperty().SetColor(1, 0, 0);
                    //
                    return true;
                }
            }
            // Outside click
            else
            {
                //_borderActor.GetProperty().SetColor(0, 0, 0);
                //
                return false;
            }
        }
        public bool LeftButtonRelease(int x, int y)
        {
            if (!_visibility) return false;
            //
            if (_clickPos != null)
            {                
                int[] size = _renderer.GetSize();
                double[] position = _position.ToArray();
                if (_widgetPosition == vtkMaxWidgetPosition.FromTopLeft)
                {
                    position[1] = size[1] - _size[1] - position[1];
                }
                // Inside release
                if (x >= position[0] && x <= position[0] + _size[0] && y >= position[1] && y <= position[1] + _size[1])
                {
                    _clickPos = null;
                    return true;
                }
                // Outside release
                else 
                {
                    _clickPos = null;
                    return true;
                }
            }
            //
            return false;
        }
        public virtual void MiddleButtonPress(int x, int y)
        {
            if (!_visibility) return;
        }
        public virtual void MiddleButtonRelease(int x, int y)
        {
            if (!_visibility) return;
        }
        public bool RightButtonPress(int x, int y)
        {
            if (!_visibility) return false;

            int[] size = _renderer.GetSize();
            double[] position = _position.ToArray();
            if (_widgetPosition == vtkMaxWidgetPosition.FromTopLeft)
            {
                position[1] = size[1] - _size[1] - position[1];
            }

            if (x >= position[0] && x <= position[0] + _size[0] &&
                y >= position[1] && y <= position[1] + _size[1])
            {
                // inside click
                return true;
            }
            else
                return false;
        }
        //
        public virtual bool MouseMove(int x, int y)
        {
            if (!_visibility) return false;
            //
            if (_clickPos != null)
            {
                int[] delta = new int[] { x - _clickPos[0], y - _clickPos[1] };
                _position[0] += delta[0];
                if (_widgetPosition == vtkMaxWidgetPosition.FromBottomLeft) _position[1] += delta[1];
                else if (_widgetPosition == vtkMaxWidgetPosition.FromTopLeft) _position[1] -= delta[1];
                else if (_widgetPosition == vtkMaxWidgetPosition.HorizontallyRelative) _position[1] += delta[1];
                else throw new NotSupportedException();
                //
                _clickPos[0] = x;
                _clickPos[1] = y;
                //
                UpdateBorderGeometry();
                OnMovedOrSizeChanged();
                //
                return true;
            }
            else return false;
        }
        public virtual void MousePan(int dx, int dy)
        {
            if (!_visibility) return;
        }
        public virtual void MouseRotate(double azimuthAngle, double ElevationAngle)
        {
            if (!_visibility) return;
        }
        public virtual void MouseWheelScrolled()
        {
            if (!_visibility) return;
        }
        public virtual void CameraModified()
        {
            if (!_visibility) return;
        }
        

        // Private methods                                                                                                          
        public virtual void UpdateBorderGeometry()
        {
            int[] size = _renderer.GetSize();
            double[] position = _position.ToArray();
            if (_widgetPosition == vtkMaxWidgetPosition.FromTopLeft)
            {
                position[1] = size[1] - _size[1] - position[1];
            }

            _positionCoordinate.SetValue(position[0], position[1], 0);

            vtkPoints points = _borderMapper.GetInput().GetPoints();
            points.SetPoint(0, position[0], position[1], 0);
            points.SetPoint(1, position[0] + _size[0], position[1], 0);
            points.SetPoint(2, position[0] + _size[0], position[1] + _size[1], 0);
            points.SetPoint(3, position[0], position[1] + _size[1], 0);
        }


        // Public setters                                                                                                           
        public virtual void SetInteractor(vtkRenderer renderer, vtkRenderWindowInteractor renderWindowInteractor)
        {
            // Dereference
            if (_renderWindowInteractor != null && _renderWindowInteractor.GetRenderWindow() != null)
                _renderWindowInteractor.GetRenderWindow().ModifiedEvt -= renderWindow_ModifiedEvt;
            // Get pointers
            if (renderWindowInteractor == null) return;
            _renderWindowInteractor = renderWindowInteractor;
            _renderer = renderer;
            // Reference
            _renderWindowInteractor.GetRenderWindow().ModifiedEvt += renderWindow_ModifiedEvt;
            // Add widget
            if (_renderWindowInteractor.GetInteractorStyle() is vtkInteractorStyleControl visc) visc.AddVtkMaxWidget(this);
            //
            _renderer.AddActor(_backgroundActor);
            _renderer.AddActor(_borderActor);
        }
        public virtual void RemoveInteractor()
        {
            // Dereference
            if (_renderWindowInteractor != null && _renderWindowInteractor.GetRenderWindow() != null)
                _renderWindowInteractor.GetRenderWindow().ModifiedEvt -= renderWindow_ModifiedEvt;
            // Remove widget
            if (_renderWindowInteractor.GetInteractorStyle() is vtkInteractorStyleControl visc) visc.RemoveVtkMaxWidget(this);
            //
            _renderer.RemoveActor(_backgroundActor);
            _renderer.RemoveActor(_borderActor);
            // Pointers
            _renderWindowInteractor = null;
            _renderer = null;
        }
        //
        public void SetPosition(double x, double y)
        {
            _widgetPosition = vtkMaxWidgetPosition.FromBottomLeft;
            //
            _position[0] = x;
            _position[1] = y;
            //
            OnMovedOrSizeChanged();
        }
        public void SetTopLeftPosition(int x, int y)
        {
            _widgetPosition = vtkMaxWidgetPosition.FromTopLeft;
            //
            _position[0] = x;
            _position[1] = y;
            //
            OnMovedOrSizeChanged();
        }
        public void SetHorizontallyRelativePosition(double x, double y)
        {
            _widgetPosition = vtkMaxWidgetPosition.HorizontallyRelative;
            //
            _position[0] = x;
            _position[1] = y;
            //
            OnMovedOrSizeChanged();
        }
        public void SetNormalizedPosition(double x, double y)
        {
            double[] position = new double[] { x, y };
            _renderer.NormalizedDisplayToDisplay(ref position[0], ref position[1]);
            SetPosition(position[0], position[1]);
        }
        public void SetBorderColor(double r, double g, double b)
        {
            _borderActor.GetProperty().SetColor(r, g, b);
        }
        public void SetBackgroundColor(double r, double g, double b)
        {
            _backgroundActor.GetProperty().SetColor(r, g, b);
        }


        // Public getters                                                                                                           
        public int GetVisibility()
        {
            if (_visibility) return 1;
            else return 0;
        }
        public int GetBorderVisibility()
        {
            if (_borderVisibility) return 1;
            else return 0;
        }
        public int GetBackgroundVisibility()
        {
            if (_backgroundVisibility) return 1;
            else return 0;
        }
        //
        public CaeMesh.BoundingBox GetBoundingBox()
        {
            CaeMesh.BoundingBox box = new CaeMesh.BoundingBox();
            box.MinX = _position[0];
            box.MaxX = _position[0] + _size[0];
            box.MinY = _position[1];
            box.MaxY = _position[1] + _size[1];
            box.MinZ = 0;
            box.MaxZ = 1;
            return box;
        }       
        public int GetWidth()
        {
            return (int)Math.Round(_size[0]);
        }
        public int GetHeight()
        {
            return (int)Math.Round(_size[1]);
        }
        public Rectangle GetRectangle()
        {
            Point location = new Point((int)Math.Round(_position[0]), (int)Math.Round(_position[1]));
            Size size = new Size((int)Math.Round(_size[0]), (int)Math.Round(_size[1]));
            return new Rectangle(location, size);
        }

        public vtkProperty2D GetBorderProperty()
        {
            return _borderActor.GetProperty();
        }
        public vtkProperty2D GetBackgroundProperty()
        {
            return _backgroundActor.GetProperty();
        }



    }
}
