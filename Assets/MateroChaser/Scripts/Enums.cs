public enum ePlaceType
{
    None,
    Station,
    Subway
}

public enum eSubwayNum
{
    None,
    First,
    Second
}

public enum eSubwaySide
{
    TOP,
    DOWN
}

public enum eSubwayState
{
    SubwayMoving,

}

public enum eUnitState
{
    None,
    UnitInitialization,
    UnitPlaying,
    UnitRespawning,
    END
}

public enum eNetworkMsg
{
    NetworkLogin,
    NetworkLogout,
    NetworkPlaying,
    NetworkInitInfoReq,
    NetworkInitInfoRes,
    NetworkHitPlayer,
}