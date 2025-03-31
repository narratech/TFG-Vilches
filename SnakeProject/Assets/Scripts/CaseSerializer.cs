using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO: Hacer un serializador interfaz que implemente el readcases y el writeCases
public class CaseSerializer
{
    #region staticUtility
    #region serializeVars
    public static string serializeVariable(float var)
    {
        return var.ToString();
    }
    public static string serializeVariable(bool var)
    {
        return var.ToString();
    }
    public static string serializeVariable(List<float> var)
    {
        return string.Join(" ", var);
    }
    public static string serializeVariable(List<bool> var)
    {
        return string.Join(" ", var);
    }
    public static string serializeVariable(Vector2 var)
    {
        float x = var.x;
        float y = var.y;

        return x + "/" + y; ;
    }
    public static string serializeVariable(Vector3 var)
    {
        float x = var.x;
        float y = var.y;
        float z = var.z;
        return x + "/" + y + "/" + z;
    }
    public static string serializeVariable(List<Vector2> var)
    {
        string serVar = "";
        foreach (Vector2 vec in var)
        {
            float x = vec.x;
            float y = vec.y;
            serVar += x + "/" + y + " ";
        }

        return serVar;
    }
    public static string serializeVariable(List<Vector3> var)
    {
        string serVar = "";
        foreach (Vector3 vec in var)
        {
            float x = vec.x;
            float y = vec.y;
            float z = vec.z;
            serVar += x + "/" + y + "/" + z + " ";
        }

        return serVar;
    }
    #endregion
    #region unserializeVars
    /// <summary>
    /// Devuelve un objeto del tipo y contenido que indicase el string type y el string var
    /// </summary>
    /// <param name="var">Variable que se quiere obtener del string</param>
    /// <param name="type">Tipo de la variable en la que se va a devolver</param>
    /// <returns></returns>
    public static dynamic unserializeVariable(string var, string type)
    {
        switch (type)
        {
            case "float":
                {
                    return float.Parse(var);
                }
            case "floatList":
                {
                    return Array.ConvertAll<string, float>(var.Split(" "), float.Parse).ToList<float>();
                }
            case "bool":
                {
                    return bool.Parse(var);
                }
            case "boolList":
                {
                    return Array.ConvertAll<string, bool>(var.Split(" "), bool.Parse).ToList<bool>();
                }
            case "vector2":
                {
                    float x = float.Parse(var.Split("/")[0]);
                    float y = float.Parse(var.Split("/")[1]);
                    return new Vector2(x, y);
                }
            case "vector2List":
                {
                    string[] vectors = var.Split(" ");
                    List<Vector2> vector2s = new List<Vector2>();
                    for (int j = 0; j < vectors.Length; j++)
                    {
                        float x = float.Parse(vectors[j].Split("/")[0]);
                        float y = float.Parse(vectors[j].Split("/")[1]);
                        vector2s.Add(new Vector2(x, y));
                    }
                    return vector2s;
                }
            case "vector3":
                {
                    float x = float.Parse(var.Split("/")[0]);
                    float y = float.Parse(var.Split("/")[1]);
                    float z = float.Parse(var.Split("/")[2]);
                    return new Vector3(x, y, z);
                }
            case "vector3List":
                {
                    string[] vectors = var.Split(" ");
                    List<Vector3> vector3s = new List<Vector3>();
                    for (int j = 0; j < vectors.Length; j++)
                    {
                        float x = float.Parse(vectors[j].Split("/")[0]);
                        float y = float.Parse(vectors[j].Split("/")[1]);
                        float z = float.Parse(vectors[j].Split("/")[2]);
                        vector3s.Add(new Vector3(x, y, z));
                    }
                    return vector3s;
                }
            default:
                {
                    return null;
                }
        }
    }
    #endregion
    #endregion
    CaseSerializer(string csvName)
    {

    }
}
