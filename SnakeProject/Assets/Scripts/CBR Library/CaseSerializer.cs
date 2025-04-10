using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class CaseSerializer
{
    #region serializeVars
    // AVISO
    // Para añadir escritura de tipos no soportados, basta con agregarle un método
    // serializeVariable(tipo consumido) y la persistencia se hace sola. El formato de serialización debe ser igual que el de deserializacion
    public virtual string serializeVariable(dynamic var)
    {
        Type tipo = var.GetType();

        if (tipo == typeof(int) || tipo == typeof(float))
        {
            return var.ToString();
        }
        else if (tipo == typeof(bool))
        {
            return var.ToString();
        }
        else if (tipo == typeof(List<int>) || tipo == typeof(List<float>))
        {
            return string.Join(" ", var);
        }
        else if (tipo == typeof(List<bool>))
        {
            return string.Join(" ", var);
        }
        else if (tipo == typeof(Vector2))
        {
            float x = var.x;
            float y = var.y;
            return x + "/" + y;
        }
        else if (tipo == typeof(Vector3))
        {
            float x = var.x;
            float y = var.y;
            float z = var.z;
            return x + "/" + y + "/" + z;
        }
        else if (tipo == typeof(List<Vector2>))
        {
            string serVar = "";
            int i = 0;
            foreach (Vector2 vec in var)
            {
                float x = vec.x;
                float y = vec.y;
                serVar += x + "/" + y;
                if(i<var.Count-1) serVar += " ";
                i++;
            }

            return serVar;
        }
        else if (tipo == typeof(List<Vector3>))
        {
            string serVar = "";
            int i = 0;
            foreach (Vector3 vec in var)
            {
                float x = vec.x;
                float y = vec.y;
                float z = vec.z;
                serVar += x + "/" + y + "/" + z;
                if (i < var.Count - 1) serVar += " ";
                i++;
            }

            return serVar;
        }
        else
        {
            return null;
        }
    }
    #endregion
    #region unserializeVars
    /// <summary>
    /// AVISO
    /// Devuelve un objeto del tipo y contenido que indicase el string type y el string var. Para tipos no soportados, overridear este metodo
    /// y el parseo de CSV a caso se hace solo.
    /// </summary>
    /// <param name="var">Variable que se quiere obtener del string</param>
    /// <param name="type">Tipo de la variable en la que se va a devolver</param>
    /// <returns>La variable en el tipo correcto</returns>
    public virtual dynamic unserializeVariable(string var, string type)
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
                    // ERROR: A lo mejor devolver un error?
                    return null;
                }
        }
    }
    #endregion
    #region caseSerialzier
    #region private
    private List<CaseCBRv2> caseList;
    private int casesCount;
    #endregion

    public CaseSerializer()
    {
        caseList = new List<CaseCBRv2>();
    }
    /// <summary>
    /// Metodo virtual de lectura de casos desde CSV. Tiene por defecto una implementación básica pero para lecturas más complejas
    /// como enumerators o clases propias, hace falta crear una instancia que herede de esta e implementar el readCases
    /// </summary>
    /// <param name="csvName">Nombre del CSV a leer</param>
    /// <param name="readedCases">Lista de casos en los que guardar los leidos</param>
    public virtual void readCases(string csvName, ref List<CaseCBRv2> readedCases)
    {
        if (File.Exists("CaseBase/" + csvName+".csv")) // Si existe una base de casos, leelos
        {
            StreamReader myReader = new StreamReader("CaseBase/" + csvName+".csv");
            // Lee y parsea los datos a casos
            caseList = readedCases;
            string[] variablesTypes = myReader.ReadLine().Split(","); // Primera linea con los nombres y tipos de las variables
            while (!myReader.EndOfStream)
            {
                string[] values = myReader.ReadLine().Split(",");
                readedCases.Add(serializeCSVToCase(variablesTypes, values));
            }
            myReader.Close();
        }
        casesCount = readedCases.Count;
    }
    /// <summary>
    /// Se encarga de escribir los casos al CSV. Para añadir escritura de tipos no soportados, basta con agregarle un método
    /// serializeVariable(tipo consumido) y la persistencia se hace sola. El formato de serialización debe ser igual que el de deserializacion
    /// </summary>
    /// <param name="CSVname"></param>
    /// <param name="caseToSave"></param>
    public virtual void writeCases(string CSVname, List<CaseCBRv2>caseToSave)
    {
        bool existedBefore = true;
        int id = 0;
        if (!Directory.Exists("CaseBase")) Directory.CreateDirectory("CaseBase");
        if (!File.Exists("CaseBase/" + CSVname +".csv")) existedBefore = false;
        else id = casesCount; //Las id de los nuevos casos que no estan escritos

        StreamWriter myWriter = new StreamWriter("CaseBase/" + CSVname + ".csv", true);
        if (!existedBefore)
        {
            string names = "id," + string.Join(",", caseToSave[0].getVariableNames());
            myWriter.WriteLine(names); //En caso de que no existiese, la primera linea es para nombres
        }
        foreach (CaseCBRv2 myCase in caseToSave) // Escribe los nuevos casos
        {
            myWriter.WriteLine(id + "," + serializeCaseToCSV(myCase));
            id++;
        }

        myWriter.Close();
    }
    /// <summary>
    /// Es un método para la lectura de casos por defecto. Genera casos leidos desde csv. 
    /// Se puede implementar de forma diferente en una clase heredada
    /// </summary>
    /// <param name="variablesTypes">Nombre de las variables del caso</param>
    /// <param name="values">Valores del caso para estas variables</param>
    /// <returns>El caso formado</returns>
    public virtual CaseCBRv2 serializeCSVToCase(string[] variablesTypes, string[] values)
    {
        //ERROR: Si recibe tipo no soportado, sacar excepcion
        CaseCBRv2 myCase = new CaseCBRv2();
        for (int i = 1; i < values.Length; i++) // Empieza en 1 porque el 0 es la id
        {
            string name = variablesTypes[i].Split(":")[0];
            string type = variablesTypes[i].Split(":")[1];
            if (name != "answer" && name != "weight") myCase.setProperty(variablesTypes[i], unserializeVariable(values[i], type));
            else if (name == "answer") myCase.setAnswer(unserializeVariable(values[i], type));
            else if (name == "weight") myCase.setWeight((int)unserializeVariable(values[i], type));
            else return null;
        }
        return myCase;
    }

    public virtual string serializeCaseToCSV(CaseCBRv2 myCase)
    {
        string myCaseParsed = "";
        List<string> variableNames = myCase.getVariableNames();
        for (int i = 0; i < variableNames.Count; i++)
        {
            string name = variableNames[i].Split(":")[0];
            string type = variableNames[i].Split(":")[1];
            myCaseParsed += serializeVariable(myCase.getProperty(name));
            if(i < variableNames.Count - 1)myCaseParsed += ",";
        }
        return myCaseParsed;

    }
    #endregion
}
