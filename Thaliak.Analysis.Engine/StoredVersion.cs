using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Thaliak.Common.Database;
using Thaliak.Common.Database.Models;
using XIVLauncher.Common;

namespace Thaliak.Analysis.Engine;

public class StoredVersion
{
    private const int MINIMUM_GB_FREE_TO_STAGE = 40;

    private readonly ThaliakContext _db;
    private readonly DirectoryInfo _storageDirectory;
    public XivVersion Version { get; }
    public DirectoryInfo StagingDirectory { get; }

    public StoredVersion(ThaliakContext db, DirectoryInfo storageDirectory, XivVersion version,
        DirectoryInfo stagingDirectory)
    {
        _db = db;
        _storageDirectory = storageDirectory;
        Version = version;
        StagingDirectory = stagingDirectory;
    }

    /**
     * Checks if the version is stored in the file repository.
     * This does a simple check to see if all associated files exist in storage via their SHA hash.
     */
    public bool CheckStored()
    {
        if (Version.Files.Count == 0) {
            return false;
        }

        if (Version.RepositoryId != 2) {
            throw new InvalidOperationException("currently only ffxivneo/win32/release/game is supported");
        }

        return Version.Files.TrueForAll(xf =>
        {
            if (!xf.IsChecksumValid) {
                throw new InvalidOperationException("XivFile.IsChecksumValid = false!");
            }

            var storName = xf.GetStorageFileName();
            if (storName == null) {
                // 0-byte file (if the checksum is valid); ignore it
                return true;
            }

            return File.Exists(Path.Join(_storageDirectory.FullName, storName));
        });
    }

    /**
     * Checks if the version is staged in the file repository.
     * This does a simple check of ffxivgame.ver to see if the version is staged in the target directory.
     */
    public bool CheckStaged()
    {
        if (Version.RepositoryId != 2) {
            throw new InvalidOperationException("currently only ffxivneo/win32/release/game is supported");
        }

        var dir = StagingDirectory;
        if (dir.Name == "game") {
            dir = dir.Parent;
        }

        return dir!.Exists && Repository.Ffxiv.GetVer(dir).Trim() == Version.VersionString.Trim();
    }

    public void StageFromStorage(bool nukeExistingStagingDir = false, bool symlink = false)
    {
        if (!CheckStored()) {
            throw new Exception("Version is not stored");
        }

        if (StagingDirectory.Exists && StagingDirectory.EnumerateFileSystemInfos().Any()) {
            if (nukeExistingStagingDir) {
                StagingDirectory.Delete(true);
                Directory.CreateDirectory(Path.GetDirectoryName(StagingDirectory.FullName)!);
            } else {
                throw new Exception("Staging directory exists and is not empty");
            }
        }

        if (!symlink && new DriveInfo(StagingDirectory.FullName).AvailableFreeSpace <
            MINIMUM_GB_FREE_TO_STAGE * 1024 * 1024) {
            throw new Exception("Not enough free space to stage with full copies (need at least " +
                                MINIMUM_GB_FREE_TO_STAGE + " GB)");
        }

        foreach (var xf in Version.Files) {
            Console.WriteLine(xf.Name + " " + xf.SHA1);
            var storName = xf.GetStorageFileName();
            if (storName == null) {
                continue;
            }

            var srcPath = Path.Join(_storageDirectory.FullName, storName);
            var dstPath = Path.Join(StagingDirectory.FullName, xf.Name);

            // ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(dstPath)!);

            if (symlink) {
                File.CreateSymbolicLink(dstPath, srcPath);
            } else {
                File.Copy(srcPath, dstPath);
            }
        }
    }

    public void StoreFromStaging(bool storeGameData = true)
    {
        if (!CheckStaged() || !StagingDirectory.Exists || !StagingDirectory.EnumerateFileSystemInfos().Any()) {
            throw new Exception("Version is not staged");
        }

        var now = DateTime.UtcNow;

        // recursively enumerate and store filenames from the staging directory
        var filesToStore = StagingDirectory.EnumerateFiles("*", SearchOption.AllDirectories)
            .Select(f => f.FullName.Substring(StagingDirectory.FullName.Length))
            .Where(f => !f.StartsWith(Path.Join("sqpack", "ex"))) // exclude expansion sqpacks
            .Where(f => !f.StartsWith(Path.Join("movie", "ex"))); // exclude expansion movies

        if (!storeGameData) {
            filesToStore = filesToStore.Where(f => !f.StartsWith("sqpack") && !f.StartsWith("movie"));
        }

        foreach (var fileName in filesToStore.ToList()) {
            // calculate SHA1 hash of file
            Console.WriteLine(fileName);
            var srcPath = Path.Join(StagingDirectory.FullName, fileName);
            var fileInfo = new FileInfo(srcPath);

            // follow symlinks
            var linkTarget = fileInfo.ResolveLinkTarget(true);
            if (linkTarget != null) {
                srcPath = linkTarget.FullName;
            }

            var fileSize = (ulong) fileInfo.Length;

            using var fs = new FileStream(srcPath, FileMode.Open, FileAccess.Read);
            using var bs = new BufferedStream(fs);
            using var sha1 = SHA1.Create();

            var hash = sha1.ComputeHash(bs);
            var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            var xf = _db.Files
                .Include(f => f.Versions)
                .FirstOrDefault(xf => xf.Name == fileName && xf.SHA1 == hashString);

            // do we need a new entry?
            if (xf != null) {
                xf.LastUsed = now;
                _db.Files.Attach(xf);
            } else {
                // file doesn't exist in the db, create a new entry
                xf = new XivFile
                {
                    Name = fileName,
                    Size = fileSize,
                    SHA1 = hashString,
                    LastUsed = now
                };

                _db.Files.Add(xf);
            }

            Console.WriteLine(xf.SHA1);
            var dstPath = Path.Join(_storageDirectory.FullName, xf.GetStorageFileName());
            Console.WriteLine($"{srcPath} -> {dstPath}");

            if (!File.Exists(dstPath)) {
                // copy to storage
                Directory.CreateDirectory(Path.GetDirectoryName(dstPath)!);
                File.Copy(srcPath, dstPath);
            }

            // add this version to the list of versions this file is part of
            if (!xf.Versions.Any(v => v.VersionString == Version.VersionString)) {
                xf.Versions.Add(Version);
            }

            _db.Versions.Attach(Version);

            // commit crimes
            _db.SaveChanges();
        }
    }
}
