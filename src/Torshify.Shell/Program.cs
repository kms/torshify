﻿using System;
using System.Threading;

namespace Torshify.Shell
{
    class Program
    {
        #region Fields

        private ManualResetEventSlim _logInEvent = new ManualResetEventSlim(false);

        #endregion Fields

        #region Properties

        public ISession Session
        {
            get;
            private set;
        }

        #endregion Properties

        #region Public Methods

        public void Run()
        {
            InitializeSession();

            ConsoleEx.Write("Enter username >> ", ConsoleColor.Green);
            string userName = Console.ReadLine();
            ConsoleEx.Write("Enter password >> ", ConsoleColor.Green);
            string password = ConsoleEx.GetPassword();

            for (int i = 0; i < password.Length; i++)
            {
                Console.Write("*");
            }
            Console.WriteLine();

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ConsoleEx.WriteLine("Please provide a username and password", ConsoleColor.Red);
                return;
            }

            ConsoleEx.WriteLine("Logging in..");

            Session.Login(userName, password);

            _logInEvent.Wait(5000);

            ConsoleKeyInfo keyInfo;

            do
            {
                using (ConsoleEx.BeginColorBlock(ConsoleColor.Cyan))
                {
                    Console.WriteLine("=== Main menu ===");
                    Console.WriteLine("1: Search");
                    Console.WriteLine("2: Toplists");
                    Console.WriteLine("3: Playlists");
                    Console.WriteLine("4: Friends");
                    Console.WriteLine("5: Radio");
                    Console.WriteLine("6: Current user info");
                    Console.WriteLine("=================");
                }

                ConsoleEx.Write("Enter menu >> ", ConsoleColor.Green);
                keyInfo = Console.ReadKey();
                Console.Write(Environment.NewLine);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                        SearchMenu();
                        break;
                    case ConsoleKey.D2:
                        ToplistsMenu();
                        break;
                    case ConsoleKey.D3:
                        PlaylistsMenu();
                        break;
                    case ConsoleKey.D4:
                        FriendsMenu();
                        break;
                    case ConsoleKey.D5:
                        RadioMenu();
                        break;
                    case ConsoleKey.D6:
                        CurrentUserInfoMenu();
                        break;
                }

                if (keyInfo.Key != ConsoleKey.Escape)
                {
                    ConsoleEx.Write("Press any key to continue (except Esc which will exit)", ConsoleColor.Yellow);
                    keyInfo = Console.ReadKey();
                    Console.WriteLine();
                }

            } while (keyInfo.Key != ConsoleKey.Escape);

