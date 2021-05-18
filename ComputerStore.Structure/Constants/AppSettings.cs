//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

namespace ComputerStore.Structure.Constants
{
    public static class AppSettings
    {
        /// <summary>
        /// Name of the settings file name
        /// </summary>
        public const string AppSettingsFileName = "appsettings.json";

        /// <summary>
        /// Name of the connection string
        /// </summary>
        public const string DefaultConnection = nameof(DefaultConnection);

        /// <summary>
        /// Connection string section
        /// </summary>
        public const string ConnectionStrings = nameof(ConnectionStrings);

        /// <summary>
        /// Id of the swagger client
        /// </summary>
        public const string SwaggerClientId = "ComputerStoreApi_Swagger";
        /// <summary>
        /// Jwt configuration
        /// </summary>
        public const string JwtConfiguration = "JwtSettings";
    }
}
