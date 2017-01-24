using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TestSimpleRNG;

namespace GJJ2017StockWave {
	
	public class StockWaveGame : MonoBehaviour {

		public int myInitialCapital = 100;
		public int myInitialStockNumber = 0;
		public int initialStockPrice = 10;
		public float standardDeviation = 5f;
		public float stockPriceUpdateTimeUnit = 0.5f; // seconds
		public float capitalReduceTimeUnit = 1f;
		public int capitalReduceAmount = 1;
		//public float longTermMeanTimeInterval = 10f;
		//public float shortTermSDPercentage = 0.3f;

		public int emotionWaveThreshold = 10;
		public int sadBelow = 3;
		public float emotionTickInterval = 3f;

		public Text myCapitalText;
		public Text myStockNumberText;
		public Text myStockValueText;
		public Text stockPriceText;
		public TipsOnInteraction tipsOnInteraction;

		public GameObject happyEmotion;
		public GameObject sadEmotion;

		public AudioSource buyAudio;
		public AudioSource sellAudio;

		AudioSource happyAudio;
		AudioSource sadAudio;

		int myCapital;
		int myStockNumber;
		int lastStockPriceForEmotion;
		float lastEmotionTime;

		public static StockWaveGame instance;

		int curStockPrice;
		int stockPriceDelta = 0;

		float stockPriceUpdateTimeDeltaAccumulated = 0f;
		float capitalReduceTimeDeltaAccumulated = 0f;
		//float longTimeMeanTimeDeltaAccumulated = 0;

		float lastedTime = 0f;
		int highestCapital;

		bool isGameOver = false;

		// Use this for initialization
		void Awake () {
			instance = this;
			lastStockPriceForEmotion = curStockPrice = initialStockPrice;
			//SimpleRNG.SetSeedFromSystemTime ();
			SimpleRNG.SetSeed (1952963668, 3696519570);
			Debug.Log ("Seed1: " + SimpleRNG.GetSeed1 () + ", Seed2: " + SimpleRNG.GetSeed2 ());
		}

		void Start () {
			happyAudio = happyEmotion.GetComponent<AudioSource> ();
			sadAudio = sadEmotion.GetComponent<AudioSource> ();
			Reset ();
		}

		public int GetCurrentStockPrice () {
			return curStockPrice;
		}

		int GenerateNextStockPrice () {
			int nextStockPrice = curStockPrice;

			if (StockEventSystem.instance.IsRegularEventTrigger ()) {
				if (!StockEventSystem.instance.IsEventStartMeanSet ()) {
					StockEventSystem.instance.SetEventStartMean (curStockPrice);
				}
				nextStockPrice = StockEventSystem.instance.GenerateNextStockPrice();
			} else {
				/*
				if (longTimeMeanTimeDeltaAccumulated >= longTermMeanTimeInterval) {
					nextStockPrice = (int)SimpleRNG.GetNormal (initialStockPrice, standardDeviation);
					longTimeMeanTimeDeltaAccumulated = 0f;
				} else {
				*/
				nextStockPrice = (int)SimpleRNG.GetNormal (curStockPrice, standardDeviation);
				Debug.Log ("mean = prevStockPrice = " + curStockPrice);
			}

			if (nextStockPrice < 0) {
				nextStockPrice *= -1;
			} else if (nextStockPrice == 0) {
				nextStockPrice = 1;
			}

			return nextStockPrice;
		}

		public int GetStockPriceDelta () {
			return stockPriceDelta;
		}

		void Reset () {
			isGameOver = false;

			if (Wave.instance) {
				Wave.instance.Reset ();
			}
			if (StockEventSystem.instance) {
				StockEventSystem.instance.Reset ();
			}

			tipsOnInteraction.gameObject.SetActive (false);
			SetMyCapital (myInitialCapital);
			SetMyStockNumber (myInitialStockNumber);
			SetStockPrice (initialStockPrice);

			stockPriceUpdateTimeDeltaAccumulated = 0f;
			capitalReduceTimeDeltaAccumulated = 0f;
			//longTimeMeanTimeDeltaAccumulated = 0;

			lastedTime = 0f;
			highestCapital = myInitialCapital;

			Happy ();
		}

