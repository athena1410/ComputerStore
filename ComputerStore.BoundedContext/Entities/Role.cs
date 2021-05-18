//-----------------------------------------------------------------------
// <copyright file="Role.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace ComputerStore.BoundedContext.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
