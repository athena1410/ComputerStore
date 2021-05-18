//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace ComputerStore.Structure.Constants
{
   public class Constants
   {
      //Default setting for Paging
      public class Paging
      {
         public const int PageSizeDefault = 10;
         public const int PageInit = 1;
         public const int NoPageSize = 0;
      }

      public class MessageResponse
      {
         public const string LoginFailed = "User name or password is not correct!";
         public const string WebsiteNotValid = "Website is not valid";
         public const string UniqueWebsitePerCompanyError = "Company only have one website";
         public const string ForbiddenError = "You don't have permission to access";
         public const string AddProductError = "Could not add product to parent category";
         public const string CompanyNameExisted = "Company with name existed in server";
         public const string WebsiteNameExisted = "Website with name existed in server";
         public const string WebsiteUrlPathExisted = "Website with url path existed in server";
         public const string NotFoundError = "Not found {0} with id: {1}";
         public const string ExistedError = "{0} with {1} existed in server";
         public const string CategoryNameExisted = "Category with name existed in server";
         public const string PhoneNumberInvalid = "Phone number is not valid";
         public const string ExistedProductInCategory = "This category can not be parent category, because existed product.";
         public const string ExistsChildrenCatgory = "This category must be root category, because existed children category.";
         public const string MissingInventoryData = "This {0} with name: {1} is not enough in stock";
         public const string NotChildrentOfSubCategory = "Category with name: {0} is sub category. Can't children of sub category";
         public const string CannotActionSubCategory = "Can't perform any action with this category, because parent category is deactivate";
         public const string OldPasswordIncorrect = "Old password is incorrect";
         public const string NewPasswordNotMeetExpected = "This new password not valid";
         public const string ProductCodeExisted = "Product with product code existed in server";
         public const string InvalidUpdateOrderOperationError = "Can't update this order, because it was {0}.";
         public const string InvalidDiscount = "Discount must be from 0 to 100%";
         public const string InvalidQuantity = "Quantity must be greater than 0";
         public const string InvalidPrice = "Price must be greater than 0";
         public const string InvalidWaranty = "Waranty must be greater than 0";
         public const string CheckOutWithEmptyCartError = "You can't execute check out with empty cart, please reload page and try again!";
        }

      public class RegularExpression
      {
         public const string Phone = @"^((\\+84-?)|0)?[0-9]{10}$";
      }

      public class Utility
      {
         public const int PasswordMinLength = 6;
      }

      public class FolderConst
      {
         public const string TempImages = "TmpImages";
         public const string ProductImages = "ProductImages";
         public const string Assets = "Assets";
      }
   }
}
