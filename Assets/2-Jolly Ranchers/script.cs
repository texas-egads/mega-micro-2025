using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JollyRanchers{
	public class script : MonoBehaviour {
		public TextMeshProUGUI UIText;
		public Canvas parent;
		public string winText;

		public AudioClip loopSound;
		public AudioClip winSound;
		public AudioClip lossSound;
		public AudioClip collectSound;

		public Sprite[] snowSprites;
		public Sprite[] playerSprites;

		private int spaceCount;

		private bool isWin = false;
		private bool isLoss = false;

		private World wrld;
		private WorldObject plr;
		private List<WorldObject> snow;
		private int snowCount;
		private int caught = 0;

		private float minX = 160.0f;
		private float maxX = 815.0f;
		private float dX = 300.0f;

		private float minY = 119.0f;
		private float maxY = 420.0f;
		private float dY = 140.0f;

		private float covered = 0f;

		private void Start(){			
			// Difficulty
			float difficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty();
			snowCount = Mathf.CeilToInt(3.0f*difficulty - 0.5f) + 2;
			// ------------------------------------------------------------------ //

			UIText.text = $"Catch {snowCount} snowballs!";
			
			// Play Looped song
			AudioSource loop = Managers.AudioManager.CreateAudioSource();
			loop.loop = true;
			loop.clip = loopSound;
			loop.Play();
			// --------- //

			// Set up game World
			wrld = new World(maxX,maxY,minX,minY);

			snow = new List<WorldObject>();
			for(int i = 0; i < snowCount; i++){
				GameObject newImageObject = new GameObject("Snow");
				newImageObject.transform.SetParent(parent.transform, false);
				Image newImage = newImageObject.AddComponent<Image>();
				newImage.sprite = snowSprites[0];

				snow.Add(new WorldObject(
					(524.0f * UnityEngine.Random.Range(0.0f,1.0f)) + 225.5f,
					maxY + ((800.0f * i)/snowCount),
					50.0f,50.0f,newImage));
				snow[i].move();
			}

			GameObject newPlrImageObject = new GameObject("Player");
			newPlrImageObject.transform.SetParent(parent.transform, false);
			Image newPlrImage = newPlrImageObject.AddComponent<Image>();
			newPlrImage.sprite = playerSprites[0];
			plr = new WorldObject(minX + ((maxX - minX) / 2.0f),minY,50.0f,50.0f,newPlrImage);
			plr.move();
		}

		private void Update(){
			// move player around
			float x = Time.deltaTime * dX * Input.GetAxis("Horizontal");
			if(wrld.collideWorldBorder_X(plr.x + x)){
				x = 0.0f;
			}
			plr.x = plr.x + x;
			plr.move();

			if(x < 0f){
				if(covered < 10f){
					plr.img.sprite = playerSprites[1];
				} else {
					plr.img.sprite = playerSprites[3];
				}
			}
			if(x > 0f){
				if(covered < 10f){
					plr.img.sprite = playerSprites[0];
				} else {
					plr.img.sprite = playerSprites[2];
				}
			}

			// Move snow around
			foreach(WorldObject obj in snow){
				if(obj.doCollide){
					float y = Time.deltaTime * (-dY);
					obj.y = obj.y + y;
					obj.move();

					if(obj.collide(plr)){
						AudioSource collect = Managers.AudioManager.CreateAudioSource();
						collect.PlayOneShot(collectSound);
						obj.img.enabled = false;
						obj.doCollide = false;
						caught++;
						covered = 20;
						if(plr.img.sprite == playerSprites[0]){
							plr.img.sprite = playerSprites[2];
						}
						if(plr.img.sprite == playerSprites[1]){
							plr.img.sprite = playerSprites[3];
						}
					}

					if(obj.y <= minY - obj.h){
						isLoss = true;
					}
				}
			}

			if(covered > 10f){
				covered = covered - (10.0f * Time.deltaTime);
			}
			// --------------------------------------------------- //

			UIText.text = $"Catch {snowCount} snow!";

			if(caught >= snowCount){
				isWin = true;
			}

			if(isWin){
				UIText.text = winText;
				AudioSource win = Managers.AudioManager.CreateAudioSource();
				win.PlayOneShot(winSound);

				Managers.MinigamesManager.DeclareCurrentMinigameWon();
				Managers.MinigamesManager.EndCurrentMinigame(1f);
				this.enabled = false;
			}

			if(isLoss){
				UIText.text = "Snow dropped!";
				AudioSource loss = Managers.AudioManager.CreateAudioSource();
				loss.PlayOneShot(lossSound);

				Managers.MinigamesManager.DeclareCurrentMinigameLost();
				Managers.MinigamesManager.EndCurrentMinigame(1f);
				this.enabled = false;
			}
		}
	}
}
