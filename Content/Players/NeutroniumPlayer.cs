using Terraria;
using Terraria.ModLoader;

namespace Neutronium.Content.Players
{
    public class NeutroniumPlayer : ModPlayer
    {
        // Tracks how much regen the beam has given this tick
        public float celestialRegenStack = 0f;

        public override void ResetEffects()
        {
            // Reset each tick so only active hits count
            celestialRegenStack = 0f;
        }

        public override void UpdateLifeRegen()
        {
            // Convert the stack to actual lifeRegen
            Player.lifeRegen += (int)(celestialRegenStack * Player.statLifeMax2 / 60f);
        }
    }
}