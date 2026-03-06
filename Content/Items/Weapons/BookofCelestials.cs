using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.CameraModifiers;
using Terraria.DataStructures;
using Neutronium.Content.Buffs;
using Neutronium.Content.Players;

namespace Neutronium.Content.Items.Weapons
{
    public class CelestialBeam : ModProjectile
    {
        public override string Texture => "Neutronium/Content/Projectiles/InvisibleProj";

        private float time = 0f;
        private bool doneAttack = false;
        private int attackTime = 8;
        private float beamFX = 0f;

        private Vector2 BeamStart;
        private Vector2 BeamEnd;
        private Vector2 Direction;

        private Color drawColor = Color.Yellow;

        public ref float attackSpeed => ref Projectile.ai[0];
        public ref float rotation => ref Projectile.ai[1];

        private bool canDamage => doneAttack && beamFX >= 1f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.scale = 2.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            float pulseSpeed = 0.3f;

            // Update beam color based on day/night
            if (Main.dayTime)
                drawColor = Color.Lerp(Color.Yellow, Color.Orange, (float)((Math.Sin(time * pulseSpeed) + 1) / 2));
            else
                drawColor = Color.Lerp(Color.CornflowerBlue, Color.LightBlue, (float)((Math.Sin(time * pulseSpeed) + 1) / 2));

            // --- Beam charge-up ---
            if (!doneAttack)
            {
                beamFX = MathHelper.Min(beamFX + 0.1f, 1f); // slow fade-in
            }
            // --- After attack, fade-out fast ---
            else
            {
                beamFX = MathHelper.Lerp(beamFX, 0f, 0.3f); // fast fade-out, no lingering
                if (beamFX < 0.01f) // optional: remove projectile once fully faded
                    Projectile.Kill();
            }

            if (time == 0f)
            {
                if (attackSpeed == 0f) attackSpeed = 0.3f;

                float horizontalOffset = Main.rand.NextFloat(-3f, 3f);
                Vector2 cursor = Main.MouseWorld + new Vector2(horizontalOffset, 0f);

                float verticalSpan = 3000f;
                Vector2 halfBeam = new Vector2(0f, verticalSpan).RotatedBy(rotation);

                BeamStart = cursor - halfBeam;
                BeamEnd = cursor + halfBeam;

                Direction = (BeamEnd - BeamStart).SafeNormalize(Vector2.UnitY);

                Projectile.Center = cursor;
            }

            // --- Attack trigger ---
            if (time >= attackTime && !doneAttack)
            {
                SoundEngine.PlaySound(SoundID.Item72 with { Volume = 0.8f, Pitch = -0.2f }, Projectile.Center);
                beamFX = 1f;
                doneAttack = true;

                if (Main.LocalPlayer.Distance(Projectile.Center) < 2000)
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 8f, 12f, 20));

                Color dustColor = Main.dayTime ? Color.Orange : Color.Cyan;

                for (int i = 0; i < 40; i++)
                {
                    Vector2 dustPos = Projectile.Center + Main.rand.NextVector2Circular(100, 100);
                    Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(5f, 15f);

                    Dust dust = Dust.NewDustPerfect(dustPos, DustID.GemDiamond, velocity, 0, dustColor, 2f);
                    dust.noGravity = true;
                }

                for (int i = 0; i < 20; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8f, 8f);
                    Dust flash = Dust.NewDustPerfect(Projectile.Center, DustID.WhiteTorch, velocity, 0, Color.White, 2.5f);
                    flash.noGravity = true;
                }
            }

            // --- Beam collision ---
            if (canDamage)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                    {
                        float collisionPoint = 0f;
                        float beamWidth = 100f * Projectile.scale;

                        if (Collision.CheckAABBvLineCollision(npc.Hitbox.TopLeft(), npc.Hitbox.Size(), BeamStart, BeamEnd, beamWidth, ref collisionPoint))
                        {
                            NPC.HitInfo hitInfo = new NPC.HitInfo()
                            {
                                Damage = Projectile.damage,
                                Knockback = Projectile.knockBack,
                                HitDirection = Math.Sign(npc.Center.X - Projectile.Center.X)
                            };

                            npc.StrikeNPC(hitInfo);
                            OnHitNPC(npc, hitInfo, Projectile.damage);
                        }
                    }
                }
            }

            time += attackSpeed;
        }

        public override bool? CanHitNPC(NPC target) => false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            var modPlayer = player.GetModPlayer<NeutroniumPlayer>();

            if (Main.dayTime)
            {
                int buffType = ModContent.BuffType<CelestialRegen>();
                int buffDuration = 600;

                if (!player.HasBuff(buffType))
                    player.AddBuff(buffType, buffDuration);
                else
                    player.buffTime[player.FindBuffIndex(buffType)] = buffDuration;

                modPlayer.celestialRegenStack += 0.02f;
                if (modPlayer.celestialRegenStack > 0.2f)
                    modPlayer.celestialRegenStack = 0.2f;
            }
            else
            {
                modPlayer.celestialDamageStack += 0.02f;
                if (modPlayer.celestialDamageStack > 0.2f)
                    modPlayer.celestialDamageStack = 0.2f;
            }

            Color dustColor = Main.dayTime ? Color.Orange : Color.Cyan;

            for (int i = 0; i < 15; i++)
            {
                Vector2 dustPos = target.Center + Main.rand.NextVector2Circular(50, 50);
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3, 10);

                Dust dust = Dust.NewDustPerfect(dustPos, DustID.GemDiamond, velocity, 0, dustColor, 2f);
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (beamFX == 0f) return false;

            Texture2D beam = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineThick").Value;
            float beamLength = Vector2.Distance(BeamStart, BeamEnd);
            float opacity = (doneAttack ? 0.9f : 0.5f) * (float)Math.Pow(Math.Min(beamFX, 1f), 2);
            Color beamColor = drawColor with { A = 0 };

            // outer glow
            Main.EntitySpriteDraw(
                beam,
                BeamStart - Main.screenPosition,
                null,
                beamColor * 0.35f,
                Direction.ToRotation() + MathHelper.PiOver2,
                new Vector2(beam.Width / 2, beam.Height),
                new Vector2(0.12f, beamLength / 1000f) * Projectile.scale,
                SpriteEffects.None,
                0);

            // inner core
            Main.EntitySpriteDraw(
                beam,
                BeamStart - Main.screenPosition,
                null,
                beamColor * opacity,
                Direction.ToRotation() + MathHelper.PiOver2,
                new Vector2(beam.Width / 2, beam.Height),
                new Vector2(0.06f, beamLength / 1000f) * Projectile.scale,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}