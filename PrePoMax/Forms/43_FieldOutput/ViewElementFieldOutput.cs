using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    [Serializable]
    [EnumResource("PrePoMax.Properties.Resources")]
    [Editor(typeof(StandardValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [Flags]
    public enum ViewElementFieldVariable
    {
        // Must start at 1 for the UI to work
        [StandardValue("S", Description = "Stresses.")]
        S = 1,
        //
        [StandardValue("E", Description = "Total strains.")]
        E = 2,
        //
        [StandardValue("ME", Description = "Mechanical strains.")]
        ME = 4,
        //
        [StandardValue("PEEQ", Description = "Equivalent plastic strain.")]
        PEEQ = 8,
        //
        [StandardValue("ENER", Description = "Energy density.")]
        ENER = 16,
        // Thermal
        [StandardValue("HFL", Description = "Heat flux.")]
        HFL = 32,
        // Error
        [StandardValue("ERR", Description = "Extrapolation error estimator for stress calculations. " +
                                            "ERR and ZZS are mutually exclusive.")]
        ERR = 64,
        //
        [StandardValue("HER", Description = "Extrapolation error estimator for heat calculations. " +
                                            "HER and ZZS are mutually exclusive.")]
        HER = 128,
        //
        [StandardValue("ZZS", Description = "Zienkiewicz-Zhu improved stress. ERR and ZZS are mutually exclusive.")]
        ZZS = 256
    }

    [Serializable]
    public class ViewElementFieldOutput : ViewFieldOutput
    {
        // Variables                                                                                                                
        private ElementFieldOutput _fieldOutput;


        // Properties                                                                                                               
        public override string Name { get { return _fieldOutput.Name; } set { _fieldOutput.Name = value; } }
        public override int Frequency { get { return _fieldOutput.Frequency; } set { _fieldOutput.Frequency = value; } }
        public override bool LastIterations
        {
            get { return _fieldOutput.LastIterations; }
            set { _fieldOutput.LastIterations = value; }
        }
        public override bool ContactElements
        {
            get { return _fieldOutput.ContactElements; }
            set { _fieldOutput.ContactElements = value; }
        }
        //
        [OrderedDisplayName(4, 10, "Output (2D/3D)")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("If Output=3D, the 1D and 2D elements are stored in their expanded 3D form.")]
        public ElementFieldOutputOutputEnum Output
        {
            get { return _fieldOutput.Output; }
            set { _fieldOutput.Output = value; }
        }
        //
        [OrderedDisplayName(5, 10, "Variables to output")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Element field variables")]
        public ViewElementFieldVariable Variables
        {
            get
            {
                return (ViewElementFieldVariable)_fieldOutput.Variables;
            }
            set
            {
                if (((value & ViewElementFieldVariable.ERR) == ViewElementFieldVariable.ERR) &&
                    ((value & ViewElementFieldVariable.ZZS) == ViewElementFieldVariable.ZZS))
                {
                    throw new Exception("ERR and ZZS are mutually exclusive.");
                }
                //
                if (((value & ViewElementFieldVariable.HER) == ViewElementFieldVariable.HER) &&
                    ((value & ViewElementFieldVariable.ZZS) == ViewElementFieldVariable.ZZS))
                {
                    throw new Exception("HER and ZZS are mutually exclusive.");
                }
                //
                _fieldOutput.Variables = (CaeModel.ElementFieldVariable)value;
            }
        }
        //
        public override FieldOutput Base { get { return _fieldOutput; } set { _fieldOutput = (ElementFieldOutput)value; } }


        // Constructors                                                                                                             
        public ViewElementFieldOutput(ElementFieldOutput fieldOutput)
        {
            _fieldOutput = fieldOutput;
            _dctd = ProviderInstaller.Install(this);
            //
            _dctd.RenameBooleanPropertyToOnOff(nameof(LastIterations));
            _dctd.RenameBooleanPropertyToOnOff(nameof(ContactElements));
        }


    }

}
