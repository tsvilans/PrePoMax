using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrePoMax;
using CaeModel;
using CaeMesh;
using CaeGlobals;
using Calculix = FileInOut.Output.Calculix;
using System.Runtime.Serialization;


namespace PrePoMax.Commands
{
    [Serializable]
    class CSetCalculixUserKeywords : Command, ISerializable
    {
        // Variables                                                                                                                
        private OrderedDictionary<int[], Calculix.CalculixUserKeyword> _userKeywords;   //ISerializable


        // Constructor                                                                                                              
        public CSetCalculixUserKeywords(OrderedDictionary<int[], Calculix.CalculixUserKeyword> userKeywords)
            : base("Set CalculiX user keywords")
        {
            _userKeywords = userKeywords.DeepClone();
        }
        public CSetCalculixUserKeywords(SerializationInfo info, StreamingContext context)
            : base("") // this can be empty
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "Command+_name":
                        _name = (string)entry.Value; break;
                    case "Command+_dateCreated":
                        _dateCreated = (DateTime)entry.Value; break;
                    case "_userKeywords":
                        if (entry.Value is Dictionary<int[], Calculix.CalculixUserKeyword> cukd)
                        {
                            // Compatibility for version v.0.5.1
                            cukd.OnDeserialization(null);
                            _userKeywords = new OrderedDictionary<int[], Calculix.CalculixUserKeyword>("User CalculiX keywords", cukd);
                        }
                        else if (entry.Value is OrderedDictionary<int[], Calculix.CalculixUserKeyword> cukod)
                            _userKeywords = cukod;
                        else if (entry.Value == null) _userKeywords = null;
                        else throw new NotSupportedException();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }


        // Methods                                                                                                                  
        public override bool Execute(Controller receiver)
        {
            receiver.SetCalculixUserKeywords(_userKeywords.DeepClone());
            return true;
        }

        public override string GetCommandString()
        {
            return base.GetCommandString() + _userKeywords.Count;
        }

        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            info.AddValue("Command+_name", _name, typeof(string));
            info.AddValue("Command+_dateCreated", _dateCreated, typeof(DateTime));
            info.AddValue("_userKeywords", _userKeywords, typeof(OrderedDictionary<int[], Calculix.CalculixUserKeyword>));
        }
    }
}
