using mediapipe.wrapper;
using UnityEngine;

public class DebugTrackingData : MonoBehaviour
{
    public void OnTrackingData(Handtracking.TrackingData data)
    {
        Debug.Log($"Timestamp: {data.timestamp}\nHand count: {data.hands.Length}\n");
    }
}
