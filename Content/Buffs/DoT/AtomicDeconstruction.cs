using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Neutronium.Content.Buffs.DoT
{
    public class AtomicDeconstruction : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            // Defense reduction
            npc.defense -= 15;

            // Damage over time — every 30 ticks (0.5 seconds)
           if (npc.buffTime[buffIndex] % 30 == 0)
            {
                npc.SimpleStrikeNPC(20, 0, false, 0f, DamageClass.Melee, true);
            }

            // Purple aura particles
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(
                    npc.position,
                    npc.width,
                    npc.height,
                    DustID.GemAmethyst
                );
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
                dust.color = new Color(180, 50, 255) with { A = 0 };
                dust.velocity = new Vector2(0, -Main.rand.NextFloat(1f, 3f));
                dust.fadeIn = Main.rand.NextFloat(0.5f, 1f);
            }

            // Faint purple light on the enemy
            Lighting.AddLight(npc.Center, new Vector3(0.3f, 0f, 0.5f));
        }
    }
}