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
            // TODO For some reason when a path is too long the IsGitRepo gives an exception
            // For now just ignore these folders. 
            if (PathIsTooLong(rootFolder))
                return;

            if (IsGitRepo(rootFolder))
            {
                CheckChangesInGitRepo(rootFolder);
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

        void CheckChangesInGitRepo(DirectoryInfo rootFolder)
        {
            try
            {
                var repo = new Repository(rootFolder.FullName);
                var options = new StatusOptions();
                var repositoryStatus = repo.RetrieveStatus(options);

                if (repositoryStatus.IsDirty)
                    Console.WriteLine("Repo [{0} in {1}] is dirty", rootFolder.Name, rootFolder.Parent.FullName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Repo [{0}] cannot be read: [{1}]", rootFolder.Name, ex.Message);
            }
        }
    }
}