using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(showLogo());
    }

    IEnumerator showLogo()
    {
        Color visable = Color.white;
        Color transparent = Color.white;
        transparent.a = 0f;
        float duration = 1f;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            //right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            this.GetComponent<SpriteRenderer>().color = Color.Lerp(transparent, visable, normalizedTime);
            yield return null;
        }
        this.GetComponent<SpriteRenderer>().color = visable; //without this, the value will end at something like 0.9992367
    }
}
