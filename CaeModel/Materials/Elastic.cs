using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using System.Runtime.Serialization;
using CaeResults;
using FileInOut.Output.Calculix;
using System.Xml.Linq;

namespace CaeModel
{
    [Serializable]
    public class Elastic : MaterialProperty
    {
        // Variables                                                                                                                
        private double[][] _youngsPoissonsTemp;
        

        // Properties                                                                                                               
        public double[][] YoungsPoissonsTemp
        {
            get { return _youngsPoissonsTemp; }
            set
            {
                _youngsPoissonsTemp = value;
                if (_youngsPoissonsTemp != null)
                {
                    for (int i = 0; i < _youngsPoissonsTemp.Length; i++)
                    {
                        if (_youngsPoissonsTemp[i][0] <= 0) throw new CaeException(_positive);
                    }
                }
            }
        }


        // Constructors                                                                                                             
        public Elastic(double[][] youngsPoissonsTemp)
        {
            // The constructor must wotk with E = 0
            _youngsPoissonsTemp = youngsPoissonsTemp;
        }


        // Methods                                                                                                                  

    }
}
