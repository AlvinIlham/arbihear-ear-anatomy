using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Soal
    {
        [Header("Panel & Opsi")]
        public GameObject panelSoal;
        public Button opsiA;
        public Button opsiB;
        public Button opsiC;
        [Tooltip("Huruf besar: A / B / C")]
        public string jawabanBenar = "A";

        [Header("Navigasi (di panel ini)")]
        public Button btnNext;      // boleh null
        public Button btnPrevious;  // boleh null
        public Button btnKembali;   // sebaiknya ada

        [Header("Ikon (di panel ini)")]
        public GameObject iconBenar;
        public GameObject iconSalah;
    }

    [Header("Konfigurasi")]
    public Soal[] daftarSoal;
    public string namaSceneKembali = "menu_utama";

    [Header("Skor")]
    public GameObject panelSkor;
    public TextMeshProUGUI textSkor;
    public Button btnKembaliSkor;
    public int poinPerBenar = 10;

    private int indexSoal = 0;
    private int skor = 0;
    private bool[] soalSudahDiberiSkor;

    void Start()
    {
        skor = 0;
        soalSudahDiberiSkor = new bool[daftarSoal.Length];

        for (int i = 0; i < daftarSoal.Length; i++)
        {
            if (daftarSoal[i].panelSoal) daftarSoal[i].panelSoal.SetActive(false);

            // pastikan ikon tidak memblokir klik (matikan semua raycast di bawah ikon)
            MakeIconNonBlocking(daftarSoal[i]);

            // sembunyikan ikon
            SetIcons(daftarSoal[i], false, false);

            int idx = i;

            // setup tombol jawaban (tidak pernah dikunci)
            if (daftarSoal[i].opsiA) { daftarSoal[i].opsiA.onClick.RemoveAllListeners(); daftarSoal[i].opsiA.onClick.AddListener(() => Jawab(idx, "A")); }
            if (daftarSoal[i].opsiB) { daftarSoal[i].opsiB.onClick.RemoveAllListeners(); daftarSoal[i].opsiB.onClick.AddListener(() => Jawab(idx, "B")); }
            if (daftarSoal[i].opsiC) { daftarSoal[i].opsiC.onClick.RemoveAllListeners(); daftarSoal[i].opsiC.onClick.AddListener(() => Jawab(idx, "C")); }

            // setup navigasi
            if (daftarSoal[i].btnNext)     { daftarSoal[i].btnNext.onClick.RemoveAllListeners();     daftarSoal[i].btnNext.onClick.AddListener(() => PindahSoal(idx + 1)); }
            if (daftarSoal[i].btnPrevious) { daftarSoal[i].btnPrevious.onClick.RemoveAllListeners(); daftarSoal[i].btnPrevious.onClick.AddListener(() => PindahSoal(idx - 1)); }
            if (daftarSoal[i].btnKembali)  { daftarSoal[i].btnKembali.onClick.RemoveAllListeners();  daftarSoal[i].btnKembali.onClick.AddListener(KembaliKeMenu); }
        }

        if (panelSkor) panelSkor.SetActive(true);
        if (panelSkor) panelSkor.SetActive(false);
        if (panelSkor) MakePanelSkorNonBlocking(panelSkor);

        if (btnKembaliSkor)
        {
            btnKembaliSkor.onClick.RemoveAllListeners();
            btnKembaliSkor.onClick.AddListener(KembaliKeMenu);
        }

        UpdateTampilanSkor();
        TampilkanSoal(0);
    }

    void TampilkanSoal(int index)
    {
        if (panelSkor) panelSkor.SetActive(false);

        indexSoal = Mathf.Clamp(index, 0, daftarSoal.Length - 1);

        for (int i = 0; i < daftarSoal.Length; i++)
        {
            bool aktif = (i == indexSoal);
            if (daftarSoal[i].panelSoal) daftarSoal[i].panelSoal.SetActive(aktif);

            if (aktif)
            {
                // reset ikon & pastikan tombol aktif lagi
                SetIcons(daftarSoal[i], false, false);
                SetOpsiInteractable(daftarSoal[i], true);
                MakeIconNonBlocking(daftarSoal[i]);
            }
        }

        if (daftarSoal[indexSoal].btnNext)
            daftarSoal[indexSoal].btnNext.gameObject.SetActive(true);
        if (daftarSoal[indexSoal].btnPrevious)
            daftarSoal[indexSoal].btnPrevious.gameObject.SetActive(indexSoal > 0);
    }

    void PindahSoal(int targetIndex)
    {
        if (targetIndex < 0) return;

        if (targetIndex >= daftarSoal.Length)
        {
            TampilkanSkorAkhir();
            return;
        }

        TampilkanSoal(targetIndex);
    }

    void TampilkanSkorAkhir()
    {
        for (int i = 0; i < daftarSoal.Length; i++)
        {
            if (daftarSoal[i].panelSoal) daftarSoal[i].panelSoal.SetActive(false);
        }

        if (panelSkor) panelSkor.SetActive(true);
        UpdateTampilanSkor();
    }

    void Jawab(int idxSoal, string pilihan)
    {
        // pastikan ikon tidak memblokir klik
        MakeIconNonBlocking(daftarSoal[idxSoal]);

        // jangan pernah kunci tombol; tetap interactable
        SetOpsiInteractable(daftarSoal[idxSoal], true);

        // tampilkan ikon 1 detik lalu hilangkan
        bool benar = pilihan == daftarSoal[idxSoal].jawabanBenar;
        if (benar && !soalSudahDiberiSkor[idxSoal])
        {
            skor += poinPerBenar;
            soalSudahDiberiSkor[idxSoal] = true;
            UpdateTampilanSkor();
        }

        StopAllCoroutines(); // jaga-jaga kalau user klik cepat beruntun
        StartCoroutine(TampilkanIconSementara(daftarSoal[idxSoal], benar));
    }

    IEnumerator TampilkanIconSementara(Soal s, bool benar)
    {
        if (benar) SetIcons(s, true, false);
        else       SetIcons(s, false, true);

        yield return new WaitForSeconds(1f);

        SetIcons(s, false, false);

        // tombol tetap aktif sehingga user bisa klik lagi
        SetOpsiInteractable(s, true);
    }

    // ---------- helpers ----------
    void SetIcons(Soal s, bool showBenar, bool showSalah)
    {
        if (s.iconBenar) s.iconBenar.SetActive(showBenar);
        if (s.iconSalah) s.iconSalah.SetActive(showSalah);
    }

    void SetOpsiInteractable(Soal s, bool state)
    {
        if (s.opsiA) s.opsiA.interactable = state;
        if (s.opsiB) s.opsiB.interactable = state;
        if (s.opsiC) s.opsiC.interactable = state;
    }

    void UpdateTampilanSkor()
    {
        if (textSkor) textSkor.text = "Skor: " + skor;
    }

    void MakePanelSkorNonBlocking(GameObject panel)
    {
        var graphics = panel.GetComponentsInChildren<Graphic>(true);
        foreach (var graphic in graphics)
        {
            if (btnKembaliSkor != null && graphic.transform.IsChildOf(btnKembaliSkor.transform))
                continue;

            graphic.raycastTarget = false;
        }
    }

    // Matikan semua raycast target pada komponen UI di bawah ikon
    void MakeIconNonBlocking(Soal s)
    {
        if (s.iconBenar)
        {
            var gs = s.iconBenar.GetComponentsInChildren<Graphic>(true);
            foreach (var g in gs) g.raycastTarget = false;
        }
        if (s.iconSalah)
        {
            var gs = s.iconSalah.GetComponentsInChildren<Graphic>(true);
            foreach (var g in gs) g.raycastTarget = false;
        }
    }

    void KembaliKeMenu()
    {
        if (!string.IsNullOrEmpty(namaSceneKembali))
            SceneManager.LoadScene(namaSceneKembali);
        else
            Debug.LogWarning("Nama scene kembali belum diisi.");
    }
}
