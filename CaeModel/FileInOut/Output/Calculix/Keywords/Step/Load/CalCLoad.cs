﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;
using CaeGlobals;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalCLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private CLoad _load;
        private Dictionary<string, int[]> _referencePointsNodeIds;


        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalCLoad(CLoad load, Dictionary<string, int[]> referencePointsNodeIds)
        {
            _load = load;
            _referencePointsNodeIds = referencePointsNodeIds;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _load.Name);
            string amplitude = "";
            if (_load.AmplitudeName != Load.DefaultAmplitudeName) amplitude = ", Amplitude=" + _load.AmplitudeName;
            //
            sb.AppendFormat("*Cload{0}{1}", amplitude, Environment.NewLine);
            //
            return sb.ToString();
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            //
            int[] rpNodeIds = null;
            if (_load.RegionType == RegionTypeEnum.ReferencePointName) rpNodeIds = _referencePointsNodeIds[_load.RegionName];
            //
            List<int> directions = new List<int>();
            if (_load.F1 != 0) directions.Add(1);
            if (_load.F2 != 0) directions.Add(2);
            if (_load.F3 != 0) directions.Add(3);
            //
            foreach (var dir in directions)
            {
                if (_load.RegionType == RegionTypeEnum.NodeId)
                    sb.AppendFormat("{0}, {1}, {2}", _load.NodeId, dir, _load.GetDirection(dir - 1).ToCalculiX16String());
                else if (_load.RegionType == CaeGlobals.RegionTypeEnum.NodeSetName) // node set
                    sb.AppendFormat("{0}, {1}, {2}", _load.RegionName, dir, _load.GetDirection(dir - 1).ToCalculiX16String());
                else if (_load.RegionType == CaeGlobals.RegionTypeEnum.ReferencePointName) // reference point
                    sb.AppendFormat("{0}, {1}, {2}", rpNodeIds[0], dir, _load.GetDirection(dir - 1).ToCalculiX16String());
                //
                sb.AppendLine();
            }
            //
            return sb.ToString();
        }
    }
}
