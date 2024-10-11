﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaApi.Models.Dto
{
    public class VillaDto
    {
        public int Id { get; set; }
        [Required] //this makes sure the field is required 
        [MaxLength(30)] //self explantory
        public string Name { get; set; }

        public int Occupancy { get; set; }

        public int Sqft { get; set; }
    }
}
