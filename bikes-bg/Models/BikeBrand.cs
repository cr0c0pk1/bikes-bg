﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using bikes_bg.Models.Base;

namespace bikes_bg.Models
{
    [Table("BIKE_BRANDS")]
    public class BikeBrand : BaseEntity
    {
        public virtual List<BikeModel> bikeModels  { get; set; }
    }
}
