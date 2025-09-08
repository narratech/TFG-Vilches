using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

class myComparer : ICaseComparer
{
    public CaseWithSimilarity computeSimilarity(in CaseCBR query, in CaseCBR caseToLook, Dictionary<string, float> weigths)
    {
        float maxDistance = (Math.Abs(0 - 30) + Math.Abs(0 - 19));
        float similarity = 0;
        similarity += CaseUtility.computeV2ManhattanSimilarity(query.getProperty("position"),
           caseToLook.getProperty("position"), maxDistance) * weigths["position"];
        similarity += CaseUtility.computeFloatSimilarity(query.getProperty("fruitDis"),
           caseToLook.getProperty("fruitDis"), maxDistance) * weigths["fruitDis"];
        similarity += (Vector3.Equals(query.getProperty("headDirection"),
            caseToLook.getProperty("headDirection"))? 1 * weigths["headDirection"] : 0 );
        similarity += CaseUtility.computeBoolListSimilarity(query.getProperty("checkCollisions"),
            caseToLook.getProperty("checkCollisions")) * weigths["checkCollisions"];
        int totalSimRelPos = 0;
        for(int i=0; i < query.getProperty("fruitRelPos").Count;i++)
        {
            totalSimRelPos += query.getProperty("fruitRelPos")[i] == caseToLook.getProperty("fruitRelPos")[i] ? 1 : 0;
        }
        similarity += (totalSimRelPos / query.getProperty("fruitRelPos").Count) * weigths["fruitRelPos"];

        return new CaseWithSimilarity(caseToLook, similarity);
    }
}
