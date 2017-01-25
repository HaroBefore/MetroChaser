using UnityEngine;

class DataColor
{
    static Color enemyColor = Color.red;
    public static Color EnemyColor
    {
        get { return enemyColor; }
    }
    static Color playerColor = Color.blue;
    public static Color PlayerColor
    {
        get { return playerColor; }
    }
    static Color passengerColor = new Color(0f,0.9f,0f);
    public static Color PassengerColor
    {
        get { return passengerColor; }
    }
}