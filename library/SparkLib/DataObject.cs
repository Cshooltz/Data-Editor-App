using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SparkLib
{
    public class DataObject<T> : List<T>
    {
        public bool IsPrimitive { get; protected set; } = false;
        public bool IsObject { get; protected set; } = false;
        public readonly string[] fieldNames;
        public readonly System.Type[] fieldTypes;
        public readonly FieldInfo[] fieldInfos;

        public DataObject() : base()
        {
            InitializeFields(out fieldNames, out fieldTypes, out fieldInfos);
            return;
        }

        public DataObject(IEnumerable<T> data) : base(data)
        {
            InitializeFields(out fieldNames, out fieldTypes, out fieldInfos);
            return;
        }

        public DataObject(int capacity) : base(capacity)
        {
            InitializeFields(out fieldNames, out fieldTypes, out fieldInfos);
            return;
        }

        protected void InitializeFields(out string[] names, out System.Type[] types, out FieldInfo[] infos)
        {
            IsObject = true;
            infos = typeof(T).GetFields();
            names = new string[infos.Length];
            types = new System.Type[infos.Length];
            int i = 0;
            foreach (FieldInfo info in infos)
            {
                names[i] = info.Name;
                types[i] = info.FieldType;
                i++;
            }
            return;
        }

        public object[] GetEntryData(int index)
        {
            T item = this[index];
            object[] fieldData;
            if (IsObject)
            {
                fieldData = new object[fieldInfos.Length];
                int i = 0;
                foreach (FieldInfo info in fieldInfos)
                {
                    fieldData[i] = info.GetValue(item);
                    i++;
                }
            }
            else
            {
                fieldData = new object[] { item! };
            }

            return fieldData;
        }

        /// Returns: Object[] representing the data held in the fields of the entry.
        /// Returns null if not found.
        public object[]? GetEntryData(T obj)
        {
            int index = this.IndexOf(obj);
            if (index < 0) return null;
            T item = this[index];
            object[] fieldData;
            if (IsObject)
            {
                fieldData = new object[fieldInfos.Length];
                int i = 0;
                foreach (FieldInfo info in fieldInfos)
                {
                    fieldData[i] = info.GetValue(item);
                    i++;
                }
            }
            else
            {
                fieldData = new object[] { item! };
            }

            return fieldData;
        }
    }
}