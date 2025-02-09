using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Neutronium.Content.Projectiles
{
    public class BlackHole : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180; // Lasts for 3 seconds
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
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

            // Pull enemies toward the black hole (excluding bosses)
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.Distance(Projectile.Center) < 300 && !npc.boss) // Exclude bosses
                {
                    Vector2 direction = (Projectile.Center - npc.Center).SafeNormalize(Vector2.Zero);
                    npc.velocity = direction * 8f; // Pull strength
                }
            }
        }
    }
}