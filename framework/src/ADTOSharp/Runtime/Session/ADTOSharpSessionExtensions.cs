using System;

namespace ADTOSharp.Runtime.Session
{
    /// <summary>
    /// Extension methods for <see cref="IADTOSharpSession"/>.
    /// </summary>
    public static class ADTOSharpSessionExtensions
    {
        /// <summary>
        /// Gets current User's Id.
        /// Throws <see cref="ADTOSharpException"/> if <see cref="IADTOSharpSession.UserId"/> is null.
        /// </summary>
        /// <param name="session">Session object.</param>
        /// <returns>Current User's Id.</returns>
        public static Guid GetUserId(this IADTOSharpSession session)
        {
            if (!session.UserId.HasValue)
            {
                throw new ADTOSharpException("Session.UserId is null! Probably, user is not logged in.");
            }

            return session.UserId.Value;
        }

        /// <summary>
        /// Gets current Tenant's Id.
        /// Throws <see cref="ADTOSharpException"/> if <see cref="IADTOSharpSession.TenantId"/> is null.
        /// </summary>
        /// <param name="session">Session object.</param>
        /// <returns>Current Tenant's Id.</returns>
        /// <exception cref="ADTOSharpException"></exception>
        public static Guid GetTenantId(this IADTOSharpSession session)
        {
            if (!session.TenantId.HasValue)
            {
                throw new ADTOSharpException("Session.TenantId is null! Possible problems: No user logged in or current logged in user in a host user (TenantId is always null for host users).");
            }

            return session.TenantId.Value;
        }

        /// <summary>
        /// Creates <see cref="UserIdentifier"/> from given session.
        /// Returns null if <see cref="IADTOSharpSession.UserId"/> is null.
        /// </summary>
        /// <param name="session">The session.</param>
        public static UserIdentifier ToUserIdentifier(this IADTOSharpSession session)
        {
            return session.UserId == null
                ? null
                : new UserIdentifier(session.TenantId, session.GetUserId());
        }
    }
}