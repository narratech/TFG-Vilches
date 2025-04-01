using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public enum ReviseType
{
    alwaysRetain,custom
}
public enum reuseAnswerType
{
    mostSimilar, mostVoted, weighted
}
public class CBRBrain
{
    CaseSerializer caseSerializer; // Va a hacer falta para leer y guardar los archivos
    Queue<CaseCBRv2> casesToEvaluate;
    List<CaseCBRv2> caseToSave;
    List<CaseCBRv2> readedCases;

    List<CaseCBRv2> possibleTwins;
    CaseFitness fitness;
    CaseComparer comparer;
    Func<System.Object[], bool> myFunc;
    System.Object[] myFuncArgs;

    Dictionary<string, float> weights;
    int kNNRequired;
    bool evaluateNextCase;
    float similThresh;

    ///<summary>
    /// Crea el CBRBrain necesario para funcionar
    /// </summary>
    /// <param name="CSVname">Nombre de la base de datos</param>
    /// <param name="myFintess">Funcion para decidir que caso es mejor</param>
    /// <param name="similarityThreshold">Entre 0 y 1, como de similar tienen que ser los casos para que no se guarden</param>
    /// <param name="kNNRequired">Numero de casos que se recuperan para la respuesta</param>
    /// <param name="myComparer">Comparador para decidir como se computa la similiritud de los casos</param>
    /// <param name="myFunc">Función lambda para la revisión custom. Puede ser null si se va con la por defecto</param>
    /// <param name="myFuncArgs"> Argumentos necesarios para la función lamda. Puede ser null si esta no los necesita</param>gs">
    public CBRBrain(string CSVname,float similarityThreshold,CaseComparer myComparer = null, int kNNRequired = 1, CaseFitness myFintess = null,
         Func<System.Object[], bool> myFunc =null, System.Object[] myFuncArgs = null)
    {
        caseToSave = new List<CaseCBRv2>();
        readedCases = new List<CaseCBRv2>();
        caseSerializer = new CaseSerializer();
        fitness = myFintess;
        comparer = myComparer;
        caseSerializer.readCases(CSVname, ref readedCases);
        this.kNNRequired = kNNRequired;
        this.similThresh = similarityThreshold;
        this.myFunc = myFunc;
        this.myFuncArgs = myFuncArgs;
    }
    #region public
    /// <summary>
    /// Añade un valor al peso de una caracteristica y se normaliza
    /// </summary>
    /// <param name="key">Nombre de la caracteristica a la que se asocia el peso</param>
    /// <param name="w">Valor del peso (entre 0 y 1, preferiblemente todos los pesos suman 1)</param>
    public void setWeigth(string key, float w)
    {
        weights[key] = w;
        float sum = 0;
        Dictionary<string,float>.Enumerator it = weights.GetEnumerator();
        while (it.MoveNext())
        {
            sum += it.Current.Value;
        }
        it = weights.GetEnumerator();
        while (it.MoveNext())
        {
            weights[it.Current.Key] = weights[it.Current.Key] / sum; // Se normalizan los pesos
        }
    }
    /// <summary>
    /// Se llama cuando el usuario considere que se tiene que evaluar el siguiente caso de la cola de evaluacion
    /// </summary>
    /// <param name="evaluate">Si se tiene que evaluar</param>
    public void setEvaluateNextCase(bool evaluate)
    {
        evaluateNextCase = evaluate;
    }
    #region CBRModules

