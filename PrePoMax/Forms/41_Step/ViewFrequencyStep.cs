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
        private CaeModel.FrequencyStep _frequencystep;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "Perturbation")]
        [DescriptionAttribute("Perturbation parameter set to On applies preloads from the previous step if it exists.")]
        public bool Perturbation { get { return _frequencystep.Perturbation; } set { _frequencystep.Perturbation = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "Storage")]
        [DescriptionAttribute("Store eigenvalues, eigenmodes, mass and stiffness matrix in a binary form in file jobname.eig " +
                              "for further use.")]
        public bool Storage { get { return _frequencystep.Storage; } set { _frequencystep.Storage = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(6, 10, "Number of frequencies")]
        [DescriptionAttribute("Number of eigenfrequencies to compute.")]
        public int NumOfFrequencies
        {
            get { return _frequencystep.NumOfFrequencies; }
            set { _frequencystep.NumOfFrequencies = value; }
        }


        // Constructors                                                                                                             
        public ViewFrequencyStep(CaeModel.FrequencyStep step, bool installProvider = true)
            : base(step)
        {
            _frequencystep = step;
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
            return _frequencystep;
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
