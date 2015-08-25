using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class OldStateVector : MonoBehaviour
{
    public bool allowAllTasks;


    private int numStates;

    private int numTasks;
    private List<Task> tasks;

    private float[] values;
    private float[] startingValues;
    private float[] minValues;
    private float[] maxValues;

    private float[,] modValues;

    private int currentModVector;


    public float updateFrequency = 15.0f; //updates every 15 minutes
    public float giveUpTime = 30.0f;


    private float lastTime;
    private float timePassed;

    private List<Task> bestTasks;
    private bool notSatisfied = false;

    private bool readyForTaskChange = false;
    private bool mustChange = false;


    void Awake()
    {
        bestTasks = new List<Task>();

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

        lastTime = DaylightScript.GetCurrentTime();
    }

    void Update()
    {
        timePassed = GetTimePassed();

        if (timePassed >= updateFrequency)
        {
            if ((currentModVector != -1) || (timePassed >= giveUpTime))
            {
                if (timePassed >= giveUpTime)
                    mustChange = true;
                else
                    ApplyModification(timePassed);


                readyForTaskChange = true;

                lastTime = DaylightScript.GetCurrentTime();
            }
        }
    }

    void LateUpdate()
    {
        if (timePassed >= updateFrequency)
        {
            bool[] tooHigh = new bool[numStates];
            bool[] tooLow = new bool[numStates];
            notSatisfied = false;

            for (int i = 0; i < numStates; i++)
            {
                tooHigh[i] = false;
                tooLow[i] = false;

                if (values[i] < minValues[i])
                    tooLow[i] = true;
                else if (values[i] > maxValues[i])
                    tooHigh[i] = true;
                else
                    continue;

                notSatisfied = true;

                State state = (State)i;

                string highOrLow = "low";

                if (tooHigh[i])
                    highOrLow = "high";


                Debug.Log("Oh no! My " + state + " is too " + highOrLow);
            }

            bestTasks.Clear();

            if (notSatisfied)
            {

                for (int i = 0; i < numTasks; i++)
                {
                    bool isBestTask = true;
                    for (int j = 0; j < numStates; j++)
                    {
                        if (tooHigh[j])
                        {
                            if (modValues[i, j] >= 0.0f)
                            {
                                isBestTask = false;
                                break;
                            }
                        }
                        else if (tooLow[j])
                        {
                            if (modValues[i, j] <= 0.0f)
                            {
                                isBestTask = false;
                                break;
                            }
                        }
                    }
                    if (isBestTask)
                        bestTasks.Add(tasks[i]);
                }
            }


        }
    }



    public void SetupTasks( Routine routine )
    {
        tasks = new List<Task>();

        if (allowAllTasks)
        {
            int numAllTasks = System.Enum.GetNames(typeof(Task)).Length;

            for (int i = 0; i < numAllTasks; i++)
            {
                Task currentTask = (Task)i;
                if (currentTask != Task.Idle)
                    tasks.Add(currentTask);
            }
        }
        else
        {
            Task[] allTasks = routine.GetTasks();

            int numAllTasks = allTasks.Length;

            for (int i = 0; i < numAllTasks; i++)
            {
                if (!tasks.Contains(allTasks[i]))
                    tasks.Add(allTasks[i]);
            }
        }

        numTasks = tasks.Count;

        modValues = new float[numTasks, numStates];

        for (int i = 0; i < numTasks; i++)
            for (int j = 0; j < numStates; j++)
                modValues[i, j] = Random.Range(-0.1f, 0.1f);
    }



    private void ApplyModification( float timeDiff )
    {
        if (currentModVector == -1)
            return;

        float proportion = timeDiff / updateFrequency;


        for (int i = 0; i < numStates; i++)
        {
            float propValue = proportion * modValues[currentModVector, i];
            values[i] = ModifyValue(values[i], propValue);
        }


    }



    public void StartModification( Task task )
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

        lastTime = DaylightScript.GetCurrentTime();
    }

    public void StopModification()
    {
        float timePassed = GetTimePassed();
        ApplyModification(timePassed);
        currentModVector = -1;
    }

    private float GetTimePassed()
    {
        float currentTime = DaylightScript.GetCurrentTime();
        float diffTime = currentTime - lastTime;

        if (diffTime < 0)
            diffTime += 24 * 60;

        return diffTime;
    }

    public Task GetBestTask( Task currentTask )
    {
        if (readyForTaskChange)
        {
            readyForTaskChange = false;

            float currentDeltaSum = GetCurrentDeltaSum();

            if ((currentDeltaSum > 0) || (mustChange))
            {
                if (mustChange)
                {
                    mustChange = false;
                    return GetBestTaskByDelta(currentDeltaSum, currentTask);
                }

                return GetBestTaskByDelta(currentDeltaSum);
            }
        }
        return currentTask;
    }



    /*
       private Task GetBestTaskOriginal(Task currentTask)
       {
           if (readyForTaskChange)
           {
               readyForTaskChange = false;

               int numBestTasks = bestTasks.Count;

               if (numBestTasks == 1)
               {
                   Debug.Log("I only have one best task. So I'll do that.");
                   return bestTasks[0];
               }

               if (numBestTasks > 0)
               {
                   if (bestTasks.Contains(currentTask))
                   {
                       Debug.Log("What I was just doing is still a good task. So I'll keep doing that.");
                       return currentTask;
                   }

                   Debug.Log("I have a number of good tasks. Will randomly select one.");
                   int randomNum = Random.Range(0, numBestTasks);
                   return bestTasks[randomNum];
               }



               if (!notSatisfied)
               {
                   Debug.Log("Have no needs right now. Will keep doing the same task.");
                   return currentTask;
               }



               Debug.Log("I have needs, but no tasks that satisfy all of them. Will do a random task.");



               int num = Random.Range(0, numTasks);
               return tasks[num];
           }
           return currentTask;
       }
       */

    private Task GetBestTaskByDelta( float currentDeltaSum )
    {
        List<Task> bestCaseTasks = new List<Task>();
        List<Task> goodCaseTasks = new List<Task>();
        List<float> goodCaseTasksDeltaDiffs = new List<float>();

        bool isBestTaskEmpty = true;

        for (int i = 0; i < numTasks; i++)
        {
            float deltaSum = GetNewDeltaSum(i);
            if (deltaSum == 0.0f)
            {
                bestCaseTasks.Add(tasks[i]);
                isBestTaskEmpty = false;
                continue;
            }

            if (isBestTaskEmpty)
            {
                float deltaDiff = currentDeltaSum - deltaSum;

                if (deltaDiff <= 0)
                    continue;

                goodCaseTasks.Add(tasks[i]);
                goodCaseTasksDeltaDiffs.Add(deltaDiff);
            }
        }

        if (!isBestTaskEmpty)
        {
            int numBestCase = bestCaseTasks.Count;

            if (numBestCase == 1)
                return bestCaseTasks[0];

            int randomNum = Random.Range(0, numBestCase);
            return bestCaseTasks[randomNum];

        }

        int numGoodCase = goodCaseTasks.Count;
        if (numGoodCase > 0)
        {
            if (numGoodCase == 1)
                return goodCaseTasks[0];


            float totalDeltaDiff = 0.0f;

            for (int i = 0; i < numGoodCase; i++)
                totalDeltaDiff += goodCaseTasksDeltaDiffs[i];

            for (int i = 0; i < numGoodCase; i++)
                goodCaseTasksDeltaDiffs[i] /= totalDeltaDiff;

            float randFloat = Random.Range(0.0f, 1.0f);
            float prevTotal = 0.0f;

            for (int i = 0; i < numGoodCase; i++)
            {
                prevTotal += goodCaseTasksDeltaDiffs[i];

                if (randFloat <= prevTotal)
                    return goodCaseTasks[i];
            }
        }

        int randNum = Random.Range(0, numTasks);
        return tasks[randNum];
    }



    private Task GetBestTaskByDelta( float currentDeltaSum, Task currentTask )
    {
        List<Task> bestCaseTasks = new List<Task>();
        List<Task> goodCaseTasks = new List<Task>();
        List<float> goodCaseTasksDeltaDiffs = new List<float>();

        for (int i = 0; i < numTasks; i++)
        {
            float deltaSum = GetNewDeltaSum(i);
            if (deltaSum == 0.0f)
            {
                bestCaseTasks.Add(tasks[i]);
                continue;
            }

            float deltaDiff = currentDeltaSum - deltaSum;

            if (deltaDiff <= 0)
                continue;

            goodCaseTasks.Add(tasks[i]);
            goodCaseTasksDeltaDiffs.Add(deltaDiff);
        }

        int numBestCase = bestCaseTasks.Count;

        while (numBestCase > 1)
        {
            int randomNum = Random.Range(0, numBestCase);

            if (bestCaseTasks[randomNum] != currentTask)
                return bestCaseTasks[randomNum];

            bestCaseTasks.RemoveAt(randomNum);
            numBestCase--;
        }

        if (numBestCase == 1)
            if (bestCaseTasks[0] != currentTask)
                return bestCaseTasks[0];


        int numGoodCase = goodCaseTasks.Count;

        float totalDeltaDiff = 0.0f;

        for (int i = 0; i < numGoodCase; i++)
            totalDeltaDiff += goodCaseTasksDeltaDiffs[i];

        while (numGoodCase > 1)
        {
            float[] propDeltas = new float[numGoodCase];

            if (totalDeltaDiff <= 0.0f)
                break;

            for (int i = 0; i < numGoodCase; i++)
                propDeltas[i] = goodCaseTasksDeltaDiffs[i] /= totalDeltaDiff;


            int rejectedTaskNum = -1;

            float randFloat = Random.Range(0.0f, 1.0f);
            float prevTotal = 0.0f;

            for (int i = 0; i < numGoodCase; i++)
            {
                prevTotal += goodCaseTasksDeltaDiffs[i];

                if (randFloat <= prevTotal)
                {
                    if (goodCaseTasks[i] != currentTask)
                        return goodCaseTasks[i];

                    rejectedTaskNum = i;
                    break;
                }
            }

            goodCaseTasks.RemoveAt(rejectedTaskNum);
            totalDeltaDiff -= goodCaseTasksDeltaDiffs[rejectedTaskNum];
            goodCaseTasksDeltaDiffs.RemoveAt(rejectedTaskNum);
            numGoodCase--;
        }

        if (numGoodCase == 1)
            if (goodCaseTasks[0] != currentTask)
                return goodCaseTasks[0];


        int falloutLimit = numTasks * 2;

        for (int i = 0; i < falloutLimit; i++)
        {
            int randNum = Random.Range(0, numTasks);

            if (tasks[randNum] != currentTask)
                return tasks[randNum];
        }

        Debug.Log("COULDNT FIND A DIFFERENT TASK");
        return currentTask;
    }

    private float GetCurrentDeltaSum()
    {
        float deltaSum = GetDeltaSum(values);

        return deltaSum;
    }

    private float GetNewDeltaSum( int taskNum )
    {
        float[] newValues = GetModifiedStateVector(taskNum);

        float deltaSum = GetDeltaSum(newValues);

        return deltaSum;
    }

    private float GetDeltaSum( float[] valueArray )
    {
        float deltaSum = 0.0f;

        for (int i = 0; i < numStates; i++)
        {
            float delta = GetDelta(i, valueArray[i]);
            deltaSum += delta;
        }

        return deltaSum;
    }


    private float GetDelta( int stateNum, float value )
    {
        float minValue = minValues[stateNum];

        if (value < minValue)
        {
            float delta = minValue - value;
            return delta;
        }

        float maxValue = maxValues[stateNum];

        if (value > maxValue)
        {
            float delta = value - maxValue;
            return delta;
        }

        return 0.0f;
    }

    private float[] GetModifiedStateVector( int taskNum )
    {
        float[] newValues = new float[numStates];

        for (int i = 0; i < numStates; i++)
            newValues[i] = ModifyValue(values[i], modValues[taskNum, i]);

        return newValues;
    }

    private float ModifyValue( float original, float change )
    {
        float newValue = original + change;

        if (newValue < -1.0f)
            return -1.0f;

        if (newValue > 1.0f)
            return 1.0f;

        return newValue;
    }

}