    /// <summary>
    /// Se encarga de comparar los casos leidos con el query y devolver los kNN Casos más prometedores
    /// (Parecidos o mejores, según ponderación del usuario)
    /// </summary>
    /// <param name="query">Caso presentado al sistema</param>
    /// <returns>Los kNN casos más prometedores, ordenados de más a menos prometedor</returns>
    public SortedSet<Tuple<CaseCBRv2, float>> retrieveKNNCases(in CaseCBRv2 query)
    {
        SortedSet<Tuple<CaseCBRv2, float>> kNNCases = new SortedSet<Tuple<CaseCBRv2, float>>(fitness); // Ordenas por fitness
        foreach(CaseCBRv2 myCase in readedCases) 
        {
            Tuple<CaseCBRv2, float> myCaseWithSimil = comparer.computeSimilarity(query, myCase, weights);
            if (kNNCases.Count < kNNRequired)
            {
                kNNCases.Add(myCaseWithSimil);
            }
            else
            {
                IEnumerator iterator = kNNCases.Reverse().GetEnumerator();
                iterator.MoveNext(); // El elemento menos parecido o menos valioso

                // Si el nuevo caso visto es mejor que el peor, borra el peor guardado y mete el nuevo
                if (((Tuple<CaseCBRv2, float>)iterator.Current).Item2 < myCaseWithSimil.Item2) 
                {
                    kNNCases.Remove((Tuple<CaseCBRv2, float>)iterator.Current);
                    kNNCases.Add(myCaseWithSimil);
                }
            }
            if(myCaseWithSimil.Item2 >= similThresh) possibleTwins.Add(myCaseWithSimil.Item1); // Si son muy parecidos, mira despues
        }
        return kNNCases;
    }
    /// <summary>
    /// Se elige el método de selección para ver cómo se elige la respuesta que se va a usar.
    /// Los tres modos son: 
    /// Usar el más similar
    /// Voto de la mayoría
    /// Por pesos
    /// </summary>
    /// <param name="knnCases">Los casos más prometedores de los que escoger</param>
    /// <param name="query">El caso presentado que se va a generar</param>
    /// <returns>El resultado a utilizar en el juego</returns>
    public  dynamic reuseCases(in SortedSet<Tuple<CaseCBRv2, float>> knnCases, ref CaseCBRv2 query, reuseAnswerType type)
    {
        
        if(type == reuseAnswerType.mostVoted)
        {
            Dictionary <System.Object, int> votes = new Dictionary<object, int>();
            Tuple<dynamic, int> answer = null;
            foreach(Tuple<CaseCBRv2, float> myCase in knnCases)
            {
                if (!votes.ContainsKey(query.getAnswer()))
                {
                    votes.Add(myCase.Item1.getAnswer(), 1);
                    if(answer == null) answer = new Tuple<dynamic,int>(myCase.Item1.getAnswer(),1);
                }
                else
                {
                    votes[myCase.Item1.getAnswer()]++;
                    if(answer.Item2 < votes[myCase.Item1.getAnswer()]) answer = new Tuple<dynamic, int>(myCase.Item1.getAnswer(), 1);
                }
            }
            query.setAnswer(answer.Item1);
            return answer.Item1;
        }
        else if(type == reuseAnswerType.weighted)
        {
            Dictionary<System.Object, int> votes = new Dictionary<object, int>();
            Tuple<dynamic, int> answer = null;
            foreach (Tuple<CaseCBRv2, float> myCase in knnCases)
            {
                if (!votes.ContainsKey(query.getAnswer()))
                {
                    votes.Add(myCase.Item1.getAnswer(), myCase.Item1.getWeight());
                    if (answer == null) answer = new Tuple<dynamic, int>(myCase.Item1.getAnswer(), myCase.Item1.getWeight());
                }
                else
                {
                    votes[myCase.Item1.getAnswer()]+= myCase.Item1.getWeight();
                    if (answer.Item2 < votes[myCase.Item1.getAnswer()]) 
                        answer = new Tuple<dynamic, int>(myCase.Item1.getAnswer(), votes[myCase.Item1.getAnswer()]);
                }
            }
            query.setAnswer(answer.Item1);
            return answer.Item1;
        }
        else
        {
            IEnumerator iterator = knnCases.GetEnumerator();
            iterator.MoveNext(); // El elemento más prometedor 
            query.setAnswer(((Tuple<CaseCBR, float>)iterator.Current).Item1.getAnswer());
            return ((Tuple<CaseCBR, float>)iterator.Current).Item1.getAnswer(); // Supongo que, al ser en tiempo de ejecucion, esto se resolvera solo
        }
    }
// PENDIENTE DE REVISIÓN
/// <summary>
/// Recibe un enum con el tipo de revisión que se quiere hacer y la función de revisión cómo expresión lamda introducida
/// por el usuario, que debe devolver un booleano indicando si el caso es útil y se guarda o no
/// Por defecto, siempre guarda el caso
/// </summary>
/// <param name="type">Enumerador indicando el tipo de revisión</param>
/// <returns></returns>
    public bool reviseCase(ReviseType type)
    {
        if (type == ReviseType.custom)
        {
            //ERROR: myFunc es null
            bool save = myFunc(myFuncArgs);
            if(!save) possibleTwins.Clear(); // Si no lo vas a guardar, no te interesa saber si ya hay en la base de datos
            return save;
        }
        else return true;
    }
    //TODO: Revisar si dos casos son muy iguales (estoy guardando los posibles gemelos en una variable de la clase que
    // se vacia en cada caso y que se rellena en el retrieve knn
    /// <summary>
    /// Añade el caso a la lista de casos por guardar y también a la base de datos para usar
    /// DUDA: No se si hacer la lista de tamaño fijo e ir escribiendo casos periodicamente (flush) o escribirlos todos al final
    /// BASE: GUARDA AL FINAL
    /// UTILIDAD: GUARDAR CON UN BOTON
    /// </summary>
    /// <param name="myCase">Caso para guardar</param>
    ///<param name="similarityThreshold">Limite de similaridad con el más parecido para guardar o sumar peso al previo</param>
    public void retainCases(in CaseCBRv2 myCase)
    {
        if (possibleTwins.Count != 0)
        {
            bool matchFound = false;
            int i = 0;
            while(!matchFound && i < possibleTwins.Count)
            {
                if (possibleTwins[i].getAnswer() == myCase.getAnswer()) // Si has encontrado un gemelo, sumale peso en vez de guardarlo
                {
                    matchFound = true;
                    CaseCBRv2 match = readedCases.Find(x => x == possibleTwins[i]);
                    match.setWeight(match.getWeight()+1);
                }
                i++;
            }
            if (!matchFound)
            {
                caseToSave.Add(myCase);
                readedCases.Add(myCase);
            }
        }
        else
        {
            caseToSave.Add(myCase);
            readedCases.Add(myCase);
        }
    }
    #endregion
    /// <summary>
    /// Se encarga de hacer el ciclo cbr cada vez que se llama
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public dynamic CBRCycle(CaseCBRv2 query)
    {
        SortedSet<Tuple<CaseCBRv2, float>> caseSimil = retrieveKNNCases(query);
        dynamic res = reuseCases(caseSimil, ref query,reuseAnswerType.mostSimilar);
        casesToEvaluate.Enqueue(query);
        if(evaluateNextCase) // TODO: Hacer manejo de errores si no hay más que evaluar
        {
            CaseCBRv2 caseToEvaluate;
            if(casesToEvaluate.TryDequeue(out caseToEvaluate))
            {
                if(reviseCase(caseToEvaluate, COMPARADOR ???))
                {
                    retainCases(in caseToEvaluate, 0.95f);
                    evaluateNextCase = false;
                }
            }
        }
    }
    #endregion
}
