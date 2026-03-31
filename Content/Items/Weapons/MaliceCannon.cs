using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Content.Projectiles;

namespace Neutronium.Content.Items.Weapons
{
    public class MaliceCannon : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 152;
            Item.height = 42;
            Item.damage = 180;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8f;
            Item.scale = 0.75f;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<MaliceBeam>();
            Item.shootSpeed = 1f;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item122;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10f, 0f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = velocity.SafeNormalize(Vector2.Zero);
            float rotation = dir.ToRotation();

            // Raycast to find beam length — stop at first tile or enemy
            float beamLength = 2400f;

            // Check tile collision
            Vector2 hit = Collision.TileCollision(position, dir * 2400f, 1, 1);
            float tileLength = hit.Length();
            if (tileLength < beamLength)
                beamLength = tileLength;

            // Check enemy collision
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy()) continue;
                float _ = float.NaN;
                if (Collision.CheckAABBvLineCollision(npc.Hitbox.TopLeft(), npc.Hitbox.Size(),
                    position, position + dir * beamLength, 8f, ref _))
                {
                    float dist = Vector2.Distance(position, npc.Center);
                    if (dist < beamLength)
                        beamLength = dist;
                }
            }

            // Spawn the beam projectile
            Projectile beam = Projectile.NewProjectileDirect(
                source,
                position,
                Vector2.Zero,
                ModContent.ProjectileType<MaliceBeam>(),
                damage,
                knockback,
                player.whoAmI
            );
            beam.rotation = rotation;
            beam.ai[0] = beamLength;

            // Muzzle flash
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(position, 1, 1, DustID.RedTorch);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(1f, 2f);
                dust.velocity = dir.RotatedByRandom(0.5f) * Main.rand.NextFloat(4f, 12f);
                dust.color = new Color(255, 50, 50) with { A = 0 };
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 15)
                .AddIngredient(ItemID.SoulofFright, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}