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
                elapsedTime = 0;
            }
        }
    }

    void HandleInput()
    {
        if((playerOne && Input.GetKeyDown(KeyCode.A)) || (!playerOne && Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            int nodeX = 17 + Mathf.RoundToInt(headPart.parte.transform.position.x);
            int nodeY = 9 - Mathf.RoundToInt(headPart.parte.transform.position.z);
            //Marcas el siguiente nodo de tu direccion para giro
            Vector3 newDirect = Quaternion.AngleAxis(-90, Vector3.up) * myDirection;
            newDirect.x = Mathf.RoundToInt(newDirect.x);
            newDirect.z = Mathf.RoundToInt(newDirect.z);
            newDirect = newDirect.normalized;
            myNodos[nodeX , nodeY].direccion = newDirect;
            myNodos[nodeX, nodeY].rotationNeeded = -90;
        }
        else if ((playerOne && Input.GetKeyDown(KeyCode.D)) || (!playerOne && Input.GetKeyDown(KeyCode.RightArrow)))
        {
            int nodeX = 17 + Mathf.RoundToInt(headPart.parte.transform.position.x);
            int nodeY = 9 - Mathf.RoundToInt(headPart.parte.transform.position.z);
            //Marcas el siguiente nodo de tu direccion para giro
            Vector3 newDirect = Quaternion.AngleAxis(90, Vector3.up) * myDirection;
            newDirect.x = Mathf.RoundToInt(newDirect.x);
            newDirect.z = Mathf.RoundToInt(newDirect.z);
            newDirect = newDirect.normalized;
            myNodos[nodeX, nodeY].direccion = newDirect ;
            myNodos[nodeX, nodeY].rotationNeeded = 90;
        }
    }
}
