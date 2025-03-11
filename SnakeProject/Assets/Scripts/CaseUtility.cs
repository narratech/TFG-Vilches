using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaseUtility
{
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

}
