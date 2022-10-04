using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeResults;

namespace PrePoMax
{
    [Serializable]
    public class TransformationsCollection
    {
        // Variables                                                                                                                
        [NonSerialized] private Controller _controller;
        private Dictionary<string, List<Transformation>> _allTransformations;


        // Properties                                                                                                               
        public string CurrentResultName { get { return _controller.AllResults.GetCurrentResultName(); } }


        // Constructors                                                                                                             
        public TransformationsCollection(Controller controller)
        {
            _controller = controller;
            _allTransformations = new Dictionary<string, List<Transformation>>();
        }

        // Methods                                                                                                                  
        public bool AreTransformationsActive()
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                List<Transformation> transformations = GetCurrentTransformations();
                if (transformations != null && transformations.Count > 0) return true;
            }
            return false;
        }
        public List<Transformation> GetCurrentTransformations()
        {
            List<Transformation> transformations = null;
            string name = CurrentResultName;
            if (name != null) _allTransformations.TryGetValue(name, out transformations);
            return transformations;
        }
        public void SetCurrentTransformations(List<Transformation> transformations)
        {
            string name = CurrentResultName;
            if (name != null)
            {
                if (_allTransformations.ContainsKey(name)) _allTransformations[name] = transformations;
                else _allTransformations.Add(name, transformations);
            }
        }
        // Clear

        // Remove
        public void RemoveCurrentTransformations()
        {
            string name = CurrentResultName;
            if (name != null)
            {
                if (_allTransformations.ContainsKey(name)) _allTransformations.Remove(name);
            }
        }
    }
}
