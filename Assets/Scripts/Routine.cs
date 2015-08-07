using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class Routine
{
    Task[] tasks;
    float[] timetable;

    public Routine(TextAsset tA)
    {
        GenerateRoutine(tA);
    }

    private void GenerateRoutine(TextAsset tA)
    {
        string[] entries = tA.text.Split('\n');
        string[] splits;
        tasks = new Task[entries.Length];
        timetable = new float[entries.Length];

        for(int i = 0; i < entries.Length; i++)
        {
            splits = entries[i].Split(',');
            timetable[i] = float.Parse(splits[0].Trim()) * 60.0f;             //Gets the execution time in minutes.
            tasks[i] = (Task)Enum.Parse(typeof(Task), splits[1].Trim());     //Gets the task to start executing at that time.
        }
    }

    public Task GetCurrentTask(float currentTime)
    {
        for(int i = 1; i < timetable.Length; i++)
        {
            if (timetable[i] > currentTime)
                return tasks[i - 1];
        }

        return tasks[tasks.Length - 1];     //In this case, the only valid action is the last one.
    }

    public Task[] GetTasks()
    {
        return tasks;
    }
}
