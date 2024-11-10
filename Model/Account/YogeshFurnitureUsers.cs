using Microsoft.AspNetCore.Identity;

namespace YogeshFurnitureAPI.Model.Account
{
    public class YogeshFurnitureUsers : IdentityUser
    {
        public string? Name { get; set; }
        
        public string? Pincode { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Landmark { get; set; }
        public string? Address { get; set; }
       
        public string? ProfileImage { get; set; } 

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastModifyDate { get; set; }
    }

}
