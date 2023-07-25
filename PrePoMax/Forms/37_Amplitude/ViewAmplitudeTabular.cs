using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ViewAmplitudeTabular : ViewAmplitude
    {
        // Variables                                                                                                                
        private AmplitudeTabular _amplitudeTabular;
        private List<AmplitudeDataPoint> _points;


        // Properties                                                                                                               
        public override string Name { get { return _amplitudeTabular.Name; } set { _amplitudeTabular.Name = value; } }
        public override AmplitudeTimeSpanEnum TimeSpan
        {
            get { return _amplitudeTabular.TimeSpan; }
            set { _amplitudeTabular.TimeSpan = value; }
        }
        public override double ShiftX { get { return _amplitudeTabular.ShiftX; } set { _amplitudeTabular.ShiftX = value; } }
        public override double ShiftY { get { return _amplitudeTabular.ShiftY; } set { _amplitudeTabular.ShiftY = value; } }

        [Browsable(false)]
        public override Amplitude Base
        {
            get
            {
                int i = 0;
                double[][] timeAmplitude = new double[_points.Count][];
                //
                foreach (AmplitudeDataPoint point in _points)
                {
                    timeAmplitude[i] = new double[2];
                    timeAmplitude[i][0] = point.Time;
                    timeAmplitude[i][1] = point.Amplitude;
                    i++;
                }
                _amplitudeTabular.TimeAmplitude = timeAmplitude;
                //
                return _amplitudeTabular;
            }
        }
        //
        [Browsable(false)]
        public List<AmplitudeDataPoint> DataPoints { get { return _points; } set { _points = value; } }


        // Constructors                                                                                                             
        public ViewAmplitudeTabular(AmplitudeTabular amplitudeTabular)
        {
            _amplitudeTabular = amplitudeTabular;
            //
            _points = new List<AmplitudeDataPoint>();
            for (int i = 0; i < amplitudeTabular.TimeAmplitude.Length; i++)
            {
                _points.Add(new AmplitudeDataPoint(amplitudeTabular.TimeAmplitude[i][0],
                                                   amplitudeTabular.TimeAmplitude[i][1]));
            }
            //
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
    }
}
