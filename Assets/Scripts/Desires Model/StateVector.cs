using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum State
{
    Mood,
    Energy,
    Satedness
};


public class StateVector : MonoBehaviour
{
    private int numStates;

    private int numTasks;
    private List<Task> tasks;

    private float[] values;
    private float[] startingValues;
    private float[] minValues;
    private float[] maxValues;

    private float[,] modValues;

    private int currentModVector;

    void Awake()
    {
        int[] stateEnums = (int[])System.Enum.GetValues(typeof(State));
        numStates = stateEnums.Length;

        values = new float[numStates];
        startingValues = new float[numStates];

        for (int i = 0; i < numStates; i++)
            startingValues[i] = Random.Range(-0.5f, 0.5f);

        minValues = new float[numStates];
        maxValues = new float[numStates];



        for (int i = 0; i < numStates; i++)
        {
            float middleVal = Random.Range(-0.3f, 0.3f);

            minValues[i] = Random.Range(-0.8f, middleVal - 0.1f);
            maxValues[i] = Random.Range(middleVal + 0.1f, 0.8f);
        }

        currentModVector = -1;
    }

    void Start()
    {
        for (int i = 0; i < numStates; i++)
            values[i] = startingValues[i];
    }

    void Update()
    {
        if(currentModVector != -1)
            applyModification();


        for (int i = 0; i < numStates; i++)
        {
            bool low = false;
            bool high = false;

            if (values[i] < minValues[i])
                low = true;
            else if (values[i] > maxValues[i])
                high = true;
            else
                continue;

            State state = (State)i;

            string highOrLow = "low";

            if (high)
                highOrLow = "high";

            Debug.Log("Oh no! My " + state + " is too " + highOrLow);

        }
    }

    void LateUpdate()
    {
    }

    public void setupTasks(Routine routine)
    {


        Task[] allTasks = routine.getTasks();
        tasks = new List<Task>();

        int numAllTasks = allTasks.Length;

        for (int i = 0; i < numAllTasks; i++)
        {
            if(!tasks.Contains(allTasks[i]))
                tasks.Add(allTasks[i]);
        }

        numTasks = tasks.Count;

        modValues = new float[numTasks, numStates];

        for (int i = 0; i < numTasks; i++)
            for (int j = 0; j < numStates; j++)
                modValues[i, j] = Random.Range(-0.1f, 0.1f);


        Debug.Log("lol");
    }




    private void applyModification()
    {
        for (int i = 0; i < numStates; i++)
        {
            values[i] += modValues[currentModVector, i];
            if (values[i] > 1.0f)
                values[i] = 1.0f;
            else if (values[i] < -1.0f)
                values[i] = -1.0f;
        }
        
    }

    public void startModification(Task task)
    {
        if (!tasks.Contains(task))
        {
            Debug.Log("ERROR: invalid task " + task);
            return;
        }


        for (int i = 0; i < numTasks; i++)
        {
            if (task == tasks[i])
            {
                currentModVector = i;
                break;
            }
        }
    }

    public void stopModification()
    {
        currentModVector = -1;
    }

    
}