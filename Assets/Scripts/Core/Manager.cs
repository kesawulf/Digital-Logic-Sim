using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager instance;

    public static ChipEditor ActiveChipEditor => instance.activeChipEditor;

    public event System.Action<Chip> customChipCreated;

    public ChipEditor chipEditorPrefab;
    public ChipPackage chipPackagePrefab;
    public Wire wirePrefab;
    public Chip[] builtinChips;

    private ChipEditor activeChipEditor;
    private int currentChipCreationIndex;

    private void Awake()
    {
        instance = this;
        activeChipEditor = FindObjectOfType<ChipEditor>();
        FindObjectOfType<CreateMenu>().onChipCreatePressed += SaveAndPackageChip;
    }

    private void Start()
    {
        SaveSystem.Init();
        SaveSystem.LoadAll(this);
    }

    public Chip LoadChip(ChipSaveData loadedChipData)
    {
        activeChipEditor.LoadFromSaveData(loadedChipData);
        currentChipCreationIndex = activeChipEditor.creationIndex;

        Chip loadedChip = PackageChip();
        LoadNewEditor();
        return loadedChip;
    }

    private void SaveAndPackageChip()
    {
        ChipSaver.Save(activeChipEditor);
        PackageChip();
        LoadNewEditor();
    }

    private Chip PackageChip()
    {
        ChipPackage package = Instantiate(chipPackagePrefab, parent: transform);
        package.PackageCustomChip(activeChipEditor);
        package.gameObject.SetActive(false);

        Chip customChip = package.GetComponent<Chip>();
        customChipCreated?.Invoke(customChip);
        currentChipCreationIndex++;
        return customChip;
    }

    private void LoadNewEditor()
    {
        if (activeChipEditor)
        {
            Destroy(activeChipEditor.gameObject);
        }
        activeChipEditor = Instantiate(chipEditorPrefab, Vector3.zero, Quaternion.identity);
        activeChipEditor.creationIndex = currentChipCreationIndex;
    }

    public void SpawnChip(Chip chip)
    {
        activeChipEditor.chipInteraction.SpawnChip(chip);
    }

    public void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}