using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Breakout.Behaviour
{
    public enum SoundType
    {
        Injured = 0,
        Destroy,
        Combo,
        LevelUp,
        FireBullet,
        LevelDown,
        GetAmmo
    }

    /// <summary>
    /// controller to play audio
    /// </summary>
    public class AudioController : MonoBehaviour
    {
    	public AudioClip m_injuredSound;
	    public AudioClip m_destroySound;
        public AudioClip m_comboSound;
        public AudioClip m_levelUpSound;
        public AudioClip m_fireBullletSound;
        public AudioClip m_levelDownSound;
        public AudioClip m_getAmmoSound;

        private AudioSource m_audioSource;

        void Awake()
        {
            m_audioSource = GetComponent<AudioSource>();
        }

        public void Play(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.Injured:
                    m_audioSource.PlayOneShot(m_injuredSound);
                    break;

                case SoundType.Destroy:
                    m_audioSource.PlayOneShot(m_destroySound);
                    break;

                case SoundType.Combo:
                    m_audioSource.PlayOneShot(m_comboSound);
                    break;

                case SoundType.LevelUp:
                    m_audioSource.PlayOneShot(m_levelUpSound);
                    break;

                case SoundType.FireBullet:
                    m_audioSource.PlayOneShot(m_fireBullletSound);
                    break;

                case SoundType.LevelDown:
                    m_audioSource.PlayOneShot(m_levelDownSound);
                    break;

                case SoundType.GetAmmo:
                    m_audioSource.PlayOneShot(m_getAmmoSound);
                    break;
            }
        }
    }
}
