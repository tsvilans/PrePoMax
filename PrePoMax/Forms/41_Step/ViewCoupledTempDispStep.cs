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
    public class ViewCoupledTempDispStep : ViewUncoupledTempDispStep
    {
        // Variables                                                                                                                


        // Properties                                                                                                               
        

        // Constructors                                                                                                             
        public ViewCoupledTempDispStep(CaeModel.UncoupledTempDispStep step, bool installProvider = true)
            : base(step, false)
        {
            if (installProvider)
            {
                InstallProvider();
                UpdateVisibility();
            }
        }


        // Methods
        public override CaeModel.Step GetBase()
        {
            return _heatTransferStep;
        }
        public override void InstallProvider()
        {
            base.InstallProvider();
        }
        public override void UpdateVisibility()
        {
            base.UpdateVisibility();
        }
    }
}
