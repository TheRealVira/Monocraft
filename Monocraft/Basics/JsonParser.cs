#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: JsonParser.cs
// Date - created: 2016.06.26 - 19:34
// Date - current: 2016.06.26 - 20:15

#endregion

#region Usings

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

#endregion

namespace Monocraft.Basics
{
    static class JsonParser<T>
    {
        public static Dictionary<string, T> LoadObjects(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return null;
            }

            var toRet = new Dictionary<string, T>();

            foreach (var file in Directory.GetFiles(directory))
            {
                using (var reader = new StreamReader(file))
                {
                    toRet.Add(Path.GetFileName(file).Replace(".json", ""),
                        JsonConvert.DeserializeObject<T>(reader.ReadToEnd(),
                            new JsonSerializerSettings {ObjectCreationHandling = ObjectCreationHandling.Replace}));
                    reader.Close();
                    reader.Dispose();
                }
            }

            return toRet;
        }

        // Just some testing purpose
        public static void SaveObjects(Dictionary<string, T> objects, string directory)
        {
            if (objects == null)
            {
                return;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            foreach (var ob in objects)
            {
                var test = JsonConvert.SerializeObject(ob.Value,
                    new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
                using (var writer = new StreamWriter(directory + "//" + ob.Key + ".json"))
                {
                    writer.Write(test);
                    writer.Close();
                    writer.Dispose();
                }
            }
        }
    }
}