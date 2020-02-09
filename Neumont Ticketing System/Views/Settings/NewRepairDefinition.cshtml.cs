﻿using Neumont_Ticketing_System.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Views.Settings
{
    public class NewRepairDefinitionModel
    {
        public List<AssetType> AssetTypes { get; private set; }
        public List<AssetManufacturer> AssetManufacturers { get; private set; }
        public List<AssetModel> AssetModels { get; private set; }

        public NewRepairDefinitionModel(List<AssetType> assetTypes,
            List<AssetManufacturer> assetManufacturers,
            List<AssetModel> assetModels)
        {
            AssetTypes = assetTypes;
            AssetManufacturers = assetManufacturers;
            AssetModels = assetModels;
        }
    }
}
