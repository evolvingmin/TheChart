using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SSSceneManager : MonoBehaviour {


    private static SSSceneManager s_Instance = null;


    // Use this for initialization 

    void Start() {
        DontDestroyOnLoad(this);
        
    }


    // Update is called once per frame 

    void Update() {

    }



    public static SSSceneManager Instance {
        get {
            if ( s_Instance == null ) {
                s_Instance = new GameObject("SSSceneManager").AddComponent<SSSceneManager>();
                //오브젝트가 생성이 안되있을경우 생성. 
            }
            return s_Instance;
        }
    }


    void OnApplicationQuit() {
        s_Instance = null;
        //게임종료시 삭제. 
    }

    public void ManageWealth(string transaction) {
        //StarShip.GameWealth.Start("Table/GameWealth");
    }



    public void MoveScene(string sceneName) {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
        
}
