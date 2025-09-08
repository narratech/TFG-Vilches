using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
public class CaseCBR
{
    Dictionary<string, dynamic> caseProperties;
    Dictionary<string, string> propertyTypes;
    dynamic answer = null;
    int weight;

    public CaseCBR()
    {
        caseProperties = new Dictionary<string, dynamic>();
        propertyTypes = new Dictionary<string, string>();
        answer = null;
        weight = 1;
        caseProperties["weight"] = 1;
        propertyTypes["weight"] = "float";
        caseProperties["answer"] = null;
        propertyTypes["answer"] = null; // ERROR: Si no has hecho el setType previo
        propertyTypes["timeStamp"] = "DateTime";
        caseProperties["timeStamp"] = DateTime.Now;
    }
    /// <summary>
    /// Se encarga de comprobar si esa propiedad ya está creada, creandola si no, y asignandole un valor
    /// El nombre de la propiedad deve tener el tipo nombre:tipo para que se serialicen correctamente después
    /// </summary>
    /// <param name="name">Nombre de la propiedad</param>
    /// <param name="value">Valor de la propiedad</param>
    public void setProperty(string name, dynamic value)
    {
        //if (!caseProperties.ContainsKey(name))
        //{
        //    CreateProperty(name, value.GetType());
        //}
        string[] separation = name.Split(":");
        caseProperties[separation[0]] = value;
        propertyTypes[separation[0]] = separation[1];
    }

    public dynamic getProperty(string name)
    {
        // ERROR: En caso de no tener esa propiedad.
        return caseProperties[name];
    }
    public dynamic getAnswer()
    {
        return answer;
    }
    /// <summary>
    /// Añade de que tipo es la respuesta para luego serialiazarla y deserializarla.
    /// El nombre tiene que coincidir con el escrito en los nuevos unserialize y deserialize
    /// </summary>
    /// <param name="nameType">String con el nombre del tipo de la respuesta</param>
    public void setAnswerType(string nameType)
    {
        this.propertyTypes["answer"] = nameType;
    }
    public void setAnswer(dynamic answer)
    {
        this.answer = answer;
        this.caseProperties["answer"] = answer;
    }
    public void setWeight(int weigth)
    {
        this.weight = weigth;
        this.caseProperties["weight"] = (float)weight;
    }
    public int getWeight()
    {
        return weight;
    }
    public List<string> getVariableNames()
    {
        List<string> names = new List<string>();
        foreach (string name in caseProperties.Keys) names.Add(name + ":" + propertyTypes[name]);
        return names;
    }
}
