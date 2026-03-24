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
    public class BibbleglogBlaster : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Magic;
            Item.width = 38;
            Item.height = 30;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item9; 
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BibbleglogBlasterProjectile>();
            Item.shootSpeed = 12f;
            Item.mana = 10; 
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.RainbowRod)
                .AddIngredient(ItemID.RodofDiscord)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

    