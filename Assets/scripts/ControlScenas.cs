using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // M�todo para cargar la escena del �tico
    public void ChangeSceneToAtico()
    {
        SceneManager.LoadScene("Atico 1");
    }

    // M�todo para cargar la escena fueteSirenas
    public void ChangeSceneToFueteSirenas()
    {
        SceneManager.LoadScene("fueteSirenas");
    }
}
