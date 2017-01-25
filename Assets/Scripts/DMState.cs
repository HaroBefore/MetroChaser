using System;
using System.Collections;
using UnityEngine;

public class DMStateEnemyIdle : State<DMEnemyCtrl>
{
    static DMStateEnemyIdle instance;
    public static DMStateEnemyIdle Instance
    {
        get
        {
            if (instance == null)
                instance = new DMStateEnemyIdle();
            return instance;
        }
    }

    public override void Enter(DMEnemyCtrl owner)
    {
        Debug.Log("StateEnemyIdle Enter");
        owner.isMoving = false;
        owner.changeColor.TurnColor(DataColor.PassengerColor, owner.turnColorDelay);
    }

    public override void Execute(DMEnemyCtrl owner)
    {
        if (owner.isMoving == true)
        {
            owner.ChangeState(DMStateEnemyMoving.Instance);
        }
    }

    public override void Exit(DMEnemyCtrl owner)
    {
        Debug.Log("StateEnemyIdle Exit");
    }
}

public class DMStateEnemyMoving : State<DMEnemyCtrl>
{
    static DMStateEnemyMoving instance;
    public static DMStateEnemyMoving Instance
    {
        get
        {
            if (instance == null)
                instance = new DMStateEnemyMoving();
            return instance;
        }
    }

    public override void Enter(DMEnemyCtrl owner)
    {
        Debug.Log("StateEnemyMoving Enter");

        owner.isMoving = true;
        owner.changeColor.TurnColor(DataColor.EnemyColor, 0f);
    }

    public override void Execute(DMEnemyCtrl owner)
    {
        if (owner.isMoving == false)
        {
            owner.ChangeState(DMStateEnemyIdle.Instance);
        }
    }

    public override void Exit(DMEnemyCtrl owner)
    {
        Debug.Log("StateEnemyMoving Exit");
    }
}