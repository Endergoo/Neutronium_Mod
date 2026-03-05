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
            // Spawn the beam at a fixed height above cursor, clamped to world top
            float beamOffset = 800f;
            float spawnY = Main.MouseWorld.Y - beamOffset;
            if (spawnY < 0) spawnY = 0;

            float beamRotation = MathHelper.ToRadians(Main.rand.NextFloat(-7f, 7f));

            Projectile.NewProjectile(
                source,
                Main.MouseWorld.X,      // X aligned to mouse
                spawnY,                 // Y is top of beam
                Vector2.Zero,
                type,
                damage,
                knockback,
                player.whoAmI,
                ai0: 0.3f,             // attack speed
                ai1: beamRotation       // rotation
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

        public float time = 0;
        public ref float attackSpeed => ref Projectile.ai[0];
        public ref float beamRotation => ref Projectile.ai[1];

        public bool doneAttack = false;
        public int attackTime = 12;

        public float beamLength = 900;
        public float beamFX = 0;
        public float storedTime = 0;

        public Color drawColor = Color.Yellow;
        public Color explosionColor = Color.Orange;

        // Beam positions in world space
        private Vector2 BeamStart;
        private Vector2 BeamEnd;
        private Vector2 Direction;

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
            Projectile.timeLeft = 60; // duration of attack
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

                if (attackSpeed == 0) attackSpeed = 0.3f;

                // Set beam start and end in world coordinates
                BeamStart = Projectile.Center;
                Direction = Vector2.UnitY.RotatedBy(beamRotation);
                BeamEnd = BeamStart + Direction * beamLength;

                Projectile.velocity = Vector2.Zero;
                beamFX = 1f;
            }

            if (time >= attackTime && !doneAttack)
            {
                SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Item_72") { Volume = 0.8f, Pitch = -0.2f }, Projectile.Center);
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

            time += attackSpeed;

            // DAMAGE: check all NPCs every tick along full beam
            if (doneAttack)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                    {
                        float collisionPoint = 0f;
                        float beamWidth = 140f * Projectile.scale;

                        if (Collision.CheckAABBvLineCollision(npc.Hitbox.TopLeft(), npc.Hitbox.Size(), BeamStart, BeamEnd, beamWidth, ref collisionPoint))
                        {
                            int damage = Projectile.damage;
                            NPC.HitInfo hitInfo = new NPC.HitInfo()
                            {
                                Damage = damage,
                                Knockback = Projectile.knockBack,
                                HitDirection = Math.Sign(npc.Center.X - Projectile.Center.X)
                            };
                            npc.StrikeNPC(hitInfo);
                            OnHitNPC(npc, hitInfo, damage);
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