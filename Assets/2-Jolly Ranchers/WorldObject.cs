namespace JollyRanchers{
	public class WorldObject{
		public float x, y, w, h;

		public WorldObject(float x, float y, float w, float h){
			this.x = x;
			this.y = y;
			this.w = w;
			this.h = h;
		}

		public void setX(float x){
			this.x = x;
		}

		public void setY(float y){
			this.y = y;
		}

		public void setWidth(float w){
			this.w = w;
		}

		public void setHeight(float h){
			this.h = h;
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
	}
}
