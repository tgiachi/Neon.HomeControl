using Neon.HomeControl.Api.Core.Attributes.EventDatabase;
using Neon.HomeControl.Api.Core.Impl.EventsDatabase;

namespace Neon.HomeControl.Components.EventsDb
{
	[EventDatabaseEntity("spotify_current_playing")]
	public class SporifyCurrentTrackEd : BaseEventDatabaseEntity
	{
		public string ArtistName { get; set; }

		public string SongName { get; set; }

		public string Uri { get; set; }

		public bool IsPlaying { get; set; }
	}
}