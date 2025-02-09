using Microsoft.Xna.Framework;
using Neutronium.Content.Items.Placeables;
using Neutronium.Content.Projectiles;
using Neutronium.Content.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Weapons
{
    public class PygylyggStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item8; // Magical sound
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PygylyggJellyfish>();
            Item.shootSpeed = 10f;
            Item.mana = 12; // Mana cost
            Item.scale = 2f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.JellyfishNecklace, 1) // Example ingredient
                .AddIngredient(ItemID.Pearlwood, 20) // Example ingredient
                .AddIngredient(ItemID.SpiderFang, 10)
                .AddTile(TileID.Anvils) // Crafting station
                .Register();
        }
    }   
}