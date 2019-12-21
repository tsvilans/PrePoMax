﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalSubmodel : CalculixKeyword
    {
        // Variables                                                                                                                
        string _globalResultsFileName;
        string[] _nodeSetNames;


        // Properties                                                                                                               
        public override object BaseItem { get { return _nodeSetNames; } }


        // Events                                                                                                                   


        // Constructor                                                                                                              
        public CalSubmodel(string globalResultsFileName, string[] nodeSetNames)
        {
            _globalResultsFileName = globalResultsFileName;
            _nodeSetNames = nodeSetNames;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            return string.Format("*Submodel, Type=Node, Input=\"{0}\"{1}", _globalResultsFileName.ToUTF8(), Environment.NewLine);
        }

        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var nodeSetName in _nodeSetNames) sb.AppendLine(nodeSetName);
            return sb.ToString();
        }
    }
}
