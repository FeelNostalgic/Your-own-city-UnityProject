using System;

namespace Commons
{
    [Serializable]
    public enum GameState
    {
        NotStarted, Starting, Paused, Playing, GameOver, Win
    }
    
    public enum HighlightType
    {
        none,
        highlighted,
        selected
    }
    
    public enum BuildingType
    {
        none,
        house,
        playground,
        hospital,
        police,
        road
    }

    public enum BuildingStatus
    {
        none,
        demolishing,
        building
    }
    
    public enum HUDPanels
    {
        none = 0,
        buildPanel,
        roadPanel,
        housePanel,
        multiplierPanel
    }
}

