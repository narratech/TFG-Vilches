using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public struct CaseWithSimilarity
{
    public CaseCBRv2 myCase;
    public float similarity;
    public CaseWithSimilarity(CaseCBRv2 cas,float sim)
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
public class CaseUtility
{
    #region VarComparers
    /// <summary>
    /// Se encarga de devolver, siendo 0 nada parecidos y 1 iguales, como de similares son las dos variables
    /// </summary>
    /// <param name="query">Variable 1</param>
    /// <param name="caseToLook">Variable 2</param>
    /// <param name="range">Rango para identificar como de significativa es la diferencia</param>
    /// <returns>Como de similar son las dos variables</returns>
    public static float computeFloatSimilarity(in float query, in float caseToLook, float range)
    {
        return 1 - (Math.Abs(query - caseToLook)/range);
    }
    /// <summary>
    /// Se encarga de devolver, siendo 0 nada parecidos y 1 iguales, como de similares son las dos variables
    /// </summary>
    /// <param name="query">Variable 1</param>
    /// <param name="caseToLook">Variable 2</param>
    /// <param name="range">Rango para identificar como de significativa es la diferencia</param>
    /// <returns>Como de similar son las dos variables</returns>
    public static float computeFloatListSimilarity(in List<float> query, in List<float> caseToLook, float range)
    {
        float disparity = 0;
        int mostItems = Math.Max(caseToLook.Count, query.Count);
        int lessItems = Math.Min(caseToLook.Count, query.Count);
        for (int i=0;i<lessItems;i++)
        {
            disparity += (1 - (Math.Abs(query[i] - caseToLook[i]) / range)) / mostItems;
        }
        return 1 - disparity;
    }
    /// <summary>
    /// Se encarga de devolver, siendo 0 nada parecidos y 1 iguales, como de similares son las dos variables
    /// </summary>
    /// <param name="query">Variable 1</param>
    /// <param name="caseToLook">Variable 2</param>
    /// <returns>Como de similar son las dos variables</returns>
    public static float computeBoolSimilarity(in bool query, in bool caseToLook)
    {
        return query == caseToLook? 1 : 0 ;
    }
    /// <summary>
    /// Se encarga de devolver, siendo 0 nada parecidos y 1 iguales, como de similares son las dos variables
    /// </summary>
    /// <param name="query">Variable 1</param>
    /// <param name="caseToLook">Variable 2</param>
    /// <returns>Como de similar son las dos variables</returns>
    public static float computeBoolListSimilarity(in List<bool> query, in List<bool> caseToLook)
    {
        float disparity = 0;
        int mostItems = Math.Max(caseToLook.Count, query.Count);
        int lessItems = Math.Min(caseToLook.Count, query.Count);
        for (int i = 0; i < lessItems; i++)
        {
            disparity += (1 - (query[i] == caseToLook[i] ? 1 : 0)) /mostItems;
        }
        return 1-disparity;
    }
    /// <summary>
    /// Se encarga de devolver, siendo 0 nada parecidos y 1 iguales, como de similares son las dos variables
    /// </summary>
    /// <param name="query">Variable 1</param>
    /// <param name="caseToLook">Variable 2</param>
    /// <param name="range">Rango para identificar como de significativa es la diferencia</param>
    /// <returns>Como de similar son las dos variables</returns>

    public static float computeV2ManhattanSimilarity(in Vector2 query, in Vector2 caseToLook, float range)
    {
        float disparity = 0;
        disparity = (Math.Abs(query.x - caseToLook.x) + Math.Abs(query.y - caseToLook.y)) / range;
        return 1 - disparity;
    }
    /// <summary>
    /// Se encarga de devolver, siendo 0 nada parecidos y 1 iguales, como de similares son las dos variables
    /// </summary>
    /// <param name="query">Variable 1</param>
    /// <param name="caseToLook">Variable 2</param>
    /// <param name="range">Rango para identificar como de significativa es la diferencia</param>
    /// <returns>Como de similar son las dos variables</returns>
    public static float computeV2EuclideanSimilarity(in Vector2 query, in Vector2 caseToLook, float range)
    {
        float disparity = 0;
        disparity = ((float)Math.Sqrt((float)Math.Pow(query.x - caseToLook.x, 2) + (float)Math.Pow(query.y - caseToLook.y, 2))) / range;
        return 1 - disparity;
    }
    /// <summary>
    /// Se encarga de devolver, siendo 0 nada parecidos y 1 iguales, como de similares son las dos variables
    /// </summary>
    /// <param name="query">Variable 1</param>
    /// <param name="caseToLook">Variable 2</param>
    /// <param name="range">Rango para identificar como de significativa es la diferencia</param>
    /// <returns>Como de similar son las dos variables</returns>
    public static float computeV2ListManhattanSimilarity(in List<Vector2> query, in List<Vector2> caseToLook, float range)
    {
        float disparity = 0;
        int mostItems = Math.Max(caseToLook.Count, query.Count);
        int lessItems = Math.Min(caseToLook.Count, query.Count);
        for (int i = 0; i < lessItems; i++)
        {
            disparity += (1 - ((Math.Abs(query[i].x - caseToLook[i].x) + Math.Abs(query[i].y - caseToLook[i].y)) / range))/mostItems;
        }
        return 1 - disparity;
    }
    /// <summary>
    /// Se encarga de devolver, siendo 0 nada parecidos y 1 iguales, como de similares son las dos variables
    /// </summary>
    /// <param name="query">Variable 1</param>
    /// <param name="caseToLook">Variable 2</param>
    /// <param name="range">Rango para identificar como de significativa es la diferencia</param>
    /// <returns>Como de similar son las dos variables</returns>
    public static float computeV2ListEuclideanSimilarity(in List<Vector2> query, in List<Vector2> caseToLook, float range)
    {
        float disparity = 0;
        int mostItems = Math.Max(caseToLook.Count, query.Count);
        int lessItems = Math.Min(caseToLook.Count, query.Count);
        for (int i = 0; i < lessItems; i++)
        {
            disparity += (1 - (((float)Math.Sqrt((float)Math.Pow(query[i].x - caseToLook[i].x, 2) + (float)Math.Pow(query[i].y - caseToLook[i].y, 2))) / range)) / mostItems;
        }
        return 1 - disparity;
    }
    /// <summary>
    /// Se encarga de devolver, siendo 0 nada parecidos y 1 iguales, como de similares son las dos variables
    /// </summary>
    /// <param name="query">Variable 1</param>
    /// <param name="caseToLook">Variable 2</param>
    /// <param name="range">Rango para identificar como de significativa es la diferencia</param>
    /// <returns>Como de similar son las dos variables</returns>
    public static float computeV3ManhattanSimilarity(in Vector3 query, in Vector3 caseToLook, float range)
    {
        float disparity = 0;
        disparity = (Math.Abs(query.x - caseToLook.x) + Math.Abs(query.y - caseToLook.y) + Math.Abs(query.z-caseToLook.z)) / range;
        return 1 - disparity;
    }
    /// <summary>
    /// Se encarga de devolver, siendo 0 nada parecidos y 1 iguales, como de similares son las dos variables
    /// </summary>
    /// <param name="query">Variable 1</param>
    /// <param name="caseToLook">Variable 2</param>
    /// <param name="range">Rango para identificar como de significativa es la diferencia</param>
    /// <returns>Como de similar son las dos variables</returns>
    public static float computeV3EuclideanSimilarity(in Vector3 query, in Vector3 caseToLook, float range)
    {
        float disparity = 0;
        disparity = ((float)Math.Sqrt((float)Math.Pow(query.x - caseToLook.x, 2) + 
            (float)Math.Pow(query.y - caseToLook.y, 2) + (float)Math.Pow(query.z - caseToLook.z,2))) / range;
        return 1 - disparity;
    }
    /// <summary>
    /// Se encarga de devolver, siendo 0 nada parecidos y 1 iguales, como de similares son las dos variables
    /// </summary>
    /// <param name="query">Variable 1</param>
    /// <param name="caseToLook">Variable 2</param>
    /// <param name="range">Rango para identificar como de significativa es la diferencia</param>
    /// <returns>Como de similar son las dos variables</returns>
    public static float computeV3ListManhattanSimilarity(in List<Vector3> query, in List<Vector3> caseToLook, float range)
    {
        float disparity = 0;
        int mostItems = Math.Max(caseToLook.Count, query.Count);
        int lessItems = Math.Min(caseToLook.Count, query.Count);
        for (int i = 0; i < lessItems; i++)
        {
            disparity += (1 - ((Math.Abs(query[i].x - caseToLook[i].x) + 
                Math.Abs(query[i].y - caseToLook[i].y) + Math.Abs(query[i].z - caseToLook[i].z)) / range)) / mostItems;
        }
        return 1 - disparity;
    }
    /// <summary>
    /// Se encarga de devolver, siendo 0 nada parecidos y 1 iguales, como de similares son las dos variables
    /// </summary>
    /// <param name="query">Variable 1</param>
    /// <param name="caseToLook">Variable 2</param>
    /// <param name="range">Rango para identificar como de significativa es la diferencia</param>
    /// <returns>Como de similar son las dos variables</returns>
    public static float computeV3ListEuclideanSimilarity(in List<Vector3> query, in List<Vector3> caseToLook, float range)
    {
        float disparity = 0;
        int mostItems = Math.Max(caseToLook.Count, query.Count);
        int lessItems = Math.Min(caseToLook.Count, query.Count);
        for (int i = 0; i < lessItems; i++)
        {
            disparity += (1 - (((float)Math.Sqrt((float)Math.Pow(query[i].x - caseToLook[i].x, 2) + 
                (float)Math.Pow(query[i].y - caseToLook[i].y, 2)+(float)Math.Pow(query[i].z - caseToLook[i].z,2) / range)))) / mostItems;
        }
        return 1 - disparity;
    }
    #endregion


}
