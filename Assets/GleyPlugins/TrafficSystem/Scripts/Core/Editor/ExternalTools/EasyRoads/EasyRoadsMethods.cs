#if USE_EASYROADS3D
using EasyRoads3Dv3;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

namespace GleyTrafficSystem
{
    public class EasyRoadsMethods : Editor
    {
        const string EasyRoadsWaypointsHolder = "GleyEasyRoadsWaypoints";

        private static List<GenericIntersectionSettings> allGleyIntersections;
        private static List<Waypoint> points;
        private static List<Transform> waypointParents;
        private static List<Waypoint> connectors;
        private static List<Transform> connectionParents;
        private static List<Transform> allWaypoints;
        private static List<Transform> allConnectors;
        private static ERCrossings[] allERIntersections;


        public static void ExtractWaypoints(IntersectionType intersectionType, float greenLightTime, float yellowLightTime, bool linkLanes, int waypointDistance, List<int> vehicleTypes)
        {
            //destroy existing roads
            DestroyImmediate(GameObject.Find(EasyRoadsWaypointsHolder));

            //create scene hierarchy
            ERRoadNetwork roadNetwork = new ERRoadNetwork();
            ERRoad[] roads = roadNetwork.GetRoads();
            Debug.Log("Roads: " + roads.Length);

            points = new List<Waypoint>();
            waypointParents = new List<Transform>();
            allWaypoints = new List<Transform>();
            allConnectors = new List<Transform>();
            connectors = new List<Waypoint>();
            connectionParents = new List<Transform>();
            allGleyIntersections = new List<GenericIntersectionSettings>();
            Transform holder = new GameObject(EasyRoadsWaypointsHolder).transform;
            Transform waypointsHolder = new GameObject("WaypointsHolder").transform;
            Transform intersectionHolder = new GameObject(Constants.intersectionHolderName).transform;

            waypointsHolder.SetParent(holder);
            intersectionHolder.SetParent(holder);

            AddIntersections(intersectionHolder, intersectionType, greenLightTime, yellowLightTime);

            // NOTE: we can only set lane edges AFTER the call to CreateTrafficWaypoints() below
            //       so we first save the INDEX of in/out waypoints per lane
            List<int> inWaypoints = new List<int>();
            List<int> outWaypoints = new List<int>();
            List<int> laneIdxs = new List<int>();
            List<GameObject> roadHolders = new List<GameObject>();

            //extract information from EasyRoads
            for (int i = 0; i < roads.Length; i++)
            {
                if (!roads[i].roadScript.isSideObject)
                {
                    Debug.Log("MTS-ER3D: Processing road " + roads[i].gameObject);
                    GameObject road = new GameObject(roads[i].GetName() + "(ER3DRoad_)");
                    road.transform.SetParent(waypointsHolder);
                    GameObject lanesHolder = new GameObject("Lanes");
                    Transform connectorsHolder = new GameObject("Connectors").transform;
                    lanesHolder.transform.SetParent(road.transform);
                    connectorsHolder.SetParent(road.transform);

                    roadHolders.Add(road);
                    if (roads[i].GetLaneCount() > 0)
                    {
                        // TODO: make these configurable
                        const int NUMBER_OF_CARS = 10;

                        // Add Road Script so that we can interact with this road via Gley Window
                        Road roadScript = road.gameObject.AddComponent<Road>();
                        Debug.Log("script added??? " + roadScript);
                        roadScript.SetDefaults(roads[i].GetLaneCount(), roads[i].GetLaneWidth(), waypointDistance);
                        roadScript.SetRoadProperties((int)roads[i].GetSpeedLimit(), NUMBER_OF_CARS);

                        // Add Paths just to make it easier to modify
                        // TODO: make sure modified path is not overridden when regenerating ?
                        Vector3[] markers = roads[i].GetSplinePointsCenter();
                        // roadScript.CreatePath(markers[0], markers[markers.Length-2]);    // Only add ends of path

                        // HACK: only use every Nth marker to reduce their number
                        roadScript.CreatePath(markers[0], markers[1]);
                        const int N = 4;
                        for (int j = 2; j < markers.Length-1; j += N) {
                            roadScript.path.AddSegment(markers[j]);
                        }
                        roadScript.path.AddSegment(markers[markers.Length-1]);

                        ExtractLaneWaypoints(roads[i].GetLeftLaneCount(), lanesHolder, roads[i], ERLaneDirection.Left, i,
                                             inWaypoints, outWaypoints, laneIdxs);
                        ExtractLaneWaypoints(roads[i].GetRightLaneCount(), lanesHolder, roads[i], ERLaneDirection.Right, i,
                                             inWaypoints, outWaypoints, laneIdxs);
                        ExtractConnectors(roads[i].GetLaneCount(), roads[i], connectorsHolder, i);
                    }
                    else
                    {
                        Debug.LogError("No lane data found for " + roads[i].gameObject+". Make sure this road has at least one lane inside Lane Info tab.", roads[i].gameObject);
                    }
                }
            }

            //convert extracted information to waypoints
            CreateTrafficWaypoints(vehicleTypes);

            LinkAllWaypoints(waypointsHolder);

            if (linkLanes)
            {
                LinkOvertakeLanes(waypointsHolder, waypointDistance);
            }

            CreateConnectorWaypoints(vehicleTypes);

            LinkAllConnectors(waypointsHolder);

            LinkConnectorsToRoadWaypoints();

            AssignIntersections(intersectionType);

            {
            // Add Lane information into Road Script for MTS interactability
            int numLanes = inWaypoints.Count;
            Debug.Assert(numLanes == outWaypoints.Count, "MTS-ER3D: Unbalanced number of in and out waypoints.");
            for (int i = 0; i < numLanes; ++i) {
                GameObject lane = waypointParents[inWaypoints[i]].gameObject;
                Debug.Assert(lane == waypointParents[outWaypoints[i]].gameObject, "MTS-ER3D: Mismatched in and out waypoints.");
                Road roadScript = lane.transform.parent.parent.gameObject.GetComponent<Road>();

                WaypointSettings inConnector = allWaypoints[inWaypoints[i]].GetComponent<WaypointSettings>();
                WaypointSettings outConnector = allWaypoints[outWaypoints[i]].GetComponent<WaypointSettings>();

                roadScript.AddLaneConnector(inConnector, outConnector, laneIdxs[i]);
            }
            }

            // For each road, Setup change lane detects
            foreach (GameObject road in roadHolders) {
                Transform lanesHolder = road.transform.GetChild(0);
                int numLanes = lanesHolder.childCount;
                GameObject[] lanes = new GameObject[numLanes];
                Road roadScript = road.GetComponent<Road>();

                // Add ChangeLaneChecker
                road.AddComponent<ChangeLaneChecker>();

                const float LANESPACING = 0.5f;
                const float BIKELENGTH = 3f;
                const float SIDESPACE = LANESPACING + BIKELENGTH/2;
                for (int i = 0; i < numLanes; ++i) {
                    Transform curLane = lanesHolder.GetChild(i);
                    lanes[i] = curLane.gameObject;

                    GameObject[] adjacentLanes;

                    if (numLanes == 1) {
                        adjacentLanes = new GameObject[0];
                    } else if (i == 0) {
                        adjacentLanes = new GameObject[] {lanesHolder.GetChild(i+1).gameObject};
                    } else if (i == numLanes-1) {
                        adjacentLanes = new GameObject[] {lanesHolder.GetChild(i-1).gameObject};
                    } else {
                        adjacentLanes = new GameObject[] {lanesHolder.GetChild(i-1).gameObject, lanesHolder.GetChild(i+1).gameObject};
                    }

                    for (int j = 0; j < curLane.childCount; ++j) {
                        GameObject waypoint = curLane.GetChild(j).gameObject;
                        WaypointSettings waypointSettings = waypoint.GetComponent<WaypointSettings>();

                        // Add change lane detect script
                        ChangeLaneDetect detectScript = waypoint.AddComponent<ChangeLaneDetect>();
                        detectScript.changeLaneMsgReceiver = road;
                        detectScript.allLaneHolders = lanes;
                        detectScript.adjacentLaneHolders = adjacentLanes;

                        // Add speed limit detect script
                        SpeedChecker speedChecker = waypoint.AddComponent<SpeedChecker>();
                        speedChecker.speedLimit = waypointSettings.maxSpeed;

                        // add collider for triggering detect scriptss
                        BoxCollider collider = waypoint.AddComponent<BoxCollider>();
                        collider.isTrigger = true;
                        const int layer_Detect = 3;
                        waypoint.layer = layer_Detect;

                        if (j < curLane.childCount-1) {
                            waypoint.transform.LookAt(curLane.GetChild(j+1));
                        }
                        collider.size = new Vector3(roadScript.laneWidth - SIDESPACE,
                                                    1,
                                                    (j > 0) ? Vector3.Distance(waypoint.transform.position, curLane.GetChild(j-1).position): 2
                                                   );
                        collider.center += new Vector3(0, 0, collider.size.z/2);
                    }
                }
            }


            RemoveNonRequiredWaypoints();

            Debug.Log("total waypoints generated " + allWaypoints.Count);

            Debug.Log("Done generating waypoints for Easy Roads");
        }


