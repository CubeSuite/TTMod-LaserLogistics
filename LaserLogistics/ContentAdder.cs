using EquinoxsModUtils.Additions;
using EquinoxsModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.ComponentModel;

namespace LaserLogistics
{
    internal static class ContentAdder
    {
        internal static void AddHeaders() {
            EMUAdditions.AddNewSchematicsSubHeader("Lasers", "Logistics", 999);
            EMUAdditions.AddNewSchematicsSubHeader("Laser Modules", "Intermediates", 998);
            EMUAdditions.AddNewSchematicsSubHeader("Laser Upgrades", "Intermediates", 999);
            EMUAdditions.AddNewSchematicsSubHeader("Laser Logistics", "Equipment", 999);
        }

        internal static void AddUnlocks() {
            EMUAdditions.AddNewUnlock(new EquinoxsModUtils.Additions.NewUnlockDetails() {
                displayName = Names.Unlocks.laserNodes,
                description = $"Unlocks {Names.Items.laserNode}s and basic Modules",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier4,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Red,
                coreCountNeeded = 20,
                treePosition = 20,
                // ToDo: LaserNode Unlock Sprite
            });

            EMUAdditions.AddNewUnlock(new EquinoxsModUtils.Additions.NewUnlockDetails() {
                displayName = Names.Unlocks.voidModule,
                description = "Destroys the item stack stored in the Laser Node",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier7,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Red,
                coreCountNeeded = 50,
                treePosition = 20,
                sprite = Images.Modules.voidModule.sprite
            });

            EMUAdditions.AddNewUnlock(new EquinoxsModUtils.Additions.NewUnlockDetails() {
                displayName = Names.Unlocks.advancedModules,
                description = "Unlocks modules that can interact with up to 8 inventories at once",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier9,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Green,
                coreCountNeeded = 100,
                treePosition = 20,
                sprite = Images.Modules.distributorModule.sprite
            });

            EMUAdditions.AddNewUnlock(new EquinoxsModUtils.Additions.NewUnlockDetails() {
                displayName = Names.Unlocks.quantumModules,
                description = "Unlocks the Quantum Storage Network",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier10,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Green,
                coreCountNeeded = 200,
                treePosition = 40,
                sprite = Images.Modules.compressorModule.sprite
            });

            EMUAdditions.AddNewUnlock(new EquinoxsModUtils.Additions.NewUnlockDetails() {
                displayName = Names.Unlocks.rangeUpgrade,
                description = $"Increases the range of the Laser Node by {Settings.rangeUpgradeAmount}",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier6,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Green,
                coreCountNeeded = 25,
                treePosition = 20,
                sprite = Images.Upgrades.rangeUpgrade.sprite
            });

            EMUAdditions.AddNewUnlock(new EquinoxsModUtils.Additions.NewUnlockDetails() {
                displayName = Names.Unlocks.infiniteRangeUpgrade,
                description = "Allows for unlimited range",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier10,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Green,
                coreCountNeeded = 200,
                treePosition = 20,
                sprite = Images.Upgrades.infiniteRangeUpgrade.sprite
            });

            EMUAdditions.AddNewUnlock(new EquinoxsModUtils.Additions.NewUnlockDetails() {
                displayName = Names.Unlocks.speedUpgrade,
                description = $"Decreases the delay between tasks by {Settings.speedUpgradeAmount}s",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier5,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Red,
                coreCountNeeded = 50,
                treePosition = 20,
                sprite = Images.Upgrades.speedUpgrade.sprite
            });

            EMUAdditions.AddNewUnlock(new EquinoxsModUtils.Additions.NewUnlockDetails() {
                displayName = Names.Unlocks.stackUpgrade,
                description = $"Increases the items transferred per task by {Settings.stackUpgradeAmount}",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier8,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Green,
                coreCountNeeded = 50,
                treePosition = 20,
                sprite = Images.Upgrades.stackUpgrade.sprite
            });
        }

