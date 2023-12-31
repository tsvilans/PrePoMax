﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using CaeModel;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    [Serializable]
    public class ViewDynamicStep : ViewStaticStep
    {
        // Variables                                                                                                                
        protected DynamicStep _dynamicStep;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "Alpha")]
        [DescriptionAttribute("Enter the value between -1/3 and 0 to control the dissipation of the high frequency response " +
                              "(numerical damping).")]
        [TypeConverter(typeof(StringDoubleConverter))]
        [Id(5, 1)]
        public double Alpha { get { return _dynamicStep.Alpha; } set { _dynamicStep.Alpha = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "Solution procedure")]
        [DescriptionAttribute("Select the solution procedure for structural/fluid computations.")]
        [Id(6, 1)]
        public SolutionProcedureEnum SolutionProcedure
        {
            get { return _dynamicStep.SolutionProcedure; }
            set { _dynamicStep.SolutionProcedure = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(6, 10, "Relative to absolute")]
        [DescriptionAttribute("Set to yes if in the previous step the coordinate system was attached to a rotating system and " + 
                              "the coordinate system in the present dynamic step should be absolute.")]
        [Id(7, 1)]
        public bool RelativeToAbsolute
        {
            get { return _dynamicStep.RelativeToAbsolute; }
            set { _dynamicStep.RelativeToAbsolute = value; }
        }
        //
        [CategoryAttribute("Damping")]
        [OrderedDisplayName(0, 10, "Damping type")]
        [DescriptionAttribute("Select the damping type.")]
        [Id(1, 2)]
        public DampingTypeEnum DampingType
        {
            get { return _dynamicStep.Damping.DampingType; }
            set
            {
                _dynamicStep.Damping.DampingType = value;
                UpdateVisibility();
            }
        }
        //
        [CategoryAttribute("Damping")]
        [OrderedDisplayName(1, 10, "Alpha")]
        [DescriptionAttribute("Mass-proportional damping coefficient of the Rayleigh damping.")]
        [TypeConverter(typeof(StringReciprocalTimeConverter))]
        [Id(2, 2)]
        public double AlphaRayleigh { get { return _dynamicStep.Damping.Alpha; } set { _dynamicStep.Damping.Alpha = value; } }
        //
        [CategoryAttribute("Damping")]
        [OrderedDisplayName(2, 10, "Beta")]
        [DescriptionAttribute("Stiffness-proportional damping coefficient of the Rayleigh damping.")]
        [TypeConverter(typeof(StringTimeConverter))]
        [Id(3, 2)]
        public double BetaRayleigh { get { return _dynamicStep.Damping.Beta; } set { _dynamicStep.Damping.Beta = value; } }


        // Constructors                                                                                                             
        public ViewDynamicStep(DynamicStep step, bool installProvider = true)
            : base(step, false)
        {
            _dynamicStep = step;
            //
            if (installProvider)
            {
                InstallProvider();
                UpdateVisibility();
            }
        }


        // Methods
        public override Step GetBase()
        {
            return _dynamicStep;
        }
        public override void InstallProvider()
        {
            base.InstallProvider();
            //
            _dctd.RenameBooleanPropertyToOnOff(nameof(RelativeToAbsolute));
        }
        public override void UpdateVisibility()
        {
            base.UpdateVisibility();
            //
            //_dctd.GetProperty(nameof(Nlgeom)).SetIsBrowsable(false);
            //
            bool browsable = _dynamicStep.Damping.DampingType == DampingTypeEnum.Rayleigh;
            _dctd.GetProperty(nameof(AlphaRayleigh)).SetIsBrowsable(browsable);
            _dctd.GetProperty(nameof(BetaRayleigh)).SetIsBrowsable(browsable);
        }

    }
}
