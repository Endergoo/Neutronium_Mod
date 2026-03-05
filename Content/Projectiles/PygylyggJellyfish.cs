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
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
    }

    public override void AI()
{
    Projectile.velocity.Y += 0.3f; // Gravity
    
    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
    Lighting.AddLight(Projectile.Center, new Vector3(0.0f, 0.5f, 1.5f));
}

public override bool OnTileCollide(Vector2 oldVelocity)
{
    if (Projectile.velocity.X != oldVelocity.X)
        Projectile.velocity.X = -oldVelocity.X * 0.8f; // Slight energy loss
    if (Projectile.velocity.Y != oldVelocity.Y)
        Projectile.velocity.Y = -oldVelocity.Y * 0.8f; // Slight energy loss
    return false;
}

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        // Apply a debuff on hit
        target.AddBuff(BuffID.Venom, 120); // Poison for 2 seconds
    }
}
