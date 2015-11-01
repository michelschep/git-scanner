using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace Ixmucane.GitScannerConsole
{
    class GitScanner
    {
        public void Scan(string root)
        {
            var rootFolder = new DirectoryInfo(root);

            Scan(rootFolder);
        }

        void Scan(DirectoryInfo rootFolder)
        {
            if (PathIsTooLong(rootFolder))
                return;

            if (IsGitRepo(rootFolder))
            {
                CheckStatusGitFor(rootFolder);
                return;
            }

            foreach (var directoryInfo in rootFolder.GetDirectories())
            {
                Scan(directoryInfo);
            }
        }

        bool PathIsTooLong(DirectoryInfo rootFolder)
        {
            // Hack...
            try
            {
                var fullName = rootFolder.FullName;
                rootFolder.GetDirectories(".git");
                return false;
            }
            catch (PathTooLongException)
            {
                return true;
            }

        }

        bool IsGitRepo(DirectoryInfo rootFolder)
        {
             return rootFolder.GetDirectories(".git").Any();
        }

        void CheckStatusGitFor(DirectoryInfo rootFolder)
        {
            CheckChangesInGitRepo(rootFolder);
        }

        void CheckChangesInGitRepo(DirectoryInfo rootFolder)
        {
            try
            {
                using (var repo = new LibGit2Sharp.Repository(rootFolder.FullName))
                {
                    var options = new StatusOptions();
                    var repositoryStatus = repo.RetrieveStatus(options);

                    if (repositoryStatus.IsDirty)
                        Console.WriteLine("Repo [{0}] is dirty", rootFolder.Name);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Repo [{0}] cannot be read: [{1}]", rootFolder.Name, ex.Message);
            }
        }
    }
}