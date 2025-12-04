using System.Collections;
using UnityEngine;

public class GlassHider : MonoBehaviour
{
    public float hideDuration = 2f;

    public void HideForSeconds()
    {
        StopAllCoroutines();
        StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(hideDuration);
        gameObject.SetActive(true);
    }
}
