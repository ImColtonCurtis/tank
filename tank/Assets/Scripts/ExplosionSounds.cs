using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSounds : MonoBehaviour
{
    [SerializeField] AudioSource pelletExplosion, rocketExplosion, tankExplosion;
    public bool isRocket, isTank;

    private void Awake()
    {
        isRocket = false;
        isTank = false;
    }

    private void OnEnable()
    {
        StartCoroutine(WaitTwoFrames());
    }

    IEnumerator WaitTwoFrames()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        if (isRocket)
            rocketExplosion.Play();
        else if (isTank)
            tankExplosion.Play();
        else
            pelletExplosion.Play();
    }
}
