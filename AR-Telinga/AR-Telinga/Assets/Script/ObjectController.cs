using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using System.Collections.Generic;

/// <summary>
/// Controller untuk ZoomIn, ZoomOut, Rotate Kiri-Kanan, Rotate Kanan-Kiri,
/// dan Play/Pause animasi pada objek telinga-full.
/// Tahan tombol zoom/rotasi = aksi berlanjut | Klik tombol Play = toggle animasi.
/// </summary>
public class ObjectController : MonoBehaviour
{
    [Header("Objek yang Dikontrol")]
    [Tooltip("Assign root GameObject marker Telinga Full (ImageTarget).")]
    public GameObject markerRoot;
    [Tooltip("Assign root GameObject marker Telinga Luar (ImageTarget).")]
    public GameObject markerLuar;
    [Tooltip("Assign root GameObject marker Telinga Tengah (ImageTarget).")]
    public GameObject markerTengah;
    [Tooltip("Assign root GameObject marker Telinga Dalam (ImageTarget).")]
    public GameObject markerDalam;

    [Header("Tombol Kontrol")]
    public Button btnZoomIn;
    public Button btnZoomOut;
    public Button btnRotate1;   // Rotasi kiri ke kanan (Y+)
    public Button btnRotate2;   // Rotasi kanan ke kiri (Y-)
    public Button btnPlay;      // Play / Pause animasi telinga-full
    public Button btnSeru;      // Tampilkan video proses pendengaran

    [Header("Tombol Bagian Telinga")]
    public Button btnLuar;      // Tombol tampilkan telinga luar
    public Button btnTengah;    // Tombol tampilkan telinga tengah
    public Button btnDalam;     // Tombol tampilkan telinga dalam

    [Header("Video Proses Pendengaran")]
    public GameObject videoPanel;   // Panel UI yang memuat video player
    public VideoPlayer videoPlayer; // Komponen VideoPlayer di dalam videoPanel
    public Button btnCloseVideo;    // Tombol tutup panel video

    [Header("Pengaturan")]
    [Tooltip("Kecepatan rotasi (derajat per detik)")]
    public float rotateSpeed = 90f;

    [Tooltip("Kecepatan zoom (faktor perkalian per detik, misal 0.5 = 50%/detik)")]
    public float zoomSpeed = 0.5f;

    [Tooltip("Faktor skala minimum relatif dari skala awal (0.2 = 20% dari skala awal)")]
    public float minScale = 0.2f;

    [Tooltip("Faktor skala maksimum relatif dari skala awal (5 = 500% dari skala awal)")]
    public float maxScale = 5f;

    // Status tombol yang sedang ditekan
    private bool isZoomingIn   = false;
    private bool isZoomingOut  = false;
    private bool isRotating1   = false;
    private bool isRotating2   = false;

    private Vector3 initialScaleRoot;
    private Vector3 initialScaleLuar;
    private Vector3 initialScaleTengah;
    private Vector3 initialScaleDalam;
    private float currentZoomFactor = 1f;

    private Transform rotationAnchorRoot;
    private Transform rotationAnchorLuar;
    private Transform rotationAnchorTengah;
    private Transform rotationAnchorDalam;

    [Header("Animasi yang Dikontrol")]
    [Tooltip("Isi dengan semua objek yang memiliki Animator dan ingin dikontrol oleh tombol Play.")]
    public Animator[] animatorsToControl;

    private bool isPlaying = false;

    // ------------------------------------------------------------------ //

