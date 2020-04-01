using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Models.Owners;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Views.Settings
{
    public class OwnerCreatorModel
    {
        public List<AssetModel> AssetModels { get; set; }

        public Owner Owner { get; set; }

        public List<Asset> OwnedAssets { get; set; }

        public bool HasOwner
        {
            get
            {
                return Owner != null;
            }
        }
    }
}
