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
		private World wrld;
		private WorldObject plr;

		private float minX = 160.0f;
		private float maxX = 815.0f;
		private float dX = 1.0f;

		private float minY = 119.0f;
		private float maxY = 420.0f;
		private float dY = 1.0f;

		private void Start(){
			float difficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty();
			DifficultyText.text = $"Current Difficulty: {difficulty.ToString()}";
			spaceCount = Mathf.CeilToInt(SpacePressesNeeded.Evaluate(difficulty));

			UIText.text = $"Press space {spaceCount} times!";
			UIText.text = $"Press space {spaceCount} times! | {UIText.transform.position}";
			UIText.text = $"{player.transform.position}";
			
			UIText.transform.position = new Vector3(490,334,0);
			
			AudioSource loop = Managers.AudioManager.CreateAudioSource();
			loop.loop = true;
			loop.clip = loopSound;
			loop.Play();

			wrld = new World(maxX,maxY,minX,minY);
			plr = new WorldObject(wrld.Min_X,wrld.Min_Y,50.0f,50.0f);
			player.transform.position = new Vector3(plr.x,plr.y,0);
		}

		private void Update(){
			float x = dX * Input.GetAxis("Horizontal");
			if(wrld.collideWorldBorder_X(plr.x + x)){
				x = 0.0f;
			}
			plr.x = plr.x + x;

			/*
			float y = dY * Input.GetAxis("Vertical");
			if(wrld.collideWorldBorder_Y(plr.y + y)){
				y = 0.0f;
			}
			plr.y = plr.y + y;
			*/

			player.transform.position = new Vector3(plr.x,plr.y,0);
			UIText.text = $"{player.transform.position}";

			if (Input.GetButtonDown("Space")){
				spaceCount--;
				UIText.text = $"Press space {spaceCount} times!";
				UIText.text = $"Press space {spaceCount} times! | {UIText.transform.position}";
			}

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
