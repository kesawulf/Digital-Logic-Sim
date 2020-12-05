using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadProjectMenu : MonoBehaviour
{
    public Button projectButtonPrefab;
    public Transform scrollHolder;

    [SerializeField, HideInInspector]
    private List<Button> loadButtons;

    private void OnEnable()
    {
        string[] projectNames = SaveSystem.GetSaveNames();

        for (int i = 0; i < projectNames.Length; i++)
        {
            string projectName = projectNames[i];
            if (i >= loadButtons.Count)
            {
                loadButtons.Add(Instantiate(projectButtonPrefab, parent: scrollHolder));
            }
            Button loadButton = loadButtons[i];
            loadButton.GetComponentInChildren<TMPro.TMP_Text>().text = projectName.Trim();
            loadButton.onClick.AddListener(() => LoadProject(projectName));
        }
    }

    public void LoadProject(string projectName)
    {
        SaveSystem.SetActiveProject(projectName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}