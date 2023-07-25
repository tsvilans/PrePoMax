using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using System.Runtime.Serialization;

namespace CaeModel
{
    [Serializable]
    public class EngineeringConstants : MaterialProperty
    {
        // Variables                                                                                                                
        private double[] _youngsModuli;
        private double[] _shearModuli;
        private double[] _poissonsRatios;


        // Properties                                                                                                               
        public double[] YoungsModuli
        {
            get { return _youngsModuli; }
            set { if (value[0] > 0 && value[1] > 0 && value[2] > 0) _youngsModuli = value; else throw new CaeException(_positive); }
        }
        public double[] ShearModuli
        {
            get { return _shearModuli; }
            set { if (value[0] > 0 && value[1] > 0 && value[2] > 0) _shearModuli = value; else throw new CaeException(_positive); }
        }
        public double[] PoissonsRatios
        {
            get { return _poissonsRatios; }
            set { if (value[0] < 1 && value[1] < 1 && value[2] < 1) _poissonsRatios = value; else throw new CaeException("Poissons ratios must be between -1 and 0.5."); }
        }

        // Constructors                                                                                                             
        public EngineeringConstants(double[] youngsModuli, double[] poissonsRatios, double[] shearModuli)
        {
            // The constructor must wotk with E = 0
            _youngsModuli = youngsModuli;
            ShearModuli = shearModuli;
            // Use the method to perform any checks necessary
            PoissonsRatios = poissonsRatios;
        }


        // Methods                                                                                                                  
    }
}
