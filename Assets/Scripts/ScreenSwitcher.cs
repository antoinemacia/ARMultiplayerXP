using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenSwitcher : Singleton<ScreenSwitcher> {

  private string sceneNameToBeLoaded;

  public void LoadScene (string _sceneName) {
    sceneNameToBeLoaded = _sceneName;

    // TODO - What is a co-routine?
    StartCoroutine (InitializeSceneLoading ());
  }

  // TODO - What is a IEnumerator
  IEnumerator InitializeSceneLoading () {
    // First, we load the loading scene asynchronously (so that
    // we can do things in the background)
    yield return SceneManager.LoadSceneAsync ("Scene_Loading");

    // Load the actual scene
    StartCoroutine (LoadActualScene ());
  }

  IEnumerator LoadActualScene () {
    var asyncSceneLoading = SceneManager.LoadSceneAsync (sceneNameToBeLoaded);

    // This calue stops the new scene from displaying when it is still loading...
    asyncSceneLoading.allowSceneActivation = false;

    while (!asyncSceneLoading.isDone) {

      Debug.Log (asyncSceneLoading.progress);
      if (asyncSceneLoading.progress >= 0.9f) {
        // Past 90% loading, start showing the scene
        asyncSceneLoading.allowSceneActivation = true;
      }

      yield return null;
    }
  }
}
