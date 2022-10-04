using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    public class EdgesVisibilitiesCollection
    {
        // Variables                                                                                                                
        [NonSerialized] private Controller _controller;
        private vtkControl.vtkEdgesVisibility _geometryEdgesVisibility;
        private vtkControl.vtkEdgesVisibility _modelEdgesVisibility;
        private Dictionary<string, vtkControl.vtkEdgesVisibility> _allResultsEdgesVisibilities;


        // Properties                                                                                                               
        public string CurrentResultName { get { return _controller.AllResults.GetCurrentResultName(); } }


        // Constructors                                                                                                             
        public EdgesVisibilitiesCollection(Controller controller)
        {
            _controller = controller;
            ClearModelEdgesVisibility();
            _allResultsEdgesVisibilities = new Dictionary<string, vtkControl.vtkEdgesVisibility>();
        }


        // Methods                                                                                                                  
        public vtkControl.vtkEdgesVisibility GetCurrentEdgesVisibility()
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) return _geometryEdgesVisibility;
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) return _modelEdgesVisibility;
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                string name = CurrentResultName;
                vtkControl.vtkEdgesVisibility ev;
                if (name != null && _allResultsEdgesVisibilities.TryGetValue(name, out ev)) return ev;
                else return vtkControl.vtkEdgesVisibility.ElementEdges;
            }
            else throw new NotSupportedException();
        }
        public void SetCurrentEdgesVisibility(vtkControl.vtkEdgesVisibility edgesVisibility)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) _geometryEdgesVisibility = edgesVisibility;
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) _modelEdgesVisibility = edgesVisibility;
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                string name = CurrentResultName;
                if (name != null)
                {
                    if (!_allResultsEdgesVisibilities.ContainsKey(name))
                        _allResultsEdgesVisibilities.Add(name, edgesVisibility);
                    else _allResultsEdgesVisibilities[name] = edgesVisibility;
                }
            }
            else throw new NotSupportedException();
        }
        // Clear
        public void ClearModelEdgesVisibility()
        {
            _geometryEdgesVisibility = vtkControl.vtkEdgesVisibility.ModelEdges;
            _modelEdgesVisibility = vtkControl.vtkEdgesVisibility.ElementEdges;
        }
        public void ClearAllResultsEdgesVisibility()
        {
            _allResultsEdgesVisibilities.Clear();
        }
        // Remove
        public void RemoveCurrentResultEdgesVisibility()
        {
            string name = CurrentResultName;
            if (name != null) _allResultsEdgesVisibilities.Remove(CurrentResultName);
        }
        
    }
}
