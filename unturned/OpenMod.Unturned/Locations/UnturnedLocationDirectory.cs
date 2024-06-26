﻿using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MoreLinq;

namespace OpenMod.Unturned.Locations
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class UnturnedLocationDirectory : IUnturnedLocationDirectory
    {
        public IReadOnlyCollection<UnturnedLocation> GetLocations()
        {
            return LocationDevkitNodeSystem.Get().GetAllNodes().Select(x => new UnturnedLocation(x)).ToArray();
        }

        public UnturnedLocation? FindLocation(string name, bool exact = true)
        {
            if (exact)
            {
                return GetLocations()
                    .FirstOrDefault(location => location.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

            return GetLocations()
                .Where(location => location.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                .Minima(location => StringHelper.LevenshteinDistance(name, location.Name))
                .FirstOrDefault();
        }

        public UnturnedLocation? GetNearestLocation(Vector3 position)
        {
            return GetLocations()
                .Minima(location => (location.Position - position).LengthSquared())
                .FirstOrDefault();
        }
    }
}
