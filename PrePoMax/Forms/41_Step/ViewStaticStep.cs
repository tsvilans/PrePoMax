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
    public class ViewStaticStep : ViewStep
    {
        // Variables                                                                                                                
        private CaeModel.StaticStep _staticStep;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Nlgeom")]
        [DescriptionAttribute("Enable/disable the nonlinear effects of large deformations and large displacements.")]
        public bool Nlgeom { get { return _staticStep.Nlgeom; } set { _staticStep.Nlgeom = value; } }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(0, 10, "Type")]
        [DescriptionAttribute("Select the incrementation type.")]
        public CaeModel.IncrementationTypeEnum IncrementationType
        { 
            get { return _staticStep.IncrementationType; }
            set
            {
                _staticStep.IncrementationType = value;
                _staticStep.Direct = _staticStep.IncrementationType == CaeModel.IncrementationTypeEnum.Direct;
            }
        }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(1, 10, "Direct")]
        [DescriptionAttribute("By using the 'Direct' keyword automatic incrementation of nonlinear step is switched off.")]
        public bool Direct { get { return _staticStep.Direct; } set { _staticStep.Direct = value; } }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(2, 10, "Max increments")]
        [DescriptionAttribute("The maximum number of increments in the step.")]
        public int MaxIncrements { get { return _staticStep.MaxIncrements; } set { _staticStep.MaxIncrements = value; } }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(3, 10, "Time period")]
        [DescriptionAttribute("Time period of the step.")]
        [TypeConverter(typeof(StringTimeConverter))]
        public double TimePeriod { get { return _staticStep.TimePeriod; } set { _staticStep.TimePeriod = value; } }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(4, 10, "Initial time increment")]
        [DescriptionAttribute("Initial time increment of the step.")]
        [TypeConverter(typeof(StringTimeConverter))]
        public double InitialTimeIncrement
        {
            get { return _staticStep.InitialTimeIncrement; }
            set { _staticStep.InitialTimeIncrement = value; }
        }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(5, 10, "Min time increment")]
        [DescriptionAttribute("Minimum time increment allowed.")]
        [TypeConverter(typeof(StringTimeConverter))]
        public double MinTimeIncrement
        {
            get { return _staticStep.MinTimeIncrement; }
            set { _staticStep.MinTimeIncrement = value; }
        }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(6, 10, "Max time increment")]
        [DescriptionAttribute("Maximum time increment allowed.")]
        [TypeConverter(typeof(StringTimeConverter))]
        public double MaxTimeIncrement
        {
            get { return _staticStep.MaxTimeIncrement; }
            set { _staticStep.MaxTimeIncrement = value; }
        }


        // Constructors                                                                                                             
        public ViewStaticStep(CaeModel.StaticStep step, bool installProvider = true)
            : base(step)
        {
            _staticStep = step;
            if (installProvider) InstallProvider();
        }


        // Methods
        public override CaeModel.Step GetBase()
        {
            return _staticStep;
        }
        protected void InstallProvider()
        {
            _dctd = ProviderInstaller.Install(this);
            //
            _dctd.RenameBooleanPropertyToOnOff(nameof(Nlgeom));
            _dctd.RenameBooleanPropertyToOnOff(nameof(Direct));
            //
            UpdateVisibility();
        }
        public override void UpdateVisibility()
        {
            _dctd.GetProperty(nameof(Direct)).SetIsBrowsable(false);
            //
            if (_staticStep.IncrementationType == CaeModel.IncrementationTypeEnum.Default)
            {
                _dctd.GetProperty(nameof(TimePeriod)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(MaxIncrements)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(InitialTimeIncrement)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(MinTimeIncrement)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(MaxTimeIncrement)).SetIsBrowsable(false);
            }
            else if (_staticStep.IncrementationType == CaeModel.IncrementationTypeEnum.Automatic)
            {
                _dctd.GetProperty(nameof(TimePeriod)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(MaxIncrements)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(InitialTimeIncrement)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(MinTimeIncrement)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(MaxTimeIncrement)).SetIsBrowsable(true);
            }
            else if (_staticStep.IncrementationType == CaeModel.IncrementationTypeEnum.Direct)
            {
                _dctd.GetProperty(nameof(TimePeriod)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(MaxIncrements)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(InitialTimeIncrement)).SetIsBrowsable(true);
                _dctd.GetProperty(nameof(MinTimeIncrement)).SetIsBrowsable(false);
                _dctd.GetProperty(nameof(MaxTimeIncrement)).SetIsBrowsable(false);
            }
            else throw new NotSupportedException();
        }

    }
}
