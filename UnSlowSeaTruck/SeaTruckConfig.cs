﻿namespace UnSlowSeaTruck;

using Nautilus.Json;

public class SeaTruckConfig : ConfigFile
{
    public float SteeringMultiplier { get; set; } = 1.17f;

    public float AccelerationMultiplier { get; set; } = 1.28f;

    public float WeightOverride { get; set; } = 0.0001f;
    
    public float HorsePowerModifier { get; set; } = 0.4f;
}
