using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public ref float Time => ref Projectile.ai[2];

        private const int MaxChains = 3;
        private const float ChainRange = 300f;
        private const float MaxRange = 350f;

        private Vector2 spawnPos;

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
            Projectile.timeLeft = 25;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (Time == 0)
                spawnPos = Projectile.Center;

            Projectile.rotation = Projectile.velocity.ToRotation();

            // Blue/white electric glow
            Lighting.AddLight(Projectile.Center, 0.2f, 0.5f, 1f);

            // Zigzag motion
            Projectile.velocity += Main.rand.NextVector2Circular(2f, 2f);

            // Cap speed
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

            // Home toward nearest enemy
            if (ChainCount == 0)
            {
                NPC target = FindClosestNPC(MaxRange);
                if (target != null)
                {
                    Vector2 toTarget = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget * 20f, 0.15f);
                }
            }

            Time++;
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
            Vector2 start = spawnPos - Main.screenPosition;
            Vector2 end = Projectile.Center - Main.screenPosition;
            Vector2 dir = end - start;
            float length = dir.Length();

            if (length == 0)
                return false;

            dir = Vector2.Normalize(dir);

            Vector2 current = start;
            float step = 12f;
            Color drawColor = new Color(100, 220, 255) with { A = 0 };
            Color coreColor = Color.White with { A = 0 };

            while ((current - start).Length() < length)
            {
                Vector2 next = current + dir * step + Main.rand.NextVector2Circular(8f, 8f);

                // Clamp so we don't overshoot
                if ((next - start).Length() > length)
                    next = end;

                // Outer glow
                Utils.DrawLine(Main.spriteBatch, current, next, drawColor * 0.8f, drawColor * 0.4f, 4f);
                // Bright core
                Utils.DrawLine(Main.spriteBatch, current, next, coreColor * 0.9f, coreColor * 0.5f, 1.5f);

                current = next;
            }

            // Glowing tip
            Texture2D texture = ModContent.Request<Texture2D>("Neutronium/Content/Particles/SmoothCircle").Value;
            Main.spriteBatch.Draw(texture, end, null, drawColor, 0f, texture.Size() / 2f, 0.15f, SpriteEffects.None, 0f);

            return false;
        }
    }
}