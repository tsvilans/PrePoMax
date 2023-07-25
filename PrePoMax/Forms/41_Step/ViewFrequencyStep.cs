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
    public class ViewFrequencyStep : ViewStep
    {
        // Variables                                                                                                                
        private CaeModel.FrequencyStep _frequencyStep;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "Perturbation")]
        [DescriptionAttribute("Perturbation parameter set to On applies preloads from the previous step if it exists.")]
        [Id(5, 1)]
        public bool Perturbation { get { return _frequencyStep.Perturbation; } set { _frequencyStep.Perturbation = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "Storage")]
        [DescriptionAttribute("Store eigenvalues, eigenmodes, mass and stiffness matrix in a binary form in file jobname.eig " +
                              "for further use.")]
        [Id(6, 1)]
        public bool Storage { get { return _frequencyStep.Storage; } set { _frequencyStep.Storage = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(6, 10, "Number of frequencies")]
        [DescriptionAttribute("Number of eigenfrequencies to compute.")]
        [Id(7, 1)]
        public int NumOfFrequencies
        {
            get { return _frequencyStep.NumOfFrequencies; }
            set { _frequencyStep.NumOfFrequencies = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(7, 10, "Lower frequency bound")]
        [DescriptionAttribute("Lower bound of the frequency range.")]
        [TypeConverter(typeof(StringFrequencyDefaultConverter))]
        [Id(8, 1)]
        public double LowestFrequency
        {
            get { return _frequencyStep.LowerFrequency; }
            set { _frequencyStep.LowerFrequency = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(8, 10, "Upper frequency bound")]
        [DescriptionAttribute("Upper bound of the frequency range.")]
        [TypeConverter(typeof(StringFrequencyDefaultConverter))]
        [Id(9, 1)]
        public double UpperFrequency
        {
            get { return _frequencyStep.UpperFrequency; }
            set { _frequencyStep.UpperFrequency = value; }
        }


        // Constructors                                                                                                             
        public ViewFrequencyStep(CaeModel.FrequencyStep step, bool installProvider = true)
            : base(step)
        {
            _frequencyStep = step;
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
            return _frequencyStep;
        }
        public override void InstallProvider()
        {
            base.InstallProvider();
            //
            _dctd.RenameBooleanPropertyToOnOff("Perturbation");
            _dctd.RenameBooleanPropertyToYesNo("Storage");
        }
        public override void UpdateVisibility()
        {
            base.UpdateVisibility();
        }
    }
}
