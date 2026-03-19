using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Neutronium.Core.Utils;

namespace Neutronium.Content.Items.Weapons
{
    public class CrykalsChoir : ModItem, IGlowmaskItem
    {
        private int time = 0;
        private int swingCount = 0;
        private Vector2 bladeHitboxPos;
        private float completion = 0f;
        private bool canHit => (completion >= 0.35f && completion <= 0.8f);
        private bool playSound = true;

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

                float lerp = Utils.GetLerpValue(cutoff, cutoff2, completion, true);
                float eased = EaseInOut(lerp);
                player.itemRotation = player.Center.DirectionTo(mPos).ToRotation()
                    + MathHelper.Lerp(minRot, endRot, eased);
                player.itemRotation += MathHelper.Pi * (dir == 1 ? 0 : 1) + MathHelper.PiOver4 * dir;
            }

            // Track blade tip along actual sword rotation
            Vector2 bladeDir = player.itemRotation.ToRotationVector2();
            bladeHitboxPos = player.Center + bladeDir * 180f;

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
                hitbox = new Rectangle(-10000, -10000, 1, 1);
                return;
            }

            hitbox = new Rectangle(
                (int)(bladeHitboxPos.X - 150f),
                (int)(bladeHitboxPos.Y - 150f),
                300,
                300);
        }

        public override bool? CanHitNPC(Player player, NPC target)
        {
            if (!canHit)
                return false;

            Vector2 bladeDir = player.itemRotation.ToRotationVector2();
            float _ = 0f;

            bool hit = Collision.CheckAABBvLineCollision(
                target.Hitbox.TopLeft(),
                target.Hitbox.Size(),
                player.Center - bladeDir * 20f,   // slightly behind player
                player.Center + bladeDir * 180f,  // full blade length
                60f,                               // blade thickness
                ref _);

            return hit ? null : false;
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