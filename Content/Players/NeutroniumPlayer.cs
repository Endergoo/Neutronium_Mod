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

        public override void UpdateDead()
        {
            // Optional: reset stack on death
            celestialRegenStack = 0f;
        }

        public override void PostUpdate()
        {
            if (celestialRegenStack > 0f)
            {
                Main.NewText($"Celestial Regen Stack: {celestialRegenStack * 100f}% | LifeRegen: {Player.lifeRegen}", 255, 255, 0);
            }
        }
    }
}