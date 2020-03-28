using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Neumont_Ticketing_System.Models.Assets;

namespace Neumont_Ticketing_System.Views.Settings
{
    public class OwnerManagerModel : PageModel
    {
        public List<AssetModel> AssetModels { get; set; }

        public OwnerManagerModel(List<AssetModel> assetModels)
        {
            AssetModels = assetModels;
        }
    }
}
