﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    [Flags]
    public enum ContactHistoryVariable
    {
        // Must start at 1 for the UI to work
        CDIS = 1,
        CSTR = 2,
        CELS = 4,
        CNUM = 8,
        CF = 16
        //CFN = 32,
        //CFS = 64
    }

    [Serializable]
    public class ContactHistoryOutput : HistoryOutput
    {
        // Variables                                                                                                                
        private ContactHistoryVariable _variables;
        private bool _isInWearStep;


        // Properties                                                                                                               
        public ContactHistoryVariable Variables { get { return _variables; } set { _variables = value; } }
        public bool IsInWearStep { get { return _isInWearStep; } set { _isInWearStep = value; } }


        // Constructors                                                                                                             
        public ContactHistoryOutput(string name, ContactHistoryVariable variables, string contactPairName, bool isInWearStep)
            : base(name, contactPairName, RegionTypeEnum.ContactPair)
        {
            _variables = variables;
            _isInWearStep = isInWearStep;
        }


        // Methods                                                                                                                  
    }
}
