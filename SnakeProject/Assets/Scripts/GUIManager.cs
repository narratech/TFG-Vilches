using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pointsPlayer1;
    [SerializeField]
    private GameObject pointsPlayer2;
    [SerializeField]
    private GameObject winnerText;
    [SerializeField]
    private GameObject retryButton;
    [SerializeField]
    private GameObject returnButton;
    // Start is called before the first frame update
    void Start()
    {
        pointsPlayer1.GetComponent<TextMeshProUGUI>().SetText("0");
        pointsPlayer2.GetComponent<TextMeshProUGUI>().SetText("0");
        winnerText.SetActive(false);
        retryButton.SetActive(false);
        returnButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeP1Points(int points)
    {
        pointsPlayer1.GetComponent<TextMeshProUGUI>().SetText(points.ToString());
    }
    public void ChangeP2Points(int points)
    {
        pointsPlayer2.GetComponent<TextMeshProUGUI>().SetText(points.ToString());
    }
    public void ShowWinText(bool isPlayerOne)
    {
        TextMeshProUGUI myText = winnerText.GetComponent<TextMeshProUGUI>();
        if (isPlayerOne)
        {
            myText.SetText("Player 1 Wins");
            myText.color = pointsPlayer1.GetComponent<TextMeshProUGUI>().color;
        }
        else
        {
            myText.SetText("Player 2 Wins");
            myText.color = pointsPlayer2.GetComponent<TextMeshProUGUI>().color;
        }
        winnerText.SetActive(true);
        retryButton.SetActive(true);
        returnButton.SetActive(true);
    }
    public void OnRetryReset()
    {
        winnerText.SetActive(false);
        retryButton.SetActive(false);
        returnButton.SetActive(false);
    }
}
