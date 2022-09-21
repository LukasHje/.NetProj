using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Models
{
    public class Picture
    {
        public int PictureId { get; set; }
        public String PictureName { get; set; }
        public int ErrandId { get; set; }
    }
}
