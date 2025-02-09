using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Weapons
{
    public class CurrentCaller : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 42;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 30;
            Item.knockBack = 6;
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ProjectileID.FrostBoltStaff; 
            Item.damage = 15;
            Item.shootSpeed = 15f;           
        }
        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 target = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
            float ceilingLimit = target.Y;
            if (ceilingLimit > player.Center.Y - 200f)
            {
                ceilingLimit = player.Center.Y - 200f;
            }
            // Loop func 3 times.
            for (int i = 0; i < 3; i++)
            {
                position = player.Center - new Vector2(Main.rand.NextFloat(401) * player.direction, 600f);
                position.Y -= 100 * i;
                Vector2 heading = target - position;

                if (heading.Y < 0f)
                {
                    heading.Y *= -1f;
                }

                if (heading.Y < 20f)
                {
                    heading.Y = 20f;
                }

                heading.Normalize();
                heading *= velocity.Length();
                heading.Y += Main.rand.Next(-40, 41) * 0.02f;
                Projectile.NewProjectile(source, position, heading, type, damage * 2, knockback, player.whoAmI, 0f, ceilingLimit);
            }

            return false;
        }

        //Recipe
        public override void AddRecipes()
        {
            Recipe CurrentCaller = CreateRecipe();
            CurrentCaller.AddIngredient(ItemID.WaterCandle, 1);
            CurrentCaller.AddIngredient(ItemID.SandBlock, 20);
            CurrentCaller.AddIngredient(ItemID.PearlstoneBlock, 15);
            CurrentCaller.AddTile(TileID.CrystalBall);
            CurrentCaller.Register();
        }
    }
}