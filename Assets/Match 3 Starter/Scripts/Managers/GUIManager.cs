using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour {
	public static GUIManager instance;

	public GameObject gameOverPanel;
	public Text yourScoreTxt;
	public Text highScoreTxt;

	public Text scoreTxt;
	public Text moveCounterTxt;
	

	public int moveCounter;


	private int score;
	
	public Text SpriteTxt;

	[SerializeField] private string nameScoreSprite;
	private int scoreSprite;


	private void Update()
	{
		
	}
	
	public int Score
	{
		get
		{
			return score;
		}

		set
		{
			score = value;
			scoreTxt.text = score.ToString();
		}
	}

	public int askdalsk
	{
		get
		{
			return scoreSprite;
		}
		set
		{
			score = value;
			if (Tile.kringe == "BubleGum")
			{
				scoreSprite++;
			}

			SpriteTxt.text = scoreSprite.ToString();
		}
	}
	public int MoveCounter
	{
		get
		{
			return moveCounter;
		}

		set
		{
			
			moveCounter = value;
			if (moveCounter <= 0)
			{
				moveCounter = 0;
				StartCoroutine(WaitForShifting());

			}

			moveCounterTxt.text = moveCounter.ToString();
		}
	}

	void Awake() {
		//moveCounter = 60;
		moveCounterTxt.text = moveCounter.ToString();

		instance = GetComponent<GUIManager>();
	}

	// Show the game over panel
	public void GameOver() {
		GameManager.instance.gameOver = true;

		gameOverPanel.SetActive(true);

		if (score > PlayerPrefs.GetInt("HighScore")) {
			PlayerPrefs.SetInt("HighScore", score);
			highScoreTxt.text = "New Best: " + PlayerPrefs.GetInt("HighScore").ToString();
		} else {
			highScoreTxt.text = "Best: " + PlayerPrefs.GetInt("HighScore").ToString();
		}

		yourScoreTxt.text = score.ToString();
	}
	private IEnumerator WaitForShifting()
	{
		yield return new WaitUntil(() => !BoardManager.instance.IsShifting);
		yield return new WaitForSeconds(.25f);
		GameOver();
	}


}
