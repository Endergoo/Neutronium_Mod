using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Neutronium.Content.Rarities
{
    public class NeutronTouched : ModRarity
    {
        public override Color RarityColor => new Color(180, 50, 255);

        public static Color BloomClr = new Color(80, 0, 120, 0);
        public static Color TextClr = new Color(180, 50, 255, 50);

        private static float lastFlashTime = 0f;
        private static bool isFlashing = false;

        public static void Draw(Item item, SpriteBatch spriteBatch, string text, int X, int Y, Color textColor, Color lightColor, float rotation,
        Vector2 origin, Vector2 baseScale, float time, DynamicSpriteFont font)
        {
            // Flash trigger
            float flashDuration = 0.2f;
            if (Main.GameUpdateCount - lastFlashTime > flashDuration * 60)
            {
                if (Main.rand.NextFloat() < 0.005f)
                {
                    isFlashing = true;
                    lastFlashTime = Main.GameUpdateCount;
                }
                else
                {
                    isFlashing = false;
                }
            }

            var fontSize = font.MeasureString(text);

            textColor.A = 0;

            // Pulsing glow orbiting the text
            float pulsing = 1.5f + (float)Math.Sin(time * 5f);
            for (float f = 0f; f < MathHelper.TwoPi; f += 0.79f)
            {
                ChatManager.DrawColorCodedString(
                    spriteBatch, font, text,
                    new Vector2(X, Y) + new Vector2(pulsing, 0f).RotatedBy(f + time * 2f % MathHelper.TwoPi),
                    textColor * 0.5f, rotation, origin, baseScale);

                // Shake the glow when flashing
                if (isFlashing)
                    origin += Main.rand.NextVector2Circular(2f, 1.2f);
            }

            // Change color when flashing to electric purple/white
            if (isFlashing)
            {
                textColor = new Color(220, 100, 255, 50);
                lightColor = new Color(255, 100, 255, 50);
            }

            textColor.A = 255;

            // Base position with shake when flashing
            Vector2 basePos = new Vector2(X, Y);
            if (isFlashing)
                basePos += Main.rand.NextVector2Circular(3f, 10f);

            // Shadow
            ChatManager.DrawColorCodedStringShadow(spriteBatch, font, text, basePos, textColor * 2f, rotation, origin, baseScale);

            // Dark outline
            ChatManager.DrawColorCodedString(spriteBatch, font, text, basePos, new Color(40, 0, 60), rotation, origin, baseScale);

            // Shine sweep
            float shineWidth = 40f;
            float shineSpeed = 80f;
            float shineDisp = time * shineSpeed;
            float shinePos = shineDisp % (fontSize.X + shineWidth);

            float charOffsetX = 0f;
            for (int i = 0; i < text.Length; i++)
            {
                string c = text[i].ToString();
                Vector2 charSize = font.MeasureString(c);

                Vector2 charPos = basePos + new Vector2(charOffsetX, 0f);

                float centerX = charPos.X + (charSize.X * baseScale.X) / 2f + 10.5f;
                float dist = Math.Abs(centerX - (X + shinePos - shineWidth * 0.15f));
                float intensity = 1f - MathHelper.Clamp(dist / shineWidth, 0f, 1f);

                if (intensity > 0f)
                {
                    // Shine turns white/bright when flashing, purple otherwise
                    Color shineColor = isFlashing
                        ? new Color(255, 200, 255) * intensity * 2f
                        : new Color(220, 150, 255) * intensity * 1.5f;

                    ChatManager.DrawColorCodedString(
                        spriteBatch, font, c, charPos, shineColor, rotation, origin, baseScale);
                }

                charOffsetX += charSize.X - text.Length * 0.0085f;
            }
        }

        public static void Draw(Item item, string text, int X, int Y, float rotation, Vector2 origin, Vector2 baseScale)
        {
            Draw(item, Main.spriteBatch, text, X, Y, TextClr, BloomClr, rotation, origin, baseScale,
                Main.GlobalTimeWrappedHourly, FontAssets.MouseText.Value);
        }

        public static void Draw(Item item, DrawableTooltipLine line)
        {
            Draw(item, line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale);
        }
    }
}