        private static void RemoveNonRequiredWaypoints()
        {
            // HACK: Modified for MetroCycle
            // for (int j = allWaypoints.Count - 1; j >= 0; j--)
            for (int j = allWaypoints.Count - 2; j >= 0; j--)
            {
                if (allWaypoints[j].GetComponent<WaypointSettings>().neighbors.Count == 0)
                {
                    DestroyImmediate(allWaypoints[j].gameObject);
                }
            }
        }


        private static void AssignIntersections(IntersectionType intersectionType)
        {
            for (int i = 0; i < connectors.Count; i++)
            {
                if (connectors[i].listIndex != -1)
                {
                    if (intersectionType == IntersectionType.Priority)
                    {
                        PriorityIntersectionSettings currentIntersection = (PriorityIntersectionSettings)allGleyIntersections[connectors[i].listIndex];
                        if (connectors[i].enter == true)
                        {
                            WaypointSettings waypointToAdd = allConnectors[i].GetComponent<WaypointSettings>();
                            if (waypointToAdd.prev.Count > 0)
                            {
                                waypointToAdd = (WaypointSettings)waypointToAdd.prev[0];
                                AssignEnterWaypoints(currentIntersection.enterWaypoints, waypointToAdd);
                            }
                            else
                            {
                                Debug.Log(waypointToAdd.name + " has no previous waypoints", waypointToAdd);
                            }
                        }

                        if (connectors[i].exit)
                        {
                            if (currentIntersection.exitWaypoints == null)
                            {
                                currentIntersection.exitWaypoints = new List<WaypointSettings>();
                            }
                            WaypointSettings waypointToAdd = allConnectors[i].GetComponent<WaypointSettings>();
                            if (waypointToAdd.neighbors.Count > 0)
                            {
                                waypointToAdd = (WaypointSettings)waypointToAdd.neighbors[0];
                                if (!currentIntersection.exitWaypoints.Contains(waypointToAdd))
                                {
                                    currentIntersection.exitWaypoints.Add(waypointToAdd);
                                }
                            }
                            else
                            {
                                Debug.Log(waypointToAdd.name + " has no neighbors.", waypointToAdd);
                            }
                        }
                    }
                    else
                    {
                        TrafficLightsIntersectionSettings currentIntersection = (TrafficLightsIntersectionSettings)allGleyIntersections[connectors[i].listIndex];
                        if (connectors[i].enter == true)
                        {
                            WaypointSettings waypoint = allConnectors[i].GetComponent<WaypointSettings>();
                            if (waypoint.prev.Count > 0)
                            {
                                AssignEnterWaypoints(currentIntersection.stopWaypoints, (WaypointSettings)allConnectors[i].GetComponent<WaypointSettings>().prev[0]);
                            }
                            else
                            {
                                Debug.Log(waypoint.name + " is not properly linked", waypoint);
                            }
                        }
                    }
                }
            }
        }


