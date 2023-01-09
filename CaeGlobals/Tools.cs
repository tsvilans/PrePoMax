using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using DynamicTypeDescriptor;
using System.Linq.Expressions;
using System.Diagnostics;

namespace CaeGlobals
{
    [Serializable]
    public static class Tools
    {
        public static string GetEnumStandardValueDescription(Enum value)
        {
            System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());
            StandardValueAttribute[] attributes = (StandardValueAttribute[])fi.GetCustomAttributes(typeof(StandardValueAttribute), false);
            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return value.ToString();
        }

        public static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                    return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static void SetLabelColumnWidth(PropertyGrid grid, double labelRatio)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            // get the grid view
            Control view = (Control)grid.GetType().GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(grid);
            //Control view = (Control)typeof(PropertyGrid).GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(grid);

            // set label width
            //FieldInfo fi = view.GetType().GetField("labelWidth", BindingFlags.Instance | BindingFlags.NonPublic);
            //fi.SetValue(view, width);

            FieldInfo fi2 = view.GetType().GetField("labelRatio");
            fi2.SetValue(view, labelRatio);

            // refresh
            view.Invalidate();
        }

        // Read clone from File
        public static T LoadDumpFromFile<T>(string fileName)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                stream.Write(bytes, 0, (int)fs.Length);
                fs.Close();
                stream.Position = 0;
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(stream);
            }
        }

        public static T LoadDumpFromFile<T>(BinaryReader br)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                long length = br.ReadInt64();
                byte[] bytes = br.ReadBytes((int)length);
                stream.Write(bytes, 0, (int)length);
                stream.Position = 0;
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(stream);
            }
        }

        public static async Task<T> LoadDumpFromFileAsync<T>(string fileName)
        {
            T a = default(T);
            await Task.Run(() => a = LoadDumpFromFile<T>(fileName));
            return a;
        }

        public static int IntParseFast(string value)
        {
            int result = 0;
            int length = value.Length;
            for (int i = 0; i < length; i++)
            {
                result = 10 * result + (value[i] - 48);
            }
            return result;
        }

        public static double RoundToSignificantDigits(double d, int digits)
        {
            //double[] stdValues = new double[] { 0.1, 0.2, 0.25, 0.5 };
            if (d == 0.0)
            {
                return 0.0;
            }
            else
            {
                double leftSideNumbers = Math.Floor(Math.Log10(Math.Abs(d))) + 1;
                double scale = Math.Pow(10, leftSideNumbers);
                double result = scale * Math.Round(d / scale, digits, MidpointRounding.AwayFromZero);
                // Clean possible precision error.
                if ((int)leftSideNumbers >= digits)
                {
                    return Math.Round(result, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    int roundingDigits = digits - (int)leftSideNumbers;
                    roundingDigits = Math.Max(0, Math.Min(roundingDigits, 15));
                    return Math.Round(result, roundingDigits, MidpointRounding.AwayFromZero);
                }
            }
        }

        // Windows version
        public static string GetWindowsName()
        {
            var reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string productName = (string)reg.GetValue("ProductName");
            return productName;
        }
        /// Check if it's Windows 8.1
        public static bool IsWindows10orNewer()
        {
            try
            {
                string[] tmp = GetWindowsName().Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                double version;

                if (tmp.Length > 2 && double.TryParse(tmp[1], out version) && version >= 10) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }

        // File locked
        public static bool IsFileLocked(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            FileStream stream = null;

            try
            {
                stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
        public static bool WaitForFileToUnlock(string fileName, long miliseconds)
        {
            DateTime start = DateTime.Now;
            while (IsFileLocked(fileName))
            {
                if ((DateTime.Now - start).TotalMilliseconds >= miliseconds) return false;
                System.Threading.Thread.Sleep(100);
            }
            return true;
        }
        public static string[] ReadAllLines(string fileName, bool trimStart = false)
        {
            long count;
            string[] lines = null;
            //
            if (!WaitForFileToUnlock(fileName, 5000)) return null;
            //
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 16*4096))
            {
                count = CountLines(fileStream);
                lines = new string[count];
                //
                count = 0;
                fileStream.Position = 0;
                while (!streamReader.EndOfStream)
                {
                    if (trimStart) lines[count++] = streamReader.ReadLine().TrimStart();
                    else lines[count++] = streamReader.ReadLine();
                }
                //
                streamReader.Close();
                fileStream.Close();
            }
            //
            return lines;
        }
        // Read number of lines        
        //
        [DebuggerStepThrough]
        public static bool IsNullOrEmptyOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        //
        [DebuggerStepThrough]
        public static T NotNull<T>(T value, string argName) where T : class
        {
            if (argName.IsNullOrEmptyOrWhiteSpace()) { argName = "Invalid"; }
            //
            if (value == null) throw new Exception(argName);
            return value;
        }
        //
        [DebuggerStepThrough]
        public static long CountLines(this Stream stream, Encoding encoding = default)
        {
            const char CR = '\r';
            const char LF = '\n';
            const char NULL = (char)0;
            //
            NotNull(stream, nameof(stream));
            //
            var lineCount = 0L;
            var byteBuffer = new byte[1024 * 1024];
            var detectedEOL = NULL;
            var currentChar = NULL;
            int bytesRead;
            //
            if (encoding is null || Equals(encoding, Encoding.ASCII) || Equals(encoding, Encoding.UTF8))
            {
                while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
                {
                    for (var i = 0; i < bytesRead; i++)
                    {
                        currentChar = (char)byteBuffer[i];
                        //
                        if (detectedEOL != NULL)
                        {
                            if (currentChar == detectedEOL)
                            {
                                lineCount++;
                            }
                        }
                        else if (currentChar == LF || currentChar == CR)
                        {
                            detectedEOL = currentChar;
                            lineCount++;
                        }
                    }
                }
            }
            else
            {
                var charBuffer = new char[byteBuffer.Length];
                //
                while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
                {
                    var charCount = encoding.GetChars(byteBuffer, 0, bytesRead, charBuffer, 0);
                    //
                    for (var i = 0; i < charCount; i++)
                    {
                        currentChar = charBuffer[i];
                        //
                        if (detectedEOL != NULL)
                        {
                            if (currentChar == detectedEOL)
                            {
                                lineCount++;
                            }
                        }
                        else if (currentChar == LF || currentChar == CR)
                        {
                            detectedEOL = currentChar;
                            lineCount++;
                        }
                    }
                }
            }
            //
            if (currentChar != LF && currentChar != CR && currentChar != NULL)
            {
                lineCount++;
            }
            //
            return lineCount;
        }
        // String
        public static string GetRandomString(int len)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[len];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new String(stringChars);
        }

        // Sort
        public static void Sort3_descending(ref float arr0, ref float arr1, ref float arr2)
        {
            float arr120;
            float arr121;

            // sort first two values
            if (arr0 > arr1)
            {
                arr120 = arr0;
                arr121 = arr1;
            } // if
            else
            {
                arr120 = arr1;
                arr121 = arr0;
            } // else

            // decide where to put arr12 and the third original value arr[ 3 ]
            if (arr121 > arr2)
            {
                arr0 = arr120;
                arr1 = arr121;
            } // if
            else if (arr2 > arr120)
            {
                arr0 = arr2;
                arr1 = arr120;
                arr2 = arr121;
            } // if
            else
            {
                arr0 = arr120;
                arr1 = arr2;
                arr2 = arr121;
            } // else
        }
        public static void Sort3_descending(ref double arr0, ref double arr1, ref double arr2)
        {
            double arr120;
            double arr121;

            // sort first two values
            if (arr0 > arr1)
            {
                arr120 = arr0;
                arr121 = arr1;
            } // if
            else
            {
                arr120 = arr1;
                arr121 = arr0;
            } // else

            // decide where to put arr12 and the third original value arr[ 3 ]
            if (arr121 > arr2)
            {
                arr0 = arr120;
                arr1 = arr121;
            } // if
            else if (arr2 > arr120)
            {
                arr0 = arr2;
                arr1 = arr120;
                arr2 = arr121;
            } // if
            else
            {
                arr0 = arr120;
                arr1 = arr2;
                arr2 = arr121;
            } // else
        }
        public static int SolveQubicEquationNotWorking(double a1, double b, double c, double d, ref double x1, ref double x2, ref double x3)
        {
            // https://www.codeproject.com/Articles/798474/To-Solve-a-Cubic-Equation
            double a, x2imag, x3imag;
            double p, q, u, v;
            double r, alpha;
            int res;
            res = 0;
            if (a1 != 0)
            {
                a = b / a1;
                b = c / a1;
                c = d / a1;

                p = -(a * a / 3.0) + b;
                q = (2.0 / 27.0 * a * a * a) - (a * b / 3.0) + c;
                d = q * q / 4.0 + p * p * p / 27.0;
                if (Math.Abs(d) < Math.Pow(10.0, -11.0))
                    d = 0;
                // 3 cases D > 0, D == 0 and D < 0
                if (d > 1e-20)
                {
                    u = Xroot(-q / 2.0 + Math.Sqrt(d), 3.0);
                    v = Xroot(-q / 2.0 - Math.Sqrt(d), 3.0);
                    x1 = u + v - a / 3.0;
                    x2 = -(u + v) / 2.0 - a / 3.0;
                    x2imag = Math.Sqrt(3.0) / 2.0 * (u - v);
                    x3 = x2;
                    x3imag = -x2imag;
                    res = 1;
                }
                if (Math.Abs(d) <= 1e-20)
                {
                    u = Xroot(-q / 2.0, 3.0);
                    v = Xroot(-q / 2.0, 3.0);
                    x1 = u + v - a / 3.0;
                    x2 = -(u + v) / 2.0 - a / 3.0;
                    res = 2;
                }
                if (d < -1e-20)
                {
                    r = Math.Sqrt(-p * p * p / 27.0);
                    alpha = Math.Atan(Math.Sqrt(-d) / -q * 2.0);
                    if (q > 0)                         // if q > 0 the angle becomes 2 * PI - alpha
                        alpha = 2.0 * Math.PI - alpha;

                    x1 = Xroot(r, 3.0) * (Math.Cos((6.0 * Math.PI - alpha) / 3.0) + Math.Cos(alpha / 3.0)) - a / 3.0;
                    x2 = Xroot(r, 3.0) * (Math.Cos((2.0 * Math.PI + alpha) / 3.0) + Math.Cos((4.0 * Math.PI - alpha) / 3.0)) - a / 3.0;
                    x3 = Xroot(r, 3.0) * (Math.Cos((4.0 * Math.PI + alpha) / 3.0) + Math.Cos((2.0 * Math.PI - alpha) / 3.0)) - a / 3.0;
                    res = 3;
                }
            }
            else
                res = 0;
            return res;
        }
        public static void SolveQubicEquationDepressedCubic(double a, double b, double c, double d, ref double x1, ref double x2, ref double x3)
        {
            // https://en.wikipedia.org/wiki/Cubic_function
            double p;
            double q;
            double tmp1;
            double tmp2;
            double alpha;

            p = (3.0 * a * c - b * b) / (3.0 * a * a);
            if (p > 0)
            {
                x1 = x2 = x3 = 0;
            }
            else
            {
                q = (2.0 * b * b * b - 9.0 * a * b * c + 27.0 * a * a * d) / (27.0 * a * a * a);

                tmp1 = (3.0 * q) / (2.0 * p) * Math.Sqrt(-3.0 / p);
                if (tmp1 > 1.0) tmp1 = 1.0;
                else if (tmp1 < -1.0) tmp1 = -1.0;
                alpha = 1.0 / 3.0 * Math.Acos(tmp1);

                tmp1 = 2.0 * Math.Sqrt(-p / 3.0);
                tmp2 = b / (3.0 * a);
                x1 = tmp1 * Math.Cos(alpha) - tmp2;
                x2 = tmp1 * Math.Cos(alpha - 2.0 * Math.PI / 3.0) - tmp2;
                x3 = tmp1 * Math.Cos(alpha - 4.0 * Math.PI / 3.0) - tmp2;
            }
        }
        public static double Xroot(double a, double x)
        {
            double i = 1;
            if (a < 0) i = -1;
            return (i * Math.Exp(Math.Log(a * i) / x));
        }

        // <summary>
        // Get the name of a static or instance property from a property access lambda.
        // </summary>
        // <typeparam name="T">Type of the property</typeparam>
        // <param name="propertyLambda">lambda expression of the form: '() => Class.Property' or '() => object.Property'</param>
        // <returns>The name of the property</returns>
        public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return me.Member.Name;
        }
        //
        public static int[] GetSortedKey(int id1, int id2)
        {
            if (id1 < id2) return new int[] { id1, id2 };
            else return new int[] { id2, id1 };
        }
    }
}
