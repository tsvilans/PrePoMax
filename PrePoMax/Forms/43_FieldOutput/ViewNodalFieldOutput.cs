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
    public enum ViewNodalFieldVariable
    {
        // Must start at 1 for the UI to work
        [StandardValue("RF", Description = "Reaction forces.")]
        RF = 1,
        [StandardValue("U", Description = "Displacements.")]
        U = 2,
        // Thermal
        [StandardValue("NT", Description = "Temperatures.")]
        NT = 4,
        [StandardValue("RFL", Description = "External concentrated heat sources.")]
        RFL = 8
    }

    [Serializable]
    public class ViewNodalFieldOutput : ViewFieldOutput
    {
        // Variables                                                                                                                
        private CaeModel.NodalFieldOutput _fieldOutput;


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
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "Variables to output")]
        [DescriptionAttribute("Nodal field variables")]
        public ViewNodalFieldVariable Variables 
        { 
            get
            { 
                return (ViewNodalFieldVariable)_fieldOutput.Variables; 
            } 
            set
            { 
                _fieldOutput.Variables = (CaeModel.NodalFieldVariable)value;
            } 
        }
        //
        
        public override CaeModel.FieldOutput Base { get { return _fieldOutput; } set { _fieldOutput = (CaeModel.NodalFieldOutput)value; } }


        // Constructors                                                                                                             
        public ViewNodalFieldOutput(CaeModel.NodalFieldOutput fieldOutput)
        {
            _fieldOutput = fieldOutput;
            _dctd = ProviderInstaller.Install(this);
            //
            _dctd.RenameBooleanPropertyToOnOff(nameof(LastIterations));
            _dctd.RenameBooleanPropertyToOnOff(nameof(ContactElements));
        }
    }



   
}
