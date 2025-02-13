using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public enum DistanceMethod
{
    Manhattan,
    Euclidean
}
public class CaseCBR 
{
    // Pensando en hacerlo todo en un dicionario <string, Tuple<enumerador con la clase,Object>> para meterlo todo
    // En una sola estructura
    private Dictionary<string, float> similarityFloat = null;
    private Dictionary<string, List<float>> similarityFloatLists = null;

    private Dictionary<string, Tuple<DistanceMethod, UnityEngine.Vector3>> similarityVec3 = null;
    private Dictionary<string, Tuple<DistanceMethod, UnityEngine.Vector2>> similarityVec2 = null;
    private Dictionary<string, Tuple<DistanceMethod,List<UnityEngine.Vector3>>> similarityVec3List = null;
    private Dictionary<string, Tuple<DistanceMethod, List<UnityEngine.Vector2>>> similarityVec2List = null;

    private Dictionary<string, string> similarityStrings = null; //????

    private Dictionary<string,bool> similarityBool;
    private Dictionary<string,List<bool>> similarityBoolList;

    private dynamic answer; // Es dinamico porque no sabemos si sera un string, enumerador, int... Lo que el usuario quiera
    private float score; // Que el usuario decida como de bueno es el caso una vez utilizado según sus propias métricas

    public CaseCBR() 
    { 
        answer = null;
    }
    // Métodos para añadir a sus respectivas listas
    #region addToCase/setters
    public void addFloatToCase(float value, string name)
    {
        if(similarityFloat == null) similarityFloat = new Dictionary<string, float>();
        similarityFloat[name] = value;
    }

    public void addFloatListToCase(List<float> value, string name)
    {
        if (similarityFloatLists == null) similarityFloatLists = new Dictionary<string, List<float>>();
        similarityFloatLists[name] = value;
    }

    public void addVector2ToCase(UnityEngine.Vector2 value,DistanceMethod method, string name)
    {
        if (similarityVec2 == null) similarityVec2 = new Dictionary<string, Tuple<DistanceMethod, UnityEngine.Vector2>>();
        similarityVec2[name] = new Tuple<DistanceMethod,UnityEngine.Vector2>(method,value);
    }

    public void addVector3ToCase(UnityEngine.Vector3 value, DistanceMethod method, string name)
    {
        if (similarityVec3 == null) similarityVec3 = new Dictionary<string, Tuple<DistanceMethod, UnityEngine.Vector3>>();
        similarityVec3[name] = new Tuple<DistanceMethod, UnityEngine.Vector3>(method, value);
    }

    public void addVector2ListToCase(List<UnityEngine.Vector2> value, DistanceMethod method, string name)
    {
        if (similarityVec2List == null) similarityVec2List = new Dictionary<string, Tuple<DistanceMethod, List<UnityEngine.Vector2>>>();
        similarityVec2List[name] = new Tuple<DistanceMethod, List<UnityEngine.Vector2>>(method, value);
    }
    public void addVector3ListToCase(List<UnityEngine.Vector3> value, DistanceMethod method, string name)
    {
        if (similarityVec3List == null) similarityVec3List = new Dictionary<string, Tuple<DistanceMethod, List<UnityEngine.Vector3>>>();
        similarityVec3List[name] = new Tuple<DistanceMethod, List<UnityEngine.Vector3>>(method, value);
    }

    public void addBoolToCase(bool value,string name)
    {
        if (similarityBool == null) similarityBool = new Dictionary<string, bool>();
        similarityBool[name] = value;
    }
    public void addBoolListToCase(List<bool> value, string name)
    {
        if (similarityBoolList == null) similarityBoolList = new Dictionary<string, List<bool>>();
        similarityBoolList[name] = value;
    }

    public void setAnswer(dynamic value)
    {
        answer = value;
    }
    public void setScore(float value)
    {
        score = value;
    }
    #endregion

    #region getters
    public float getFloat(string name)
    {
        if (similarityFloat == null) throw new NullReferenceException("No existen elementos que mirar");
        else if(!similarityFloat.ContainsKey(name)) throw new ArgumentOutOfRangeException("El argumento no existe");
        else return similarityFloat[name];
    }

    public List<float> getFloatList(string name)
    {
        if (similarityFloatLists == null) throw new NullReferenceException("No existen elementos que mirar");
        else if (!similarityFloatLists.ContainsKey(name)) throw new ArgumentOutOfRangeException("El argumento no existe");
        else return similarityFloatLists[name];
    }

    public Tuple<DistanceMethod,UnityEngine.Vector2> getVector2(string name)
    {
        if (similarityVec2 == null) throw new NullReferenceException("No existen elementos que mirar");
        else if (!similarityVec2.ContainsKey(name)) throw new ArgumentOutOfRangeException("El argumento no existe");
        else return similarityVec2[name];
    }

    public Tuple<DistanceMethod, UnityEngine.Vector3> getVector3(string name)
    {
        if (similarityVec3 == null) throw new NullReferenceException("No existen elementos que mirar");
        else if (!similarityVec3.ContainsKey(name)) throw new ArgumentOutOfRangeException("El argumento no existe");
        else return similarityVec3[name];
    }

    public Tuple<DistanceMethod, List<UnityEngine.Vector2>> getVector2List(string name)
    {
        if (similarityVec2List == null) throw new NullReferenceException("No existen elementos que mirar");
        else if (!similarityVec2List.ContainsKey(name)) throw new ArgumentOutOfRangeException("El argumento no existe");
        else return similarityVec2List[name];
    }
    public Tuple<DistanceMethod, List<UnityEngine.Vector3>> getVector3List(string name)
    {
        if (similarityVec3List == null) throw new NullReferenceException("No existen elementos que mirar");
        else if (!similarityVec3List.ContainsKey(name)) throw new ArgumentOutOfRangeException("El argumento no existe");
        else return similarityVec3List[name];
    }

    public bool getBool(string name)
    {
        if (similarityBool == null) throw new NullReferenceException("No existen elementos que mirar");
        else if (!similarityBool.ContainsKey(name)) throw new ArgumentOutOfRangeException("El argumento no existe");
        else return similarityBool[name];
    }
    public bool getBoolList(string name)
    {
        if (similarityBool == null) throw new NullReferenceException("No existen elementos que mirar");
        else if (!similarityBool.ContainsKey(name)) throw new ArgumentOutOfRangeException("El argumento no existe");
        else return similarityBool[name];
    }

    public dynamic getAnswer()
    {
        return answer;
    }
    public float getScore()
    {
        return score;
    }

    #endregion
}
