using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using System.Xml.Linq;
using System.Numerics;
using System.Diagnostics.Eventing.Reader;

namespace CaeResults
{
    [Serializable]
    public enum DataTypeEnum
    {
        None,
        Scalar,
        Vector,
        Tensor
    }

    [Serializable]
    public enum DataStateEnum
    {
        OK,
        UpdateComplexMinMax,
        UpdateResultFieldOutput
    }


    [Serializable]
    public struct IDValuePair
    {
        public int Id;
        public float Value;
    }

    [Serializable]
    public class Field : NamedClass
    {
        // Variables                                                                                                                
        private OrderedDictionary<string,  FieldComponent> _components;
        private bool _complex;
        private DataStateEnum _dataState;


        // Properties                                                                                                               
        public bool Complex { get { return _complex; } set { _complex = value; } }
        public DataStateEnum DataState { get { return _dataState; } set { _dataState = value; } }


        // Constructor                                                                                                              
        public Field(string name)
            : base(name)
        {
            _components = new OrderedDictionary<string, FieldComponent>("Components");
            //
            _complex = false;
            _dataState = DataStateEnum.OK;
        }
        public Field(Field field)
            : base(field)
        {
            _components = new OrderedDictionary<string, FieldComponent>("Components");
            foreach (var entry in field._components) _components.Add(entry.Key, new FieldComponent(entry.Value));   // copy
            //
            _complex = field._complex;
            _dataState = field._dataState;
        }


        // Static methods                                                                                                           
        public static void WriteToFile(Field field, System.IO.BinaryWriter bw)
        {
            if (field == null)
            {
                bw.Write((int)0);
            }
            else
            {
                bw.Write((int)1);
                // Name
                bw.Write(field._name);
                // Components
                if (field._components == null) bw.Write((int)0);
                else
                {
                    bw.Write((int)1);
                    //
                    bw.Write(field._components.Count);
                    foreach (var entry in field._components) FieldComponent.WriteToFile(entry.Value, bw);
                }
                //
                bw.Write(field.Complex);
                bw.Write((int)field.DataState);
                //
                bw.Write(field.Active);
                bw.Write(field.Visible);
                bw.Write(field.Valid);
                bw.Write(field.Internal);
            }
        }
        public static Field ReadFromFile(System.IO.BinaryReader br, int version)
        {
            int numItems;
            FieldComponent component;
            //
            int exists = br.ReadInt32();
            if (exists == 1)
            {
                string name = br.ReadString();
                Field field = new Field(name);
                // Components
                exists = br.ReadInt32();
                if (exists == 1)
                {
                    numItems = br.ReadInt32();
                    //
                    field._components = new OrderedDictionary<string, FieldComponent>("Components");
                    for (int i = 0; i < numItems; i++)
                    {
                        component = FieldComponent.ReadFromFile(br);
                        field._components.Add(component.Name, component);
                    }
                }
                //
                if (version >= 1_004_000)
                {
                    field.Complex = br.ReadBoolean();
                    field.DataState = (DataStateEnum)br.ReadInt32();
                    //
                    field.Active = br.ReadBoolean();
                    field.Visible = br.ReadBoolean();
                    field.Valid = br.ReadBoolean();
                    field.Internal = br.ReadBoolean();
                }
                return field;
            }
            return null;
        }


