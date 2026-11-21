# ARBIHear (AR-Telinga) - Interactive Augmented Reality for Human Ear Anatomy & Auditory Biomechanics

![Unity Version](https://img.shields.io/badge/Unity-2022.3.62f3-blue.svg?style=for-the-badge&logo=unity)
![AR Framework](https://img.shields.io/badge/AR_SDK-PTC_Vuforia_Engine-orange.svg?style=for-the-badge)
![UI Framework](https://img.shields.io/badge/UI-TextMeshPro_%26_UGUI-blueviolet.svg?style=for-the-badge)
![Platform](https://img.shields.io/badge/Platform-Android_APK-green.svg?style=for-the-badge&logo=android)
![Target Audience](https://img.shields.io/badge/Audience-Middle%2FHigh_School_%26_College-yellow.svg?style=for-the-badge)
![License](https://img.shields.io/badge/Status-Complete_%26_Tested-success.svg?style=for-the-badge)

---

## 📖 1. Project Overview & Clinical/Pedagogical Rationale

### 1.1 The Challenge of Teaching Auditory Anatomy
The human peripheral auditory system is among the most intricate, biomechanically sophisticated sensory apparatuses in vertebrate biology. Unlike surface organs, the functional mechanisms responsible for converting ambient sound pressure waves into neuroelectrical impulses are completely enclosed inside the dense petrous portion of the temporal bone of the human skull.

Conventional pedagogical methodologies in middle school, high school, and undergraduate biology classes rely heavily on static, two-dimensional textbook diagrams or rigid plastic anatomical cross-sections. These traditional tools suffer from profound educational limitations:
1. **Spatial Occlusion:** 2D diagrams fail to represent how the auditory ossicles (malleus, incus, and stapes) articulate in three-dimensional space, or how the stapes footplate translates piston-like motion into the oval window.
2. **Dynamic Mechanism Invisibility:** Students cannot visualize how longitudinal sound waves traveling through the air are transformed into mechanical vibrations in the eardrum, amplified ~20-fold by the ossicular lever system, propagated as fluid waves through the perilymph and endolymph of the cochlea, and finally transduced by the hair cells along the basilar membrane within the Organ of Corti.
3. **Passive Memorization vs. Active Mastery:** Without tactile and spatial interaction, students struggle to retain the eight sequential milestones of auditory transmission.

### 1.2 The ARBIHear Solution
**ARBIHear** (*Augmented Reality Biomechanics of Hearing*) is an interactive mobile Augmented Reality application developed to transform human ear anatomy education into an engaging, multi-sensory virtual laboratory. 

Using an Android smartphone or tablet camera pointed at a designated printed flashcard target, students instantly project an interactive, photorealistic 3D anatomical ear model directly onto their desk or classroom tabletop. ARBIHear allows learners to isolate sub-anatomical compartments (Outer Ear, Middle Ear, and Inner Ear), rotate and scale models in full 360-degree freedom, trigger synchronized explanatory video animations rendered directly inside the augmented scene, and participate in three gamified assessment modalities:
- **Sequential Sound Wave Ordering (*Drag & Order*):** A tactile puzzle reinforcing the mechanical pathway of sound.
- **Dynamic Vector Line-Matching Quiz:** An interactive matching module connecting anatomical terms to diagrammatic positions using real-time vector lines.
- **Multiple-Choice Knowledge Verification:** A comprehensive assessment module with immediate feedback icons and scoring.

---

## 🔬 2. Biological Curriculum & Anatomical Concepts Covered

ARBIHear provides comprehensive coverage of the human auditory system across three primary anatomical divisions and its physiological transduction pathway:

### 2.1 The Outer Ear (*Auris Externa*)
- **Pinna / Auricle:** Cartilaginous structure designed to capture, filter, and funnel acoustic sound pressure waves into the canal.
- **External Auditory Meatus (Ear Canal):** Resonating acoustic tube that channels sound waves toward the tympanic membrane while protecting deeper structures via cerumen (earwax) glands.
- **Tympanic Membrane (Eardrum):** Thin, semi-transparent fibrous membrane that vibrates in direct correspondence to the frequency and amplitude of incoming sound waves, serving as the anatomical boundary between outer and middle ear.

### 2.2 The Middle Ear (*Auris Media*)
- **Tympanic Cavity:** Air-filled chamber within the temporal bone.
- **Auditory Ossicles (The Three Smallest Bones in the Human Body):**
  - **Malleus (Hammer):** Attached directly to the inner surface of the tympanic membrane.
  - **Incus (Anvil):** Intermediate articulating hinge bone transmitting kinetic energy.
  - **Stapes (Stirrup):** Smallest bone; its footplate fits into the oval window.
- **Biomechanical Impedance Matching:** The ossicular lever system and the surface area ratio between the large eardrum and the small oval window amplify sound pressure approximately 20 to 22 times, allowing airborne sound to overcome the mechanical resistance of cochlear fluid.
- **Eustachian Tube:** Ventilating channel equalizing middle ear air pressure with ambient atmospheric pressure.

### 2.3 The Inner Ear (*Auris Interna*)
- **Bony & Membranous Labyrinth:** Complex sensory apparatus housing hearing and equilibrium organs.
- **Semicircular Canals & Vestibule:** Dynamic and static equilibrium organs monitoring head acceleration and balance.
- **Cochlea (Snail Shell):** Coiled, fluid-filled spiral chamber divided into three parallel fluid compartments: *Scala Vestibuli*, *Scala Media*, and *Scala Tympani*.
- **Organ of Corti & Basilar Membrane:** Houses inner and outer stereocilia hair cells. Sound-induced fluid displacement causes the basilar membrane to ripple; stereocilia deflection opens mechanically gated potassium channels, depolarizing hair cells and generating action potentials.
- **Auditory Nerve (Cranial Nerve VIII):** Transmits action potentials along the auditory pathway to the cochlear nuclei, thalamus, and primary auditory cortex in the temporal lobe of the brain.

---

## ⚙️ 3. Technical Specifications & Software Stack

- **Game Engine:** Unity Editor `2022.3.62f3` (Long Term Support / LTS)
- **Augmented Reality Framework:** PTC Vuforia Engine SDK (`com.ptc.vuforia.engine`)
- **Render Pipeline:** Unity Built-in Render Pipeline with Forward Rendering
- **Scripting Architecture:** C# (.NET Standard 2.1)
- **Input Frameworks:** Unity Legacy Input Manager & New Input System (`com.unity.inputsystem`)
- **UI & Typography:** Unity TextMesh Pro (`com.unity.textmeshpro`) & UGUI Canvas System
- **Scripting Backend:** IL2CPP / Mono (Targeting 64-bit ARM64-v8a & 32-bit ARMv7-a)
- **Target Operating System:** Android 8.0 (API Level 26 / Oreo) up to Android 14 (API Level 34)
- **Graphics API:** OpenGLES3 / Vulkan fallback
- **Camera Configuration:** Continuous Autofocus Mode enabled via Vuforia Camera Device API

---

## 💻 4. Deep-Dive Source Code & Architectural Mechanics

### 4.1 Dual-State Content Coordinator (`MarkerContentController.cs`)
This script resides on the AR Marker Canvas. It coordinates the active state of 3D models and synchronized video playback without UI overlapping:

```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using Vuforia;

/// <summary>
/// Attached to the Canvas element anchored inside the Vuforia Image Target.
/// Automatically detects ObserverBehaviour in parent hierarchy.
/// Manages selective visibility of 3D sub-organs and controls paired video streams.
/// </summary>
public class MarkerContentController : MonoBehaviour
{
    [System.Serializable]
    public class MarkerContent
    {
        [Tooltip("UI Button that triggers this specific anatomical view")]
        public Button triggerButton;

        [Tooltip("Specific 3D sub-organ model to display")]
        public GameObject targetObject;

        [Tooltip("Corresponding video player for anatomical explanation")]
        public VideoPlayer videoPlayer;

        [Tooltip("Automatically play video when view is activated")]
        public bool autoPlayVideo = true;
    }

    [Header("Main Root Model")]
    public GameObject fullEarModel;

    [Header("Sub-Organ Views")]
    public List<MarkerContent> contentViews = new List<MarkerContent>();

    void Start()
    {
        // Bind click events dynamically across all sub-organ buttons
        for (int i = 0; i < contentViews.Count; i++)
        {
            int index = i;
            if (contentViews[index].triggerButton != null)
            {
                contentViews[index].triggerButton.onClick.AddListener(() => ShowContent(index));
            }
        }
    }

    public void ShowContent(int selectedIndex)
    {
        // Hide full ear model to avoid visual occlusion
        if (fullEarModel != null)
            fullEarModel.SetActive(false);

        // Deactivate all sub-views and stop ongoing video streams
        for (int i = 0; i < contentViews.Count; i++)
        {
            bool isTarget = (i == selectedIndex);
            if (contentViews[i].targetObject != null)
                contentViews[i].targetObject.SetActive(isTarget);

            if (contentViews[i].videoPlayer != null)
            {
                if (isTarget && contentViews[i].autoPlayVideo)
                    contentViews[i].videoPlayer.Play();
                else
                    contentViews[i].videoPlayer.Stop();
            }
        }
    }
}
```

### 4.2 Sequence Evaluation & Scoring Engine (`DragOrderScoreController.cs`)
Evaluates student puzzle arrangements for the sound wave propagation sequence:

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DragOrderScoreController : MonoBehaviour
{
    [Header("Evaluation Controls")]
    public Button btnSubmitOrder;
    public GameObject panelScoreCard;
    public TextMeshProUGUI textFinalScore;
    public TextMeshProUGUI textFeedbackRemarks;

    [Header("Points Configuration")]
    public int pointsPerCorrectSlot = 10;

    private DropImageSlot[] registeredSlots;

    void Start()
    {
        registeredSlots = FindObjectsOfType<DropImageSlot>();
        if (btnSubmitOrder != null)
            btnSubmitOrder.onClick.AddListener(EvaluateOrder);
    }

    public void EvaluateOrder()
    {
        int totalScore = 0;
        int correctCount = 0;

        foreach (var slot in registeredSlots)
        {
            if (slot.IsCorrectlyAssigned())
            {
                totalScore += pointsPerCorrectSlot;
                correctCount++;
            }
        }

        // Display Score Panel and disable interaction
        if (panelScoreCard != null)
            panelScoreCard.SetActive(true);

        if (textFinalScore != null)
            textFinalScore.text = $"Score: {totalScore} / {registeredSlots.Length * pointsPerCorrectSlot}";

        if (textFeedbackRemarks != null)
        {
            if (correctCount == registeredSlots.Length)
                textFeedbackRemarks.text = "Outstanding! Perfect understanding of auditory transmission.";
            else
                textFeedbackRemarks.text = "Good attempt! Review the ossicle pathway and try again.";
        }
    }
}
```

### 4.3 Screen-Space Vector Line-Matching (`ConnectionLine.cs`)
Renders dynamic connection lines between UI terms and anatomical diagram anchors during touch interactions:

```csharp
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConnectionLine : MonoBehaviour
{
    public RectTransform anchorNodeA;
    public RectTransform anchorNodeB;
    public Camera uiCamera;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
    }

    public void BindNodes(RectTransform a, RectTransform b, Camera cam)
    {
        anchorNodeA = a;
        anchorNodeB = b;
        uiCamera = cam;
        UpdatePositions();
    }

    void LateUpdate()
    {
        UpdatePositions();
    }

    void UpdatePositions()
    {
        if (anchorNodeA == null || anchorNodeB == null) return;
        lineRenderer.SetPosition(0, anchorNodeA.position);
        lineRenderer.SetPosition(1, anchorNodeB.position);
    }
}
```

---

## 📂 5. Directory Layout & Asset Inventory

```text
AR-Telinga/
├── AR-Telinga/
│   ├── AR-Telinga/                             <-- Main Unity Project
│   │   ├── Assets/
│   │   │   ├── 3D Model/                       # Anatomical 3D Meshes & Materials
│   │   │   │   ├── Telinga full/               # Complete integrated human ear model
│   │   │   │   ├── Telinga luar/               # Isolated Outer Ear (Pinna & Canal)
│   │   │   │   ├── Telinga tengah/             # Middle Ear (Ossicle bones & eardrum)
│   │   │   │   └── Telinga dalam/              # Inner Ear (Cochlea, canals & nerve)
│   │   │   ├── Video/                          # Educational Explanatory MP4 Videos
│   │   │   │   ├── mp4/Telinga bagian luar.mp4 # Video: Outer ear functions
│   │   │   │   ├── mp4/Telinga bagian tengah.mp4 # Video: Ossicles & amplification
│   │   │   │   ├── mp4/Telinga bagian dalam.mp4# Video: Cochlea & fluid wave mechanics
│   │   │   │   └── mp4/Proses pendengaran.mp4  # Video: Complete hearing cycle
│   │   │   ├── Scenes/                         # 18+ Interactive Unity Scenes
│   │   │   ├── Script/                         # C# Architecture & Interaction Code
│   │   │   │   ├── MarkerContentController.cs  # Multi-subview & video coordinator
│   │   │   │   ├── DragOrderScoreController.cs # Sequential drag puzzle evaluator
│   │   │   │   ├── ConnectionManager.cs        # Vector line quiz manager
│   │   │   │   ├── ConnectionLine.cs           # LineRenderer vector drawer
│   │   │   │   ├── QuizManager.cs              # Multiple-choice quiz logic
│   │   │   │   ├── ObjectController.cs         # 3D model rotation & manipulation
│   │   │   │   ├── scale.cs                    # Two-finger pinch zoom controller
│   │   │   │   └── LevelManager.cs             # Scene transition & quit handling
│   │   │   ├── TextMesh Pro/                   # High-DPI font assets & shaders
│   │   │   └── UIUX/                           # Buttons, score panels, and icons
│   │   └── Packages/manifest.json              # Package dependency manifest
│   ├── ARBIHear-14.apk                         # Final Production Android APK (Build 14)
│   ├── ARBIHear-13.apk                         # Testing Build 13
│   ├── ARBIHear-12.apk                         # Testing Build 12
│   ├── ARBIHear-11.apk                         # Testing Build 11
│   ├── ARBIHear-10.apk                         # Testing Build 10
│   └── ARBIHear-8.apk & 9.apk                  # Alpha prototype builds
└── README.md
```

---

## 🎮 6. Complete Scene Inventory & Application Flow

```
   [tampilan_awal.unity]  --->  [menu_utama.unity]
                                      |
         +----------------------------+----------------------------+
         |                            |                            |
         v                            v                            v
   [AR.unity]                 [struktur.unity]             [dragnorder.unity]
   Live AR Tracking           3D Anatomical Anatomy        Sequential Ordering
         |                            |                            |
         v                            v                            v
   [AR-Main.unity]            [Proses.unity]               [match_quiz.unity]
   Sub-Organ Switcher         Sound Wave Dynamics          Line Matching Quiz
                                                                   |
                                                                   v
                                                           [quiz_pilih.unity]
                                                           Multiple Choice Exam
```

| Scene Name | Type | Description |
|:---|:---|:---|
| **`tampilan_awal.unity`** | Splash | Opening screen with animated logo and hardware initialization. |
| **`menu_utama.unity`** | Dashboard | Main menu hub: AR Mode, Structural Study, Sound Mechanics, Quizzes, Guide, About. |
| **`AR.unity` / `AR-Main.unity`** | AR Camera | Live Vuforia tracking view recognizing flashcard markers to project the 3D ear model. |
| **`struktur.unity`** | Study | Exploded 3D view examining Outer, Middle, and Inner Ear compartments. |
| **`Proses.unity`** | Simulation | Real-time particle animation illustrating sound vibration transformation into nerve impulses. |
| **`dragnorder.unity`** | Mini-Game | Drag-and-drop puzzle arranging the 8 sequential milestones of hearing. |
| **`match_quiz.unity`** | Mini-Game | Vector line matching connecting organ terminology to anatomical diagrams. |
| **`quiz_pilih.unity`** | Exam | 10-question randomized multiple-choice assessment with instant feedback. |
| **`petunjuk.unity`, `1`, `2`** | Guide | Operational tips on camera distance (20–40 cm), surface lighting, and marker alignment. |
| **`tentang1.unity`, `2`** | Credits | Development team credentials, faculty supervision, and medical references. |

---

## 📲 7. Compiled Android Deliverables
Ready-to-install Android Application Packages (.apk) in `AR-Telinga/AR-Telinga/`:
- **`ARBIHear-14.apk`** *(Final Production Release - Recommended)*: Features high-resolution 3D models, smooth video playback, optimized memory garbage collection, and responsive UI.
- **`ARBIHear-10.apk` through `ARBIHear-13.apk`**: Beta release candidates.
- **`ARBIHear-8.apk` & `9.apk`**: Alpha releases containing baseline Vuforia tracking.

---

## 🛠 8. Unity Editor Setup & Android Compilation Guide
1. Launch **Unity Hub**, click **Add** -> **Add project from disk**.
2. Select directory: `E:\Project\AR\AR-Telinga\AR-Telinga\AR-Telinga`.
3. Ensure **Unity 2022.3.62f3** (or compatible 2022.3 LTS) is selected.
4. In Unity Editor, open: `Assets/Scenes/tampilan_awal.unity`.
5. Open **Build Settings** (`Ctrl + Shift + B`), ensure **Android** is the active platform.
6. Verify all main scenes are included in **Scenes In Build**:
   - `tampilan_awal`, `menu_utama`, `AR`, `AR-Main`, `struktur`, `Proses`, `dragnorder`, `match_quiz`, `quiz_pilih`.
7. Connect an Android smartphone with USB Debugging enabled, then click **Build and Run**.
