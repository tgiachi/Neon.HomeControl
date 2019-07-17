namespace Neon.HomeControl.Components.AirCo.Model.Response
{
	public class LoginResponse
	{
		public string UToken { get; set; }
		public int Result { get; set; }
		public int Language { get; set; }
		public int ShowFlg { get; set; }
		public ModelAvl ModeAvlList { get; set; }
	}
}