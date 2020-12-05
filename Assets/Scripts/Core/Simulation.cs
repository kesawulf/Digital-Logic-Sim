using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public static int simulationFrame { get; private set; }

    private InputSignal[] inputSignals;
    private ChipEditor chipEditor;

    public float minStepTime = 0.075f;
    private float lastStepTime;

    private void Awake()
    {
        simulationFrame = 0;
    }

    private void Update()
    {
        if (Time.time - lastStepTime > minStepTime)
        {
            lastStepTime = Time.time;
            StepSimulation();
        }
    }

    private void StepSimulation()
    {
        simulationFrame++;
        RefreshChipEditorReference();

        // Clear output signals
        var outputSignals = chipEditor.outputsEditor.signals;
        for (int i = 0; i < outputSignals.Count; i++)
        {
            outputSignals[i].SetDisplayState(0);
        }

        // Init chips
        var allChips = chipEditor.chipInteraction.allChips;
        for (int i = 0; i < allChips.Count; i++)
        {
            allChips[i].InitSimulationFrame();
        }

        // Process inputs
        var inputSignals = chipEditor.inputsEditor.signals;
        // Tell all signal generators to send their signal out
        for (int i = 0; i < inputSignals.Count; i++)
        {
            ((InputSignal)inputSignals[i]).SendSignal();
        }
    }

    private void RefreshChipEditorReference()
    {
        if (chipEditor == null)
        {
            chipEditor = FindObjectOfType<ChipEditor>();
        }
    }
}