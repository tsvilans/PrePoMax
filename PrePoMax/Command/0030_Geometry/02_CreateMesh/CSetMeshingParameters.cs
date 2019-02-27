﻿using System;
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
    class CSetMeshingParameters : Command, ICommandWithDialog
    {
        // Variables                                                                                                                
        private string _partName;
        private MeshingParameters _meshingParameters;


        // Constructor                                                                                                              
        public CSetMeshingParameters(string partName, MeshingParameters meshingParameters)
            : base("Set meshing parameters")
        {
            _partName = partName;
            _meshingParameters = meshingParameters.DeepClone();
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.SetMeshingParameters(_partName, _meshingParameters.DeepClone());
            return true;
        }

        public void ExecuteWithDialogs(Controller receiver)
        {
            _meshingParameters = receiver.GetMeshingParameters(_partName);
            if (_meshingParameters == null) _meshingParameters = receiver.GetDefaultMeshingParameters(_partName);
            Execute(receiver);
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _partName;
        }
    }
}
