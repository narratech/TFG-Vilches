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
        else if(tipo == typeof(DateTime))
        {
            return var.ToString();
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
            case "DateTime":
                {
                    return DateTime.Parse(var);
                }
            default:
                {
                    return null;
                }
        }
    }
    #endregion
    #region caseSerialzier
    #region private
    private List<CaseCBR> caseList;
    private int casesCount;
    StreamReader myReader;
    StreamWriter myWriter;
    #endregion

    public CaseSerializer()
    {
        caseList = new List<CaseCBR>();
    }
    ~CaseSerializer()
    {
        if(myReader != null) myReader.Close();
        if(myWriter != null) myWriter.Close();
    }
    /// <summary>
    /// Metodo virtual de lectura de casos desde CSV. Tiene por defecto una implementación básica pero para lecturas más complejas
    /// como enumerators o clases propias, hace falta crear una instancia que herede de esta e implementar el readCases
    /// </summary>
    /// <param name="csvName">Nombre del CSV a leer</param>
    /// <param name="readedCases">Lista de casos en los que guardar los leidos</param>
    public virtual void readCases(string csvName, ref List<CaseCBR> readedCases)
    {
        string filePath = Application.persistentDataPath + "/CaseBase/" + csvName + ".csv";

        if (File.Exists(filePath)) // Si existe una base de casos, leelos
        {
            using (StreamReader myReader = new StreamReader(filePath))
            {
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
        }
        casesCount = readedCases.Count;
    }
    /// <summary>
    /// Se encarga de escribir los casos al CSV. Para añadir escritura de tipos no soportados, basta con agregarle un método
    /// serializeVariable(tipo consumido) y la persistencia se hace sola. El formato de serialización debe ser igual que el de deserializacion
    /// </summary>
    /// <param name="CSVname">Nombre de la base de casos</param>
    /// <param name="caseToSave">Casos para escribir</param>
    /// <param name="ignoreExisting">Ignora si existia antes una base de casos</param>
    public virtual void writeCases(string CSVname, List<CaseCBR>caseToSave, bool ignoreExisting = false)
    {
        string filePath = Application.persistentDataPath + "/CaseBase/" + CSVname + ".csv";

        bool existedBefore = true;
        int id = 0;
        if (!Directory.Exists(Application.persistentDataPath + "/CaseBase")) Directory.CreateDirectory(Application.persistentDataPath + "/CaseBase");
        if (!File.Exists(filePath) || ignoreExisting) existedBefore = false;
        else if(existedBefore) id = casesCount; //Las id de los nuevos casos que no estan escritos

        using (StreamWriter myWriter = new StreamWriter(filePath, existedBefore))
        {
            if (!existedBefore)
            {
                string names = "id," + string.Join(",", caseToSave[0].getVariableNames());
                myWriter.WriteLine(names); //En caso de que no existiese, la primera linea es para nombres
            }
            foreach (CaseCBR myCase in caseToSave) // Escribe los nuevos casos
            {
                myWriter.WriteLine(id + "," + serializeCaseToCSV(myCase));
                id++;
            }

            myWriter.Close();
        }
    }
    /// <summary>
    /// Es un método para la lectura de casos por defecto. Genera casos leidos desde csv. 
    /// Se puede implementar de forma diferente en una clase heredada
    /// </summary>
    /// <param name="variablesTypes">Nombre de las variables del caso</param>
    /// <param name="values">Valores del caso para estas variables</param>
    /// <returns>El caso formado</returns>
    public virtual CaseCBR serializeCSVToCase(string[] variablesTypes, string[] values)
    {
        //ERROR: Si recibe tipo no soportado, sacar excepcion
        CaseCBR myCase = new CaseCBR();
        for (int i = 1; i < values.Length; i++) // Empieza en 1 porque el 0 es la id
        {
            string name = variablesTypes[i].Split(":")[0];
            string type = variablesTypes[i].Split(":")[1];
            if (name != "answer" && name != "weight") myCase.setProperty(variablesTypes[i], unserializeVariable(values[i], type));
            else if (name == "answer")
            {
                myCase.setAnswer(unserializeVariable(values[i], type));
                myCase.setAnswerType(type);
            }
            else if (name == "weight") myCase.setWeight((int)unserializeVariable(values[i], type));
            else return null;
        }
        return myCase;
    }

    public virtual string serializeCaseToCSV(CaseCBR myCase)
    {
        string myCaseParsed = "";
        List<string> variableNames = myCase.getVariableNames();
        for (int i = 0; i < variableNames.Count; i++)
        {
            if (variableNames[i]==null)
            {
                Debug.Log("Error");
            }
            string name = variableNames[i].Split(":")[0];
            string type = variableNames[i].Split(":")[1];
            myCaseParsed += serializeVariable(myCase.getProperty(name));
            if(i < variableNames.Count - 1)myCaseParsed += ",";
        }
        return myCaseParsed;

    }
    #endregion
}
