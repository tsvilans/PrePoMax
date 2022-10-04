using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using CaeMesh;

namespace PrePoMax
{
    [Serializable]
    public class AnnotationContainer
    {
        // Variables                                                                                                                
        private bool _suppressed;                                           // used only for min/max annotations
        public static string MeasureAnnotationName = "Measure_Annotation";
        public static string MinAnnotationName = "Min_Annotation";          // there is a copy of this in vtkControl.Globals
        public static string MaxAnnotationName = "Max_Annotation";          // there is a copy of this in vtkControl.Globals
        //
        private Dictionary<string, AnnotationBase> _geometryAnnotations;
        private Dictionary<string, AnnotationBase> _modelAnnotations;
        private Dictionary<string, Dictionary<string, AnnotationBase>> _allResultsAnnotations;

        // Properties                                                                                                               
        public string CurrentResultName { get { return AnnotationBase.Controller.AllResults.GetCurrentResultName(); } }


        // Constructors                                                                                                             
        public AnnotationContainer(Controller controller)
        {
            AnnotationBase.Controller = controller;
            //
            _suppressed = false;
            //
            _geometryAnnotations = new Dictionary<string, AnnotationBase>();
            _modelAnnotations = new Dictionary<string, AnnotationBase>();
            _allResultsAnnotations = new Dictionary<string, Dictionary<string, AnnotationBase>>();
        }
        public AnnotationContainer(AnnotationContainer annotationContainer, Controller controller)
        {
            AnnotationBase.Controller = controller;
            //
            _suppressed = false;
            //
            if (annotationContainer == null)    // compatibility v1.3.1
            {
                _geometryAnnotations = new Dictionary<string, AnnotationBase>();
                _modelAnnotations = new Dictionary<string, AnnotationBase>();
                _allResultsAnnotations = new Dictionary<string, Dictionary<string, AnnotationBase>>();
            }
            else
            {
                _geometryAnnotations = annotationContainer._geometryAnnotations;
                _modelAnnotations = annotationContainer._modelAnnotations;
                _allResultsAnnotations = annotationContainer._allResultsAnnotations;
                if (_allResultsAnnotations == null)
                    _allResultsAnnotations = new Dictionary<string, Dictionary<string, AnnotationBase>>(); // compatibility v1.3.4
            }
        }


