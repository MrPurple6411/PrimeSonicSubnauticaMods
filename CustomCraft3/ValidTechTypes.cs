﻿namespace CustomCraft3;

using System.Collections.Generic;

internal class ValidTechTypes
{

#if SUBNAUTICA
    // Because other mods (not using Nautilus) are adding to CraftData, the only safe way to get all items is by keeping a local copy here
    // A few known problematic TechTypes have also been excluded
    internal static readonly Dictionary<TechGroup,
                                Dictionary<TechCategory,
                                    List<TechType>>> groups = new Dictionary<TechGroup,
                                                                      Dictionary<TechCategory,
                                                                          List<TechType>>>(CraftData.sTechGroupComparer)
{
    {
        TechGroup.Resources,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.BasicMaterials,
                new List<TechType>
                {
                    TechType.Titanium,
                    TechType.TitaniumIngot,
                    TechType.FiberMesh,
                    TechType.Silicone,
                    TechType.Glass,
                    TechType.Bleach,
                    TechType.Lubricant,
                    TechType.EnameledGlass,
                    TechType.PlasteelIngot
                }
            },
            {
                TechCategory.AdvancedMaterials,
                new List<TechType>
                {
                    TechType.HydrochloricAcid,
                    TechType.Benzene,
                    TechType.AramidFibers,
                    TechType.Aerogel,
                    TechType.Polyaniline,
                    TechType.HatchingEnzymes
                }
            },
            {
                TechCategory.Electronics,
                new List<TechType>
                {
                    TechType.CopperWire,
                    TechType.Battery,
                    TechType.PrecursorIonBattery,
                    TechType.PowerCell,
                    TechType.PrecursorIonPowerCell,
                    TechType.ComputerChip,
                    TechType.WiringKit,
                    TechType.AdvancedWiringKit,
                    TechType.ReactorRod
                }
            }
        }
    },
    {
        TechGroup.Survival,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.Water,
                new List<TechType>
                {
                    TechType.FilteredWater,
                    TechType.DisinfectedWater
                }
            },
            {
                TechCategory.CookedFood,
                new List<TechType>
                {
                    TechType.CookedHoleFish,
                    TechType.CookedPeeper,
                    TechType.CookedBladderfish,
                    TechType.CookedGarryFish,
                    TechType.CookedHoverfish,
                    TechType.CookedReginald,
                    TechType.CookedSpadefish,
                    TechType.CookedBoomerang,
                    TechType.CookedLavaBoomerang,
                    TechType.CookedEyeye,
                    TechType.CookedLavaEyeye,
                    TechType.CookedOculus,
                    TechType.CookedHoopfish,
                    TechType.CookedSpinefish
                }
            },
            {
                TechCategory.CuredFood,
                new List<TechType>
                {
                    TechType.CuredHoleFish,
                    TechType.CuredPeeper,
                    TechType.CuredBladderfish,
                    TechType.CuredGarryFish,
                    TechType.CuredHoverfish,
                    TechType.CuredReginald,
                    TechType.CuredSpadefish,
                    TechType.CuredBoomerang,
                    TechType.CuredLavaBoomerang,
                    TechType.CuredEyeye,
                    TechType.CuredLavaEyeye,
                    TechType.CuredOculus,
                    TechType.CuredHoopfish,
                    TechType.CuredSpinefish
                }
            }
        }
    },
    {
        TechGroup.Personal,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.Equipment,
                new List<TechType>
                {
                    TechType.Tank,
                    TechType.DoubleTank,
                    TechType.Fins,
                    TechType.RadiationSuit,
                    TechType.ReinforcedDiveSuit,
                    TechType.WaterFiltrationSuit,
                    TechType.FirstAidKit,
                    TechType.FireExtinguisher,
                    TechType.Rebreather,
                    TechType.Compass,
                    TechType.Thermometer,
                    TechType.Pipe,
                    TechType.PipeSurfaceFloater,
                    TechType.PrecursorKey_Purple,
                    TechType.PrecursorKey_Blue,
                    TechType.PrecursorKey_Orange
                }
            },
            {
                TechCategory.Tools,
                new List<TechType>
                {
                    TechType.Scanner,
                    TechType.Welder,
                    TechType.Flashlight,
                    TechType.Knife,
                    TechType.DiveReel,
                    TechType.AirBladder,
                    TechType.Flare,
                    TechType.Builder,
                    TechType.LaserCutter,
                    TechType.StasisRifle,
                    TechType.Terraformer,
                    TechType.PropulsionCannon,
                    TechType.LEDLight,
                    TechType.Transfuser
                }
            }
        }
    },
    {
        TechGroup.Machines,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.Machines,
                new List<TechType>
                {
                    TechType.Seaglide,
                    TechType.Constructor,
                    TechType.Beacon,
                    TechType.SmallStorage,
                    TechType.Gravsphere,
                    TechType.CyclopsDecoy
                }
            }
        }
    },
    {
        TechGroup.Constructor,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.Constructor,
                new List<TechType>
                {
                    TechType.Seamoth,
                    TechType.Exosuit,
                    TechType.RocketBase,
                    TechType.RocketBaseLadder,
                    TechType.RocketStage1,
                    TechType.RocketStage2,
                    TechType.RocketStage3
                }
            }
        }
    },
    {
        TechGroup.Workbench,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.Workbench,
                new List<TechType>
                {
                    TechType.LithiumIonBattery,
                    TechType.HeatBlade,
                    TechType.PlasteelTank,
                    TechType.HighCapacityTank,
                    TechType.UltraGlideFins,
                    TechType.SwimChargeFins,
                    TechType.RepulsionCannon,
                    TechType.CyclopsHullModule2,
                    TechType.CyclopsHullModule3,
                    TechType.VehicleHullModule2,
                    TechType.VehicleHullModule3,
                    TechType.ExoHullModule2,
                    TechType.PowerGlide
                }
            }
        }
    },
    {
        TechGroup.VehicleUpgrades,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.VehicleUpgrades,
                new List<TechType>
                {
                    TechType.VehicleHullModule1,
                    TechType.VehicleArmorPlating,
                    TechType.VehiclePowerUpgradeModule,
                    TechType.VehicleStorageModule,
                    TechType.SeamothSolarCharge,
                    TechType.SeamothElectricalDefense,
                    TechType.SeamothTorpedoModule,
                    TechType.SeamothSonarModule,
                    TechType.ExoHullModule1,
                    TechType.ExosuitThermalReactorModule,
                    TechType.ExosuitJetUpgradeModule,
                    TechType.ExosuitPropulsionArmModule,
                    TechType.ExosuitGrapplingArmModule,
                    TechType.ExosuitDrillArmModule,
                    TechType.ExosuitTorpedoArmModule,
                    TechType.WhirlpoolTorpedo,
                    TechType.GasTorpedo
                }
            }
        }
    },
    {
        TechGroup.MapRoomUpgrades,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.MapRoomUpgrades,
                new List<TechType>
                {
                    TechType.MapRoomHUDChip,
                    TechType.MapRoomCamera,
                    TechType.MapRoomUpgradeScanRange,
                    TechType.MapRoomUpgradeScanSpeed
                }
            }
        }
    },
    {
        TechGroup.Cyclops,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.Cyclops,
                new List<TechType>
                {
                    TechType.Cyclops
                }
            },
            {
                TechCategory.CyclopsUpgrades,
                new List<TechType>
                {
                    TechType.CyclopsHullModule1,
                    TechType.PowerUpgradeModule,
                    TechType.CyclopsShieldModule,
                    TechType.CyclopsSonarModule,
                    TechType.CyclopsSeamothRepairModule,
                    TechType.CyclopsFireSuppressionModule,
                    TechType.CyclopsDecoyModule,
                    TechType.CyclopsThermalReactorModule
                }
            }
        }
    },
    {
        TechGroup.BasePieces,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.BasePiece,
                new List<TechType>
                {
                    TechType.BaseFoundation,
                    TechType.BaseCorridorI,
                    TechType.BaseCorridorL,
                    TechType.BaseCorridorT,
                    TechType.BaseCorridorX,
                    TechType.BaseCorridorGlassI,
                    TechType.BaseCorridorGlassL
                }
            },
            {
                TechCategory.BaseRoom,
                new List<TechType>
                {
                    TechType.BaseRoom,
                    TechType.BaseMapRoom,
                    TechType.BaseMoonpool,
                    TechType.BaseObservatory
                }
            },
            {
                TechCategory.BaseWall,
                new List<TechType>
                {
                    TechType.BaseHatch,
                    TechType.BaseWindow,
                    TechType.BaseReinforcement
                }
            }
        }
    },
    {
        TechGroup.ExteriorModules,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.ExteriorModule,
                new List<TechType>
                {
                    TechType.SolarPanel,
                    TechType.ThermalPlant,
                    TechType.PowerTransmitter
                }
            },
            {
                TechCategory.ExteriorLight,
                new List<TechType>
                {
                    TechType.Techlight,
                    TechType.Spotlight
                }
            },
            {
                TechCategory.ExteriorOther,
                new List<TechType>
                {
                    TechType.FarmingTray,
                    TechType.BasePipeConnector
                }
            }
        }
    },
    {
        TechGroup.InteriorPieces,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.InteriorPiece,
                new List<TechType>
                {
                    TechType.BaseLadder,
                    TechType.BaseFiltrationMachine,
                    TechType.BaseBulkhead,
                    TechType.BaseUpgradeConsole
                }
            },
            {
                TechCategory.InteriorRoom,
                new List<TechType>
                {
                    TechType.BaseBioReactor,
                    TechType.BaseNuclearReactor,
                    TechType.BaseWaterPark
                }
            }
        }
    },
    {
        TechGroup.InteriorModules,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.InteriorModule,
                new List<TechType>
                {
                    TechType.Fabricator,
                    TechType.Radio,
                    TechType.MedicalCabinet,
                    TechType.SmallLocker,
                    TechType.Locker,
                    TechType.BatteryCharger,
                    TechType.PowerCellCharger,
                    TechType.Aquarium,
                    TechType.Workbench,
                    TechType.Centrifuge,
                    TechType.PlanterPot,
                    TechType.PlanterPot2,
                    TechType.PlanterPot3,
                    TechType.PlanterBox,
                    TechType.PlanterShelf
                }
            }
        }
    },
    {
        TechGroup.Miscellaneous,
        new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
        {
            {
                TechCategory.Misc,
                new List<TechType>
                {
                    TechType.Bench,
                    TechType.Bed1,
                    TechType.Bed2,
                    TechType.NarrowBed,
                    TechType.StarshipDesk,
                    TechType.StarshipChair,
                    TechType.StarshipChair2,
                    TechType.StarshipChair3,
                    TechType.Sign,
                    TechType.PictureFrame,
                    TechType.StarshipCargoCrate,
                    TechType.StarshipCircuitBox,
                    TechType.StarshipMonitor,
                    TechType.BarTable,
                    TechType.Trashcans,
                    TechType.LabTrashcan,
                    TechType.VendingMachine,
                    TechType.CoffeeVendingMachine,
                    TechType.LabCounter,
                    TechType.BasePlanter,
                    TechType.SingleWallShelf,
                    TechType.WallShelves
                }
            },
            {
                TechCategory.MiscHullplates,
                new List<TechType>
                {
                    TechType.DevTestItem,
                    TechType.SpecialHullPlate,
                    TechType.BikemanHullPlate,
                    TechType.EatMyDictionHullPlate,
                    TechType.DioramaHullPlate,
                    TechType.MarkiplierHullPlate,
                    TechType.MuyskermHullPlate,
                    TechType.LordMinionHullPlate,
                    TechType.JackSepticEyeHullPlate,
                    TechType.IGPHullPlate,
                    TechType.GilathissHullPlate,
                    TechType.Marki1,
                    TechType.Marki2,
                    TechType.JackSepticEye,
                    TechType.EatMyDiction
                }
            }
        }
    }
};

