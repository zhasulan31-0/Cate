using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    [Header("UI")]
    public Toggle soundToggle;

    private const string SoundPrefKey = "SoundEnabled";

    private void Start()
    {
        // ��������� ���������� ���������, �� ��������� ���� �������
        bool soundEnabled = PlayerPrefs.GetInt(SoundPrefKey, 1) == 1;
        soundToggle.isOn = soundEnabled;

        ApplySoundState(soundEnabled);

        // ������������� �� ��������� Toggle
        soundToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        ApplySoundState(isOn);

        // ��������� ���������
        PlayerPrefs.SetInt(SoundPrefKey, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplySoundState(bool enabled)
    {
        AudioListener.volume = enabled ? 1f : 0f;
    }
}
