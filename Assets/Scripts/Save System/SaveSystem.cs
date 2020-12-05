using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SaveSystem
{
    private static string activeProjectName = "Untitled";
    private const string fileExtension = ".txt";

    public static void SetActiveProject(string projectName)
    {
        activeProjectName = projectName;
    }

    public static void Init()
    {
        // Create save directory (if doesn't exist already)
        Directory.CreateDirectory(CurrentSaveProfileDirectoryPath);
        Directory.CreateDirectory(CurrentSaveProfileWireLayoutDirectoryPath);
    }

    public static void LoadAll(Manager manager)
    {
        // Load any saved chips
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var chipSavePaths = Directory.GetFiles(CurrentSaveProfileDirectoryPath, "*" + fileExtension);
        ChipLoader.LoadAllChips(chipSavePaths, manager);
        Debug.Log("Load time: " + sw.ElapsedMilliseconds);
    }

    public static string GetPathToSaveFile(string saveFileName) => Path.Combine(CurrentSaveProfileDirectoryPath, saveFileName + fileExtension);

    public static string GetPathToWireSaveFile(string saveFileName) => Path.Combine(CurrentSaveProfileWireLayoutDirectoryPath, saveFileName + fileExtension);

    private static string CurrentSaveProfileDirectoryPath => Path.Combine(SaveDataDirectoryPath, activeProjectName);

    private static string CurrentSaveProfileWireLayoutDirectoryPath => Path.Combine(CurrentSaveProfileDirectoryPath, "WireLayout");

    public static string[] GetSaveNames()
    {
        if (Directory.Exists(SaveDataDirectoryPath))
        {
            return Directory.GetDirectories(SaveDataDirectoryPath)
                .Select(dir => dir.Split(Path.DirectorySeparatorChar).Last())
                .ToArray();
        }
        else
        {
            return Array.Empty<string>();
        }
    }

    public static string SaveDataDirectoryPath
    {
        get
        {
            const string saveFolderName = "SaveData";
            return Path.Combine(Application.persistentDataPath, saveFolderName);
        }
    }
}