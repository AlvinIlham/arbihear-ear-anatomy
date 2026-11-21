using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Fungsi untuk ganti scene
    public void LoadToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Fungsi untuk keluar dari aplikasi
    public void QuitGame()
    {
        Debug.Log("Game exited!"); // pesan ini hanya muncul di Editor
        Application.Quit();        // keluar saat game sudah di-build (bukan di editor)
    }
}
