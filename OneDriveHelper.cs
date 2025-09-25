using System;
using System.IO;
using System.Runtime.InteropServices;

/// <summary>
/// Helper class for detecting OneDrive cloud files (Files On-Demand) that are not stored locally.
/// </summary>
public static class OneDriveHelper
{
    // File attributes for OneDrive Files On-Demand
    private const FileAttributes FILE_ATTRIBUTE_RECALL_ON_OPEN = (FileAttributes)0x00040000;
    private const FileAttributes FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS = (FileAttributes)0x00400000;

    /// <summary>
    /// Checks if a file is a OneDrive cloud file (not stored locally).
    /// </summary>
    /// <param name="filePath">The full path to the file</param>
    /// <returns>True if the file is a OneDrive cloud file, false otherwise</returns>
    public static bool IsOneDriveCloudFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return false;

            var fileInfo = new FileInfo(filePath);
            var attributes = fileInfo.Attributes;

            // Check for OneDrive Files On-Demand attributes
            bool hasRecallOnOpen = (attributes & FILE_ATTRIBUTE_RECALL_ON_OPEN) != 0;
            bool hasRecallOnDataAccess = (attributes & FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS) != 0;

            return hasRecallOnOpen || hasRecallOnDataAccess;
        }
        catch (Exception)
        {
            // If we can't read the file attributes, assume it's not a cloud file
            return false;
        }
    }

    /// <summary>
    /// Checks if a directory path is within a OneDrive folder.
    /// </summary>
    /// <param name="path">The directory or file path to check</param>
    /// <returns>True if the path is within OneDrive, false otherwise</returns>
    public static bool IsInOneDriveFolder(string path)
    {
        try
        {
            // Get common OneDrive paths
            string oneDrivePersonal = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string oneDriveCommercial = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "OneDrive -");

            // Check if path contains OneDrive indicators
            string normalizedPath = path.ToLowerInvariant();
            
            return normalizedPath.Contains("onedrive") || 
                   normalizedPath.Contains(oneDrivePersonal.ToLowerInvariant() + "\\onedrive") ||
                   normalizedPath.Contains(oneDriveCommercial.ToLowerInvariant());
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the OneDrive status of a file.
    /// </summary>
    /// <param name="filePath">The full path to the file</param>
    /// <returns>OneDriveStatus indicating the file's cloud status</returns>
    public static OneDriveStatus GetOneDriveStatus(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return OneDriveStatus.NotOneDrive;

            if (!IsInOneDriveFolder(filePath))
                return OneDriveStatus.NotOneDrive;

            if (IsOneDriveCloudFile(filePath))
                return OneDriveStatus.CloudOnly;

            return OneDriveStatus.LocallyAvailable;
        }
        catch (Exception)
        {
            return OneDriveStatus.Unknown;
        }
    }

    /// <summary>
    /// Gets a user-friendly description of the OneDrive status.
    /// </summary>
    /// <param name="status">The OneDrive status</param>
    /// <returns>A descriptive string</returns>
    public static string GetStatusDescription(OneDriveStatus status)
    {
        return status switch
        {
            OneDriveStatus.CloudOnly => "Cloud only (not downloaded)",
            OneDriveStatus.LocallyAvailable => "Available locally",
            OneDriveStatus.NotOneDrive => "Not in OneDrive",
            OneDriveStatus.Unknown => "Unknown status",
            _ => "Unknown status"
        };
    }
}

/// <summary>
/// Enumeration representing the OneDrive status of a file.
/// </summary>
public enum OneDriveStatus
{
    /// <summary>
    /// File is not in OneDrive
    /// </summary>
    NotOneDrive,
    
    /// <summary>
    /// File is in OneDrive but only stored in the cloud (not downloaded locally)
    /// </summary>
    CloudOnly,
    
    /// <summary>
    /// File is in OneDrive and available locally
    /// </summary>
    LocallyAvailable,
    
    /// <summary>
    /// Unable to determine the status
    /// </summary>
    Unknown
}