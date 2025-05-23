using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSnakeControl : SnakeControl
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (GameManager.Instance.getKeepPlaying())
        {
            base.Update();
            HandleInput();
            if (elapsedTime > 1 / speed)
            {
                Move();
                if (playerOne)
                {
                    GameManager.Instance.setPlayer1Positions(snakePositions);
                    GameManager.Instance.setPlayer1Dir(new Vector2(myDirection.x, myDirection.z));
                }
                else
                {
                    GameManager.Instance.setPlayer2Positions(snakePositions);
                    GameManager.Instance.setPlayer2Dir(new Vector2(myDirection.x, myDirection.z));
                }
                elapsedTime = 0;
            }
        }
    }

    void HandleInput()
    {
        if((playerOne && Input.GetKeyDown(KeyCode.A)) || (!playerOne && Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            if(myDirection.x != 1) turn(Dir.LEFT);
        }
        else if ((playerOne && Input.GetKeyDown(KeyCode.D)) || (!playerOne && Input.GetKeyDown(KeyCode.RightArrow)))
        {
            if (myDirection.x != -1) turn(Dir.RIGHT);
        }
        else if ((playerOne && Input.GetKeyDown(KeyCode.W)) || (!playerOne && Input.GetKeyDown(KeyCode.UpArrow)))
        {
            if (myDirection.z != -1) turn(Dir.UP);
        }
        else if ((playerOne && Input.GetKeyDown(KeyCode.S)) || (!playerOne && Input.GetKeyDown(KeyCode.DownArrow)))
        {
            if (myDirection.z != 1) turn(Dir.DOWN);
        }
    }
}