        private static void AssignEnterWaypoints(List<IntersectionStopWaypointsSettings> enterWaypoints, WaypointSettings waypointToAdd)
        {
            if (enterWaypoints == null)
            {
                enterWaypoints = new List<IntersectionStopWaypointsSettings>();
            }
            string roadName = waypointToAdd.name.Split('-')[0];
            int index = -1;

            for (int j = 0; j < enterWaypoints.Count; j++)
            {
                if (enterWaypoints[j].roadWaypoints.Count > 0)
                {
                    if (enterWaypoints[j].roadWaypoints[0].name.Contains(roadName))
                    {
                        index = j;
                    }
                }
            }
            if (index == -1)
            {
                enterWaypoints.Add(new IntersectionStopWaypointsSettings());
                index = enterWaypoints.Count - 1;
                enterWaypoints[index].roadWaypoints = new List<WaypointSettings>();
            }

            if (!enterWaypoints[index].roadWaypoints.Contains(waypointToAdd))
            {
                enterWaypoints[index].roadWaypoints.Add(waypointToAdd);
            }
        }


        private static void LinkConnectorsToRoadWaypoints()
        {
            for (int i = 0; i < allConnectors.Count; i++)
            {
                if (allConnectors[i].name.Contains(GleyUrbanAssets.Constants.connectionEdgeName))
                {
                    for (int j = 0; j < allWaypoints.Count; j++)
                    {
                        if (Vector3.Distance(allConnectors[i].position, allWaypoints[j].position) < 0.01f)
                        {
                            WaypointSettings connectorScript = allConnectors[i].GetComponent<WaypointSettings>();
                            WaypointSettings waypointScript = allWaypoints[j].GetComponent<WaypointSettings>();

                            if (connectorScript.prev.Count == 0)
                            {
                                connectorScript.prev = waypointScript.prev;
                                waypointScript.prev[0].neighbors.Remove(waypointScript);
                                waypointScript.prev[0].neighbors.Add(connectorScript);

                            }

                            if (connectorScript.neighbors.Count == 0)
                            {
                                connectorScript.neighbors = waypointScript.neighbors;
                                if (waypointScript.neighbors.Count > 0)
                                {
                                    waypointScript.neighbors[0].prev.Add(connectorScript);
                                }
                                //else
                                //{
                                //    Debug.Log(waypointScript.name + " has no neighbors", waypointScript);
                                //}
                            }
                            break;
                        }
                    }
                }
            }
        }


