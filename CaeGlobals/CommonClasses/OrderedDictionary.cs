// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace CaeGlobals
{
    /// <summary>
    ///   Ordered dictionary.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class provides a ordered dictionary implementation for C#/.NET. Unlike the rest
    ///   of the framework, this class is available under a MIT license, so please feel free to
    ///   re-use its source code in your own projects.
    /// </remarks>
    /// 
    /// <typeparam name="TKey">The types of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// 
    [Serializable]
    public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializable
    {
        // Variables                                                                                                                
        private static StringComparer _comparer = StringComparer.OrdinalIgnoreCase;
        private string _name;                               //ISerializable
        private List<TKey> _list;                           //ISerializable
        private Dictionary<TKey, TValue> _dictionary;       //ISerializable


        // Constructors                                                                                                             
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class.
        /// </summary>
        public OrderedDictionary(string name)
        {
            _name = name;
            _dictionary = new Dictionary<TKey, TValue>();
            _list = new List<TKey>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// 
        /// <param name="capacity">The initial number of elements that the <see cref="OrderedDictionary{TKey, TValue}"/> can contain.</param>
        /// 
        public OrderedDictionary(string name, int capacity)
        {
            _name = name;
            _dictionary = new Dictionary<TKey, TValue>(capacity);
            _list = new List<TKey>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// 
        /// <param name="comparer">The IEqualityComparer implementation to use when comparing keys, or null to use 
        ///     the default EqualityComparer for the type of the key.</param>
        /// 
        public OrderedDictionary(string name, IEqualityComparer<TKey> comparer)
        {
            _name = name;
            _dictionary = new Dictionary<TKey, TValue>(comparer);
            _list = new List<TKey>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// 
        /// <param name="capacity">The initial number of elements that the <see cref="OrderedDictionary{TKey, TValue}"/> can contain.</param>
        /// <param name="comparer">The IEqualityComparer implementation to use when comparing keys, or null to use 
        ///     the default EqualityComparer for the type of the key.</param>
        /// 
        public OrderedDictionary(string name, int capacity, IEqualityComparer<TKey> comparer)
        {
            _name = name;
            _dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
            _list = new List<TKey>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// 
        /// <param name="dictionary">The System.Collections.Generic.IDictionary`2 whose elements are copied to the
        ///     new <see cref="OrderedDictionary{TKey, TValue}"/>.</param>
        /// 
        public OrderedDictionary(string name, IDictionary<TKey, TValue> dictionary)
        {
            _name = name;
            _dictionary = new Dictionary<TKey, TValue>(dictionary);
            _list = new List<TKey>(_dictionary.Keys);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// 
        /// <param name="dictionary">The System.Collections.Generic.IDictionary`2 whose elements are copied to the
        ///     new <see cref="OrderedDictionary{TKey, TValue}"/>.</param>
        /// <param name="comparer">The IEqualityComparer implementation to use when comparing keys, or null to use 
        ///     the default EqualityComparer for the type of the key.</param>
        /// 
        public OrderedDictionary(string name, IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _name = name;
            _dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
            _list = new List<TKey>(_dictionary.Keys);
        }
        //ISerializable
        public OrderedDictionary(SerializationInfo info, StreamingContext context)
        {
            int count = 0;
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_name":
                        _name = (string)entry.Value; count++; break;
                    case "list":        // Compatibility v1.3.4
                    case "_list":
                        _list = (List<TKey>)entry.Value; count++; break;
                    case "dictionary":  // Compatibility v1.3.4
                    case "_dictionary":
                        _dictionary = (Dictionary<TKey, TValue>)entry.Value; count++; break;
                }
                // Compatibility v1.3.4
                if (_name == null || _name == "") _name = "Ordered dictionary";
            }
        }


        // Methods                                                                                                                  
        /// <summary>
        ///   Gets the <typeparam ref="TValue"/> at the specified index.
        /// </summary>
        /// 
        /// <param name="index">The index.</param>
        /// 
        public TKey GetKeyByIndex(int index)
        {
            return _list[index];
        }

        /// <summary>
        ///   Gets the <typeparam ref="TValue"/> at the specified index.
        /// </summary>
        /// 
        /// <param name="index">The index.</param>
        /// 
        public TValue GetValueByIndex(int index)
        {
            return this[GetKeyByIndex(index)];
        }

        /// <summary>
        ///   Gets or sets the <typeparam ref="TValue"/> with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The key.</param>
        /// 
        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (_dictionary.TryGetValue(key, out value)) return value;
                else throw new CaeException("The given key '" + key.ToString() + "' was not present in the dictionary.");
            }
            set
            {
                _dictionary[key] = value;
                if (!_list.Contains(key))
                    _list.Add(key);
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        /// <value>The keys.</value>
        /// 
        public ICollection<TKey> Keys
        {
            get { return _list; }
        }

        public void SortKeysAs(ICollection<TKey> keys)
        {
            foreach (var key in keys)
            {
                if (_list.IndexOf(key) < 0) throw new NotSupportedException();
            }
            _list.Clear();
            _list.AddRange(keys);
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        public ICollection<TValue> Values
        {
            get { return _list.Select(x => _dictionary[x]).ToList(); }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// 
        public int Count
        {
            get { return _dictionary.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// 
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        /// 
        public bool IsReadOnly
        {
            get { return ((IDictionary<TKey, TValue>)_dictionary).IsReadOnly; }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// 
        public void Add(TKey key, TValue value)
        {
            try
            {
                _dictionary.Add(key, value);
                if (!_list.Contains(key)) _list.Add(key);
            }
            catch (Exception ex)
            {
                string name = _name;
                if (name != null && name.Length > 0) name += " ";
                throw new Exception("The dictionary " + name + "already contains the key " + key.ToString() + "." +
                                    Environment.NewLine + ex.Message);
            }

        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// 
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// 
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((IDictionary<TKey, TValue>)_dictionary).Add(item);
            if (!_list.Contains(item.Key))
                _list.Add(item.Key);
        }

        /// <summary>
        /// Adds another dictionary to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        /// <param name="dicToAdd">The dictionary to add.</param>
        /// 
        public void AddRange(Dictionary<TKey, TValue> dicToAdd)
        {
            foreach (var item in dicToAdd) Add(item.Key, item.Value);
        }

        /// <summary>
        /// Replace an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        /// <param name="oldKey">The object to use as the key of the element to remove.</param>
        /// <param name="newKey">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// 
        public bool Replace(TKey oldKey, TKey newKey, TValue value)
        {
            if (_dictionary.Remove(oldKey))
            {
                _dictionary.Add(newKey, value);
                //
                int index = _list.IndexOf(oldKey);
                _list.RemoveAt(index);
                _list.Insert(index, newKey);
                return true;
            }
            //
            return false;
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// 
        public void Clear()
        {
            _dictionary.Clear();
            _list.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// 
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// 
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        /// 
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)_dictionary).Contains(item);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
        /// 
        /// <returns>true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.</returns>
        /// 
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// 
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// 
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<TKey, TValue> pair in this)
                array[arrayIndex++] = pair;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// 
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// 
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (TKey key in _list)
                yield return new KeyValuePair<TKey, TValue>(key, _dictionary[key]);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// 
        /// <param name="key">The key of the element to remove.</param>
        /// 
        /// <returns>true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
        /// 
        public bool Remove(TKey key)
        {
            if (_dictionary.Remove(key))
            {
                _list.Remove(key);
                return true;
            }

            return false;
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            if (_dictionary.TryGetValue(key, out value))
            {
                _dictionary.Remove(key);
                _list.Remove(key);
                return true;
            }
            //
            return false;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// 
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// 
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// 
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (((IDictionary<TKey, TValue>)_dictionary).Remove(item))
            {
                _list.Remove(item.Key);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// 
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// 
        /// <returns>true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.</returns>
        /// 
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            if (key == null) return false;
            else return ((IDictionary<TKey, TValue>)_dictionary).TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// 
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            info.AddValue("_name", _name, typeof(string));
            info.AddValue("_list", _list, typeof(List<TKey>));
            info.AddValue("_dictionary", _dictionary, typeof(Dictionary<TKey, TValue>));
        }
    }
}