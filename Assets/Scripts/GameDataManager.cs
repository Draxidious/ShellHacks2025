using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    // Assign your JobData assets in the Inspector
    public JobData[] jobs;

    // Find a job by name (Botanist, Doctor, etc.)
    public JobData GetJob(string name)
    {
        foreach (var job in jobs)
        {
            if (job.jobName == name) return job;
        }
        Debug.LogWarning("Job not found: " + name);
        return null;
    }
}
