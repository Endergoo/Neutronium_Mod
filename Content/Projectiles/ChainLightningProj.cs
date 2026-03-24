using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles
{
    public class ChainLightningProj : ModProjectile
    {
       public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CultistBossLightningOrbArc;

        // ai[0] = how many times it has chained
        // ai[1] = last NPC it hit (to avoid chaining back)
        public ref float ChainCount => ref Projectile.ai[0];
        public ref float LastHitNPC => ref Projectile.ai[1];

        private const int MaxChains = 3;
        private const float ChainRange = 250f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 30; // short range — dies quickly
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Blue/white electric glow
            Lighting.AddLight(Projectile.Center, 0.2f, 0.4f, 1f);

            // Electric spark dust trail
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Electric
                );
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.5f, 1f);
                dust.color = new Color(150, 200, 255) with { A = 0 };
                dust.velocity *= 0.3f;
            }

            // Slight zigzag motion
            if (Main.rand.NextBool(2))
                Projectile.velocity += Main.rand.NextVector2Circular(1.5f, 1.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Spark burst on hit
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    target.position,
                    target.width,
                    target.height,
                    DustID.Electric
                );
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.8f, 1.5f);
                dust.color = new Color(150, 200, 255) with { A = 0 };
                dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
            }

            // Chain to next enemy if we haven't hit max chains
            if (ChainCount < MaxChains)
            {
                NPC nextTarget = FindNextTarget(target);
                if (nextTarget != null)
                {
                    // Spawn a new chain lightning projectile toward the next target
                    Vector2 direction = (nextTarget.Center - target.Center).SafeNormalize(Vector2.Zero);
                    Projectile chain = Projectile.NewProjectileDirect(
                        Projectile.GetSource_FromThis(),
                        target.Center,
                        direction * 18f,
                        Type,
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner
                    );
                    chain.ai[0] = ChainCount + 1; // increment chain count
                    chain.ai[1] = target.whoAmI;  // remember last hit NPC
                }
            }
        }

        private NPC FindNextTarget(NPC lastHit)
        {
            NPC closest = null;
            float closestDist = ChainRange * ChainRange;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy()) continue;
                if (npc.whoAmI == lastHit.whoAmI) continue; // don't chain back
                if (npc.whoAmI == (int)LastHitNPC) continue; // don't chain to previous

                float dist = Vector2.DistanceSquared(lastHit.Center, npc.Center);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = npc;
                }
            }

            return closest;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return true;
        }
    }
}