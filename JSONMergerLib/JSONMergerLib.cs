using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JSONMergerLib;

public static class JSONMergerLib
{
    public static JObject MergeAllJSONs(string path)
    {
        JContainer result = ProcessFolder(path, true);
        if (result is not JObject)
        {
            throw new InvalidOperationException("The root of the JSON tree must be an object.");
        }
        return (JObject)result;
    }

    private static JContainer ProcessFolder(string path, bool isRoot = false)
    {
        string folderMappingJSONpath = Path.Combine(path, "folderMapping.json");
        Dictionary<string, string> folderMapping = new Dictionary<string, string>();
        JContainer result = isRoot ? new JObject() : null;
        if (File.Exists(folderMappingJSONpath))
        {
            folderMapping = ReadFolderMappingJSON(folderMappingJSONpath);
            JObject objectToBuild = new JObject();
            foreach (string key in folderMapping.Keys)
            {
                string rawPath = folderMapping[key];
                if (rawPath.StartsWith("./"))
                {
                    rawPath = Path.Combine(path, rawPath[2..]);
                }
                folderMapping[key] = Path.GetFullPath(rawPath);
                if (key == "content")
                {
                    throw new InvalidOperationException(
                        "The key 'content' is reserved for the content of the JSON file."
                    );
                }
                objectToBuild.Add(key, ProcessFolder(Path.Combine(path, folderMapping[key])));
            }
            result = objectToBuild;
        }
        IEnumerable<string> Directories = Directory
            .GetDirectories(path)
            .ToList()
            .Where(path => !folderMapping.ContainsValue(Path.GetFullPath(path)));
        IEnumerable<string> Files = Directory
            .GetFiles(path)
            .Concat(
                Directories.SelectMany(dir =>
                    Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
                )
            )
            .ToList()
            .Where(path => !path.EndsWith("folderMapping.json"));
        JArray arrayToBuild = new JArray();
        arrayToBuild = Files
            .Select(file => JArray.Parse(File.ReadAllText(file)))
            .Aggregate(
                new JArray(),
                (current, next) =>
                {
                    current.Merge(
                        next,
                        new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union }
                    );
                    return current;
                }
            );
        if (result is not null && result is JObject)
        {
            ((JObject)result).Add("content", arrayToBuild);
        }
        else
        {
            result = arrayToBuild;
        }
        return result;
    }

    private static Dictionary<string, string> ReadFolderMappingJSON(string folderMappingJSONpath)
    {
        JObject folderMappingJSON = JObject.Parse(File.ReadAllText(folderMappingJSONpath));
        Dictionary<string, string> folderMapping = new Dictionary<string, string>();
        foreach (JProperty property in folderMappingJSON.Properties())
        {
            folderMapping.Add(property.Name, property.Value.ToString());
        }

        return folderMapping;
    }
}
