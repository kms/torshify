using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
    internal partial class Spotify
    {
        /// <summary>
        /// Create a search object from the given query.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="query">Query search string, e.g. 'The Rolling Stones' or 'album:"The Black Album"'</param>
        /// <param name="track_offset">The offset among the tracks of the result.</param>
        /// <param name="track_count">The number of tracks to ask for.</param>
        /// <param name="album_offset">The offset among the albums of the result.</param>
        /// <param name="album_count">The number of albums to ask for.</param>
        /// <param name="artist_offset">The offset among the artists of the result.</param>
        /// <param name="artist_count">The number of artists to ask for.</param>
        /// <param name="callbackPtr">Callback that will be called once the search operation is complete.
        /// Pass null if you are not interested in this event.</param>
        /// <param name="userdataPtr">Opaque pointer passed to callback.</param>
        /// <returns>Pointer to a search object. To free the object, use <c>sp_search_release()</c></returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_search_create(
            IntPtr sessionPtr, 
            string query, 
            int track_offset,
            int track_count, 
            int album_offset,
            int album_count,
            int artist_offset,
            int artist_count,
            IntPtr callbackPtr, 
            IntPtr userdataPtr);

        /// <summary>
        /// Create a search object from the radio channel.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="fromYear">Include tracks starting from this year.</param>
        /// <param name="toYear">Include tracks up to this year.</param>
        /// <param name="genres">Bitmask of genres to include.</param>
        /// <param name="callbackPtr">Callback that will be called once the search operation is complete.
        /// Pass null if you are not interested in this event.</param>
        /// <param name="userdataPtr">Opaque pointer passed to callback.</param>
        /// <returns>Pointer to a search object. To free the object, use <c>sp_search_release()</c></returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_radio_search_create(
            IntPtr sessionPtr,
            uint fromYear,
            uint toYear,
            RadioGenre genres, 
            IntPtr callbackPtr,
            IntPtr userdataPtr);

        /// <summary>
        /// Get load status for the specified search. Before it is loaded, it will behave as an empty search result.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>True if search is loaded, otherwise false.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_search_is_loaded(IntPtr searchPtr);

        /// <summary>
        /// Check if search returned an error code.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_search_error(IntPtr searchPtr);

        /// <summary>
        /// Get the number of tracks for the specified search.
        /// </summary>
        /// <param name="searchPtr">A serach object.</param>
        /// <returns>The number of tracks for the specified search.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_num_tracks(IntPtr searchPtr);

        /// <summary>
        /// Return the track at the given index in the given search object.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <param name="index">Index of the wanted track. Should be in the interval [0, <c>sp_search_num_tracks()</c> - 1]</param>
        /// <returns>The track at the given index in the given search object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_search_track(IntPtr searchPtr, int index);

        /// <summary>
        /// Get the number of albums for the specified search.
        /// </summary>
        /// <param name="searchPtr">A serach object.</param>
        /// <returns>The number of albums for the specified search.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_num_albums(IntPtr searchPtr);

        /// <summary>
        /// Return the album at the given index in the given search object.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <param name="index">Index of the wanted album. Should be in the interval [0, <c>sp_search_num_albums()</c> - 1]</param>
        /// <returns>The album at the given index in the given search object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_search_album(IntPtr searchPtr, int index);

        /// <summary>
        /// Get the number of artists for the specified search.
        /// </summary>
        /// <param name="searchPtr">A serach object.</param>
        /// <returns>The number of artists for the specified search.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_num_artists(IntPtr searchPtr);

        /// <summary>
        /// Return the artist at the given index in the given search object.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <param name="index">Index of the wanted artist. Should be in the interval [0, <c>sp_search_num_artists()</c> - 1]</param>
        /// <returns>The artist at the given index in the given search object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_search_artist(IntPtr searchPtr, int index);

        /// <summary>
        /// Return the search query for the given search object.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The search query for the given search object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_search_query(IntPtr searchPtr);

        /// <summary>
        /// Return the "Did you mean" query for the given search object.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The "Did you mean" query for the given search object, or the empty string if no such info is available.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_search_did_you_mean(IntPtr searchPtr);

        /// <summary>
        /// Return the total number of tracks for the search query - regardless of the interval requested at creation.
        /// If this value is larger than the interval specified at creation of the search object, more search results
        /// are available. To fetch these, create a new search object with a new interval.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The total number of tracks matching the original query.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_total_tracks(IntPtr searchPtr);

        /// <summary>
        /// Return the total number of albums for the search query - regardless of the interval requested at creation.
        /// If this value is larger than the interval specified at creation of the search object, more search results
        /// are available. To fetch these, create a new search object with a new interval.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The total number of albums matching the original query.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_total_albums(IntPtr searchPtr);

        /// <summary>
        /// Return the total number of artists for the search query - regardless of the interval requested at creation.
        /// If this value is larger than the interval specified at creation of the search object, more search results
        /// are available. To fetch these, create a new search object with a new interval.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The total number of artists matching the original query.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_total_artists(IntPtr searchPtr);

        /// <summary>
        /// Increase the reference count of a search result.
        /// </summary>
        /// <param name="searchPtr">A serach object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_search_add_ref(IntPtr searchPtr);

        /// <summary>
        /// Decrease the reference count of a search result.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_search_release(IntPtr searchPtr);
    }
}