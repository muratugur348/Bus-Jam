using System.Collections.Generic;
using UnityEngine;

namespace BusJam.Scripts.Runtime.Managers.SoundManager
{
    public class SoundManager : MonoBehaviour, ISoundManager
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<GameSound> gameSounds = new();

        public void Awake()
        {
            IsSoundOn = PlayerPrefs.GetInt("is_sound_on", 1) == 1;
        }

        public bool IsSoundOn { get; private set; } = true;

        public void PlaySound(string key, float volume = 1f)
        {
            if (!IsSoundOn)
                return;
            var gameSound = gameSounds.Find(x => x.key == key);
            if (gameSound.randomizePitch)
            {
                var pitch = Random.Range(0.9f, 1.1f);
                audioSource.pitch = pitch;
            }
            else if (gameSound.changePitch)
            {
                audioSource.pitch += gameSound.changeAmount;
                if (audioSource.pitch > gameSound.maxPitch)
                {
                    audioSource.pitch = 1f;
                }
            }

            if (audioSource != null)
                audioSource.PlayOneShot(gameSound.clip, volume);
        }

        public void ToggleSounds(bool isOn)
        {
            IsSoundOn = isOn;
            audioSource.mute = !isOn;
            PlayerPrefs.SetInt("is_sound_on", isOn ? 1 : 0);
        }
    }
}