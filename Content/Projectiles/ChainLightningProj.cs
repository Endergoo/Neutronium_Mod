using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Projectiles
{
    public class ChainLightningProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        private HashSet<NPC> shockedBefore = new HashSet<NPC>();
        private int prevX = 0;

        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 25;
            Projectile.penetrate = 4; // hits up to 4 enemies
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            // Normalize velocity on first frame
            if (Projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref Projectile.velocity);
                Projectile.localAI[0] = 1f;
            }

            Vector2 move = Vector2.Zero;
            float distance = 200f; // chain range
            bool target = false;
            NPC npc = null;
            bool pastNPC = false;

            // Look for new NPCs to chain to
            if (Projectile.timeLeft < 23)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC n = Main.npc[k];
                    if (n.active && !n.dontTakeDamage && !n.friendly && n.lifeMax > 5 && !shockedBefore.Contains(n))
                    {
                        Vector2 newMove = n.Center - (Projectile.velocity + Projectile.Center);
                        float distanceTo = newMove.Length();
                        if (distanceTo < distance)
                        {
                            move = newMove;
                            distance = distanceTo;
                            target = true;
                            npc = n;
                        }
                    }
                }
            }

            // If no new target found, look through already shocked NPCs to bounce away
            if (!target)
            {
                foreach (NPC pastNpc in shockedBefore)
                {
                    Vector2 newMove = pastNpc.Center - (Projectile.velocity + Projectile.Center);
                    float distanceTo = newMove.Length();
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                        npc = pastNpc;
                        pastNPC = true;
                    }
                }
            }

            // Draw the lightning arc using dust particles
            Vector2 current = Projectile.Center;
            if (target)
            {
                shockedBefore.Add(npc);
                move += new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)) * distance / 30;
                if (pastNPC)
                {
                    prevX++;
                    move += new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)) * prevX;
                }
            }
            else
            {
                move = (Projectile.velocity + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6))) * 5;
            }

            // Place dust particles along the arc to create the lightning visual
            for (int i = 0; i < 20; i++)
            {
                // Outer glow dust
                Dust glowDust = Dust.NewDustDirect(current, Projectile.width, Projectile.height, DustID.Electric);
                glowDust.velocity = Vector2.Zero;
                glowDust.noGravity = true;
                glowDust.scale = Main.rand.NextFloat(0.8f, 1.4f);
                glowDust.color = new Color(100, 200, 255) with { A = 0 };

                // Bright white core dust every other particle
                if (i % 2 == 0)
                {
                    Dust coreDust = Dust.NewDustDirect(current, Projectile.width, Projectile.height, DustID.Electric);
                    coreDust.velocity = Vector2.Zero;
                    coreDust.noGravity = true;
                    coreDust.scale = Main.rand.NextFloat(0.4f, 0.8f);
                    coreDust.color = Color.White with { A = 0 };
                }

                // Add light along the arc
                Lighting.AddLight(current, 0.1f, 0.3f, 0.8f);

                current += move / 20f;
            }

            Projectile.position = current;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            Projectile.timeLeft -= 12;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Spark burst on hit
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    target.Center + Main.rand.NextVector2Circular(target.width / 2f, target.height / 2f),
                    DustID.Electric,
                    Main.rand.NextVector2Circular(6f, 6f)
                );
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.8f, 1.6f);
                dust.color = new Color(100, 200, 255) with { A = 0 };
            }
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = vector.Length();
            if (magnitude > 6f)
                vector *= 6f / magnitude;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}