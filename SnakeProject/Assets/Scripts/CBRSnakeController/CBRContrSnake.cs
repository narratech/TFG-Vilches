using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public enum Direction
{
    LEFT,RIGHT, UP, DOWN
}


public class CBRContrSnake : SnakeControl
{
    #region private
    CBRBrain myBrain;
    myCaseSerializer caseSerializer;
    int reviseCounter;
    bool humanControl = false;
    Direction lastDirectionPicked;
    #endregion
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        caseSerializer = new myCaseSerializer();
        myBrain = new CBRBrain("Default",new myComparer(), caseSerializer, 0.95f,reuseAnswerType.mostSimilar,
            5,new myCaseFitness(),ReviseType.custom,customEvaluateCase);
        reviseCounter = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (GameManager.Instance.getKeepPlaying())
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.G)) myBrain.persistCases();
            if(Input.GetKeyDown(KeyCode.M)) humanControl = !humanControl; //Cambiar con player?
            if(humanControl) HandleHumanInput();
            if (elapsedTime > 1 / speed)
            {
                CaseCBR query = formACase();
                Move();
                if (playerOne)
                {
                    GameManager.Instance.setPlayer1Positions(snakePositions);
                    GameManager.Instance.setPlayer1Dir(new Vector2(myDirection.x, myDirection.z));
                }
                else
                {
                    GameManager.Instance.setPlayer2Positions(snakePositions);
                    GameManager.Instance.setPlayer2Dir(new Vector2(myDirection.x,myDirection.z));
                }
                if(!humanControl)HandleInput();
                else myBrain.learnFromHuman(query, lastDirectionPicked);
                elapsedTime = 0;
                if(!humanControl)reviseCounter++;
            }
            
        }
    }
    public override void onResetTry()
    {
        myBrain.persistCases();
        base.onResetTry();
    }

    void HandleInput()
    {
        Direction myDir = myBrain.CBRCycle(formACase(), (System.Object[] args) => 
        {
            System.Random rnd = new System.Random();
            int xd = rnd.Next(0, 3);
            if (myDirection.x != 0)
            {
                if (xd == 0 && myDirection.x == -1) return Direction.LEFT;
                if (xd == 0 && myDirection.x == 1) return Direction.RIGHT;
                if (xd == 1) return Direction.UP;
                else return Direction.DOWN;
            }
            else
            {
                if (xd == 0 && myDirection.z == -1) return Direction.DOWN;
                if (xd == 0 && myDirection.z == 1) return Direction.UP;
                if (xd == 1) return Direction.LEFT;
                else return Direction.RIGHT;
            }
        });

        if (myDir == Direction.LEFT && (myDirection.x != 1))
        {
            turn(Dir.LEFT);
        }
        else if (myDir == Direction.RIGHT && (myDirection.x != -1))
        {
            turn(Dir.RIGHT);
        }
        else if (myDir == Direction.UP && (myDirection.z != -1))
        {
            turn(Dir.UP);
        }
        else if (myDir == Direction.DOWN && (myDirection.z != 1))
        {
            turn(Dir.DOWN);
        }

        if (reviseCounter >= 3) myBrain.setEvaluateNextCase(true);

    }
    void HandleHumanInput()
    {
        if ((playerOne && Input.GetKeyDown(KeyCode.A)) || (!playerOne && Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            if (myDirection.x != 1) turn(Dir.LEFT);
            lastDirectionPicked = Direction.LEFT;
        }
        else if ((playerOne && Input.GetKeyDown(KeyCode.D)) || (!playerOne && Input.GetKeyDown(KeyCode.RightArrow)))
        {
            if (myDirection.x != -1) turn(Dir.RIGHT);
            lastDirectionPicked = Direction.RIGHT;
        }
        else if ((playerOne && Input.GetKeyDown(KeyCode.W)) || (!playerOne && Input.GetKeyDown(KeyCode.UpArrow)))
        {
            if (myDirection.z!= -1) turn(Dir.UP);
            lastDirectionPicked = Direction.UP;
        }
        else if ((playerOne && Input.GetKeyDown(KeyCode.S)) || (!playerOne && Input.GetKeyDown(KeyCode.DownArrow)))
        {
            if (myDirection.z != 1) turn(Dir.DOWN);
            lastDirectionPicked = Direction.DOWN;
        }
    }

    CaseCBR formACase()
    {
        CaseCBR query = new CaseCBR();
        query.setAnswerType("direction");
        query.setProperty("position:vector2", headNode);
        myBrain.setWeigth("position", 0.25f);
        query.setProperty("headDirection:vector3",this.myDirection);
        myBrain.setWeigth("headDirection", 0.2f);
        query.setProperty("DistanceToWalls:floatList",this.getWallsDistance());
        myBrain.setWeigth("DistanceToWalls", 0.05f);
        query.setProperty("Score:float", 10); // Esto luego se modifica si juega la cbr y lo deja como buen movimiento si lo ha puesto un humano
        if(playerOne)
        {
            List<Vector2>myList = new List<Vector2>(GameManager.Instance.getPlayer1Positions());
            query.setProperty("myPartsNodes:vector2List", myList);
            List<Vector2>myList2 = new List<Vector2>(GameManager.Instance.getPlayer2Positions());
            query.setProperty("otherSnakePartsNode:vector2List", myList2);
            query.setProperty("levelScore:float", GameManager.Instance.getPlayer1Score());
        }
        else
        {
            List<Vector2> myList = new List<Vector2>(GameManager.Instance.getPlayer2Positions());
            query.setProperty("myPartsNodes:vector2List", myList);
            List<Vector2> myList2 = new List<Vector2>(GameManager.Instance.getPlayer1Positions());
            query.setProperty("otherSnakePartsNode:vector2List", myList2);
            query.setProperty("levelScore:float", GameManager.Instance.getPlayer2Score());
        }
        
        myBrain.setWeigth("myPartsNodes", 0.1f);
        myBrain.setWeigth("otherSnakePartsNode", 0.05f);
        query.setProperty("fruitPos:vector2", GameManager.Instance.getFruitNode());
        myBrain.setWeigth("fruitPos", 0.2f);
        query.setProperty("inTrackToCollide:bool", inTrackToCollision());
        myBrain.setWeigth("inTrackToCollide", 0.15f);

        return query;

    }


    bool customEvaluateCase(CaseCBR query, CaseCBR futureQuery, System.Object[] myArgs)
    {
        if (!humanControl)
        {
            float score = caseScore(query, futureQuery);
            query.setProperty("Score:float", score);
            if (score >= 0) return true;
            else return false;
        }
        else return true;
    }

    /// <summary>
    /// Se encarga de evaluar cómo de buena ha sido una jugada comparandola con el estado de la partida un tiempo despues
    /// </summary>
    /// <param name="query">El caso a evaluar</param>
    /// <param name="futureQuery">El estado actual del juego</param>
    /// <returns>El valor Score del caso</returns>

    int caseScore(in CaseCBR query, in CaseCBR futureQuery)
    {
        int score = 0;

        if (query.getProperty("levelScore") > futureQuery.getProperty("levelScore")) // Se ha reiniciado el nivel
            score -= 100; 
        if (query.getProperty("myPartsNodes").Count < futureQuery.getProperty("myPartsNodes").Count) // Se ha comido fruta
            score += 5;
        if (query.getProperty("inTrackToCollide")) //Mirar la distancia hacia la pared que esta mirando(Escalar recompensa con dsitancia
        { if (!futureQuery.getProperty("inTrackToCollide"))
                score += 3;
        }
        if (!query.getProperty("inTrackToCollide")) //Mirar la distancia hacia la pared que esta mirando(Escalar recompensa con dsitancia
        {
            if (futureQuery.getProperty("inTrackToCollide"))
                score -= 2;
        }

        float distanceBefore = (Math.Abs(query.getProperty("position").x - query.getProperty("fruitPos").x) + Math.Abs(query.getProperty("position").y - query.getProperty("fruitPos").y));
        float distanceAfter = (Math.Abs(futureQuery.getProperty("position").x - futureQuery.getProperty("fruitPos").x) + Math.Abs(futureQuery.getProperty("position").y - futureQuery.getProperty("fruitPos").y));

        if (distanceAfter < distanceBefore)
            score += 2;
        else if (distanceAfter > distanceBefore)
            score -= 1;
        return score;
    }

    bool inTrackToCollision()
    {
        bool inTrackToCollision = false;
        if(this.myDirection.x != 0)
        {
            for(int i = 0; i < 5; i++) // Mira en 5 casillas desde donde estoy
            {
                if ((this.headNode.x + this.myDirection.x * i) >= 29 || (this.headNode.x + this.myDirection.x * i) <= 0) inTrackToCollision = true;
                else if (GameManager.Instance.isThereSnake((int)(this.headNode.x + (this.myDirection.x * i)), (int)this.headNode.y, this.playerOne)) inTrackToCollision = true;
            }
            
        }
        else
        {
            for (int i = 0; i < 5; i++) // Mira en 5 casillas desde donde estoy
            {
                if ((this.headNode.y - this.myDirection.z * 5) >= 18 || (this.headNode.y - this.myDirection.z * 5) <= 0) inTrackToCollision = true;
                else if (GameManager.Instance.isThereSnake((int)this.headNode.x, (int)(this.headNode.y - (this.myDirection.z * i)), this.playerOne)) inTrackToCollision = true;
            }
        }
        return inTrackToCollision;
    }

    public void setHumanControl(bool humanContr)
    {
        humanControl=humanContr;
    }
    public void changeBaseCase(string newCaseBase)
    {
        myBrain = new CBRBrain(newCaseBase, new myComparer(), caseSerializer, 0.95f, reuseAnswerType.mostSimilar,
            5, new myCaseFitness(), ReviseType.custom, customEvaluateCase);
    }
}
