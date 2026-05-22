using System.Collections.Generic;
using UnityEngine;

public enum ClassesInimigos { Minion, Basico, Chefe, ChefeFinal, Jogador }
public struct DadosPorClasse
{
    public float ChanceStunado;
    public int QuantidadeAcoes;
};

public class AtributosPorClasse
{
    public static Dictionary<ClassesInimigos, DadosPorClasse> S_ValoresPorClasse = new Dictionary<ClassesInimigos, DadosPorClasse>
    {
        {
            ClassesInimigos.Basico, new DadosPorClasse
            {
                ChanceStunado = 0.75f,
                QuantidadeAcoes = 1
            }
        },
        {
            ClassesInimigos.Minion, new DadosPorClasse
            {
                ChanceStunado = 1f,
                QuantidadeAcoes = 1
            }
        },
        {
            ClassesInimigos.Chefe, new DadosPorClasse
            {
                ChanceStunado = 0.15f,
                QuantidadeAcoes = 1
            }
        },
        {
            ClassesInimigos.ChefeFinal, new DadosPorClasse
            {
                ChanceStunado = 0.15f,
                QuantidadeAcoes = 2
            }
        },
        {
            ClassesInimigos.Jogador, new DadosPorClasse
            {
                ChanceStunado = 1f,
                QuantidadeAcoes = 1
            }
        }
    };
}
