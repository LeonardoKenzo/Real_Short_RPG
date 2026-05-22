using System.Collections.Generic;
using UnityEngine;

public class Escuridao : PersonagemController
{
    public override void UsarHabilidade(HabilidadesSO habilidade, List<PersonagemController> aliadosTarget, List<PersonagemController> inimigosTarget)
    {
        if (habilidade.Effects.Contains(SkillEffects.POISON))
        {
            FiltroEscuridao.S_ReferenciaFiltro.Escurecer();
        }

        base.UsarHabilidade(habilidade, aliadosTarget, inimigosTarget);
    }
}
