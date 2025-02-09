using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Neutronium.Content.Projectiles
{
    public class MoonlitStilettoProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16; // Width of the projectile's hitbox
            Projectile.height = 16; // Height of the projectile's hitbox
            Projectile.aiStyle = -1; // Use custom AI
            Projectile.friendly = true; // Does not damage players
            Projectile.hostile = false; // Does not damage enemies
            Projectile.DamageType = DamageClass.Melee; // Damage type is melee
            Projectile.penetrate = 3; // How many enemies the projectile can hit
            Projectile.timeLeft = 300; // Lifespan of the projectile in frames (5 seconds at 60 FPS)
            Projectile.light = 0.7f; // Light emission
            Projectile.ignoreWater = false; // Affected by water
            Projectile.tileCollide = true; // Collides with tiles
            Projectile.extraUpdates = 1; // Extra updates per frame
        }

        public override void AI()
        {
            // Apply gravity
            Projectile.velocity.Y += 0.05f; // Gravity strength (adjust as needed)

            // Rotate the projectile based on its velocity
            Projectile.rotation += Projectile.velocity.X * 0.1f;

            // Simulate rolling when on the ground
            if (Projectile.velocity.Y != 0) // Falling
            {
                Projectile.velocity.X *= 0.99f; // Slow down horizontally while falling
            }
            else // On the ground
            {
                // Roll along the ground
                Projectile.velocity.X *= 0.98f; // Slow down over time
            }

            // Add some dust for visual effect
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }

        public void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Example: Apply a debuff to the target
            target.AddBuff(BuffID.Frostburn, 180); // Apply Frostburn for 3 seconds
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Bounce off tiles
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X * 0.8f; // Reverse X velocity and reduce it
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f; // Reverse Y velocity and reduce it
            }

            // Play a sound when bouncing
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

            return false; // Prevent the projectile from dying on tile collision
        }

        public override void Kill(int timeLeft)
        {
            // Example: Spawn dust when the projectile dies
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }
    }
}