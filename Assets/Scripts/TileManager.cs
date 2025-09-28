using UnityEngine;


public class TileManager : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
    }

    public void PropertyLandedOn()
    {

    }
    public void TriviaLandedOn()
    {
        Debug.Log("Trivia Landed On - TileManager");
    }
    public void EventLandedOn()
    {
        Debug.Log("Event Landed On - TileManager");
    }
}
