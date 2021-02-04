using System;
using System.Collections.Generic;

namespace SparkLib
{
    class DataSet
    {
        public List<string> fieldNames = new List<string>();
        // Each list is a column, and should correspond with the names in fieldNames.
        public List<List<Object>> fieldData = new List<List<Object>>();

        public void AddField(string name, List<Object> list)
        {
            fieldNames.Add(name);
            fieldData.Add(list);
            return;
        }

        public void RemoveField(string name)
        {
            int index = fieldNames.FindIndex(s => s.Equals(name));
            if (!(index < 0))
            {
                fieldNames.RemoveAt(index);
                fieldData.RemoveAt(index);
            }
            return;
        }

        public void RemoveField(int index)
        {
            if (!(index < 0))
            {
                fieldNames.RemoveAt(index);
                fieldData.RemoveAt(index);
            }
            return;
        }
    }
}
