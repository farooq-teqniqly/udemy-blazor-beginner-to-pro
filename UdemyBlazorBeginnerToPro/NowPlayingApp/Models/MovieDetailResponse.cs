using System.Text.Json.Serialization;

namespace NowPlayingApp.Models
{
    /// <summary>
    /// Represents the full movie detail response returned by TMDB.
    /// </summary>
    public class MovieDetailResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the movie is marked as adult content.
        /// </summary>
        [JsonPropertyName("adult")]
        public bool Adult { get; set; }

        /// <summary>
        /// Gets or sets the backdrop image path.
        /// </summary>
        [JsonPropertyName("backdrop_path")]
        public string BackdropPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the franchise or series this movie belongs to, or <see langword="null"/> if none.
        /// </summary>
        [JsonPropertyName("belongs_to_collection")]
        public MovieFranchise? BelongsToCollection { get; set; }

        /// <summary>
        /// Gets or sets the production budget in US dollars.
        /// </summary>
        [JsonPropertyName("budget")]
        public long Budget { get; set; }

        /// <summary>
        /// Gets or sets the genres associated with the movie.
        /// </summary>
        [JsonPropertyName("genres")]
        public List<Genre> Genres { get; set; } = [];

        /// <summary>
        /// Gets or sets the official homepage URL of the movie.
        /// </summary>
        [JsonPropertyName("homepage")]
        public string Homepage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the TMDB movie identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the IMDb identifier.
        /// </summary>
        [JsonPropertyName("imdb_id")]
        public string ImdbId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ISO 3166-1 alpha-2 country codes of the movie's origin countries.
        /// </summary>
        [JsonPropertyName("origin_country")]
        public string[] OriginCountry { get; set; } = [];

        /// <summary>
        /// Gets or sets the original language code for the movie.
        /// </summary>
        [JsonPropertyName("original_language")]
        public string OriginalLanguage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the original title of the movie.
        /// </summary>
        [JsonPropertyName("original_title")]
        public string OriginalTitle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the movie overview text.
        /// </summary>
        [JsonPropertyName("overview")]
        public string Overview { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the popularity score returned by TMDB.
        /// </summary>
        [JsonPropertyName("popularity")]
        public float Popularity { get; set; }

        /// <summary>
        /// Gets or sets the poster image path.
        /// </summary>
        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the companies that produced the movie.
        /// </summary>
        [JsonPropertyName("production_companies")]
        public List<ProductionCompany> ProductionCompanies { get; set; } = [];

        /// <summary>
        /// Gets or sets the countries where the movie was produced.
        /// </summary>
        [JsonPropertyName("production_countries")]
        public List<ProductionCountry> ProductionCountries { get; set; } = [];

        /// <summary>
        /// Gets or sets the release date string returned by TMDB.
        /// </summary>
        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the box office revenue in US dollars.
        /// </summary>
        [JsonPropertyName("revenue")]
        public long Revenue { get; set; }

        /// <summary>
        /// Gets or sets the runtime of the movie in minutes.
        /// </summary>
        [JsonPropertyName("runtime")]
        public int Runtime { get; set; }

        /// <summary>
        /// Gets or sets the languages spoken in the movie.
        /// </summary>
        [JsonPropertyName("spoken_languages")]
        public List<SpokenLanguage> SpokenLanguages { get; set; } = [];

        /// <summary>
        /// Gets or sets the release status of the movie.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the marketing tagline of the movie.
        /// </summary>
        [JsonPropertyName("tagline")]
        public string Tagline { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display title of the movie.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the item is a video.
        /// </summary>
        [JsonPropertyName("video")]
        public bool Video { get; set; }

        /// <summary>
        /// Gets or sets the average vote score.
        /// </summary>
        [JsonPropertyName("vote_average")]
        public float VoteAverage { get; set; }

        /// <summary>
        /// Gets or sets the number of votes received.
        /// </summary>
        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }
    }
}
