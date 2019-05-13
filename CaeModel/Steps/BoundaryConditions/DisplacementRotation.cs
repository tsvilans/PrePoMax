﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class DisplacementRotation : BoundaryCondition, IMultiRegion
    {
        // Variables                                                                                                                
        private RegionTypeEnum _regionType;
        private string _regionName;


        // Properties                                                                                                               
        public string RegionName { get { return _regionName; } set { _regionName = value; } }
        public RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public double U1 { get; set; }
        public double U2 { get; set; }
        public double U3 { get; set; }
        public double UR1 { get; set; }
        public double UR2 { get; set; }
        public double UR3 { get; set; }


        // Constructors                                                                                                             
        public DisplacementRotation(string name, string regionName, RegionTypeEnum regionType)
            : base(name) 
        {
            _regionName = regionName;
            _regionType = regionType;
            U1 = double.NaN;
            U2 = double.NaN;
            U3 = double.NaN;
            UR1 = double.NaN;
            UR2 = double.NaN;
            UR3 = double.NaN;
        }


        // Methods                                                                                                                  
        public int[] GetConstrainedDirections()
        {
            List<int> directions = new List<int>();
            if (!double.IsNaN(U1)) directions.Add(1);
            if (!double.IsNaN(U2)) directions.Add(2);
            if (!double.IsNaN(U3)) directions.Add(3);
            if (!double.IsNaN(UR1)) directions.Add(4);
            if (!double.IsNaN(UR2)) directions.Add(5);
            if (!double.IsNaN(UR3)) directions.Add(6);
            return directions.ToArray();
        }
        public double[] GetConstrainValues()
        {
            List<double> values = new List<double>();
            if (!double.IsNaN(U1)) values.Add(U1);
            if (!double.IsNaN(U2)) values.Add(U2);
            if (!double.IsNaN(U3)) values.Add(U3);
            if (!double.IsNaN(UR1)) values.Add(UR1);
            if (!double.IsNaN(UR2)) values.Add(UR2);
            if (!double.IsNaN(UR3)) values.Add(UR3);
            return values.ToArray();
        }
    }
}