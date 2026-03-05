using Terraria;
using Terraria.ModLoader;

namespace Neutronium.Content.Buffs
{
    public class CelestialRegen : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // +2% regen per stack, capped at 20%
            float maxRegen = 0.20f;
            float increment = 0.02f;
            float stacks = player.CountBuffs(ModContent.BuffType<CelestialRegen>());
            
            float regenPercent = Math.Min(stacks * increment, maxRegen);

            player.lifeRegen += (int)(regenPercent * player.statLifeMax2 / 60f); // life per tick
        }
    }
}