		public bool IsGameOver () {
			return isGameOver;
		}

		void GameOver () {
			isGameOver = true;
			tipsOnInteraction.ShowForEver (
				"没钱了！\n" + 
				"你坚持了" + (int)lastedTime + "秒\n" +
				"曾拥有过" + highestCapital + "元\n" +
				"点击重来"
			);
			Sad ();
		}

		void Update () {
			if (isGameOver) {
				if (Input.GetMouseButtonDown (0) || Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
					Reset ();
				}
				return;
			}

			if (Input.GetKeyDown (KeyCode.S)) {
				Buy ();
			}

			if (Input.GetKeyDown (KeyCode.K)) {
				Sell ();
			}

			lastedTime += Time.deltaTime;

			stockPriceUpdateTimeDeltaAccumulated += Time.deltaTime;
			//longTimeMeanTimeDeltaAccumulated += Time.deltaTime;
			if (stockPriceUpdateTimeDeltaAccumulated >= stockPriceUpdateTimeUnit) {
				int nextStockPrice = GenerateNextStockPrice ();
				stockPriceDelta = nextStockPrice - curStockPrice;
				SetStockPrice (nextStockPrice);

				Wave.instance.UpdateWave ();

				stockPriceUpdateTimeDeltaAccumulated = 0f;
			}

			capitalReduceTimeDeltaAccumulated += Time.deltaTime;
			if (capitalReduceTimeDeltaAccumulated >= capitalReduceTimeUnit) {
				if (myCapital <= capitalReduceAmount) {
					SetMyCapital (0);
				} else {
					SetMyCapital (myCapital - capitalReduceAmount);
				}
				capitalReduceTimeDeltaAccumulated = 0f;
			}

			// emotion
			if (Time.time - lastEmotionTime >= emotionTickInterval) {
				if (curStockPrice < sadBelow) {
					Sad ();
				} else if (curStockPrice - lastStockPriceForEmotion >= emotionWaveThreshold) {
					Happy ();
				} else if (curStockPrice - lastStockPriceForEmotion <= -emotionWaveThreshold) {
					Sad ();
				}
				lastStockPriceForEmotion = curStockPrice;
			}
		}

		void Happy () {
			if (!happyEmotion.activeSelf) {
				happyEmotion.SetActive (true);
				sadEmotion.SetActive (false);
				happyAudio.Play ();
				lastEmotionTime = Time.time;
			}
		}

		void Sad () {
			if (!sadEmotion.activeSelf) {
				happyEmotion.SetActive (false);
				sadEmotion.SetActive (true);
				sadAudio.Play ();
				lastEmotionTime = Time.time;
			}
		}

		public void Buy () {
			if (myCapital < curStockPrice) {
				tipsOnInteraction.Show ("现金不足");
				return;
			}
			SetMyCapital(myCapital - curStockPrice);
			SetMyStockNumber(myStockNumber + 1);
			buyAudio.Play ();
		}

		public void Sell () {
			if (myStockNumber <= 0) {
				tipsOnInteraction.Show ("持仓份额不足");
				return;
			}
			SetMyStockNumber(myStockNumber - 1);
			SetMyCapital(myCapital + curStockPrice);
			sellAudio.Play ();
		}

		void SetMyCapital (int value) {
			myCapital = value;
			myCapitalText.text = myCapital.ToString();

			if (value > highestCapital) {
				highestCapital = value;
			}

			if (value <= 0) {
				GameOver ();
			}
		}

		void SetMyStockNumber(int value) {
			myStockNumber = value;
			myStockNumberText.text = myStockNumber.ToString();
			myStockValueText.text = (myStockNumber * curStockPrice).ToString ();
		}

		void SetStockPrice (int value) {
			curStockPrice = value;
			stockPriceText.text = curStockPrice.ToString ();
			myStockValueText.text = (myStockNumber * curStockPrice).ToString();
		}
	}
}