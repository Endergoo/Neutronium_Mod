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
            float beamRotation = Main.rand.NextFloat(-0.05f, 0.05f);

            Projectile.NewProjectile(
                source,
                player.Center,
                Vector2.Zero,
                type,
                damage,
                knockback,
                player.whoAmI,
                0.3f,       // attack speed
                beamRotation
            );

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.CelestialStone, 1);
            recipe.AddIngredient(ItemID.SpellTome, 1);
            recipe.AddIngredient(ItemID.SoulofLight, 10);
            recipe.AddIngredient(ItemID.SoulofNight, 10);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }

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
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            float pulseSpeed = 0.3f;

            // Beam colors (day = yellow/orange, night = cyan/lightblue)
            if (Main.dayTime)
                drawColor = Color.Lerp(Color.Yellow, Color.Orange, (float)((Math.Sin(time * pulseSpeed) + 1) / 2));
            else
                drawColor = Color.Lerp(Color.Cyan, Color.LightBlue, (float)((Math.Sin(time * pulseSpeed) + 1) / 2));

            // Fade in/out
            if (!doneAttack)
                beamFX = MathHelper.Min(beamFX + 0.1f, 1f); // fade in
            else
            {
                beamFX = MathHelper.Lerp(beamFX, 0f, 0.15f); // fade out
                if (beamFX < 0.02f)
                    Projectile.Kill();
            }

            if (time == 0f)
            {
                if (attackSpeed == 0f) attackSpeed = 0.3f;

                Vector2 cursor = Main.MouseWorld + new Vector2(Main.rand.NextFloat(-3f, 3f), 0f);
                float verticalSpan = 3000f;
                Vector2 halfBeam = new Vector2(0f, verticalSpan).RotatedBy(rotation);

                BeamStart = cursor - halfBeam;
                BeamEnd = cursor + halfBeam;
                Direction = (BeamEnd - BeamStart).SafeNormalize(Vector2.UnitY);
                Projectile.Center = cursor;

                // Lighting along beam
                Vector2 beamVector = BeamEnd - BeamStart;
                float beamLength = beamVector.Length();
                Vector2 beamDir = beamVector.SafeNormalize(Vector2.UnitY);

                for (float i = 0; i <= beamLength; i += 60f)
                {
                    Vector2 lightPos = BeamStart + beamDir * i;
                    float brightness = 1f - (i / beamLength) * 0.5f;

                    if (Main.dayTime)
                        Lighting.AddLight(lightPos, 0.9f * brightness, 0.85f * brightness, 0.4f * brightness);
                    else
                        Lighting.AddLight(lightPos, 0.3f * brightness, 0.45f * brightness, 0.9f * brightness);
                }
            }

            // Trigger attack
            if (time >= attackTime && !doneAttack)
            {
                SoundEngine.PlaySound(SoundID.Item72 with { Volume = 0.8f, Pitch = -0.2f }, Projectile.Center);
                beamFX = 1.5f;
                doneAttack = true;

                if (Main.LocalPlayer.Distance(Projectile.Center) < 2000f)
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 8f, 12f, 20));

                Color dustColor = Main.dayTime ? Color.Orange : Color.Cyan;
                for (int i = 0; i < 30; i++)
                {
                    Vector2 dustPos = Projectile.Center + Main.rand.NextVector2Circular(100, 100);
                    Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(5f, 15f);
                    Dust dust = Dust.NewDustPerfect(dustPos, DustID.GemDiamond, velocity, 0, dustColor, 2f);
                    dust.noGravity = true;
                }
            }

            // Beam collision
            if (canDamage)
            {
                float beamWidth = 100f * Projectile.scale;
                foreach (NPC npc in Main.npc)
                {
                    if (!npc.active || npc.friendly || !npc.CanBeChasedBy())
                        continue;

                    float collisionPoint = 0f;
                    if (Collision.CheckAABBvLineCollision(npc.Hitbox.TopLeft(), npc.Hitbox.Size(), BeamStart, BeamEnd, beamWidth, ref collisionPoint))
                    {
                        int direction = Math.Sign(npc.Center.X - Projectile.Center.X);
                        npc.StrikeNPC(Projectile.damage, Projectile.knockBack, direction);

                        // Lifesteal during day
                        Player player = Main.player[Projectile.owner];
                        if (Main.dayTime)
                        {
                            int heal = Projectile.damage / 6;
                            if (heal > 0)
                            {
                                player.statLife += heal;
                                if (player.statLife > player.statLifeMax2)
                                    player.statLife = player.statLifeMax2;
                                player.HealEffect(heal);
                            }
                        }

                        // Dust effect
                        Color dustColor = Main.dayTime ? Color.Orange : Color.Cyan;
                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 dustPos = npc.Center + Main.rand.NextVector2Circular(50, 50);
                            Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 10f);
                            Dust dust = Dust.NewDustPerfect(dustPos, DustID.GemDiamond, velocity, 0, dustColor, 2f);
                            dust.noGravity = true;
                        }
                    }
                }
            }

            time += attackSpeed;
        }

        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            if (beamFX <= 0f) return false;

            Texture2D beam = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineThick").Value;
            float beamLength = Vector2.Distance(BeamStart, BeamEnd);
            float opacity = (doneAttack ? 0.9f : 0.5f) * (float)Math.Pow(Math.Min(beamFX, 1f), 2);
            Color beamColor = drawColor with { A = 0 };

            // Outer glow
            Main.EntitySpriteDraw(
                beam,
                BeamStart - Main.screenPosition,
                null,
                beamColor * 0.35f * opacity,
                Direction.ToRotation() + MathHelper.PiOver2,
                new Vector2(beam.Width / 2, beam.Height),
                new Vector2(0.12f, beamLength / 1000f) * Projectile.scale,
                SpriteEffects.None,
                0);

            // Inner core
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