        private static void CreateConnectorWaypoints(List<int> vehicleTypes)
        {
            for (int i = 0; i < connectors.Count; i++)
            {
                allConnectors.Add(CreateInstance<WaypointGeneratorTraffic>().CreateWaypoint(connectionParents[i], connectors[i].position, connectors[i].name, vehicleTypes, connectors[i].maxSpeed, null));
            }
        }


        private static void CreateTrafficWaypoints(List<int> vehicleTypes)
        {
            for (int i = 0; i < points.Count; i++)
            {
                allWaypoints.Add(CreateInstance<WaypointGeneratorTraffic>().CreateWaypoint(waypointParents[i], points[i].position, points[i].name, vehicleTypes, points[i].maxSpeed, null));
            }
        }


        private static void AddIntersections(Transform intersectionHolder, IntersectionType intersectionType, float greenLightTime, float yellowLightTime)
        {
            allERIntersections = FindObjectsOfType<ERCrossings>();
            for (int i = 0; i < allERIntersections.Length; i++)
            {
                GameObject intersection = new GameObject(allERIntersections[i].name);
                intersection.transform.SetParent(intersectionHolder);
                intersection.transform.position = allERIntersections[i].gameObject.transform.position;
                GenericIntersectionSettings intersectionScript = null;
                switch (intersectionType)
                {
                    case IntersectionType.Priority:
                        intersectionScript = intersection.AddComponent<PriorityIntersectionSettings>();
                        break;
                    case IntersectionType.TrafficLights:
                        intersectionScript = intersection.AddComponent<TrafficLightsIntersectionSettings>();
                        ((TrafficLightsIntersectionSettings)intersectionScript).greenLightTime = greenLightTime;
                        ((TrafficLightsIntersectionSettings)intersectionScript).yellowLightTime = yellowLightTime;
                        break;
                    default:
                        Debug.LogWarning(intersectionType + " not supported");
                        break;
                }

                allGleyIntersections.Add(intersectionScript);
            }
        }


