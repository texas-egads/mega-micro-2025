using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JollyRanchers{
	public class script : MonoBehaviour {
		public TextMeshProUGUI DifficultyText;
		public TextMeshProUGUI UIText;
		public AnimationCurve SpacePressesNeeded;
		public Image player;
		public string winText;

		public AudioClip loopSound;
		public AudioClip winSound;

		private int spaceCount;

		private bool isWin = false;

		private void Start(){
			float difficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty();
			DifficultyText.text = $"Current Difficulty: {difficulty.ToString()}";
			spaceCount = Mathf.CeilToInt(SpacePressesNeeded.Evaluate(difficulty));

			UIText.text = $"Press space {spaceCount} times!";
			UIText.text = $"Press space {spaceCount} times! | {UIText.transform.position}";
			
			UIText.transform.position = new Vector3(490,334,0);
			
			AudioSource loop = Managers.AudioManager.CreateAudioSource();
			loop.loop = true;
			loop.clip = loopSound;
			loop.Play();

			float x = UnityEngine.Random.Range(0.0f, 800.0f);
			float y = UnityEngine.Random.Range(0.0f, 800.0f);
			player.transform.position = new Vector3(x,y,0);
		}

		private void Update(){
			if (Input.GetButtonDown("Space")){
				spaceCount--;
				UIText.text = $"Press space {spaceCount} times!";
				UIText.text = $"Press space {spaceCount} times! | {UIText.transform.position}";
			}

			float x = UnityEngine.Random.Range(0.0f, 800.0f);
			float y = UnityEngine.Random.Range(0.0f, 800.0f);
			player.transform.position = new Vector3(x,y,0);

			if(spaceCount == 0){
				UIText.text = winText;

				AudioSource win = Managers.AudioManager.CreateAudioSource();
				win.PlayOneShot(winSound);

				Managers.MinigamesManager.DeclareCurrentMinigameWon();
				Managers.MinigamesManager.EndCurrentMinigame(1f);
				this.enabled = false;
			}
		}
	}
}
