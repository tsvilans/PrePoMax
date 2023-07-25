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
    class CAddMeshingParameters : Command
    {
        // Variables                                                                                                                
        private MeshingParameters _meshingParameters;


        // Constructor                                                                                                              
        public CAddMeshingParameters(MeshingParameters meshingParameters)
            : base("Add meshing parameters")
        {
            _meshingParameters = meshingParameters.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.AddMeshingParameters(_meshingParameters.DeepClone());
            return true;
        }
        public override string GetCommandString()
        {
            return base.GetCommandString() + _meshingParameters.ToString();
        }
    }
}
