﻿using System;
using System.Collections.Generic;

namespace OnlineShop.Db.Models
{
    public class Cart
    {
        public Guid Id { get; init; }
        public string UserId { get; set; }
        public List<CartPosition> Positions { get; set; } = [];
    }
}
