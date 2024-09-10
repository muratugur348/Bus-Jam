using System;
using UnityEngine;

namespace BusJam.Scripts.Runtime.Managers.SoundManager
{
    [Serializable]
    public struct GameSound
    {
        public string key;
        public AudioClip clip;
        
        [Header("Randomize Pitch Settings")]
        [Tooltip("It randomize the pitch value between 'minRandomizePitch' and 'maxRandomizePitch'. Note: Cannot be activated if the 'changePitch' option is on.")]
        public bool randomizePitch;
        public float minRandomizePitch;
        public float maxRandomizePitch;
        
        [Header("Change Pitch Settings")]
        [Tooltip("Adds this value to pitch. Note: Cannot be activated if the 'randomizePitch' option is on.")]
        public bool changePitch;
        public float changeAmount;
        public float maxPitch;
    }
    
    public interface ISoundManager
    {
        public bool IsSoundOn { get; }
        public void PlaySound(string key, float volume = 1f);
        public void ToggleSounds(bool isOn);
    }
}