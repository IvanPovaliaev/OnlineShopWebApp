using AutoMapper;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Application.Services
{
	public class CartsService : ICartsService
	{
		private readonly ICartsRepository _cartsRepository;
		private readonly IMapper _mapper;
		private readonly IProductsService _productsService;

		public CartsService(ICartsRepository cartsRepository, IMapper mapper, IProductsService productsService)
		{
			_cartsRepository = cartsRepository;
			_mapper = mapper;
			_productsService = productsService;
		}

		public async Task<Cart> GetAsync(string userId) => await _cartsRepository.GetAsync(userId);

		public async Task<CartViewModel> GetViewModelAsync(string userId)
		{
			var cartDb = await GetAsync(userId);
			return _mapper.Map<CartViewModel>(cartDb);
		}

		public async Task<bool> AddAsync(Guid productId, string userId)
		{
			var cart = await _cartsRepository.GetAsync(userId);

			if (cart is null)
			{
				return await CreateAsync(productId, userId);
			}

			var position = cart.Positions.FirstOrDefault(position => position.Product.Id == productId);

			if (position is null)
			{
				var isPositionAdded = await AddPositionAsync(cart, productId);

				return isPositionAdded && await _cartsRepository.UpdateAsync(cart);
			}

			return await IncreasePositionAsync(userId, position.Id);
		}

		public async Task<bool> IncreasePositionAsync(string userId, Guid positionId)
		{
			var cart = await _cartsRepository.GetAsync(userId);

			var position = cart?.Positions.FirstOrDefault(pos => pos.Id == positionId);

			if (position is null)
			{
				return false;
			}

			position.Quantity++;

			return await _cartsRepository.UpdateAsync(cart!);
		}

		public async Task<bool> DecreasePositionAsync(string userId, Guid positionId)
		{
			var cart = await _cartsRepository.GetAsync(userId);

			var position = cart?.Positions.FirstOrDefault(pos => pos.Id == positionId);
			if (position is null)
			{
				return false;
			}

			if (position.Quantity == 1)
			{

				return await DeletePositionAsync(userId, positionId); ;
			}

			position.Quantity--;

			return await _cartsRepository.UpdateAsync(cart!);
		}

		public async Task<bool> DeleteAsync(string userId) => await _cartsRepository.DeleteAsync(userId);

		public async Task<bool> DeletePositionAsync(string userId, Guid positionId)
		{
			var cart = await _cartsRepository.GetAsync(userId);
			var position = cart?.Positions.FirstOrDefault(pos => pos.Id == positionId);

			if (position is null)
			{
				return false;
			}

			cart!.Positions.Remove(position);
			return await _cartsRepository.UpdateAsync(cart);
		}

		public async Task ReplaceFromCookieAsync(CartViewModel cookieCart, string userId)
		{
			if (cookieCart is null || cookieCart.Positions.Count == 0)
			{
				return;
			}

			var cart = await _cartsRepository.GetAsync(userId);

			cart ??= new Cart()
			{
				UserId = userId
			};

			cart.Positions.Clear();

			foreach (var position in cookieCart.Positions)
			{
				var productDb = await _productsService.GetAsync(position.Product.Id);
				var newCartPosition = new CartPosition()
				{
					Product = productDb,
					Quantity = position.Quantity,
					CartId = cart.Id
				};

				cart.Positions.Add(newCartPosition);
			}

			if (cart.Id == new Guid())
			{
				await _cartsRepository.CreateAsync(cart);
				return;
			}

			await _cartsRepository.UpdateAsync(cart);
		}

		/// <summary>
		/// Create a new cart for related user.
		/// </summary>        
		/// <param name="productId">product Id (GUID)</param>
		/// <param name="userId">GUID user id</param>
		private async Task<bool> CreateAsync(Guid productId, string userId)
		{
			var cart = new Cart()
			{
				UserId = userId
			};

			var result = await AddPositionAsync(cart, productId);

			if (!result)
			{
				return false;
			}

			await _cartsRepository.CreateAsync(cart);
			return true;
		}

		/// <summary>
		/// Add new product position to cart.
		/// </summary>        
		/// <param name="cart">Cart with products</param>
		/// <param name="productId">product Id (GUID)</param>
		private async Task<bool> AddPositionAsync(Cart cart, Guid productId)
		{
			var product = await _productsService.GetAsync(productId);

			if (product is null)
			{
				return false;
			}

			var position = new CartPosition()
			{
				Product = product,
				Quantity = 1,
				CartId = cart.Id
			};

			cart.Positions.Add(position);
			return true;
		}
	}
}
