using mediapipe.wrapper;
using UnityEngine;
using UnityEngine.Events;

public class HandTrackingManager : MonoBehaviour
{
    Handtracking handTracker;

    Handtracking.TrackingData latestData;
    volatile Handtracking.TrackingData newTrackingdata;

    [SerializeField]
    UnityEvent<Handtracking.TrackingData> TrackingEvent = new UnityEvent<Handtracking.TrackingData>();

    private void Awake()
    {
        handTracker = new Handtracking();
        handTracker.OnData.AddListener(data => newTrackingdata = data);
    }

    private void Update()
    {
        if (latestData != newTrackingdata)
        {
            latestData = newTrackingdata;
            TrackingEvent.Invoke(latestData);
        }
    }

    private void OnDestroy()
    {
        handTracker?.Dispose();
    }

    public void OnInputTexture(Texture texture)
    {
        handTracker.Update(texture);
    }
}
