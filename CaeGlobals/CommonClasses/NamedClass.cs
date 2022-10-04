using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.Serialization;


namespace CaeGlobals
{
    [Serializable]
    public abstract class NamedClass //: ISerializable - this would mean that all derived classes must be Serializable !!!
    {
        // Variables                                                                                                                
        protected string _name;             //ISerializable
        protected bool _active;             //ISerializable
        protected bool _visible;            //ISerializable
        protected bool _valid;              //ISerializable
        protected bool _internal;           //ISerializable
        protected bool _checkName;          //ISerializable


        // Properties                                                                                                               
        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (_checkName) CheckNameForErrors(ref value);
                _name = value;
            }
        }
        public virtual bool Active { get { return _active; } set { _active = value; } }
        public virtual bool Visible { get { return _visible; } set { _visible = value; } }
        public virtual bool Valid { get { return _valid; }
            set { _valid = value; } }
        public virtual bool Internal { get { return _internal; } set { _internal = value; } }


        // Constructors                                                                                                             
        public NamedClass()
            : this("NoName")
        {
        }
        public NamedClass(string name)
        {
            _checkName = true;
            Name = name;
            //
            _active = true;
            _visible = true;
            _valid = true;
            _internal = false;
        }
        public NamedClass(NamedClass namedClass)
        {
            _name = namedClass._name;
            _active = namedClass._active;
            _visible = namedClass._visible;
            _valid = namedClass._valid;
            _internal = namedClass._internal;
            _checkName = namedClass._checkName;
        }
        public NamedClass(SerializationInfo info, StreamingContext context)
        {
            int count = 0;
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_name":
                        _name = (string)entry.Value; count++; break;
                    case "_active":
                        _active = (bool)entry.Value; count++; break;
                    case "_visible":
                        _visible = (bool)entry.Value; count++; break;
                    case "_valid":
                        _valid = (bool)entry.Value; count++; break;
                    case "_internal":
                        _internal = (bool)entry.Value; count++; break;
                    case "_checkName":
                        _checkName = (bool)entry.Value; count++; break;
                }
            }
            if (count != 6) throw new NotSupportedException();
        }


        // Static methods
        public static string GetNewValueName1(ICollection<string> existingNames, string nameRoot, char splitter = '-')
        {
            int max = 0;
            int tmp;
            string[] parts;
            foreach (var names in existingNames)
            {
                parts = names.Split(splitter);
                if (parts.Length >= 2 && nameRoot.StartsWith(parts[0]))
                {
                    if (int.TryParse(parts.Last(), out tmp))
                    {
                        if (tmp > max) max = tmp;
                    }
                }
            }
            max++;
            //
            return nameRoot + max.ToString();
        }
        public static string GetNameWithoutLastValue(string name, char splitter = '-')
        {
            int tmp;
            string[] parts;
            parts = name.Split(splitter);
            int numOfParts = parts.Length;
            //
            if (int.TryParse(parts.Last(), out tmp)) numOfParts--;
            //
            string newName = "";
            for (int i = 0; i < numOfParts; i++)
            {
                newName += parts[i];
                if (i < numOfParts - 1) newName += splitter;
            }
            return newName;
        }


        // Methods                                                                                                                  
        public static bool CheckName(string name)
        {
            try
            {
                CheckNameForErrors(ref name);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string CheckNameError(string name)
        {
            try
            {
                CheckNameForErrors(ref name);
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static void CheckNameForErrors(ref string name)
        {
            if (name == null) throw new CaeException("The name can not be null.");
            if (name == "") throw new CaeException("The name can not be an empty string.");
            // Trim spaces
            name = name.Trim();
            //
            if (name.Contains(' ')) throw new CaeException("The name can not contain space characters: '" + name + "'.");
            if (name == "Missing") throw new CaeException("The name 'Missing' is a reserved name.");
            //
            char c;
            int letterCount = 0;
            int digitCount = 0;
            //
            for (int i = 0; i < name.Length; i++)
            {
                c = name[i];
                if (char.IsLetter(c)) letterCount++;
                else if (char.IsDigit(c)) digitCount++;
                else if (c != '_' && c != '-' && c != '(' && c != ')')
                    throw new CaeException("The name can only contain a letter, a digit or characters: minus, " + 
                                           "underscore and parenthesis: '" + name + "'.");
            }
            //
            if (letterCount <= 0)
                throw new CaeException("The name must contain at least one letter: '" + name + "'.");
        }
        public static string GetErrorFreeName(string name)
        {
            name = name.Replace(' ', '_');
            byte[] bytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(name);
            name = Encoding.UTF8.GetString(bytes);
            //
            string newName = "";
            for (int i = 0; i < name.Length; i++)
            {
                if (CheckName(newName + name[i])) newName += name[i];
                else newName += "_";
            }
            return newName;
        }
        //
        public override string ToString()
        {
            return _name;
        }

        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            info.AddValue("_name", _name, typeof(string));
            info.AddValue("_active", _active, typeof(bool));
            info.AddValue("_visible", _visible, typeof(bool));
            info.AddValue("_valid", _valid, typeof(bool));
            info.AddValue("_internal", _internal, typeof(bool));
            info.AddValue("_checkName", _checkName, typeof(bool));
        }
    }
}
