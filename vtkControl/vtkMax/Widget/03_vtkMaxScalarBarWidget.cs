using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;


namespace vtkControl
{
    class vtkMaxScalarBarWidget : vtkMaxTextWidget
    {
        // Variables                                                                                                                
        protected vtkPolyDataMapper2D _scalarBarBorderMapper;
        protected vtkActor2D _scalarBarBorderActor;
        //
        protected vtkPolyDataMapper2D _scalarBarColorsMapper;
        protected vtkActor2D _scalarBarColorsActor;
        //
        protected vtkLookupTable _lookupTable;
        //
        protected vtkActor2D _textActorFooter;
        protected vtkTextMapper _textMapperFooter;
        //
        protected vtkActor2D _textActorLabel;
        protected vtkTextMapper _textMapperLabel;
        //
        private int _numberOfColors;
        private string[] _labels;
        private string _labelFormat;
        private bool _addMinColor;
        private bool _addMaxColor;
        private double _minUserValue;
        private double _maxUserValue;
        private System.Drawing.Color _minColor;
        private System.Drawing.Color _maxColor;
        private string _unitAbbreviation;


        // Properties                                                                                                               
        public System.Drawing.Color MinColor { get { return _minColor; } set { _minColor = value; } }
        public System.Drawing.Color MaxColor { get { return _maxColor; } set { _maxColor = value; } }
        public string UnitAbbreviation { get { return _unitAbbreviation; } }


        // Constructors                                                                                                             
        public vtkMaxScalarBarWidget()
        {
            _backgroundVisibility = false;
            _borderVisibility = false;
            //
            SetBorderColor(0, 0, 0);
            //
            InitializeFooter();
            InitializeLabels();
            InitializeBar();
            //
            _numberOfColors = 12;
            _labelFormat = "G3";
            _lookupTable = vtkLookupTable.New();
            _lookupTable.SetTableRange(-5.5, 11);
            //
            _addMinColor = false;
            _addMaxColor = false;
            //
            _unitAbbreviation = "";
        }


