using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GameController : MonoBehaviour
{

    public static GameController Instance;

    public Text scoreText;
    public Text soundText;
    public GameObject gameOverPanel;
    public Slider healthSlider;
    public Slider sensitivitySlider;
    public GameObject monsterPrefab;
    public GameObject bulletprefab;
    public float timeBetweenFire = .001f;
    [Tooltip("The time gap between zombie spawns")]
    public float spawnDelay = 2f;
    public AudioClip[] zombieSounds;
    public AudioClip zombieDeathSound;
    public AudioClip zombieBiteSound;
    public Vector2 xLimits, zLimits;
    [Tooltip("Tick this after pressing play to spawn enemies around origin")]
    public bool isFoundSpawnPoint;
    public bool isFire;

    [HideInInspector]
    public int playerHealth = 100;

    ARPlaneManager arPlaneManager;
    ARSessionOrigin arSessionOrigin;
    ARRaycastManager arRaycastManager; //for arfoundation v2.0 raycasting.

    GameObject player;
    Pose hitPose;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    
    float time;
    int score;

    private void Awake()
    {
        if (!Instance)
            Instance = this;


        if (!PlayerPrefs.HasKey("highscore"))
            PlayerPrefs.SetInt("highscore", 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        arPlaneManager = FindObjectOfType<ARPlaneManager>();
        arSessionOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        isFire = false;
        isFoundSpawnPoint = false;
        time = 0;
        playerHealth = 100;
        healthSlider.value = 100;
        score = 0;
        hitPose.position = Vector3.zero;
        hitPose.rotation = Quaternion.identity;
        StartCoroutine(SpawnWaves());
    }

    // Spawn waves of enemies at random posiions at equal intervals.
    public IEnumerator SpawnWaves()
    {
        yield return new WaitUntil(() => isFoundSpawnPoint);
        while (isFoundSpawnPoint)
        {
            GameObject monster = Instantiate(monsterPrefab,
                hitPose.position + new Vector3(Random.Range(xLimits.x, xLimits.y), 0, Random.Range(zLimits.x, zLimits.y)),
                hitPose.rotation);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // function assigned to fire button.
    public void OnFireButtonClicked()
    {
        isFire = true;
    }

    public void TakeDamage(int damage)
    {
        Handheld.Vibrate();
        playerHealth -= damage;
        healthSlider.value = playerHealth;

        if (playerHealth < 0)
        {
            EndGame();
        }
    }

    public void AddScore(int bonus)
    {
        score += bonus;
        scoreText.text = "Score : " + score.ToString();
    }

    public void EndGame()
    {
        int highScore = PlayerPrefs.GetInt("highscore");     
        if (score > highScore)
        {
            gameOverPanel.transform.GetChild(0).GetChild(5).GetChild(0).GetChild(1).GetComponent<Text>().text = "your score : " + score + " !!!";
            gameOverPanel.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
            PlayerPrefs.SetInt("highscore", score);
        }
        else
        {
            gameOverPanel.transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
            gameOverPanel.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = "Your Score : " + score;
            gameOverPanel.transform.GetChild(0).GetChild(4).GetComponent<Text>().text = "Top Score : " + highScore;
        }
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void TogglePause()
    {
        Time.timeScale = (Time.timeScale == 1) ? 0 : 1;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToggleVisibility(GameObject go)
    {
        go.SetActive(go.activeInHierarchy ? false : true);
    }

    public void SetSliderValue()
    {
        sensitivitySlider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = "Sensitivity : " + sensitivitySlider.value.ToString();
    }


    public bool isTouchOnUI(Touch touch)
    {
        PointerEventData eventdata = new PointerEventData(EventSystem.current);
        eventdata.position = touch.position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventdata, results);
        return (results.Count > 0) ? true : false;
    }

    public int SetDamageProportionalToSound()
    {
        float currentSoundMagnitude = MicrophoneController.Instance.maxAmplitude;

        if (currentSoundMagnitude > MicrophoneController.Instance.minimumThreshold + 100)
            return 100;
        else if (currentSoundMagnitude > MicrophoneController.Instance.minimumThreshold + 70)
            return 60;
        else if (currentSoundMagnitude > MicrophoneController.Instance.minimumThreshold + 30)
            return 40;
        else if (currentSoundMagnitude > MicrophoneController.Instance.minimumThreshold)
            return 10;
        else return 0;
    }

    // Update is called once per frame
    void Update()
    {

        // Bullet fire rate controlling part.
        if (isFire && time > timeBetweenFire)
        {
            GameObject bullet = Instantiate(bulletprefab, player.transform.GetChild(0).transform.position, player.transform.GetChild(0).transform.rotation);
            bullet.GetComponent<BulletController>().damage = SetDamageProportionalToSound();
            isFire = false;
            time = 0;
        }
        else
            time += Time.deltaTime;


        // AR raycasting and searching for a plane to set spawn point.
        if (!isFoundSpawnPoint)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (arRaycastManager.Raycast(touch.position, hits,UnityEngine.XR.ARSubsystems.TrackableType.Planes))
                {
                    hitPose = hits[0].pose;
                    isFoundSpawnPoint = true;
                    arPlaneManager.enabled = false;
                }
            }
        }

       
    }


}
