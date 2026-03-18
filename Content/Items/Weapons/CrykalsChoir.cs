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
        private Vector2 bladeHitboxPos;
        private float completion = 0f;
        private bool canHit => (completion >= 0.35f && completion <= 0.8f);
        private bool trailSpawned = false;
        private bool playSound = true;

        // GlowmaskItem implementation — used by GlowmaskGlobalItem for in-hand glow
        public Texture2D GlowTexture =>
            ModContent.Request<Texture2D>("Neutronium/Content/Items/Weapons/CrykalsChoirGlow").Value;

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
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
            trailSpawned = false;
            playSound = true;
            bladeHitboxPos = player.Center;
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

            if (completion <= cutoff)
            {
                float lerp = Utils.GetLerpValue(0f, cutoff, completion, true);
                float eased = EaseInOut(lerp);
                player.itemRotation = player.Center.DirectionTo(mPos).ToRotation()
                    + MathHelper.Lerp(startRot, minRot, eased);
                player.itemRotation += MathHelper.Pi * (dir == 1 ? 0 : 1) + MathHelper.PiOver4 * dir;
            }
            else
            {
                if (playSound)
                {
                    SoundEngine.PlaySound(SoundID.Item1, player.Center);
                    playSound = false;
                }

                if (!trailSpawned && completion >= 0.4f)
                {
                    Projectile.NewProjectile(
                        player.GetSource_ItemUse(Item),
                        bladeHitboxPos,
                        Vector2.Zero,
                        ModContent.ProjectileType<CrykalsChoirTrail>(),
                        0, 0f, player.whoAmI);
                    trailSpawned = true;
                }

                float lerp = Utils.GetLerpValue(cutoff, cutoff2, completion, true);
                float eased = EaseInOut(lerp);
                player.itemRotation = player.Center.DirectionTo(mPos).ToRotation()
                    + MathHelper.Lerp(minRot, endRot, eased);
                player.itemRotation += MathHelper.Pi * (dir == 1 ? 0 : 1) + MathHelper.PiOver4 * dir;
            }

            float extraRot = (dir == 1 ? -MathHelper.PiOver4 : MathHelper.ToRadians(225f));
            bladeHitboxPos = player.Center + (player.itemRotation + extraRot).ToRotationVector2() * 60f;

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
            float scale = 8f;
            Vector2 newSize = new Vector2(hitbox.Width, hitbox.Height) * scale;
            hitbox = new Rectangle(
                (int)(bladeHitboxPos.X - newSize.X / 2f),
                (int)(bladeHitboxPos.Y - newSize.Y / 2f),
                (int)newSize.X, (int)newSize.Y);
        }

        public override bool? CanHitNPC(Player player, NPC target)
        {
            Vector2 mPos = Main.MouseWorld;
            Vector2 shootDir = player.Center.DirectionTo(mPos);
            float _ = float.NaN;
            bool hitCheck = Collision.CheckAABBvLineCollision(
                target.Hitbox.TopLeft(), target.Hitbox.Size(),
                player.Center - shootDir * 30f,
                player.Center + shootDir * 100f,
                Item.width * 3f, ref _);
            return (canHit && hitCheck) ? null : false;
        }

        // Drawn when item is on the ground — glow ignores world lighting
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, GlowTexture);
        }

        // Drawn when item is in the inventory/hotbar
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(GlowTexture, position, frame, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
        }

        // Sine-curve ease — perfectly smooth in and out
        private static float EaseInOut(float t)
        {
            return (float)(-(Math.Cos(Math.PI * t) - 1f) / 2f);
        }
    }
}