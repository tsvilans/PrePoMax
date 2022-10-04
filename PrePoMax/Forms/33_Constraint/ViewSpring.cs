using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing;
using System.Drawing.Design;

namespace PrePoMax
{
    [Serializable]
    public abstract class ViewSpring : ViewConstraint
    {
        // Variables                                                                                                                
        protected CaeModel.SpringConstraint _spring;


        // Properties                                                                                                               
        public override string Name { get { return _spring.Name; } set { _spring.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(0, 10, "Region type")]
        [DescriptionAttribute("Select the region type for the creation of the constraint definition.")]
        [Id(1, 2)]
        public override string MasterRegionType { get { return base.MasterRegionType; } set { base.MasterRegionType = value; } }
        //
        [CategoryAttribute("Stiffness")]
        [OrderedDisplayName(0, 10, "K1")]
        [DescriptionAttribute("Value of the stiffness per node in the direction of the first axis.")]
        [TypeConverter(typeof(StringForcePerLenghtConverter))]
        [Id(1, 3)]
        public virtual double K1 { get { return _spring.K1; } set { _spring.K1 = value; } }
        //
        [CategoryAttribute("Stiffness")]
        [OrderedDisplayName(1, 10, "K2")]
        [DescriptionAttribute("Value of the stiffness per node in the direction of the second axis.")]
        [TypeConverter(typeof(StringForcePerLenghtConverter))]
        [Id(2, 3)]
        public virtual double K2 { get { return _spring.K2; } set { _spring.K2 = value; } }
        //
        [CategoryAttribute("Stiffness")]
        [OrderedDisplayName(2, 10, "K3")]
        [DescriptionAttribute("Value of the stiffness per node in the direction of the third axis.")]
        [TypeConverter(typeof(StringForcePerLenghtConverter))]
        [Id(3, 3)]
        public virtual double K3 { get { return _spring.K3; } set { _spring.K3 = value; } }
        //
        [Category("Appearance")]
        [DisplayName("Color")]
        [Description("Select the constraint color.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        [Id(1, 10)]
        public Color Color { get { return _spring.MasterColor; } set { _spring.MasterColor = value; } }


        // Constructors                                                                                                             
        public ViewSpring(CaeModel.SpringConstraint springConstraint)
        {
            _spring = springConstraint;
        }


        // Methods                                                                                                                  
        public override CaeModel.Constraint GetBase()
        {
            return _spring;
        }
        
    }

}
