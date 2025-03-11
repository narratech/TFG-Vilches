using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CBRBrain
{
    Queue<CaseCBR> caseToEvaluate;
    List<CaseCBR> caseToSave;
    List<CaseCBR> readedCases;
    string pathToCSV;
    CBRBrain(string CSVname)
    {
        caseToSave = new List<CaseCBR>();
        readedCases = new List<CaseCBR>();
        if (File.Exists("CaseBase/" + CSVname)) // Si existe una base de casos, leelos
        {
            StreamReader myReader = new StreamReader("CaseBase/" + CSVname);
            // Lee y parsea los datos a casos
            parseCase(myReader);
            myReader.Close();
        }
    }
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
}
