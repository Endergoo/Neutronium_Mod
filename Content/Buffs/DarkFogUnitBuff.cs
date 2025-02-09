using Terraria;
using Terraria.ModLoader;
using Neutronium.Content.Projectiles.Minions;

namespace Neutronium.Content.Buffs
{
    public class DarkFogUnitBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<DarkFogUnit>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
                return;
            }

            player.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}