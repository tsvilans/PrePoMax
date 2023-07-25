using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Management;
using System.ComponentModel;
using CaeJob;
using CaeGlobals;
using System.Drawing.Design;
using DynamicTypeDescriptor;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewJob
    {
        // Variables                                                                                                                
        protected AnalysisJob _job;
        protected DynamicCustomTypeDescriptor _dctd;

        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the analysis.")]
        [Id(1, 1)]
        public string Name { get { return _job.Name; } set { _job.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Executable")]
        [DescriptionAttribute("Calculix executable file (ccx.exe).")]
        [Id(2, 1)]
        public string Executable
        {
            get { return _job.Executable; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Executable arguments")]
        [DescriptionAttribute("Addtional Calculix arguments. Change this value only if you want to run the solver in a different way.")]
        [Id(3, 1)]
        public string Argument
        {
            get { return _job.Argument; }
            set { _job.Argument = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Work directory")]
        [DescriptionAttribute("Work directory.")]
        [Id(4, 1)]
        public string WorkDirectory { get { return _job.WorkDirectory; } }
        //
        [CategoryAttribute("Advanced settings")]
        [OrderedDisplayName(0, 10, "Compatibility mode")]
        [DescriptionAttribute("Run executable in a simplified mode for improved compatibility.")]
        [Id(1, 2)]
        public bool CompatibilityMode { get { return _job.CompatibilityMode; } set { _job.CompatibilityMode = value; } }


        // Constructor                                                                                                              
        public ViewJob(AnalysisJob job)
        {
            _job = job;
            _dctd = ProviderInstaller.Install(this);
            //
            _dctd.RenameBooleanPropertyToOnOff(nameof(CompatibilityMode));
        }


        // Methods                                                                                                                  
        public AnalysisJob GetBase()
        {
            return _job;
        }
    }
}