        // Methods                                                                                                                  
        public static bool IsAnnotationNameReserved(string name)
        {
            if (name == MeasureAnnotationName || name == MinAnnotationName || name == MaxAnnotationName) return true;
            else return false;
        }
        public string GetFreeAnnotationName()
        {
            string prefix = "";
            if (AnnotationBase.Controller.CurrentView == ViewGeometryModelResults.Geometry) prefix = "G_";
            else if (AnnotationBase.Controller.CurrentView == ViewGeometryModelResults.Model) prefix = "M_";
            else if (AnnotationBase.Controller.CurrentView == ViewGeometryModelResults.Results) prefix = "R_";
            else throw new NotSupportedException();
            //
            return GetCurrentAnnotations().GetNextNumberedKey(prefix + "Annotation");
        }
        public string[] GetCurrentAnnotationNames()
        {
            return GetCurrentAnnotations().Keys.ToArray();
        }
        public string[] GetAllAnnotationNames()
        {
            List<string> allNames = new List<string>();
            allNames.AddRange(_geometryAnnotations.Keys);
            allNames.AddRange(_modelAnnotations.Keys);
            foreach (var entry in _allResultsAnnotations) allNames.AddRange(entry.Value.Keys);
            return allNames.ToArray();
        }
        //
        public void AddAnnotation(AnnotationBase annotation)
        {
            GetCurrentAnnotations().Add(annotation.Name, annotation);
            AnnotationBase.Controller.ModelChanged = true;
            //
            DrawAnnotations();
        }
        public void AddNodeAnnotation(int nodeId)
        {
            AddAnnotation(new NodeAnnotation(GetFreeAnnotationName(), nodeId));
        }
        public void AddElementAnnotation(int elementId)
        {
            AddAnnotation(new ElementAnnotation(GetFreeAnnotationName(), elementId));
        }
        public void AddEdgeAnnotation(int geometryId)
        {
            AddAnnotation(new EdgeAnnotation(GetFreeAnnotationName(), geometryId));
        }
        public void AddSurfaceAnnotation(int geometryId)
        {
            AddAnnotation(new SurfaceAnnotation(GetFreeAnnotationName(), geometryId));
        }
        public void AddPartAnnotation(string partName)
        {
            AddAnnotation(new PartAnnotation(GetFreeAnnotationName(), partName));
        }
        public void AddMeasureAnnotation(string text, double[] anchor)
        {
            AddAnnotation(new TextAnnotation(MeasureAnnotationName, text, anchor));
        }
        //
        public AnnotationBase GetCurrentAnnotation(string name)
        {
            if (GetCurrentAnnotations().TryGetValue(name, out AnnotationBase annotation)) return annotation;
            else return null;
        }
        public Dictionary<string, AnnotationBase> GetCurrentAnnotations()
        {
            return GetAnnotations(AnnotationBase.Controller.CurrentView, CurrentResultName);
        }
        public Dictionary<string, AnnotationBase> GetAnnotations(ViewGeometryModelResults view, string resultName)
        {
            if (view == ViewGeometryModelResults.Geometry) return _geometryAnnotations;
            else if (view == ViewGeometryModelResults.Model) return _modelAnnotations;
            else if (view == ViewGeometryModelResults.Results && resultName != null)
            {
                if (!_allResultsAnnotations.ContainsKey(resultName))
                    _allResultsAnnotations.Add(resultName, new Dictionary<string, AnnotationBase>());
                return _allResultsAnnotations[resultName];
            }
            else throw new NotSupportedException();
        }
        //
        public string GetAnnotationText(string data)
        {
            int itemId;
            string text;
            AnnotationBase annotation;
            //
            int.TryParse(data, out itemId);
            //
            if (AnnotationBase.Controller.SelectBy == vtkSelectBy.QueryNode)
            {
                annotation = new NodeAnnotation("tmp", itemId);
            }
            else if (AnnotationBase.Controller.SelectBy == vtkSelectBy.QueryElement)
            {
                annotation = new ElementAnnotation("tmp", itemId);
            }
            else if (AnnotationBase.Controller.SelectBy == vtkSelectBy.QueryEdge)
            {
                annotation = new EdgeAnnotation("tmp", itemId);
            }
            else if (AnnotationBase.Controller.SelectBy == vtkSelectBy.QuerySurface)
            {
                annotation = new SurfaceAnnotation("tmp", itemId);
            }
            else if (AnnotationBase.Controller.SelectBy == vtkSelectBy.QueryPart)
            {
                annotation = new PartAnnotation("tmp", data);
            }
            else throw new NotSupportedException();
            //
            annotation.GetAnnotationData(out text, out double[] coor);
            //
            return text;
        }
        //
        public void ResetAnnotation(string annotationName)
        {
            AnnotationBase annotation = GetCurrentAnnotation(annotationName);
            if (annotation != null)
            {
                annotation.OverridenText = null;
                AnnotationBase.Controller.ModelChanged = true;
                //
                DrawAnnotations();
            }
        }
        public void SuppressCurrentAnnotations()
        {
            _suppressed = true;
            //
            RemoveCurrentMeasureAnnotation();   // measure annotations cannot be suppressed
            //
            foreach (var entry in GetCurrentAnnotations()) entry.Value.Visible = false;
            //
            DrawAnnotations();
        }
        public void ResumeCurrentAnnotations()
        {
            _suppressed = false;
            //
            foreach (var entry in GetCurrentAnnotations()) entry.Value.Visible = true;
            //
            DrawAnnotations();
        }
        // Draw
        public void DrawAnnotations(bool minMaxOnly = false)
        {
            string text;
            double[] arrowCoor;
            //
            bool drawBackground = AnnotationBase.Controller.Settings.Annotations.BackgroundType == AnnotationBackgroundType.White;
            bool drawBorder = AnnotationBase.Controller.Settings.Annotations.DrawBorder;
            string numberFormat = AnnotationBase.Controller.Settings.Annotations.GetNumberFormat();
            //
            if (!minMaxOnly)
            {
                AnnotationBase annotation;
                List<string> invalidAnnotationNames = new List<string>();
                foreach (var entry in GetCurrentAnnotations())
                {
                    try // some annotation might become unvalid
                    {
                        annotation = entry.Value;
                        // First get the annotation data to determine if the annotation is valid
                        annotation.GetAnnotationData(out text, out arrowCoor);
                        AnnotationBase.Controller.Form.AddArrowWidget(entry.Value.Name, text, numberFormat, arrowCoor, drawBackground,
                                                                      drawBorder, annotation.IsAnnotationVisible());

                    }
                    catch
                    {
                        invalidAnnotationNames.Add(entry.Key);
                    }
                }
                // Remove invalid annotations
                if (invalidAnnotationNames.Count > 0)
                {
                    RemoveCurrentArrowAnnotations(invalidAnnotationNames.ToArray());
                    AnnotationBase.Controller.Form.RemoveArrowWidgets(invalidAnnotationNames.ToArray());
                }
            }
            // Min/Max annotations
            if (AnnotationBase.Controller.CurrentView == ViewGeometryModelResults.Results)
            {
                bool showMin = AnnotationBase.Controller.Settings.Post.ShowMinValueLocation && !_suppressed;
                bool showMax = AnnotationBase.Controller.Settings.Post.ShowMaxValueLocation && !_suppressed;
                //
                AnnotationBase.Controller.Form.AddArrowWidget(MinAnnotationName, "", numberFormat, new double[3],
                                                                drawBackground, drawBorder, showMin);
                AnnotationBase.Controller.Form.AddArrowWidget(MaxAnnotationName, "", numberFormat, new double[3],
                                                                drawBackground, drawBorder, showMax);
            }
        }
        // Remove
        public void RemoveCurrentArrowAnnotation(string annotationName)
        {
            AnnotationBase.Controller.Form.RemoveArrowWidgets(new string[] { annotationName });
            RemoveCurrentArrowAnnotations(new string[] { annotationName });
        }
        public void RemoveCurrentMeasureAnnotation()
        {
            RemoveCurrentArrowAnnotation(MeasureAnnotationName);
        }
        public void RemoveCurrentArrowAnnotations(string[] annotationNames)
        {
            var annotations = GetCurrentAnnotations();
            foreach (var annotationName in annotationNames) annotations.Remove(annotationName);
            AnnotationBase.Controller.ModelChanged = true;
        }
        public void RemoveCurrentArrowAnnotationsByParts(BasePart[] parts, ViewGeometryModelResults view)
        {
            FeMesh mesh;
            if (view == ViewGeometryModelResults.Geometry) mesh = AnnotationBase.Controller.Model.Geometry;
            else if (view == ViewGeometryModelResults.Model) mesh = AnnotationBase.Controller.Model.Mesh;
            else if (view == ViewGeometryModelResults.Results) mesh = AnnotationBase.Controller.CurrentResult.Mesh;
            else throw new NotSupportedException();
            //
            Dictionary<string, AnnotationBase> annotations;
            HashSet<int> partIds = new HashSet<int>();
            if (mesh != null)
            {
                foreach (var part in parts) partIds.Add(part.PartId);
                //
                annotations = GetAnnotations(view, CurrentResultName);
                List<AnnotationBase> annotationsToRemove = new List<AnnotationBase>();
                foreach (var entry in annotations)
                {
                    if (partIds.Contains(entry.Value.PartId)) annotationsToRemove.Add(entry.Value);
                }
                // Remove
                foreach (var annotation in annotationsToRemove) annotations.Remove(annotation.Name);
                //
                AnnotationBase.Controller.ModelChanged = true;
            }
        }
        public void RemoveCurrentArrowAnnotationsByParts(string[] partNames, ViewGeometryModelResults view)
        {
            FeMesh mesh;
            if (view == ViewGeometryModelResults.Geometry) mesh = AnnotationBase.Controller.Model.Geometry;
            else if (view == ViewGeometryModelResults.Model) mesh = AnnotationBase.Controller.Model.Mesh;
            else if (view == ViewGeometryModelResults.Results) mesh = AnnotationBase.Controller.CurrentResult.Mesh;
            else throw new NotSupportedException();
            //
            BasePart part;
            List<BasePart> parts = new List<BasePart>();
            foreach (var name in partNames)
            {
                if (mesh.Parts.TryGetValue(name, out part)) parts.Add(part);
            }
            //
            RemoveCurrentArrowAnnotationsByParts(parts.ToArray(), view);
        }
        public void RemoveCurrentArrowAnnotations()
        {
            var annotations = GetCurrentAnnotations();
            AnnotationBase.Controller.Form.RemoveArrowWidgets(annotations.Keys.ToArray());
            annotations.Clear();
            //
            AnnotationBase.Controller.ModelChanged = true;
        }
        public void RemoveCurrentResultArrowAnnotations()
        {
            Dictionary<string, AnnotationBase> resultAnnotations;
            if (_allResultsAnnotations.TryGetValue(CurrentResultName, out resultAnnotations))
            {
                AnnotationBase.Controller.Form.RemoveArrowWidgets(resultAnnotations.Keys.ToArray());
                //
                _allResultsAnnotations.Remove(CurrentResultName);
                //
                AnnotationBase.Controller.ModelChanged = true;
            }
        }
        public void RemoveAllResultArrowAnnotations()
        {
            _allResultsAnnotations.Clear();
            //
            AnnotationBase.Controller.ModelChanged = true;
        }
        public void RemoveAllArrowAnnotations()
        {
            _geometryAnnotations.Clear();
            _modelAnnotations.Clear();
            _allResultsAnnotations.Clear();
            //
            AnnotationBase.Controller.ModelChanged = true;
        }
    }
}
