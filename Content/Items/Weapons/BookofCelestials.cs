using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;

namespace Neutronium.Items.Weapons
{
    public class BookofCelestials : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Book of Celestials");
            Tooltip.SetDefault("Calls upon the sun and moon\nRain falls when submerged in water");
        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Magic;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 0f;
            Item.mana = 12;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 target = Main.MouseWorld;
            
            // Determine beam type based on day/night
            int beamType = Main.dayTime ? 
                ModContent.ProjectileType<SunBeam>() : 
                ModContent.ProjectileType<MoonBeam>();
            
            // Spawn the main beam
            Projectile.NewProjectile(source, target, Vector2.Zero, beamType, damage, knockback, player.whoAmI);
            
            // If player is in water, add rain projectiles
            if (player.wet)
            {
                int rainCount = Main.rand.Next(3, 6);
                for (int i = 0; i < rainCount; i++)
                {
                    float offsetX = Main.rand.NextFloat(-150f, 150f);
                    Vector2 rainSpawn = new Vector2(target.X + offsetX, target.Y - 500);
                    Vector2 rainVelocity = new Vector2(Main.rand.NextFloat(-1f, 1f), 8f);
                    
                    Projectile.NewProjectile(source, rainSpawn, rainVelocity, 
                        ModContent.ProjectileType<CelestialRain>(), 
                        (int)(damage * 0.6f), knockback * 0.5f, player.whoAmI);
                }
            }
            
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.SpellTome);
            recipe.AddIngredient(ItemID.SunplateBlock, 20);
            recipe.AddIngredient(ItemID.Meteorite, 15);
            recipe.AddIngredient(ItemID.FallenStar, 10);
            recipe.AddTile(TileID.Bookcases);
            recipe.Register();
        }
    }

    // Base class for celestial beams
    public abstract class CelestialBeamBase : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0"; // Invisible
        
        private float time = 0;
        private bool doneAttack = false;
        private int attackTime = 15; // Telegraph time
        private float laserLength = 2000f;
        private float laserFX = 0;
        private float storedTime = 0;
        private float sine = 0;
        
        private Vector2 beamStart = Vector2.Zero;
        private Vector2 targetPos = Vector2.Zero;
        
        protected abstract Color BeamColor { get; }
        protected abstract int DustType { get; }
        
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            // Fade out effect
            if (laserFX > 0)
                laserFX = MathHelper.Lerp(laserFX, 0, time > attackTime ? 0.08f : 0.01f);
            
            sine = (float)Math.Sin(time * 0.2f);
            
            // Initialize on first frame
            if (time == 0)
            {
                targetPos = Projectile.Center;
                beamStart = targetPos - Vector2.UnitY * laserLength; // Spawn above target
                laserFX = 1f;
                SoundEngine.PlaySound(SoundID.Item109, targetPos);
            }
            
            // Attack phase starts
            if (time >= attackTime && !doneAttack)
            {
                SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.8f, Pitch = 0.3f }, targetPos);
                laserFX = 2.5f;
                doneAttack = true;
                storedTime = time;
                
                // Screen shake
                if (Main.LocalPlayer.Distance(targetPos) < 1000)
                    Main.LocalPlayer.GetModPlayer<ModPlayer>().ScreenshakePower = 3;
            }
            
            float endTime = storedTime + 12;
            if (time >= endTime && doneAttack)
            {
                Projectile.Kill();
                return;
            }
            
            // Spawn dust effects
            if (doneAttack && Main.rand.NextBool(2))
            {
                Vector2 dustPos = beamStart + Vector2.UnitY * Main.rand.NextFloat(laserLength);
                Dust dust = Dust.NewDustPerfect(dustPos, DustType, Vector2.Zero, 100, default, 1.5f);
                dust.noGravity = true;
            }
            
            time++;
        }

        public override bool? CanHitNPC(NPC target) => doneAttack && laserFX >= 1f ? null : false;
        
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!doneAttack || laserFX < 1f)
                return false;
            
            float _ = float.NaN;
            Vector2 start = beamStart;
            Vector2 end = beamStart + Vector2.UnitY * laserLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), 
                start, end, 40, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (laserFX == 0)
                return false;
            
            // You'll need to add these textures to your mod
            // For now, we'll use Terraria's built-in textures as placeholders
            Texture2D beamTex = ModContent.Request<Texture2D>("Terraria/Images/Extra_193").Value; // Laser texture
            
            float opacity = (doneAttack ? 0.8f : 0.4f) * (float)Math.Pow(Math.Min(laserFX, 1), 2);
            Color beamColor = BeamColor with { A = 0 } * opacity;
            
            Vector2 origin = new Vector2(beamTex.Width / 2, 0);
            float rotation = MathHelper.PiOver2; // Point downward
            float scale = 0.5f * (laserFX <= 1 ? laserFX : laserFX * 0.5f) * (0.9f + sine * 0.1f);
            
            // Draw multiple passes for glow effect
            for (int i = 0; i < (doneAttack ? 3 : 1); i++)
            {
                float layerScale = scale * (1f - i * 0.2f);
                Main.EntitySpriteDraw(
                    beamTex,
                    beamStart - Main.screenPosition,
                    null,
                    beamColor * (1f - i * 0.3f),
                    rotation,
                    origin,
                    new Vector2(layerScale, laserLength / beamTex.Height),
                    SpriteEffects.None
                );
            }
            
            // Draw bloom at base
            Texture2D bloomTex = ModContent.Request<Texture2D>("Terraria/Images/Extra_189").Value;
            Main.EntitySpriteDraw(
                bloomTex,
                beamStart - Main.screenPosition,
                null,
                beamColor * 0.5f,
                0,
                bloomTex.Size() / 2,
                scale * 0.3f,
                SpriteEffects.None
            );
            
            return false;
        }
    }

    // Sun Beam implementation
    public class SunBeam : CelestialBeamBase
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sun Beam");
        }

        protected override Color BeamColor => new Color(255, 200, 100);
        protected override int DustType => DustID.SolarFlare;
    }

    // Moon Beam implementation
    public class MoonBeam : CelestialBeamBase
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moon Beam");
        }

        protected override Color BeamColor => new Color(100, 150, 255);
        protected override int DustType => DustID.BlueCrystalShard;
    }

    // Rain Projectile (simple version)
    public class CelestialRain : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Rain");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = true;
            Projectile.alpha = 100;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.Water, 0f, 0f, 100, default, 0.8f);
                dust.velocity *= 0.3f;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.Water, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
            }
        }
    }
}