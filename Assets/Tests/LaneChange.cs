using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using UnityEngine.SceneManagement;
using Metrocycle;

public class LaneChange
{
    private ChangeLaneChecker laneChangeScript;
    private Transform lanesHolder;
    private blinkers blinkerScript;
    private bool isSceneLoaded = false;

    private GameObject AICar;
    private GameObject obstacle;

    private static float minBlinkerTime = 1f;
    // NOTE: For now, the maxHeadCheckDelay is hardcoded (see value in scene) since it needs to be determined before the UnitySetup above
    // TODO:  maybe use a dynamic array for the ValueSource instead so that we can modify it in runtime?
    private static float leewayTime = 0.1f;
    private static float maxHeadCheckDelay = 1f - leewayTime;

    [UnitySetUp]
    public IEnumerator TestLaneChangeSetup()
    {
        if (isSceneLoaded) {
            yield break;
        }

        SceneManager.LoadScene("Test_LaneChange");

        yield return new WaitForSeconds(0.5f);
        isSceneLoaded = true;

        GameManager.Instance.isTestMode = true;

        GameObject laneChangeObj = GameObject.Find("/ChangeLaneChecker");
        Debug.Log("CHECKING laneChangeObj");
        Assert.IsNotNull(laneChangeObj);

        GameObject AICar = GameObject.Find("/AICar");
        Debug.Log($"CHECKING AICar {AICar}");
        Assert.IsNotNull(AICar);

        GameObject obstacle = GameObject.Find("/Obstacle");
        Debug.Log($"CHECKING obstacle {obstacle}");
        Assert.IsNotNull(obstacle);

        laneChangeScript = laneChangeObj.GetComponent<ChangeLaneChecker>();
        Debug.Log("CHECKING laneChangeScript");
        Assert.IsNotNull(laneChangeScript);

        lanesHolder = laneChangeScript.bikeLane.transform.parent;
        Debug.Log("CHECKING lanesHolder");
        Assert.IsNotNull(lanesHolder);

        for (int i = 0; i < lanesHolder.childCount; ++i) {
            Debug.Log($"CHILD {i} {lanesHolder.GetChild(i)}");
        }

        blinkerScript = GameManager.Instance.getBlinkers().GetComponent<blinkers>();
        Debug.Log("CHECKING blinkerScript");
        Assert.IsNotNull(blinkerScript);

        // NOTE: For now, the blinker time is hardcoded (see value in scene) since it needs to be determined before the UnitySetup above
        // TODO:  maybe use a dynamic array for the ValueSource instead so that we can modify it in runtime?
        // minBlinkerTime = blinkerScript.minBlinkerTime + leewayTime;

        GameManager.Instance.resetSignal.AddListener(() => {
            Debug.Log($"GAME RESET blinker {blinkerScript.leftStatus + blinkerScript.rightStatus == 0}{blinkerScript.blinkerActivationTime} {blinkerScript.blinkerOffTime} Headcheck {GameManager.Instance.HeadCheckScript.leftCheckTime} {GameManager.Instance.HeadCheckScript.rightCheckTime}");
        });

        // Some time-based timers don't work if current clock (Time.time) is near 0, so wait for a few seconds
        yield return new WaitForSeconds(minBlinkerTime + maxHeadCheckDelay + leewayTime);
    }

    [UnityTest]
    public IEnumerator BlinkerAndHeadCheckBasic([ValueSource(nameof(BlinkerAndHeadCheckBasicTestCases))] LaneChangeTestCase tc)
    {
        yield return GenericLaneChangeTest(tc, true);
    }

    [UnityTest]
    public IEnumerator BlinkerAndHeadCheck([ValueSource(nameof(BlinkerAndHeadCheckTestCases))] LaneChangeTestCase tc)
    {
        yield return GenericLaneChangeTest(tc, true, true);
    }

