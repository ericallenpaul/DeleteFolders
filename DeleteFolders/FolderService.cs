using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DeleteFolders
{
    public class FolderService
    {
        private cliSettings _settings;
        private ILogger<FolderService> _logger;
        private static int _newOrders;
        private static DateTime _LastUpdate;

        public FolderService(ILogger<FolderService> logger, IOptions<cliSettings> settings)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<bool> DeleteFolders(int Renewalday, int RenewalMonth, bool IncludeErrors)
        {
            try
            {
                _logger.LogInformation("Preparing to renew certificates...");

                foreach (var domain in renewals.ToList())
                {
                    try
                    {
                        
                    }
                    catch (Exception exe)
                    {
                        _logger.LogError($"Unexpected error while processing renewal: {exe}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error while processing directories: {ex}");
            }

            return await Task.FromResult(true);
        }

        public void GetSubDirectories()
        {
            string root = @"C:\Temp";

            // Get all subdirectories
            string[] subdirectoryEntries = Directory.GetDirectories(root);

            // Loop through them to see if they have any other subdirectories
            foreach (string subdirectory in subdirectoryEntries)
            {
                LoadSubDirs(subdirectory);
            }
        }

        private void LoadSubDirs(string dir)
        {
            Console.WriteLine(dir);

            string[] subdirectoryEntries = Directory.GetDirectories(dir);
            foreach (string subdirectory in subdirectoryEntries)
            {
                LoadSubDirs(subdirectory);
            }
        }

    }
}
