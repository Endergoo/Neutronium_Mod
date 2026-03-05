using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;

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
            Vector2 targetPos = Main.MouseWorld;

            // Max beam length
            float maxBeamLength = 900f;

            // Spawn position above target, clamped to not be ridiculously offscreen
            Vector2 spawnPos = targetPos - Vector2.UnitY * maxBeamLength;
            if (spawnPos.Y < Main.screenPosition.Y - 50) // clamp slightly above screen top
                spawnPos.Y = Main.screenPosition.Y - 50;

            // Slight random rotation
            float beamRotation = MathHelper.ToRadians(Main.rand.NextFloat(-7f, 7f));

            Projectile.NewProjectile(
                source,
                spawnPos,
                Vector2.Zero,
                type,
                damage,
                knockback,
                player.whoAmI,
                0.3f,        // attack speed
                beamRotation  // rotation
            );

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }

    public class CelestialBeam : ModProjectile
    {
        public override string Texture => "Neutronium/Content/Projectiles/InvisibleProj";

        private float time = 0f;
        private bool doneAttack = false;
        private int attackTime = 12;
        private float beamLength = 900f;
        private float beamFX = 0f;
        private float storedTime = 0f;

        private Color drawColor = Color.Yellow;

        private Vector2 BeamStart;
        private Vector2 BeamEnd;
        private Vector2 Direction;

        public ref float attackSpeed => ref Projectile.ai[0];
        public ref float beamRotation => ref Projectile.ai[1];

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
            Projectile.timeLeft = 60;
            Projectile.scale = 2.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (beamFX > 0)
                beamFX = MathHelper.Lerp(beamFX, 0f, time > attackTime + 5 ? 0.07f : 0.01f);

            // Initialize beam
            if (time == 0f)
            {
                if (attackSpeed == 0f) attackSpeed = 0.3f;

                // Beam starts at spawn position, direction toward targetPos
                Vector2 targetPos = Main.MouseWorld;
                BeamStart = Projectile.Center;
                Direction = Vector2.UnitY.RotatedBy(beamRotation); // slight rotation
                BeamEnd = BeamStart + Direction * beamLength;

                Projectile.velocity = Vector2.Zero;
                beamFX = 1f;
            }

            if (time >= attackTime && !doneAttack)
            {
                SoundEngine.PlaySound(SoundID.Item72 with { Volume = 0.8f, Pitch = -0.2f }, Projectile.Center);
                beamFX = 3f;
                doneAttack = true;
                storedTime = time;

                if (Main.LocalPlayer.Distance(Projectile.Center) < 2000)
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 8f, 12f, 20));

                // Dust FX
                for (int i = 0; i < 30; i++)
                {
                    Vector2 dustPos = Projectile.Center + Main.rand.NextVector2Circular(100, 100);
                    Dust dust = Dust.NewDustPerfect(dustPos, DustID.IchorTorch, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 15), 0, Color.Orange, 2f);
                    dust.noGravity = true;
                }
            }

            time += attackSpeed;

            // Full-length damage
            if (canDamage)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                    {
                        float collisionPoint = 0f;
                        float beamWidth = 140f * Projectile.scale;
                        Vector2 end = BeamStart + Direction * beamLength * 2;

                        if (Collision.CheckAABBvLineCollision(npc.Hitbox.TopLeft(), npc.Hitbox.Size(), BeamStart, end, beamWidth, ref collisionPoint))
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
            Texture2D bloom = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            float opacity = (doneAttack ? 0.9f : 0.5f) * (float)Math.Pow(Math.Min(beamFX, 1f), 2);
            Color beamColor = drawColor with { A = 0 };

            // Draw bloom
            Main.EntitySpriteDraw(
                bloom,
                Projectile.Center - Main.screenPosition,
                null,
                beamColor * opacity,
                0f,
                bloom.Size() / 2f,
                1.5f,
                SpriteEffects.None,
                0);

            // Draw beam line
            Main.EntitySpriteDraw(
                beam,
                BeamStart - Main.screenPosition,
                null,
                beamColor * opacity,
                Direction.ToRotation() + MathHelper.PiOver2,
                new Vector2(beam.Width / 2, beam.Height),
                new Vector2(0.07f, beamLength / 1000f) * Projectile.scale,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}