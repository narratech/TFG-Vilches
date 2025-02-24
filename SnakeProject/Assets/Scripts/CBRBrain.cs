using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CBRBrain
{
    Queue<CaseCBR> caseToEvaluate;
    StreamReader myReader;
    List<CaseCBR> caseToSave;
    List<CaseCBR> readedCases;
    CBRBrain(string path)
    {
        caseToSave = new List<CaseCBR>();
        readedCases = new List<CaseCBR>();
        myReader = new StreamReader(Application.dataPath + path);
        string[] thisCaseDataNames = myReader.ReadLine().Split(',');
        // Lee y parsea los datos a casos
        while (!myReader.EndOfStream)
        {
            string[] thisCaseData = myReader.ReadLine().Split(',');
            readedCases.Add(parseCase(thisCaseDataNames, thisCaseData));
        }
        myReader.Close();
    }
    private CaseCBR parseCase(string[] variableNames, string[] variableData)
    {
        CaseCBR myCase = new CaseCBR();
        //  Como hago para saber que tipo de caso tengo entre manos si todavia no hay base de datos
        return myCase;
    }
}