        // Methods                                                                                                                  
        public void AddComponent(FieldComponent component)
        {
            _components.Add(component.Name, component);
        }
        public void AddComponent(string name, float[] values, bool invariant = false)
        {
            _components.Add(name, new FieldComponent(name, values, invariant));
        }
        public string[] GetComponentNames()
        {
            return _components.Keys.ToArray();
        }
        public bool ContainsComponent(string name)
        {
            return _components.ContainsKey(name);
        }
        public bool IsComponentInvariant(string name)
        {
            return _components[name].Invariant;
        }
        public float[] GetComponentValues(string name)
        {
            if (_components.ContainsKey(name)) return _components[name].Values;
            else return null;
        }
        public float GetComponentAbsMax(string name)
        {
            float max = Math.Abs(_components[name].Max.Value);
            float min = Math.Abs(_components[name].Min.Value);
            return Math.Max(min, max);
        }
        public float GetComponentMax(string name)
        {
            return _components[name].Max.Value;
        }
        public int GetComponentMaxId(string name)
        {
            return _components[name].Max.Id;
        }
        public float GetComponentMin(string name)
        {
            return _components[name].Min.Value;
        }
        public int GetComponentMinId(string name)
        {
            return _components[name].Min.Id;
        }
        public void ReplaceComponent(string name, FieldComponent newComponent)
        {
            _components.Replace(name, name, newComponent);
        }
        public FieldComponent RemoveComponent(string name)
        {
            FieldComponent removedComponent;
            _components.TryRemove(name, out removedComponent);
            return removedComponent;
        }
        //
        public void RemoveInvariants()
        {
            DataTypeEnum dataType = FOFieldNames.GetDataType(_name);
            //
            if (dataType == DataTypeEnum.Vector)
            {
                _components.Remove(FOComponentNames.All);
            }
            else if (dataType == DataTypeEnum.Tensor)
            {
                _components.Remove(FOComponentNames.Mises);
                _components.Remove(FOComponentNames.Tresca);
                _components.Remove(FOComponentNames.SgnMaxAbsPri);
                _components.Remove(FOComponentNames.PrincipalMax);
                _components.Remove(FOComponentNames.PrincipalMid);
                _components.Remove(FOComponentNames.PrincipalMin);
            }
        }
        public void ComputeInvariants()
        {
            DataTypeEnum dataType = FOFieldNames.GetDataType(_name);
            if (dataType == DataTypeEnum.Vector) ComputeVectorFieldInvariant();
            else if (dataType == DataTypeEnum.Tensor) ComputeTensorFieldInvariant();
        }
        private void ComputeVectorFieldInvariant()
        {
            int count = 0;
            count += _components.ContainsKey(FOComponentNames.All) ? 1 : 0;
            //
            if (_components.Count - count == 3)
            {
                _components.Remove(FOComponentNames.All);
                //
                count = 0;
                float[][] values = new float[3][];
                foreach (var entry in _components)
                {
                    values[count++] = entry.Value.Values;
                }
                //
                float[] magnitude = new float[values[0].Length];
                for (int i = 0; i < magnitude.Length; i++)
                {
                    magnitude[i] = (float)Math.Sqrt(values[0][i] * values[0][i] +
                                                    values[1][i] * values[1][i] +
                                                    values[2][i] * values[2][i]);

                }
                _components.Insert(0, FOComponentNames.All, new FieldComponent(FOComponentNames.All, magnitude, true));
            }
        }
        private void ComputeTensorFieldInvariant()
        {
            int count = 0;
            count += _components.ContainsKey(FOComponentNames.Mises) ? 1 : 0;
            count += _components.ContainsKey(FOComponentNames.Tresca) ? 1 : 0;
            count += _components.ContainsKey(FOComponentNames.SgnMaxAbsPri) ? 1 : 0;
            count += _components.ContainsKey(FOComponentNames.PrincipalMax) ? 1 : 0;
            count += _components.ContainsKey(FOComponentNames.PrincipalMid) ? 1 : 0;
            count += _components.ContainsKey(FOComponentNames.PrincipalMin) ? 1 : 0;
            //
            if (_components.Count - count == 6)
            {
                _components.Remove(FOComponentNames.Mises);
                _components.Remove(FOComponentNames.Tresca);
                _components.Remove(FOComponentNames.SgnMaxAbsPri);
                _components.Remove(FOComponentNames.PrincipalMax);
                _components.Remove(FOComponentNames.PrincipalMid);
                _components.Remove(FOComponentNames.PrincipalMin);
                //
                count = 0;
                float[][] values = new float[6][];
                foreach (var entry in _components)
                {
                    values[count++] = entry.Value.Values;
                }
                //
                float[] vonMises = new float[values[0].Length];
                float a, b, c;
                for (int i = 0; i < vonMises.Length; i++)
                {
                    a = values[0][i] - values[1][i];
                    b = values[1][i] - values[2][i];
                    c = values[2][i] - values[0][i];
                    vonMises[i] = (float)Math.Sqrt(0.5f * (a * a + b * b + c * c +
                                  6 * (values[3][i] * values[3][i] + values[4][i] * values[4][i] + values[5][i] * values[5][i])));
                }
                // Mises
                _components.Insert(0, FOComponentNames.Mises, new FieldComponent(FOComponentNames.Mises, vonMises, true));
                // Principal
                ComputeAndAddPrincipalInvariants(values);
                // Tresca
                float[] tresca = new float[values[0].Length];
                float[] eMax = GetComponentValues(FOComponentNames.PrincipalMax);
                float[] eMin = GetComponentValues(FOComponentNames.PrincipalMin);
                for (int i = 0; i < tresca.Length; i++) tresca[i] = eMax[i] - eMin[i];
                //
                _components.Insert(1, FOComponentNames.Tresca, new FieldComponent(FOComponentNames.Tresca, tresca, true));
            }
        }
        private void ComputeAndAddPrincipalInvariants(float[][] values)
        {
            // https://en.wikipedia.org/wiki/Cubic_function#General_solution_to_the_cubic_equation_with_arbitrary_coefficients
            // https://en.wikiversity.org/wiki/Principal_stresses
            //
            float[] s0 = new float[values[0].Length];
            float[] s1 = new float[values[0].Length];
            float[] s2 = new float[values[0].Length];
            float[] s3 = new float[values[0].Length];
            //
            float s11;
            float s22;
            float s33;
            float s12;
            float s23;
            float s31;
            //
            float I1;
            float I2;
            float I3;
            //
            float sp1, sp2, sp3;
            sp1 = sp2 = sp3 = 0;
            //
            for (int i = 0; i < s1.Length; i++)
            {
                s11 = values[0][i];
                s22 = values[1][i];
                s33 = values[2][i];
                s12 = values[3][i];
                s23 = values[4][i];
                s31 = values[5][i];
                //
                I1 = s11 + s22 + s33;
                I2 = s11 * s22 + s22 * s33 + s33 * s11 - s12 * s12 - s23 * s23 - s31 * s31;
                I3 = s11 * s22 * s33 - s11 * s23 * s23 - s22 * s31 * s31 - s33 * s12 * s12 + 2f * s12 * s23 * s31;
                //
                Tools.SolveQubicEquationDepressedCubicF(1f, -I1, I2, -I3, ref sp1, ref sp2, ref sp3);
                Tools.Sort3_descending(ref sp1, ref sp2, ref sp3);
                //
                s0[i] = Math.Abs(sp1) > Math.Abs(sp3) ? sp1 : sp3;
                s1[i] = sp1;
                s2[i] = sp2;
                s3[i] = sp3;
                //
                if (float.IsNaN(s0[i])) s0[i] = 0;
                if (float.IsNaN(s1[i])) s1[i] = 0;
                if (float.IsNaN(s2[i])) s2[i] = 0;
                if (float.IsNaN(s3[i])) s3[i] = 0;
            }
            //
            AddComponent(FOComponentNames.SgnMaxAbsPri, s0, true);
            AddComponent(FOComponentNames.PrincipalMax, s1, true);
            AddComponent(FOComponentNames.PrincipalMid, s2, true);
            AddComponent(FOComponentNames.PrincipalMin, s3, true);
        }
        //
        public void RemoveNonInvariants()
        {
            DataTypeEnum dataType = FOFieldNames.GetDataType(_name);
            HashSet<string> componentNames = new HashSet<string>(_components.Keys);
            //
            if (dataType == DataTypeEnum.Vector)
            {
                componentNames.Remove(FOComponentNames.All);
            }
            else if (dataType == DataTypeEnum.Tensor)
            {
                componentNames.Remove(FOComponentNames.Mises);
                componentNames.Remove(FOComponentNames.Tresca);
                componentNames.Remove(FOComponentNames.SgnMaxAbsPri);
                componentNames.Remove(FOComponentNames.PrincipalMax);
                componentNames.Remove(FOComponentNames.PrincipalMid);
                componentNames.Remove(FOComponentNames.PrincipalMin);
            }
            //
            foreach (string componentName in componentNames) _components.Remove(componentName);
        }
        public void SetComponentValuesToZero()
        {
            foreach (var entr in _components) entr.Value.SetValuesToZero();
        }
        public void SetComponentValuesTo(float value)
        {
            foreach (var entr in _components) entr.Value.SetValuesTo(value);
        }
        public static void FindMax(Field fieldMax, Field fieldAng, Field currentField, float angleDeg)
        {
            bool update;
            FieldComponent fieldMaxComponent;
            FieldComponent fieldAngComponent;
            FieldComponent currentComponent;
            //
            foreach (var entry in fieldMax._components)
            {
                update = false;
                fieldMaxComponent = entry.Value;
                //
                if (fieldAng._components.TryGetValue(entry.Key, out fieldAngComponent) &&
                    currentField._components.TryGetValue(entry.Key, out currentComponent) &&
                    fieldMaxComponent.Values.Length == fieldAngComponent.Values.Length &&
                    fieldMaxComponent.Values.Length == currentComponent.Values.Length)
                {
                    for (int i = 0; i < fieldMaxComponent.Values.Length; i++)
                    {
                        if (currentComponent.Values[i] > fieldMaxComponent.Values[i])
                        {
                            fieldMaxComponent.Values[i] = currentComponent.Values[i];
                            fieldAngComponent.Values[i] = angleDeg;
                            update = true;
                        }
                    }
                }
                //
                if (update)
                {
                    fieldMaxComponent.UpdateMaxMin();
                    fieldAngComponent.UpdateMaxMin();
                }
            }
        }
        public static void FindMin(Field fieldMin, Field fieldAng, Field currentField, float angleDeg)
        {
            bool update;
            FieldComponent fieldMinComponent;
            FieldComponent fieldAngComponent;
            FieldComponent currentComponent;
            //
            foreach (var entry in fieldMin._components)
            {
                update = false;
                fieldMinComponent = entry.Value;
                //
                if (fieldAng._components.TryGetValue(entry.Key, out fieldAngComponent) &&
                    currentField._components.TryGetValue(entry.Key, out currentComponent) &&
                    fieldMinComponent.Values.Length == fieldAngComponent.Values.Length &&
                    fieldMinComponent.Values.Length == currentComponent.Values.Length)
                {
                    for (int i = 0; i < fieldMinComponent.Values.Length; i++)
                    {
                        if (currentComponent.Values[i] < fieldMinComponent.Values[i])
                        {
                            fieldMinComponent.Values[i] = currentComponent.Values[i];
                            fieldAngComponent.Values[i] = angleDeg;
                            update = true;
                        }
                    }
                }
                //
                if (update)
                {
                    fieldMinComponent.UpdateMaxMin();
                    fieldAngComponent.UpdateMaxMin();
                }
            }
        }

    }
}

