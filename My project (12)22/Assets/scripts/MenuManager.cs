using UnityEngine;
using UnityEngine.SceneManagement; // Обязательно добавьте эту строку!

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // Загружает сцену с игрой (проверьте, как называется ваша сцена с игрой, обычно SampleScene)
        SceneManager.LoadScene("Round1");
    }

    public void RestartGame()
    {
        // Перезагружает текущую активную сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
