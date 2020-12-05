﻿using System.Collections.Generic;
using UnityEngine;

public class SavedChipEditor : MonoBehaviour
{
    public bool loadInEditMode;
    public string chipToEditName;
    public Wire wirePrefab;
    private GameObject loadedChipHolder;

    public void Load(string chipName, GameObject chipHolder)
    {
        var loadedChip = Instantiate(chipHolder);
        loadedChip.transform.parent = transform;

        var topLevelChips = new List<Chip>(GetComponentsInChildren<Chip>());

        var subChips = GetComponentsInChildren<CustomChip>(includeInactive: true);
        for (int i = 0; i < subChips.Length; i++)
        {
            if (subChips[i].transform.parent == loadedChip.transform)
            {
                subChips[i].gameObject.SetActive(true);
                topLevelChips.Add(subChips[i]);
            }
        }

        //topLevelChips.Sort ((a, b) => a.chipSaveIndex.CompareTo (b.chipSaveIndex));

        var wiringSaveData = ChipLoader.LoadWiringFile(SaveSystem.GetPathToWireSaveFile(chipName));
        var wireIndex = 0;
        foreach (var savedWire in wiringSaveData.serializableWires)
        {
            var loadedWire = Instantiate(wirePrefab, parent: loadedChip.transform);
            loadedWire.SetDepth(wireIndex);
            var parentPin = topLevelChips[savedWire.parentChipIndex].outputPins[savedWire.parentChipOutputIndex];
            var childPin = topLevelChips[savedWire.childChipIndex].inputPins[savedWire.childChipInputIndex];
            loadedWire.Connect(parentPin, childPin);
            loadedWire.SetAnchorPoints(savedWire.anchorPoints);

            FindObjectOfType<PinAndWireInteraction>().LoadWire(loadedWire);

            wireIndex++;
        }

        loadedChipHolder = loadedChip;
    }

    public void CaptureLoadedChip(GameObject chipHolder)
    {
        if (loadedChipHolder)
        {
            for (int i = loadedChipHolder.transform.childCount - 1; i >= 0; i--)
            {
                loadedChipHolder.transform.GetChild(i).parent = chipHolder.transform;
            }
            Destroy(loadedChipHolder);
        }
    }
}