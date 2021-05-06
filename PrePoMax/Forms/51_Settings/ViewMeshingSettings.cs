﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;

namespace PrePoMax.Settings
{
    [Serializable]
    public class ViewMeshingSettings : Forms.ViewMeshingParameters, IViewSettings, IReset
    {
        // Variables                                                                                                                
        MeshingSettings _meshingSettings;


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public ViewMeshingSettings(MeshingSettings meshingSettings)
            : base(meshingSettings.MeshingParameters)
        {
            _meshingSettings = meshingSettings;
            //
            _dctd.GetProperty(nameof(MaxH)).SetIsBrowsable(false);
            _dctd.GetProperty(nameof(MinH)).SetIsBrowsable(false);
        }


        // Methods                                                                                                                  
        new public ISettings GetBase()
        {
            return _meshingSettings;
        }
        public void Reset()
        {
            _meshingSettings.Reset();
        }
    }
}