    void Start()
    {
        // Simpan skala awal semua marker
        if (markerRoot   != null) initialScaleRoot   = markerRoot.transform.localScale;
        if (markerLuar   != null) initialScaleLuar   = markerLuar.transform.localScale;
        if (markerTengah != null) initialScaleTengah = markerTengah.transform.localScale;
        if (markerDalam  != null) initialScaleDalam  = markerDalam.transform.localScale;

        // Jika belum diisi manual, ambil semua Animator di bawah markerRoot
        if (animatorsToControl == null || animatorsToControl.Length == 0)
        {
            if (markerRoot != null)
                animatorsToControl = markerRoot.GetComponentsInChildren<Animator>();
        }

        // Mulai dalam kondisi pause
        SetAnimationSpeed(0f);

        // Daftarkan event tahan & lepas untuk tombol zoom & rotasi
        RegisterHoldButton(btnZoomIn,  () => isZoomingIn  = true,  () => isZoomingIn  = false);
        RegisterHoldButton(btnZoomOut, () => isZoomingOut = true,  () => isZoomingOut = false);
        RegisterHoldButton(btnRotate1, () => isRotating1  = true,  () => isRotating1  = false);
        RegisterHoldButton(btnRotate2, () => isRotating2  = true,  () => isRotating2  = false);

        // Daftarkan tombol Play sebagai toggle klik biasa
        if (btnPlay != null)
            btnPlay.onClick.AddListener(ToggleAnimation);

        // Daftarkan tombol Seru untuk toggle video panel
        if (btnSeru != null)
            btnSeru.onClick.AddListener(ToggleVideoPanel);

        // Daftarkan tombol tutup video
        if (btnCloseVideo != null)
            btnCloseVideo.onClick.AddListener(CloseVideoPanel);

        // Sembunyikan panel video saat awal
        if (videoPanel != null)
            videoPanel.SetActive(false);

        // Buat anchor rotasi di bawah marker agar isi marker bisa diputar tanpa ditimpa tracking.
        rotationAnchorRoot   = EnsureRotationAnchor(markerRoot);
        rotationAnchorLuar   = EnsureRotationAnchor(markerLuar);
        rotationAnchorTengah = EnsureRotationAnchor(markerTengah);
        rotationAnchorDalam  = EnsureRotationAnchor(markerDalam);
    }

    // ------------------------------------------------------------------ //

    void Update()
    {
        if (markerRoot == null && markerLuar == null && markerTengah == null && markerDalam == null) return;

        if (isZoomingIn)  ApplyZoom(+zoomSpeed * Time.deltaTime);
        if (isZoomingOut) ApplyZoom(-zoomSpeed * Time.deltaTime);
        if (isRotating1)  ApplyRotation(+rotateSpeed * Time.deltaTime);
        if (isRotating2)  ApplyRotation(-rotateSpeed * Time.deltaTime);
    }

    // ------------------------------------------------------------------ //

    /// <summary>Perbesar / perkecil semua marker secara seragam.</summary>
    private void ApplyZoom(float delta)
    {
        currentZoomFactor += delta;
        currentZoomFactor = Mathf.Clamp(currentZoomFactor, minScale, maxScale);
        if (markerRoot   != null) markerRoot.transform.localScale   = initialScaleRoot   * currentZoomFactor;
        if (markerLuar   != null) markerLuar.transform.localScale   = initialScaleLuar   * currentZoomFactor;
        if (markerTengah != null) markerTengah.transform.localScale = initialScaleTengah * currentZoomFactor;
        if (markerDalam  != null) markerDalam.transform.localScale  = initialScaleDalam  * currentZoomFactor;
    }

    /// <summary>Toggle play / pause animasi di dalam marker.</summary>
    private void ToggleAnimation()
    {
        isPlaying = !isPlaying;
        SetAnimationSpeed(isPlaying ? 1f : 0f);
    }

    private void SetAnimationSpeed(float speed)
    {
        if (animatorsToControl == null) return;

        for (int i = 0; i < animatorsToControl.Length; i++)
        {
            Animator animator = animatorsToControl[i];
            if (animator != null)
                animator.speed = speed;
        }
    }

    /// <summary>Putar semua marker root sehingga seluruh isi di dalamnya ikut berputar.</summary>
    private void ApplyRotation(float degrees)
    {
        Transform targetRoot = rotationAnchorRoot != null ? rotationAnchorRoot : (markerRoot != null ? markerRoot.transform : null);
        Transform targetLuar = rotationAnchorLuar != null ? rotationAnchorLuar : (markerLuar != null ? markerLuar.transform : null);
        Transform targetTengah = rotationAnchorTengah != null ? rotationAnchorTengah : (markerTengah != null ? markerTengah.transform : null);
        Transform targetDalam = rotationAnchorDalam != null ? rotationAnchorDalam : (markerDalam != null ? markerDalam.transform : null);

        RotateTarget(targetRoot, degrees);
        RotateTarget(targetLuar, degrees);
        RotateTarget(targetTengah, degrees);
        RotateTarget(targetDalam, degrees);
    }

