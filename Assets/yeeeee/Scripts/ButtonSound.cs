using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public AudioSource tap;

    public void PlaySound ()
    {
        tap.Play();
    }
}
