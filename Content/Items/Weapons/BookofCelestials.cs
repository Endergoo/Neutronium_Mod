using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.CameraModifiers;
using Terraria.DataStructures;

namespace Neutronium.Content.Items.Weapons
{
    public class BookofCelestials : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CelestialBeam>();
            Item.shootSpeed = 0f;
            Item.scale = 0.25f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Slight random rotation in radians
            float beamRotation = Main.rand.NextFloat(-0.05f, 0.05f); // ±~3 degrees

            Projectile.NewProjectile(
                source,
                player.Center,
                Vector2.Zero,
                type,
                damage,
                knockback,
                player.whoAmI,
                0.3f,        // attack speed
                beamRotation  // rotation for slight tilt
            );

            return false;
        }
    }

    public class CelestialBeam : ModProjectile
    {
        public override string Texture => "Neutronium/Content/Projectiles/InvisibleProj";

        private float time = 0f;
        private bool doneAttack = false;
        private int attackTime = 12;
        private float beamFX = 0f;
        private float storedTime = 0f;

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
            if (beamFX > 0f)
                beamFX = MathHelper.Lerp(beamFX, 0f, time > attackTime + 5 ? 0.07f : 0.01f);

            if (time == 0f)
            {
                if (attackSpeed == 0f) attackSpeed = 0.3f;

                // Optional subtle horizontal variance (±3px)
                float horizontalOffset = Main.rand.NextFloat(-3f, 3f);
                Vector2 cursor = Main.MouseWorld + new Vector2(horizontalOffset, 0f);

                // Beam is centered on cursor
                float verticalSpan = 2000f; // adjust height as needed
                Vector2 halfBeam = new Vector2(0f, verticalSpan).RotatedBy(rotation);

                BeamStart = cursor - halfBeam;
                BeamEnd = cursor + halfBeam;

                Direction = (BeamEnd - BeamStart).SafeNormalize(Vector2.UnitY);

                Projectile.Center = cursor;
                beamFX = 1f;
            }

            // Attack trigger
            if (time >= attackTime && !doneAttack)
            {
                SoundEngine.PlaySound(SoundID.Item72 with { Volume = 0.8f, Pitch = -0.2f }, Projectile.Center);
                beamFX = 3f;
                doneAttack = true;
                storedTime = time;

                if (Main.LocalPlayer.Distance(Projectile.Center) < 2000)
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 8f, 12f, 20));

                for (int i = 0; i < 30; i++)
                {
                    Vector2 dustPos = Projectile.Center + Main.rand.NextVector2Circular(100, 100);
                    Dust dust = Dust.NewDustPerfect(dustPos, DustID.IchorTorch, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 15), 0, Color.Orange, 2f);
                    dust.noGravity = true;
                }
            }

            // Full-length collision along beam
            if (canDamage)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                    {
                        float collisionPoint = 0f;
                        float beamWidth = 140f * Projectile.scale;

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
        public override bool CanHitPlayer(Player target) => false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
            for (int i = 0; i < 15; i++)
            {
                Vector2 dustPos = target.Center + Main.rand.NextVector2Circular(50, 50);
                Dust dust = Dust.NewDustPerfect(dustPos, DustID.IchorTorch, Main.rand.NextVector2Unit() * Main.rand.NextFloat(3, 10), 0, Color.Orange, 2f);
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (beamFX == 0f) return false;

            Texture2D beam = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineThick").Value;

            float opacity = (doneAttack ? 0.9f : 0.5f) * (float)Math.Pow(Math.Min(beamFX, 1f), 2);
            Color beamColor = drawColor with { A = 0 };

            // Draw full-length beam
            Main.EntitySpriteDraw(
                beam,
                BeamStart - Main.screenPosition,
                null,
                beamColor * opacity,
                Direction.ToRotation() + MathHelper.PiOver2,
                new Vector2(beam.Width / 2, beam.Height),
                new Vector2(0.07f, Vector2.Distance(BeamStart, BeamEnd) / 1000f) * Projectile.scale,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}