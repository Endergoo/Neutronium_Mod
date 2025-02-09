using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria;
using Neutronium.Content.Tiles;

public class NeutroniumOreSystem : ModSystem
{
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        int shiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

        if (shiniesIndex != -1)
        {
            tasks.Insert(shiniesIndex + 1, new PassLegacy("Neutronium Ore", GenerateNeutroniumOre));
        }
    }

    private void GenerateNeutroniumOre(GenerationProgress progress, GameConfiguration configuration)
    {
        if (!NeutroniumWorldSystem.MoonLordDefeated)
        {
            return;
        }

        progress.Message = "Generating Neutronium Ore...";
        GenerateOre();
    }

    public static void GenerateOre()
    {
        for (int k = 0; k < Main.maxTilesX * Main.maxTilesY * 0.0006; k++)
        {
            int x = WorldGen.genRand.Next(0, Main.maxTilesX);
            int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);

            Tile tile = Main.tile[x, y];
            if (tile.HasTile && (tile.TileType == TileID.Stone || tile.TileType == TileID.Dirt))
            {
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 5), ModContent.TileType<NeutroniumOre>());
            }
        }
    }
}