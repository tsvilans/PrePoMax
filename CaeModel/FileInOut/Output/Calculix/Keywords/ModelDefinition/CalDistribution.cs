using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;

namespace FileInOut.Output.Calculix
{
    [Serializable]
    internal class CalDistribution : CalculixKeyword
    {
        // Variables                                                                                                                
        private FeDistribution _distribution;
        private FeModel _model;

        // Properties                                                                                                               


        // Constructor                                                                                                              
        public CalDistribution(FeDistribution distribution, FeModel model)
        {
            _distribution = distribution;
            _model = model;
        }


        // Methods                                                                                                                  
        public override string GetKeywordString()
        {
            if (_distribution.Name == null || _distribution.Name.Length < 1) _distribution.Name = "dist";
            //
            return string.Format("*Distribution, Name={0}{1}", _distribution.Name, Environment.NewLine);
        }
        public override string GetDataString()
        {
            StringBuilder sb = new StringBuilder();
            // Sort
            List<int> sortedLabels = _distribution.Labels.OrderBy(x => x).ToList();
            //
            FeMaterialOrientation _defaultOrientation = _distribution.DefaultMaterialOrientation;
            sb.AppendFormat(",{0}, {1}, {2}, {3}, {4}, {5}",
                _defaultOrientation.XX, _defaultOrientation.XY, _defaultOrientation.XZ,
                _defaultOrientation.YX, _defaultOrientation.YY, _defaultOrientation.YZ);
            sb.AppendLine();

            foreach (int id in sortedLabels)
            {
                var materialOrientation = _model.Mesh.ElementOrientations[id];
                sb.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}, {6}", materialOrientation.Id,
                    materialOrientation.XX, materialOrientation.XY, materialOrientation.XZ,
                    materialOrientation.YX, materialOrientation.YY, materialOrientation.YZ);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
