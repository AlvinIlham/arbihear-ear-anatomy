using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using Vuforia;

/// <summary>
/// Attach script ini pada Canvas (atau GameObject lain) di dalam marker.
/// ObserverBehaviour dicari otomatis ke parent.
/// Setiap item memiliki tombol pemicu dan konten (objek/video/panel).
/// Saat konten ditampilkan, objek utama disembunyikan, dan sebaliknya.
/// </summary>
public class MarkerContentController : MonoBehaviour
{
    [System.Serializable]
    public class MarkerContent
    {
        [Tooltip("Tombol pemicu untuk menampilkan / menyembunyikan konten ini")]
        public Button triggerButton;

        [Tooltip("Objek 3D yang ditampilkan saat tombol ditekan")]
        public GameObject targetObject;

        [Tooltip("Isi jika ada VideoPlayer yang perlu diputar/dihentikan")]
        public VideoPlayer videoPlayer;

        [Tooltip("Putar video otomatis saat konten ditampilkan")]
        public bool autoPlayVideo = true;
    }

    [Header("Objek Utama")]
    [Tooltip("Daftar objek utama yang ditampilkan saat tidak ada konten aktif. " +
             "Semua akan disembunyikan saat konten aktif, dan ditampilkan kembali saat konten ditutup.")]
    public List<GameObject> mainObjects = new List<GameObject>();

    [Header("Daftar Konten")]
    [Tooltip("Tambahkan sebanyak yang dibutuhkan. Setiap item punya tombol pemicu dan kontennya.")]
    public List<MarkerContent> contents = new List<MarkerContent>();

    [Header("Pengaturan")]
    [Tooltip("Sembunyikan semua konten dan tampilkan objek utama saat scene dimuat")]
    public bool resetOnStart = true;

    // ------------------------------------------------------------------ //

    private ObserverBehaviour _observer;

    void Start()
    {
        // Cari ObserverBehaviour di parent (karena script ada di Canvas dalam marker)
        _observer = GetComponentInParent<ObserverBehaviour>();

        if (_observer != null)
        {
            _observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }
        else
        {
            Debug.LogWarning($"[MarkerContentController] ObserverBehaviour tidak ditemukan di parent {gameObject.name}. " +
                             "Pastikan script ini berada di dalam hierarchy ImageTarget.");
        }

        // Daftarkan tombol pemicu
        for (int i = 0; i < contents.Count; i++)
        {
            int idx = i;
            if (contents[i].triggerButton != null)
                contents[i].triggerButton.onClick.AddListener(() => OnTriggerButtonClicked(idx));
        }

        if (resetOnStart)
            ResetToMain();
    }

    void OnDestroy()
    {
        if (_observer != null)
            _observer.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    // ------------------------------------------------------------------ //

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool tracked = status.Status == Status.TRACKED ||
                       status.Status == Status.EXTENDED_TRACKED ||
                       status.Status == Status.LIMITED;

        if (!tracked)
            ResetToMain();
    }

    // ------------------------------------------------------------------ //

    /// <summary>
    /// Dipanggil saat tombol pemicu ditekan.
    /// Jika konten sedang tersembunyi → tampilkan konten, sembunyikan mainObject.
    /// Jika konten sedang aktif → sembunyikan konten, tampilkan kembali mainObject.
    /// </summary>
    private void OnTriggerButtonClicked(int index)
    {
        if (index < 0 || index >= contents.Count) return;

        var item = contents[index];

        // Jika targetObject kosong tapi videoPlayer ada, gunakan gameObject videoPlayer sebagai target
        if (item.targetObject == null && item.videoPlayer != null)
            item.targetObject = item.videoPlayer.gameObject;

        if (item.targetObject == null) return;

        bool willShow = !item.targetObject.activeSelf;

        if (willShow)
        {
            // Sembunyikan semua konten lain terlebih dahulu
            HideAllContents();

            // Sembunyikan semua objek utama
            foreach (var obj in mainObjects)
                if (obj != null) obj.SetActive(false);

            // Tampilkan konten ini
            item.targetObject.SetActive(true);
            if (item.videoPlayer != null && item.autoPlayVideo)
            {
                item.videoPlayer.Stop();
                item.videoPlayer.Play();
            }
        }
        else
        {
            // Sembunyikan konten ini → kembali ke objek utama
            HideContent(item);
            foreach (var obj in mainObjects)
                if (obj != null) obj.SetActive(true);
        }
    }

    // ------------------------------------------------------------------ //

    /// <summary>Tampilkan semua objek utama dan sembunyikan semua konten.</summary>
    public void ResetToMain()
    {
        HideAllContents();
        foreach (var obj in mainObjects)
            if (obj != null) obj.SetActive(true);
    }

    /// <summary>Sembunyikan semua konten dan hentikan semua video.</summary>
    public void HideAllContents()
    {
        foreach (var item in contents)
            HideContent(item);
    }

    private void HideContent(MarkerContent item)
    {
        if (item.targetObject == null) return;
        if (item.videoPlayer != null)
            item.videoPlayer.Stop();
        item.targetObject.SetActive(false);
    }

    /// <summary>Aktifkan/nonaktifkan konten tertentu by index (0-based) secara manual.</summary>
    public void SetContentActive(int index, bool active)
    {
        if (index < 0 || index >= contents.Count) return;
        OnTriggerButtonClicked(index); // gunakan logika yang sama
    }
}
