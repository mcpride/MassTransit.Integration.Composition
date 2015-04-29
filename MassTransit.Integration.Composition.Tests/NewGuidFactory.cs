using System;
using System.ComponentModel.Composition;

namespace MassTransit.Integration.Composition.Tests
{

    /// <summary>
    /// Just a dummy helper for telling MEF that constructors with a Guid as parameter are handable.
    /// </summary>
    public class NewGuidFactory
    {
        [Export(typeof(Guid))]
        public Guid NewGuid
        {
            get
            {
                return new Guid();
            }
        }
    }
}
