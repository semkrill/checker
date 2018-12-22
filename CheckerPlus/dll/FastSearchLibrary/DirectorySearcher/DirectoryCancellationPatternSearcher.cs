using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace FastSearchLibrary
{
    internal class DirectoryCancellationPatternSearcher : DirectoryCancellationSearcherBase
    {

        private string pattern;

        public DirectoryCancellationPatternSearcher(string folder, string pattern, CancellationToken token, ExecuteHandlers handlerOption, bool suppressOperationCanceledException)
            : base(folder, token, handlerOption, suppressOperationCanceledException)
        {
            this.pattern = pattern;
        }


        protected override void GetDirectories(string folder)
        {
            token.ThrowIfCancellationRequested();

            DirectoryInfo dirInfo = null;
            DirectoryInfo[] directories = null;

            try
            {
                dirInfo = new DirectoryInfo(folder);
                directories = dirInfo.GetDirectories();

                if (directories.Length == 0) return;
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }
            catch (PathTooLongException)
            {
                return;
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }


            foreach (var d in directories)
            {
                token.ThrowIfCancellationRequested();

                GetDirectories(d.FullName);
            }


            token.ThrowIfCancellationRequested();

            try
            {
                var resultDirs = dirInfo.GetDirectories(pattern);
                if (resultDirs.Length > 0)
                    OnDirectoriesFound(resultDirs.ToList());
            }
            catch (UnauthorizedAccessException )
            {
            }
            catch (PathTooLongException )
            {
            }
            catch (DirectoryNotFoundException )
            {
            }
        }

        protected override List<DirectoryInfo> GetStartDirectories(string folder)
        {
            token.ThrowIfCancellationRequested();

            DirectoryInfo dirInfo = null;
            DirectoryInfo[] directories = null;
            DirectoryInfo[] resultDirs = null;

            try
            {
                dirInfo = new DirectoryInfo(folder);
                directories = dirInfo.GetDirectories();


                if (directories.Length > 1)
                {
                    resultDirs = dirInfo.GetDirectories(pattern);
                    if (resultDirs.Length > 0)
                        OnDirectoriesFound(resultDirs.ToList());

                    return new List<DirectoryInfo>(directories);
                }

                if (directories.Length == 0)
                    return new List<DirectoryInfo>();

            }
            catch (UnauthorizedAccessException )
            {
                return new List<DirectoryInfo>();
            }
            catch (PathTooLongException )
            {
                return new List<DirectoryInfo>();
            }
            catch (DirectoryNotFoundException )
            {
                return new List<DirectoryInfo>();
            }

            // if directories.Length == 1
            resultDirs = dirInfo.GetDirectories(pattern);
            if (resultDirs.Length > 0)
                OnDirectoriesFound(resultDirs.ToList());

            return GetStartDirectories(directories[0].FullName);
        }
    }
}
