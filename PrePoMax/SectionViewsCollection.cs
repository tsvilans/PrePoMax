using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrePoMax
{
    [Serializable]
    public class SectionViewsCollection
    {
        // Variables                                                                                                                
        [NonSerialized] private Controller _controller;
        private Octree.Plane _geometrySectionViewPlane;
        private Octree.Plane _modelSectionViewPlane;
        private Dictionary<string, Octree.Plane> _allResultsSectionViewPlane;
        private Octree.Plane _plane;

        // Properties                                                                                                               
        public string CurrentResultName { get { return _controller.AllResults.GetCurrentResultName(); } }


        // Constructors                                                                                                             
        public SectionViewsCollection(Controller controller)
        {
            _controller = controller;
            ClearModelSectionViews();
            _allResultsSectionViewPlane = new Dictionary<string, Octree.Plane>();
        }


        // Methods                                                                                                                  
        public bool IsSectionViewActive()
        {
            return GetCurrentSectionViewPlane() != null;
        }
        public Octree.Plane GetCurrentSectionViewPlane()
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) return _geometrySectionViewPlane;
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) return _modelSectionViewPlane;
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                string name = CurrentResultName;
                if (name != null && _allResultsSectionViewPlane.TryGetValue(name, out _plane)) return _plane;
                else return null;
            }
            else throw new NotSupportedException();
        }
        public void SetCurrentSectionViewPlane(double[] point, double[] normal)
        {
            _plane = new Octree.Plane(point, normal);
            SetCurrentSectionViewPlane(_plane);
        }
        public void SetCurrentSectionViewPlane(Octree.Plane plane)
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) _geometrySectionViewPlane = plane;
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) _modelSectionViewPlane = plane;
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                string name = CurrentResultName;
                if (name != null)
                {
                    if (!_allResultsSectionViewPlane.ContainsKey(name))
                        _allResultsSectionViewPlane.Add(name, plane);
                    else _allResultsSectionViewPlane[name] = plane;
                }
            }
            else throw new NotSupportedException();
        }
        public void SetCurrentPointAndNormal(double[] point, double[] normal)
        {
            _plane = GetCurrentSectionViewPlane();
            if (_plane != null) _plane.SetPointAndNormal(point, normal);
        }
        // Clear
        public void ClearModelSectionViews()
        {
            _geometrySectionViewPlane = null;
            _modelSectionViewPlane = null;
        }
        public void ClearAllResultsSectionViews()
        {
            _allResultsSectionViewPlane.Clear();
        }
        // Remove
        public void RemoveCurrentSectionView()
        {
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) _geometrySectionViewPlane = null;
            else if (_controller.CurrentView == ViewGeometryModelResults.Model) _modelSectionViewPlane = null;
            else if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                string name = CurrentResultName;
                if (name != null) _allResultsSectionViewPlane.Remove(CurrentResultName);
            }
            else throw new NotSupportedException();
        }
        
    }
}
