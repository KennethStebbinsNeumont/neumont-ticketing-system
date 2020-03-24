﻿using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Models.Owners;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Views.Settings
{
    public class AssetCreatorModel
    {
        public List<AssetModel> AssetModels { get; private set; }
        public List<Asset> Assets { get; private set; }
        public List<Owner> Owners { get; private set; }

        public AssetCreatorModel(List<AssetModel> assetModels,
            List<Asset> assets,
            List<Owner> owners)
        {
            AssetModels = assetModels;
            Assets = assets;
            Owners = owners;
        }


    }
}
