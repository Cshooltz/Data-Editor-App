using System;
using System.Collections;
using System.Collections.Generic;

namespace SparkLib
{
    // Defines parsing methods for each of the 5 data sources
    // 1) A List of POCO objects
    // 2) A dictionary that defines a table (a dictionary of lists)
    // 3) A list of dictionaries
    // 4) A plain array (a one column table)
    // 5) An array of arrays.
    public interface IDataParser
    {
        void ParseDataObject<T>(DataObject<T> data);
        void ParseDictOfLists<T, U>(Dictionary<T, U[]> data);
        void ParseListOfDicts<T, U>(Dictionary<T, U>[] data);
        void ParseSingleArray<T>(string name, T[] data);
        void ParseDoubleArray<T>(string[] names, T[][] data);
    }
}