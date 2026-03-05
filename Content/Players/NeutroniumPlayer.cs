using Terraria;
using Terraria.ModLoader;

namespace Neutronium.Content.Players
{
    public class NeutroniumPlayer : ModPlayer
    {
        // Tracks regen from celestial beam hits
        public float celestialRegenStack = 0f;

        public override void ResetEffects()
        {
            // Reset every tick
            celestialRegenStack = 0f;
        }

        public override void UpdateLifeRegen()
        {
            // Apply the regen to player's lifeRegen
            Player.lifeRegen += (int)(celestialRegenStack * Player.statLifeMax2 / 60f);
        }
    }
}