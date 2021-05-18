using System;
using System.IO;
using static ComputerStore.Structure.Constants.Constants;

namespace ComputerStore.Structure.Helper
{
   public static class FileHelper
   {
      public static string CreateTempFolder()
      {
         var folderName = Path.Combine(FolderConst.Assets, FolderConst.TempImages);

         // Create image temp if not existed
         if (!Directory.Exists(folderName))
         {
            Directory.CreateDirectory(folderName);
         }

         return folderName;
      }

      /// <summary>
      /// Create new product folder if not exist
      /// </summary>
      public static string CreateProductFolder(string productCode)
      {
         var productFolder = Path.Combine(FolderConst.Assets, FolderConst.ProductImages, productCode);

         // Create folder if not exist
         if (!Directory.Exists(productFolder))
         {
            Directory.CreateDirectory(productFolder);
         }
         return productFolder;
      }

      /// <summary>
      /// Generate new file name from exist file
      /// </summary>
      public static string GenerateFileName(string fileName)
      {
         var extension = Path.GetExtension(fileName);
         return $"{Guid.NewGuid()}{extension}";
      }

      /// <summary>
      /// Move file to new folder
      /// </summary>
      public static string MoveFile(string folderPath, string path)
      {
         if (!File.Exists(path))
         {
            return null;
         }

         var newFileName = GenerateFileName(path);
         var filePath = Path.Combine(folderPath, newFileName);
         File.Move(path, filePath);

         return filePath;
      }
   }
}
