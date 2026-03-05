using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
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

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = new Vector2(Main.MouseWorld.X, Main.MouseWorld.Y - 800);

            Projectile.NewProjectile(
                player.GetSource_ItemUse(Item),
                position,
                Vector2.Zero,
                type,
                damage,
                knockback,
                player.whoAmI,
                ai0: 0.3f,
                ai1: 1f
            );
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
        public override string Texture => "Neutronium/Content/Projectiles/InvisibleProj.png"
        public float time = 0;
        public ref float attackSpeed => ref Projectile.ai[0];
        public ref float beamType => ref Projectile.ai[1];
        public bool canDamage => doneAttack && beamFX >= 1f;
        public bool doneAttack = false;
        public int attackTime = 12;
        public float beamLength => 900;
        public float beamFX = 0;
        public float storedTime = 0;
        public Color drawColor = Color.Yellow;
        public Color explosionColor = Color.Orange;
        public float sine = 0;
        Vector2 beamStart = Vector2.Zero;
        Vector2 directionToTarget = Vector2.Zero;
        public Vector2 targetPos => new Vector2(Projectile.Center.X, Projectile.Center.Y + 800);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 6000;
            Projectile.scale = 2.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (beamFX > 0)
                beamFX = MathHelper.Lerp(beamFX, 0, time > attackTime + 5 ? 0.07f : 0.01f);

            sine = (float)Math.Sin(time * 4f / MathHelper.Pi);

            if (time == 0)
            {
                drawColor = Color.Yellow;
                explosionColor = Color.Orange;

                beamStart = targetPos - new Vector2(0, beamLength);
                directionToTarget = Vector2.UnitY;

                if (attackSpeed == 0)
                    attackSpeed = 0.3f;

                Projectile.velocity = Vector2.Zero;
                beamFX = 1f;
            }

            if (time >= attackTime && !doneAttack)
            {
                SoundStyle attack = new SoundStyle("Terraria/Sounds/Item_72") with { Volume = 0.8f, Pitch = -0.2f };
                SoundEngine.PlaySound(attack, targetPos);

                beamFX = 3f;
                doneAttack = true;
                storedTime = time;

                if (Main.LocalPlayer.Distance(targetPos) < 2000)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(targetPos, Main.rand.NextVector2Unit(), 8f, 12f, 20);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                for (int i = 0; i < 30; i++)
                {
                    Vector2 dustPos = targetPos + Main.rand.NextVector2Circular(100, 100);
                    Dust dust = Dust.NewDustPerfect(dustPos, DustID.IchorTorch, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 15), 0, Color.Orange, 2f);
                    dust.noGravity = true;

                    Dust dust2 = Dust.NewDustPerfect(dustPos, DustID.YellowTorch, Main.rand.NextVector2Unit() * Main.rand.NextFloat(3, 10), 0, Color.Yellow, 1.5f);
                    dust2.noGravity = true;
                }
            }

            float endTime = storedTime + 15;
            if (time >= endTime && doneAttack)
            {
                Projectile.Kill();
                return;
            }
            else if (doneAttack)
            {
                float progress = (time - storedTime) / (endTime - storedTime);
                drawColor = Color.Lerp(Color.Yellow, Color.Orange, progress);

                if (Main.rand.NextBool(3))
                {
                    Vector2 dustPos = targetPos + Main.rand.NextVector2Circular(150, 150);
                    Dust dust = Dust.NewDustPerfect(dustPos, DustID.IchorTorch, Main.rand.NextVector2Unit() * Main.rand.NextFloat(2, 8), 0, Color.Orange, 1.5f);
                    dust.noGravity = true;
                }
            }

            time += attackSpeed;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (canDamage)
                return null;
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

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

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!canDamage)
                return false;

            float _ = float.NaN;
            Vector2 start = beamStart;
            Vector2 end = beamStart + directionToTarget * beamLength;
            float beamWidth = 30 * Projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (beamFX == 0)
                return false;

            // Only use BloomLineThick - LineThick (bBeam) was causing black square artifacts
            Texture2D beam = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineThick").Value;
            Texture2D bloom = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            float opacity = (doneAttack ? 0.9f : 0.5f) * (float)Math.Pow(Math.Min(beamFX, 1), 2);
            Color beamColor = drawColor with { A = 0 };
            Color orangeBeam = explosionColor with { A = 0 };

            // Bloom at impact point
            float bloomScale = 0.8f * Projectile.scale * (doneAttack ? 2f : 0.6f);
            Color bloomColor = (doneAttack ? explosionColor : drawColor) * opacity * 0.8f;
            Main.EntitySpriteDraw(bloom, targetPos - Main.screenPosition, null, bloomColor, 0f, bloom.Size() / 2f, bloomScale, SpriteEffects.None, 0);

            // Draw beam - inner bright core + softer outer glow, both using BloomLineThick
            int passes = doneAttack ? 3 : 2;
            for (int t = 0; t < passes; t++)
            {
                float beamThickness = (t == 0)
                    ? 0.05f * beamFX * Utils.Remap(sine, -1, 1, 0.8f, 1.2f)           // core
                    : 0.09f * (1f - t * 0.2f) * beamFX * Utils.Remap(sine, -1, 1, 0.9f, 1.1f); // glow layers

                Color layerColor = (doneAttack && t > 0)
                    ? Color.Lerp(beamColor, orangeBeam, (float)t / passes) * opacity * (0.6f - t * 0.15f)
                    : beamColor * opacity * (1f - t * 0.25f);

                Main.EntitySpriteDraw(
                    beam,
                    beamStart - Main.screenPosition,
                    null,
                    layerColor,
                    directionToTarget.ToRotation() + MathHelper.PiOver2,
                    new Vector2(beam.Width / 2, beam.Height),
                    new Vector2(beamThickness, beamLength / 1000f) * Projectile.scale,
                    SpriteEffects.None
                );
            }

            // Sparkles at impact point when firing
            if (doneAttack)
            {
                for (int i = 0; i < 5; i++)
                {
                    float offset = sine * 15 + i * 30;
                    Color sparkleColor = Color.Lerp(Color.Yellow, Color.Orange, (float)i / 5) * opacity * 0.5f;
                    Vector2 sparklePos = targetPos + new Vector2(offset - 30, 0) + Main.rand.NextVector2Circular(20, 20);
                    Main.EntitySpriteDraw(bloom, sparklePos - Main.screenPosition, null, sparkleColor, 0f, bloom.Size() / 2f, 0.4f, SpriteEffects.None, 0);
                }
            }

            return false;
        }
    }
}