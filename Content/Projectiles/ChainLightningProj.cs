using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles
{
    public class ChainLightningProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public ref float ChainCount => ref Projectile.ai[0];
        public ref float LastHitNPC => ref Projectile.ai[1];

        private const int MaxChains = 3;
        private const float ChainRange = 300f;
        private const float MaxRange = 350f;

        // Stores the zigzag points for drawing
        private static List<Vector2> zigzagPoints = new List<Vector2>();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 25; // short range
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.extraUpdates = 2; // moves faster with more updates
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Blue/white electric glow
            Lighting.AddLight(Projectile.Center, 0.2f, 0.5f, 1f);

            // Zigzag motion — more aggressive than before
            Projectile.velocity += Main.rand.NextVector2Circular(2f, 2f);

            // Cap speed so it doesn't go too fast
            if (Projectile.velocity.Length() > 20f)
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 20f;

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
                dust.scale = Main.rand.NextFloat(0.6f, 1.2f);
                dust.color = new Color(100, 200, 255) with { A = 0 };
                dust.velocity *= 0.2f;
            }

            // If no enemy hit yet, check if one is close enough to home toward
            if (ChainCount == 0)
            {
                NPC target = FindClosestNPC(MaxRange);
                if (target != null)
                {
                    Vector2 toTarget = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget * 20f, 0.15f);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Spark burst on hit
            for (int i = 0; i < 12; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    target.Center + Main.rand.NextVector2Circular(target.width / 2f, target.height / 2f),
                    DustID.Electric,
                    Main.rand.NextVector2Circular(5f, 5f)
                );
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.8f, 1.6f);
                dust.color = new Color(100, 200, 255) with { A = 0 };
            }

            // Chain to next enemy
            if (ChainCount < MaxChains)
            {
                NPC nextTarget = FindNextTarget(target);
                if (nextTarget != null)
                {
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
                    chain.ai[0] = ChainCount + 1;
                    chain.ai[1] = target.whoAmI;
                    chain.timeLeft = 20;
                }
            }
        }

        private NPC FindClosestNPC(float maxRange)
        {
            NPC closest = null;
            float closestDist = maxRange * maxRange;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy()) continue;
                float dist = Vector2.DistanceSquared(Projectile.Center, npc.Center);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = npc;
                }
            }
            return closest;
        }

        private NPC FindNextTarget(NPC lastHit)
        {
            NPC closest = null;
            float closestDist = ChainRange * ChainRange;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy()) continue;
                if (npc.whoAmI == lastHit.whoAmI) continue;
                if (npc.whoAmI == (int)LastHitNPC) continue;
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
            Color drawColor = new Color(100, 220, 255) with { A = 0 };
            Color coreColor = Color.White with { A = 0 };

            for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                if (Projectile.oldPos[i + 1] == Vector2.Zero) continue;

                float progress = 1f - i / (float)Projectile.oldPos.Length;

                Vector2 start = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Vector2 end = Projectile.oldPos[i + 1] + Projectile.Size / 2f - Main.screenPosition;

                // Draw thick outer glow line
                Utils.DrawLine(Main.spriteBatch, start, end, drawColor * progress * 0.8f, drawColor * progress * 0.4f, 4f);

                // Draw bright white core line
                Utils.DrawLine(Main.spriteBatch, start, end, coreColor * progress, coreColor * progress * 0.5f, 1.5f);
            }

            return false;
        }
    }
}