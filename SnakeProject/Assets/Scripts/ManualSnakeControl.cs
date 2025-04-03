using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSnakeControl : SnakeControl
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (keepPlaying)
        {
            base.Update();
            HandleInput();
            if (elapsedTime > 1 / speed)
            {
                Move();
                if(playerOne)GameManager.Instance.setPlayer1Positions(snakePositions);
                else GameManager.Instance.setPlayer2Positions(snakePositions);
                elapsedTime = 0;
            }
        }
    }

    void HandleInput()
    {
        if((playerOne && Input.GetKeyDown(KeyCode.A)) || (!playerOne && Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            turnLeft();
        }
        else if ((playerOne && Input.GetKeyDown(KeyCode.D)) || (!playerOne && Input.GetKeyDown(KeyCode.RightArrow)))
        {
            turnRigth();
        }
    }
}
