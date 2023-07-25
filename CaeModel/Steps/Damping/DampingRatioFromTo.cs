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
    public class DampingRatioAndRange
    {
        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Lowest mode")]
        [DescriptionAttribute("Lowest mode of the mode range.")]
        public int LowestMode { get; set; }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Highest mode")]
        [DescriptionAttribute("Highest mode of the mode range.")]
        public int HighestMode { get; set; }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Damping ratio")]
        [DescriptionAttribute("Viscous damping ratio between the damping coefficient and the critical damping coefficient.")]
        public double DampingRatio { get; set; }


        // Constructors                                                                                                             
        public DampingRatioAndRange()
        {
        }
        public DampingRatioAndRange(double dampingRatio, int lowestMode, int highestMode)
            : this()
        {
            DampingRatio = dampingRatio;
            LowestMode = lowestMode;
            HighestMode = highestMode;
        }
    }
}
