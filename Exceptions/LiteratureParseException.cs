using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when parsing literature type fails.
    /// </summary>
    internal class LiteratureParseException(String message) : Exception(message)
    {
        // Primary constructor - оголошується біля самого класу
    }
}
