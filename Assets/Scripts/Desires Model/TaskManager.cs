using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TaskManager : MonoBehaviour
{
    public Task[] tasks;
    public Task work;
    public List<Task> pastimes;
    public List<Task> needs;

    private bool hasWork = false;


    void Awake()
    {
    }

    void Start()
    {
    }

    void Update()
    {

    }

    void LateUpdate()
    {
    }

    public void setupTasks(Routine routine)
    {

        tasks = routine.getTasks();
        int numTasks = tasks.Length;

        
        for(int i=0; i<numTasks; i++)
        {

            switch(tasks[i])
            {
                case Task.Blacksmith:
                    addWork(tasks[i]);
                    break;

                case Task.Fisher:
                    addWork(tasks[i]);
                    break;

                case Task.Lumberjack:
                    addWork(tasks[i]);
                    break;

                case Task.StableWorker:
                    addWork(tasks[i]);
                    break;

                case Task.Woodcutter:
                    addWork(tasks[i]);
                    break;

                case Task.Custom:    //This is for any custom initial goals you only want to apply to a specific person for testing.
                    addPastime(tasks[i]);
                    break;

                case Task.Sleep:
                    addNeed(tasks[i]);
                    break;

                case Task.Eat:
                    addNeed(tasks[i]);
                    break;

                case Task.Idle:
                    addPastime(tasks[i]);
                    break;

                case Task.Shopkeeper:
                    addWork(tasks[i]);
                    break;

                default:
                    Debug.Log(string.Format("{0} task is not properly initalised, so agent will idle.", tasks[i]));
                    break;
            }
    }

 





}

    private void addWork(Task task)
    {
        if (!hasWork)
        {
            work = task;
            hasWork = true;
            return;
        }
        
        if(work != task)
            addPastime(task);
    }
    private void addPastime(Task task)
    {
        addToList(pastimes, task);
    }
    private void addNeed(Task task)
    {
        addToList(needs, task);
    }

    private void addToList(List<Task> list, Task task)
    {
        if (!list.Contains(task))
            list.Add(task);
    }
}