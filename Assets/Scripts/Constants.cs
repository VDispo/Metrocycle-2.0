namespace Metrocycle {
    public static class Constants
    {
        public const string laneNamePrefix = "Lane_";
        public const float ASSUME_HEADCHECK = -100f;
    }

    public enum BikeType {
        Motorcycle,
        Bicycle
    }

    public enum ErrorReason {
        NOERROR,    // Default progress

        // Error codes for turn/lane change
        LEFTTURN_NO_BLINKER,
        RIGHTTURN_NO_BLINKER,
        LEFTTURN_NO_HEADCHECK,
        RIGHTTURN_NO_HEADCHECK,
        SHORT_BLINKER_TIME, // turn/lane change performed immediately without waiting e.g. 2s for other vehicles to react
        EXPIRED_HEADCHECK,  // Headcheck performed, but too much time has passed (e.g. last head check was 10s ago)
        NO_HEADCHECK_AFTER_BLINKER,
        WRONG_BLINKER,


        // Error codes for intersections
        INTERSECTION_REDLIGHT,
        INTERSECTION_WRONGWAY,
        INTERSECTION_RIGHTTURN_FROM_OUTERLANE,
        INTERSECTION_RIGHTTURN_TO_OUTERLANE,
        INTERSECTION_LEFTTURN_FROM_OUTERLANE,
        INTERSECTION_LEFTTURN_TO_OUTERLANE,
        INTERSECTION_LEFT_UTURN_TO_OUTERLANE,
        INTERSECTION_LEFT_UTURN_FROM_OUTERLANE,
        LANECHANGE_NOTALLOWED,    // no lane change in solid line

        // Error codes for collisions
        COLLISION_AIVEHICLE,
        COLLISION_OBSTACLE,

        // Error codes for forbidden lanes
        EXCLUSIVE_BIKELANE,
        EXCLUSIVE_BUSLANE,
        BIKE_NOTALLOWED,

        OVERSPEEDING,
        UNCANCELLED_BLINKER,
    }

    public enum CustomizationAssetType
    {
        Character,
        Headgear,
        Footwear,
    }

    /// <summary>
    /// Negative for BASIC, 0 for tutorial, positive for ADVANCED.
    /// </summary>
    public enum SceneScenarioTypes
    {
        Multilane = -4, 
        Uturn = -3,
        Intersection = -2,
        Straight = -1,
        Tutorial = 0,
        Commonwealth = 1,
        EDSA = 2,
    }
}
