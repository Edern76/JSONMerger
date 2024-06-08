# JSONMerger

## Introduction

A JSON merging utility that merges multiple JSON files into one.

Made it mainly for myself for future projects, so don't expect clean code or tons of features, but feel free to use it if you feel like this could be of use to you.

## Behaviour

When pointing it to a folder, the utility has two different possible behaviors : 

- **Array mode** : If the folder doesn't have a `folderMappping.json`, it reads all JSON files from the folder and its subfolder (**and expects them to be arrays**), merges them all together and returns an array with all the merged results.
- **Object mode** : If the folder has a `folderMapping.json`, then it reads it (and expects it to be a single object with string properties, keys being arbitrary names and values being folder paths), then for each property, it will create a property in the result object, its key being the folder mapping property's key and the value being the value of the recursive call of the JSONMerger on the folder defined in the value of the folder. "Loose" elements (those not in any mapped folder) will be added to the result object under the `content` key. Trying to define manually a folder mapping with the `content` key will throw an exception.

The first call to the JSONMapper will always operate in object mode. If there is no `folderMapping.json`, then all elements will be added to `content`. Therefore, the resulting JSON will always be an object which can have multiple arrays as property values.

To illustrate the intended behaviour, example inputs with the expected output are available in the `JSONTestResources` folder.

## Usage

This project is mainly meant to be used as a library in other projects. However, we provide a very simple CLI interface for quick testing in the `JSONMergerCLI` C# project.

It is meant to be used like this

```
JSONMergerCLI.exe <input_path> <output_path>
```
You need to manually specify the output file name at the end of the output path. Also note that there is absolutely zero validation on the arguments you provide to the CLI tool.