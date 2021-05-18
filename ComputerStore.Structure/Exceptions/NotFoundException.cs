//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System;

namespace ComputerStore.Structure.Exceptions
{
    //Throw when system not found object
    [Serializable]
    public class NotFoundException : Exception
    {
        public NotFoundException() : base() { }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
    }
}
