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
    public class BibbleGlogBlaster : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Magic;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item9; 
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BibbleGlogBlasterProjectile>();
            Item.shootSpeed = 12f;
            Item.mana = 10; 
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.RainbowRod) // Example ingredient
                .AddIngredient(ItemID.RodofDiscord) // Example ingredient
                .AddTile(TileID.MythrilAnvil) // Crafting station
                .Register();
        }
    }
}

    