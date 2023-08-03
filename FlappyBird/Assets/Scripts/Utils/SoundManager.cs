using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBird.Utils
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] audioClips;

        public static SoundManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            this.Init();
        }

        private Dictionary<string, AudioClip> dicSounds;
        public void Init()
        {
            this.dicSounds = new Dictionary<string, AudioClip>(this.audioClips.Length);
            for (var i = 0; i < this.audioClips.Length; i++)
            {
                this.dicSounds[this.audioClips[i].name] = this.audioClips[i];
            }
        }

        public void PlaySingle(string soundName)
        {
            if (this.dicSounds.TryGetValue(soundName, out var clip))
            {
                this.audioSource.clip = clip;
                this.audioSource.Play();
            }
            else
            {
                Debug.LogWarning("Sound not found for name: " + soundName);
            }
        }

        public void PlayOneShot(string soundName)
        {
            if (this.dicSounds.TryGetValue(soundName, out var clip))
            {
                this.audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning("Sound not found for name: " + soundName);
            }
        }

        public void StopSound()
        {
            this.audioSource.Stop();
        }
    }
}