        internal static void AddPMT() {
            EquinoxsModUtils.Additions.NewResourceDetails details = new EquinoxsModUtils.Additions.NewResourceDetails() {
                name = Names.Items.pmt,
                description = "Stores the location of inventories for importing into Laser Modules",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Equipment",
                subHeaderTitle = "Laser Logistics",
                maxStackCount = 1,
                sortPriority = 0,
                unlockName = Names.Unlocks.laserNodes,
                parentName = ResourceNames.Scanner,
                sprite = Images.pmt.sprite,
            };

            EMUAdditions.AddNewEquipment<PositionMemoryTabletInfo>(PositionMemoryTablet.instance, details);
           
            EMUAdditions.AddNewRecipe(new EquinoxsModUtils.Additions.NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 10f,
                unlockName = Names.Unlocks.laserNodes,
                ingredients = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.IronIngot,
                        quantity = 1
                    }
                },
                outputs = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.pmt,
                        quantity = 1
                    }
                }
            });
        }

        // Buildings

        internal static void AddLaserNode() {
            EquinoxsModUtils.Additions.NewResourceDetails details = new EquinoxsModUtils.Additions.NewResourceDetails() {
                name = Names.Items.laserNode,
                description = "Uses Modules to move and store items",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Logistics",
                subHeaderTitle = "Lasers",
                maxStackCount = 500,
                sortPriority = 0,
                unlockName = Names.Unlocks.laserNodes,
                parentName = ResourceNames.FilterInserter
            };

            InserterDefinition definition;
            definition = ScriptableObject.CreateInstance<InserterDefinition>();
            EMUAdditions.AddNewMachine(definition, details);

            EMUAdditions.AddNewRecipe(new EquinoxsModUtils.Additions.NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 10f,
                unlockName = Names.Unlocks.laserNodes,
                ingredients = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.IronFrame,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.ProcessorUnit,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.laserNode,
                        quantity = 1
                    }
                }
            });
        }
        
        // Modules

        internal static void AddPullerModule() {
            EMUAdditions.AddNewResource(new EquinoxsModUtils.Additions.NewResourceDetails() {
                name = Names.Items.pullerModule,
                description = "Moves items from the target inventory to the Laser Node",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 0,
                unlockName = Names.Unlocks.laserNodes,
                parentName = ResourceNames.ProcessorUnit,
                sprite = Images.Modules.pullerModule.sprite
            });

            EMUAdditions.AddNewRecipe(new EquinoxsModUtils.Additions.NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.laserNodes,
                sortPriority = 0,
                ingredients = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.ConveyorBelt,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo(){
                        name = ResourceNames.Inserter,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.pullerModule,
                        quantity = 1
                    }
                }
            });
        }
        
        internal static void AddPusherModule() {
            EMUAdditions.AddNewResource(new EquinoxsModUtils.Additions.NewResourceDetails() {
                name = Names.Items.pusherModule,
                description = "Moves items from the Laser Node to the target inventory",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 1,
                unlockName = Names.Unlocks.laserNodes,
                parentName = ResourceNames.ProcessorUnit,
                sprite = Images.Modules.pusherModule.sprite
            });

            EMUAdditions.AddNewRecipe(new EquinoxsModUtils.Additions.NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.laserNodes,
                sortPriority = 1,
                ingredients = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.ConveyorBelt,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.Inserter,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.pusherModule,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddCollectorModule() {
            EMUAdditions.AddNewResource(new EquinoxsModUtils.Additions.NewResourceDetails() {
                name = Names.Items.collectorModule,
                description = "Moves items from the target inventories to the Laser Node",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 2,
                unlockName = Names.Unlocks.advancedModules,
                parentName = ResourceNames.ProcessorUnit,
                sprite = Images.Modules.collectorModule.sprite
            });

            EMUAdditions.AddNewRecipe(new EquinoxsModUtils.Additions.NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.advancedModules,
                sortPriority = 3,
                ingredients = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.pullerModule,
                        quantity = 8
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.AdvancedCircuit,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.collectorModule,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddDistributorModule() {
            EMUAdditions.AddNewResource(new EquinoxsModUtils.Additions.NewResourceDetails() {
                name = Names.Items.distributorModule,
                description = "Moves items from the Laser Node to the target inventories",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 3,
                unlockName = Names.Unlocks.advancedModules,
                parentName = ResourceNames.ProcessorUnit,
                sprite = Images.Modules.distributorModule.sprite
            });

            EMUAdditions.AddNewRecipe(new EquinoxsModUtils.Additions.NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.advancedModules,
                sortPriority = 3,
                ingredients = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.pusherModule,
                        quantity = 8
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.AdvancedCircuit,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.distributorModule,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddVoidModule() {
            EMUAdditions.AddNewResource(new EquinoxsModUtils.Additions.NewResourceDetails() {
                name = Names.Items.voidModule,
                description = "Destroys the item stack stored in the Laser Node",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 4,
                unlockName = Names.Unlocks.voidModule,
                parentName = ResourceNames.ProcessorUnit,
                sprite = Images.Modules.voidModule.sprite
            });

            EMUAdditions.AddNewRecipe(new EquinoxsModUtils.Additions.NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.voidModule,
                sortPriority = 4,
                ingredients = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.TheMOLE,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.Container,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.voidModule,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddCompressorModule() {
            EMUAdditions.AddNewResource(new EquinoxsModUtils.Additions.NewResourceDetails() {
                name = Names.Items.compressorModule,
                description = "Stores the item in the Laser Node in your Quantum Storage Network",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 5,
                unlockName = Names.Unlocks.quantumModules,
                parentName = ResourceNames.ProcessorUnit,
                sprite = Images.Modules.compressorModule.sprite
            });

            EMUAdditions.AddNewRecipe(new EquinoxsModUtils.Additions.NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.quantumModules,
                sortPriority = 5,
                ingredients = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.TheMOLE,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.RelayCircuit,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.Gearbox, 
                        quantity = 1
                    }
                },
                outputs = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.compressorModule,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddExpanderModule() {
            EMUAdditions.AddNewResource(new EquinoxsModUtils.Additions.NewResourceDetails() {
                name = Names.Items.expanderModule,
                description = "Retrieves the filtered item from your Quantum Storage Network",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 6,
                unlockName = Names.Unlocks.quantumModules,
                parentName = ResourceNames.ProcessorUnit,
                sprite = Images.Modules.expanderModule.sprite
            });

            EMUAdditions.AddNewRecipe(new EquinoxsModUtils.Additions.NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.quantumModules,
                sortPriority = 6,
                ingredients = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.TheMOLE,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.RelayCircuit,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.Gearbox,
                        quantity = 1
                    }
                },
                outputs = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.expanderModule,
                        quantity = 1
                    }
                }
            });
        }

        // Upgrades

        internal static void AddSpeedUpgrade() {
            EMUAdditions.AddNewResource(new EquinoxsModUtils.Additions.NewResourceDetails() {
                name = Names.Items.speedUpgrade,
                description = $"Decreases the delay between tasks by {Settings.speedUpgradeAmount}s",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Upgrades",
                maxStackCount = 50,
                sortPriority = 0,
                unlockName = Names.Unlocks.speedUpgrade,
                parentName = ResourceNames.ProcessorUnit,
                sprite = Images.Upgrades.speedUpgrade.sprite
            });

            EMUAdditions.AddNewRecipe(new EquinoxsModUtils.Additions.NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 6f,
                unlockName = Names.Unlocks.speedUpgrade,
                sortPriority = 0,
                ingredients = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.KindlevineExtract,
                        quantity = 10
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.speedUpgrade,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddStackUpgrade() {
            EMUAdditions.AddNewResource(new EquinoxsModUtils.Additions.NewResourceDetails() {
                name = Names.Items.stackUpgrade,
                description = $"Increases the items transferred per task by {Settings.stackUpgradeAmount}",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Upgrades",
                maxStackCount = 50,
                sortPriority = 1,
                unlockName = Names.Unlocks.stackUpgrade,
                parentName = ResourceNames.ProcessorUnit,
                sprite = Images.Upgrades.stackUpgrade.sprite
            });

            EMUAdditions.AddNewRecipe(new EquinoxsModUtils.Additions.NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 60f,
                unlockName = Names.Unlocks.stackUpgrade,
                sortPriority = 1,
                ingredients = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.speedUpgrade,
                        quantity = 10
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.AdvancedCircuit,
                        quantity = 1
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.stackUpgrade,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddRangeUpgrade() {
            EMUAdditions.AddNewResource(new EquinoxsModUtils.Additions.NewResourceDetails() {
                name = Names.Items.rangeUpgrade,
                description = $"Increases the range of the Laser Node by {Settings.rangeUpgradeAmount}",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Upgrades",
                maxStackCount = 50,
                sortPriority = 2,
                unlockName = Names.Unlocks.rangeUpgrade,
                parentName = ResourceNames.ProcessorUnit,
                sprite = Images.Upgrades.rangeUpgrade.sprite
            });

            EMUAdditions.AddNewRecipe(new EquinoxsModUtils.Additions.NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 6f,
                unlockName = Names.Unlocks.rangeUpgrade,
                sortPriority = 2,
                ingredients = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.HighVoltageCable,
                        quantity = 2
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.rangeUpgrade,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddInfiniteRangeUpgrade() {
            EMUAdditions.AddNewResource(new EquinoxsModUtils.Additions.NewResourceDetails() {
                name = Names.Items.infiniteRangeUpgrade,
                description = "Allows for unlimited range",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Upgrades",
                maxStackCount = 1,
                sortPriority = 3,
                unlockName = Names.Unlocks.infiniteRangeUpgrade,
                parentName = ResourceNames.ProcessorUnit,
                sprite = Images.Upgrades.infiniteRangeUpgrade.sprite
            });

            EMUAdditions.AddNewRecipe(new EquinoxsModUtils.Additions.NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 60f,
                unlockName = Names.Unlocks.infiniteRangeUpgrade,
                sortPriority = 3,
                ingredients = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.rangeUpgrade,
                        quantity = 10
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.AdvancedCircuit,
                        quantity = 2
                    },
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = ResourceNames.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<EquinoxsModUtils.Additions.RecipeResourceInfo>() {
                    new EquinoxsModUtils.Additions.RecipeResourceInfo() {
                        name = Names.Items.infiniteRangeUpgrade,
                        quantity = 1
                    }
                }
            });
        }
    }
}
