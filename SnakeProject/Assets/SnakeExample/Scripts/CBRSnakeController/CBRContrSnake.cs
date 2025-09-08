using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public enum Direction
{
    LEFT,RIGHT, UP, DOWN, NULL
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
            if (elapsedTime > 1 / speed)
            {
                if (!humanControl) HandleInput();
                else
                {
                    CaseCBR query = formACase();
                    myBrain.learnFromHuman(query, lastDirectionPicked);
                }
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
                
                elapsedTime = 0;
                if(!humanControl)reviseCounter++;
            }
            if (humanControl) HandleHumanInput();

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
            if (myDirection.x != 1)
            {
                turn(Dir.LEFT);
                lastDirectionPicked = Direction.LEFT;
            }
        }
        else if ((playerOne && Input.GetKeyDown(KeyCode.D)) || (!playerOne && Input.GetKeyDown(KeyCode.RightArrow)))
        {
            if (myDirection.x != -1)
            {
                turn(Dir.RIGHT);
                lastDirectionPicked = Direction.RIGHT;
            }
        }
        else if ((playerOne && Input.GetKeyDown(KeyCode.W)) || (!playerOne && Input.GetKeyDown(KeyCode.UpArrow)))
        {
            if (myDirection.z != -1)
            {
                turn(Dir.UP);
                lastDirectionPicked = Direction.UP;
            }
        }
        else if ((playerOne && Input.GetKeyDown(KeyCode.S)) || (!playerOne && Input.GetKeyDown(KeyCode.DownArrow)))
        {
            if (myDirection.z != 1)
            {
                turn(Dir.DOWN);
                lastDirectionPicked = Direction.DOWN;
            }
        }
    }

    CaseCBR formACase()
    {
        CaseCBR query = new CaseCBR();
        query.setAnswerType("direction");

        // Posición de la cabeza
        query.setProperty("position:vector2", headNode);
        myBrain.setWeigth("position", 0.2f);

        // Dirección de la cabeza
        query.setProperty("headDirection:vector3",this.myDirection);
        myBrain.setWeigth("headDirection", 0.2f);

        query.setProperty("Score:float", 10); // Esto luego se modifica si juega la cbr y lo deja como buen movimiento si lo ha puesto un humano
        if(playerOne)
        {   
            query.setProperty("levelScore:float", GameManager.Instance.getPlayer1Score());
        }
        else
        {
            query.setProperty("levelScore:float", GameManager.Instance.getPlayer2Score());
        }

        query.setProperty("numberOfParts:float", numberOfParts);

        // Posicion relativa de la fruta
        query.setProperty("fruitRelPos:directionList", fruitRelativePos());
        myBrain.setWeigth("fruitRelPos", 0.25f);

        // Distancia manhattan a la fruta
        query.setProperty("fruitDis:float", (Math.Abs(GameManager.Instance.getFruitNode().x - headNode.x) +
            Math.Abs(GameManager.Instance.getFruitNode().y - headNode.y)));
        myBrain.setWeigth("fruitDis", 0.1f);

        // Colisiones inmediatas
        query.setProperty("checkCollisions:boolList", checkCollisions());
        myBrain.setWeigth("checkCollisions", 0.25f);
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
        if (query.getProperty("numberOfParts") < futureQuery.getProperty("numberOfParts")) // Se ha comido fruta
            score += 5;

        float distanceBefore = query.getProperty("fruitDis");
        float distanceAfter = futureQuery.getProperty("fruitDis");

        if (distanceAfter < distanceBefore)
            score += 2;
        else if (distanceAfter > distanceBefore)
            score -= 1;
        return score;
    }
    /// <summary>
    /// Checkea si la serpiente puede colisionar en sus 4 direcciones
    /// </summary>
    /// <returns>Lista de booleanos con choques derecha,izda,abajo y arriba en ese orden</returns>
    List<bool> checkCollisions()
    {
        List<bool> result = new List<bool>();
        // Derecha
        if ((this.headNode.x + 1) >= 29 || 
            (GameManager.Instance.isThereSnake((int)(this.headNode.x + 1), (int)this.headNode.y))) result.Add(true);
        else result.Add(false);
        // Izquierda
        if ((this.headNode.x - 1) <= 0 ||
            (GameManager.Instance.isThereSnake((int)(this.headNode.x - 1), (int)this.headNode.y))) result.Add(true);
        else result.Add(false);
        // Abajo
        if ((this.headNode.y + 1) >= 18 ||
            (GameManager.Instance.isThereSnake((int)(this.headNode.x), (int)this.headNode.y + 1))) result.Add(true);
        else result.Add(false);
        // Arriba
        if ((this.headNode.y - 1) <= 0 ||
            (GameManager.Instance.isThereSnake((int)(this.headNode.x), (int)this.headNode.y - 1))) result.Add(true);
        else result.Add(false);

        return result;
    }

    List<Direction> fruitRelativePos()
    {
        List<Direction> result = new List<Direction>();
        // Fruta a la derecha
        if (this.headNode.x < GameManager.Instance.getFruitNode().x) result.Add(Direction.RIGHT);
        // Fruta a la izquierda
        else if(this.headNode.x > GameManager.Instance.getFruitNode().x) result.Add(Direction.LEFT);
        // Fruta en el mismo X
        else result.Add(Direction.NULL);

        // Fruta abajo
        if (this.headNode.y < GameManager.Instance.getFruitNode().y) result.Add(Direction.DOWN);
        // Fruta arriba
        else if (this.headNode.y > GameManager.Instance.getFruitNode().y) result.Add(Direction.UP);
        // Fruta en el mismo X
        else result.Add(Direction.NULL);

        return result;
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
