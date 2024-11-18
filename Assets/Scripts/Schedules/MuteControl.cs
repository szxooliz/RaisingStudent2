using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteControl : MonoBehaviour
{
    public void MuteUnMuter(AudioSource m_AudioSource)
    {
        m_AudioSource.mute  = !m_AudioSource.mute;
    }
}
