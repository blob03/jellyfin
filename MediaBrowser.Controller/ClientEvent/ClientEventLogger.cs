﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace MediaBrowser.Controller.ClientEvent
{
    /// <inheritdoc />
    public class ClientEventLogger : IClientEventLogger
    {
        private readonly IServerApplicationPaths _applicationPaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEventLogger"/> class.
        /// </summary>
        /// <param name="applicationPaths">Instance of the <see cref="IServerApplicationPaths"/> interface.</param>
        public ClientEventLogger(IServerApplicationPaths applicationPaths)
        {
            _applicationPaths = applicationPaths;
        }

        /// <inheritdoc />
        public async Task<string> WriteDocumentAsync(string clientName, string clientVersion, Stream fileContents)
        {
            var fileName = $"upload_{clientName}_{clientVersion}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}.log";
            var logFilePath = Path.Combine(_applicationPaths.LogDirectoryPath, fileName);
            if (!Path.GetFullPath(logFilePath).StartsWith(_applicationPaths.LogDirectoryPath, StringComparison.Ordinal))
            {
                throw new ArgumentException("Path resolved to filename not in log directory");
            }

            await using var fileStream = new FileStream(logFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await fileContents.CopyToAsync(fileStream).ConfigureAwait(false);
            return fileName;
        }
    }
}
