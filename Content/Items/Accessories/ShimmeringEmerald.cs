using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Neutronium.Content.Players;

namespace Neutronium.Content.Items.Accessories
{
    public class ShimmeringEmerald : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(7, 10));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 34;
            Item.accessory = true;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Increase mana regen
            player.GetDamage(DamageClass.Melee) += 0.10f;

            player.GetModPlayer<NeutroniumPlayer>().shimmeringEmerald = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Emerald, 3)
                .AddIngredient(ItemID.StoneBlock, 15)
                .AddIngredient(ItemID.JungleSpores, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}