using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CaeGlobals
{
    public static class ExtensionMethods
    {
        // Faster serialization
        // https://github.com/tomba/netserializer/blob/master/Doc.md
        //

        // Deep clone
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
        // Save clone to File
        public static void DumpToFile<T>(this T a, string fileName)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                //
                FileStream fs = new FileStream(fileName, FileMode.Create);
                //
                stream.WriteTo(fs);
                fs.Close();
            }
        }
        public static void DumpToStream<T>(this T a, BinaryWriter bw)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;

                long length = stream.Length;
                bw.Write(length);
                //stream.WriteTo(bw.BaseStream);
                stream.CopyTo(bw.BaseStream);
            }
        }
        public static string SerializeToXML<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
        public static T GetNewObject<T>()
        {
            try
            {
                return (T)typeof(T).GetConstructor(new Type[] { }).Invoke(new object[] { });
            }
            catch
            {
                return default(T);
            }
        }
        // Dictionary
        public static bool ContainsValidKey<T>(this IDictionary<string, T> dictionary, string key)
        {
            T value;
            if (dictionary.TryGetValue(key, out value))
            {
                if (value is NamedClass) return (value as NamedClass).Valid;
                else return true;  // act as ordinary ContainsKey
            }
            else return false;
        }
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
        {
            foreach (var item in dicToAdd) dic.Add(item.Key, item.Value);
        }
        public static void AddUniqueItemsFromRange<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
        {
            foreach (var item in dicToAdd)
            {
                if (!dic.ContainsKey(item.Key)) dic.Add(item.Key, item.Value);
            }
                
        }
        public static string GetNextNumberedKey<T>(this IDictionary<string, T> dictionary, string key, string postFix = "")
        {
            int n = 0;
            string newKey;
            while (true)
            {
                n++;
                newKey = key + "-" + n + postFix;
                if (!dictionary.ContainsKey(newKey)) break;
            }
            return newKey;
        }
        public static string GetNextNumberedKey(this HashSet<string> hashSet, string key, string postFix = "")
        {
            int n = 0;
            string newKey;
            while (true)
            {
                n++;
                newKey = key + "-" + n + postFix;
                if (!hashSet.Contains(newKey)) break;
            }
            return newKey;
        }
        public static string GetNextNumberedKey(this List<string> list, string key, string postFix = "")
        {
            return new HashSet<string>(list).GetNextNumberedKey(key, postFix);
        }
        public static string GetNextNumberedKey(this string[] array, string key, string postFix = "")
        {
            return new HashSet<string>(array).GetNextNumberedKey(key, postFix);
        }
        public static int FindFreeIntervalOfKeys<T>(this Dictionary<int, T> dic, int numOfKeys, int maxKey)
        {
            int count = 0;
            int firstId = 1;                    // start at 1
            //
            for (int i = 1; i <= maxKey; i++)   // start at 1
            {
                if (dic.ContainsKey(i))
                {
                    count = 0;
                    firstId = i + 1;
                }
                else
                {
                    if (++count == numOfKeys) break;
                }
            }
            //
            return firstId;
        }
        // Property grid items
        public static IEnumerable<GridItem> EnumerateAllItems(this PropertyGrid grid)
        {
            if (grid == null) yield break;
            // Get to root item
            GridItem start = grid.SelectedGridItem;
            while (start.Parent != null)
            {
                start = start.Parent;
            }
            //
            foreach (GridItem item in start.EnumerateAllItems())
            {
                yield return item;
            }
        }
        public static IEnumerable<GridItem> EnumerateAllItems(this GridItem item)
        {
            if (item == null) yield break;
            //
            yield return item;
            foreach (GridItem child in item.GridItems)
            {
                foreach (GridItem gc in child.EnumerateAllItems())
                {
                    yield return gc;
                }
            }
        }
        // String Array
        public static string ToUTF8(this string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Encoding.Default.GetString(bytes);
        }
        public static string ToUnicode(this string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            return Encoding.Default.GetString(bytes);
        }
        public static string ToASCII(this string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            return Encoding.Default.GetString(bytes);
        }
        //
        public static string ToShortString(this string[] stringArray)
        {
            string allNames = null;
            if (stringArray != null)
            {
                if (stringArray.Length >= 1) allNames = stringArray[0];
                if (stringArray.Length >= 2) allNames += ", ...";
            }
            return allNames;
        }
        //
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        // vtkSelectBy
        public static bool IsGeometryBased(this vtkSelectBy selectBy)
        {
            return selectBy == vtkSelectBy.QueryEdge ||
                   selectBy == vtkSelectBy.QuerySurface ||
                   selectBy == vtkSelectBy.Geometry ||
                   selectBy == vtkSelectBy.GeometryVertex ||
                   selectBy == vtkSelectBy.GeometryEdge ||
                   selectBy == vtkSelectBy.GeometrySurface ||
                   selectBy == vtkSelectBy.GeometryEdgeAngle ||
                   selectBy == vtkSelectBy.GeometrySurfaceAngle ||
                   selectBy == vtkSelectBy.GeometryPart;
        }
        // Double
        public static string ToCalculiX16String(this double value, bool enforceSeparator = false)
        {
            string result = value.ToString();
            if (result.Length > 16) result = value.ToString("E8");
            else if (enforceSeparator && !result.Contains(".")) result += ".";
            return result;
        }
        // String[]
        public static string ToRows(this string[] names, int maxRows = 30)
        {
            string rows = "";
            for (int i = 0; i < Math.Min(names.Length, maxRows); i++)
            {
                rows += names[i];
                if (i < names.Length - 1) rows += Environment.NewLine;
            }
            if (maxRows < names.Length) rows += "...";
            return rows;
        }
        // Enum
        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }
        public static string GetDisplayedName<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DynamicTypeDescriptor.StandardValueAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DynamicTypeDescriptor.StandardValueAttribute)attrs[0]).DisplayName;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }
        // NamedClass[]
        public static string[] GetNames(this NamedClass[] namedClasses)
        {
            if (namedClasses == null) return new string[0];
            //
            List<string> names = new List<string>();
            foreach (var namedClass in namedClasses) names.Add(namedClass.Name);
            return names.ToArray();
        }               
    }




//    // Create the geometry of a point (the coordinate)
//    vtkPoints points = vtkPoints.New();
//    // Create the topology of the point (a vertex)
//    vtkCellArray vertices = vtkCellArray.New();
//            for (int i = 0; i<data.Geometry.Nodes.Coor.GetLength(0); i++)
//            {
//                points.InsertNextPoint(data.Geometry.Nodes.Coor[i][0], data.Geometry.Nodes.Coor[i][1], data.Geometry.Nodes.Coor[i][2]);
//                vertices.InsertNextCell(1);
//                vertices.InsertCellPoint(i);
//            }
//// Create a polydata object
//vtkPolyData pointsPolyData = vtkPolyData.New();
//// Set the points and vertices created as the geometry and topology of the polydata
//pointsPolyData.SetPoints(points);
//pointsPolyData.SetVerts(vertices);
//// Mapper
//vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
//mapper.SetInput(pointsPolyData);
//_geometryMapper = mapper;
//// Actor
//_geometry.SetMapper(_geometryMapper);
////
//_geometryProperty = _geometry.GetProperty();
//_geometryProperty.SetColor(data.Color.R / 255d, data.Color.G / 255d, data.Color.B / 255d);
//            //_geometryProperty.SetPointSize(data.NodeSize);
}