    private void RotateTarget(Transform target, float degrees)
    {
        if (target == null) return;

        // Rotasi diterapkan ke anchor/grup isi marker, bukan ke root tracking.
        target.transform.Rotate(0f, degrees, 0f, Space.Self);
    }

    private Transform EnsureRotationAnchor(GameObject marker)
    {
        if (marker == null) return null;

        Transform existingAnchor = marker.transform.Find("RotationAnchor");
        if (existingAnchor != null)
            return existingAnchor;

        GameObject anchorObject = new GameObject("RotationAnchor");
        Transform anchorTransform = anchorObject.transform;
        anchorTransform.SetParent(marker.transform, false);
        anchorTransform.localPosition = Vector3.zero;
        anchorTransform.localRotation = Quaternion.identity;
        anchorTransform.localScale = Vector3.one;

        List<Transform> childrenToMove = new List<Transform>();
        for (int i = 0; i < marker.transform.childCount; i++)
        {
            Transform child = marker.transform.GetChild(i);
            if (child == anchorTransform) continue;
            childrenToMove.Add(child);
        }

        for (int i = 0; i < childrenToMove.Count; i++)
        {
            childrenToMove[i].SetParent(anchorTransform, true);
        }

        return anchorTransform;
    }

    // ------------------------------------------------------------------ //

    /// <summary>Aktifkan/nonaktifkan seluruh isi semua marker sekaligus.</summary>
    private void SetObjectsActive(bool active)
    {
        if (markerRoot   != null) markerRoot.SetActive(active);
        if (markerLuar   != null) markerLuar.SetActive(active);
        if (markerTengah != null) markerTengah.SetActive(active);
        if (markerDalam  != null) markerDalam.SetActive(active);
    }

    /// <summary>Toggle tampilan panel video (buka jika tertutup, tutup jika terbuka).
    /// Saat video aktif, semua objek 3D disembunyikan, dan sebaliknya.</summary>
    public void ToggleVideoPanel()
    {
        if (videoPanel == null) return;

        bool willShow = !videoPanel.activeSelf;
        videoPanel.SetActive(willShow);

        // Sembunyikan objek saat video tampil, tampilkan kembali saat video ditutup
        SetObjectsActive(!willShow);

        if (videoPlayer != null)
        {
            if (willShow)
            {
                videoPlayer.Stop();
                videoPlayer.Play();
            }
            else
            {
                videoPlayer.Stop();
            }
        }
    }

    /// <summary>Tampilkan panel video, sembunyikan objek 3D, dan putar video proses pendengaran.</summary>
    public void ShowVideoPanel()
    {
        if (videoPanel != null)
            videoPanel.SetActive(true);

        SetObjectsActive(false);

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.Play();
        }
    }

    /// <summary>Tutup panel video, tampilkan kembali objek 3D, dan hentikan pemutaran.</summary>
    public void CloseVideoPanel()
    {
        if (videoPlayer != null)
            videoPlayer.Stop();

        if (videoPanel != null)
            videoPanel.SetActive(false);

        SetObjectsActive(true);
    }

    // ------------------------------------------------------------------ //

    /// <summary>
    /// Daftarkan EventTrigger PointerDown / PointerUp pada sebuah tombol
    /// sehingga aksi aktif selama tombol ditahan.
    /// </summary>
    private void RegisterHoldButton(Button btn,
                                    System.Action onDown,
                                    System.Action onUp)
    {
        if (btn == null) return;

        EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = btn.gameObject.AddComponent<EventTrigger>();

        // PointerDown
        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((_) => onDown());
        trigger.triggers.Add(entryDown);

        // PointerUp
        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((_) => onUp());
        trigger.triggers.Add(entryUp);

        // Jaga-jaga jika pointer keluar area tombol
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((_) => onUp());
        trigger.triggers.Add(entryExit);
    }
}
