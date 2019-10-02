using UnityEngine;

public class LightSettings : MonoBehaviour
{
    public const int TestOpaueLightLevel = 5;
    public static readonly Vector3 TestGlobalLightDirection = new Vector3(1, 0.7f, 0.3f).normalized;

    public static Color TestGIColor { get; private set; }
    public static Color TestSunColor { get; private set; }

    public static int GlobalAmbiantLevel = 2;
    public static int GlobalLightLevel = 7;

    public Color GIColor;
    public Color SunColor;
    
    void Start()
    {
        TestGIColor = GIColor;
        TestSunColor = SunColor;
    }
}