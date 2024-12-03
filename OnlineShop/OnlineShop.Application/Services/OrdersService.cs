using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OnlineShop.Application.Helpers.Specifications;
using OnlineShop.Application.Interfaces;
using OnlineShop.Application.Models;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Application.Services
{
	public class OrdersService : IOrdersService
	{
		private readonly IOrdersRepository _ordersRepository;
		private readonly IExcelService _excelService;
		private readonly IMapper _mapper;

		public OrdersService(IOrdersRepository ordersRepository, IExcelService excelService, IMapper mapper)
		{
			_ordersRepository = ordersRepository;
			_excelService = excelService;
			_mapper = mapper;
		}

		public async Task<List<OrderViewModel>> GetAllAsync()
		{
			var orders = await _ordersRepository.GetAllAsync();
			return orders.Select(_mapper.Map<OrderViewModel>)
						 .ToList();
		}

		public async Task<List<OrderViewModel>> GetAllAsync(string userId)
		{
			var specification = new OrdersByUserIdSpecification(userId);

			var orders = await _ordersRepository.GetAllAsync(specification);
			return orders.Select(_mapper.Map<OrderViewModel>)
						 .ToList();
		}

		public async Task<OrderViewModel> GetLastAsync(string userId)
		{
			var orders = await GetAllAsync(userId);
			return orders.OrderByDescending(order => order.CreationDate)
						 .FirstOrDefault()!;
		}

		public async Task<Guid?> CreateAsync(string userId, UserDeliveryInfoViewModel deliveryInfo, List<CartPosition> positions)
		{
			var deliveryInfoDb = _mapper.Map<UserDeliveryInfo>(deliveryInfo);
			var order = new Order
			{
				UserId = userId,
				Info = deliveryInfoDb
			};

			order.Positions = positions.Select(p => new OrderPosition()
			{
				Product = p.Product,
				Quantity = p.Quantity,
				OrderId = order.Id
			}).ToList();

			return await _ordersRepository.CreateAsync(order);
		}

		public bool IsCreationValid(ModelStateDictionary modelState, List<CartPosition>? positions)
		{
			if (positions is null || positions.Count == 0)
			{
				modelState.AddModelError(string.Empty, "В заказе отсутствуют товары");
			}

			return modelState.IsValid;
		}

		public async Task<bool> UpdateStatusAsync(Guid id, OrderStatusViewModel newStatus)
		{
			return await _ordersRepository.UpdateStatusAsync(id, (OrderStatus)newStatus);
		}

		public async Task<MemoryStream> ExportAllToExcelAsync()
		{
			var orders = await GetAllAsync();
			return _excelService.ExportOrders(orders);
		}
	}
}
