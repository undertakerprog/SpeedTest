namespace Web.Src.Model
{
    /// <summary>
    /// Represents a request to update the host of a server.
    /// </summary>
    public class UpdateHostRequest
    {
        /// <summary>
        /// Gets or initializes the current hostname or IP address that needs to be updated.
        /// Defaults to "unknown" if not specified.
        /// </summary>
        public string OldHost { get; init; } = "unknown";

        /// <summary>
        /// Gets or initializes the new hostname or IP address to replace the old one.
        /// Defaults to "unknown" if not specified.
        /// </summary>
        public string NewHost { get; init; } = "unknown";
    }
}