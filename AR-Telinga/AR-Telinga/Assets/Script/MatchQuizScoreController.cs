using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MatchQuizScoreController : MonoBehaviour
{
    [Header("Trigger")]
    public Button btnNext;
    public ConnectionManager connectionManager;

    [Header("Score Panel")]
    public GameObject panelSkor;
    public TextMeshProUGUI textSkor;
    public Button btnKembaliSkor;
    public string namaSceneKembali = "menu_utama";

    [Header("Objects To Disable")]
    [Tooltip("Isi dengan root object yang harus dinonaktifkan saat panel skor tampil. Jangan masukkan panel skor di sini.")]
    public List<GameObject> objekDinonaktifkanSaatSkor = new List<GameObject>();

    [Header("Score")]
    public int skorAwal = 0;
    public int poinPerBenar = 10;

    private int skor;
    private bool panelSkorDitampilkan;

    void Start()
    {
        skor = skorAwal;
        panelSkorDitampilkan = false;

        if (panelSkor)
            panelSkor.SetActive(false);

        if (btnNext)
        {
            btnNext.onClick.RemoveAllListeners();
            btnNext.onClick.AddListener(OnNextClicked);
        }

        if (btnKembaliSkor)
        {
            btnKembaliSkor.onClick.RemoveAllListeners();
            btnKembaliSkor.onClick.AddListener(KembaliKeMenu);
        }

        UpdateTampilanSkor();
    }

    public void SetSkor(int nilai)
    {
        skor = nilai;
        UpdateTampilanSkor();
    }

    public void TambahSkor(int nilai)
    {
        skor += nilai;
        UpdateTampilanSkor();
    }

    private void OnNextClicked()
    {
        if (connectionManager != null)
            skor = connectionManager.GetCorrectConnectionCount() * poinPerBenar;

        TampilkanPanelSkor();
    }

    private void TampilkanPanelSkor()
    {
        if (panelSkorDitampilkan)
            return;

        panelSkorDitampilkan = true;

        if (connectionManager != null)
            connectionManager.ClearAllConnections();

        foreach (var objek in objekDinonaktifkanSaatSkor)
        {
            if (objek != null)
                objek.SetActive(false);
        }

        if (panelSkor)
            panelSkor.SetActive(true);

        UpdateTampilanSkor();
    }

    private void UpdateTampilanSkor()
    {
        if (textSkor)
            textSkor.text = "Skor: " + skor;
    }

    private void KembaliKeMenu()
    {
        if (!string.IsNullOrEmpty(namaSceneKembali))
            SceneManager.LoadScene(namaSceneKembali);
    }
}