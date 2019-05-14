using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SD
{
    public class SceneLoader : MonoBehaviour
    {
        public string nameOfSceneToLoad;

        public void LoadScene()
        {
            SceneManager.LoadScene(nameOfSceneToLoad);
        }
    }
}
