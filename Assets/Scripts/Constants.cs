using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metrocycle {
    public static class Constants
    {
        public const string laneNamePrefix = "Lane_";
    }

    public enum BikeType {
        Motorcycle,
        Bicycle
    }

    public enum ErrorReason {
        NOERROR,    // Default value

        // Error codes for turn/lane change
        LEFTTURN_NO_BLINKER,
        RIGHTTURN_NO_BLINKER,
        LEFTTURN_NO_HEADCHECK,
        RIGHTTURN_NO_HEADCHECK,

        // Error codes for intersections
        INTERSECTION_REDLIGHT,
        INTERSECTION_WRONGWAY,
        INTERSECTION_RIGHTTURN_FROM_OUTERLANE,
        INTERSECTION_RIGHTTURN_TO_OUTERLANE,
        INTERSECTION_LEFTTURN_FROM_OUTERLANE,
        INTERSECTION_LEFTTURN_TO_OUTERLANE,
        INTERSECTION_LEFT_UTURN_TO_OUTERLANE,
        INTERSECTION_LEFT_UTURN_FROM_OUTERLANE,
    }
}
