using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeGlobals;
using Kitware.VTK;
using static Kitware.VTK.vtkMultiThreshold;

namespace vtkControl
{
    class vtkMaxStatusBlockWidget : vtkMaxTextWidget
    {
        // Variables                                                                                                                
        private string _text;
        private string _name;
        private DateTime _dateTime;
        private float _analysisTime;
        private string _analysisTimeUnit;
        private float _animationScaleFactor;
        private float _deformationScaleFactor;
        private string _deformationVariable;
        private vtkMaxFieldDataType _fieldType;
        private vtkMaxAnimationType _animationType;
        private int _stepNumber;
        private int _incrementNumber;
        private bool _deformationScaleFactorTextClicked;
        private System.Drawing.Rectangle _deformationScaleFactorRect;
        private StringAngleDegConverter _stringAngleDegConverter;


        // Properties                                                                                                               
        public string Name { get { return _name; } set { _name = value; SetText(); } }
        public DateTime DateTime { get { return _dateTime; } set { _dateTime = value; SetText(); } }
        public float AnalysisTime { get { return _analysisTime; } set { _analysisTime = value; SetText(); } }
        public string AnalysisTimeUnit { get { return _analysisTimeUnit; } set { _analysisTimeUnit = value; SetText(); } }
        public float DeformationScaleFactor
        {
            get { return _deformationScaleFactor; }
            set { _deformationScaleFactor = value; SetText(); }
        }
        public string DeformationVariable { get { return _deformationVariable; } set { _deformationVariable = value; SetText(); } }
        public float AnimationScaleFactor
        { 
            get { return _animationScaleFactor; } 
            set { _animationScaleFactor = value; SetText(); } 
        }
        public vtkMaxFieldDataType FieldType { get { return _fieldType; } set { _fieldType = value; SetText(); } }
        public vtkMaxAnimationType AnimationType { get { return _animationType; } set { _animationType = value; SetText(); } }
        public int StepNumber { get { return _stepNumber; } set { _stepNumber = value; SetText(); } }
        public int IncrementNumber { get { return _incrementNumber; } set { _incrementNumber = value; SetText(); } }
        public bool DeformationScaleFactorTextClicked
        {
            get { return _deformationScaleFactorTextClicked; }
            set { _deformationScaleFactorTextClicked = value; }
        }


        // Constructors                                                                                                             
        public vtkMaxStatusBlockWidget()
        {
            _text = "text";
            _name = "name";
            _dateTime = DateTime.Now;
            _analysisTime = 0;
            _analysisTimeUnit = "";
            _animationScaleFactor = -1;
            _deformationScaleFactor = 1;
            _fieldType = vtkMaxFieldDataType.Static;
            _animationType = vtkMaxAnimationType.ScaleFactor;
            _stringAngleDegConverter = new StringAngleDegConverter();
        }


        // Private methods                                                                                                          
        private void SetText()
        {
            string sysUIFormat = System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
            //
            string line1 = null;
            string line2 = null;
            string line3fix = null;
            string line3factor = null;
            string line4 = null;
            //
            line1 = "Name: " + _name + "   ";
            line1 += "Date: " + _dateTime.ToString(sysUIFormat) + "   Time: " + _dateTime.ToString("HH:mm:ss");
            //
            if (_fieldType == vtkMaxFieldDataType.Static)
            {
                line2 = "Step: #" + _stepNumber + "   Increment: #" + _incrementNumber +
                        "   Analysis time: " + _analysisTime.ToString() + " " + _analysisTimeUnit;
            }
            else if (_fieldType == vtkMaxFieldDataType.Frequency)
            {
                line2 = "Step: #" + _stepNumber + "   Mode: #" + _incrementNumber + "   Frequency: " +
                        _analysisTime.ToString() + " " + _analysisTimeUnit;
            }
            else if (_fieldType == vtkMaxFieldDataType.FrequencySensitivity)
            {
                // subtract 1 since first increment contains only normals
                line2 = "Step: #" + _stepNumber + "   Mode: #" + _incrementNumber + "   Frequency: " +
                        _analysisTime.ToString() + " " + _analysisTimeUnit;
            }
            else if (_fieldType == vtkMaxFieldDataType.Buckling)
            {
                line2 = "Step: #" + _stepNumber + "   Buckling factor: " + _analysisTime.ToString();
            }
            else if (_fieldType == vtkMaxFieldDataType.SteadyStateDynamic)
            {
                line2 = "Step: #" + _stepNumber + "   Data point: #" + _incrementNumber + "   Frequency: " +
                        _analysisTime.ToString() + " " + _analysisTimeUnit;
            }
            else if (_fieldType == vtkMaxFieldDataType.LastIterations)
            {
                line2 = "Increment: #" + _stepNumber + "   Iteration: #" + _incrementNumber +
                        "   Analysis time: " + _analysisTime.ToString() + " " + _analysisTimeUnit;
            }
            else throw new NotSupportedException();
            //
            line3fix = "Deformation variable: " + _deformationVariable + "   " + "Deformation scale factor: ";
            line3factor = _deformationScaleFactor.ToString();
            //
            if (_animationScaleFactor >= 0)
            {
                if (_animationType == vtkMaxAnimationType.ScaleFactor)
                {
                    line4 = "Animation scale factor: " + _animationScaleFactor.ToString();
                }
                else if (_animationType == vtkMaxAnimationType.TimeIncrements) { }
                else if (_animationType == vtkMaxAnimationType.Harmonic)
                {
                    line4 = "Complex angle: " + _stringAngleDegConverter.ConvertToString(_animationScaleFactor);
                }
            }
            //
            _text = line1 + Environment.NewLine + line2 + Environment.NewLine + line3fix + line3factor;
            if (line4 != null) _text += Environment.NewLine + line4;
            // Compute the position and size of the deformation scale factor text
            this.SetText(line1);
            double[] line1Size = _size.ToArray();
            this.SetText(line1 + Environment.NewLine + line2);
            double[] line1And2Size = _size.ToArray();
            this.SetText(line1 + Environment.NewLine + line2 + Environment.NewLine + line3fix);
            double[] line1And2And3Size = _size.ToArray();
            this.SetText(line3fix);
            double[] line3fixSize = _size.ToArray();
            this.SetText(line3fix + line3factor);
            double[] line3fixAndFactorSize = _size.ToArray();
            //
            _deformationScaleFactorRect = new System.Drawing.Rectangle((int)line3fixSize[0],
                                                                       (int)line1And2Size[1],
                                                                       (int)(line3fixAndFactorSize[0] - line3fixSize[0]),
                                                                       (int)(line1And2And3Size[1] - line1And2Size[1]));
            //
            this.SetText(_text);
        }


        // Public methods                                                                                                           
        public override bool LeftButtonPress(MouseEventArgs e)
        {
            int[] size = _renderer.GetSize();
            int localX = e.Location.X - (int)_position[0];
            int loaclY = size[1] - e.Location.Y - (int)_position[1];
            //
            _deformationScaleFactorTextClicked = _deformationScaleFactorRect.Contains(localX, loaclY);
            //
            return base.LeftButtonPress(e);
        }
    }
}
