using System.Collections.Generic;
using System;
using System.IO;
using Newtonsoft.Json;

namespace LamiaSimulation
{
    public abstract class DataType
    {
        static Dictionary<string, Dictionary<string, DataType>> data;

        public string ID;

        public static void LoadDataFromJson<T>(string filepath)
        {
            data ??= new Dictionary<string, Dictionary<string, DataType>>();
            var typeName = typeof(T).Name;

            if(!data.ContainsKey(typeName))
                data[typeName] = new Dictionary<string, DataType>();

            var json = File.ReadAllText(filepath);
            var castData = JsonConvert.DeserializeObject<Dictionary<string, T>>(json);
            var typeData = new Dictionary<string, DataType>();
            foreach(var singleData in castData)
            {
                typeData[singleData.Key] = singleData.Value as DataType;
                typeData[singleData.Key].ID = singleData.Key;
            }

            data[typeName] = typeData;
        }

        public static Dictionary<string, DataType> GetTypeData<T>()
        {
            var dataTypeName = typeof(T).Name;
            return data.ContainsKey(dataTypeName) ? data[dataTypeName] : null;
        }
    }

    public static class DataQuery<T> where T: DataType
    {
        public static Dictionary<string, T> GetAll()
        {
            return DataType.GetTypeData<T>() as Dictionary<string, T>;
        }

        public static T GetByID(string ID)
        {
            var data = DataType.GetTypeData<T>();
            if(data == null)
                return default;
            return data.ContainsKey(ID) ? (T)data[ID] : default;
        }

        public static string IDOf(T obj)
        {
            var data = DataType.GetTypeData<T>();
            if(data == null)
                return default;
            foreach(var singleData in data)
                if(singleData.Value == obj)
                    return singleData.Key;
            return default;
        }

    }
}