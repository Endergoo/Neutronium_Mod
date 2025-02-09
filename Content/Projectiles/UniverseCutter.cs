using Microsoft.Xna.Framework;
using Neutronium.Content.DamageClasses;
using Neutronium.Content.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles
{
    public class UniverseCutter : ModProjectile, ILocalizedModType
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 66;
            Projectile.aiStyle = ProjAIStyleID.Sickle;
            Projectile.alpha = 55;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            AIType = ProjectileID.DeathSickle;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }


        public override void AI()
        {
            // Add orange light around the projectile (RGB for orange: 2f, 1.6f, 0f)
            Lighting.AddLight(Projectile.Center, 2f, 1.6f, 0f);

            // Spawn orange dust around the projectile to simulate a glowing effect
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeTorch, 0f, 0f, 100, default, 1.2f);
            dust.noGravity = true; // Keep the dust particles suspended in the air
            dust.velocity *= 0.2f; // Slow down the dust for a subtle effect
            dust.fadeIn = 0.1f; // Gradually fade in the dust to make it more organic

            // Optionally, you can add more dust to enhance the effect by repeating the above line with slight adjustments to the position and velocity.
        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= target.lifeMax * 0.15f)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<UniverseCutterExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            //Debuffs

            target.AddBuff(BuffID.OnFire, 240);
            target.AddBuff(BuffID.Burning, 240);

        }

    }
}
