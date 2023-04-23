using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static bool levelStarted, levelFailed, levelPassed, playerMoved, playerFired, loadLevelTime, cheatOn;
    public static int tanksAlive;

    [SerializeField]
    Transform levelObjectsFolder;

    [SerializeField]
    Camera myCam;

    [SerializeField] Transform spawnFolder;

    [SerializeField] SpriteRenderer maintTitle, mainTitleBG, whiteSquare, levelPassedSR, levelPassedBG;
    [SerializeField] SpriteRenderer[] moveTut, fireTut, nextLevelButton;

    [SerializeField] Transform[] retryTexts, winTexts;

    bool restartLogic, startLogic, passedLogic, moveLogic, fireLogic, loadingLevel;

    [SerializeField] TextMeshPro currentLevel;

    [SerializeField] GameObject confettiObj, nextLevelObj;

    // level spawner literature
     Vector3 spawnPosition;
    int objToSpawnInt;

    [SerializeField] SpriteRenderer[] soundIcons;

    [SerializeField] SoundManagerLogic mySoundManager;
    public static SoundManagerLogic staticSoundManager;

    [SerializeField] GameObject harHadtObj;

    // Sounds: BulletLogic.cs, PlayerController.cs, ControlsLogic.cs, EnemyLogicPro.cs, GameManager.cs
    public static int bulletSound;

    [SerializeField] Material wallMat, groundMat, bgMat;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        staticSoundManager = mySoundManager;        

        levelStarted = false;
        levelFailed = false;
        levelPassed = false;

        restartLogic = false;
        startLogic = false;
        passedLogic = false;

        loadLevelTime = false;
        loadingLevel = false;

        fireLogic = false;
        moveLogic = false;
        playerFired = false;
        playerMoved = false;
        tanksAlive = 0;
        bulletSound = 0;

        cheatOn = false;

        if (PlayerPrefs.GetInt("eggierrEnabled", 0) == 0) // is off
        {
            harHadtObj.SetActive(false);
            cheatOn = false;
        }
        else if (PlayerPrefs.GetInt("eggierrEnabled", 0) == 1) // is on
        {
            harHadtObj.SetActive(true);
            cheatOn = true;
        }

        confettiObj.SetActive(false);

        currentLevel.text = "stage " + PlayerPrefs.GetInt("levelCount", 1);
        PlayerPrefs.SetInt("levelCount", 12);

        // change mat colors
        int currentLevelInt = PlayerPrefs.GetInt("levelCount", 1);
        if (currentLevelInt % 5 == 0 && currentLevelInt > 1) // dark mats
        {
            Color wallColor = new Color(0.09199997f, 0.01533334f, 0, 1),
                groundColor = new Color(0.149f, 0.06796052f, 0.003128995f, 1),
                bgColor = new Color(0.008814003f, 0.02832801f, 0.07799999f, 1);

            switch (PlayerPrefs.GetInt("MatInt", 0))
            {
                case 0: // brown w/ blue -
                    wallColor = new Color(0.09199997f, 0.01533334f, 0, 1);
                    groundColor = new Color(0.149f, 0.06796052f, 0.003128995f, 1);
                    bgColor = new Color(0.008814003f, 0.02832801f, 0.07799999f, 1);
                    break;
                case 1: // green w/ red -
                    wallColor = new Color(0, 0.08049998f, 0.09199997f, 1);
                    groundColor = new Color(0, 0.135f, 0.06074999f, 1);
                    bgColor = new Color(0.128f, 0.01651614f, 0f, 1);
                    break;
                case 2: // red w/ yellow -
                    wallColor = new Color(0.00969696f, 0, 0.07999998f, 1);
                    groundColor = new Color(0.137f, 0, 0.0747273f, 1);
                    bgColor = new Color(0.05322581f, 0.11f, 0, 1);
                    break;
                case 3: // purple w/ blue -
                    wallColor = new Color(0.016125f, 0, 0.1289999f, 1);
                    groundColor = new Color(0.07004077f, 0, 0.156f, 1);
                    bgColor = new Color(0, 0.12f, 0.07161287f, 1);
                    break;
                case 4: // green w/ purple -
                    wallColor = new Color(0.09999997f, 0, 0.08749997f, 1);
                    groundColor = new Color(0.06722221f, 0.121f, 0, 1);
                    bgColor = new Color(0.06612498f, 0, 0.09199997f, 1);
                    break;
                case 5: // red w/ red -
                    wallMat.color = new Color(0.107f, 0.01380645f, 0.107f, 1);
                    groundMat.color = new Color(0.2079999f, 0.04451197f, 0.04451197f, 1);
                    bgMat.color = new Color(0.021675f, 0.04067247f, 0.08499997f, 1);
                    break;
                default:
                    wallColor = new Color(0.09199997f, 0.01533334f, 0, 1);
                    groundColor = new Color(0.149f, 0.06796052f, 0.003128995f, 1);
                    bgColor = new Color(0.008814003f, 0.02832801f, 0.07799999f, 1);
                    break;
            }
            wallMat.color = wallColor;
            groundMat.color = groundColor;
            bgMat.color = bgColor;


            if (PlayerPrefs.GetInt("ChangeMatColor", 0) == 0)
                PlayerPrefs.SetInt("ChangeMatColor", 1);

        }
        else if (currentLevelInt % 5 == 1 && currentLevelInt > 2 && PlayerPrefs.GetInt("ChangeMatColor", 0) == 1) // new mat color
        {
            PlayerPrefs.SetInt("ChangeMatColor", 0);

            PlayerPrefs.SetInt("MatInt", (PlayerPrefs.GetInt("MatInt", 0) + 1)%6);

            Color wallColor = new Color(0.1226415f, 0.0670036f, 0.05958521f, 1),
                groundColor = new Color(0.206f, 0.1506058f, 0.103f, 1),
                bgColor = new Color(0.2907303f, 0.4213482f, 0.75f, 1);

            switch (PlayerPrefs.GetInt("MatInt", 0))
            {
                case 0: // brown w/ blue
                    wallColor = new Color(0.1226415f, 0.0670036f, 0.05958521f, 1);
                    groundColor = new Color(0.206f, 0.1506058f, 0.103f, 1);
                    bgColor = new Color(0.2907303f, 0.4213482f, 0.75f, 1);
                    break;
                case 1: // green w/ red
                    wallColor = new Color(0.0588235f, 0.1132026f, 0.1215686f, 1);
                    groundColor = new Color(0.1019608f, 0.2078431f, 0.1496078f, 1);
                    bgColor = new Color(0.482f, 0.2714624f, 0.239072f, 1);
                    break;
                case 2: // red w/ yellow
                    wallColor = new Color(0.06718951f, 0.0588235f, 0.1215686f, 1);
                    groundColor = new Color(0.2078431f, 0.1019608f, 0.160196f, 1);
                    bgColor = new Color(0.6803925f, 0.872f, 0.5127359f, 1);
                    break;
                case 3: // purple w/ blue
                    wallColor = new Color(0.06718951f, 0.0588235f, 0.1215686f, 1);
                    groundColor = new Color(0.1496078f, 0.1019608f, 0.2078431f, 1);
                    bgColor = new Color(0.208372f, 0.452f, 0.3545488f, 1);
                    break;
                case 4: // green w/ purple
                    wallColor = new Color(0.1215686f, 0.0588235f, 0.1132026f, 1);
                    groundColor = new Color(0.160196f, 0.2078431f, 0.1019608f, 1);
                    bgColor = new Color(0.7000379f, 0.432016f, 0.806f, 1);
                    break;
                case 5: // red w/ red
                    wallColor = new Color(0.122f, 0.05465598f, 0.122f, 1);
                    groundColor = new Color(0.2078431f, 0.1019608f, 0.1019608f, 1);
                    bgColor = new Color(0.433f, 0.224294f, 0.224294f, 1);
                    break;
                default:
                    wallColor = new Color(0.1226415f, 0.0670036f, 0.05958521f, 1);
                    groundColor = new Color(0.206f, 0.1506058f, 0.103f, 1);
                    bgColor = new Color(0.2907303f, 0.4213482f, 0.75f, 1);
                    break;
            }
            wallMat.color = wallColor;
            groundMat.color = groundColor;
            bgMat.color = bgColor;
        }
    }

    private void Start()
    {
        StartCoroutine(StartLogic());

        for (int i = 0; i < moveTut.Length; i++)
        {
            StartCoroutine(FadeImageIn(moveTut[i], 6));
        }
        for (int i = 0; i<fireTut.Length; i++)
        {
            StartCoroutine(FadeImageIn(fireTut[i], 6));
        }
    }
    IEnumerator FadeOutAudio(AudioSource myAudio)
    {
        float timer = 0, totalTime = 24;
        float startingLevel = myAudio.volume;
        while (timer <= totalTime)
        {
            myAudio.volume = Mathf.Lerp(startingLevel, 0, timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    private void Update()
    {
        if (cheatOn && PlayerPrefs.GetInt("eggierrEnabled", 0) == 0) // turn on
        {
            harHadtObj.SetActive(true);
            PlayerPrefs.SetInt("eggierrEnabled", 1);
        }
        else if (!cheatOn && PlayerPrefs.GetInt("eggierrEnabled", 0) == 1) // turn off
        {
            harHadtObj.SetActive(false);
            PlayerPrefs.SetInt("eggierrEnabled", 0);
        }

        // level won
        if (tanksAlive == 0 && !levelFailed && !levelPassed && false)
        {
            GameManager.staticSoundManager.Play("WinJingle"); // win jingle sound

            levelPassed = true;
        }

        // level failed
        if (!restartLogic && levelFailed && !passedLogic)
        {
            GameManager.staticSoundManager.Play("LoseJingle"); // win jingle sound

            Transform tempObj = retryTexts[Random.Range(0, retryTexts.Length)].transform;
            SpriteRenderer retryTitle, retryBg;
            retryTitle = tempObj.GetComponent<SpriteRenderer>();
            retryBg = tempObj.GetComponentsInChildren<SpriteRenderer>()[1];

            Camera_Tracker.stopCameraTracking = true;

            StartCoroutine(RetryLiterature(retryTitle, retryBg));
            StartCoroutine(RestartWait());            

            restartLogic = true;
        }

        if (loadLevelTime && !loadingLevel)
        {
            StartCoroutine(RestartWait());
            MoveButton();
            loadingLevel = true;
        }

        // level started
        if (!startLogic && levelStarted)
        {
            GameManager.staticSoundManager.Play("StartJingle"); // level start jingle sound
            //GameManager.staticSoundManager.Play("Noise"); // bg music sound
            
            foreach (SpriteRenderer sprite in soundIcons)
            {
                StartCoroutine(FadeImageOut(sprite));
            }

            StartCoroutine(FadeImageOut(maintTitle));
            StartCoroutine(FadeImageOut(mainTitleBG));
            startLogic = true;
        }
        
        // level passed
        if (!passedLogic && levelPassed)
        {
            confettiObj.SetActive(true);


            PlayerPrefs.SetInt("levelCount", PlayerPrefs.GetInt("levelCount", 1) + 1); // increment

            PlayerPrefs.SetInt("SpawnNewLevel", 1);
            PlayerPrefs.SetInt("SpawnNewTanks", 1);

            int objNum = Random.Range(0, winTexts.Length);
            // winTexts
            Transform tempObj = winTexts[objNum].transform;

            SpriteRenderer winTitle, winBG;
            winTitle = tempObj.GetComponent<SpriteRenderer>();
            winBG = tempObj.GetComponentsInChildren<SpriteRenderer>()[1];

            StartCoroutine(RetryLiterature(winTitle, winBG));
            // next level button
            for (int i = 0; i < nextLevelButton.Length; i++)
            {
                StartCoroutine(FadeImageIn(nextLevelButton[i], 12));
            }

            passedLogic = true;
        }

        if (playerFired && !fireLogic)
        {
            for (int i = 0; i < moveTut.Length; i++)
            {
                StartCoroutine(FadeImageOut(moveTut[i]));
            }
            fireLogic = true;
        }
        if (playerMoved && !moveLogic)
        {
            for (int i = 0; i < fireTut.Length; i++)
            {
                StartCoroutine(FadeImageOut(fireTut[i]));
            }
            moveLogic = true;
        }
    }

    void MoveButton()
    {
        nextLevelObj.transform.position += new Vector3(0, -0.05f, 0);
    }

    IEnumerator StartLogic()
    {
        whiteSquare.enabled = true;
        whiteSquare.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(FadeImageOut(whiteSquare));
    }

    IEnumerator RetryLiterature(SpriteRenderer mainText, SpriteRenderer bgText)
    {
        float timer = 0, totalTime = 40;
        Color startingColor1 = mainText.color;
        Color startingColor2 = bgText.color;
        Transform textTransform = mainText.gameObject.transform.parent.transform;

        Vector3 startingScale = textTransform.localScale;

        while (timer <= totalTime)
        {
            if (timer <= 18)
                textTransform.localScale = Vector3.Lerp(startingScale*0.1f, startingScale * 1.65f, timer / (totalTime-18));

            if (timer < totalTime * 0.75f)
            {
                mainText.color = Color.Lerp(startingColor1, new Color(startingColor1.r, startingColor1.g, startingColor1.b, 1), timer / (totalTime*0.7f));
                bgText.color = Color.Lerp(startingColor2, new Color(startingColor2.r, startingColor2.g, startingColor2.b, 1), timer / (totalTime*0.7f));
            }

            yield return new WaitForFixedUpdate();
            timer++;
        }

        timer = 0;
        totalTime = 80;
        startingScale = textTransform.localScale;
        while (timer <= totalTime)
        {
            textTransform.localScale = Vector3.Lerp(startingScale, new Vector3(startingScale.x*1.15f, startingScale.y*1.5f, startingScale.z*1.5f), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    IEnumerator RestartWait()
    {

        if (levelFailed)
            yield return new WaitForSecondsRealtime(1.7f);
        else
            yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(RestartLevel(whiteSquare));
    }

    IEnumerator RestartLevel(SpriteRenderer myImage)
    {
        float timer = 0, totalTime = 24;
        Color startingColor = myImage.color;
        myImage.enabled = true;
        while (timer <= totalTime)
        {
            myImage.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 0), new Color(startingColor.r, startingColor.g, startingColor.b, 1), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    IEnumerator FadeImageOut(SpriteRenderer myImage)
    {
        float timer = 0, totalTime = 24;
        Color startingColor = myImage.color;
        myImage.enabled = true;
        while (timer <= totalTime)
        {
            myImage.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 1), new Color(startingColor.r, startingColor.g, startingColor.b, 0), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
        myImage.enabled = false;
    }

    IEnumerator FadeImageIn(SpriteRenderer myImage, float totalTime)
    {
        float timer = 0;
        Color startingColor = myImage.color;
        myImage.enabled = true;
        while (timer <= totalTime)
        {
            myImage.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 0), new Color(startingColor.r, startingColor.g, startingColor.b, 1), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    IEnumerator FadeTextOut(TextMeshPro myTtext)
    {
        float timer = 0, totalTime = 24;
        Color startingColor = myTtext.color;
        while (timer <= totalTime)
        {
            myTtext.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 1), new Color(startingColor.r, startingColor.g, startingColor.b, 0), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    IEnumerator FadeTextIn(TextMeshPro myTtext)
    {
        float timer = 0, totalTime = 24;
        Color startingColor = myTtext.color;
        while (timer <= totalTime)
        {
            myTtext.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 0), new Color(startingColor.r, startingColor.g, startingColor.b, 1), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }
}
