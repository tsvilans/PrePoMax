﻿using System;
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
    public class ViewSolidSection : ViewSection
    {
        // Variables                                                                                                                
        private CaeModel.SolidSection _solidSection;
        

        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "Thickness")]
        [DescriptionAttribute("Set the thickness in the case of 2D plain strain/stress state.")]
        public double Thickness
        {
            get { return _solidSection.Thickness; }
            set { _solidSection.Thickness = value; }
        }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(6, 10, "Type")]
        [DescriptionAttribute("The type of the solid section.")]
        public CaeModel.SolidSectionType Type
        {
            get { return _solidSection.Type; }
        }


        // Constructors                                                                                                             
        public ViewSolidSection(CaeModel.SolidSection solidSection)
        {
            _solidSection = solidSection;
            SetBase(_solidSection);
        }


        // Methods                                                                                                                  
       


    }

}
