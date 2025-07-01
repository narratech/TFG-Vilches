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
        similarity += CaseUtility.computeV2ManhattanSimilarity(query.getProperty("fruitPos"),
           caseToLook.getProperty("fruitPos"), maxDistance) * weigths["fruitPos"];
        similarity += CaseUtility.computeV3ManhattanSimilarity(query.getProperty("headDirection"),
           caseToLook.getProperty("headDirection"), maxDistance) * weigths["headDirection"];
        similarity += CaseUtility.computeV2ListManhattanSimilarity(query.getProperty("myPartsNodes"),
           caseToLook.getProperty("myPartsNodes"), maxDistance) * weigths["myPartsNodes"];
        similarity += CaseUtility.computeV2ListManhattanSimilarity(query.getProperty("otherSnakePartsNode"),
          caseToLook.getProperty("otherSnakePartsNode"), maxDistance) * weigths["otherSnakePartsNode"];
        similarity += CaseUtility.computeFloatListSimilarity(query.getProperty("DistanceToWalls"), caseToLook.getProperty("DistanceToWalls"), maxDistance)
            * weigths["DistanceToWalls"];
        similarity += CaseUtility.computeBoolSimilarity(query.getProperty("inTrackToCollide"), caseToLook.getProperty("inTrackToCollide"))
            * weigths["inTrackToCollide"];

        return new CaseWithSimilarity(caseToLook, similarity);
    }
}
