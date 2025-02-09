using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles
{
    public class BlazarProjectile : ModProjectile, ILocalizedModType
    {
        public override string Texture => "Neutronium/Content/Projectiles/Fireball";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.frameCounter++ % 4 == 0)
            {
                Projectile.frame++;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.25f / 255f, (255 - Projectile.alpha) * 0.2f / 255f, (255 - Projectile.alpha) * 0.01f / 255f);
            if (Projectile.wet && !Projectile.lavaWet)
            {
                Projectile.Kill();
            }
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                Projectile.localAI[0] += 1f;
            }
            if (Main.rand.NextBool(4))
            {
                int forbid = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? 16 : 174, 0f, 0f);
                Main.dust[forbid].noGravity = true;
                Main.dust[forbid].velocity *= 0f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<BlazarBurst>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 300);
        }
    }
}