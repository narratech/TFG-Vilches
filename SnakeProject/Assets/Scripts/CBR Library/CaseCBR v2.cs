using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor.Compilation;
using UnityEngine;

/// <summary>
/// Interfaz que se tiene que implementar a gusto del usuario para decidir como quiere comparar sus casos y contabilizar
/// la similitud (este solo mira similitud)
/// </summary>
public interface CaseComparer
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
    public abstract CaseWithSimilarity computeSimilarity(in CaseCBRv2 query, in CaseCBRv2 caseToLook, Dictionary<string, float> weigths);
}
public class CaseCBRv2
{
    // DUDA: PUEDE QUE ACABE HACIENDOLO DYNAMIC, GETTERS Y SETTERS SIN NECESIDAD DE REFLEXIÓN
    private TypeBuilder typeBuilder;
    dynamic myRealCase; // La instancia de la nueva clase

    Dictionary<string, dynamic> caseProperties;
    Dictionary<string, string> propertyTypes;
    dynamic answer = null;
    int weight;

    public CaseCBRv2()
    {
        //AssemblyName assemblyName = new AssemblyName("RealCaseCBR");
        //System.Reflection.Emit.AssemblyBuilder assBuilder = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

        //ModuleBuilder moduleBuilder = assBuilder.DefineDynamicModule("RealCaseCBR");
        // typeBuilder = moduleBuilder.DefineType(assemblyName.FullName
        //                      , TypeAttributes.Public |
        //                      TypeAttributes.Class |
        //                      TypeAttributes.AutoClass |
        //                      TypeAttributes.AnsiClass |
        //                      TypeAttributes.BeforeFieldInit |
        //                      TypeAttributes.AutoLayout
        //                      , null); // DUDA: null es el padre. Puedo hacer que herede de otra clase que implemente el answer y weight
        //typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName 
        //    | MethodAttributes.RTSpecialName);
        caseProperties = new Dictionary<string, dynamic>();
        propertyTypes = new Dictionary<string, string>();
        answer = null;
        weight = 1;
        caseProperties["weight"] = 1;
        propertyTypes["weight"] = "float";
        caseProperties["answer"] = null;
        propertyTypes["answer"] = null; // ERROR: Si no has hecho el setType previo
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
