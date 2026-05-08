using UnityEngine;
using UnityEngine.SceneManagement; // Нужна для загрузки сцен

public class SceneLoader : MonoBehaviour
{
    // Этот метод будет вызываться при нажатии кнопки "Выход"
    public void LoadMainMenu()
    {
        // Название сцены главного меню. 
        // Замени "MainMenu" на реальное название твоей сцены с главным меню.
        SceneManager.LoadScene("MainMenu");
    }
}