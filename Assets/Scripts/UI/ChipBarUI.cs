﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChipBarUI : MonoBehaviour
{
    public RectTransform bar;
    public Transform buttonHolder;
    public CustomButton buttonPrefab;
    public float buttonSpacing = 15f;
    public float buttonWidthPadding = 10;
    private float rightmostButtonEdgeX;
    private Manager manager;
    public List<string> hideList;
    public Scrollbar horizontalScroll;

    private void Awake()
    {
        manager = FindObjectOfType<Manager>();
        manager.customChipCreated += AddChipButton;
        for (int i = 0; i < manager.builtinChips.Length; i++)
        {
            AddChipButton(manager.builtinChips[i]);
        }

        Canvas.ForceUpdateCanvases();
    }

    private void LateUpdate()
    {
        UpdateBarPos();
    }

    private void UpdateBarPos()
    {
        float barPosY = (horizontalScroll.gameObject.activeSelf) ? 16 : 0;
        bar.localPosition = new Vector3(0, barPosY, 0);
    }

    private void AddChipButton(Chip chip)
    {
        if (hideList.Contains(chip.chipName))
        {
            //Debug.Log("Hiding")
            return;
        }
        var button = Instantiate(buttonPrefab);
        button.gameObject.name = "Create (" + chip.chipName + ")";

        // Set button text
        var buttonTextUI = button.GetComponentInChildren<TMP_Text>();
        buttonTextUI.text = chip.chipName;

        // Set button size
        var buttonRect = button.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(buttonTextUI.preferredWidth + buttonWidthPadding, buttonRect.sizeDelta.y);

        // Set button position
        buttonRect.SetParent(buttonHolder, false);
        //buttonRect.localPosition = new Vector3 (rightmostButtonEdgeX + buttonSpacing + buttonRect.sizeDelta.x / 2f, 0, 0);
        rightmostButtonEdgeX = buttonRect.localPosition.x + (buttonRect.sizeDelta.x / 2f);

        // Set button event
        //button.onClick.AddListener (() => manager.SpawnChip (chip));
        button.onPointerDown += (() => manager.SpawnChip(chip));
    }
}