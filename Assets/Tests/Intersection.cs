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

    [OneTimeSetUp]
    public void TestIntersectionSetup()
    {
        SceneManager.LoadScene("Assets/Scenes/TestScenes/Test_Intersection.unity");

        intersection = GameObject.Find("/IntersectionLaneDetects");
        Assert.IsNotNull(intersection);

        intersectionScript = intersection.GetComponent<IntersectionChecker>();
        Assert.IsNotNull(intersectionScript);
    }

    [UnityTest]
    public IEnumerator TestIntersectionChecks([ValueSource(nameof(IntersectionTestCases))] IntersectionTestCase tc)
    {
       yield return null;
    }

    private static IEnumerable IntersectionTestCases()
    {
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
        yield return new IntersectionTestCase {from = 0, to = 10, expectedError = ErrorReason.NOERROR};  // Proper left turn
        yield return new IntersectionTestCase {from = 0, to = 11, expectedError = ErrorReason.INTERSECTION_LEFTTURN_TO_OUTERLANE};
        yield return new IntersectionTestCase {from = 0, to = 12, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 0, to = 13, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 0, to = 14, expectedError = ErrorReason.NOERROR};   // Proper left U-turn
        yield return new IntersectionTestCase {from = 0, to = 15, expectedError = ErrorReason.INTERSECTION_LEFT_UTURN_TO_OUTERLANE};   // Proper left U-turn
        yield return new IntersectionTestCase {from = 0, to = 1, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 1, to = 1, expectedError = ErrorReason.INTERSECTION_WRONGWAY};
        yield return new IntersectionTestCase {from = 1, to = 2, expectedError = ErrorReason.INTERSECTION_RIGHTTURN_TO_OUTERLANE};
        yield return new IntersectionTestCase {from = 1, to = 3, expectedError = ErrorReason.NOERROR};  // Proper right turn
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

    public struct IntersectionTestCase
    {
        public int from;
        public int to;
        public ErrorReason expectedError;
    }
}
