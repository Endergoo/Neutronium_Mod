using Terraria;
using Terraria.ModLoader;

namespace Neutronium.Content.Players
{
        public class NeutroniumPlayer : ModPlayer
    {
        // 0.0 → 0.2 (max 20% of max HP per second)
        public float celestialRegenStack = 0f;

        public override void ResetEffects()
        {
            // Nothing here for regen stack since we don't want decay
        }

        public override void PostUpdate()
        {
            if (celestialRegenStack > 0f)
            {
                // Apply regen per tick (60 ticks per second)
                Player.statLife += (int)((Player.statLifeMax2 * celestialRegenStack) / 60f);

                // Clamp so you don't exceed max life
                if (Player.statLife > Player.statLifeMax2)
                    Player.statLife = Player.statLifeMax2;
            }
        }
    }
}