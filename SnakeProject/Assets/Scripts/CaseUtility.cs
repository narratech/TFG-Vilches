using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

// TODO: Mover cosas de aquí al case serializer
public class CaseUtility
{
    //Falta guardar respuesta al caso
    public static CaseCBR serializeCSVToCase(string[] variablesTypes, string[] values)
    {
        CaseCBR myCase = new CaseCBR();
        for (int i = 1; i < values.Length; i++) // Empieza en 1 porque el 0 es la id
        {
            string name = variablesTypes[i].Split(":")[0];
            string type = variablesTypes[i].Split(":")[1];
            // Se va a encargar el CaseSerializer
            //    case "Weigth":
            //        myCase.setWeigth(int.Parse(values[i]));
            //        break;
            //}
        }
        return myCase;
    }
    // Si se va a hacer con la calse dinamica, puedes hacer un delegate que llame a los métodos "get_"+NombreVariable, 
    // iterando por todos los nombres de variables y hacer un switch para cada tipo con los typeOf (en caso de enums, mirar si se puede con strings, si no, se hara con ints)
    public static string serializeCaseToCSV(CaseCBR myCase)
    {
        string myCaseParsed = "";
        List<string> variableNames = myCase.getVariableNames();
        for (int i = 0; i < variableNames.Count; i++) 
        {
            string name = variableNames[i].Split(":")[0];
            string type = variableNames[i].Split(":")[1];
            switch (type)
            {
                // Lo mismo, se va a encargar el CaseSerializer
                case "Weigth":
                    {
                        myCaseParsed += myCase.getWeight();
                        break;
                    }
            }
            myCaseParsed += ",";
        }
        return myCaseParsed;

    }


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
        for(int i=0;i<query.Count;i++)
        {
            disparity += (1 - (Math.Abs(query[i] - caseToLook[i]) / range)) / query.Count;
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
        for (int i = 0; i < query.Count; i++)
        {
            disparity += (1 - (query[i] == caseToLook[i] ? 1 : 0)) / query.Count;
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
        for (int i = 0; i < query.Count; i++)
        {
            disparity += 1 - ((Math.Abs(query[i].x - caseToLook[i].x) + Math.Abs(query[i].y - caseToLook[i].y)) / range)/query.Count;
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
        for (int i = 0; i < query.Count; i++)
        {
            disparity += 1- (((float)Math.Sqrt((float)Math.Pow(query[i].x - caseToLook[i].x, 2) + (float)Math.Pow(query[i].y - caseToLook[i].y, 2))) / range)/query.Count;
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
        for (int i = 0; i < query.Count; i++)
        {
            disparity += 1 - ((Math.Abs(query[i].x - caseToLook[i].x) + 
                Math.Abs(query[i].y - caseToLook[i].y) + Math.Abs(query[i].z - caseToLook[i].z)) / range) / query.Count;
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
        for (int i = 0; i < query.Count; i++)
        {
            disparity += 1 - (((float)Math.Sqrt((float)Math.Pow(query[i].x - caseToLook[i].x, 2) + 
                (float)Math.Pow(query[i].y - caseToLook[i].y, 2)+(float)Math.Pow(query[i].z - caseToLook[i].z,2) / range))) / query.Count;
        }
        return 1 - disparity;
    }
    #endregion


}
