using System.Collections;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;
    void Start()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void OnPressedPlay()
    {
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        AsyncOperation a = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
        a.allowSceneActivation = false;
        yield return new WaitForEndOfFrame();
        a.allowSceneActivation = true;
    }
}
