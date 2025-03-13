using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class CBRBrain
{
    Queue<CaseCBR> caseToEvaluate;
    List<CaseCBR> caseToSave;
    List<CaseCBR> readedCases;

    Dictionary<string, float> weights;
    int kNNRequired;
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
        else id = readedCases.Count- caseToSave.Count; //Las id de los nuevos casos que no estan escritos, teniendo en cuenta que los readedCases van creciendo en partida

        StreamWriter myWriter = new StreamWriter("CaseBase/" + CSVname, true);
        if(!existedBefore) myWriter.WriteLine(caseToSave[0].getVariableNames()); //En caso de que no existiese, la primera linea es para nombres
        foreach (CaseCBR myCase in caseToSave) // Escribe los nuevos casos
        {
            myWriter.WriteLine(id+ "," + CaseUtility.parseCaseToCSV(myCase));
        }

        myWriter.Close();
    }
    #endregion
    #region public
    //DUDA: Luego normalizo los pesos yo para que sumen 1?
    /// <summary>
    /// Añade un valor al peso de una caracteristica
    /// </summary>
    /// <param name="key">Nombre de la caracteristica a la que se asocia el peso</param>
    /// <param name="w">Valor del peso (entre 0 y 1, preferiblemente todos los pesos suman 1)</param>
    public void setWeigth(string key, float w)
    {
        weights[key] = w;
    }

    /// <summary>
    /// Se encarga de comparar los casos leidos con el query y devolver los kNN Casos más prometedores
    /// (Parecidos o mejores, según ponderación del usuario)
    /// </summary>
    /// <param name="query">Caso presentado al sistema</param>
    /// <returns>Los kNN casos más prometedores, ordenados de más a menos prometedor</returns>
    public SortedSet<Tuple<CaseCBR, float>> retrieveKNNCases(CaseCBR query)
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
                Tuple<CaseCBR, float> otherCase = CaseUtility.computeSimilarity(query,myCase,weights);

                // Si el nuevo caso visto es mejor que el peor, borra el peor guardado y mete el nuevo
                if (((Tuple<CaseCBR, float>)iterator.Current).Item2 <  otherCase.Item2) 
                {
                    kNNCases.Remove((Tuple<CaseCBR, float>)iterator.Current);
                    kNNCases.Add(otherCase);
                }
            }
        }
        return kNNCases;
    }
    //TODO ReuseCases; ReviseAnswer; RetainCases;
    #endregion
}
