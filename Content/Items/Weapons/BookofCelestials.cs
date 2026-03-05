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
            // Spawn the beam above the cursor
            float beamOffset = 800f;
            Vector2 spawnPos = Main.MouseWorld - new Vector2(0, beamOffset);

            if (spawnPos.Y < 0) // Clamp to top of world
                spawnPos.Y = 0;

            // Subtle random rotation (-7° to 7°)
            float beamRotation = MathHelper.ToRadians(Main.rand.NextFloat(-7f, 7f));

            Projectile.NewProjectile(
                source,
                spawnPos,
                Vector2.Zero,
                type,
                damage,
                knockback,
                player.whoAmI,
                ai0: 0.3f,      // attack speed
                ai1: beamRotation // store rotation in ai1
            );

            return false; // prevent default projectile spawn
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

        public float time = 0;
        public ref float attackSpeed => ref Projectile.ai[0];
        public ref float beamRotation => ref Projectile.ai[1]; // stored rotation

        public bool doneAttack = false;
        public int attackTime = 12;

        public float beamLength = 900;
        public float beamFX = 0;
        public float storedTime = 0;

        public Color drawColor = Color.Yellow;
        public Color explosionColor = Color.Orange;

        Vector2 beamStart => Projectile.Center;

        // Use downward direction with slight rotation
        Vector2 directionToTarget => Vector2.UnitY.RotatedBy(beamRotation);

        Vector2 beamEnd => beamStart + directionToTarget * beamLength;

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
            Projectile.timeLeft = 6000;

            Projectile.scale = 2.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (beamFX > 0)
                beamFX = MathHelper.Lerp(beamFX, 0, time > attackTime + 5 ? 0.07f : 0.01f);

            if (time == 0)
            {
                drawColor = Color.Yellow;
                explosionColor = Color.Orange;

                if (attackSpeed == 0)
                    attackSpeed = 0.3f;

                Projectile.velocity = Vector2.Zero;
                beamFX = 1f;
            }

            if (time >= attackTime && !doneAttack)
            {
                SoundStyle attack = new SoundStyle("Terraria/Sounds/Item_72") { Volume = 0.8f, Pitch = -0.2f };
                SoundEngine.PlaySound(attack, Projectile.Center);

                beamFX = 3f;
                doneAttack = true;
                storedTime = time;

                if (Main.LocalPlayer.Distance(Projectile.Center) < 2000)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 8f, 12f, 20);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                for (int i = 0; i < 30; i++)
                {
                    Vector2 dustPos = Projectile.Center + Main.rand.NextVector2Circular(100, 100);
                    Dust dust = Dust.NewDustPerfect(
                        dustPos,
                        DustID.IchorTorch,
                        Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 15),
                        0,
                        Color.Orange,
                        2f);
                    dust.noGravity = true;
                }
            }

            float endTime = storedTime + 15;
            if (time >= endTime && doneAttack)
            {
                Projectile.Kill();
                return;
            }

            time += attackSpeed;
        }

        public override bool? CanHitNPC(NPC target) => doneAttack ? (bool?)null : false;

        public override bool CanHitPlayer(Player target) => false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);

            for (int i = 0; i < 15; i++)
            {
                Vector2 dustPos = target.Center + Main.rand.NextVector2Circular(50, 50);
                Dust dust = Dust.NewDustPerfect(
                    dustPos,
                    DustID.IchorTorch,
                    Main.rand.NextVector2Unit() * Main.rand.NextFloat(3, 10),
                    0,
                    Color.Orange,
                    2f);
                dust.noGravity = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!doneAttack) return false;

            float collisionPoint = 0f;
            float beamWidth = 140f * Projectile.scale;

            // Full vertical line collision: hits any enemy along the beam regardless of cursor
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                beamStart,
                beamEnd,
                beamWidth,
                ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (beamFX == 0) return false;

            Texture2D beam = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineThick").Value;
            Texture2D bloom = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            float opacity = (doneAttack ? 0.9f : 0.5f) * (float)Math.Pow(Math.Min(beamFX, 1), 2);
            Color beamColor = drawColor with { A = 0 };

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

            Main.EntitySpriteDraw(
                beam,
                beamStart - Main.screenPosition,
                null,
                beamColor * opacity,
                directionToTarget.ToRotation() + MathHelper.PiOver2,
                new Vector2(beam.Width / 2, beam.Height),
                new Vector2(0.07f, beamLength / 1000f) * Projectile.scale,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}