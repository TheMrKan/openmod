﻿using System.Threading.Tasks;

namespace OpenMod.API.Permissions
{
    public interface IPermissionCheckProvider
    {
        /// <summary>
        ///     Defines if the given actor is supported by this provider.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <returns><b>true</b> if the given actor is supported; otherwise, <b>false</b>.</returns>
        bool SupportsActor(object actor);

        /// <summary>
        ///     Check if an actor has permission to execute an action.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <param name="permission">The permission to check.</param>
        /// <returns>
        ///     <see cref="PermissionGrantResult.Grant" /> if the actor explicitly has the permission,
        ///     <see cref="PermissionGrantResult.Deny" /> if the actor explicitly does not have the permission; otherwise,
        ///     <see cref="PermissionGrantResult.Default" />
        /// </returns>
        Task<PermissionGrantResult> CheckPermissionAsync(IPermissionActor actor, string permission);
    }
}