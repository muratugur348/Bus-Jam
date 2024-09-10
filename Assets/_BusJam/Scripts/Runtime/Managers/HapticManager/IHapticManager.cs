namespace BusJam.Scripts.Runtime.Managers.HapticManager
{
    public interface IHapticManager
    {
        public bool IsHapticOn { get; }
        public void Initialize();
        public void ToggleHaptic(bool isOn);
        public void TriggerHaptic(HapticType hapticType);
    }
}