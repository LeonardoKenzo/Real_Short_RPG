using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ceifeiro : PersonagemController
{
    public override void UsarHabilidade(HabilidadesSO habilidade, List<PersonagemController> aliadosTarget, List<PersonagemController> inimigosTarget)
    {
        //Para que o ceifeiro não ataque todos os jogadores, vai ser necessário remover o efeito ATTACK
        //Para isso, precisa criar uma cópia da habilidade, senão afetará a referência global 
        //(do próprio editor, afetando permanentemente)
        HabilidadesSO copiaHabilidade = habilidade.Copiar();

        //Se for o ataque de foice, causa dano apenas no alvo principal
        if (copiaHabilidade.Effect.Contains(SkillEffects.ATTACK))
        {
            inimigosTarget[0].ReceberDano(copiaHabilidade.Dano + base._buffs.Values.Sum() + base._debuffs.Values.Sum());
            base.VerificarBuffsDebuffs();

            copiaHabilidade.Effect.Remove(SkillEffects.ATTACK);
        }

        base.UsarHabilidade(copiaHabilidade, aliadosTarget, inimigosTarget);
    }
}
