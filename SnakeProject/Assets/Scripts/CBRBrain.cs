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
    Queue<CaseCBR> casesToEvaluate;
    List<CaseCBR> caseToSave;
    List<CaseCBR> readedCases;

    List<Tuple<CaseCBR, float>> possibleTwins;

    Dictionary<string, float> weights;
    int kNNRequired;
    int casesNum;
    bool evaluateNextCase;
    public CBRBrain(string CSVname, int kNNRequired = 1)
    {
        caseToSave = new List<CaseCBR>();
        readedCases = new List<CaseCBR>();
        if (File.Exists("CaseBase/" + CSVname)) // Si existe una base de casos, leelos
        {
            StreamReader myReader = new StreamReader("CaseBase/" + CSVname);
            // Lee y parsea los datos a casos
            readCases(myReader);
            myReader.Close();
        }
        casesNum = readedCases.Count;

        this.kNNRequired = kNNRequired;
    }
    #region private
    /// <summary>
    /// Se encarga de leer los cases del csv y convertirlos a casos de la logica
    /// </summary>
    /// <param name="myReader">Streamreader para leer</param>
    private void readCases(StreamReader myReader)
    {
        string[] variablesTypes = myReader.ReadLine().Split(","); // Primera linea con los nombres y tipos de las variables
        while (!myReader.EndOfStream)
        {
            string[] values = myReader.ReadLine().Split(",");
            readedCases.Add(CaseUtility.parseCSVToCase(variablesTypes, values));
        }
    }
    // Esto en caso de que no se borren movimientos antiguos o malos
    // En otro caso, habria que reescribir el archivo entero o buscar formas de mover el puntero de escritura y borrar esas lineas
    // pero igual habria que actualizar la id del resto.
    private void writeCases(string CSVname)
    {
        bool existedBefore = true;
        int id = 0;
        if (!Directory.Exists("CaseBase")) Directory.CreateDirectory("CaseBase");
        if (!File.Exists("CaseBase/" + CSVname)) existedBefore = false;
        else id = casesNum; //Las id de los nuevos casos que no estan escritos

        StreamWriter myWriter = new StreamWriter("CaseBase/" + CSVname, true);
        if(!existedBefore) myWriter.WriteLine(caseToSave[0].getVariableNames()); //En caso de que no existiese, la primera linea es para nombres
        foreach (CaseCBR myCase in caseToSave) // Escribe los nuevos casos
        {
            myWriter.WriteLine(id+ "," + CaseUtility.parseCaseToCSV(myCase));
            id++;
        }

        myWriter.Close();
    }
    #endregion
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
    public SortedSet<Tuple<CaseCBR, float>> retrieveKNNCases(in CaseCBR query)
    {
        SortedSet<Tuple<CaseCBR, float>> kNNCases = new SortedSet<Tuple<CaseCBR, float>>(new CaseComparer());
        foreach(CaseCBR myCase in readedCases) 
        {
            if (kNNCases.Count < kNNRequired)
            {
                kNNCases.Add(CaseUtility.computeSimilarity(query, myCase, weights));
            }
            else
            {
                IEnumerator iterator = kNNCases.Reverse().GetEnumerator();
                iterator.MoveNext(); // El elemento menos parecido o menos valioso
                Tuple<CaseCBR, float> myCaseWithSimil = CaseUtility.computeSimilarity(query,myCase,weights);

                // Si el nuevo caso visto es mejor que el peor, borra el peor guardado y mete el nuevo
                if (((Tuple<CaseCBR, float>)iterator.Current).Item2 < myCaseWithSimil.Item2) 
                {
                    kNNCases.Remove((Tuple<CaseCBR, float>)iterator.Current);
                    kNNCases.Add(myCaseWithSimil);
                }
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
    public  T reuseCases <T>(in SortedSet<Tuple<CaseCBR, float>> knnCases, ref CaseCBR query, reuseAnswerType type)
    {
        
        if(type == reuseAnswerType.mostVoted)
        {
            Dictionary <System.Object, int> votes = new Dictionary<object, int>();
            Tuple<T, int> answer = null;
            foreach(Tuple<CaseCBR,float> myCase in knnCases)
            {
                if (!votes.ContainsKey(query.getAnswer()))
                {
                    votes.Add(myCase.Item1.getAnswer(), 1);
                    if(answer == null) answer = new Tuple<T,int>(myCase.Item1.getAnswer(),1);
                }
                else
                {
                    votes[myCase.Item1.getAnswer()]++;
                    if(answer.Item2 < votes[myCase.Item1.getAnswer()]) answer = new Tuple<T, int>(myCase.Item1.getAnswer(), 1);
                }
            }
            query.setAnswer(answer.Item1);
            return answer.Item1;
        }
        else if(type == reuseAnswerType.weighted)
        {
            Dictionary<System.Object, int> votes = new Dictionary<object, int>();
            Tuple<T, int> answer = null;
            foreach (Tuple<CaseCBR, float> myCase in knnCases)
            {
                if (!votes.ContainsKey(query.getAnswer()))
                {
                    votes.Add(myCase.Item1.getAnswer(), myCase.Item1.getWeight());
                    if (answer == null) answer = new Tuple<T, int>(myCase.Item1.getAnswer(), myCase.Item1.getWeight());
                }
                else
                {
                    votes[myCase.Item1.getAnswer()]+= myCase.Item1.getWeight();
                    if (answer.Item2 < votes[myCase.Item1.getAnswer()]) 
                        answer = new Tuple<T, int>(myCase.Item1.getAnswer(), votes[myCase.Item1.getAnswer()]);
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
/// <param name="myFunc">Función lambda para la revisión custom. Puede ser null si se va con la por defecto</param>
/// <param name="args">Argumentos necesarios para la función lamda. Puede ser null si esta no los necesita</param>
/// <returns></returns>
    public bool reviseCase(ReviseType type, Func<System.Object[], bool> myFunc = null, System.Object[] args = null)
    {
        if (type == ReviseType.custom)
        {
            //ERROR: myFunc es null
            bool save = myFunc(args);
            if(!save) possibleTwins.Clear(); // Si n o lo vas a guardar, no te interesa saber si ya hay en la base de datos
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
    public void retainCases(in CaseCBR myCase, float similarityThreshold)
    {
        caseToSave.Add(myCase);
        readedCases.Add(myCase);
    }
    #endregion
    /// <summary>
    /// Se encarga de hacer el ciclo cbr cada vez que se llama
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public dynamic CBRCycle(CaseCBR query)
    {
        SortedSet<Tuple<CaseCBR,float>> caseSimil = retrieveKNNCases(query);
        dynamic res = reuseCases(caseSimil, ref query);
        casesToEvaluate.Enqueue(query);
        if(evaluateNextCase) // TODO: Hacer manejo de errores si no hay más que evaluar
        {
            CaseCBR caseToEvaluate;
            if(casesToEvaluate.TryDequeue(out caseToEvaluate))
            {
                if(reviseCase<TIPO DEL COMPARADOR> (caseToEvaluate, COMPARADOR ???))
                {
                    retainCases(in caseToEvaluate, 0.95f);
                    evaluateNextCase = false;
                }
            }
        }
    }
    #endregion
}
