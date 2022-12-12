using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    [Serializable]
    public class ViewHeatTransferStep : ViewStaticStep
    {
        // Variables                                                                                                                
        protected CaeModel.HeatTransferStep _heatTransferStep;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(9, 10, "Steady state")]
        [DescriptionAttribute("Enable/disable the steady state heat transfer solution.")]
        public bool SteadyState { get { return _heatTransferStep.SteadyState; } set { _heatTransferStep.SteadyState = value; } }
        //
        //[CategoryAttribute("Advanced")]
        //[OrderedDisplayName(0, 10, "Deltmx")]
        //[DescriptionAttribute("Value of the parameter DELTMX that is used to limit the temperature change" + 
        //                      " in two subsequent increments.")]
        //public double Deltmx { get { return _heatTransferStep.Deltmx; } set { _heatTransferStep.Deltmx = value; } }


        // Constructors                                                                                                             
        public ViewHeatTransferStep(CaeModel.HeatTransferStep step)
            : base(step, false)
        {
            _heatTransferStep = step;
            InstallProvider();
            //
            _dctd.RenameBooleanPropertyToOnOff(nameof(SteadyState));
            //
            _dctd.GetProperty(nameof(Nlgeom)).SetIsBrowsable(false);
        }


        // Methods
        public override CaeModel.Step GetBase()
        {
            return _heatTransferStep;
        }
        public override void UpdateVisibility()
        {
            base.UpdateVisibility();
        }
    }
}
