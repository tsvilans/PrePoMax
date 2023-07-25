using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class ResultPart : BasePart
    {
        public static bool Undeformed = true;


        // Variables                                                                                                                
        private bool _colorContours;
        private bool _createdFromBasePart;

        // Properties                                                                                                               
        public bool ColorContours { get { return _colorContours; } set { _colorContours = value; } }
        public bool CreatedFromBasePart { get { return _createdFromBasePart; } }

        // Constructors                                                                                                             
        public ResultPart(string name, int partId, int[] nodeLabels, int[] elementLabels, Type[] elementTypes)
            : base(name, partId, nodeLabels, elementLabels, elementTypes)
        {
            _colorContours = true;
            //
            _createdFromBasePart = false;
        }
        public ResultPart(BasePart part)
            : base(part)
        {
            _colorContours = true;
            //
            _createdFromBasePart = true;
        }
        public ResultPart(ResultPart part)
            : base((ResultPart)part)
        {
            _colorContours = part.ColorContours;
            _createdFromBasePart = part.CreatedFromBasePart;
        }


        // Methods                                                                                                                  

        public override BasePart DeepCopy()
        {
            return new ResultPart(this);
        }
        public override PartProperties GetProperties()
        {
            PartProperties properties = base.GetProperties();
            properties.ColorContours = _colorContours;
            return properties;
        }
        public override void SetProperties(PartProperties properties)
        {
            base.SetProperties(properties);
            _colorContours = properties.ColorContours;
        }
    }
}
