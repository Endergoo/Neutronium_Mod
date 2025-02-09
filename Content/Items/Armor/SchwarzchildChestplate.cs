using Neutronium.Content.Items.Placeables;
using Neutronium.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting a X_Body.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
    public class SchwarzchildChestplate : ModItem
    {
        
        public static readonly int MaxManaIncrease = 50;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxManaIncrease);

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.LightPurple; // The rarity of the item
            Item.defense = 32; // The amount of defense the item will give when equipped
        }
       
        public override void UpdateEquip(Player player)
        {
            player.buffImmune[BuffID.OnFire] = true; // Make the player immune to Fire
            player.statManaMax2 += MaxManaIncrease; // Increase how many mana points the player can have by 20
            player.GetModPlayer<GlobalPlayer>().StellarDamage += 0.05f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe CondensedNeutron = CreateRecipe();
            CondensedNeutron.AddIngredient(ModContent.ItemType<CondensedNeutronium>(), 12);
            CondensedNeutron.AddTile(TileID.AdamantiteForge);
            CondensedNeutron.Register();
        }

    }
}