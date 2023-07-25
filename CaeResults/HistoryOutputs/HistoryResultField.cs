using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;

namespace CaeResults
{
    [Serializable]
    public class HistoryResultField : NamedClass, ISerializable
    {
        // Variables                                                                                                                
        protected OrderedDictionary<string, HistoryResultComponent> _components;        //ISerializable


        // Properties                                                                                                               
        public OrderedDictionary<string, HistoryResultComponent> Components
        {
            get { return _components; }
            set { _components = value; }
        }
        

        // Constructor                                                                                                              
        public HistoryResultField(string name)
            : base()
        {
            _checkName = false;
            _name = name;
            _components = new OrderedDictionary<string, HistoryResultComponent>("Components");
        }
        //ISerializable
        public HistoryResultField(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_components":
                        // Compatibility v1.3.5
                        if (entry.Value is Dictionary<string, HistoryResultComponent> oldComponents)
                        {
                            _components = new OrderedDictionary<string, HistoryResultComponent>("Components");
                            _components.AddRange(oldComponents);
                        }
                        else _components = (OrderedDictionary<string, HistoryResultComponent>)entry.Value;
                        break;
                }
            }
        }


        // Static methods                                                                                                           


        // Methods                                                                                                                  
        public void AppendComponents(HistoryResultField historyResultField)
        {
            HistoryResultComponent component;
            foreach (var entry in historyResultField.Components)
            {
                if (_components.TryGetValue(entry.Key, out component)) throw new NotSupportedException();
                else _components.Add(entry.Key, entry.Value);
            }
        }

        // ISerialization
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            base.GetObjectData(info, context);
            //
            info.AddValue("_components", _components, typeof(OrderedDictionary<string, HistoryResultComponent>));
        }

    }
}
