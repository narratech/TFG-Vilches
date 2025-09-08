using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Interfaz que se tiene que implementar a gusto del usuario para decidir como quiere comparar sus casos y contabilizar
/// la similitud (este solo mira similitud)
/// </summary>
public interface ICaseComparer
{
    /// <summary>
    /// Interfaz que se tiene que implementar a gusto del usuario para decidir como quiere comparar sus casos y contabilizar
    /// la similitud (Si quiere contar todo en la similitud o si decide que algun parametro es importante.
    /// Este resultado determinará el orden de los KNN casos recogidos
    /// </summary>
    /// <param name="query">El caso que se presenta</param>
    /// <param name="caseToLook">El caso a comparar</param>
    /// <param name="weigths">Diccionario con los pesos de cada variable, asignados en el cbrBrain</param>
    /// <returns>La tupla con el caso a comparar y su similitud con el presentado</returns>
    public abstract CaseWithSimilarity computeSimilarity(in CaseCBR query, in CaseCBR caseToLook, Dictionary<string, float> weigths);
}
