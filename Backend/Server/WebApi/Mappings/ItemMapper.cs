using Core.Entities;
using Core.Enums;
using System.Diagnostics.Metrics;
using System.Net;
using WebApi.Controllers;
using WebApi.Dtos;
using static WebApi.Dtos.ItemDto;

namespace WebApi.Mappings
{
    public static class ItemMapper
    {
        public static Item ToEntity(this ItemPostDto itemToPost)
        {
            return new Item
            {
                CreatedAt = DateTime.UtcNow,
                Name = itemToPost.Name ?? string.Empty,
                Description = itemToPost.Description ?? string.Empty,
                IsAvailable = itemToPost.IsAvailable,
                Address = itemToPost.Address ?? string.Empty,
                Price = itemToPost.Price,
                Stock = itemToPost.Stock,
                Deposit = itemToPost.Deposit,
                RentalType = itemToPost.RentalType, // Standart Unknown
                ItemCondition = itemToPost.ItemCondition, // Standart Unknown
                SubCategoryId = itemToPost.SubCategoryId,
                GeoPostalId = itemToPost.GeoPostaldId,
                OwnerId = itemToPost.OwnerId
            };
        }


        public static void UpdateEntity(this ItemPutDto itemDto, Item itemToPut)
        {
            itemToPut.UpdatedAt = DateTime.UtcNow;
            itemToPut.RowVersion = itemDto.RowVersion;
            itemToPut.Name = itemDto.Name ?? string.Empty;
            itemToPut.Description = itemDto.Description ?? string.Empty;
            itemToPut.IsAvailable = itemDto.IsAvailable;
            itemToPut.Address = itemDto.Address ?? string.Empty;
            itemToPut.Price = itemDto.Price;
            itemToPut.Stock = itemDto.Stock;
            itemToPut.Deposit = itemDto.Deposit;
            itemToPut.RentalType = itemDto.RentalType; // Standart Unknown
            itemToPut.ItemCondition = itemDto.ItemCondition; // Standart Unknown
            itemToPut.SubCategoryId = itemDto.SubCategoryId;
            itemToPut.GeoPostalId = itemDto.GeoPostaldId;
            itemToPut.OwnerId = itemDto.OwnerId;
        }
    }
}
