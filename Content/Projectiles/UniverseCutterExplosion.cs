using System.Numerics;
using Microsoft.Xna.Framework;
using Neutronium.Content.DamageClasses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles;

public class UniverseCutterExplosion : ModProjectile, ILocalizedModType
{
    public override string Texture => "Neutronium/Content/Projectiles/InvisibleProj";

    public override void SetDefaults()
    {
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = 5;
        Projectile.damage = 100;
    }


    public override void AI()
    {
        if (Projectile.timeLeft == 5)
        {
            for (int i = 0; i < 30; i++)
            {
                int soulDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemRuby, 0f, 0f, 100, default, 2f);
                Main.dust[soulDust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[soulDust].scale = 0.5f;
                    Main.dust[soulDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 50; j++)
            {
                int soulDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemRuby, 0f, 0f, 100, default, 3f);
                Main.dust[soulDust2].noGravity = true;
                Main.dust[soulDust2].velocity *= 5f;
                soulDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz, 0f, 0f, 100, default, 2f);
                Main.dust[soulDust2].velocity *= 2f;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire, 240);
        target.AddBuff(BuffID.Burning, 240);

    }

}