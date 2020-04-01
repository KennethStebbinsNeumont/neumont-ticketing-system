using Neumont_Ticketing_System.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Views.Settings
{
    public class LoanerEditorModel
    {
        public LoanerAsset Loaner { get; set; }

        public bool HasLoaner
        {
            get
            {
                return Loaner != null;
            }
        }

        public Asset AssociatedAsset { get; set; }

        public List<AssetModel> AssetModels { get; set; }
    }
}
