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
    public abstract Tuple<CaseCBRv2, float> computeSimilarity(in CaseCBRv2 query, in CaseCBRv2 caseToLook, Dictionary<string, float> weigths);
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

    ///// <summary>
    ///// Se encarga de crear la propiedad y sus getters y setters en la clase dinámicamente
    ///// </summary>
    ///// <param name="propertyName">Nombre de la propiedad</param>
    ///// <param name="propertyType">Tipo de la propiedad</param>
    //private void CreateProperty(string propertyName, Type propertyType)
    //{
    //    // Crea el fiedl
    //    FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
    //    // Crea el property asociado al field
    //    PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
    //    // Crea el metodo para el getter
    //    MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | 
    //        MethodAttributes.HideBySig, propertyType, Type.EmptyTypes); // Esto significa que no recibe nada
    //    ILGenerator getIl = getPropMthdBldr.GetILGenerator();

    //    getIl.Emit(OpCodes.Ldarg_0); // Carga la instancia de la clase
    //    getIl.Emit(OpCodes.Ldfld, fieldBuilder); // Carga el valor del field en la pila
    //    getIl.Emit(OpCodes.Ret); // Lo devuelve

    //    // Crea el método para el setter
    //    MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
    //          MethodAttributes.Public |
    //          MethodAttributes.SpecialName |
    //          MethodAttributes.HideBySig,
    //          null, new[] { propertyType }); // Esto significa que no deuvelve nada y que recibe un objeto del tipo de la propiedad

    //    ILGenerator setIl = setPropMthdBldr.GetILGenerator();
    //    Label modifyProperty = setIl.DefineLabel();
    //    Label exitSet = setIl.DefineLabel();

    //    setIl.MarkLabel(modifyProperty);  // Empieza a modificar el valor
    //    setIl.Emit(OpCodes.Ldarg_0); // Carga la instancia de la clase
    //    setIl.Emit(OpCodes.Ldarg_1); // Carga el nuevo valor recibido
    //    setIl.Emit(OpCodes.Stfld, fieldBuilder); // Mete el valor nuevo en el field

    //    setIl.Emit(OpCodes.Nop); // No hace nah
    //    setIl.MarkLabel(exitSet); // Acaba la modificación
    //    setIl.Emit(OpCodes.Ret);

    //    propertyBuilder.SetGetMethod(getPropMthdBldr);
    //    propertyBuilder.SetSetMethod(setPropMthdBldr);
    //}



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
    }
    /// <summary>
    /// Se encarga de comprobar si esa propiedad ya está creada, creandola si no, y asignandole un valor
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
    public void setAnswer(dynamic answer)
    {
        this.answer = answer;
    }
    public void setWeight(int weigth)
    {
        this.weight = weigth;
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