    private IEnumerator GenericLaneChangeTest(LaneChangeTestCase tc,
                                                bool needHeadCheckAndBlinkers=false,
                                                bool doAHeadCheckBeforeBlinker=false)
    {
        GameManager.Instance.resetErrorReason();
        GameManager.Instance.PopupSystem.closePopup();
        yield return new WaitForSeconds(0.1f);

        Debug.Log($"LANE CHANGE TESTING FROM {tc.from} TO {tc.to}");
        if (needHeadCheckAndBlinkers) {
            if (!tc.doBlinker) {
                Debug.Log("\tWITHOUT Blinker");
            }

            if (!tc.doHeadCheck) {
                Debug.Log("\tWITHOUT Head Check");
            }
        }

        // Reset game to turn off blinkers, reset headcheck timers
        GameManager.Instance.resetSignal.Invoke();
        // --- Simulate the driver driving through the intersection by entering the lane at index *from* and exiting the lane at index *to*

        // Touch first detect in *from* lane
        GameManager.Instance.teleportBike(lanesHolder.GetChild(tc.from).GetChild(0));
        yield return new WaitForSeconds(0.1f);

        if (doAHeadCheckBeforeBlinker) {
            simulateHeadCheck(tc.dir);
            yield return new WaitForSeconds(leewayTime);
        }

        // Use blinker and do headcheck first before entering intersection
        if (needHeadCheckAndBlinkers) {
            if (tc.doBlinker) {
                blinkerScript.setBlinker(tc.dir, BlinkerStatus.ON);
                yield return new WaitForSeconds(tc.blinkerTime);
            }
            if (tc.doHeadCheck) {
                simulateHeadCheck(tc.dir);
                yield return new WaitForSeconds(tc.headCheckTime);
            }
        }

        // Touch first detect in *to* lane
        GameManager.Instance.teleportBike(lanesHolder.GetChild(tc.to).GetChild(0));
        yield return new WaitForSeconds(0.1f);

        // Check error code
        Assert.AreEqual(tc.expectedError, GameManager.Instance.getLastErrorReason());

        GameManager.Instance.resetErrorReason();
        GameManager.Instance.PopupSystem.closePopup();

        yield return null;
    }

    private static IEnumerable BlinkerAndHeadCheckBasicTestCases()
    {
        // First, test proper lane chang
        yield return new LaneChangeTestCase {from = 5, to = 4, expectedError = ErrorReason.NOERROR, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };  // Proper lane change to the left
        yield return new LaneChangeTestCase {from = 4, to = 5, expectedError = ErrorReason.NOERROR, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };   // Proper lane change to the right

        // Next, test no blinker AND no headcheck
        // NOTE: blinker is checked first, so we have no blinker errors
        yield return new LaneChangeTestCase {from = 5, to = 4, expectedError = ErrorReason.LEFTTURN_NO_BLINKER, dir = Direction.LEFT,
            doBlinker = false, blinkerTime = minBlinkerTime,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };  // lane change to left
        yield return new LaneChangeTestCase {from = 4, to = 5, expectedError = ErrorReason.RIGHTTURN_NO_BLINKER, dir = Direction.RIGHT,
            doBlinker = false, blinkerTime = minBlinkerTime,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };  // lane change to right

        // Next, test with headcheck, NO blinker
        yield return new LaneChangeTestCase {from = 5, to = 4, expectedError = ErrorReason.LEFTTURN_NO_BLINKER, dir = Direction.LEFT,
            doBlinker = false, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };  // lane change to left
        yield return new LaneChangeTestCase {from = 4, to = 5, expectedError = ErrorReason.RIGHTTURN_NO_BLINKER, dir = Direction.RIGHT,
            doBlinker = false, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };  // lane change to right

        // Next, test with blinker, NO headcheck
        yield return new LaneChangeTestCase {from = 5, to = 4, expectedError = ErrorReason.LEFTTURN_NO_HEADCHECK, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };  // lane change to left
        yield return new LaneChangeTestCase {from = 4, to = 5, expectedError = ErrorReason.RIGHTTURN_NO_HEADCHECK, dir = Direction.RIGHT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };  // lane change to right
    }

    private static IEnumerable BlinkerAndHeadCheckTestCases()
    {
        Debug.Log("MIN BLINKER TIME" + minBlinkerTime);

        // Test short blinker time
        yield return new LaneChangeTestCase {from = 5, to = 4, expectedError = ErrorReason.SHORT_BLINKER_TIME, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime / 4,
            doHeadCheck = true, headCheckTime = 0f,
        };  // lane change to left
        yield return new LaneChangeTestCase {from = 4, to = 5, expectedError = ErrorReason.SHORT_BLINKER_TIME, dir = Direction.RIGHT,
            doBlinker = true, blinkerTime = minBlinkerTime / 4,
            doHeadCheck = true, headCheckTime = 0f,
        };  // lane change to right

        // Test wrong blinker direction
        yield return new LaneChangeTestCase {from = 5, to = 4, expectedError = ErrorReason.WRONG_BLINKER, dir = Direction.RIGHT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };  // lane change to left
        yield return new LaneChangeTestCase {from = 4, to = 5, expectedError = ErrorReason.WRONG_BLINKER, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };  // lane change to right

        // Test exoired head check (head check done, but too early, e.g. 10s before turn. Needs another head check)
        yield return new LaneChangeTestCase {from = 5, to = 4, expectedError = ErrorReason.EXPIRED_HEADCHECK, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay*2,
        };  // lane change to left
        yield return new LaneChangeTestCase {from = 4, to = 5, expectedError = ErrorReason.EXPIRED_HEADCHECK, dir = Direction.RIGHT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay*2,
        };  // lane change to right

        // Test head check done BEFORE blinker (head check is done by main test code)
        yield return new LaneChangeTestCase {from = 5, to = 4, expectedError = ErrorReason.NO_HEADCHECK_AFTER_BLINKER, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime + maxHeadCheckDelay*3,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };  // lane change to left
        yield return new LaneChangeTestCase {from = 4, to = 5, expectedError = ErrorReason.NO_HEADCHECK_AFTER_BLINKER, dir = Direction.RIGHT,
            doBlinker = true, blinkerTime = minBlinkerTime + maxHeadCheckDelay*3,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };  // lane change to right
    }

