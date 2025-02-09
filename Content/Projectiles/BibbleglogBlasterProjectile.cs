using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles
{
    public class BibbleGlogBlasterProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 5; // Bounces multiple times
            Projectile.timeLeft = 300;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = ProjAIStyleID.Bounce; // Makes it bounce
        }

        public override void AI()
        {
            // Create rainbow trail
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Confuse enemies on hit
            target.AddBuff(BuffID.Confused, 180); // Confuse for 3 seconds
        }
    }
}