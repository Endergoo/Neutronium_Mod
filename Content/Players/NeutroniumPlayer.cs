using Terraria;
using Terraria.ModLoader;

namespace Neutronium.Content.Players
{
    public class NeutroniumPlayer : ModPlayer
    {
        // Daytime regen
        public float celestialRegenStack = 0f; // 0.0 -> 0.2 (max 20%)

        // Nighttime Crit buff
        public float celestialCritStack = 0f; // 0.0 -> 0.5 (max 50%)

        public override void PostUpdate()
        {
            if (Main.dayTime)
            {
                // Apply HP/sec regen for day
                if (celestialRegenStack > 0f)
                {
                    Player.statLife += (int)((Player.statLifeMax2 * celestialRegenStack) / 60f);
                    if (Player.statLife > Player.statLifeMax2)
                        Player.statLife = Player.statLifeMax2;
                }
            }
            else
            {
                // Apply crit bonus for night
                if (celestialCritStack > 0f)
                {
                    // Apply to magic crit chance
                    Player.GetCritChance(DamageClass.Magic) += (int)(celestialCritStack * 100);
                }
            }
        }
    }
}