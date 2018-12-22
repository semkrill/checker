// This is a personal academic project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace FastSearchLibrary
{
    internal class DirectoryCancellationDelegateSearcher : DirectoryCancellationSearcherBase
    {

        private Func<DirectoryInfo, bool> isValid;

        public DirectoryCancellationDelegateSearcher(string folder, Func<DirectoryInfo, bool> isValid, CancellationToken token, ExecuteHandlers handlerOption, bool suppressOperationCanceledException)
            :base(folder, token, handlerOption, suppressOperationCanceledException)
        {
            this.isValid = isValid;
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
            catch (UnauthorizedAccessException )
            {
                return;
            }
            catch (PathTooLongException )
            {
                return;
            }
            catch (DirectoryNotFoundException )
            {
                return;
            }


            foreach (var dir in directories)
            {
                token.ThrowIfCancellationRequested();

                GetDirectories(dir.FullName);
            }

            token.ThrowIfCancellationRequested();

            try
            {
                List<DirectoryInfo> resultDirs = new List<DirectoryInfo>();

                foreach (var dir in directories)
                {
                    if (isValid(dir))
                        resultDirs.Add(dir);
                }

                if (resultDirs.Count > 0)
                    OnDirectoriesFound(resultDirs);

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
            List<DirectoryInfo> resultDirs = new List<DirectoryInfo>();

            try
            {
                dirInfo = new DirectoryInfo(folder);
                directories = dirInfo.GetDirectories();

                if (directories.Length > 1)
                {

                    foreach (var dir in directories)
                    {
                        if (isValid(dir))
                            resultDirs.Add(dir); 
                    }

                    if (resultDirs.Count > 0)
                        OnDirectoriesFound(resultDirs);

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
            foreach (var dir in directories)
            {
                if (isValid(dir))
                    OnDirectoriesFound(new List<DirectoryInfo> { dir });
            }

            return GetStartDirectories(directories[0].FullName);
        }
    }
}
