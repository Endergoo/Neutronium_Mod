using Neutronium.Content.Items.Placeables;
using Neutronium.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
    public class SchwarzchildGreaves : ModItem
    {
        public static readonly int MaxManaIncrease = 25;

        public static readonly int MoveSpeedBonus = 5;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBonus);

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.LightPurple; // The rarity of the item
            Item.defense = 25; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += MoveSpeedBonus / 100f; // Increase the movement speed of the player
            player.statManaMax2 += MaxManaIncrease; // Increase how many mana points the player can have by 20
            player.statLifeMax2 += 25;
            player.GetModPlayer<GlobalPlayer>().StellarDamage += 0.02f;

        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe CondensedNeutron = CreateRecipe();
            CondensedNeutron.AddIngredient(ModContent.ItemType<CondensedNeutronium>(), 7);
            CondensedNeutron.AddTile(TileID.AdamantiteForge);
            CondensedNeutron.Register();
        }
    }
}