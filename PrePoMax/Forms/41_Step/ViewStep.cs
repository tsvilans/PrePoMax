using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    [Serializable]
    public class ViewStep
    {
        // Variables                                                                                                                
        protected CaeModel.Step _step;
        protected DynamicCustomTypeDescriptor _dctd = null;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the step.")]
        public string Name { get { return _step.Name; } set { _step.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Procedure")]
        [DescriptionAttribute("Select Check model to only check the input deck and geometry. The set of equations is built " +
                              "but not solved (the Jacobian determinant is checked).")]
        public bool RunAnalysis
        {
            get { return _step.RunAnalysis; }
            set { _step.RunAnalysis = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Solver")]
        [DescriptionAttribute("Select the matrix solver type.")]
        public CaeModel.SolverTypeEnum SolverType
        {
            get { return _step.SolverType; }
            set { _step.SolverType = value; }
        }


        // Constructors                                                                                                             
        public ViewStep(CaeModel.Step step)
        {
            if (step == null) throw new ArgumentNullException();
            //
            _step = step;
        }

        // Methods
        public virtual CaeModel.Step GetBase()
        {
            return _step;
        }
        public virtual void InstallProvider()
        {
            _dctd = ProviderInstaller.Install(this);
            //
            _dctd.RenameBooleanProperty(nameof(RunAnalysis), "Analysis", "Check model");
        }
        public virtual void UpdateVisibility()
        {
            _dctd.GetProperty(nameof(RunAnalysis)).SetIsBrowsable(false);
        }
    }
}
