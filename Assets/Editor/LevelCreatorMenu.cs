using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class LevelCreatorMenu : ScriptableWizard
{
    private IConfig config = new Config();
    private static List<LevelDescription> levelDescriptions;

    [MenuItem("Level creator/Show window")]
    public static void ShowWindow()
    {
        ScriptableWizard.DisplayWizard<LevelCreatorMenu>("Level creator", "Create levels");
        LevelCreatorMenu window = (LevelCreatorMenu)EditorWindow.GetWindow(typeof(LevelCreatorMenu));
        window.titleContent = new GUIContent("Level creator");
        levelDescriptions = Levels.GetLevelDescriptions();
    }

    void OnWizardCreate()
    {
        for (int i = 0; i < levelDescriptions.Count; i++)
        {
            LevelCreator creator = new LevelCreator(config.GetNodeDistance(), 
                levelDescriptions,
                prefabName => (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath("Assets/Resources/" + prefabName + ".prefab", typeof(GameObject))));
            creator.CreateLevel(i);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "Assets/Scenes/Levels/Level" + (i + 1) + ".unity", false);
        }
    }
}
