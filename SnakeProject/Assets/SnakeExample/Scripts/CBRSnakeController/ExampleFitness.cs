using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class myCaseFitness : CaseFitness
{
    public override int Compare(CaseWithSimilarity x, CaseWithSimilarity y)
    {
        double actualValueX = x.similarity * 0.8 + (x.myCase.getProperty("Score") / 10) * 0.2;
        double actualValueY = y.similarity * 0.8 + (y.myCase.getProperty("Score") / 10) * 0.2;
        // TODO: Calcular el score y ver cuanto afecta a la elección
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