        // Private methods                                                                                                          
        private void InitializeFooter()
        {
            // Footer text                                                                              
            _textMapperFooter = vtkTextMapper.New();
            _textMapperFooter.SetTextProperty(_textMapper.GetTextProperty());

            // Actor
            _textActorFooter = vtkActor2D.New();
            _textActorFooter.SetMapper(_textMapperFooter);

            // Set relative text position
            _textActorFooter.GetPositionCoordinate().SetCoordinateSystemToDisplay();  // set offsets in pixels
            _textActorFooter.GetPositionCoordinate().SetValue(_padding, _padding);
            _textActorFooter.GetPositionCoordinate().SetReferenceCoordinate(_positionCoordinate);
        }
        private void InitializeLabels()
        {
            // Bar label                                                                                
            _textMapperLabel = vtkTextMapper.New();
            _textMapperLabel.SetTextProperty(_textMapper.GetTextProperty());
            //_textMapperLabel.GetTextProperty().SetJustificationToRight();

            // Actor
            _textActorLabel = vtkActor2D.New();
            _textActorLabel.SetMapper(_textMapperLabel);

            // Set relative text position
            _textActorLabel.GetPositionCoordinate().SetCoordinateSystemToDisplay();  // set offsets in pixels
            _textActorLabel.GetPositionCoordinate().SetReferenceCoordinate(_positionCoordinate);
        }
        private void InitializeBar()
        {
            // Bar border                                                                          
            vtkPolyData scalarBarPolyData = vtkPolyData.New();
            scalarBarPolyData.SetPoints(vtkPoints.New());
            scalarBarPolyData.SetPolys(vtkCellArray.New());

            _scalarBarBorderMapper = vtkPolyDataMapper2D.New();
            _scalarBarBorderMapper.SetInput(scalarBarPolyData);

            vtkProperty2D scalarBarProperty = vtkProperty2D.New();
            scalarBarProperty.SetColor(0, 0, 0);

            _scalarBarBorderActor = vtkActor2D.New();
            _scalarBarBorderActor.SetMapper(_scalarBarBorderMapper);
            _scalarBarBorderActor.SetProperty(scalarBarProperty);

            _scalarBarBorderActor.GetPositionCoordinate().SetCoordinateSystemToDisplay();
            _scalarBarBorderActor.GetPositionCoordinate().SetReferenceCoordinate(_positionCoordinate);


            // Bar filled                                                                          
            scalarBarPolyData = vtkPolyData.New();
            scalarBarPolyData.SetPoints(vtkPoints.New());
            scalarBarPolyData.SetPolys(vtkCellArray.New());

            _scalarBarColorsMapper = vtkPolyDataMapper2D.New();
            _scalarBarColorsMapper.SetInput(scalarBarPolyData);

            _scalarBarColorsActor = vtkActor2D.New();
            _scalarBarColorsActor.SetMapper(_scalarBarColorsMapper);
            _scalarBarColorsActor.SetProperty(scalarBarProperty);

            _scalarBarColorsActor.GetPositionCoordinate().SetCoordinateSystemToDisplay();
            _scalarBarColorsActor.GetPositionCoordinate().SetReferenceCoordinate(_positionCoordinate);
        }
        //
        private void GenerateGeometry()
        {
            // Geometry
            double boxAspectRatio = 3.0 / 4.0;

            string tmp = _textMapperLabel.GetInput();
            _textMapperLabel.SetInput("1");
            int[] sizeOfOne = vtkMaxWidgetTools.GetTextSize(_textMapperLabel, _renderer);
            _textMapperLabel.SetInput("1" + Environment.NewLine + "1");
            int[] sizeOfTwo = vtkMaxWidgetTools.GetTextSize(_textMapperLabel, _renderer);
            _textMapperLabel.SetInput(tmp);

            double lineSpacing = ((double)sizeOfTwo[1] - 2 * sizeOfOne[1]);
            double boxHeight = sizeOfOne[1] + lineSpacing;
            double boxWidth = boxHeight / boxAspectRatio;
            double spacing = boxHeight * 0.5;
            double verticalLineLength = boxWidth + spacing;
            double lineOffset = _textMapperLabel.GetTextProperty().GetLineOffset();
            // Footer
            double[] size1 = GenerateFooter(_padding, _padding);
            size1[1] += spacing;

            // Labels
            double[] size2 = GenerateLabels(_padding + verticalLineLength + spacing, size1[1]);
            size2[1] += spacing;

            // Bar lines
            double offsetY = -lineOffset + 2.0 / 7 * sizeOfOne[1];
            GenerateBarBorders(_padding, offsetY + size1[1], boxWidth, boxHeight, verticalLineLength);
            GenerateBarColors(_padding, offsetY + size1[1], boxWidth, boxHeight, verticalLineLength);

            // Text
            double[] size3 = GenerateText(_padding, size2[1]);

            double maxX = size1[0];
            if (size2[0] > maxX) maxX = size2[0];
            if (size3[0] > maxX) maxX = size3[0];

            _size[0] = maxX + _padding;
            _size[1] = size3[1] + _padding;
        }
        private double[] GenerateFooter(double offsetX, double offsetY)
        {
            _textActorFooter.GetPositionCoordinate().SetValue(offsetX, offsetY);

            int[] sizeFooter = vtkMaxWidgetTools.GetTextSize(_textMapperFooter, _renderer);
            return new double[] { offsetX + sizeFooter[0], offsetY + sizeFooter[1] };
        }
        private double[] GenerateLabels(double offsetX, double offsetY)
        {
            int _numberOfLabels = _numberOfColors + 1;

            int _numberOfAllLabels = _numberOfLabels;
            if (_addMinColor) _numberOfAllLabels++;
            if (_addMaxColor) _numberOfAllLabels++;

            double[] minMaxRange = _lookupTable.GetTableRange();    // min, max
            double[] labelsRange = new double[] { minMaxRange[1], minMaxRange[0] };

            if (_addMaxColor) labelsRange[0] = _maxUserValue;
            if (_addMinColor) labelsRange[1] = _minUserValue;

            int labelCount = 0;
            _labels = new string[_numberOfAllLabels];

            // above range color
            if (_addMaxColor)
            {
                if (minMaxRange[1] <= _maxUserValue) _labels[labelCount++] = GetString(_maxUserValue);
                else _labels[labelCount++] = GetString(minMaxRange[1]);
            }

            // between range color
            double delta = (labelsRange[1] - labelsRange[0]) / (_numberOfLabels - 1);
            for (int i = 0; i < _numberOfLabels - 1; i++) _labels[labelCount++] = GetString(labelsRange[0] + delta * i);
            _labels[labelCount++] = GetString(labelsRange[1]);

            // below range color
            if (_addMinColor)
            {
                if (minMaxRange[0] >= _minUserValue) _labels[labelCount++] = GetString(_minUserValue);
                else _labels[labelCount++] = GetString(minMaxRange[0]);
            }

            string label = "";
            for (int i = 0; i < _labels.Length; i++)
            {
                if (i != 0) label += Environment.NewLine;
                label += _labels[i];
            }
            _textMapperLabel.SetInput(label);

            int[] size = vtkMaxWidgetTools.GetTextSize(_textMapperLabel, _renderer);
            _textActorLabel.GetPositionCoordinate().SetValue(offsetX, offsetY);

            return new double[] { offsetX + size[0], offsetY + size[1] };
        }
        private double[] GenerateLabels_HideUserColors(double offsetX, double offsetY)
        {
            int _numberOfLabels = _numberOfColors + 1;

            int _numberOfAllLabels = _numberOfLabels;
            if (_addMinColor) _numberOfAllLabels++;
            if (_addMaxColor) _numberOfAllLabels++;

            double[] minMaxRange = _lookupTable.GetTableRange();    // min, max
            double[] labelsRange = new double[] { minMaxRange[1], minMaxRange[0] };

            if (_addMaxColor) labelsRange[0] = _maxUserValue;
            if (_addMinColor) labelsRange[1] = _minUserValue;

            int labelCount = 0;
            _labels = new string[_numberOfAllLabels];

            // above range color
            if (_addMaxColor) _labels[labelCount++] = GetString(minMaxRange[1]);

            // between range color
            double delta = (labelsRange[1] - labelsRange[0]) / (_numberOfLabels - 1);
            for (int i = 0; i < _numberOfLabels; i++) _labels[labelCount++] = GetString(labelsRange[0] + delta * i);

            // below range color
            if (_addMinColor) _labels[labelCount++] = GetString(minMaxRange[0]);

            string label = "";
            for (int i = 0; i < _labels.Length; i++)
            {
                if (i != 0) label += Environment.NewLine;
                label += _labels[i];
            }
            _textMapperLabel.SetInput(label);

            int[] size = vtkMaxWidgetTools.GetTextSize(_textMapperLabel, _renderer);
            _textActorLabel.GetPositionCoordinate().SetValue(offsetX, offsetY);

            return new double[] { offsetX + size[0], offsetY + size[1] };
        }
        //
        private string GetString(double value)
        {
            string result = value.ToString(_labelFormat);
            if (value >= 0) result = "+" + result;
            if (value < 0) result = result.Insert(1, " ");
            return result;
        }
        private void GenerateBarBorders(double offsetX, double offsetY, double boxWidth, double boxHeight, double verticalLineLength)
        {
            vtkPoints scalarBarPoints = vtkPoints.New();
            vtkCellArray scalarBarLines = vtkCellArray.New();

            int numOfTableColors = (int)_lookupTable.GetNumberOfColors();

            scalarBarPoints.SetNumberOfPoints(2 * (numOfTableColors + 1) + 2);
            scalarBarLines.SetNumberOfCells((numOfTableColors + 1) + 2);

            double h = 0;

            for (int i = 0; i <= numOfTableColors; i++)
            {
                h = i * boxHeight;
                scalarBarPoints.SetPoint(2 * i, 0, h, 0);
                scalarBarPoints.SetPoint(2 * i + 1, verticalLineLength, h, 0);

                scalarBarLines.InsertNextCell(2);
                scalarBarLines.InsertCellPoint(2 * i);
                scalarBarLines.InsertCellPoint(2 * i + 1);
            }

            scalarBarPoints.SetPoint(2 * (numOfTableColors + 1), boxWidth, 0, 0);
            scalarBarPoints.SetPoint(2 * (numOfTableColors + 1) + 1, boxWidth, h, 0);

            scalarBarLines.InsertNextCell(2);
            scalarBarLines.InsertCellPoint(0);
            scalarBarLines.InsertCellPoint(2 * numOfTableColors);

            scalarBarLines.InsertNextCell(2);
            scalarBarLines.InsertCellPoint(2 * (numOfTableColors + 1));
            scalarBarLines.InsertCellPoint(2 * (numOfTableColors + 1) + 1);

            vtkPolyData scalarBarPoly = _scalarBarBorderMapper.GetInput();
            scalarBarPoly.SetPoints(scalarBarPoints);
            scalarBarPoly.SetLines(scalarBarLines);

            _scalarBarBorderActor.GetPositionCoordinate().SetValue(offsetX, offsetY);
        }
        private void GenerateBarColors(double offsetX, double offsetY, double boxWidth, double boxHeight, double verticalLineLength)
        {
            vtkPoints scalarBarPoints = vtkPoints.New();
            vtkCellArray scalarBarPolygons = vtkCellArray.New();
            vtkFloatArray scalars = vtkFloatArray.New();

            int numOfTableColors = (int)_lookupTable.GetNumberOfColors();

            scalarBarPoints.SetNumberOfPoints(4 * numOfTableColors);
            scalarBarPolygons.SetNumberOfCells(numOfTableColors);
            scalars.SetNumberOfValues(4 * numOfTableColors);

            double h;
            float value;

            for (int i = 0; i < numOfTableColors; i++)
            {
                h = i * boxHeight;
                scalarBarPoints.SetPoint(4 * i + 0, 0, h, 0);
                scalarBarPoints.SetPoint(4 * i + 1, boxWidth, h, 0);

                h += boxHeight;
                scalarBarPoints.SetPoint(4 * i + 2, boxWidth, h, 0);
                scalarBarPoints.SetPoint(4 * i + 3, 0, h, 0);


                scalarBarPolygons.InsertNextCell(4);
                scalarBarPolygons.InsertCellPoint(4 * i + 0);
                scalarBarPolygons.InsertCellPoint(4 * i + 1);
                scalarBarPolygons.InsertCellPoint(4 * i + 2);
                scalarBarPolygons.InsertCellPoint(4 * i + 3);

                value = 1f / (numOfTableColors - 1) * i;
                scalars.SetValue(4 * i + 0, value);
                scalars.SetValue(4 * i + 1, value);
                scalars.SetValue(4 * i + 2, value);
                scalars.SetValue(4 * i + 3, value);
            }

            vtkPolyData scalarBarPoly = _scalarBarColorsMapper.GetInput();
            scalarBarPoly.SetPoints(scalarBarPoints);
            scalarBarPoly.SetPolys(scalarBarPolygons);

            // Set scalars
            _scalarBarColorsMapper.GetInput().GetPointData().SetScalars(scalars);

            // Edit actors mapper
            _scalarBarColorsMapper.SetScalarRange(0, 1);
            _scalarBarColorsMapper.SetLookupTable(_lookupTable);

            _scalarBarColorsActor.GetPositionCoordinate().SetValue(offsetX, offsetY);
        }
        private double[] GenerateText(double offsetX, double offsetY)
        {
            _textActor.GetPositionCoordinate().SetValue(offsetX, offsetY);

            int[] sizeText = vtkMaxWidgetTools.GetTextSize(_textMapper, _renderer);
            return new double[] { offsetX + sizeText[0], offsetY + sizeText[1] };
        }


