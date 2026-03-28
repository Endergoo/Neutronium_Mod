using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles
{
    public class CorruptorFlame : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CursedFlameHostile;
        
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.scale = 0.5f; // smaller than vanilla
        }

        public override void AI()
        {
            // Green cursed flame light
            Lighting.AddLight(Projectile.Center, 0f, 0.4f, 0f);

            // Rotate sprite
            Projectile.rotation += 0.3f;

            // Home toward nearest enemy
            NPC target = FindClosestNPC(400f);
            if (target != null)
            {
                Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 10f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.08f);
            }

            // Cursed flame dust trail
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.CursedTorch
                );
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.3f, 0.6f);
                dust.velocity *= 0.2f;
            }
        }

        private NPC FindClosestNPC(float maxRange)
        {
            NPC closest = null;
            float closestDist = maxRange * maxRange;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy()) continue;
                float dist = Vector2.DistanceSquared(Projectile.Center, npc.Center);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = npc;
                }
            }
            return closest;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
        }
    }
}