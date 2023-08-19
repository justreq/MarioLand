using MarioLand.Content.Tiles;
using Microsoft.Xna.Framework;
using StructureHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace MarioLand.Common.Systems;
public class StructureGeneration : ModSystem
{
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        int index = tasks.FindIndex(genpass => genpass.Name.Equals("Quick Cleanup"));

        if (index != -1)
        {
            tasks.Insert(index + 1, new PowerUpStationsPass("Mario Land Power-Up Stations", 100f));
        }
    }

    public class PowerUpStationsPass : GenPass
    {
        public PowerUpStationsPass(string name, float loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Mario Land Power-Up Stations";

            SpawnPowerUpStations();
        }

        public void SpawnPowerUpStations()
        {
            for (int e = 0; e < 50; e++)
            {
                bool success = true;
                int attempts = 0;

                do
                {
                    attempts++;

                    if (attempts > 1000) break;

                    int x = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                    int y = 0;

                    for (int i = 50; i < (int)Main.worldSurface; i++)
                    {
                        if (Main.tile[x, i].HasTile)
                        {
                            y = i - 8;
                            break;
                        }
                    }

                    if (y == 0) success = false;

                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (Main.tile[x + i, y + j].HasTile) success = false;
                        }
                    }

                    if (success)
                    {
                        Generator.GenerateMultistructureRandom($"Assets/Structures/PowerUpStations", new(x, y), MarioLand.Instance);
                    }
                }
                while (!success);
            }
        }
    }
}
