using System;
using System.Collections;
using UnityEngine;

public abstract class State<OWNER_TYPE> {
    public abstract void Enter(OWNER_TYPE owner);
    public abstract void Execute(OWNER_TYPE owner);
    public abstract void Exit(OWNER_TYPE owner);
}

public class StateEnemyIdle : State<EnemyCtrl>
{
    static StateEnemyIdle instance;
    public static StateEnemyIdle Instance
    {
        get
        {
            if (instance == null)
                instance = new StateEnemyIdle();
            return instance;
        }
    }

    public override void Enter(EnemyCtrl owner)
    {
        Debug.Log("StateEnemyIdle Enter");
        owner.isMoving = false;
        owner.changeColor.TurnColor(DataColor.PassengerColor, owner.turnColorDelay);
    }

    public override void Execute(EnemyCtrl owner)
    {
        if (owner.isMoving == true)
        {
            owner.ChangeState(StateEnemyMoving.Instance);
        }
    }

    public override void Exit(EnemyCtrl owner)
    {
        Debug.Log("StateEnemyIdle Exit");
    }
}

public class StateEnemyMoving : State<EnemyCtrl>
{
    static StateEnemyMoving instance;
    public static StateEnemyMoving Instance
    {
        get
        {
            if (instance == null)
                instance = new StateEnemyMoving();
            return instance;
        }
    }

    public override void Enter(EnemyCtrl owner)
    {
        Debug.Log("StateEnemyMoving Enter");

        owner.isMoving = true;
        owner.changeColor.TurnColor(DataColor.EnemyColor, 0f);
    }

    public override void Execute(EnemyCtrl owner)
    {
        if (owner.isMoving == false)
        {
            owner.ChangeState(StateEnemyIdle.Instance);
        }
    }

    public override void Exit(EnemyCtrl owner)
    {
        Debug.Log("StateEnemyMoving Exit");
    }
}