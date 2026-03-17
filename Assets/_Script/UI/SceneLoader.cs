using UnityEngine.SceneManagement;
public static class SceneLoader {
    public enum Scene
    {
        MainMenuScene,
        GameScene,
        GameOverScene
    }

    // Transient (runtime) request for GameScene
    public static bool IsContinue { get; set; }
    public static int RequestedLevelIndex { get; set; }

    public static void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
}

