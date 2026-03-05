using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Items.Weapons
{
    public class BookOfCelestials : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CelestialBeam>();
            Item.shootSpeed = 0f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
            
            Projectile.NewProjectile(
                player.GetSource_ItemAI(Item),
                position,
                Vector2.Zero,
                type,
                damage,
                knockback,
                player.whoAmI,
                ai0: 0.4f,
                ai1: (float)(Main.time % 4)
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

    // Put the CelestialBeam class HERE, inside the same namespace
    public class CelestialBeam : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public float time = 0;
        public ref float attackSpeed => ref Projectile.ai[0];
        public ref float beamType => ref Projectile.ai[1];
        public bool canDamage => doneAttack && beamFX >= 1f;
        public bool doneAttack = false;
        public int attackTime = 15;
        public float beamLength => 2500;
        public float beamFX = 0;
        public float storedTime = 0;
        public Color drawColor = Color.White;
        public float sine = 0;
        public float beamRot = 0;
        Vector2 beamStart = Vector2.Zero;
        Vector2 directionToTarget = Vector2.Zero;
        public Vector2 targetPos => Projectile.Center;

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
            Projectile.scale = 2;
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
                drawColor = beamType switch
                {
                    0 => Color.Cyan,
                    1 => Color.Gold,
                    2 => Color.Silver,
                    3 => Color.Magenta,
                    _ => Color.White
                };

                beamRot = Main.rand.NextFloat(-0.3f, 0.3f);
                beamStart = targetPos + Vector2.UnitX.RotatedBy(beamRot) * beamLength;
                directionToTarget = beamStart.DirectionTo(targetPos);

                Projectile.Center += Main.rand.NextVector2CircularEdge(300, 300);
                
                if (attackSpeed == 0)
                    attackSpeed = 0.4f;
                
                Projectile.velocity = Vector2.Zero;
                beamFX = 1f;
            }

            if (time >= attackTime && !doneAttack)
            {
                SoundStyle attack = beamType switch
                {
                    0 => new SoundStyle("Terraria/Sounds/Item_28") with { Volume = 0.5f },
                    1 => new SoundStyle("Terraria/Sounds/Item_29") with { Volume = 0.6f },
                    2 => new SoundStyle("Terraria/Sounds/Item_30") with { Volume = 0.5f },
                    3 => new SoundStyle("Terraria/Sounds/Item_72") with { Volume = 0.7f },
                    _ => new SoundStyle("Terraria/Sounds/Item_28") with { Volume = 0.5f }
                };
                
                SoundEngine.PlaySound(attack, targetPos);
                beamFX = 2.5f;
                doneAttack = true;
                storedTime = time;

                if (Main.LocalPlayer.Distance(Projectile.Center) < 1600)
                    Main.LocalPlayer.SetScreenshake(3f);
            }

            float endTime = storedTime + 12;
            if (time >= endTime && doneAttack)
            {
                Projectile.Kill();
                return;
            }
            else if (doneAttack)
            {
                drawColor = Color.Lerp(drawColor, Color.White, (float)Math.Pow(Utils.GetLerpValue(endTime, storedTime, time, true), 2));
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
            switch (beamType)
            {
                case 0:
                    target.AddBuff(BuffID.Frostburn, 180);
                    break;
                case 1:
                    target.AddBuff(BuffID.OnFire, 180);
                    break;
                case 2:
                    target.AddBuff(BuffID.ShadowFlame, 180);
                    break;
                case 3:
                    target.AddBuff(BuffID.CursedInferno, 180);
                    break;
                default:
                    target.AddBuff(BuffID.OnFire, 120);
                    break;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!canDamage)
                return false;
            
            float _ = float.NaN;
            Vector2 start = beamStart;
            Vector2 end = beamStart + directionToTarget * beamLength * 2;
            float beamWidth = 50 * Projectile.scale;
            
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (beamFX == 0)
                return false;

            Texture2D beam = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineThick").Value;
            Texture2D bBeam = ModContent.Request<Texture2D>("CalamityMod/Particles/LineThick").Value;
            Texture2D bloom = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            
            float opacity = (doneAttack ? 0.7f : 0.4f) * (float)Math.Pow(Math.Min(beamFX, 1), 2);
            Color beamColor = drawColor with { A = 0 };

            float bloomScale = 0.5f * Projectile.scale * (doneAttack ? 2f : 1f);
            Color bloomColor = drawColor * opacity * 0.5f;
            Main.EntitySpriteDraw(bloom, beamStart - Main.screenPosition, null, bloomColor, 0f, bloom.Size() / 2f, bloomScale, SpriteEffects.None, 0);

            for (int t = 0; t < (doneAttack ? 3 : 1); t++)
            {
                bool isOutline = (t > 0);
                Texture2D beamTexture = isOutline ? bBeam : beam;
                
                float beamThickness = isOutline ? 
                    0.15f * (0.8f - 0.1f * t) * beamFX * Utils.Remap(sine, -1, 1, 0.9f, 1.1f) :
                    0.1f * beamFX * Utils.Remap(sine, -1, 1, 0.8f, 1.2f);
                
                Color finalColor = isOutline ? 
                    Color.Black * opacity * (0.3f + 0.1f * t) :
                    beamColor * opacity * (1 - t * 0.2f);

                Main.EntitySpriteDraw(
                    beamTexture, 
                    beamStart - Main.screenPosition, 
                    null, 
                    finalColor,
                    directionToTarget.ToRotation() + MathHelper.PiOver2,
                    new Vector2(beamTexture.Width / 2, beamTexture.Height),
                    new Vector2(beamThickness, beamLength / 1000) * Projectile.scale,
                    SpriteEffects.None
                );
            }

            if (beamType == 3 && doneAttack)
            {
                for (int i = 0; i < 3; i++)
                {
                    float offset = sine * 10;
                    Color sparkleColor = Color.Lerp(Color.Magenta, Color.Cyan, (float)i / 3) * opacity * 0.3f;
                    Vector2 sparklePos = beamStart + directionToTarget * (beamLength * 0.5f + offset + i * 50);
                    Main.EntitySpriteDraw(bloom, sparklePos - Main.screenPosition, null, sparkleColor, 0f, bloom.Size() / 2f, 0.3f, SpriteEffects.None, 0);
                }
            }

            return false;
        }
    }
}