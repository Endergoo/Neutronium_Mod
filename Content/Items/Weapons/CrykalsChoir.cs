using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Core.Utils;
using Neutronium.Content.Projectiles;

namespace Neutronium.Content.Items.Weapons
{
    public class CrykalsChoir : ModItem, IGlowmaskItem
    {
        private int time = 0;
        private int swingCount = 0;
        private Vector2 bladeTipPos;
        private float completion = 0f;
        private bool canHit => (completion >= 0.35f && completion <= 0.8f);
        private bool playSound = true;
        private bool spawnProj = true;

        public Texture2D GlowTexture =>
            ModContent.Request<Texture2D>("Neutronium/Content/Items/Weapons/CrykalsChoirGlow").Value;

        public override void SetDefaults()
        {
            Item.width = 140;
            Item.height = 140;
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UseAnimation(Player player)
        {
            swingCount++;
            time = 0;
            playSound = true;
            bladeTipPos = player.Center;
            spawnProj = true;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            completion = (float)time / (Item.useAnimation / player.GetAttackSpeed(DamageClass.Melee));
            Vector2 mPos = Main.MouseWorld;
            int dir = -Math.Sign(player.Center.X - mPos.X);

            float startRot = MathHelper.ToRadians(-110f) * dir * (swingCount % 2 == 0 ? 1 : -1);
            float endRot   = MathHelper.ToRadians(-110f) * dir * (swingCount % 2 == 0 ? -1 : 1);
            float minRot   = MathHelper.ToRadians(-150f) * dir * (swingCount % 2 == 0 ? 1 : -1);
            float cutoff  = 0.2f;
            float cutoff2 = 0.95f;

            float lerp = completion <= cutoff
                ? Utils.GetLerpValue(0f, cutoff, completion, true)
                : Utils.GetLerpValue(cutoff, cutoff2, completion, true);
            float eased = EaseInOut(lerp);

            if (completion <= cutoff)
            {
                player.itemRotation = player.Center.DirectionTo(mPos).ToRotation() + MathHelper.Lerp(startRot, minRot, eased);
            }
            else
            {
                if (playSound)
                {
                    SoundEngine.PlaySound(SoundID.Item1, player.Center);
                    playSound = false;
                }
                player.itemRotation = player.Center.DirectionTo(mPos).ToRotation() + MathHelper.Lerp(minRot, endRot, eased);

                if (completion >= 0.6f && spawnProj)
                {
                    int projCount = 2;
                    float spreadAngle = 0.20f;
                    Vector2 baseDir = player.Center.DirectionTo(Main.MouseWorld) * 18f;

                    for (int i = 0; i < projCount; i++)
                    {
                        float angleOffset = (i - (projCount - 1) / 2f) * spreadAngle;
                        float randomOffset = Main.rand.NextFloat(-0.1f, 0.1f);
                        Vector2 spreadVelocity = baseDir.RotatedBy(angleOffset);

                        Projectile.NewProjectile(
                            player.GetSource_ItemUse(Item),
                            player.Center,
                            spreadVelocity,
                            ModContent.ProjectileType<CrykalsChoirProj>(),
                            Item.damage,
                            Item.knockBack,
                            player.whoAmI
                        );
                    }

                    SoundEngine.PlaySound(SoundID.Item9, player.Center);
                    spawnProj = false;
                }
            }

            player.itemRotation += MathHelper.Pi * (dir == 1 ? 0 : 1) + MathHelper.PiOver4 * dir;

            float extraRot = (dir == 1 ? -MathHelper.PiOver4 : MathHelper.ToRadians(225f));
            bladeTipPos = player.Center + (player.itemRotation + extraRot).ToRotationVector2() * 180f;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full,
                player.itemRotation + MathHelper.ToRadians(-130f) * dir);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full,
                player.itemRotation + MathHelper.ToRadians(-130f) * dir);

            player.itemLocation = player.Center;
            player.direction = dir;
            time++;
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (!canHit)
            {
                hitbox = new Rectangle(-10000, -10000, 1, 1); // disable when swing not active
                return;
            }

            // Adjusted for larger 140x140 sprite
            Vector2 swingDir = (bladeTipPos - player.Center).SafeNormalize(Vector2.UnitX);

            // Length along the swing, width across the blade
            Vector2 size = new Vector2(100f, 60f); // increased from 80x40 to 100x60
            Vector2 center = player.Center + swingDir * 140f; // move center along blade path

            hitbox = new Rectangle(
                (int)(center.X - size.X / 2f),
                (int)(center.Y - size.Y / 2f),
                (int)size.X,
                (int)size.Y);
        }

        public override bool? CanHitNPC(Player player, NPC target)
        {
            Vector2 mPos = Main.MouseWorld;
            Vector2 shootDir = player.Center.DirectionTo(mPos);
            float _ = float.NaN;

            // Extend the line to match larger sprite
            bool hitCheck = Collision.CheckAABBvLineCollision(
                target.Hitbox.TopLeft(), target.Hitbox.Size(),
                player.Center - shootDir * 40f, // slightly farther back
                bladeTipPos + shootDir * 20f,   // extend tip along swing
                Item.width * 3.5f,              // slightly wider for bigger sprite
                ref _);

            return (canHit && hitCheck) ? null : false;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(GlowTexture, position, frame, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
        }

        private static float EaseInOut(float t)
        {
            return (float)(-(Math.Cos(Math.PI * t) - 1f) / 2f);
        }
    }
}