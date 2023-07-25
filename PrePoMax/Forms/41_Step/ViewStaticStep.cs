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
        [OrderedDisplayName(3, 10, "Nlgeom")]
        [DescriptionAttribute("Enable/disable the nonlinear effects of large deformations and large displacements.")]
        [Id(4, 1)]
        public bool Nlgeom { get { return _staticStep.Nlgeom; } set { _staticStep.Nlgeom = value; } }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(0, 10, "Type")]
        [DescriptionAttribute("Select the incrementation type.")]
        [Id(1, 2)]
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
        [Id(2, 2)]
        public bool Direct { get { return _staticStep.Direct; } set { _staticStep.Direct = value; } }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(2, 10, "Max increments")]
        [DescriptionAttribute("The maximum number of increments in the step.")]
        [TypeConverter(typeof(StringIntegerConverter))]
        [Id(3, 2)]
        public int MaxIncrements { get { return _staticStep.MaxIncrements; } set { _staticStep.MaxIncrements = value; } }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(3, 10, "Time period")]
        [DescriptionAttribute("Time period of the step.")]
        [TypeConverter(typeof(StringTimeConverter))]
        [Id(4, 2)]
        public double TimePeriod { get { return _staticStep.TimePeriod; } set { _staticStep.TimePeriod = value; } }
        //
        [CategoryAttribute("Incrementation")]
        [OrderedDisplayName(4, 10, "Initial time increment")]
        [DescriptionAttribute("Initial time increment of the step.")]
        [TypeConverter(typeof(StringTimeConverter))]
        [Id(5, 2)]
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
        [Id(6, 2)]
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
        [Id(7, 2)]
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
            //
            if (installProvider)
            {
                InstallProvider();
                UpdateVisibility();
            }
        }


        // Methods
        public override CaeModel.Step GetBase()
        {
            return _staticStep;
        }
        public override void InstallProvider()
        {
            base.InstallProvider();
            //
            _dctd.RenameBooleanPropertyToOnOff(nameof(Nlgeom));
            _dctd.RenameBooleanPropertyToOnOff(nameof(Direct));
        }
        public override void UpdateVisibility()
        {
            base.UpdateVisibility();
            //
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
