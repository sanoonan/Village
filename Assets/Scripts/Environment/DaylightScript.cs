//Controls the environment's internal respresentation of the day. Keeps a 24 clock, updated at the given "internal time rate".
//Updates the direction and colour / intensity of the light representing the sun and fades between a day and night skybox.
//Controls the ambient colour, from a warm day colour to a cool night colour.
//Turns on / off street lamps and other lights at the appropriate times.
//The 24 clock can be accessed by agents when planning their day.
using UnityEngine;
using System.Collections;
public class WorldTime
{
    public enum Day
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    };

    public enum Month
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    };

    public Day currentDay = Day.Monday;
    public Month currentMonth = Month.July;
    public int currentDate = 1;

    public int currentHour = 0;
    public int currentMinute = 0;
}

public class DaylightScript : MonoBehaviour 
{
    public static float GetCurrentTime()
    {
        return DaylightScript.currentWorldTime.currentHour * 60.0f + DaylightScript.currentWorldTime.currentMinute;
    }

    public static string GetTimeStamp()
    {
        return string.Format("{0}:{1}", DaylightScript.currentWorldTime.currentHour.ToString("00"), DaylightScript.currentWorldTime.currentMinute.ToString("00"));
    }

    GameObject[] nightLights;

    public Material skybox;

    //public UILabel lblClock;

    public float dayLength = 24.0f;         //We''ll have a day be 24 minutes long.
    public float startingTime = 8.0f;       //Starting at 8am
    float hourLength;                       //So an hour is 60 seconds.
    float minuteLength;

    float currentInternalTime;
    float lightIntensity = 1.0f;

    public static WorldTime currentWorldTime = new WorldTime();

    enum DayPhase
    {
        Dawn,
        Day,
        Dusk,
        Night
    };

    DayPhase currentPhase = DayPhase.Dawn;

    float dawnTime = 4.0f;      //4am
    float dayTime = 10.0f;      //10am
    float duskTime = 19.0f;     //7pm
    float nightTime = 22.0f;    //10pm

    public Color fullDark = new Color(32.0f / 255.0f, 28.0f / 255.0f, 46.0f / 255.0f);
    public Color fullLight = new Color(253.0f / 255.0f, 248.0f / 255.0f, 223.0f / 255.0f);

    void Awake()
    {
        hourLength = dayLength / 24.0f;
        minuteLength = hourLength / 60.0f;

        //Update the times to account for the length of an in-game day, which might not be "24".
        dawnTime = dawnTime * 60.0f * (dayLength / 24.0f);
        dayTime = dayTime * 60.0f * (dayLength / 24.0f);
        duskTime = duskTime * 60.0f * (dayLength / 24.0f);
        nightTime = nightTime * 60.0f * (dayLength / 24.0f);

        dayLength *= 60.0f;

        currentInternalTime = (dayLength / 24.0f) * startingTime;
    }

    void Start()
    {
        nightLights = GameObject.FindGameObjectsWithTag("Light");
        SetDay();

        UpdateWorldTime();
        UpdateDaylight();
    }
 
    void Update() 
    {
        //// argument for cosine
        //float phi = Time.time / duration * 2 * Mathf.PI;
 
        // // get cosine in the -1 to +1 range.
        //timeOfDay = Mathf.Cos( phi );

        if ((currentInternalTime > nightTime || currentInternalTime < dawnTime) && currentPhase == DayPhase.Dusk)
            SetNight();
        else if (currentInternalTime > duskTime && currentPhase == DayPhase.Day)
            SetDusk();
        else if (currentInternalTime > dayTime && currentPhase == DayPhase.Dawn)
            SetDay();
        else if (currentInternalTime > dawnTime && currentInternalTime < dayTime && currentPhase == DayPhase.Night)
            SetDawn();

        currentInternalTime += Time.deltaTime;
        currentInternalTime = currentInternalTime % dayLength;

        UpdateWorldTime();
        UpdateDaylight();
 
        // set light color
        //light.intensity = timeOfDay * 0.5f + 0.5f;
        
    }

    private void UpdateDaylight()
    {
        if (currentPhase == DayPhase.Dawn)
        {
            float relativeTime = (currentInternalTime - dawnTime) / (dayTime - dawnTime);
            RenderSettings.ambientLight = Color.Lerp(fullDark, fullLight, relativeTime);

            if (GetComponent<Light>() != null)
                GetComponent<Light>().intensity = Mathf.SmoothStep(0.0f, lightIntensity, relativeTime);

            skybox.SetFloat("_Blend", 1.0f - relativeTime);
        }
        else if (currentPhase == DayPhase.Dusk)
        {
            float relativeTime = (currentInternalTime - duskTime) / (nightTime - duskTime);
            RenderSettings.ambientLight = Color.Lerp(fullLight, fullDark, relativeTime);

            if (GetComponent<Light>() != null)
                GetComponent<Light>().intensity = Mathf.SmoothStep(lightIntensity, 0.0f, relativeTime);

            skybox.SetFloat("_Blend", relativeTime);
        }

        transform.Rotate(Vector3.up * ((Time.deltaTime / dayLength) * 360.0f), Space.Self);
    }

    private void UpdateWorldTime()
    {
        float currentTime = (currentInternalTime / dayLength) * 24;
        currentTime %= 24;

        DaylightScript.currentWorldTime.currentHour = Mathf.FloorToInt(currentTime);
        DaylightScript.currentWorldTime.currentMinute = Mathf.FloorToInt((currentTime - currentWorldTime.currentHour) * 60);

        //lblClock.text = string.Format("{0}:{1}", DaylightScript.currentWorldTime.currentHour.ToString("00"), DaylightScript.currentWorldTime.currentMinute.ToString("00"));
    }

    private void SetDawn()
    {
        Debug.Log("It is now Dawn.");

        skybox.SetFloat("_Blend", 1.0f);
        currentPhase = DayPhase.Dawn;

        if (GetComponent<Light>() != null)
            GetComponent<Light>().enabled = true;
    }

    private void SetDay()
    {
        //Debug.Log("It is now Day.");

        for (int i = 0; i < nightLights.Length; i++)
            nightLights[i].SetActive(false);

        skybox.SetFloat("_Blend", 0.0f);
        currentPhase = DayPhase.Day;
        RenderSettings.ambientLight = fullLight;

        if (GetComponent<Light>() != null)
            GetComponent<Light>().intensity = lightIntensity;
    }

    private void SetDusk()
    {
        Debug.Log("It is now Dusk.");
        skybox.SetFloat("_Blend", 0.0f);
        currentPhase = DayPhase.Dusk;
    }

    private void SetNight()
    {
        Debug.Log("It is now Night.");

        for (int i = 0; i < nightLights.Length; i++)
            nightLights[i].SetActive(true);

        skybox.SetFloat("_Blend", 1.0f);
        currentPhase = DayPhase.Night;
        RenderSettings.ambientLight = fullDark;

        if (GetComponent<Light>() != null)
            GetComponent<Light>().enabled = false;
    }
}
