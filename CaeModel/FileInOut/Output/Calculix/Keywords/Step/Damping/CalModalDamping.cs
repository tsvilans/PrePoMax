using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;
using CaeGlobals;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalModalDamping : CalculixKeyword
    {
        // Variables                                                                                                                
        private ModalDamping _modalDamping;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalModalDamping(ModalDamping modalDamping)
        {
            if (modalDamping.DampingType == ModalDampingTypeEnum.Off)
                throw new NotSupportedException();
            //
            _modalDamping = modalDamping;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            string type = _modalDamping.DampingType == ModalDampingTypeEnum.Rayleigh ? ", Rayleigh" : "";
            return string.Format("*Modal damping{0}{1}", type, Environment.NewLine);
        }
        public override string GetDataString()
        {
            string data;
            if (_modalDamping.DampingType == ModalDampingTypeEnum.Constant)
            {
                data = string.Format("1, 1000000, {0}{1}", _modalDamping.ViscousDampingRatio.ToCalculiX16String(),
                                     Environment.NewLine);
            }
            else if (_modalDamping.DampingType == ModalDampingTypeEnum.Direct)
            {
                data = "";
                foreach (var entry in _modalDamping.DampingRatiosAndRanges)
                {
                    data += string.Format("{0}, {1}, {2}{3}", entry.LowestMode, entry.HighestMode,
                                                              entry.DampingRatio.ToCalculiX16String(), Environment.NewLine);
                }
            }
            else if (_modalDamping.DampingType == ModalDampingTypeEnum.Rayleigh)
            {
                data = string.Format(" , , {0}, {1}{2}", _modalDamping.Alpha.ToCalculiX16String(),
                                     _modalDamping.Beta.ToCalculiX16String(), Environment.NewLine);
            }
            else throw new NotSupportedException();
            //
            return data;
        }
    }
}
