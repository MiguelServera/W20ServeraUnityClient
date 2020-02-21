using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public List<GameObject> targets;
    private float spawnRate = 1.0f;
    private int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI playerNameText;
    public GameObject titleScreen;
    public GameObject gameOverScreen;
    public TextMeshProUGUI names;
    public bool isGameActive;

    public Player player;

    public GameObject leftColumn;
    private DateTime birthday;
    private DateTime today;
    private TimeSpan difference;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        DateTime birthday = player.BirthDay;
        DateTime today = DateTime.Now;
        TimeSpan difference = today - birthday;
        playerNameText.text = player.Name + "(" + difference.Days / 365 + ")";
        scoreText.text = score.ToString();
        //intento de hacer que cuando un cliente se conecta salga su nombre. No lo he podido hacer.
        //names.text = names.text + "\n" + player.Name + "(" + difference.Days / 365 + ")";
    }

    public void StartGame(int difficulty)
    {
        isGameActive = true;
        score = 0;
        UpdateScore(0);
        titleScreen.gameObject.SetActive(false);
        spawnRate /= difficulty;
        StartCoroutine(SpawnTarget());
    }

    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);
            int randomIndex = UnityEngine.Random.Range(0, 4);
            Instantiate(targets[randomIndex]);
        }
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        gameOverScreen.gameObject.SetActive(true);
        isGameActive = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
