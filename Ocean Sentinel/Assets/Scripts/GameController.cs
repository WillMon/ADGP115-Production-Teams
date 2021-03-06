﻿////////////////////////////////////////////////////////////////
//Austin Morrell//
//May 31 2016//
//ADGP-115 Production Teams//
///////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    // This is a list of empty gameObjects that have a transform. Used for spawn positions.
    [SerializeField]
    private GameObject[] spawnPoints;

    // This is the Enemy Prefab.
    [SerializeField]
    private GameObject EnemyPre;

    // Use for the Leaderboard Serializing.
    [SerializeField]
    private GameObject Leader;

    // Use for playing audio.
    [SerializeField]
    private AudioClip[] listOfAudio;

    private AudioSource SFX;

    // This is the number of enemies that will be spawned.
    private float numbEnemies;
    // This is the amount of time between each enemy being spawned.
    private float spawnWait;
    // This is the amount of Gold the Player has.
    public float Gold;
    // This is the wave the Player is on.
    private float numbWaves;

    // Show I spawn the next wave?
    public bool spawnWave;
    // Checks to do something.
    public bool Doit;
    // Checks if we are ready to start checking.
    private bool Ready;

    // The UI text for the wave.
    [SerializeField]
    private Text waveText;
    // The UI text for the gold.
    [SerializeField]
    private Text goldText;
    // The UI text for Base HP.
    [SerializeField]
    private Text baseHPText;
    // The UI text for Base Armor.
    [SerializeField]
    private Text baseArmorText;
    // The UI text for the amount of current enemies.
    [SerializeField]
    private Text numbEnemiesText;

    // These are the power-up buttons.
    [SerializeField]
    private GameObject UIButton1;
    [SerializeField]
    private GameObject UIButton2;
    [SerializeField]
    private GameObject UIButton3;
    [SerializeField]
    private GameObject UIButton4;
    [SerializeField]
    private GameObject SkipButton;
    [SerializeField]
    private GameObject upgrades;
    [SerializeField]
    private GameObject ControllerSelect;
    [SerializeField]
    private GameObject TheBase;

    // This is used for initialization.
    void Start ()
    {
        spawnWait = 1;
        numbEnemies = 5;
        numbWaves = 0;
        Gold = 0;

        spawnWave = true;
        Doit = true;
        Ready = false;

        TurnUIOff();

        SFX = GetComponent<AudioSource>();

        StartCoroutine(SpawnWaves());
	}
	
	void Update ()
    {
        waveText.text = "Wave: " + numbWaves;
        goldText.text = "Gold: " + Gold;
        baseHPText.text = "HP: " + TheBase.GetComponent<Base>().HP;
        baseArmorText.text = "Armor: " + TheBase.GetComponent<Base>().Armor;
        numbEnemiesText.text = "Enemies Remaining: " + GameObject.FindGameObjectsWithTag("Enemy").Length;



        if (GameObject.FindGameObjectsWithTag("Enemy").Length <= 0 && Doit && Ready)
        {
            TurnUIOn();
            GameObject.Find("upgrades").GetComponent<Upgrades>().Active = true;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MoveVelocity = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().newVelocity;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().projectileRate = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().newRate;
            DestroyAll(GameObject.FindGameObjectsWithTag("Projectile"));
            Doit = false;
            Ready = false;
            if (numbWaves == 6)
            {
                TurnUIOff();
                StartCoroutine(Wait(2));
            }
        }
    }

    // This couroutine will spawn the enemies in the game.
    public IEnumerator SpawnWaves()
    {
        // Spawn the wave.
            while (spawnWave)
            {
                int g = 0;
                TurnUIOff();
            // Increase the wave count.
            numbWaves++;

            for (int i = 0; i < numbEnemies; i++)
                {
                    Vector3 spawnPosition = spawnPoints[g].transform.position;
                    Quaternion spawnRotation = Quaternion.identity;
                    Instantiate(EnemyPre, spawnPosition, spawnRotation);
                    g++;
                    if (g >= spawnPoints.Length)
                    {
                         g = 0;
                    }
                    yield return new WaitForSeconds(spawnWait);
                }
                // We are now ready to start checking if the enemies are dead.
                Ready = true;
                // Increase the number of enemies next wave.
                numbEnemies *= 1.5f;
                Mathf.Round(numbEnemies);
                // Pause the couroutine.
                spawnWave = false;
            }
    }

    private void TurnUIOff()
    {
        UIButton1.SetActive(false);
        UIButton2.SetActive(false);
        UIButton3.SetActive(false);
        UIButton4.SetActive(false);
        SkipButton.SetActive(false);
        ControllerSelect.SetActive(false);
        upgrades.SetActive(false);
    }

    private void TurnUIOn()
    {
        UIButton1.SetActive(true);
        UIButton2.SetActive(true);
        UIButton3.SetActive(true);
        UIButton4.SetActive(true);
        SkipButton.SetActive(true);
        ControllerSelect.SetActive(true);
        upgrades.SetActive(true);
    }

    public void GameOver()
    {
        Leader.GetComponent<Serializer>().Scores.Add(numbWaves);
        Leader.GetComponent<Serializer>().Scores.Sort(MySorter);
        Leader.GetComponent<Serializer>().Save();
        SceneManager.LoadScene("GameOver");
    }

    public void YouWin()
    {
        SceneManager.LoadScene("Win");
    }

    public int MySorter(float a, float b)
    {
        if (a > b)
            return -1;
        else if(a < b)
            return 1;

        return 0;
    }

    void DestroyAll(GameObject[] a)
    {
        foreach(GameObject b in a)
        {
            Destroy(b);
        }   
    }

    public void PlaySound(int a, float audioLevels, float audioPitch)
    {
		SFX.pitch = audioPitch;
        SFX.PlayOneShot(listOfAudio[a], audioLevels);
    }

    private IEnumerator Wait(float a)
    {
        yield return new WaitForSeconds(a);
        YouWin();
    }
}
