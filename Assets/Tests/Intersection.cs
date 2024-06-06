using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using UnityEngine.SceneManagement;
using Metrocycle;

public class Intersection
{
    private GameObject intersection;
    private IntersectionChecker intersectionScript;
    private blinkers blinkerScript;
    private GameObject greenLight;
    private bool isSceneLoaded = false;

    private static float minBlinkerTime = 1f;
    // NOTE: For now, the maxHeadCheckDelay is hardcoded (see value in scene) since it needs to be determined before the UnitySetup above
    // TODO:  maybe use a dynamic array for the ValueSource instead so that we can modify it in runtime?
    private static float leewayTime = 0.1f;
    private static float maxHeadCheckDelay = 1f - leewayTime;

    [UnitySetUp]
    public IEnumerator TestIntersectionSetup()
    {
        if (isSceneLoaded) {
            yield break;
        }

        SceneManager.LoadScene("Test_Intersection");


        yield return new WaitForSeconds(0.5f);
        isSceneLoaded = true;

        GameManager.Instance.isTestMode = true;

        intersection = GameObject.Find("/IntersectionLaneDetects");
        Debug.Log("CHECKING intersection");
        Assert.IsNotNull(intersection);

        intersectionScript = intersection.GetComponent<IntersectionChecker>();
        Debug.Log("CHECKING intersectionScript");
        Assert.IsNotNull(intersectionScript);

        blinkerScript = GameManager.Instance.getBlinkers().GetComponent<blinkers>();
        Debug.Log("CHECKING blinkerScript");
        Assert.IsNotNull(blinkerScript);

        greenLight = GameObject.Find("/GreenLightOn");
        Debug.Log("CHECKING greenLight");
        Assert.IsNotNull(greenLight);

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
    public IEnumerator TestIntersectionChecks([ValueSource(nameof(IntersectionTestCases))] IntersectionTestCase tc)
    {
        yield return GenericIntersectionTest(tc);
    }

    [UnityTest]
    public IEnumerator BlinkerAndHeadCheckBasic([ValueSource(nameof(BlinkerAndHeadCheckBasicTestCases))] IntersectionTestCase tc)
    {
        yield return GenericIntersectionTest(tc, true);
    }

    [UnityTest]
    public IEnumerator BlinkerAndHeadCheck([ValueSource(nameof(BlinkerAndHeadCheckTestCases))] IntersectionTestCase tc)
    {
        yield return GenericIntersectionTest(tc, true, true);
    }

    private static IEnumerable RedLightTestCases()
    {
        yield return new IntersectionTestCase {from = 0, to = 6, expectedError = ErrorReason.INTERSECTION_REDLIGHT};
        yield return new IntersectionTestCase {from = 1, to = 7, expectedError = ErrorReason.INTERSECTION_REDLIGHT};
    }

    [UnityTest]
    public IEnumerator TestRedLight([ValueSource(nameof(RedLightTestCases))] IntersectionTestCase tc)
    {
        greenLight.SetActive(false);
        yield return GenericIntersectionTest(tc);
        greenLight.SetActive(true);
    }

    private IEnumerator GenericIntersectionTest(IntersectionTestCase tc,
                                                bool needHeadCheckAndBlinkers=false,
                                                bool doAHeadCheckBeforeBlinker=false)
    {
        // NOTE: Intersection can be "rotated" by adding 4 to index (see IntersectionChecker.cs), so test all combinations
        for (int i = 0; i < intersectionScript.laneDetects.Length; i += 4) {
            tc.from = (tc.from+i) % intersectionScript.laneDetects.Length;
            tc.to = (tc.to+i) % intersectionScript.laneDetects.Length;

            GameManager.Instance.resetErrorReason();
            GameManager.Instance.PopupSystem.closePopup();

            Debug.Log($"INTERSECTION TESTING FROM {tc.from} TO {tc.to}");
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
            // Touch lane detect of *from*
            GameManager.Instance.teleportBike(intersectionScript.laneDetects[tc.from].transform);
            // Touch lane detect of *to*.
            yield return new WaitForSeconds(0.1f);
            GameManager.Instance.teleportBike(intersectionScript.laneDetects[tc.to].transform);
            yield return new WaitForSeconds(0.1f);

            // Check error code
            Assert.AreEqual(tc.expectedError, GameManager.Instance.getLastErrorReason());

            GameManager.Instance.resetErrorReason();
            GameManager.Instance.PopupSystem.closePopup();
        }

        yield return null;
    }

    private static IEnumerable IntersectionTestCases()
    {
        // NOTE: These are simply all combinations of going from the references indices (0 or 1) to ALL other indices
        // HACK: Since intersection checks are already coupled with head checks and blinker checks, we skip checking those for now
        yield return new IntersectionTestCase {from = 0, to = 0, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 0, to = 1, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 0, to = 2, expectedError = ErrorReason.INTERSECTION_RIGHTTURN_FROM_OUTERLANE};
        yield return new IntersectionTestCase {from = 0, to = 3, expectedError = ErrorReason.INTERSECTION_RIGHTTURN_FROM_OUTERLANE};
        yield return new IntersectionTestCase {from = 0, to = 4, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 0, to = 5, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 0, to = 6, expectedError = ErrorReason.NOERROR};   // Go straight
        yield return new IntersectionTestCase {from = 0, to = 7, expectedError = ErrorReason.NOERROR};   // Go straight + lane change ???
        yield return new IntersectionTestCase {from = 0, to = 8, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 0, to = 9, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
            // yield return new IntersectionTestCase {from = 0, to = 10, expectedError = ErrorReason.NOERROR};  // Proper left turn
        yield return new IntersectionTestCase {from = 0, to = 11, expectedError = ErrorReason.INTERSECTION_LEFTTURN_TO_OUTERLANE};
        yield return new IntersectionTestCase {from = 0, to = 12, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 0, to = 13, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
            // yield return new IntersectionTestCase {from = 0, to = 14, expectedError = ErrorReason.NOERROR};   // Proper left U-turn
        yield return new IntersectionTestCase {from = 0, to = 15, expectedError = ErrorReason.INTERSECTION_LEFT_UTURN_TO_OUTERLANE};   // Proper left U-turn
        yield return new IntersectionTestCase {from = 0, to = 1, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 1, to = 1, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 1, to = 2, expectedError = ErrorReason.INTERSECTION_RIGHTTURN_TO_OUTERLANE};
            // yield return new IntersectionTestCase {from = 1, to = 3, expectedError = ErrorReason.NOERROR};  // Proper right turn
        yield return new IntersectionTestCase {from = 0, to = 4, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 0, to = 5, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 1, to = 6, expectedError = ErrorReason.NOERROR};   // Go straight  + lane change ???
        yield return new IntersectionTestCase {from = 1, to = 7, expectedError = ErrorReason.NOERROR};   // Go straight
        yield return new IntersectionTestCase {from = 1, to = 8, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 1, to = 9, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 1, to = 10, expectedError = ErrorReason.INTERSECTION_LEFTTURN_FROM_OUTERLANE};
        yield return new IntersectionTestCase {from = 1, to = 11, expectedError = ErrorReason.INTERSECTION_LEFTTURN_FROM_OUTERLANE};
        yield return new IntersectionTestCase {from = 1, to = 12, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 1, to = 13, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 1, to = 14, expectedError = ErrorReason.INTERSECTION_LEFT_UTURN_FROM_OUTERLANE};
        yield return new IntersectionTestCase {from = 1, to = 15, expectedError = ErrorReason.INTERSECTION_LEFT_UTURN_FROM_OUTERLANE};
    }

    private static IEnumerable BlinkerAndHeadCheckBasicTestCases()
    {
        Debug.Log("MIN BLINKER TIME" + minBlinkerTime);

        // First, test proper turning
        yield return new IntersectionTestCase {from = 0, to = 10, expectedError = ErrorReason.NOERROR, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };  // Proper left turn
        yield return new IntersectionTestCase {from = 0, to = 14, expectedError = ErrorReason.NOERROR, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };   // Proper left U-turn
        yield return new IntersectionTestCase {from = 1, to = 3, expectedError = ErrorReason.NOERROR, dir = Direction.RIGHT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };  // Proper right turn

        // Next, test no blinker AND no headcheck
        // NOTE: blinker is checked first, so we have no blinker errors
        yield return new IntersectionTestCase {from = 0, to = 10, expectedError = ErrorReason.LEFTTURN_NO_BLINKER, dir = Direction.LEFT,
            doBlinker = false, blinkerTime = minBlinkerTime,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };  // left turn
        yield return new IntersectionTestCase {from = 0, to = 14, expectedError = ErrorReason.LEFTTURN_NO_BLINKER, dir = Direction.LEFT,
            doBlinker = false, blinkerTime = minBlinkerTime,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };   // left U-turn
        yield return new IntersectionTestCase {from = 1, to = 3, expectedError = ErrorReason.RIGHTTURN_NO_BLINKER, dir = Direction.RIGHT,
            doBlinker = false, blinkerTime = minBlinkerTime,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };  // right turn

        // Next, test with headcheck, NO blinker
        yield return new IntersectionTestCase {from = 0, to = 10, expectedError = ErrorReason.LEFTTURN_NO_BLINKER, dir = Direction.LEFT,
            doBlinker = false, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };  // left turn
        yield return new IntersectionTestCase {from = 0, to = 14, expectedError = ErrorReason.LEFTTURN_NO_BLINKER, dir = Direction.LEFT,
            doBlinker = false, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };   // left U-turn
        yield return new IntersectionTestCase {from = 1, to = 3, expectedError = ErrorReason.RIGHTTURN_NO_BLINKER, dir = Direction.RIGHT,
            doBlinker = false, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };  // right turn

        // Next, test with blinker, NO headcheck
        yield return new IntersectionTestCase {from = 0, to = 10, expectedError = ErrorReason.LEFTTURN_NO_HEADCHECK, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };  // left turn
        yield return new IntersectionTestCase {from = 0, to = 14, expectedError = ErrorReason.LEFTTURN_NO_HEADCHECK, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };   // left U-turn
        yield return new IntersectionTestCase {from = 1, to = 3, expectedError = ErrorReason.RIGHTTURN_NO_HEADCHECK, dir = Direction.RIGHT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };  // right turn
    }

    private static IEnumerable BlinkerAndHeadCheckTestCases()
    {
        Debug.Log("MIN BLINKER TIME" + minBlinkerTime);

        // Test short blinker time
        yield return new IntersectionTestCase {from = 0, to = 10, expectedError = ErrorReason.SHORT_BLINKER_TIME, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime / 4,
            doHeadCheck = true, headCheckTime = 0f,
        };  // left turn
        yield return new IntersectionTestCase {from = 0, to = 14, expectedError = ErrorReason.SHORT_BLINKER_TIME, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime / 4,
            doHeadCheck = true, headCheckTime = 0f,
        };   // left U-turn
        yield return new IntersectionTestCase {from = 1, to = 3, expectedError = ErrorReason.SHORT_BLINKER_TIME, dir = Direction.RIGHT,
            doBlinker = true, blinkerTime = minBlinkerTime / 4,
            doHeadCheck = true, headCheckTime = 0f,
        };  // right turn

        // Test wrong blinker direction
        yield return new IntersectionTestCase {from = 0, to = 10, expectedError = ErrorReason.WRONG_BLINKER, dir = Direction.RIGHT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };  // left turn
        yield return new IntersectionTestCase {from = 0, to = 14, expectedError = ErrorReason.WRONG_BLINKER, dir = Direction.RIGHT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };   // left U-turn
        yield return new IntersectionTestCase {from = 1, to = 3, expectedError = ErrorReason.WRONG_BLINKER, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay,
        };  // right turn

        // Test exoired head check (head check done, but too early, e.g. 10s before turn. Needs another head check)
        yield return new IntersectionTestCase {from = 0, to = 10, expectedError = ErrorReason.EXPIRED_HEADCHECK, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay*2,
        };  // left turn
        yield return new IntersectionTestCase {from = 0, to = 14, expectedError = ErrorReason.EXPIRED_HEADCHECK, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay*2,
        };   // left U-turn
        yield return new IntersectionTestCase {from = 1, to = 3, expectedError = ErrorReason.EXPIRED_HEADCHECK, dir = Direction.RIGHT,
            doBlinker = true, blinkerTime = minBlinkerTime,
            doHeadCheck = true, headCheckTime = maxHeadCheckDelay*2,
        };  // right turn

        // Test head check done BEFORE blinker (head check is done by main test code)
        yield return new IntersectionTestCase {from = 0, to = 10, expectedError = ErrorReason.NO_HEADCHECK_AFTER_BLINKER, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime + maxHeadCheckDelay*3,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };  // left turn
        yield return new IntersectionTestCase {from = 0, to = 14, expectedError = ErrorReason.NO_HEADCHECK_AFTER_BLINKER, dir = Direction.LEFT,
            doBlinker = true, blinkerTime = minBlinkerTime + maxHeadCheckDelay*3,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };   // left U-turn
        yield return new IntersectionTestCase {from = 1, to = 3, expectedError = ErrorReason.NO_HEADCHECK_AFTER_BLINKER, dir = Direction.RIGHT,
            doBlinker = true, blinkerTime = minBlinkerTime + maxHeadCheckDelay*3,
            doHeadCheck = false, headCheckTime = maxHeadCheckDelay,
        };  // right turn
    }

    public struct IntersectionTestCase
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

    [OneTimeTearDown]
    public void testsDone()
    {
        GameManager.Instance.isTestMode = false;
    }
}
