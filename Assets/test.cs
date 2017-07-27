using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] Renderer ren;
    [SerializeField] Color col;
    [SerializeField] float speed;
    [SerializeField] float flashDuration;
    [SerializeField] float minDif;
    [SerializeField] bool isPaused;
    

    void Update ()
    {
        if (!isPaused && !IsInvoking())
        {
            ren.materials[0].SetColor("_EmissionColor", Color.Lerp(ren.materials[0].GetColor("_EmissionColor"), col, speed * Time.deltaTime));
            var dif = ren.materials[0].GetColor("_EmissionColor") - col;
            var absDif = new Color(Mathf.Abs(dif.r), Mathf.Abs(dif.g), Mathf.Abs(dif.b));
            var clouse = absDif.r < minDif &&
                         absDif.g < minDif &&
                         absDif.b < minDif;
            if (clouse && !IsInvoking())
                InvokeRepeating("Start", 0, flashDuration * 2);
        }
    }

    
    void Start()
    {
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        ren.materials[0].SetColor("_EmissionColor", col);
        yield return new WaitForSeconds(flashDuration);
        ren.materials[0].SetColor("_EmissionColor", Color.black);
    }
}
