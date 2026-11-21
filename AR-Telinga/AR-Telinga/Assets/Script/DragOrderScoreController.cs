using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class DragOrderScoreController : MonoBehaviour
{
    [Header("Trigger")]
    public Button btnNext;

    [Header("Score Panel")]
    public GameObject panelSkor;
    public TextMeshProUGUI textSkor;
    public Button btnKembaliSkor;
    public string namaSceneKembali = "menu_utama";

    [Header("Objects To Disable")]
    [Tooltip("Isi dengan root object yang harus dinonaktifkan saat panel skor tampil. Jangan masukkan panel skor di sini.")]
    public List<GameObject> objekDinonaktifkanSaatSkor = new List<GameObject>();

    [Header("Score")]
    public int poinPerBenar = 10;

    private bool panelSkorDitampilkan;
    private DropImageSlot[] semuaSlot;

    void Start()
    {
        semuaSlot = FindObjectsOfType<DropImageSlot>(true);
        panelSkorDitampilkan = false;

        if (panelSkor)
            panelSkor.SetActive(false);

        SetupNextProxy();

        if (btnKembaliSkor)
        {
            btnKembaliSkor.onClick.RemoveAllListeners();
            btnKembaliSkor.onClick.AddListener(KembaliKeMenu);
        }

        UpdateTampilanSkor(0);
    }

    private void SetupNextProxy()
    {
        if (btnNext == null)
            return;

        btnNext.onClick.RemoveAllListeners();
        btnNext.enabled = false;

        var graphic = btnNext.targetGraphic;
        if (graphic != null)
            graphic.raycastTarget = true;

        var trigger = btnNext.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = btnNext.gameObject.AddComponent<EventTrigger>();

        if (trigger.triggers == null)
            trigger.triggers = new List<EventTrigger.Entry>();
        else
            trigger.triggers.Clear();

        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener(_ => OnNextClicked());
        trigger.triggers.Add(entry);
    }

    private void OnNextClicked()
    {
        int jumlahBenar = HitungJawabanBenar();
        int skor = jumlahBenar * poinPerBenar;

        TampilkanPanelSkor(skor);
    }

    private int HitungJawabanBenar()
    {
        if (semuaSlot == null)
            return 0;

        int jumlah = 0;

        foreach (var slot in semuaSlot)
        {
            if (slot == null || slot.currentItem == null)
                continue;

            if (slot.currentItem.correctSlotIndex == slot.slotIndex)
                jumlah++;
        }

        return jumlah;
    }

    private void TampilkanPanelSkor(int skor)
    {
        if (panelSkorDitampilkan)
            return;

        panelSkorDitampilkan = true;

        if (btnNext)
            btnNext.interactable = false;

        if (panelSkor)
            panelSkor.SetActive(true);

        foreach (var objek in objekDinonaktifkanSaatSkor)
        {
            if (objek != null)
                objek.SetActive(false);
        }

        UpdateTampilanSkor(skor);
        MakePanelSkorNonBlocking();
    }

    private void UpdateTampilanSkor(int skor)
    {
        if (textSkor)
            textSkor.text = "Skor: " + skor;
    }

    private void MakePanelSkorNonBlocking()
    {
        if (panelSkor == null)
            return;

        var graphics = panelSkor.GetComponentsInChildren<Graphic>(true);
        foreach (var graphic in graphics)
        {
            if (btnKembaliSkor != null && graphic.transform.IsChildOf(btnKembaliSkor.transform))
                continue;

            graphic.raycastTarget = false;
        }
    }

    private void KembaliKeMenu()
    {
        if (!string.IsNullOrEmpty(namaSceneKembali))
            SceneManager.LoadScene(namaSceneKembali);
    }
}