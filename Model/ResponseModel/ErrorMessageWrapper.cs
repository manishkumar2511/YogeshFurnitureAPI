using System.ComponentModel.DataAnnotations.Schema;

namespace YogeshFurnitureAPI.Model.ResponseModel
{
    public class ErrorMessageWrapper
    {
        [NotMapped]
        public bool? HasUpdated { get; set; }

        [NotMapped]
        public int? ErrorCode { get; set; }
        [NotMapped]
        public string? ErrorMessage { get; set; }
    }
}
