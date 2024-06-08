// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json.Linq;

class JSONMergerCLI
{
    static void Main(string[] args)
    {
        string inputPath = args[0];
        string outputPath = args[1];
        JObject result = JSONMergerLib.JSONMergerLib.MergeAllJSONs(inputPath);
        File.WriteAllText(outputPath, result.ToString());
    }
}
