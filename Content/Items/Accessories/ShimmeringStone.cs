using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Content.Players;

namespace Neutronium.Content.Items.Accessories
{
    public class ShimmeringStone : ModItem
    {
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

            // Enable frostburn on magic hits
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