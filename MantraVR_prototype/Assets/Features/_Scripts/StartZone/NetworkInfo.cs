using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class NetworkInfo : MonoBehaviour
{
    public static float timing;

    void Start()
    {
        SceneManager.activeSceneChanged += Loaded;
        DontDestroyOnLoad(gameObject); 
    }

    void Loaded(Scene sceneOld, Scene scene)
    {       
        if ( scene.name == "VoiceRingTransparent")            
            StartCoroutine(SetTime());
    }


    IEnumerator SetTime()
    {
        yield return new WaitForSeconds(0.5f);
       // GameObject.FindObjectOfType<GearSkipper>().OnTime(timing);
    }
}