using System.Collections.Generic;
using UnityEngine;

//Como o cruzado tem o comportamente de dar o escudo pra ele mesmo, precisei fazer essa classe derivada
//Ela tem tudo do PersonagemController, mas o UsarHabilidade força o escudo do cruzado à aplicar nele mesmo
//Pra funcionar, claro, QtdAlvosAliados do escudo tem que ser 0
public class Cruzado : PersonagemController
{
    public override void UsarHabilidade(HabilidadesSO habilidade, List<PersonagemController> aliadosTarget, List<PersonagemController> inimigosTarget)
    {
        if (habilidade.Shield > 0)
        {
            this._shield += habilidade.Shield;
            this.OnEffectsApplied?.Invoke(SkillEffects.SHIELD);
        }

        base.UsarHabilidade(habilidade, aliadosTarget, inimigosTarget);
    }
}
