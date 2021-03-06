﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SWGEmuAPI.Model.Structure;
using SWGEmuAPI.Model.Inventory;
using SWGEmuAPI.Model;

namespace SWGEmuAPI.Model
{
    public class StructureTransformModel  : IStructureTransformModel
    {
        public IStringDetailsModel StringDetails { get; set; }
        public IInventoryItemTransformModel InventoryTransform { get; set; }

        public StructureItemDetails TransformStructureDetails(swgemurpcserver.rpc.SWGEmuStructureItemDetails details)
        {
            StructureItemDetails structItem = null;
            if (details.HasBuildingDetails)
            {
                structItem = new BuildingStructureItemDetails()
                {
                    contained_items = details.BuildingDetails.ContainedItemsList.ToList()
                        .ConvertAll<object>(cur => InventoryTransform.TransformInventoryItem(cur)).ToList()
                };
            }
            else if (details.HasInstallationDetails)
            {
                InstallationStructureItemDetails instDetails = null;
                if (details.InstallationDetails.HasFactoryDetails)
                {
                    instDetails = new FactoryInstallationItemDeatils()
                    {
                        shcematic = InventoryTransform.TransformInventoryItem(details.InstallationDetails.FactoryDetails.Schematic),
                        ingredient_items = details.InstallationDetails.FactoryDetails.IngredientItemsList.ToList()
                            .ConvertAll<object>(cur => InventoryTransform.TransformInventoryItem(cur)).ToList()
                    };
                }
                else if (details.InstallationDetails.HasHarvesterDetails)
                {
                    instDetails = new HarvesterInstallationItemDetails()
                    {
                         extraction_rate = details.InstallationDetails.HarvesterDetails.ExtractionRate
                    };
                }

                if (instDetails == null)
                {
                    instDetails = new InstallationStructureItemDetails();
                }

                instDetails.actual_rate = details.InstallationDetails.ActualRate;
                instDetails.max_hopper_size = details.InstallationDetails.MaxHopperSize;
                instDetails.actual_rate = details.InstallationDetails.ActualRate;
                instDetails.hopper_items = details.InstallationDetails.HopperItemsList.ToList()
                    .ConvertAll<object>(cur => InventoryTransform.TransformInventoryItem(cur)).ToList();

                structItem = instDetails;
            }

            if (structItem == null)
            {
                structItem = new StructureItemDetails();
            }
            structItem.owner_account_id = details.OwnerAccountId;
            structItem.owner_object_guid = details.OwnerObjectId;
            structItem.portals_file_name = details.PortalsFileName;
            structItem.template_file_name = details.TemplateFileName;
            structItem.appearance_file_name = details.AppearanceFileName;
            structItem.decay_percent = details.DecayPercent;
            structItem.display_name = details.DisplayName;
            structItem.lot_size = details.LotSize;
            structItem.maintenance = details.Maintenance;
            structItem.object_id = details.ObjectId;
            structItem.object_name = details.ObjectName;
            structItem.owner_display_name = details.OwnerDisplayName;
            structItem.power = details.Power;
            structItem.world_x = details.WorldX;
            structItem.world_y = details.WorldY;
            structItem.world_z = details.WorldZ;
            structItem.zone = details.Zone;

            if (StringDetails != null)
            {
                structItem.resolved_object_name = StringDetails.Get(details.ObjectName);
            }

            return structItem;
        }

        public StructureItem TransformStructure(swgemurpcserver.rpc.SWGEmuCharacterStructureItem charStructItem)
        {
            var item = new StructureItem()
            {
                display_name = charStructItem.DisplayName,
                object_id = charStructItem.ObjectId,
                object_name = charStructItem.ObjectName
            };

            if (StringDetails != null)
            {
                item.resolved_object_name = StringDetails.Get(charStructItem.ObjectName);
            }

            return item;
        }
    }
}