using System;
using System.Collections.Generic;
public class TickSystem {
    public float updatesPerSecond = 60;
    private float timer;
    public List<Action<float>> toCall = new();
    private float CalculateDeltaTimeForCalls => 1f / updatesPerSecond;
    public void Update(float delta /* ms */) {
        timer += delta;
        float deltaForCalls = CalculateDeltaTimeForCalls;
        uint callsAmount = (uint)MathF.Floor(timer / deltaForCalls);
        timer -= callsAmount * deltaForCalls;

        for (uint i = 0; i < callsAmount; i++) {
            foreach (Action<float> action in toCall) {
                action(deltaForCalls);
            }
        }

    }

}
