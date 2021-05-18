//-----------------------------------------------------------------------
// <copyright file="MappingProfile.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AutoMapper.Configuration;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Structure.Models.Cart;
using ComputerStore.Structure.Models.Category;
using ComputerStore.Structure.Models.Company;
using ComputerStore.Structure.Models.Order;
using ComputerStore.Structure.Models.Product;
using ComputerStore.Structure.Models.ProductImage;
using ComputerStore.Structure.Models.User;
using ComputerStore.Structure.Models.Website;

namespace ComputerStore.Api.Mappings
{
	/// <summary>
	/// Contains objects mapping
	/// </summary>
	/// <seealso cref="AutoMapper.Configuration.MapperConfigurationExpression" />
	public class MappingProfile : MapperConfigurationExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MappingProfile"/> class
		/// </summary>
		public MappingProfile()
		{
			//Mapping model to entities
			CreateMap<CompanyModel, Company>()
				.ForMember(x => x.Id, opt => opt.Ignore())
				.ForMember(x => x.CreatedDate, opt => opt.Ignore())
				.ForMember(x => x.UpdatedDate, opt => opt.Ignore());

			CreateMap<WebsiteModel, Website>()
				.ForMember(x => x.Id, opt => opt.Ignore())
				.ForMember(x => x.SecretKey, opt => opt.Ignore())
				.ForMember(x => x.CreatedDate, opt => opt.Ignore())
				.ForMember(x => x.UpdatedDate, opt => opt.Ignore());

			CreateMap<CategoryModel, Category>()
				.ForMember(x => x.Id, opt => opt.Ignore())
				.ForMember(x => x.CreatedDate, opt => opt.Ignore())
				.ForMember(x => x.UpdatedDate, opt => opt.Ignore())
				.ForMember(x => x.WebsiteId, opt => opt.Ignore());

			CreateMap<ProductModel, Product>()
				.ForMember(x => x.Id, opt => opt.Ignore())
				.ForMember(x => x.CreatedDate, opt => opt.Ignore())
				.ForMember(x => x.UpdatedDate, opt => opt.Ignore())
				.ForMember(x => x.ProductImage, opt => opt.Ignore());

			CreateMap<UserModel, User>()
				.ForMember(x => x.Id, opt => opt.Ignore())
				.ForMember(x => x.CreatedDate, opt => opt.Ignore())
				.ForMember(x => x.UpdatedDate, opt => opt.Ignore())
				.ForMember(x => x.RoleId, opt => opt.Ignore())
				.ForMember(x => x.LastLogin, opt => opt.Ignore())
				.ForMember(x => x.Email, opt => opt.Condition(src => src.Id == 0))
				.ForMember(x => x.Role, opt => opt.Ignore());

			CreateMap<UserUpdateModel, User>()
				.ForMember(x => x.Id, opt => opt.Ignore())
				.ForMember(x => x.Email, opt => opt.Ignore())
				.ForMember(x => x.WebsiteId, opt => opt.Ignore())
				.ForMember(x => x.Password, opt => opt.Ignore());

			CreateMap<OrderModel, Order>()
				.ForMember(x => x.User, opt => opt.Ignore())
				.ForMember(x => x.Id, opt => opt.Ignore())
				.ForMember(x => x.OrderDetail, opt => opt.Ignore());

			CreateMap<OrderDetailModel, OrderDetail>();

			CreateMap<OrderCreateModel, Order>()
				.ForMember(x => x.OrderDetail, opt => opt.MapFrom(x => new List<OrderDetail>()));

			CreateMap<AnonymousCartModel, AnonymousCart>()
				.ForMember(x => x.WebsiteId, opt => opt.Ignore())
				.ForMember(x => x.ProductId, opt => opt.Ignore())
				.ForMember(x => x.IdentityCode, opt => opt.Ignore());

			CreateMap<AnonymousCartCreateModel, AnonymousCart>()
				.ForMember(x => x.IdentityCode, opt => opt.MapFrom(src => src.IdentityCode.ToString()));

			CreateMap<CartModel, Cart>()
				.ForMember(x => x.WebsiteId, opt => opt.Ignore())
				.ForMember(x => x.UserId, opt => opt.Ignore())
				.ForMember(x => x.ProductId, opt => opt.Ignore());

			CreateMap<CartCreateModel, Cart>();

			//Mapping entities to model
			CreateMap<User, UserModel>()
				.ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
				.ForMember(x => x.Password, opt => opt.Ignore())
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.LastName} {src.FirstName}"));

			CreateMap<Website, WebsiteModel>()
				.ForMember(dest=>dest.IsDisable,opt=>opt.MapFrom(src=>System.Convert.ToBoolean(src.Company.Status)));
			CreateMap<Company, CompanyModel>();
			CreateMap<Product, ProductModel>();
			CreateMap<Category, CategoryModel>();
			CreateMap<ProductImage, ProductImageModel>();

			CreateMap<Order, OrderModel>()
				.ForMember(dest => dest.UserDisplayName, opt => opt.MapFrom(src => $"{src.User.LastName} {src.User.FirstName}"))
				.ForMember(dest => dest.Total, opt => opt.MapFrom((src, dest) =>
				{
					return src.OrderDetail.Sum(item => (item.Price * item.Quantity) * (1 - (item.Discount / 100)));
				}))
				.ForMember(x => x.User, opt => opt.Ignore());
			CreateMap<OrderDetail, OrderDetailModel>()
				.ForMember(dest => dest.ProductDisplayName, opt => opt.MapFrom(src => src.Product.Name))
				.ForMember(x => x.Product, opt => opt.Ignore());

			CreateMap<AnonymousCart, CartItemModel>()
				.ForMember(x => x.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(x => x.ProductId, opt => opt.MapFrom(src => src.Product.Id))
				.ForMember(x => x.Price, opt => opt.MapFrom(src => src.Product.Price))
				.ForMember(x => x.ProductImage, opt => opt.MapFrom(src => src.Product.ProductImage.FirstOrDefault().ImageUrl))
				.ForMember(x => x.Quantity, opt => opt.MapFrom(src => src.Quantity))
				.ForMember(x => x.Discount, opt => opt.MapFrom(src => src.Product.Discount))
				.ForMember(x => x.ProductName, opt => opt.MapFrom(src => src.Product.Name));

			CreateMap<Cart, CartItemModel>()
				.ForMember(x => x.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(x => x.ProductId, opt => opt.MapFrom(src => src.Product.Id))
				.ForMember(x => x.Price, opt => opt.MapFrom(src => src.Product.Price))
				.ForMember(x => x.ProductImage, opt => opt.MapFrom(src => src.Product.ProductImage.FirstOrDefault().ImageUrl))
				.ForMember(x => x.Quantity, opt => opt.MapFrom(src => src.Quantity))
				.ForMember(x => x.Discount, opt => opt.MapFrom(src => src.Product.Discount))
				.ForMember(x => x.ProductName, opt => opt.MapFrom(src => src.Product.Name));

			CreateMap<List<CartItemModel>, ShoppingCartModel>()
				.ForMember(x => x.TotalItems, opt => opt.MapFrom(src => src.Count))
				.ForMember(x => x.Items, opt => opt.MapFrom(src => src))
				.ForMember(x => x.TotalPrice, opt => opt.MapFrom(src => src.Sum(x => x.Price * x.Quantity * (1 - x.Discount / 100))));
		}
	}
}
