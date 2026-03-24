using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Content.Projectiles;
using Neutronium.Core.GlobalItems;

namespace Neutronium.Content.Items.Weapons
{
    public class SpitefulPrick : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 28;
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.Stinger;
            Item.shootSpeed = 14f;
            Item.noMelee = true;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item11;
            Item.shoot = ModContent.ProjectileType<StingerProjectile>();
            Item.useAmmo = ItemID.Stinger;
        }

         public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Stinger, 10) 
                .AddIngredient(ItemID.JungleSpores, 12) 
                .AddTile(TileID.Anvils) 
                .Register();
        }
    }
}