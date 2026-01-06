namespace JollyRanchers{
	public class World{
		public WorldObject Objects;
		public float Max_X, Max_Y;
		public float Min_X, Min_Y;

		public World(float MxX, float MxY, float MnX, float MnY){
			this.Max_X = MxX;
			this.Max_Y = MxY;
			this.Min_X = MnX;
			this.Min_Y = MnY;
		}

		public bool collideWorldBorder_X(float tx){
			if(tx > this.Max_X){ return true; }
			if(tx < this.Min_X){ return true; }

			return false;
		}

		public bool collideWorldBorder_Y(float ty){
			if(ty > this.Max_Y){ return true; }
			if(ty < this.Min_Y){ return true; }

			return false;
		}
	}
}
