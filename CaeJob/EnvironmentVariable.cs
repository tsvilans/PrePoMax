using CaeGlobals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeJob
{
    [Serializable]
    public class EnvironmentVariable
    {
        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Active")]
        [DescriptionAttribute("The state of the evironment variable.")]
        public bool Active { get; set; }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Name")]
        [DescriptionAttribute("The name of the evironment variable.")]
        public string Name { get; set; }

        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Value")]
        [DescriptionAttribute("The value of the evironment variable.")]
        public string Value { get; set; }

        // Constructors                                                                                                             
        public EnvironmentVariable()
        {
            //Active = true;
        }
        public EnvironmentVariable(string name, string value)
            : this()
        {
            Active = true;
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return Name + "=" + Value;
        }

    }
}