            if (Session.ConnectionState == ConnectionState.LoggedIn)
            {
                Session.Logout();
                Session.Dispose();
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected void InitializeSession()
        {
            Session = SessionFactory
                .CreateSession(
                    Constants.ApplicationKey,
                    Constants.CacheFolder,
                    Constants.SettingsFolder,
                    Constants.UserAgent)
                .SetPreferredBitrate(Bitrate.Bitrate320k)
                .SetPreferredOfflineBitrate(Bitrate.Bitrate96k, false);

            Session.LoginComplete += UserLoggedIn;
            Session.LogoutComplete += UserLoggedOut;
            Session.ConnectionError += ConnectionError;
        }

        protected void SearchMenu()
        {
            ConsoleEx.WriteLine("=== Search ===", ConsoleColor.Cyan);
            ConsoleEx.Write("Query >> ", ConsoleColor.Green);
            string query = Console.ReadLine();

            ConsoleEx.WriteLine("Searching..", ConsoleColor.Yellow);
            ISearch search = Session
                .Search(query, 0, 25, 0, 25, 0, 25)
                .WaitForCompletion();

            if (!string.IsNullOrEmpty(search.DidYouMean))
            {
                ConsoleEx.WriteLine("Maybe you ment " + search.DidYouMean + "?", ConsoleColor.Magenta);
            }

            for (int i = 0; i < search.Tracks.Count; i++)
            {
                ITrack track = search.Tracks[i];

                ConsoleEx.Write("{0:00} : {1,-20}", ConsoleColor.White, (i + 1), ConsoleEx.Truncate(track.Name, 20));
                ConsoleEx.Write(" {0,-16}", ConsoleColor.Gray, ConsoleEx.Truncate(track.Album.Artist.Name, 15));
                ConsoleEx.WriteLine(" {0,-16}", ConsoleColor.DarkGray, ConsoleEx.Truncate(track.Album.Name, 15));
            }
        }

        protected void ToplistsMenu()
        {
            ConsoleEx.WriteLine("=== Toplist ===", ConsoleColor.Cyan);
            IToplistBrowse toplistBrowse = Session
                                            .Browse(ToplistType.Tracks)
                                            .WaitForCompletion();

            for (int i = 0; i < toplistBrowse.Tracks.Count; i++)
            {
                ITrack track = toplistBrowse.Tracks[i];

                ConsoleEx.Write("{0:00} : {1,-20}", ConsoleColor.White, (i + 1), ConsoleEx.Truncate(track.Name, 20));
                ConsoleEx.Write(" {0,-16}", ConsoleColor.Gray, ConsoleEx.Truncate(track.Album.Artist.Name, 15));
                ConsoleEx.WriteLine(" {0,-16}", ConsoleColor.DarkGray, ConsoleEx.Truncate(track.Album.Name, 15));
            }
        }

        protected void PlaylistsMenu()
        {
            ConsoleEx.WriteLine("=== Playlists ===", ConsoleColor.Cyan);

            for (int i = 0; i < Session.PlaylistContainer.Playlists.Count; i++)
            {
                IPlaylist playlist = Session.PlaylistContainer.Playlists[i];

                ConsoleEx.WriteLine("{0:00} : {1,-20}", ConsoleColor.White, (i + 1), playlist.Name);
            }
        }

        protected void FriendsMenu()
        {
            ConsoleEx.WriteLine("=== Friends ===", ConsoleColor.Cyan);

            for (int i = 0; i < Session.Friends.Count; i++)
            {
                IUser user = Session.Friends[i];

                ConsoleEx.Write("{0:00} : {1,-20}", ConsoleColor.White, (i + 1), ConsoleEx.Truncate(user.CanonicalName, 20));
                ConsoleEx.Write(" {0,-16}", ConsoleColor.Gray, ConsoleEx.Truncate(user.DisplayName, 15));
                ConsoleEx.WriteLine(" {0,-16}", ConsoleColor.DarkGray, ConsoleEx.Truncate(user.FullName, 15));
            }
        }

        protected void RadioMenu()
        {
            ConsoleEx.WriteLine("=== Radio ===", ConsoleColor.Cyan);

            string[] genres = Enum.GetNames(typeof (RadioGenre));

            for (int i = 0; i < genres.Length; i++)
            {
                string genre = genres[i];

                ConsoleEx.WriteLine("{0:00} : {1,-20}", ConsoleColor.Cyan, (i + 1), genre);
            }

            ConsoleEx.Write("Choose radio genre >> ", ConsoleColor.Green);
            var keyInfo = Console.ReadKey();
            Console.Write(Environment.NewLine);

            int index = int.Parse(keyInfo.KeyChar.ToString());
            RadioGenre radioGenre;
            Enum.TryParse(genres[index], out radioGenre);

            ISearch search =
                Session
                    .Search(1990, DateTime.Now.Year, radioGenre)
                    .WaitForCompletion();

            for (int i = 0; i < search.Tracks.Count; i++)
            {
                ITrack track = search.Tracks[i];

                ConsoleEx.Write("{0:00} : {1,-20}", ConsoleColor.White, (i + 1), ConsoleEx.Truncate(track.Name, 20));
                ConsoleEx.Write(" {0,-16}", ConsoleColor.Gray, ConsoleEx.Truncate(track.Album.Artist.Name, 15));
                ConsoleEx.WriteLine(" {0,-16}", ConsoleColor.DarkGray, ConsoleEx.Truncate(track.Album.Name, 15));
            }
        }

        protected void CurrentUserInfoMenu()
        {
            ConsoleEx.WriteLine("=== Current user info ===", ConsoleColor.Cyan);

            ConsoleEx.Write("Username:     ", ConsoleColor.DarkYellow);
            ConsoleEx.WriteLine(Session.LoggedInUser.CanonicalName, ConsoleColor.White);

            ConsoleEx.Write("Display name: ", ConsoleColor.DarkYellow);
            ConsoleEx.WriteLine(Session.LoggedInUser.DisplayName, ConsoleColor.White);

            ConsoleEx.Write("Full name:    ", ConsoleColor.DarkYellow);
            ConsoleEx.WriteLine(Session.LoggedInUser.FullName, ConsoleColor.White);

            ConsoleEx.Write("Picture ID:   ", ConsoleColor.DarkYellow);
            ConsoleEx.WriteLine(Session.LoggedInUser.Picture, ConsoleColor.White);

            ConsoleEx.Write("Country code: ", ConsoleColor.DarkYellow);
            ConsoleEx.WriteLine(Session.LoggedInUserCountry.ToString(), ConsoleColor.White);
        }

        #endregion Protected Methods

        #region Private Static Methods

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

        #endregion Private Static Methods

        #region Private Methods

        private void ConnectionError(object sender, SessionEventArgs e)
        {
            if (e.Status != Error.OK)
            {
                ConsoleEx.WriteLine("Connection error: " + e.Message, ConsoleColor.Red);
            }
        }

        private void UserLoggedOut(object sender, SessionEventArgs e)
        {
            ConsoleEx.WriteLine("Logged out..", ConsoleColor.Yellow);
        }

        private void UserLoggedIn(object sender, SessionEventArgs e)
        {
            if (e.Status == Error.OK)
            {
                ConsoleEx.WriteLine("Successfully logged in", ConsoleColor.Yellow);
            }
            else
            {
                ConsoleEx.WriteLine("Unable to log in: " + e.Status.GetMessage(), ConsoleColor.Red);
                Console.ReadLine();
                Environment.Exit(-1);
            }

            _logInEvent.Set();
        }

        #endregion Private Methods
    }
}