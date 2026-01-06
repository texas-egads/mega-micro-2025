using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JollyRanchers{
	public class WorldObject{
		public float x, y, w, h;
		public Image img;
		public bool doCollide = true;

		public WorldObject(float x, float y, float w, float h, Image img){
			this.x = x;
			this.y = y;
			this.w = w;
			this.h = h;

			this.img = img;
			this.img.rectTransform.sizeDelta = new Vector2(this.w, this.h);
		}

		public WorldObject(float x, float y, float w, float h){
			this.x = x;
			this.y = y;
			this.w = w;
			this.h = h;
		}

		public void setX(float x){
			this.x = x;
			return;
		}

		public void setY(float y){
			this.y = y;
			return;
		}

		public void setWidth(float w){
			this.w = w;
			return;
		}

		public void setHeight(float h){
			this.h = h;
			return;
		}

		public float getX(){
			return this.x;
		}

		public float getY(){
			return this.y;
		}

		public float getWidth(){
			return this.w;
		}

		public float getHeight(){
			return this.h;
		}

		public bool collide(float tx, float ty, WorldObject obj){
			if(((tx + this.w) >= obj.x) && (tx <= (obj.x + obj.w)) && ((ty + this.h) >= obj.y) && (ty <= (obj.y + obj.h))){
				return true;
			}

			return false;
		}

		public bool collide(WorldObject obj){
			if(((this.x + this.w) >= obj.x) && (this.x <= (obj.x + obj.w)) && ((this.y + this.h) >= obj.y) && (this.y <= (obj.y + obj.h))){
				return true;
			}

			return false;
		}

		public void move(){
			this.img.transform.position = new Vector3(this.x,this.y,0);
			return;
		}
	}
}