        private static void LinkAllConnectors(Transform holder)
        {
            for (int r = 0; r < holder.childCount; r++)
            {
                for (int i = 0; i < holder.GetChild(r).GetChild(1).childCount; i++)
                {
                    Transform laneHolder = holder.GetChild(r).GetChild(1).GetChild(i);
                    LinkWaypoints(laneHolder);
                }
            }
        }


        private static void LinkAllWaypoints(Transform holder)
        {
            for (int r = 0; r < holder.childCount; r++)
            {
                for (int i = 0; i < holder.GetChild(r).GetChild(0).childCount; i++)
                {
                    Transform laneHolder = holder.GetChild(r).GetChild(0).GetChild(i);
                    LinkWaypoints(laneHolder);
                }
            }
        }


        private static void LinkOvertakeLanes(Transform holder, int waypointDistance)
        {
            for (int i = 0; i < holder.childCount; i++)
            {
                for (int j = 0; j < holder.GetChild(i).GetChild(0).childCount; j++)
                {
                    Transform firstLane = holder.GetChild(i).GetChild(0).GetChild(j);
                    int laneToLink = j - 1;
                    if (laneToLink >= 0)
                    {
                        LinkLanes(firstLane, holder.GetChild(i).GetChild(0).GetChild(laneToLink), waypointDistance);
                    }
                    laneToLink = j + 1;
                    if (laneToLink < holder.GetChild(i).GetChild(0).childCount)
                    {
                        LinkLanes(firstLane, holder.GetChild(i).GetChild(0).GetChild(laneToLink), waypointDistance);
                    }
                }
            }
        }


        private static void LinkLanes(Transform firstLane, Transform secondLane, int waypointDistance)
        {
            if (secondLane.name.Split('_')[2] == firstLane.name.Split('_')[2])
            {
                LinkLaneWaypoints(firstLane, secondLane, waypointDistance);
            }
        }


        private static void LinkLaneWaypoints(Transform currentLane, Transform otherLane, int waypointDistance)
        {
            for (int i = 0; i < currentLane.childCount; i++)
            {
                int otherLaneIndex = i + waypointDistance;
                if (otherLaneIndex < currentLane.childCount - 1)
                {
                    WaypointSettings currentLaneWaypoint = currentLane.GetChild(i).GetComponent<WaypointSettings>();
                    WaypointSettings otherLaneWaypoint = otherLane.GetChild(otherLaneIndex).GetComponent<WaypointSettings>();
                    currentLaneWaypoint.otherLanes.Add(otherLaneWaypoint);
                }
            }
        }


        private static void LinkWaypoints(Transform laneHolder)
        {
            WaypointSettings previousWaypoint = laneHolder.GetChild(0).GetComponent<WaypointSettings>();
            for (int j = 1; j < laneHolder.childCount; j++)
            {
                string waypointName = laneHolder.GetChild(j).name;
                WaypointSettings waypointScript = laneHolder.GetChild(j).GetComponent<WaypointSettings>();
                if (previousWaypoint != null)
                {
                    previousWaypoint.neighbors.Add(waypointScript);
                    waypointScript.prev.Add(previousWaypoint);
                }
                if (!waypointName.Contains("Output"))
                {
                    previousWaypoint = waypointScript;
                }
                else
                {
                    previousWaypoint = null;
                }
            }
        }


