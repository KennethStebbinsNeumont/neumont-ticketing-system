﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Models.Owners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Views.Settings
{
    public class AssetManagerModel : PageModel
    {
        public List<AssetModel> AssetModels { get; private set; }
        public List<Asset> Assets { get; private set; }
        public List<Owner> Owners { get; private set; }

        public AssetManagerModel(List<AssetModel> assetModels,
            List<Asset> assets,
            List<Owner> owners)
        {
            AssetModels = assetModels;
            Assets = assets;
            Owners = owners;
        }


    }
}
