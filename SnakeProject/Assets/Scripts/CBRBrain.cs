using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CBRBrain
{
    Queue<CaseCBR> caseToEvaluate;
    List<CaseCBR> caseToSave;
    List<CaseCBR> readedCases;
    string pathToCSV;
    CBRBrain(string path)
    {
        caseToSave = new List<CaseCBR>();
        readedCases = new List<CaseCBR>();
        StreamReader myReader = new StreamReader(Application.dataPath + path);
        string[] thisCaseDataNames = myReader.ReadLine().Split(',');
        // Lee y parsea los datos a casos
        parseCase(myReader);
        myReader.Close();
    }
    /// <summary>
    /// Se encarga de leer los cases del csv y convertirlos a casos de la logica
    /// </summary>
    /// <param name="myReader">Streamreader para leer</param>
    private void parseCase(StreamReader myReader)
    {
        CaseCBR myCase = new CaseCBR();
        string[] variablesTypes = myReader.ReadLine().Split(","); // Primera linea con los nombres y tipos de las variables
        while (!myReader.EndOfStream)
        {
            string[] values = myReader.ReadLine().Split(",");
            for (int i = 0; i < values.Length; i++) 
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
                            DistanceMethod distanceMethod = new DistanceMethod();
                            if (variablesTypes[i].Split(":")[2] == "E") distanceMethod = DistanceMethod.Euclidean;
                            else if (variablesTypes[i].Split(":")[2] == "M") distanceMethod = DistanceMethod.Manhattan;
                            float x = float.Parse(values[i].Split("/")[0]);
                            float y = float.Parse(values[i].Split("/")[1]);
                            myCase.addVector2ToCase(new Vector2(x, y), distanceMethod, name);
                            break;
                        }
                    case "vector2List":
                        {
                            DistanceMethod distanceMethod = new DistanceMethod();
                            if (variablesTypes[i].Split(":")[2] == "E") distanceMethod = DistanceMethod.Euclidean;
                            else if (variablesTypes[i].Split(":")[2] == "M") distanceMethod = DistanceMethod.Manhattan;

                            string[] vectors = values[i].Split(" ");
                            List<Vector2> vector2s = new List<Vector2>();
                            for (int j = 0; j < vectors.Length; j++)
                            {
                                float x = float.Parse(vectors[j].Split("/")[0]);
                                float y = float.Parse(vectors[j].Split("/")[1]);
                                vector2s.Add(new Vector2(x, y));
                            }

                            myCase.addVector2ListToCase(vector2s, distanceMethod, name);
                            break;
                        }
                    case "vector3":
                        {
                            DistanceMethod distanceMethod = new DistanceMethod();
                            if (variablesTypes[i].Split(":")[2] == "E") distanceMethod = DistanceMethod.Euclidean;
                            else if (variablesTypes[i].Split(":")[2] == "M") distanceMethod = DistanceMethod.Manhattan;
                            float x = float.Parse(values[i].Split("/")[0]);
                            float y = float.Parse(values[i].Split("/")[1]);
                            float z = float.Parse(values[i].Split("/")[2]);
                            myCase.addVector3ToCase(new Vector3(x, y, z), distanceMethod, name);
                            break;
                        }
                    case "vector3List":
                        {
                            DistanceMethod distanceMethod = new DistanceMethod();
                            if (variablesTypes[i].Split(":")[2] == "E") distanceMethod = DistanceMethod.Euclidean;
                            else if (variablesTypes[i].Split(":")[2] == "M") distanceMethod = DistanceMethod.Manhattan;

                            string[] vectors = values[i].Split(" ");
                            List<Vector3> vector3s = new List<Vector3>();
                            for (int j = 0; j < vectors.Length; j++)
                            {
                                float x = float.Parse(vectors[j].Split("/")[0]);
                                float y = float.Parse(vectors[j].Split("/")[1]);
                                float z = float.Parse(vectors[j].Split("/")[2]);
                                vector3s.Add(new Vector3(x, y, z));
                            }

                            myCase.addVector3ListToCase(vector3s, distanceMethod, name);
                            break;
                        }
                    case "Weigth":
                        myCase.setWeigth(int.Parse(values[i]));
                        break;
                }
            }
            readedCases.Add(myCase);
        }
    }
    //Pendiente, tengo que ver donde crearlo, al parecer Application.datapath es readOnly.
    private void writeCase()
    {
        StreamWriter
    }
}
