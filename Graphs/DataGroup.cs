using Godot;
using System.Collections.Generic;
[GlobalClass, Tool]
public partial class DataGroup : Resource {
    [Export] public Color color;
    [Export] public string name;
    public List<float> data = new();
    public List<Control> points = new();
    [Export] public uint averagePoolSize = 4;
    public List<float> dataSmoothingCache = new();
    public Vector2 valueRange;
    public void AddData(float value) {
        dataSmoothingCache.Add(value);
        if (dataSmoothingCache.Count < averagePoolSize)
            return;
        data.Add(GetAverageOfSmoothingChache());
        data.RemoveAt(0);
        dataSmoothingCache.Clear();
    }

    public float GetAverageOfSmoothingChache() {
        float sum = 0;
        foreach (float value in dataSmoothingCache) sum += value;
        return Mathf.Clamp(sum / dataSmoothingCache.Count, valueRange.X, valueRange.Y);
    }

}
