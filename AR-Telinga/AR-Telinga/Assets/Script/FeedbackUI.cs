using System.Collections;
using UnityEngine;

public class FeedbackUI : MonoBehaviour
{
    public GameObject benarObject;
    public GameObject salahObject;
    public float duration = 0.7f;

    private Coroutine activeRoutine;

    private void Awake()
    {
        HideAll();
    }

    public void ShowBenar()
    {
        ShowTarget(benarObject);
    }

    public void ShowSalah()
    {
        ShowTarget(salahObject);
    }

    private void ShowTarget(GameObject target)
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }

        HideAll();
        activeRoutine = StartCoroutine(ShowRoutine(target));
    }

    private IEnumerator ShowRoutine(GameObject target)
    {
        if (target != null)
            target.SetActive(true);

        yield return new WaitForSeconds(duration);

        HideAll();
        activeRoutine = null;
    }

    private void HideAll()
    {
        if (benarObject != null)
            benarObject.SetActive(false);

        if (salahObject != null)
            salahObject.SetActive(false);
    }
}