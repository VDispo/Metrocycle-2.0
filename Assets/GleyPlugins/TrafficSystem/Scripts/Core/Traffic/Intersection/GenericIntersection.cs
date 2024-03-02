using GleyUrbanAssets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Base class for all intersections
    /// </summary>
    [System.Serializable]
    public class GenericIntersection : IIntersection
    {
        public string name;
        protected List<int> carsInIntersection;
        public List<int> exitWaypoints;

        public virtual void Initialize(WaypointManagerBase waypointManager, float greenLightTime, float yellowLightTime)
        {
            carsInIntersection = new List<int>();

            for (int i = 0; i < exitWaypoints.Count; i++)
            {
                waypointManager.GetWaypoint<Waypoint>(exitWaypoints[i]).SetIntersection(this, false, false, false, true);
            }
        }

        public virtual void ResetIntersection()
        {
            carsInIntersection = new List<int>();
        }


        public virtual void UpdateIntersection(float realtimeSinceStartup)
        {

        }


        public virtual bool IsPathFree(int waypointIndex)
        {
            return false;
        }


        public virtual void VehicleEnter(int vehicleIndex)
        {
            carsInIntersection.Add(vehicleIndex);
        }


        public virtual void VehicleLeft(int vehicleIndex)
        {
            carsInIntersection.Remove(vehicleIndex);
        }


        public virtual List<IntersectionStopWaypointsIndex> GetWaypoints()
        {
            return new List<IntersectionStopWaypointsIndex>();
        }

        public virtual void ResetIntersections(Vector3 center, float radius)
        {
            
        }

        internal virtual void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviour)
        {
            
        }

        internal virtual void SetGreenRoad(int roadIndex, bool doNotChangeAgain)
        {
            
        }

        internal virtual void RemoveCar(int index)
        {
            VehicleLeft(index);
        }
    }
}