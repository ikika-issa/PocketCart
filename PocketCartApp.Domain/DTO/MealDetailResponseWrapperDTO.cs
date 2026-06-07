using RecipeLibrary.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.DTO
{
    public class MealDetailResponseWrapperDTO
    {
        [JsonPropertyName("meals")]
        public List<MealDetailDTO>? Meals { get; set; }
    }
}
