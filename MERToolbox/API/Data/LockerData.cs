using System;
using System.Collections.Generic;
using System.Text.Json;
using Interactables.Interobjects.DoorUtils;
using MERToolbox.API.Helpers;
using ProjectMER.Features.Enums;
using UnityEngine;

namespace MERToolbox.API.Data
{
    public class LockerData
    {
        public class ChamberItemData
        {
            public ItemType Item { get; set; }
            public int Amount { get; set; }
            public int Chance { get; set; }
        }

        public class ChamberData
        {
            public int Chamber { get; set; }
            public List<ChamberItemData> ItemData { get; set; }
            public DoorPermissionFlags Permissions { get; set; }
        }

        public string FileName { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public int ObjectId { get; set; }
        public int ParentId { get; set; }
        public GameObject GameObject { get; set; }

        public List<ChamberData> Chambers { get; set; }
        public MERTLockerType LockerType { get; set; }
        public int Chance { get; set; }

        public void DeserializeProperties(Dictionary<string, object> properties) =>
            TryDeserializeProperties(properties);

        public bool TryDeserializeProperties(Dictionary<string, object> properties)
        {
            if (properties == null)
                return false;

            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true
            };

            if (properties.TryGetValue("SpawnChance", out object chanceObj) && chanceObj is JsonElement chanceElement)
            {
                this.Chance = chanceElement.GetInt32();
            }      

            if (properties.TryGetValue("LockerChambers", out object lockerChambersObj) && lockerChambersObj is JsonElement lockerChambersElement)
            {
                List<ChamberData> chambers = [];
                if (lockerChambersElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement chEl in lockerChambersElement.EnumerateArray())
                    {
                        ChamberData chamber = ParseChamber(chEl);
                        chambers.Add(chamber);
                    }
                }
                
                else if (lockerChambersElement.ValueKind == JsonValueKind.Object)
                {
                    chambers.Add(ParseChamber(lockerChambersElement));
                }

                this.Chambers = chambers;
                LogManager.Debug($"Deserialized {this.Chambers.Count} chambers");
            }
            else
            {
                LogManager.Warn("LockerChambers property not found");
                return false;
            }

            if (properties.TryGetValue("LockerType", out object lockerTypeObj))
            {
                try
                {
                    int typeInt = 0;
                    if (lockerTypeObj is JsonElement lockerTypeEl && lockerTypeEl.ValueKind != JsonValueKind.Null)
                    {
                        typeInt = lockerTypeEl.ValueKind == JsonValueKind.Number ? lockerTypeEl.GetInt32() : int.TryParse(lockerTypeEl.GetString(), out int v) ? v : 0;
                    }
                    else
                    {
                        typeInt = Convert.ToInt32(lockerTypeObj);
                    }

                    this.LockerType = (MERTLockerType)typeInt;
                    LogManager.Debug($"Deserialized LockerType: {this.LockerType}");
                }
                catch (Exception ex)
                {
                    LogManager.Warn($"Failed to parse LockerType: {ex}");
                    return false;
                }
            }
            else
            {
                LogManager.Warn("LockerType property not found");
                return false;
            }

            return true;

            ChamberData ParseChamber(JsonElement elem)
            {
                ChamberData chamber = new()
                {
                    ItemData = []
                };

                if (elem.TryGetProperty("Chamber", out JsonElement chEl) && chEl.ValueKind != JsonValueKind.Null)
                {
                    chamber.Chamber = chEl.GetInt32();
                }

                if (elem.TryGetProperty("Permissions", out JsonElement permEl) && permEl.ValueKind != JsonValueKind.Null)
                {
                    int permVal = 0;
                    if (permEl.ValueKind == JsonValueKind.Number) 
                    {
                        permVal = permEl.GetInt32();
                    }
                    else 
                    {
                        int.TryParse(permEl.GetString(), out permVal);
                    }

                    chamber.Permissions = (DoorPermissionFlags)permVal;
                }

                if (elem.TryGetProperty("ItemData", out JsonElement itemDataEl) && itemDataEl.ValueKind != JsonValueKind.Null)
                {
                    void AddItem(JsonElement itemEl)
                    {
                        ChamberItemData item = new();
                        if (itemEl.TryGetProperty("Item", out JsonElement itemVal) && itemVal.ValueKind != JsonValueKind.Null)
                        {
                            if (itemVal.ValueKind == JsonValueKind.Number)
                            {
                                item.Item = (ItemType)itemVal.GetInt32();
                            }
                            else if (int.TryParse(itemVal.GetString(), out int i))
                            {
                                item.Item = (ItemType)i;
                            }
                        }

                        if (itemEl.TryGetProperty("Amount", out JsonElement amtEl) && amtEl.ValueKind != JsonValueKind.Null)
                        {
                            if (amtEl.ValueKind == JsonValueKind.Number)
                            {
                                item.Amount = amtEl.GetInt32();
                            } 
                            else
                            {
                                int.TryParse(amtEl.GetString(), out int a);
                                item.Amount = a;
                            }
                        }

                        if (itemEl.TryGetProperty("Chance", out JsonElement chEl2) && chEl2.ValueKind != JsonValueKind.Null)
                        {
                            if (chEl2.ValueKind == JsonValueKind.Number) 
                            {
                                item.Chance = chEl2.GetInt32();
                            }
                            else
                            {
                                int.TryParse(chEl2.GetString(), out int c);
                                item.Chance = c;
                            }
                        }

                        chamber.ItemData.Add(item);
                    }

                    if (itemDataEl.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement it in itemDataEl.EnumerateArray()) 
                            AddItem(it);
                    }
                    else if (itemDataEl.ValueKind == JsonValueKind.Object)
                    {
                        AddItem(itemDataEl);
                    }
                }

                return chamber;
            }
        }
    }
}