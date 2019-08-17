using System.ComponentModel.DataAnnotations;

namespace MyLeasing.Web.Data.Entities
{
    public class PropertyImage
    {
        public int Id { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        // TODO: Change the path when publish
        public string ImageFullPath => string.IsNullOrEmpty(ImageUrl) 
            ? null 
            : $"https://TBD.azurewebsites.net{ImageUrl.Substring(1)}";

        public Property Property { get; set; }
    }
}
