using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Neutronium.Content.Projectiles;
public class PygylyggJellyfish : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.penetrate = 3; // Hits multiple enemies
        Projectile.timeLeft = 300;
        Projectile.light = 0.75f;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.aiStyle = ProjAIStyleID.Bounce; // Makes it bounce
    }

    public override void AI()
    {
        // Glowing jellyfish effect
        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        Lighting.AddLight(Projectile.Center, new Vector3(1.5f, 0.0f, 1f));

    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        // Apply a debuff on hit
        target.AddBuff(BuffID.Venom, 120); // Poison for 2 seconds
    }
}
