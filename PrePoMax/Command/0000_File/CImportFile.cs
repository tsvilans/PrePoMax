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
    class CImportFile : Command, ICommandWithDialog
    {
        // Variables                                                                                                                
        private string _fileName;
        private bool _onlyMaterials;


        // Properties                                                                                                               
        public string FileName { get { return _fileName; } }
        public bool OnlyMaterials { get { return _onlyMaterials; } }


        // Constructor                                                                                                              
        public CImportFile(string fileName, bool onlyMaterials)
            :base("Import file")
        {
            _fileName = Tools.GetLocalPath(fileName);
            _onlyMaterials = onlyMaterials;
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.ImportFile(Tools.GetGlobalPath(_fileName), _onlyMaterials);
            return true;
        }

        public void ExecuteWithDialogs(Controller receiver)
        {
            string fileName = receiver.GetFileNameToImport(_onlyMaterials);
            if (fileName != null) _fileName = Tools.GetLocalPath(fileName);
            Execute(receiver);
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _fileName;
        }
    }
}
