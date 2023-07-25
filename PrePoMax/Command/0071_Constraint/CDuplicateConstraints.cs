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
    class CDuplicateConstraints : Command
    {
        // Variables                                                                                                                
        private string[] _constraintNames;


        // Constructor                                                                                                              
        public CDuplicateConstraints(string[] constraintNames)
            : base("Duplicate constraints")
        {
            _constraintNames = constraintNames;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.DuplicateConstraints(_constraintNames);
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + GetArrayAsString(_constraintNames);
        }
    }
}
