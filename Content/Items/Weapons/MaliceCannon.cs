using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Content.Projectiles;
using Terraria.Graphics.CameraModifiers;

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
            Item.useAnimation = Item.useTime = 90;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8f;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<MaliceBeam>();
            Item.shootSpeed = 1f;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = new SoundStyle("Neutronium/Content/Sounds/Items/Mcannon") with
            {
                Volume = 0.55f
            };
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-30f, 0f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(new SoundStyle("Neutronium/Content/Sounds/Items/Mcannon") with
            {
                Pitch = Main.rand.NextFloat(-0.3f, -0.6f),
                Volume = 0.55f
            }, player.Center);

            Vector2 dir = velocity.SafeNormalize(Vector2.Zero);
            float rotation = dir.ToRotation();

            // Declare muzzlePos first
            Vector2 muzzlePos = position + dir * 40f;

            float beamLength = 2400f;

            // Check tile collision
            Vector2 hit = Collision.TileCollision(muzzlePos, dir * 2400f, 1, 1);
            float tileLength = hit.Length();
            if (tileLength < beamLength)
                beamLength = tileLength;

            // Check enemy collision
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy()) continue;
                float _ = float.NaN;
                if (Collision.CheckAABBvLineCollision(npc.Hitbox.TopLeft(), npc.Hitbox.Size(),
                    muzzlePos, muzzlePos + dir * beamLength, 8f, ref _))
                {
                    float dist = Vector2.Distance(muzzlePos, npc.Center);
                    if (dist < beamLength)
                        beamLength = dist;
                }
            }

            // Spawn beam
            Projectile beam = Projectile.NewProjectileDirect(
                source,
                muzzlePos,
                Vector2.Zero,
                ModContent.ProjectileType<MaliceBeam>(),
                damage,
                knockback,
                player.whoAmI
            );
            beam.rotation = rotation;
            beam.ai[0] = beamLength;

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