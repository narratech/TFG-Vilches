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

    ReviseType reviseType;
    reuseAnswerType reuseAnswer;
    Func<CaseCBRv2,CaseCBRv2,System.Object[], bool> myFunc;
    System.Object[] myFuncArgs;

    Dictionary<string, float> weights;
    int kNNRequired;
    bool evaluateNextCase;
    bool normalizedWeights;
    float similThresh;
    string CSVname;

    ///<summary>
    /// Crea el CBRBrain necesario para funcionar
    /// </summary>
    /// <param name="CSVname">Nombre de la base de datos</param>
    /// <param name="myFintess">Funcion para decidir que caso es mejor (por similaritud, por aptitud, etc...)</param>
    /// <param name="similarityThreshold">Entre 0 y 1, como de similar tienen que ser los casos para que no se guarden</param>
    /// <param name="kNNRequired">Numero de casos que se recuperan para la respuesta</param>
    /// <param name="myComparer">Comparador para decidir como se computa la similiritud de los casos (Necesario)</param>
    /// <param name="customReview">Función lambda para la revisión custom. Puede ser null si se va con la por defecto</param>
    /// <param name="customReviewArgs"> Argumentos necesarios para la función lamda. Puede ser null si esta no los necesita</param>gs">
    /// <param name="type">Tipo de funcion que se va a usar para revisar el perfomance del caso y si se va a guardar</param>
    public CBRBrain(string CSVname, CaseComparer myComparer , CaseSerializer mySerializer = null, float similarityThreshold = 0, reuseAnswerType reuseType = reuseAnswerType.mostSimilar, int kNNRequired = 1, CaseFitness myFintess = null,
         ReviseType type = ReviseType.alwaysRetain, Func<CaseCBRv2,CaseCBRv2,System.Object[], bool> customReview = null, System.Object[] customReviewArgs = null)
    {
        caseToSave = new List<CaseCBRv2>();
        readedCases = new List<CaseCBRv2>();
        casesToEvaluate = new Queue<CaseCBRv2>();
        if(mySerializer != null)
        {
            caseSerializer = mySerializer;
        }
        else caseSerializer = new CaseSerializer();
        possibleTwins = new List<CaseCBRv2>();
        this.weights = new Dictionary<string, float>();
        fitness = myFintess;
        comparer = myComparer;
        this.CSVname = CSVname;
        caseSerializer.readCases(CSVname, ref readedCases);
        this.kNNRequired = kNNRequired;
        this.similThresh = similarityThreshold;
        this.myFunc = customReview;
        this.myFuncArgs = customReviewArgs;
        this.reviseType = type;
        this.reuseAnswer = reuseType;
        this.normalizedWeights = false;
    }
    ~CBRBrain()
    {
        persistCases();
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
    }
    /// <summary>
    /// Se llama cuando el usuario considere que se tiene que evaluar el siguiente caso de la cola de evaluacion
    /// </summary>
    /// <param name="evaluate">Si se tiene que evaluar</param>
    public void setEvaluateNextCase(bool evaluate)
    {
        evaluateNextCase = evaluate;
    }
    /// <summary>
    /// En caso de que el usuario quiera usar su propio serializer
    /// </summary>
    /// <param name="caseSerializer">El serializer heredado que se quiere usar</param>
    public void setCaseSerializer(CaseSerializer caseSerializer)
    { 
        this.caseSerializer = caseSerializer;
    }
    /// <summary>
    /// Devuelve el threshold que se utiliza en la CBR, por si se quiere hacer con los modulos por separado
    /// </summary>
    /// <returns>El límte de similiritud para que un caso se guarde</returns>
    public float getSimilThreshold()
    {
        return this.similThresh;
    }
    public Dictionary<string, float> getPropertiesWeigth()
    {
        return weights;
    }
    private void normalizeWeights()
    {
        float sum = 0;
        Dictionary<string, float>.Enumerator it = weights.GetEnumerator();
        while (it.MoveNext())
        {
            sum += it.Current.Value;
        }
        it = weights.GetEnumerator();
        Dictionary<string, float> newWeights = new Dictionary<string, float>();
        while (it.MoveNext())
        {
            newWeights[it.Current.Key] = weights[it.Current.Key] / sum; // Se normalizan los pesos
        }
        weights = newWeights;
    }
    #region CBRModules

    /// <summary>
    /// Se encarga de comparar los casos leidos con el query y devolver los kNN Casos más prometedores
    /// (Parecidos o mejores, según ponderación del usuario)
    /// </summary>
    /// <param name="query">Caso presentado al sistema</param>
    /// <param name="minFitness">Aptitud minima para elegir un caso</param>
    /// <returns>Los kNN casos más prometedores, ordenados de más a menos prometedor</returns>
    public SortedSet<CaseWithSimilarity> retrieveKNNCases(in CaseCBRv2 query, float minFitness = 0)
    {
        SortedSet<CaseWithSimilarity> kNNCases = new SortedSet<CaseWithSimilarity>(fitness); // Ordenas por fitness
        foreach(CaseCBRv2 myCase in readedCases) 
        {
            CaseWithSimilarity myCaseWithSimil = comparer.computeSimilarity(query, myCase, weights);
            if (myCaseWithSimil.similarity >= minFitness)
            {// Tiene que ser mayor que la minima)
                if (kNNCases.Count < kNNRequired)
                {
                    kNNCases.Add(myCaseWithSimil);
                }
                else
                {
                    IEnumerator iterator = kNNCases.Reverse().GetEnumerator();
                    iterator.MoveNext(); // El elemento menos parecido o menos valioso

                    // Si el nuevo caso visto es mejor que el peor, borra el peor guardado y mete el nuevo
                    if (((CaseWithSimilarity)iterator.Current).similarity < myCaseWithSimil.similarity)
                    {
                        kNNCases.Remove((CaseWithSimilarity)iterator.Current);
                        kNNCases.Add(myCaseWithSimil);
                    }
                }
                if (similThresh > 0 && myCaseWithSimil.similarity >= similThresh) possibleTwins.Add(myCaseWithSimil.myCase); // Si son muy parecidos, mira despues
            }
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
    public dynamic reuseCases(in SortedSet<CaseWithSimilarity> knnCases, ref CaseCBRv2 query, reuseAnswerType type)
    {
        if (knnCases.Count > 0)
        {
            if (type == reuseAnswerType.mostVoted)
            {
                Dictionary<dynamic, int> votes = new Dictionary<dynamic, int>();
                foreach (CaseWithSimilarity myCase in knnCases)
                {
                    if(myCase.myCase.getAnswer()==null)
                    {
                        Debug.Log("WHAT THE FUCK");
                    }
                    if (!votes.ContainsKey(myCase.myCase.getAnswer()))
                    {
                        votes.Add(myCase.myCase.getAnswer(), 1);
                    }
                    else
                    {
                        votes[myCase.myCase.getAnswer()]++;
                    }
                }
                // Ordenar por votos y colocar la respuesta que mas votos reciba
                query.setAnswer(votes.OrderByDescending(x => x.Value).First().Key);
                return query.getAnswer();
            }
            else if (type == reuseAnswerType.weighted)
            {
                Dictionary<System.Object, int> votes = new Dictionary<object, int>();
                Tuple<dynamic, int> answer = null;
                foreach (CaseWithSimilarity myCase in knnCases)
                {
                    if (!votes.ContainsKey(query.getAnswer()))
                    {
                        votes.Add(myCase.myCase.getAnswer(), myCase.myCase.getWeight());
                        if (answer == null) answer = new Tuple<dynamic, int>(myCase.myCase.getAnswer(), myCase.myCase.getWeight());
                    }
                    else
                    {
                        votes[myCase.myCase.getAnswer()] += myCase.myCase.getWeight();
                        if (answer.Item2 < votes[myCase.myCase.getAnswer()])
                            answer = new Tuple<dynamic, int>(myCase.myCase.getAnswer(), votes[myCase.myCase.getAnswer()]);
                    }
                }
                query.setAnswer(answer.Item1);
                return answer.Item1;
            }
            else
            {
                IEnumerator iterator = knnCases.GetEnumerator();
                iterator.MoveNext(); // El elemento más prometedor 
                query.setAnswer(((CaseWithSimilarity)iterator.Current).myCase.getAnswer());
                return ((CaseWithSimilarity)iterator.Current).myCase.getAnswer(); // Supongo que, al ser en tiempo de ejecucion, esto se resolvera solo
            }
        }
        else return null;
    }
// PENDIENTE DE REVISIÓN
/// <summary>
/// Recibe un enum con el tipo de revisión que se quiere hacer y la función de revisión cómo expresión lamda introducida
/// por el usuario, que debe devolver un booleano indicando si el caso es útil y se guarda o no
/// Por defecto, siempre guarda el caso
/// </summary>
/// <param name="type">Enumerador indicando el tipo de revisión</param>
/// <returns></returns>
    public bool reviseCase(ReviseType type, CaseCBRv2 caseToRevise, CaseCBRv2 newCase)
    {
        if (type == ReviseType.custom)
        {
            //ERROR: myFunc es null
            bool save = myFunc(caseToRevise,newCase, myFuncArgs);
            if(!save) possibleTwins.Clear(); // Si no lo vas a guardar, no te interesa saber si ya hay en la base de datos
            return save;
        }
        else return true;
    }
    //TODO: Revisar si dos casos son muy iguales (estoy guardando los posibles gemelos en una variable de la clase que
    // se vacia en cada caso y que se rellena en el retrieve knn
    /// <summary>
    /// Añade el caso a la lista de casos por guardar y también a la base de datos para usar
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
                    int match = readedCases.IndexOf(possibleTwins[i]);
                    readedCases[match].setWeight(readedCases[match].getWeight() + 1);
                }
                i++;
            }
            if (!matchFound)
            {
                caseToSave.Add(myCase);
            }
        }
        else
        {
            caseToSave.Add(myCase);
        }
    }
    #endregion
    /// <summary>
    /// Se encarga de hacer el ciclo cbr cada vez que se llama. En caso de no tener un resultado de la base de casos, se ejecuta una fucnion
    /// implementada por el usuario que debe devolver un resultado(comúmnete, elige un valor resultado al azar, por ejemplo, en pacman, una de las 4 direcciones)
    /// </summary>
    /// <param name="query">El caso presentado</param>
    /// <param name="myFunc">Funcion que realizar en caso de que los casos no presenten una solucion o no haya casos</param>
    /// <param name="args">Argumentos necesarios para la función custom (puede ser nulo)</param>
    /// <param name="minFitness">Número entre 0 y 1 de aptitud mínima para utilizar un caso (Puede ser solo similitud)</param>
    /// <returns>La respuesta a ejecutar</returns>
    public dynamic CBRCycle(CaseCBRv2 query, Func<System.Object[],dynamic> myFunc, System.Object[]args = null, float minFitness = 0)
    {
        if(!normalizedWeights)
        {
            normalizeWeights();
            normalizedWeights = true;
        }
        SortedSet<CaseWithSimilarity> caseSimil = retrieveKNNCases(query, minFitness);
        dynamic res = reuseCases(caseSimil, ref query,reuseAnswer);
        Debug.Log(res);
        if (res == null) 
        {
            res = myFunc(args);
            query.setAnswer(res); 
        }
        casesToEvaluate.Enqueue(query);
        if(evaluateNextCase) 
        {
            CaseCBRv2 caseToEvaluate;
            if(casesToEvaluate.TryDequeue(out caseToEvaluate))
            {
                if(reviseCase(reviseType, caseToEvaluate,query)) // Compara el caso a evaluar (estado anterior de la partida) con la query actual (estado posterior de la partida)
                {
                    retainCases(in caseToEvaluate);
                    evaluateNextCase = false;
                }
            }
        }
        return res;
    }
    public void learnFromHuman(CaseCBRv2 query, dynamic playerAnswer)
    {
        if (!normalizedWeights)
        {
            normalizeWeights();
            normalizedWeights = true;
        }
        query.setAnswer(playerAnswer);
        retainCases(query);

    }
    public void persistCases()
    {
        if (readedCases.Count > 0) caseSerializer.writeCases(CSVname, readedCases, true);
        if(caseToSave.Count > 0)caseSerializer.writeCases(CSVname, caseToSave);
    }
    #endregion
}