        // Public methods                                                                                                           
        public override void OnSizeChanged()
        {
            GenerateGeometry();
            OnMovedOrSizeChanged();
        }
        public override void VisibilityOn()
        {
            if (_visibility == false)
            {
                base.VisibilityOn();

                if (_textActorFooter != null) _renderer.AddActor(_textActorFooter);
                if (_textActorLabel != null) _renderer.AddActor(_textActorLabel);
                if (_scalarBarColorsActor != null) _renderer.AddActor(_scalarBarColorsActor);
                if (_scalarBarBorderActor != null) _renderer.AddActor(_scalarBarBorderActor);
            }
        }
        public override void VisibilityOff()
        {
            if (_visibility == true)
            {
                base.VisibilityOff();

                if (_textActorFooter != null) _renderer.RemoveActor(_textActorFooter);
                if (_textActorLabel != null) _renderer.RemoveActor(_textActorLabel);
                if (_scalarBarBorderActor != null) _renderer.RemoveActor(_scalarBarBorderActor);
                if (_scalarBarColorsActor != null) _renderer.RemoveActor(_scalarBarColorsActor);
            }
        }
        public void SetText(string fieldName, string componentName, string unitAbbreviation, string complexComponent,
                            string minMaxType)
        {
            fieldName = fieldName.Replace('_', ' ').Replace('-', ' ');
            componentName = componentName.Replace('_', ' ').Replace('-', ' ');
            //
            _unitAbbreviation = unitAbbreviation;
            string complexLabel;
            if (complexComponent == null || complexComponent.Length == 0) complexLabel = "";
            else complexLabel = "Complex: " + complexComponent + Environment.NewLine;
            //
            string text = complexLabel + 
                          fieldName + ": " + componentName + Environment.NewLine +
                          "Unit: " + _unitAbbreviation + Environment.NewLine +
                          minMaxType;
            //
            base.SetText(text);
            OnSizeChanged();
        }
        

