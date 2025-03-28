using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaseUtility
{
    //Falta guardar respuesta al caso
    public static CaseCBR parseCSVToCase(string[] variablesTypes, string[] values)
    {
        CaseCBR myCase = new CaseCBR();
        for (int i = 1; i < values.Length; i++) // Empieza en 1 porque el 0 es la id
        {
            string name = variablesTypes[i].Split(":")[0];
            string type = variablesTypes[i].Split(":")[1];
            switch (type)
            {
                case "float":
                    {
                        myCase.addFloatToCase(float.Parse(values[i]), name);
                        break;
                    }
                case "floatList":
                    {
                        List<float> list = Array.ConvertAll<string, float>(values[i].Split(" "), float.Parse).ToList<float>();
                        myCase.addFloatListToCase(list, name);
                        break;
                    }
                case "bool":
                    {
                        myCase.addBoolToCase(bool.Parse(values[i]), name);
                        break;
                    }
                case "boolList":
                    {
                        List<bool> list = Array.ConvertAll<string, bool>(values[i].Split(" "), bool.Parse).ToList<bool>();
                        myCase.addBoolListToCase(list, name);
                        break;
                    }
                case "vector2":
                    {
                        float x = float.Parse(values[i].Split("/")[0]);
                        float y = float.Parse(values[i].Split("/")[1]);
                        myCase.addVector2ToCase(new Vector2(x, y), name);
                        break;
                    }
                case "vector2List":
                    {
                        string[] vectors = values[i].Split(" ");
                        List<Vector2> vector2s = new List<Vector2>();
                        for (int j = 0; j < vectors.Length; j++)
                        {
                            float x = float.Parse(vectors[j].Split("/")[0]);
                            float y = float.Parse(vectors[j].Split("/")[1]);
                            vector2s.Add(new Vector2(x, y));
                        }

                        myCase.addVector2ListToCase(vector2s, name);
                        break;
                    }
                case "vector3":
                    {
                        float x = float.Parse(values[i].Split("/")[0]);
                        float y = float.Parse(values[i].Split("/")[1]);
                        float z = float.Parse(values[i].Split("/")[2]);
                        myCase.addVector3ToCase(new Vector3(x, y, z), name);
                        break;
                    }
                case "vector3List":
                    {
                        string[] vectors = values[i].Split(" ");
                        List<Vector3> vector3s = new List<Vector3>();
                        for (int j = 0; j < vectors.Length; j++)
                        {
                            float x = float.Parse(vectors[j].Split("/")[0]);
                            float y = float.Parse(vectors[j].Split("/")[1]);
                            float z = float.Parse(vectors[j].Split("/")[2]);
                            vector3s.Add(new Vector3(x, y, z));
                        }

                        myCase.addVector3ListToCase(vector3s, name);
                        break;
                    }
                case "Weigth":
                    myCase.setWeigth(int.Parse(values[i]));
                    break;
            }
        }
        return myCase;
    }
    // Si se va a hacer con la calse dinamica, puedes hacer un delegate que llame a los métodos "get_"+NombreVariable, 
    // iterando por todos los nombres de variables y hacer un switch para cada tipo con los typeOf (en caso de enums, mirar si se puede con strings, si no, se hara con ints)
    public static string parseCaseToCSV(CaseCBR myCase)
    {
        string myCaseParsed = "";
        List<string> variableNames = myCase.getVariableNames();
        for (int i = 0; i < variableNames.Count; i++) 
        {
            string name = variableNames[i].Split(":")[0];
            string type = variableNames[i].Split(":")[1];
            switch (type)
            {
                case "float":
                    {
                        myCaseParsed += myCase.getFloat(name);
                        break;
                    }
                case "floatList":
                    {
                        myCaseParsed += myCase.getFloatList(name);
                        break;
                    }
                case "bool":
                    {
                        myCaseParsed += myCase.getBool(name);
                        break;
                    }
                case "boolList":
                    {
                        myCaseParsed += myCase.getBoolList(name);
                        break;
                    }
                case "vector2":
                    {
                        float x = myCase.getVector2(name).x;
                        float y = myCase.getVector2(name).y;
                        myCaseParsed += x+"/"+y;
                        break;
                    }
                case "vector2List":
                    {
                        List<Vector2> myList = myCase.getVector2List(name);
                        foreach(Vector2 vec in myList)
                        {
                            float x = vec.x;
                            float y = vec.y;
                            myCaseParsed += x + "/" + y + " ";
                        }
                        break;
                    }
                case "vector3":
                    {
                        float x = myCase.getVector3(name).x;
                        float y = myCase.getVector3(name).y;
                        float z = myCase.getVector3(name).z;
                        myCaseParsed += x + "/" + y + "/" + z;
                        break;
                    }
                case "vector3List":
                    {
                        List<Vector3> myList = myCase.getVector3List(name);
                        foreach (Vector3 vec in myList)
                        {
                            float x = vec.x;
                            float y = vec.y;
                            float z = vec.z;
                            myCaseParsed += x + "/" + y + "/"+ z + " ";
                        }
                        break;
                    }
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
    /// <summary>
    /// DUDA: La idea es que sea el usuario el que decida como quiere ponderar la similitud de sus casos, a lo mejor un método interfaz????
    /// HACER SIMILITUD STANDARD CON EUCLIDEA Y MANHATTAN Y QUE ESTA RECIBA UNA LAMBDA PARA DAR LIBERTAD DE DECIDIR
    /// DUDA: Cómo determino cómo de significativo es la diferencia o igualdad.
    /// </summary>
    /// <param name="query">El caso que se presenta</param>
    /// <param name="caseToLook">El caso a comparar</param>
    /// <param name="weigths">Diccionario con los pesos de cada variable, asignados en el cbrBrain</param>
    /// <returns>La tupla con el caso a comparar y su similitud con el presentado</returns>
    public static Tuple<CaseCBR, float> computeSimilarity(in CaseCBR query, in CaseCBR caseToLook, Dictionary<string, float> weigths)
    {
        Tuple<CaseCBR, float> myTuple = new Tuple<CaseCBR, float>(caseToLook, 0);
        List<string> variableNames = query.getVariableNames();
        //TODO
        return myTuple;
    }
    private static float computeManhattanSimilarity(in CaseCBR query, in CaseCBR caseToLook, Dictionary<string, float> weigths, Dictionary<string,float>distanceRange)
    {

    }
        

}
