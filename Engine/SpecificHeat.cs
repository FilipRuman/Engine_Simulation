using Godot;
public static class SpecificHeat
{
    // I fucking love this site <3 https://www.engineeringtoolbox 

    //  I'm using isochoric pressure
    // I'm just using linear interpolation for convenience
    // It's not great for accuracy but it's ok for now
    // I might later change it to use something like godot's curves

    public static float GetSpecificHeatOfExhaustFumes(float temperature, float totalMixtureMas, float CO2Mas, float H2OMas, float N2Mas)
    {
        float massFractionCO2 = CO2Mas / totalMixtureMas;
        float massFractionH2O = H2OMas / totalMixtureMas;
        float massFractionN2 = N2Mas / totalMixtureMas;

        return CO2(temperature) * massFractionCO2 + H2O(temperature) * massFractionH2O + N2(temperature) * massFractionN2;
    }

    // Specific GasConstants: https://www.engineeringtoolbox.com/individual-universal-gas-constant-d_588.html
    ///kJ/kgK
    ///https://www.engineeringtoolbox.com/carbon-dioxide-d_974.html
    private static float CO2(float temperature)
    {
        const float specificGasConstant = 0.1889f;
        temperature -= 175f;
        return Mathf.Lerp(CpToCv(0.709f, specificGasConstant), CpToCv(1.476f, specificGasConstant), temperature / 5825f);
    }
    ///kJ/kgK
    ///https://www.engineeringtoolbox.com/water-vapor-d_979.html
    private static float H2O(float temperature)
    {
        const float specificGasConstant = 0.4615f;
        temperature -= 175f;
        return Mathf.Lerp(CpToCv(1.850f, specificGasConstant), CpToCv(3.350f, specificGasConstant), temperature / 5825f);
    }
    ///kJ/kgK
    ///https://www.engineeringtoolbox.com/nitrogen-d_977.html
    private static float N2(float temperature)
    {
        const float specificGasConstant = 0.2968f;
        temperature -= 175f;
        return Mathf.Lerp(CpToCv(1.039f, specificGasConstant), CpToCv(1.039f, specificGasConstant), temperature / 5825f);
    }


    //cp=cv+R
    /// Converts isobaric to isochronic specific heat
    ///
    //cp is the isobaric specific heat (at constant pressure)
    //cv is the isochoric specific heat (at constant volume)
    private static float CpToCv(float Cp, float specificGasConstant)
    {
        return Cp - specificGasConstant;
    }
}
