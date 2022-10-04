using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class FeElementSet : FeGroup
    {
        // Variables                                                                                                                
        private Selection _creationData;
        private int[] _creationIds;
        private bool _createdFromParts;


        // Properties                                                                                                               
        public Selection CreationData { get { return _creationData; } set { _creationData = value; } }
        public int[] CreationIds { get { return _creationIds; } set { _creationIds = value; } }
        public bool CreatedFromParts
        {
            get { return _createdFromParts; }
            //set { _createdFromOtherElementSets = value; } 
        }


        // Constructors                                                                                                             
        public FeElementSet(string name, int[] labels, bool createdFromParts = false)
            : base(name, labels)
        {
            _creationData = null;
            _creationIds = null;
            _createdFromParts = false;
            _createdFromParts = createdFromParts;
        }
        public FeElementSet(FeElementSet elementSet)
           : base(elementSet)
        {
            _creationData = elementSet.CreationData != null ? elementSet.CreationData.DeepClone() : null;
            _creationIds = elementSet.CreationIds != null ? elementSet.CreationIds.ToArray() : null;
            _createdFromParts = elementSet._createdFromParts;
        }


        // Methods                                                                                                                  
        public int[] GetMaxNRandomElementIds(int n)
        {
            if (Labels.Length < n) return Labels;
            else
            {
                List<int> notSelectedElements = new List<int>(Labels);
                int[] selectedElements = new int[n];
                Random rand = new Random();
                int id;
                for (int i = 0; i < n; i++)
                {
                    id = rand.Next(0, notSelectedElements.Count - 1);
                    selectedElements[i] = notSelectedElements[id];
                    notSelectedElements.RemoveAt(id);
                }
                return selectedElements.ToArray();
            }
        }
        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < _labels.Length; i++)
            {
                hash ^= _labels[i].GetHashCode();
            }
            return hash;
        }
    }
}
