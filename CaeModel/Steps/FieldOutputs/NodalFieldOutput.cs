﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;

namespace CaeModel
{
    [Serializable]
    [Flags]
    public enum NodalFieldVariable
    {
        // Must start at 1 for the UI to work
        RF = 1,
        U = 2,
        PU = 4,
        V = 8,
        // Thermal
        NT = 16,
        PNT = 32,
        RFL = 64
    }

    [Serializable]
    public class NodalFieldOutput : FieldOutput
    {
        // Variables                                                                                                                
        private NodalFieldVariable _variables;
        private bool _binary;


        // Properties                                                                                                               
        public NodalFieldVariable Variables { get { return _variables; } set { _variables = value; } }
        public bool Binary { get { return _binary; } set { _binary = value; } }


        // Constructors                                                                                                             
        public NodalFieldOutput(string name, NodalFieldVariable variables, bool binary = false)
            : base(name) 
        {
            _variables |= variables;
            _binary = binary;

        }


        // Methods                                                                                                                  
    }
}
