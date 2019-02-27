﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalMomentLoad : CalculixKeyword
    {
        // Variables                                                                                                                
        private MomentLoad _load;
        private Dictionary<string, int[]> _referencePointsNodeIds;

        // Properties                                                                                                               
        public override object BaseItem { get { return _load; } }


        // Events                                                                                                                   


        // Constructor                                                                                                              
        public CalMomentLoad(MomentLoad load, Dictionary<string, int[]> referencePointsNodeIds)
        {
            _load = load;
            _referencePointsNodeIds = referencePointsNodeIds;
            _active = load.Active;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("** Name: " + _load.Name);
            sb.AppendLine("*Cload");
            return sb.ToString();
        }

        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();

            int[] rpNodeIds = null;
            if (_load.RegionType == CaeGlobals.RegionTypeEnum.ReferencePointName) rpNodeIds = _referencePointsNodeIds[_load.RegionName];

            List<int> directions = new List<int>();
            if (_load.M1 != 0) directions.Add(1);
            if (_load.M2 != 0) directions.Add(2);
            if (_load.M3 != 0) directions.Add(3);

            foreach (var dir in directions)
            {
                if (_load.RegionType == CaeGlobals.RegionTypeEnum.NodeId)
                    sb.AppendFormat("{0}, {1}, {2}", _load.NodeId, dir, _load.GetDirection(dir - 1).ToString());
                else if (_load.RegionType == CaeGlobals.RegionTypeEnum.NodeSetName) // node set
                    sb.AppendFormat("{0}, {1}, {2}", _load.RegionName, dir, _load.GetDirection(dir - 1).ToString());
                else if (_load.RegionType == CaeGlobals.RegionTypeEnum.ReferencePointName) // reference point
                    sb.AppendFormat("{0}, {1}, {2}", rpNodeIds[1], dir, _load.GetDirection(dir - 1).ToString());

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
