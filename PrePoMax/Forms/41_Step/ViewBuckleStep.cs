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
        [OrderedDisplayName(3, 10, "Perturbation")]
        [DescriptionAttribute("Perturbation parameter set to On applies preloads from the previous step if it exists.")]
        public bool Perturbation { get { return _buckleStep.Perturbation; } set { _buckleStep.Perturbation = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "Num. of buckling factors")]
        [DescriptionAttribute("Number of buckling factors desired (default: 1).")]
        public int NumBucklingFactors
        {
            get { return _buckleStep.NumOfBucklingFactors; }
            set { _buckleStep.NumOfBucklingFactors = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "Accuracy")]
        [DescriptionAttribute("Accuracy desired (default: 0.01).")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double Accuracy { get { return _buckleStep.Accuracy; } set { _buckleStep.Accuracy = value; } }


        // Constructors                                                                                                             
        public ViewBuckleStep(CaeModel.BuckleStep step)
            : base(step)
        {
            _buckleStep = step;
            _dctd = ProviderInstaller.Install(this);
            //
            _dctd.RenameBooleanPropertyToOnOff("Perturbation");
            //
            UpdateVisibility();
        }


        // Methods
        public override CaeModel.Step GetBase()
        {
            return _buckleStep;
        }
    }
}
