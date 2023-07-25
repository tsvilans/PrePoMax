using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using System.IO;
using DynamicTypeDescriptor;
using System.Drawing;
using CaeMesh;

namespace PrePoMax
{
    [Serializable]
    public class MeshingSettings : ISettings
    {
        // Variables                                                                                                                
        private MeshingParameters _meshingParameters;


        // Properties                                                                                                               
        public MeshingParameters MeshingParameters { get { return _meshingParameters; } set { _meshingParameters = value; } }


        // Constructors                                                                                                             
        public MeshingSettings()
        {
            _meshingParameters = new MeshingParameters("Settings");
        }
        public MeshingSettings(MeshingParameters meshingParameters)
        {
            _meshingParameters = meshingParameters;
        }


        // Methods                                                                                                                  
        public void CheckValues()
        {
        }
        public void Reset()
        {
            _meshingParameters.Reset();
        }
        
    }
}
