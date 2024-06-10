using FIS_API.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FIS_API.Dtos.Outbound
{
    public class InterventionInDto
	{
		[Required(ErrorMessage = "Location is required")]
        [RegularExpression(@"^[0-9a-zA-Z,._ \-]+$")] // basic attack protection, note that "\-" is an escape sequence and therefore only includes the "-"
        public string Location { get; set;}

		[Required(ErrorMessage = "Intervention ID is required")]
		public int InterventionTypeId { get; set; }
    }
}
