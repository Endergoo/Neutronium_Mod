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
            Projectile.aiStyle = -1; // Custom AI
        }


        private float rotationAngle = 0f; // Track the rotation over time
        public override void AI()
        {
            // Increment the rotation angle over time
            rotationAngle += 0.05f; // Adjust the speed of rotation

            // Create swirling dust effect
            for (int i = 0; i < 10; i++)
            {
                // Calculate the angle for swirling effect, including the rotation over time
                float angle = MathHelper.TwoPi * i / 10 + rotationAngle; // Divide the circle into 10 parts and add rotation
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 20f; // Adjust the radius as needed

                // Set the dust position relative to the projectile
                Vector2 dustPosition = Projectile.Center + offset;

                // Create or update the dust particle
                Dust dust = Dust.NewDustPerfect(dustPosition, DustID.OrangeTorch, Vector2.Zero, 100, default, 2f);
                dust.noGravity = true;

                // Set the velocity to create a circular motion
                dust.velocity = offset.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * 5f; // Rotate by 90 degrees for perpendicular motion
            }

            // Slow down the projectile
            Projectile.velocity *= 0.99f;

            // Create the black hole on impact
            if (Projectile.timeLeft <= 280) // Delay before black hole forms
            {
                Projectile.Kill(); // Destroy the projectile
            }
        }
        public override void OnKill(int timeLeft)
        {
            // Optionally, you can still spawn a black hole if the projectile dies naturally (e.g., after time runs out)
            if (Projectile.owner == Main.myPlayer && timeLeft > 120) // Only spawn if the projectile didn't already hit an enemy
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

            // Kill the projectile after hitting an enemy
            Projectile.Kill();
        }

    }
}