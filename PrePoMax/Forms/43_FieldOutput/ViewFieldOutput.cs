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
    public abstract class ViewFieldOutput
    {
        // Variables                                                                                                                
        protected DynamicCustomTypeDescriptor _dctd;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the field output.")]
        public abstract string Name { get; set; }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Frequency")]
        [DescriptionAttribute("Integer N, which indicates that only results of every N-th increment will be stored.")]
        public abstract int Frequency { get ; set; }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Last iterations")]
        [DescriptionAttribute("Turning last iterations on is useful for debugging purposes in case of divergent solution.")]
        public abstract bool LastIterations { get; set; }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Contact elements")]
        [DescriptionAttribute("Turning contact elements on stores the contact elements in a file with the .cel extension.")]
        public abstract bool ContactElements { get; set; }
        //
        [Browsable(false)]
        public abstract CaeModel.FieldOutput Base { get; set; }


        // Constructors                                                                                                             


        // Methods
       
    }
}
