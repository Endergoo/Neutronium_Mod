using System;
using Microsoft.Xna.Framework;
using Neutronium.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles
{
    public class BHLProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1; // Infinite penetration
            Projectile.timeLeft = 480;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
        }


        private float rotationAngle = 0f;
        public override void AI()
        {
            rotationAngle += 0.05f; // speed

            // swirl
            for (int i = 0; i < 10; i++)
            {
                // Calculate the angle for swirling effect, including the rotation over time
                float angle = MathHelper.TwoPi * i / 10 + rotationAngle; 
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 20f; // radius

                Vector2 dustPosition = Projectile.Center + offset;
              
                Dust dust = Dust.NewDustPerfect(dustPosition, DustID.OrangeTorch, Vector2.Zero, 100, default, 2f);
                dust.noGravity = true;

                dust.velocity = offset.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * 5f;
            }

            // Slow down the projectile
            Projectile.velocity *= 0.99f;

            // Create the black hole on impact
            if (Projectile.timeLeft <= 280) 
            {
                Projectile.Kill();
            }
        }
        public override void OnKill(int timeLeft)
        {
           
            if (Projectile.owner == Main.myPlayer && timeLeft > 120) 
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BlackHole>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Spawn the black hole when the projectile hits an enemy
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BlackHole>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

        
            Projectile.Kill();
        }

    }
}