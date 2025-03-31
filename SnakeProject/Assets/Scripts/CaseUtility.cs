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