    public struct LaneChangeTestCase
    {
        public int from;
        public int to;
        public ErrorReason expectedError;
        public Direction dir;
        public bool doBlinker;
        public float blinkerTime;       // Time between blinker and headcheck
        public bool doHeadCheck;
        public float headCheckTime;     // Time between head check and turn
    }

    private void simulateHeadCheck(Direction dir)
    {
        // NOTE: Currently, headcheck inputs are tied to KeyDown/KeyUp events which are hard to decouple
        //       and Unity does not have a native way to simulate Key Presses
        // HACK: to simulate headcheck, simply change last headcheck time
        if (dir == Direction.LEFT) {
            GameManager.Instance.HeadCheckScript.leftCheckTime = Time.time;
        } else {
            GameManager.Instance.HeadCheckScript.rightCheckTime = Time.time;
        }
    }

    [UnityTest]
    public IEnumerator TestAIVehicleCollision()
    {
        GameManager.Instance.resetErrorReason();
        GameManager.Instance.PopupSystem.closePopup();
        yield return new WaitForSeconds(0.1f);


        AICar = GameObject.Find("/AICar");
        Debug.Log(AICar);
        GameManager.Instance.teleportBike(AICar.transform);
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(ErrorReason.COLLISION_AIVEHICLE, GameManager.Instance.getLastErrorReason());
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestObstacleCollision()
    {
        GameManager.Instance.resetErrorReason();
        GameManager.Instance.PopupSystem.closePopup();
        yield return new WaitForSeconds(0.1f);

        obstacle = GameObject.Find("/Obstacle");
        Debug.Log(obstacle);
        GameManager.Instance.teleportBike(obstacle.transform);
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(ErrorReason.COLLISION_OBSTACLE, GameManager.Instance.getLastErrorReason());
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestBusLane()
    {
        GameManager.Instance.resetErrorReason();
        GameManager.Instance.PopupSystem.closePopup();
        yield return new WaitForSeconds(0.1f);

        laneChangeScript.busLane.gameObject.SetActive(true);
        GameManager.Instance.teleportBike(laneChangeScript.busLane.transform.GetChild(0));

        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(ErrorReason.EXCLUSIVE_BUSLANE, GameManager.Instance.getLastErrorReason());
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestNonBikeLane()
    {
        GameManager.Instance.resetErrorReason();
        GameManager.Instance.PopupSystem.closePopup();
        yield return new WaitForSeconds(0.1f);

        lanesHolder.GetChild(1).gameObject.SetActive(true);
        GameManager.Instance.teleportBike(lanesHolder.GetChild(1).GetChild(0));

        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(ErrorReason.BIKE_NOTALLOWED, GameManager.Instance.getLastErrorReason());
        yield return null;
    }

    // NOTE: almost exactly the same as TestNonBikeLane above, except we set isBikeRoad = true
    //       hence, all non-bike lanes in this road should become (non-exclusive) bike lanes
    [UnityTest]
    public IEnumerator TestIsBikeRoad()
    {
        laneChangeScript.isBikeRoad = true;

        GameManager.Instance.resetErrorReason();
        GameManager.Instance.PopupSystem.closePopup();
        GameManager.Instance.bike.SetActive(false);
        GameManager.Instance.resetSignal.Invoke();
        yield return new WaitForSeconds(0.1f);

        lanesHolder.GetChild(1).gameObject.SetActive(true);
        GameManager.Instance.teleportBike(lanesHolder.GetChild(1).GetChild(0));
        GameManager.Instance.bike.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(ErrorReason.NOERROR, GameManager.Instance.getLastErrorReason());
        yield return null;
    }

    [OneTimeTearDown]
    public void testsDone()
    {
        GameManager.Instance.isTestMode = false;
    }
}
