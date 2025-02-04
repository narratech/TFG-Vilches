using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro pointsPlayer1;
    [SerializeField]
    private TextMeshPro pointsPlayer2;
    [SerializeField]
    private GameObject winnerText;
    [SerializeField]
    private GameObject retryButton;
    // Start is called before the first frame update
    void Start()
    {
        pointsPlayer1.SetText("0");
        pointsPlayer1.SetText("0");
        winnerText.SetActive(false);
        retryButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeP1Points(int points)
    {
        pointsPlayer1.SetText(points.ToString());
    }
    public void ChangeP2Points(int points)
    {
        pointsPlayer2.SetText(points.ToString());
    }
    public void ShowWinText(bool isPlayerOne)
    {
        TextMeshPro myText = winnerText.GetComponent<TextMeshPro>();
        if (isPlayerOne)
        {
            myText.SetText("Player 1 Wins");
            myText.color = pointsPlayer1.color;
        }
        else
        {
            myText.SetText("Player 2 Wins");
            myText.color = pointsPlayer2.color;
        }
        winnerText.SetActive(true);
        retryButton.SetActive(true);
    }
    public void OnRetryReset()
    {
        pointsPlayer1.SetText("0");
        pointsPlayer1.SetText("0");
        winnerText.SetActive(false);
        retryButton.SetActive(false);
    }
}
