﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class ChipLoader
{
    public static void LoadAllChips(string[] chipPaths, Manager manager)
    {
        var savedChips = new SavedChip[chipPaths.Length];

        // Read saved chips from file
        for (int i = 0; i < chipPaths.Length; i++)
        {
            using (StreamReader reader = new StreamReader(chipPaths[i]))
            {
                string chipSaveString = reader.ReadToEnd();
                savedChips[i] = JsonUtility.FromJson<SavedChip>(chipSaveString);
            }
        }

        SortChipsByOrderOfCreation(ref savedChips);
        // Maintain dictionary of loaded chips (initially just the built-in chips)
        var loadedChips = new Dictionary<string, Chip>();
        for (int i = 0; i < manager.builtinChips.Length; i++)
        {
            var builtinChip = manager.builtinChips[i];
            loadedChips.Add(builtinChip.chipName, builtinChip);
        }

        for (int i = 0; i < savedChips.Length; i++)
        {
            var chipToTryLoad = savedChips[i];
            var loadedChipData = LoadChip(chipToTryLoad, loadedChips, manager.wirePrefab);
            var loadedChip = manager.LoadChip(loadedChipData);
            loadedChips.Add(loadedChip.chipName, loadedChip);
        }
    }

    // Instantiates all components that make up the given clip, and connects them up with wires
    // The components are parented under a single "holder" object, which is returned from the function
    private static ChipSaveData LoadChip(SavedChip chipToLoad, Dictionary<string, Chip> previouslyLoadedChips, Wire wirePrefab)
    {
        var loadedChipData = new ChipSaveData();
        var numComponents = chipToLoad.savedComponentChips.Length;
        loadedChipData.componentChips = new Chip[numComponents];
        loadedChipData.chipName = chipToLoad.name;
        loadedChipData.chipColour = chipToLoad.colour;
        loadedChipData.chipNameColour = chipToLoad.nameColour;
        loadedChipData.creationIndex = chipToLoad.creationIndex;

        // Spawn component chips (the chips used to create this chip)
        // These will have been loaded already, and stored in the previouslyLoadedChips dictionary
        for (int i = 0; i < numComponents; i++)
        {
            var componentToLoad = chipToLoad.savedComponentChips[i];
            var componentName = componentToLoad.chipName;
            var pos = new Vector2((float)componentToLoad.posX, (float)componentToLoad.posY);

            if (!previouslyLoadedChips.ContainsKey(componentName))
            {
                Debug.LogError("Failed to load sub component: " + componentName + " While loading " + chipToLoad.name);
            }

            var loadedComponentChip = GameObject.Instantiate(previouslyLoadedChips[componentName], pos, Quaternion.identity);
            loadedChipData.componentChips[i] = loadedComponentChip;

            // Load input pin names
            for (int inputIndex = 0; inputIndex < componentToLoad.inputPins.Length; inputIndex++)
            {
                loadedChipData.componentChips[i].inputPins[inputIndex].pinName = componentToLoad.inputPins[inputIndex].name;
            }

            // Load output pin names
            for (int ouputIndex = 0; ouputIndex < componentToLoad.outputPinNames.Length; ouputIndex++)
            {
                loadedChipData.componentChips[i].outputPins[ouputIndex].pinName = componentToLoad.outputPinNames[ouputIndex];
            }
        }

        // Connect pins with wires
        for (int chipIndex = 0; chipIndex < chipToLoad.savedComponentChips.Length; chipIndex++)
        {
            var loadedComponentChip = loadedChipData.componentChips[chipIndex];
            for (int inputPinIndex = 0; inputPinIndex < loadedComponentChip.inputPins.Length; inputPinIndex++)
            {
                var savedPin = chipToLoad.savedComponentChips[chipIndex].inputPins[inputPinIndex];
                var pin = loadedComponentChip.inputPins[inputPinIndex];

                // If this pin should receive input from somewhere, then wire it up to that pin
                if (savedPin.parentChipIndex != -1)
                {
                    Pin connectedPin = loadedChipData.componentChips[savedPin.parentChipIndex].outputPins[savedPin.parentChipOutputIndex];
                    pin.cyclic = savedPin.isCylic;
                    Pin.TryConnect(connectedPin, pin);
                }
            }
        }

        return loadedChipData;
    }

    public static SavedWireLayout LoadWiringFile(string path)
    {
        using (var reader = new StreamReader(path))
        {
            string wiringSaveString = reader.ReadToEnd();
            return JsonUtility.FromJson<SavedWireLayout>(wiringSaveString);
        }
    }

    private static void SortChipsByOrderOfCreation(ref SavedChip[] chips)
    {
        chips = chips.OrderBy(c => c.creationIndex).ToArray();
    }
}