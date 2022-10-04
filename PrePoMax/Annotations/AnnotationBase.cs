using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace PrePoMax
{
    [Serializable]
    public abstract class AnnotationBase : NamedClass
    {
        // Variables                                                                                                                
        protected int _partId;
        protected string _overridenText;
        //
        [NonSerialized] public static Controller Controller;


        // Properties                                                                                                               
        public int PartId { get { return _partId; } set { _partId = value; } }
        public bool IsTextOverriden { get { return _overridenText != null; } }
        public string OverridenText { get { return _overridenText; } set { _overridenText = value; } }


        // Constructors                                                                                                             
        public AnnotationBase(string name)
            : base(name)
        {
            _partId = -1;   // always visible
            _overridenText = null;
        }


        // Methods                                                                                                                  
        public bool IsAnnotationVisible()
        {
            if (this.Visible)
            {
                if (_partId == -1) return true;
                //
                CaeMesh.FeMesh mesh = Controller.DisplayedMesh;
                if (mesh != null)
                {
                    CaeMesh.BasePart part = mesh.GetPartById(_partId);
                    if (part != null && part.Visible) return true;
                }
            }
            //
            return false;
        }
        //
        public abstract void GetAnnotationData(out string text, out double[] coor);
        public string GetAnnotationText()
        {
            GetAnnotationData(out string text, out double[] coor);
            return text;
        }
        public string GetNotOverridenAnnotationText()
        {
            string tmp = _overridenText;
            _overridenText = null;
            //
            GetAnnotationData(out string text, out double[] coor);
            //
            _overridenText = tmp;
            //
            return text;
        }
        //
        public void ApplyExplodedViewToPosition(Vec3D position)
        {
            if (Controller.IsExplodedViewActive())
            {
                CaeMesh.BasePart part = Controller.AllResults.CurrentResult.Mesh.GetPartById(_partId);
                position.X += part.Offset[0];
                position.Y += part.Offset[1];
                position.Z += part.Offset[2];
            }
        }

    }
}
