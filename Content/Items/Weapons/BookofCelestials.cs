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

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] == 0;
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
        public override string Texture => "Neutronium/Content/Projectiles/InvisibleProj";

        public float time = 0;
        public ref float attackSpeed => ref Projectile.ai[0];
        public ref float beamType => ref Projectile.ai[1];

        public bool doneAttack = false;
        public int attackTime = 12;

        public float beamLength = 900;
        public float beamFX = 0;
        public float storedTime = 0;

        public Color drawColor = Color.Yellow;
        public Color explosionColor = Color.Orange;

        public float sine = 0;

        Vector2 beamStart = Vector2.Zero;
        Vector2 directionToTarget = Vector2.UnitY;
        float beamRotation;

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

            sine = (float)Math.Sin(time * 4f / MathHelper.Pi);

            if (time == 0)
            {
                drawColor = Color.Yellow;
                explosionColor = Color.Orange;

                beamRotation = MathHelper.ToRadians(Main.rand.NextFloat(-25f, 25f));
                directionToTarget = Vector2.UnitY.RotatedBy(beamRotation);

                // Projectile spawns at the top of the beam, so beamStart = its position
                beamStart = Projectile.Center;

                if (attackSpeed == 0)
                    attackSpeed = 0.3f;

                Projectile.velocity = Vector2.Zero;
                beamFX = 1f;
            }

            if (time >= attackTime && !doneAttack)
            {
                SoundStyle attack = new SoundStyle("Terraria/Sounds/Item_72") with { Volume = 0.8f, Pitch = -0.2f };
                SoundEngine.PlaySound(attack, Projectile.Center);

                beamFX = 3f;
                doneAttack = true;
                storedTime = time;

                Vector2 impactPos = beamStart + directionToTarget * beamLength;

                if (Main.LocalPlayer.Distance(impactPos) < 2000)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(impactPos, Main.rand.NextVector2Unit(), 8f, 12f, 20);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                for (int i = 0; i < 30; i++)
                {
                    Vector2 dustPos = impactPos + Main.rand.NextVector2Circular(100, 100);
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

            time += attackSpeed;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (doneAttack)
                return null;
            return false;
        }

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

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!doneAttack)
                return false;

            float collisionPoint = 0f;
            Vector2 start = beamStart;
            Vector2 end = beamStart + directionToTarget * beamLength;
            float beamWidth = 140f * Projectile.scale;

            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                start,
                end,
                beamWidth,
                ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (beamFX == 0)
                return false;

            Texture2D beam = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineThick").Value;
            Texture2D bloom = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            float opacity = (doneAttack ? 0.9f : 0.5f) * (float)Math.Pow(Math.Min(beamFX, 1), 2);
            Color beamColor = drawColor with { A = 0 };
            Vector2 impactPos = beamStart + directionToTarget * beamLength;

            // Bloom at impact point
            Main.EntitySpriteDraw(
                bloom,
                impactPos - Main.screenPosition,
                null,
                beamColor * opacity,
                0f,
                bloom.Size() / 2f,
                1.5f * Projectile.scale * (doneAttack ? 1.5f : 0.5f),
                SpriteEffects.None,
                0);

            // Beam drawn from beamStart downward
            // Origin at top (Y=0) so texture extends from beamStart toward impactPos
            Main.EntitySpriteDraw(
                beam,
                beamStart - Main.screenPosition,
                null,
                beamColor * opacity,
                directionToTarget.ToRotation() + MathHelper.PiOver2,
                new Vector2(beam.Width / 2f, 0f),
                new Vector2(0.07f, beamLength / beam.Height) * Projectile.scale,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}