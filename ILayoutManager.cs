using System.Collections.Generic;

namespace FrbUi
{
    public interface ILayoutManager : ILayoutable
    {
        IEnumerable<ILayoutable> Items { get; }
    }
}
