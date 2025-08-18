using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuGUIManager : MonoBehaviour
{
    private bool CBRPlayer1;
    private bool CBRPlayer2;
    [SerializeField]
    private GameObject caseBase1;
    [SerializeField]
    private GameObject caseBase2;
    // Start is called before the first frame update
    void Start()
    {
        CBRPlayer1 = false;
        CBRPlayer2 = false;
    }

    public void togleCBRPlayer1(bool toggle)
    {
        CBRPlayer1 = toggle;
    }
    public void togleCBRPlayer2(bool toggle)
    {
        CBRPlayer2 = toggle;
    }
    public void play()
    {
        PlayerPrefs.SetInt("activateCBR1", CBRPlayer1? 0 : 1);
        PlayerPrefs.SetInt("activateCBR2", CBRPlayer2? 0 : 1);
        PlayerPrefs.SetString("CBR1",caseBase1.GetComponent<TMP_InputField>().text);
        PlayerPrefs.SetString("CBR2",caseBase2.GetComponent<TMP_InputField>().text);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
