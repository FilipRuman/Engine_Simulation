using Godot;
using System.Collections.Generic;
using System.Linq;
[Tool]
public partial class GraphMain : Node {
    [Export] private ValueLineManager valueLineManager;
    [Export] private PackedScene pointPrefab;
    [Export] private Control pointsParent;

    /// X-Min Y-Max
    [Export] private Vector2 valueRange;
    [Export] private Vector2 domainRange;

    [Export] private string valueName;
    [Export] private string domainName;
    [Export] private Label valueNameLabel;
    [Export] private Label domainNameLabel;


    [Export] private bool ReSetupAll;
    [Export] private bool generateRandomData;


    [Export] public uint pointsCount;


    [Export] public DataGroup[] dataGroups;

    [Export] public Control dataGroupVisualizationLayout;
    [Export] public PackedScene dataGroupVisualizationpPrefab;

    [ExportGroup("Visuals")]
    [Export] private float pointsScale;


    [Export] private float refreshRate = .4f;

    private float refreshRateTimer = 0;

    private void SpawnDataGroupVisualization(DataGroup dataGroup) {
        var dataGroupVisualization = (DataGroupVisualization)dataGroupVisualizationpPrefab.Instantiate();
        dataGroupVisualizationLayout.AddChild(dataGroupVisualization);
        dataGroupVisualization.Setup(dataGroup.color, dataGroup.name);
    }
    private void SetupDataGroupVisualization() {
        foreach (Node node in dataGroupVisualizationLayout.GetChildren()) {
            node.QueueFree();
        }

        foreach (DataGroup dataGroup in dataGroups) {
            SpawnDataGroupVisualization(dataGroup);
        }
    }

    public override void _Process(double delta) {
        valueNameLabel.Text = valueName;
        domainNameLabel.Text = domainName;
        //
        // foreach (DataGroup dataGroup in dataGroups) {
        //     while (dataGroup.data.Count < pointsCount) {
        //         dataGroup.data.Add(0);
        //     }
        // }


        if (refreshRateTimer < (float)refreshRate) {
            refreshRateTimer += (float)delta;
            return;
        }
        refreshRateTimer = 0;


        if (ReSetupAll) {
            ReSetupAll = false;
            SetupAll();
        }
        if (generateRandomData) {
            generateRandomData = false;

            for (uint i = 0; i < dataGroups.Length; i++) {
                GenerateRandomData(i);
            }
        }

        foreach (DataGroup dataGroup in dataGroups) {
            RefreshPointsPositions(dataGroup);
        }
        base._Process(delta);
    }

    public void AddDataToEnd(float value, uint dataGroupIndex) {
        var dataGroup = dataGroups[dataGroupIndex];
        dataGroup.AddData(value);
    }
    //TODO: public void AddDataForPosition(float value, float position)

    private void RefreshPointsPositions(DataGroup dataGroup) {
        float xPositionScale = pointsParent.Size.X / pointsCount;
        float yPositionScale = pointsParent.Size.Y / (valueRange.Y - valueRange.X);
        float yPositionOffset = -valueRange.X;



        for (int i = 0; i < pointsCount; i++) {
            Vector2 pos = new(xPositionScale * i, (dataGroup.data[i] + yPositionOffset) * yPositionScale);
            dataGroup.points[i].SetPosition(new(pos.X, pointsParent.Size.Y - pos.Y), true);
        }
    }

    public float ValueBasedOnPositionPercentage(float positionPercentage/* 0-1*/ , bool horizontal) {
        if (!horizontal)
            return Mathf.Lerp(valueRange.X, valueRange.Y, 1 - positionPercentage);

        return Mathf.Lerp(domainRange.X, domainRange.Y, 1 - positionPercentage);
    }


    private void GenerateRandomData(uint dataGroupIndex) {
        var rng = new RandomNumberGenerator();
        for (int i = 0; i < pointsCount * dataGroups[dataGroupIndex].averagePoolSize; i++) {
            AddDataToEnd(rng.RandfRange(valueRange.X, valueRange.Y), dataGroupIndex);
        }
    }

    public override void _Ready() {
        SetupAll();
        base._Ready();
    }

    private void SetupAll() {
        valueLineManager.ReSpawnAllLines();
        SetupDataGroupVisualization();

        foreach (Control control in pointsParent.GetChildren()) {
            control.QueueFree();
        }

        foreach (DataGroup dataGroup in dataGroups) {
            dataGroup.points.Clear();
            dataGroup.data = new(Enumerable.Repeat(0f, (int)pointsCount).ToArray());
            dataGroup.valueRange = valueRange;

            for (int i = 0; i < pointsCount; i++) {
                var point = (TextureRect)pointPrefab.Instantiate();
                point.Modulate = dataGroup.color;
                point.Size = Vector2.One * pointsScale;
                pointsParent.AddChild(point);
                dataGroup.points.Add(point);
            }
        }

    }


}
