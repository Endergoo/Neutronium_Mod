using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Neutronium.Content.Players;

namespace Neutronium.Content.Items.Accessories
{
    public class ShimmeringStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 17));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.accessory = true;
            Item.value = Item.buyPrice(silver: 30);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Increase mana regen
            player.manaRegenBonus += 5;
            player.manaRegenDelayBonus += 30;

            player.GetModPlayer<NeutroniumPlayer>().shimmeringStone = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Sapphire, 3)
                .AddIngredient(ItemID.StoneBlock, 15)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}