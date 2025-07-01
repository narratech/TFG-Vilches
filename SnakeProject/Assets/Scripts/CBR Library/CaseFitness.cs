using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CaseWithSimilarity
{
    public CaseCBR myCase;
    public float similarity;
    public CaseWithSimilarity(CaseCBR cas, float sim)
    {
        myCase = cas;
        similarity = sim;
    }
}

/// <summary>
/// Comparador de orden para los casos en la lista de KNN. Es como un Score general del caso (similaritud solo,
/// mezcla de similaritud y fitness, etc..)
/// </summary>
public class CaseFitness : IComparer<CaseWithSimilarity>
{
    // Compara el score del caso y su similitud.
    public virtual int Compare(CaseWithSimilarity x, CaseWithSimilarity y)
    {
        double actualValueX = x.similarity;
        double actualValueY = y.similarity;
        if (actualValueX > actualValueY)
        {
            return -1;
        }
        else if (actualValueX < actualValueY)
        {
            return 1;
        }
        else return 0;
    }
}