        static void ExtractLaneWaypoints(int lanes, GameObject lanesHolder, ERRoad road, ERLaneDirection side, int r,
                                         List<int> inWaypoints, List<int> outWaypoints, List<int> laneIdxs)
        {
            if (lanes > 0)
            {
                Road roadScript = lanesHolder.transform.parent.gameObject.GetComponent<Road>();
                int leftLaneCount = road.GetLeftLaneCount();
                for (int i = 0; i < lanes; i++)
                {
                    Vector3[] positions = road.GetLanePoints(i, side);
                    if (positions != null)
                    {
                        int laneNum = 2*i;
                        int laneIdx = i;
                        bool laneDirection = false;
                        // HACK: Hardcode right lanes to odd numbers
                        //       For the list of Lane Connectors, all Left Lanes come first
                        if (side == ERLaneDirection.Right) {
                            laneNum += 1;
                            laneIdx += leftLaneCount;
                            laneDirection = true;
                        }

                        GameObject lane = new GameObject("Lane" + laneNum);
                        lane.name = side + "_Lane_" + laneNum;
                        lane.transform.SetParent(lanesHolder.transform);
                        roadScript.lanes[laneIdx].laneDirection = laneDirection;

                        int startIdx, endIdx, loopDir;
                        switch (side) {
                            case ERLaneDirection.Left:
                                startIdx = positions.Length-1;
                                endIdx = 0;
                                loopDir = -1;
                                break;
                            case ERLaneDirection.Right:
                                startIdx = 0;
                                endIdx = positions.Length-1;
                                loopDir = 1;
                                break;
                            default:
                                Debug.LogError("ExtractLaneWaypoints: Invalid side argument");
                                startIdx = endIdx = loopDir = -1;
                                break;
                        }

                        for (int j = startIdx; j != endIdx+loopDir; j += loopDir)
                        {
                            Waypoint waypoint = new Waypoint();
                            string prefix = "";
                            if (j == startIdx) {
                                prefix = "IN ";
                                // found inConnector, save INDEX for later
                                inWaypoints.Add(points.Count);
                            } else if (j == endIdx) {
                                prefix = "OUT ";

                                // found outConnector, save INDEX for later
                                outWaypoints.Add(points.Count);
                                laneIdxs.Add(laneIdx);
                            }

                            waypoint.name = prefix + "Road_" + r + "-" + GleyUrbanAssets.Constants.laneNamePrefix + i + "-" + GleyUrbanAssets.Constants.waypointNamePrefix + j;
                            waypoint.position = positions[j];
                            waypoint.maxSpeed = (int)road.GetSpeedLimit();
                            points.Add(waypoint);
                            waypointParents.Add(lane.transform);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No lane points found for " + road.gameObject.name + ", make sure Generate Lane Data is enabled from AI traffic", road.gameObject);
                    }
                }
            }
        }


        private static void ExtractConnectors(int lanes, ERRoad road, Transform lanesHolder, int roadIndex)
        {
            bool endConnectorsFound = true;
            bool connectorsFound = true;
            GameObject connectorGameobect = null;
            for (int i = 0; i < lanes; i++)
            {
                int connectionIndex;
                ERConnection conn = road.GetConnectionAtEnd(out connectionIndex);
                if (conn != null)
                {
                    ERLaneConnector[] laneConnectors = conn.GetLaneData(connectionIndex, i);
                    if (laneConnectors != null)
                    {
                        ExtractLaneConnectors(conn, laneConnectors, lanesHolder, i, roadIndex, (int)road.GetSpeedLimit());
                    }
                    else
                    {
                        connectorsFound = false;
                        connectorGameobect = conn.gameObject;
                    }
                }
                else
                {
                    endConnectorsFound = false;
                }

                conn = road.GetConnectionAtStart(out connectionIndex);
                if (conn != null)
                {
                    ERLaneConnector[] laneConnectors = conn.GetLaneData(connectionIndex, i);
                    if (laneConnectors != null)
                    {
                        ExtractLaneConnectors(conn, laneConnectors, lanesHolder, i, roadIndex, (int)road.GetSpeedLimit());
                    }
                    else
                    {
                        connectorsFound = false;
                        connectorGameobect = conn.gameObject;
                    }
                }
                else
                {
                    endConnectorsFound = false;
                }
            }

            if (endConnectorsFound == false)
            {
                Debug.LogWarning(road.gameObject + " is not connected to anything ", road.gameObject);
            }

            if (connectorsFound == false)
            {
                Debug.LogWarning("No waypoint connectors found for " + connectorGameobect + ". You should connect it manually.", connectorGameobect);
            }
        }


        private static void ExtractLaneConnectors(ERConnection conn, ERLaneConnector[] laneConnectors, Transform lanesHolder, int laneIndex, int roadIndex, int speedLimit)
        {

            if (laneConnectors != null)
            {
                for (int j = 0; j < laneConnectors.Length; j++)
                {
                    GameObject lane = new GameObject("Connector" + j);
                    lane.transform.SetParent(lanesHolder);
                    for (int k = 0; k < laneConnectors[j].points.Length; k++)
                    {
                        Waypoint waypoint = new Waypoint();
                        waypoint.listIndex = -1;
                        if (k == 0 || k == laneConnectors[j].points.Length - 1)
                        {
                            waypoint.name = "Road_" + roadIndex + "-" + GleyUrbanAssets.Constants.laneNamePrefix + laneIndex + "-" + GleyUrbanAssets.Constants.connectionEdgeName + k;
                            waypoint.listIndex = Array.FindIndex(allERIntersections, cond => cond.gameObject == conn.gameObject);
                            if (k == 0)
                            {
                                waypoint.enter = true;
                            }
                            else
                            {
                                waypoint.exit = true;
                            }
                        }
                        else
                        {
                            waypoint.name = "Road_" + roadIndex + "-" + GleyUrbanAssets.Constants.laneNamePrefix + laneIndex + "-" + GleyUrbanAssets.Constants.connectionWaypointName + k;
                        }

                        waypoint.position = laneConnectors[j].points[k];
                        waypoint.maxSpeed = speedLimit;
                        connectors.Add(waypoint);
                        connectionParents.Add(lane.transform);
                    }
                }
            }
        }
    }
}

// HACK: Implement missing ER3D funcs (only present in v3.3 beta)
public static class ERRoadExtensions{
    // HACK: encode the number of lanes in the road/material name
    //       of the format ".* [L,R]-Lane .*"
    //       where L, R are integers, and
    //          L = number of left lanes
    //          R = number of right lanes
    public static int[] GetLaneCountRaw(this ERRoad road) {
        // const string KEYWORD = "-Lane";
        const string KEYWORD = "-lane";

        string lanePart = null;
        foreach (string name in road.GetNamesWithConfig()) {
            Debug.Log("NAME " + name);
            if (name.Contains(KEYWORD)) {
                lanePart = name.Split(KEYWORD)[0];
                break;
            }
        }
        if (lanePart == null) {
            Debug.Log("ER3D road name must contain [L,R]-Lane or R-Lane. Defaulting to [0, 2].");
            return new int[] {0, 2};
        }

        if (lanePart.Contains(']')) {
            int lanePartEnd = lanePart.LastIndexOf(']');
            int lanePartStart = lanePart.LastIndexOf('[', lanePartEnd);
            Debug.Assert(lanePartStart > -1, "ER3D road name [L,R] lane part must have opening [");
            Debug.Assert(lanePartEnd > lanePartStart, "ER3D road name [L,R] lane part [ must appear after ]");

            string[] numLanes = lanePart.Substring(lanePartStart+1, (lanePartEnd-lanePartStart)-1).Split(',');
            return new int[] {int.Parse(numLanes[0]), int.Parse(numLanes[1])};
        } else {
            int lanePartStart = lanePart.LastIndexOf(' ', lanePart.Length-1);
            if (lanePartStart <= -1) {
                // GUESS: R-Lane is start of the name
                lanePartStart = -1;
            }

            int rightLanes = -1;
            if (! (int.TryParse(lanePart.Substring(lanePartStart+1), out rightLanes)) ) {
                Debug.LogError("ER3D road name R-Lane part must be preceded by space or the first part of name");
            }

            return new int[] {0, rightLanes};
        }
    }

