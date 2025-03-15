using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class MySceneManager
    {

        public enum Scenes
        {
            GameScene, MainMenuScene
        }
        
        public static AsyncOperation LoadSceneAsync(Scenes scene)
        {
            return SceneManager.LoadSceneAsync(scene.ToString());
        }

        public static AsyncOperation LoadSceneAsync(string sceneName)
        {
            return SceneManager.LoadSceneAsync(sceneName);
        }
        
        public static void LoadScene(Scenes scene)
        {
            SceneManager.LoadScene(scene.ToString());
        }

        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}