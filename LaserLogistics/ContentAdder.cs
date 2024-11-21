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
            EMUAdditions.AddNewUnlock(new NewUnlockDetails() {
                displayName = Names.Unlocks.laserNodes,
                description = $"Unlocks {Names.Items.laserNode}s and basic Modules",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier4,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Purple,
                coreCountNeeded = 20,
                treePosition = 20,
                sprite = Images.laserNode.sprite
            });

            EMUAdditions.AddNewUnlock(new NewUnlockDetails() {
                displayName = Names.Unlocks.voidModule,
                description = "Destroys the item stack stored in the Laser Node",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier5,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Purple,
                coreCountNeeded = 50,
                treePosition = 40,
                sprite = Images.Modules.voidModule.sprite
            });

            EMUAdditions.AddNewUnlock(new NewUnlockDetails() {
                displayName = Names.Unlocks.advancedModules,
                description = "Unlocks modules that can interact with up to 8 inventories at once",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier15,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Blue,
                coreCountNeeded = 400,
                treePosition = 20,
                sprite = Images.Modules.distributorModule.sprite
            });

            EMUAdditions.AddNewUnlock(new NewUnlockDetails() {
                displayName = Names.Unlocks.quantumModules,
                description = "Unlocks the Quantum Storage Network",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier20,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Green,
                coreCountNeeded = 200,
                treePosition = 40,
                sprite = Images.Modules.compressorModule.sprite
            });

            EMUAdditions.AddNewUnlock(new NewUnlockDetails() {
                displayName = Names.Unlocks.rangeUpgrade,
                description = $"Increases the range of the Laser Node by {Settings.rangeUpgradeAmount}",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier7,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Blue,
                coreCountNeeded = 25,
                treePosition = 40,
                sprite = Images.Upgrades.rangeUpgrade.sprite
            });

            EMUAdditions.AddNewUnlock(new NewUnlockDetails() {
                displayName = Names.Unlocks.infiniteRangeUpgrade,
                description = "Allows for unlimited range",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier10,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Blue,
                coreCountNeeded = 500,
                treePosition = 40,
                sprite = Images.Upgrades.infiniteRangeUpgrade.sprite
            });

            EMUAdditions.AddNewUnlock(new NewUnlockDetails() {
                displayName = Names.Unlocks.speedUpgrade,
                description = $"Decreases the delay between tasks by {Settings.speedUpgradeAmount}s",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier6,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Purple,
                coreCountNeeded = 50,
                treePosition = 40,
                sprite = Images.Upgrades.speedUpgrade.sprite
            });

            EMUAdditions.AddNewUnlock(new NewUnlockDetails() {
                displayName = Names.Unlocks.stackUpgrade,
                description = $"Increases the items transferred per task by {Settings.stackUpgradeAmount}",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier17,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Green,
                coreCountNeeded = 100,
                treePosition = 20,
                sprite = Images.Upgrades.stackUpgrade.sprite
            });

            EMUAdditions.AddNewUnlock(new NewUnlockDetails() {
                displayName = Names.Unlocks.qsnDownload,
                description = $"Maintain  up to 1 stack of each buildable from your Quantum Storage Network",
                category = Unlock.TechCategory.Logistics,
                requiredTier = TechTreeState.ResearchTier.Tier21,
                coreTypeNeeded = ResearchCoreDefinition.CoreType.Gold,
                coreCountNeeded = 200,
                treePosition = 40,
                sprite = Images.qsnDownload.sprite,
                dependencyNames = new List<string>() { Names.Unlocks.quantumModules }
            });
        }

        internal static void AddPMT() {
            NewResourceDetails details = new NewResourceDetails() {
                name = Names.Items.pmt,
                description = "Stores the location of inventories for importing into Laser Modules",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Equipment",
                subHeaderTitle = "Laser Logistics",
                maxStackCount = 1,
                sortPriority = 0,
                unlockName = Names.Unlocks.laserNodes,
                parentName = EMU.Names.Resources.Scanner,
                sprite = Images.pmt.sprite,
            };

            EMUAdditions.AddNewEquipment<PositionMemoryTabletInfo>(PositionMemoryTablet.instance, details);
           
            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 10f,
                unlockName = Names.Unlocks.laserNodes,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.IronIngot,
                        quantity = 1
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.pmt,
                        quantity = 1
                    }
                }
            });
        }

        // Buildings

        internal static void AddLaserNode() {
            NewResourceDetails details = new NewResourceDetails() {
                name = Names.Items.laserNode,
                description = "Uses Modules to move and store items",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Logistics",
                subHeaderTitle = "Lasers",
                maxStackCount = 500,
                sortPriority = 0,
                unlockName = Names.Unlocks.laserNodes,
                parentName = EMU.Names.Resources.FilterInserter,
                sprite = Images.laserNode.sprite
            };

            InserterDefinition laserNodeDefinition;
            laserNodeDefinition = ScriptableObject.CreateInstance<InserterDefinition>();
            EMUAdditions.AddNewMachine(laserNodeDefinition, details);

            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 10f,
                unlockName = Names.Unlocks.laserNodes,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.IronFrame,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.ProcessorUnit,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.laserNode,
                        quantity = 1
                    }
                }
            });
        }
        
        // Modules

        internal static void AddPullerModule() {
            EMUAdditions.AddNewResource(new NewResourceDetails() {
                name = Names.Items.pullerModule,
                description = "Moves items from the target inventory to the Laser Node",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 0,
                unlockName = Names.Unlocks.laserNodes,
                parentName = EMU.Names.Resources.ProcessorUnit,
                sprite = Images.Modules.pullerModule.sprite
            });

            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.laserNodes,
                sortPriority = 0,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.ConveyorBelt,
                        quantity = 1
                    },
                    new RecipeResourceInfo(){
                        name = EMU.Names.Resources.Inserter,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.pullerModule,
                        quantity = 1
                    }
                }
            });
        }
        
        internal static void AddPusherModule() {
            EMUAdditions.AddNewResource(new NewResourceDetails() {
                name = Names.Items.pusherModule,
                description = "Moves items from the Laser Node to the target inventory",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 1,
                unlockName = Names.Unlocks.laserNodes,
                parentName = EMU.Names.Resources.ProcessorUnit,
                sprite = Images.Modules.pusherModule.sprite
            });

            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.laserNodes,
                sortPriority = 1,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.ConveyorBelt,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.Inserter,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.pusherModule,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddCollectorModule() {
            EMUAdditions.AddNewResource(new NewResourceDetails() {
                name = Names.Items.collectorModule,
                description = "Moves items from the target inventories to the Laser Node",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 2,
                unlockName = Names.Unlocks.advancedModules,
                parentName = EMU.Names.Resources.ProcessorUnit,
                sprite = Images.Modules.collectorModule.sprite
            });

            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.advancedModules,
                sortPriority = 3,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.pullerModule,
                        quantity = 8
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.AdvancedCircuit,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.collectorModule,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddDistributorModule() {
            EMUAdditions.AddNewResource(new NewResourceDetails() {
                name = Names.Items.distributorModule,
                description = "Moves items from the Laser Node to the target inventories",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 3,
                unlockName = Names.Unlocks.advancedModules,
                parentName = EMU.Names.Resources.ProcessorUnit,
                sprite = Images.Modules.distributorModule.sprite
            });

            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.advancedModules,
                sortPriority = 3,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.pusherModule,
                        quantity = 8
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.AdvancedCircuit,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.distributorModule,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddVoidModule() {
            EMUAdditions.AddNewResource(new NewResourceDetails() {
                name = Names.Items.voidModule,
                description = "Destroys the item stack stored in the Laser Node",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 4,
                unlockName = Names.Unlocks.voidModule,
                parentName = EMU.Names.Resources.ProcessorUnit,
                sprite = Images.Modules.voidModule.sprite
            });

            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.voidModule,
                sortPriority = 4,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.TheMOLE,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.Container,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.voidModule,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddCompressorModule() {
            EMUAdditions.AddNewResource(new NewResourceDetails() {
                name = Names.Items.compressorModule,
                description = "Stores the item in the Laser Node in your Quantum Storage Network",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 5,
                unlockName = Names.Unlocks.quantumModules,
                parentName = EMU.Names.Resources.ProcessorUnit,
                sprite = Images.Modules.compressorModule.sprite
            });

            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.quantumModules,
                sortPriority = 5,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.TheMOLE,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.RelayCircuit,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.Gearbox, 
                        quantity = 1
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.compressorModule,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddExpanderModule() {
            EMUAdditions.AddNewResource(new NewResourceDetails() {
                name = Names.Items.expanderModule,
                description = "Retrieves the filtered item from your Quantum Storage Network",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Modules",
                maxStackCount = 50,
                sortPriority = 6,
                unlockName = Names.Unlocks.quantumModules,
                parentName = EMU.Names.Resources.ProcessorUnit,
                sprite = Images.Modules.expanderModule.sprite
            });

            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 0.5f,
                unlockName = Names.Unlocks.quantumModules,
                sortPriority = 6,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.TheMOLE,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.RelayCircuit,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.Gearbox,
                        quantity = 1
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.expanderModule,
                        quantity = 1
                    }
                }
            });
        }

        // Upgrades

        internal static void AddSpeedUpgrade() {
            EMUAdditions.AddNewResource(new NewResourceDetails() {
                name = Names.Items.speedUpgrade,
                description = $"Decreases the delay between tasks by {Settings.speedUpgradeAmount}s",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Upgrades",
                maxStackCount = 50,
                sortPriority = 0,
                unlockName = Names.Unlocks.speedUpgrade,
                parentName = EMU.Names.Resources.ProcessorUnit,
                sprite = Images.Upgrades.speedUpgrade.sprite
            });

            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 6f,
                unlockName = Names.Unlocks.speedUpgrade,
                sortPriority = 0,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.KindlevineExtract,
                        quantity = 10
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.speedUpgrade,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddStackUpgrade() {
            EMUAdditions.AddNewResource(new NewResourceDetails() {
                name = Names.Items.stackUpgrade,
                description = $"Increases the items transferred per task by {Settings.stackUpgradeAmount}",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Upgrades",
                maxStackCount = 50,
                sortPriority = 1,
                unlockName = Names.Unlocks.stackUpgrade,
                parentName = EMU.Names.Resources.ProcessorUnit,
                sprite = Images.Upgrades.stackUpgrade.sprite
            });

            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 60f,
                unlockName = Names.Unlocks.stackUpgrade,
                sortPriority = 1,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.speedUpgrade,
                        quantity = 10
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.AdvancedCircuit,
                        quantity = 1
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.stackUpgrade,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddRangeUpgrade() {
            EMUAdditions.AddNewResource(new NewResourceDetails() {
                name = Names.Items.rangeUpgrade,
                description = $"Increases the range of the Laser Node by {Settings.rangeUpgradeAmount}",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Upgrades",
                maxStackCount = 50,
                sortPriority = 2,
                unlockName = Names.Unlocks.rangeUpgrade,
                parentName = EMU.Names.Resources.ProcessorUnit,
                sprite = Images.Upgrades.rangeUpgrade.sprite
            });

            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 6f,
                unlockName = Names.Unlocks.rangeUpgrade,
                sortPriority = 2,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.HighVoltageCable,
                        quantity = 2
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.rangeUpgrade,
                        quantity = 1
                    }
                }
            });
        }

        internal static void AddInfiniteRangeUpgrade() {
            EMUAdditions.AddNewResource(new NewResourceDetails() {
                name = Names.Items.infiniteRangeUpgrade,
                description = "Allows for unlimited range",
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                headerTitle = "Intermediates",
                subHeaderTitle = "Laser Upgrades",
                maxStackCount = 1,
                sortPriority = 3,
                unlockName = Names.Unlocks.infiniteRangeUpgrade,
                parentName = EMU.Names.Resources.ProcessorUnit,
                sprite = Images.Upgrades.infiniteRangeUpgrade.sprite
            });

            EMUAdditions.AddNewRecipe(new NewRecipeDetails() {
                GUID = LaserLogisticsPlugin.MyGUID,
                craftingMethod = CraftingMethod.Assembler,
                craftTierRequired = 0,
                duration = 60f,
                unlockName = Names.Unlocks.infiniteRangeUpgrade,
                sortPriority = 3,
                ingredients = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.rangeUpgrade,
                        quantity = 10
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.AdvancedCircuit,
                        quantity = 2
                    },
                    new RecipeResourceInfo() {
                        name = EMU.Names.Resources.MechanicalComponents,
                        quantity = 4
                    }
                },
                outputs = new List<RecipeResourceInfo>() {
                    new RecipeResourceInfo() {
                        name = Names.Items.infiniteRangeUpgrade,
                        quantity = 1
                    }
                }
            });
        }
    }
}