    public static int GetLaneCount(this ERRoad road) {
        int[] numLanes = road.GetLaneCountRaw();
        return numLanes[0] + numLanes[1];
    }
    public static int GetLeftLaneCount(this ERRoad road) {
        int[] numLanes = road.GetLaneCountRaw();
        return numLanes[0];
    }
    public static int GetRightLaneCount(this ERRoad road) {
        int[] numLanes = road.GetLaneCountRaw();
        return numLanes[1];
    }

    // HACK: encode the speed limit (max) in the road/material name
    //       of the format ".* X-SpeedLimit .*"
    //       where X is an int
    public static int GetSpeedLimit(this ERRoad road) {
        const int DEFAULT_SPEED_LIMIT = 40;
        // const string KEYWORD = "-SpeedLimit";
        const string KEYWORD = "-speedlimit";

        string speedPart = null;
        foreach (string name in road.GetNamesWithConfig()) {
            if (name.Contains(KEYWORD)) {
                speedPart = name.Split(KEYWORD)[0];
                break;
            }
        }

        if (speedPart == null) {
            // Debug.Log("ER3D road nome does not contain " + KEYWORD + ". Defaulting Speed limit to " + DEFAULT_SPEED_LIMIT);
            return DEFAULT_SPEED_LIMIT;
        }

        int speedPartStart = speedPart.LastIndexOf(' ', speedPart.Length-1);
        Debug.Assert(speedPartStart > -1, "ER3D road name Speed limit part must be preced by space");

        int speedLimit = int.Parse(speedPart.Substring(speedPartStart+1));

        return speedLimit;
    }

