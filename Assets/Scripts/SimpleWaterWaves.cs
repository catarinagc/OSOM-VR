using UnityEngine;

public class SimpleWaterWaves : MonoBehaviour
{
    [System.Serializable]
    public struct Wave
    {
        public float amplitude;
        public float wavelength;
        public float speed;
        public Vector2 direction;
    }

    public Wave[] waves;

    static readonly int WaveCount = Shader.PropertyToID("_WaveCount");
    static readonly int WaveData = Shader.PropertyToID("_WaveData");

    Vector4[] waveDataArray;

    void Start()
    {
        UpdateShader();
    }

    void Update()
    {
        UpdateShader();
    }

    void UpdateShader()
    {
        if (waves == null || waves.Length == 0)
            return;

        if (waveDataArray == null || waveDataArray.Length != waves.Length)
            waveDataArray = new Vector4[waves.Length];

        for (int i = 0; i < waves.Length; i++)
        {
            var w = waves[i];

            waveDataArray[i] = new Vector4(
                w.amplitude,
                w.wavelength,
                w.speed,
                0
            );

            Shader.SetGlobalVector("_WaveDir" + i,
                new Vector4(w.direction.x, w.direction.y, 0, 0));
        }

        Shader.SetGlobalInt(WaveCount, waves.Length);
        Shader.SetGlobalVectorArray(WaveData, waveDataArray);
    }
}