#elif BELOWZERO
    // Because other mods (not using Nautilus) are adding to CraftData, the only safe way to get all items is by keeping a local copy here
    // A few known problematic TechTypes have also been excluded
    internal static readonly Dictionary<TechGroup,
                                Dictionary<TechCategory,
                                    List<TechType>>> groups = new Dictionary<TechGroup,
                                                                      Dictionary<TechCategory,
                                                                          List<TechType>>>(CraftData.sTechGroupComparer)
{{
    TechGroup.Resources,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.BasicMaterials,
            new List<TechType>
            {
                TechType.Titanium,
                TechType.TitaniumIngot,
                TechType.FiberMesh,
                TechType.Silicone,
                TechType.Glass,
                TechType.Lubricant,
                TechType.EnameledGlass,
                TechType.PlasteelIngot
            }
        },
        {
            TechCategory.AdvancedMaterials,
            new List<TechType>
            {
                TechType.HydrochloricAcid,
                TechType.Benzene,
                TechType.AramidFibers,
                TechType.Aerogel,
                TechType.Polyaniline,
                TechType.HydraulicFluid
            }
        },
        {
            TechCategory.Electronics,
            new List<TechType>
            {
                TechType.CopperWire,
                TechType.Battery,
                TechType.PrecursorIonBattery,
                TechType.PowerCell,
                TechType.PrecursorIonPowerCell,
                TechType.ComputerChip,
                TechType.WiringKit,
                TechType.AdvancedWiringKit,
                TechType.ReactorRod,
                TechType.RadioTowerPPU,
                TechType.RadioTowerTOM
            }
        }
    }
},
{
    TechGroup.Survival,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.FoodAndDrinks,
            new List<TechType>
            {
                TechType.SpicyFruitSalad,
                TechType.FilteredWater,
                TechType.WaterPurificationTablet,
                TechType.Coffee
            }
        }
    }
},
{
    TechGroup.Personal,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.Equipment,
            new List<TechType>
            {
                TechType.Tank,
                TechType.DoubleTank,
                TechType.SuitBoosterTank,
                TechType.Fins,
                TechType.ReinforcedDiveSuit,
                TechType.WaterFiltrationSuit,
                TechType.ColdSuit,
                TechType.ColdSuitGloves,
                TechType.ColdSuitHelmet,
                TechType.FirstAidKit,
                TechType.FireExtinguisher,
                TechType.Rebreather,
                TechType.Compass,
                TechType.Pipe,
                TechType.PipeSurfaceFloater,
                TechType.FlashlightHelmet
            }
        },
        {
            TechCategory.Tools,
            new List<TechType>
            {
                TechType.Scanner,
                TechType.Welder,
                TechType.Flashlight,
                TechType.Knife,
                TechType.DiveReel,
                TechType.AirBladder,
                TechType.Flare,
                TechType.Builder,
                TechType.LaserCutter,
                TechType.Terraformer,
                TechType.PropulsionCannon,
                TechType.LEDLight,
                TechType.Transfuser,
                TechType.Thumper,
                TechType.MetalDetector,
                TechType.SpyPenguin,
                TechType.SpyPenguinRemote,
                TechType.TeleportationTool
            }
        }
    }
},
{
    TechGroup.Machines,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.Machines,
            new List<TechType>
            {
                TechType.Seaglide,
                TechType.Constructor,
                TechType.Beacon,
                TechType.SmallStorage,
                TechType.QuantumLocker,
                TechType.Gravsphere,
                TechType.CyclopsDecoy,
                TechType.Hoverbike
            }
        }
    }
},
{
    TechGroup.Constructor,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.Constructor,
            new List<TechType>
            {
                TechType.Exosuit,
                TechType.RocketStage1,
                TechType.RocketStage2,
                TechType.RocketStage3,
                TechType.SeaTruck,
                TechType.SeaTruckAquariumModule,
                TechType.SeaTruckDockingModule,
                TechType.SeaTruckFabricatorModule,
                TechType.SeaTruckSleeperModule,
                TechType.SeaTruckStorageModule,
                TechType.SeaTruckTeleportationModule
            }
        }
    }
},
{
    TechGroup.VehicleUpgrades,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.VehicleUpgrades,
            new List<TechType>
            {
                TechType.VehicleHullModule1,
                TechType.VehicleArmorPlating,
                TechType.VehiclePowerUpgradeModule,
                TechType.VehicleStorageModule,
                TechType.SeamothSolarCharge,
                TechType.SeamothElectricalDefense,
                TechType.SeamothTorpedoModule,
                TechType.SeamothSonarModule,
                TechType.ExoHullModule1,
                TechType.ExosuitThermalReactorModule,
                TechType.ExosuitJetUpgradeModule,
                TechType.ExosuitPropulsionArmModule,
                TechType.ExosuitGrapplingArmModule,
                TechType.ExosuitDrillArmModule,
                TechType.ExosuitTorpedoArmModule,
                TechType.WhirlpoolTorpedo,
                TechType.GasTorpedo,
                TechType.SeaTruckUpgradeAfterburner,
                TechType.SeaTruckUpgradeEnergyEfficiency,
                TechType.SeaTruckUpgradeHorsePower,
                TechType.SeaTruckUpgradeHull1,
                TechType.SeaTruckUpgradePerimeterDefense,
                TechType.SeaTruckUpgradeThruster,
                TechType.HoverbikeJumpModule,
                TechType.HoverbikeIceWormReductionModule
            }
        }
    }
},
{
    TechGroup.Workbench,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.Workbench,
            new List<TechType>
            {
                TechType.LithiumIonBattery,
                TechType.HeatBlade,
                TechType.HighCapacityTank,
                TechType.SwimChargeFins,
                TechType.VehicleHullModule2,
                TechType.VehicleHullModule3,
                TechType.SeaTruckUpgradeHull2,
                TechType.SeaTruckUpgradeHull3,
                TechType.ExoHullModule2,
                TechType.PowerGlide
            }
        }
    }
},
{
    TechGroup.MapRoomUpgrades,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.MapRoomUpgrades,
            new List<TechType>
            {
                TechType.MapRoomHUDChip,
                TechType.MapRoomCamera,
                TechType.MapRoomUpgradeScanRange,
                TechType.MapRoomUpgradeScanSpeed
            }
        }
    }
},
{
    TechGroup.BasePieces,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.BasePiece,
            new List<TechType>
            {
                TechType.BaseFoundation,
                TechType.BaseWallFoundation,
                TechType.BaseCorridorI,
                TechType.BaseCorridorL,
                TechType.BaseCorridorT,
                TechType.BaseCorridorX,
                TechType.BaseCorridorGlassI,
                TechType.BaseCorridorGlassL,
                TechType.BaseConnector,
                TechType.BaseRoom,
                TechType.BaseMapRoom,
                TechType.BaseMoonpool,
                TechType.BaseObservatory,
                TechType.BaseControlRoom,
                TechType.BaseLargeRoom,
                TechType.BaseGlassDome,
                TechType.BaseLargeGlassDome,
                TechType.BaseHatch,
                TechType.BaseWindow,
                TechType.BaseReinforcement
            }
        }
    }
},
{
    TechGroup.ExteriorModules,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.ExteriorModule,
            new List<TechType>
            {
                TechType.SolarPanel,
                TechType.ThermalPlant,
                TechType.PowerTransmitter,
                TechType.Hoverpad,
                TechType.Techlight,
                TechType.Spotlight,
                TechType.FarmingTray,
                TechType.BasePipeConnector,
                TechType.Snowman
            }
        }
    }
},
{
    TechGroup.InteriorPieces,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.InteriorPiece,
            new List<TechType>
            {
                TechType.BaseLadder,
                TechType.BaseFiltrationMachine,
                TechType.BaseBulkhead,
                TechType.BaseUpgradeConsole,
                TechType.BaseBioReactor,
                TechType.BaseNuclearReactor,
                TechType.BaseWaterPark,
                TechType.BasePartition,
                TechType.BasePartitionDoor
            }
        }
    }
},
{
    TechGroup.InteriorModules,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.InteriorModule,
            new List<TechType>
            {
                TechType.Fabricator,
                TechType.Radio,
                TechType.MedicalCabinet,
                TechType.SmallLocker,
                TechType.Locker,
                TechType.BatteryCharger,
                TechType.PowerCellCharger,
                TechType.Aquarium,
                TechType.Workbench,
                TechType.Jukebox,
                TechType.Speaker,
                TechType.PlanterPot,
                TechType.PlanterPot2,
                TechType.PlanterPot3,
                TechType.PlanterBox,
                TechType.PlanterShelf,
                TechType.Recyclotron,
                TechType.CoffeeVendingMachine
            }
        }
    }
},
{
    TechGroup.Miscellaneous,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.Misc,
            new List<TechType>
            {
                TechType.Bench,
                TechType.Bed1,
                TechType.Bed2,
                TechType.NarrowBed,
                TechType.BedJeremiah,
                TechType.BedSam,
                TechType.BedZeta,
                TechType.BedDanielle,
                TechType.BedEmmanuel,
                TechType.BedFred,
                TechType.BedParvan,
                TechType.StarshipDesk,
                TechType.ExecutiveDesk,
                TechType.StarshipChair,
                TechType.StarshipChair2,
                TechType.StarshipChair3,
                TechType.Sign,
                TechType.PictureFrame,
                TechType.StarshipCargoCrate,
                TechType.StarshipCircuitBox,
                TechType.StarshipMonitor,
                TechType.BarTable,
                TechType.Trashcans,
                TechType.LabTrashcan,
                TechType.VendingMachine,
                TechType.LabCounter,
                TechType.Fridge,
                TechType.Shower,
                TechType.Sink,
                TechType.SmallStove,
                TechType.Toilet,
                TechType.BasePlanter,
                TechType.SingleWallShelf,
                TechType.WallShelves,
                TechType.AromatherapyLamp,
                TechType.EmmanuelPendulum
            }
        },
        {
            TechCategory.MiscHullplates,
            new List<TechType>
            {
                TechType.DevTestItem,
                TechType.SpecialHullPlate,
                TechType.BikemanHullPlate,
                TechType.EatMyDictionHullPlate,
                TechType.DioramaHullPlate,
                TechType.MarkiplierHullPlate,
                TechType.MuyskermHullPlate,
                TechType.LordMinionHullPlate,
                TechType.JackSepticEyeHullPlate,
                TechType.IGPHullPlate,
                TechType.GilathissHullPlate,
                TechType.Marki1,
                TechType.JackSepticEye,
                TechType.EatMyDiction
            }
        }
    }
},
{
    TechGroup.Precursor,
    new Dictionary<TechCategory, List<TechType>>(CraftData.sTechCategoryComparer)
    {
        {
            TechCategory.PrecursorBodyParts,
            new List<TechType>
            {
                TechType.PrecursorNPCTissue,
                TechType.PrecursorNPCSkeleton,
                TechType.PrecursorNPCOrgans
            }
        }
    }
}
};


#endif
}
