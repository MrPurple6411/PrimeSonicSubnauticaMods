﻿namespace MoreCyclopsUpgrades.API.Charging;

using UnityEngine;

#if SUBNAUTICA
using Sprite = Atlas.Sprite;
#endif

/// <summary>
/// Defines all the behaviors for a cyclops charger that handles a particular type of energy recharging.<para/>
/// Recharging may be part of an update module or it might be a new buidable.<para/>
/// Whatever the case, it is up to you to ensure you have all your references set and ready.<para/>
/// DO NOT recharge the Cyclops PowerRelay yourself from this class!!! The MoreCyclopsUpgrades PowerManager will handle that.<para/>
/// </summary>
public abstract class CyclopsCharger
{
    private bool gettingEnergy;

    /// <summary>
    /// A reference to the the cyclops <see cref="SubRoot"/> instance.
    /// </summary>
    public readonly SubRoot Cyclops;

    /// <summary>
    /// Initializes a new instance of the <see cref="CyclopsCharger"/> class.
    /// </summary>
    /// <param name="cyclops">The cyclops.</param>
    protected CyclopsCharger(SubRoot cyclops)
    {
        Cyclops = cyclops;
    }

    /// <summary>
    /// Gets a value indicating whether the charger icon allowed to be visible in the Cyclops displays.<br/>
    /// Depends on either <see cref="ProvidingPower"/> or <see cref="HasReservePower"/> returning <c>true</c>.
    /// </summary>
    /// <value>
    ///   <c>true</c> if can be visible in the Cyclops displays; otherwise, <c>false</c>.
    /// </value>
    public bool ShowStatusIcon => this.ProvidingPower || this.HasReservePower;

    /// <summary>
    /// Gets a value indicating whether this charger is currently providing power.
    /// </summary>
    /// <value>
    ///   <c>true</c> if currently providing power; otherwise, <c>false</c>.
    /// </value>
    public bool ProvidingPower { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this charger has reserve power.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has reserve power; otherwise, <c>false</c>.
    /// </value>
    public bool HasReservePower { get; private set; }

    internal float Generate(float requestedPower)
    {
        float energy = GenerateNewEnergy(requestedPower);
        gettingEnergy = energy > MCUServices.MinimalPowerValue;
        return energy;
    }

    internal float Drain(float requestedPower)
    {
        float energy = DrainReserveEnergy(requestedPower);
        gettingEnergy |= energy > MCUServices.MinimalPowerValue;
        return energy;
    }

    internal void UpdateStatus()
    {
        this.ProvidingPower = gettingEnergy;
        this.HasReservePower = this.TotalReserveEnergy > MCUServices.MinimalPowerValue;
    }

    /// <summary>
    /// If the charger has its own store of energy, return the total available reserve power.
    /// </summary>
    /// <returns>The total power the charger is capable of providing over time; Return <c>0f</c> if there are no power reserves.</returns>
    public abstract float TotalReserveEnergy { get; }

    /// <summary>
    /// Produces power for the Cyclops during the RechargeCyclops update cycle.<para />
    /// Use this for method energy drawn from the environment is isn't limited by a material resource.<para />
    /// This method should return <c>0f</c> if there is no power avaiable from this charging handler.<para/>
    /// You may limit the amount of power produced to only what the cyclops needs or you may return more.<para/>
    /// DO NOT recharge the Cyclops PowerRelay yourself from this method!!! The MoreCyclopsUpgrades ChargerManager will handle that.<para/>
    /// </summary>
    /// <param name="requestedPower">The amount of power being requested by the cyclops; This is the current Power Deficit of the cyclops.</param>
    /// <returns>The amount of power produced by this cyclops charger.</returns>
    protected abstract float GenerateNewEnergy(float requestedPower);

    /// <summary>
    /// Produces power for the Cyclops during the RechargeCyclops update cycle.<para/>
    /// This method is only invoked if no chargers returned any energy from <see cref="GenerateNewEnergy(float)"/>.<para />
    /// Use this for method energy from batteries, reactor rods, biomass, or anything that can otherwise run out.<para />
    /// This method should return <c>0f</c> if there is no power avaiable from this charging handler.<para/>
    /// You may limit the amount of power produced to only what the cyclops needs or you may return more.<para/>
    /// DO NOT recharge the Cyclops PowerRelay yourself from this method!!! The MoreCyclopsUpgrades PowerManager will handle that.<para/>
    /// </summary>
    /// <param name="requestedPower">The amount of power being requested by the cyclops; This is the current Power Deficit of the cyclops.</param>
    /// <returns>The amount of power produced by this cyclops charger.</returns>
    protected abstract float DrainReserveEnergy(float requestedPower);

    /// <summary>
    /// Gets the sprite to use for the power indicator. This will only be called when <see cref="ShowStatusIcon"/> returns <c>true</c>.
    /// </summary>
    /// <returns>A new <see cref="Atlas.Sprite"/> to be used in the Cyclops Helm and Holographic HUDs.</returns>
    public abstract Sprite StatusSprite();

    /// <summary>
    /// Gets the text to use under the power indicator icon. This will only be called when <see cref="ShowStatusIcon"/> returns <c>true</c>.
    /// </summary>
    /// <returns>A <see cref="string"/>, ready to use for in-game text. Should be limited to numeric values if possible.</returns>
    public abstract string StatusText();

    /// <summary>
    /// Gets the color of the text used under the power indicator icon. This will only be called when <see cref="ShowStatusIcon"/> returns <c>true</c>.
    /// </summary>
    /// <returns>A Unity <see cref="Color"/> value for the text. When in doubt, just set this to white.</returns>
    public abstract Color StatusTextColor();
}
