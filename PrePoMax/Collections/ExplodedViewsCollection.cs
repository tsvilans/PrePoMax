using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    public class ExplodedViewsCollection
    {
        // Variables                                                                                                                
        [NonSerialized] private Controller _controller;
        private ExplodedViewParameters _geometryExplodedViewParameters;
        private ExplodedViewParameters _modelExplodedViewParameters;
        private Dictionary<string, ExplodedViewParameters> _allResultsExplodedViewParameters;


        // Properties                                                                                                               
        public string CurrentResultName { get { return _controller.AllResults.GetCurrentResultName(); } }


        // Constructors                                                                                                             
        public ExplodedViewsCollection(Controller controller)
        {
            _controller = controller;
            ClearModelExplodedViews();
            _allResultsExplodedViewParameters = new Dictionary<string, ExplodedViewParameters>();
        }


        // Methods                                                                                                                  
        public bool IsExplodedViewActive()
        {
            return GetCurrentExplodedViewParametersNull() != null;
        }
        public bool IsGeometryExplodedViewActive()
        {
            return _geometryExplodedViewParameters != null;
        }
        public bool IsModelExplodedViewActive()
        {
            return _modelExplodedViewParameters != null;
        }
        public bool IsResultExplodedViewActive(string name)
        {
            ExplodedViewParameters parameters;
            _allResultsExplodedViewParameters.TryGetValue(name, out parameters);
            return parameters != null;
        }
        public ExplodedViewParameters GetResultExplodedViewParameters(string name)
        {
            ExplodedViewParameters parameters;
            _allResultsExplodedViewParameters.TryGetValue(name, out parameters);
            return parameters;
        }
        public ExplodedViewParameters GetCurrentExplodedViewParameters()
        {
            ExplodedViewParameters parameters = GetCurrentExplodedViewParametersNull();
            if (parameters == null) parameters = new ExplodedViewParameters();
            return parameters;
        }
        private ExplodedViewParameters GetCurrentExplodedViewParametersNull()
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) return _geometryExplodedViewParameters;
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) return _modelExplodedViewParameters;
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                string name = CurrentResultName;
                ExplodedViewParameters parameters;
                if (name != null && _allResultsExplodedViewParameters.TryGetValue(name, out parameters)) return parameters;
                else return null;
            }
            else throw new NotSupportedException();
        }
        public void SetCurrentExplodedViewParameters(ExplodedViewParameters parameters)
        {
            ExplodedViewParameters evp;
            if (parameters.ScaleFactor == -1) evp = null;
            else evp = parameters;
            //
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) _geometryExplodedViewParameters = evp;
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) _modelExplodedViewParameters = evp;
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                string name = CurrentResultName;
                if (name != null)
                {
                    if (!_allResultsExplodedViewParameters.ContainsKey(name))
                        _allResultsExplodedViewParameters.Add(name, evp);
                    else _allResultsExplodedViewParameters[name] = evp;
                }
            }
            else throw new NotSupportedException();
        }


        // Clear
        public void ClearModelExplodedViews()
        {
            _geometryExplodedViewParameters = null;
            _modelExplodedViewParameters = null;
        }
        public void ClearAllResultsExplodedViews()
        {
            _allResultsExplodedViewParameters.Clear();
        }
        // Remove
        public void RemoveCurrentExplodedView()
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) _geometryExplodedViewParameters = null;
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) _modelExplodedViewParameters = null;
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                string name = CurrentResultName;
                if (name != null) _allResultsExplodedViewParameters.Remove(CurrentResultName);
            }
            else throw new NotSupportedException();
        }
    }
}
