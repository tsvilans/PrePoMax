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
    public class ViewBuckleStep : ViewStep
    {
        // Variables                                                                                                                
        private CaeModel.BuckleStep _buckleStep;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "Perturbation")]
        [DescriptionAttribute("Perturbation parameter set to On applies preloads from the previous step if it exists.")]
        public bool Perturbation { get { return _buckleStep.Perturbation; } set { _buckleStep.Perturbation = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "Num. of buckling factors")]
        [DescriptionAttribute("Number of buckling factors desired (default: 1).")]
        public int NumBucklingFactors
        {
            get { return _buckleStep.NumOfBucklingFactors; }
            set { _buckleStep.NumOfBucklingFactors = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(6, 10, "Accuracy")]
        [DescriptionAttribute("Accuracy desired (default: 0.01).")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double Accuracy { get { return _buckleStep.Accuracy; } set { _buckleStep.Accuracy = value; } }


        // Constructors                                                                                                             
        public ViewBuckleStep(CaeModel.BuckleStep step, bool installProvider = true)
            : base(step)
        {
            _buckleStep = step;
            //
            if (installProvider)
            {
                InstallProvider();
                UpdateVisibility();
            }
        }


        // Methods
        public override CaeModel.Step GetBase()
        {
            return _buckleStep;
        }
        public override void InstallProvider()
        {
            base.InstallProvider();
            //
            _dctd.RenameBooleanPropertyToOnOff("Perturbation");
        }
        public override void UpdateVisibility()
        {
            base.UpdateVisibility();
        }
    }
}
