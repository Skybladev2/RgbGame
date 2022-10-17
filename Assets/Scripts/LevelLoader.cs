using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private IConfig config = new Config();

    void Start()
    {
        LevelCreator levelCreator = new LevelCreator(config.GetNodeDistance(),
            Levels.GetLevelDescriptions(),
             prefabName =>
             {
                 GameObject clone = (GameObject)Instantiate(Resources.Load(prefabName));
                 clone.name = clone.name.Replace("(Clone)", "");
                 return clone;
             });

        SceneManager.LoadScene("LevelContainer");
        levelCreator.CreateLevel(0);
    }
}