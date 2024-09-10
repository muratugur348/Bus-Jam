using Lofelt.NiceVibrations;
using UnityEngine;

namespace BusJam.Scripts.Runtime.Managers.HapticManager
{
    public enum HapticType
    {
        Selection,
        Success,
        Failure,
        LightImpact,
        MediumImpact,
        HeavyImpact,
        RigidImpact,
        SoftImpact
    }

    public class HapticManager : IHapticManager
    {
        public bool IsHapticOn { get; private set; } = true;
        public void Initialize()
        {
            IsHapticOn = PlayerPrefs.GetInt("is_haptic_on", 1) == 1;
        }

        public void ToggleHaptic(bool isOn)
        {
            IsHapticOn = isOn;
            PlayerPrefs.SetInt("is_haptic_on", IsHapticOn ? 1 : 0);
        }

        public void TriggerHaptic(HapticType hapticType)
        {
            if (!IsHapticOn)
            {
                return;
            }
#if !UNITY_ANDROID
            switch (hapticType)
            {
                case HapticType.Selection:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
                    break;
                case HapticType.Success:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
                    break;
                case HapticType.Failure:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
                    break;
                case HapticType.LightImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
                    break;
                case HapticType.MediumImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
                    break;
                case HapticType.HeavyImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
                    break;
                case HapticType.RigidImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
                    break;
                case HapticType.SoftImpact:
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
                    break;
            }
#endif
        }
    }
}