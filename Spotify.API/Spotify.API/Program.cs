using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace OpenU.SpotifyAPI
{ 
    static class Program
    {
        private static string _clientId = "21c050b5ae9c4ad7a278ead03d639cd2"; //"";
        private static string _secretId = "c7a0dd3053bd4940808aa7ea8f0d6df8"; //"";

        static void Main(string[] args)
        {
            _clientId = string.IsNullOrEmpty(_clientId)
                ? System.Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID")
                : _clientId;

            _secretId = string.IsNullOrEmpty(_secretId)
                ? System.Environment.GetEnvironmentVariable("SPOTIFY_SECRET_ID")
                : _secretId;

            Console.WriteLine("####### Spotify API Example #######");
            Console.WriteLine("This example uses AuthorizationCodeAuth.");
            Console.WriteLine(
                "Tip: If you want to supply your ClientID and SecretId beforehand, use env variables (SPOTIFY_CLIENT_ID and SPOTIFY_SECRET_ID)");


            AuthorizationCodeAuth auth =
                new AuthorizationCodeAuth(_clientId, _secretId, "http://localhost:4002", "http://localhost:4002",
                    Scope.PlaylistReadPrivate | Scope.PlaylistReadCollaborative);
            auth.AuthReceived += AuthOnAuthReceived;
            auth.Start();
            auth.OpenBrowser();

            Console.ReadLine();
        }

        private static async void AuthOnAuthReceived(object sender, AuthorizationCode payload)
        {
            AuthorizationCodeAuth auth = (AuthorizationCodeAuth)sender;
            auth.Stop();

            Token token = await auth.ExchangeCode(payload.Code);
            SpotifyWebAPI api = new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };
            PrintUsefulData(api);
        }
        

        private static async void PrintUsefulData(SpotifyWebAPI api)
        {
            PrivateProfile profile = await api.GetPrivateProfileAsync();
            string name = string.IsNullOrEmpty(profile.DisplayName) ? profile.Id : profile.DisplayName;
            Console.WriteLine($"Hello there, {name}!");

            List<string> seedsHappiness = new List<string>()
            {
                "0rTkE0FmT4zT2xL6GXwosU", "6NPVjNh8Jhru9xOmyQigds", "3TGRqZ0a2l1LRblBkJoaDx"
            };

            // Get the user top artists and tracks to use as seeds for spotify recommendation API
            Paging<FullArtist> userArtists =  api.GetUsersTopArtists();
            Paging<FullTrack> userTracks = api.GetUsersTopTracks();

            
            // Set the user mood parameters
            //TuneableTrack tuneable = new TuneableTrack();
            //tuneable.Energy = (float?)0.9;


            Console.WriteLine("Your playlists:");
            Paging<SimplePlaylist> playlists = await api.GetUserPlaylistsAsync(profile.Id);
            playlists.Items.ForEach(playlist =>
            {
                Console.WriteLine($"- {playlist.Name}");
            });

            
            
            userArtists.Items.ForEach(artist =>
            {
                Console.WriteLine($"- {artist.Name}");
            });           
        }
    }
}
