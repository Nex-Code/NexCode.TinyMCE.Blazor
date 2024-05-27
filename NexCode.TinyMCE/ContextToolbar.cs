using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexCode.TinyMCE.Blazor
{
    public class ContextToolbar : ItemSpec
    {
        public Func<object, bool> Predicate { get; init; }

        public string Items { get; init; }

        public Scopes Scope { get; init; } = Scopes.Node;


        public Positions Position { get; init; } = Positions.Line;


        public enum Scopes
        {
            Node,
            Editor
        }

        public enum Positions
        {
            Selection,
            Node,
            Line
        }

        public override string Type => "contexttoolbar";
    }


    public class ContextMenu
    {
        public Func<object, IEnumerable<string>>? Show { get; init; }
    }
    

    public class ContextForm : ItemSpec
    {
        public Func<object, IEnumerable<string>>? Show { get; init; }
        public override string Type => "contextform";
    }
}