        // Public setters                                                                                                           
        public override void SetInteractor(vtkRenderer renderer, vtkRenderWindowInteractor renderWindowInteractor)
        {
            base.SetInteractor(renderer, renderWindowInteractor);
            _renderer.AddActor(_textActorFooter);
            _renderer.AddActor(_textActorLabel);
            _renderer.AddActor(_scalarBarColorsActor);
            _renderer.AddActor(_scalarBarBorderActor);
        }
        public override void RemoveInteractor()
        {
            _renderer.RemoveActor(_textActorFooter);
            _renderer.RemoveActor(_textActorLabel);
            _renderer.RemoveActor(_scalarBarColorsActor);
            _renderer.RemoveActor(_scalarBarBorderActor);
            //
            base.RemoveInteractor();
        }
        public void SetNumberOfColors(int numOfColors)
        {
            if (_numberOfColors != numOfColors)
            {
                _numberOfColors = numOfColors;
                OnSizeChanged();
            }
        }
        public void SetLabelFormat(string labelFormat)
        {
            if (_labelFormat != labelFormat)
            {
                _labelFormat = labelFormat;
                OnSizeChanged();
            }
        }
        //
        public void CreateLookupTable(vtkColorTransferFunction ctf, double scalarRangeMin, double scalarRangeMax)
        {
            CreateLookupTable(ctf, scalarRangeMin, scalarRangeMax, double.NaN, double.NaN);
        }
        public void CreateLookupTable(vtkColorTransferFunction ctf, double scalarRangeMin, double scalarRangeMax, double minUserValue, double maxUserValue)
        {
            double delta;
            double[] color;
            _lookupTable = vtkLookupTable.New();

            double min = scalarRangeMin;
            double max = scalarRangeMax;
            _minUserValue = minUserValue;
            _maxUserValue = maxUserValue;
            _addMinColor = false;
            _addMaxColor = false;

            if (!double.IsNaN(minUserValue) && !double.IsNaN(maxUserValue))
            {
                min = Math.Min(scalarRangeMin, minUserValue);
                max = Math.Max(scalarRangeMax, maxUserValue);

                _addMinColor = true;
                _addMaxColor = true;
            }
            _lookupTable.SetTableRange(min, max);

            if (_addMinColor || _addMaxColor)
            {
                int colorCount = 0;
                int numAllColors = _numberOfColors;
                if (_addMinColor) numAllColors++;
                if (_addMaxColor) numAllColors++;

                _lookupTable.SetNumberOfColors(numAllColors);

                // below range color
                if (_addMinColor)
                {
                    color = new double[] { _minColor.R / 256.0, _minColor.G / 256.0, _minColor.B / 256.0 };
                    _lookupTable.SetTableValue(colorCount++, color[0], color[1], color[2], 1.0); //R,G,B,A
                }

                // between range color
                delta = 1.0 / (_numberOfColors - 1);
                for (int i = 0; i < _numberOfColors; i++)
                {
                    color = ctf.GetColor(i * delta);
                    _lookupTable.SetTableValue(colorCount++, color[0], color[1], color[2], 1.0); //R,G,B,A
                }

                // above range color
                if (_addMaxColor)
                {
                    color = new double[] { _maxColor.R / 256.0, _maxColor.G / 256.0, _maxColor.B / 256.0 };
                    _lookupTable.SetTableValue(colorCount++, color[0], color[1], color[2], 1.0); //R,G,B,A
                }
            }
            else
            {
                _lookupTable.SetNumberOfColors(_numberOfColors);
                delta = 1.0 / (_lookupTable.GetNumberOfColors() - 1);
                for (int i = 0; i < _lookupTable.GetNumberOfColors(); i++)
                {
                    color = ctf.GetColor(i * delta);
                    _lookupTable.SetTableValue(i, color[0], color[1], color[2], 1.0); //R,G,B,A
                }
            }

            OnSizeChanged();
        }
        public void CreateLookupTable_HideUserColors(vtkColorTransferFunction ctf, double scalarRangeMin, double scalarRangeMax, double minUserValue, double maxUserValue)
        {
            double delta;
            double[] color;
            _lookupTable = vtkLookupTable.New();

            double min = scalarRangeMin;
            double max = scalarRangeMax;
            _minUserValue = minUserValue;
            _maxUserValue = maxUserValue;
            _addMinColor = false;
            _addMaxColor = false;

            if (!double.IsNaN(minUserValue) && !double.IsNaN(maxUserValue))
            {
                min = Math.Min(scalarRangeMin, minUserValue);
                max = Math.Max(scalarRangeMax, maxUserValue);

                if (minUserValue > min && max != min) _addMinColor = true;
                if (maxUserValue < max && max != min) _addMaxColor = true;
            }
            _lookupTable.SetTableRange(min, max);

            if (_addMinColor || _addMaxColor)
            {
                int colorCount = 0;
                int numAllColors = _numberOfColors;
                if (_addMinColor) numAllColors++;
                if (_addMaxColor) numAllColors++;

                _lookupTable.SetNumberOfColors(numAllColors);

                // below range color
                if (_addMinColor)
                {
                    color = new double[] { _minColor.R / 256.0, _minColor.G / 256.0, _minColor.B / 256.0 };
                    _lookupTable.SetTableValue(colorCount++, color[0], color[1], color[2], 1.0); //R,G,B,A
                }

                // between range color
                delta = 1.0 / (_numberOfColors - 1);
                for (int i = 0; i < _numberOfColors; i++)
                {
                    color = ctf.GetColor(i * delta);
                    _lookupTable.SetTableValue(colorCount++, color[0], color[1], color[2], 1.0); //R,G,B,A
                }

                // above range color
                if (_addMaxColor)
                {
                    color = new double[] { _maxColor.R / 256.0, _maxColor.G / 256.0, _maxColor.B / 256.0 };
                    _lookupTable.SetTableValue(colorCount++, color[0], color[1], color[2], 1.0); //R,G,B,A
                }
            }
            else
            {
                _lookupTable.SetNumberOfColors(_numberOfColors);
                delta = 1.0 / (_lookupTable.GetNumberOfColors() - 1);
                for (int i = 0; i < _lookupTable.GetNumberOfColors(); i++)
                {
                    color = ctf.GetColor(i * delta);
                    _lookupTable.SetTableValue(i, color[0], color[1], color[2], 1.0); //R,G,B,A
                }
            }

            OnSizeChanged();
        }
        public override void SetTextProperty(vtkTextProperty textProperty)
        {
            base.SetTextProperty(textProperty);

            _textMapperFooter.SetTextProperty(textProperty);
            _textMapperLabel.SetTextProperty(textProperty);
        }


        // Public getters                                                                                                           
        public string GetLabelFormat()
        {
            return _labelFormat;
        }        
    }
}