    // HACK: encode the speed limit (max) in the road name
    //       of the format ".* X-SpeedLimit .*"
    //       where X is an int
    public static Vector3[] GetLanePoints(this ERRoad road, int laneIdx, ERLaneDirection dir) {
        Vector3[] markersCenter = road.GetSplinePointsCenter();
        Vector3[] markersSide;
        if (dir == ERLaneDirection.Left) {
            markersSide = road.GetSplinePointsLeftSide();
            // If we ONLY have left lanes, use whole width instead of just from center
            if (road.GetRightLaneCount() == 0) {
                markersCenter = road.GetSplinePointsRightSide();
            }
        } else {
            markersSide = road.GetSplinePointsRightSide();
            // If we ONLY have right lanes, use whole width instead of just from center
            if (road.GetLeftLaneCount() == 0) {
                markersCenter = road.GetSplinePointsLeftSide();
            }
        }

        Debug.Assert(markersSide.Length == markersCenter.Length, "ER3D road must have same number of side and center spline points");
        float laneWidth = road.GetLaneWidth();

        // HACK: only use every Nth marker to reduce their number
        const int EVERY_NTH_SEGMENT = 2;
        bool needAppend = ((markersSide.Length-1) % EVERY_NTH_SEGMENT) != 0;
        int numSegments = 1 + ((markersSide.Length-1) / EVERY_NTH_SEGMENT) + (needAppend ? 1 : 0);
        Vector3[] lanePoints = new Vector3[numSegments];
        int pointIdx = 0;
        for (int i = 0; i < markersSide.Length; i += EVERY_NTH_SEGMENT) {
            lanePoints[pointIdx++] = Vector3.MoveTowards(markersCenter[i], markersSide[i], (laneIdx + 0.5f)*laneWidth);
        }
        if (needAppend) {
            lanePoints[numSegments-1] = Vector3.MoveTowards(markersCenter[markersSide.Length-1], markersSide[markersSide.Length-1], (laneIdx + 0.5f)*laneWidth);
        }

        return lanePoints;
    }

    // Extra convenience functions
    public static float GetLaneWidth(this ERRoad road) {
        return road.GetWidth() / road.GetLaneCount();
    }

    public static string[] GetNamesWithConfig(this ERRoad road) {
        string roadMaterialName = "";
        ERRoadType roadType = road.GetRoadType();
        if (roadType?.roadMaterial != null) {
            roadMaterialName = roadType.roadMaterial.name;
        }
        return new string [] {road.GetName(), roadType?.roadTypeName ?? "", roadMaterialName}
            .Select((name, index) => name.ToLower()).ToArray();
    }
}
#endif
