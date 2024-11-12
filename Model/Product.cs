﻿using System.Text.Json.Serialization;

namespace YogeshFurnitureAPI.Model
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string ? ImageUrl { get; set; }
        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category? Category { get; set; }
    }
}
