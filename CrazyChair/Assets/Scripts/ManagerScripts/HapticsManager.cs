using Lofelt.NiceVibrations;
using UnityEngine;

public class HapticsManager : MonoBehaviour
{
    public static HapticsManager Instance;
    [SerializeField] private bool isActiveHaptice = true;
    void Awake() => Instance = this;
    private void Start()
    {
        HapticController.fallbackPreset = HapticPatterns.PresetType.RigidImpact;
    }
    public void HapticToggle(bool isActive)
    {
        isActiveHaptice = isActive;
    }
    public void CreateHaptic(float amplitude, float frequency)
    {
        if (isActiveHaptice)
            HapticPatterns.PlayEmphasis(amplitude, frequency);
    }
    public void CreateConstantHaptic(float amplitude, float frequency, float duration)
    {
        if (isActiveHaptice)
            HapticPatterns.PlayConstant(amplitude, frequency, duration);
    }
}
