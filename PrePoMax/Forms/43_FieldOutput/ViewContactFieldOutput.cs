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
    [EnumResource("PrePoMax.Properties.Resources")]
    [Editor(typeof(StandardValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [Flags]
    public enum ViewContactFieldVariable
    {
        // Must start at 1 for the UI to work
        [StandardValue("CDIS", Description = "Relative contact displacements.")]
        CDIS = 1,
        //
        [StandardValue("CSTR", Description = "Contact stresses.")]
        CSTR = 2,
        //
        //[StandardValue("CELS", Description = "Contact energy.")]
        //CELS = 4,
        //
        [StandardValue("PCON", Description = "Contact states for frequency and steady state dynamics calculations.")]
        PCON = 8
    }

    [Serializable]
    public class ViewContactFieldOutput : ViewFieldOutput
    {
        // Variables                                                                                                                
        private CaeModel.ContactFieldOutput _fieldOutput;


        // Properties                                                                                                               
        public override string Name { get { return _fieldOutput.Name; } set { _fieldOutput.Name = value; } }
        public override bool LastIterations
        {
            get { return _fieldOutput.LastIterations; }
            set { _fieldOutput.LastIterations = value; }
        }
        [Browsable(false)]
        public override bool ContactElements
        {
            get { return _fieldOutput.ContactElements; }
            set { _fieldOutput.ContactElements = value; }
        }
        //
        [OrderedDisplayName(3, 10, "Variables to output")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Contact field variables")]
        public ViewContactFieldVariable Variables 
        { 
            get
            { 
                return (ViewContactFieldVariable)_fieldOutput.Variables; 
            } 
            set
            { 
                _fieldOutput.Variables = (CaeModel.ContactFieldVariable)value;
            } 
        }
        //
        public override CaeModel.FieldOutput Base
        {
            get { return _fieldOutput; }
            set { _fieldOutput = (CaeModel.ContactFieldOutput)value; }
        }


        // Constructors                                                                                                             
        public ViewContactFieldOutput(CaeModel.ContactFieldOutput fieldOutput)
        {
            _fieldOutput = fieldOutput;
            _dctd = ProviderInstaller.Install(this);
            //
            _dctd.RenameBooleanPropertyToOnOff(nameof(LastIterations));
            //
            StringIntegerDefaultConverter.SetInitialValue = 1;
        }
    }



   
}
