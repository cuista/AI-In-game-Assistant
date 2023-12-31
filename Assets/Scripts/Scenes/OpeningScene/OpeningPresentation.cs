using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningPresentation : MonoBehaviour
{
    public Image[] openingImages;

    public Animator crossfade;
    private float _crossfadeTime = 1f;
    public bool skipPresentation = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(Presentation());
    }

    //presentation cutscene
    private IEnumerator Presentation()
    {
        if (!skipPresentation)
        {
            foreach (Image image in openingImages)
            {
                image.gameObject.SetActive(true);
                yield return new WaitForSeconds(3f);

                crossfade.SetTrigger("Start");
                yield return new WaitForSeconds(_crossfadeTime);

                image.gameObject.SetActive(false);
                crossfade.SetTrigger("End");
            }
        }

        LoadingScenesManager.LoadingScenes("MainMenu");
        yield return null;
    }
}
