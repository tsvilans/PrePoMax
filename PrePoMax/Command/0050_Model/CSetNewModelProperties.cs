using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;
using CaeMesh;
using CaeGlobals;


namespace PrePoMax.Commands
{
    [Serializable]
    class CSetNewModelProperties : Command
    {
        // Variables                                                                                                                
        private ModelSpaceEnum _modelSpace;
        private UnitSystemType _unitSystemType;


        // Properties                                                                                                               
        public ModelSpaceEnum ModelSpace { get { return _modelSpace; } }
        public UnitSystemType UnitSystemType { get { return _unitSystemType; } }


        // Constructor                                                                                                              
        public CSetNewModelProperties(ModelSpaceEnum modelSpace, UnitSystemType unitSystemType)
            : base("Set new model properties")
        {
            _modelSpace = modelSpace;
            _unitSystemType = unitSystemType;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.SetNewModelProperties(_modelSpace, _unitSystemType);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + "Model space: " + _modelSpace.GetDisplayedName() 
                                           + ", Unit system: " + _unitSystemType.GetDescription();
        }
    }
}
