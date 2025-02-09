using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace Neutronium.Content.Projectiles
{
    public class PulsarBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 2; // How long the beam lasts
            Projectile.aiStyle = 0; // Custom AI for the beam
            Projectile.damage = 1;
            Projectile.scale = 1.1f;
            Projectile.localNPCHitCooldown = 50;


        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Check if the NPC is currently in invincibility frames (i-frames)
            if (target.immune[Projectile.owner] > 0)
            {
                return; // Do not apply the buffs if the NPC is in i-frames
            }

            // Apply the buffs only if the NPC is not in i-frames
            target.AddBuff(BuffID.Electrified, 180);
            target.AddBuff(BuffID.Frostburn, 180);
        }


        public override void AI()
        {
            // Set the beam's behavior
            Lighting.AddLight(Projectile.Center, 0f, 1f, 1f); // Glow effect for the beam
            Projectile.velocity *= 1.05f; // Make the beam travel outward
        }

        public override void Kill(int timeLeft)
        {
            // Optionally add particle effects or other visuals on beam destruction
        }
    }
}